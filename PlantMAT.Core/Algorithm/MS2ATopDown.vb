#Region "Microsoft.VisualBasic::9df25aaa019db733a848e7b906dcadb3, PlantMAT.Core\Algorithm\MS2ATopDown.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
'       Feng Qiu (fengqiu1982 https://sourceforge.net/u/fengqiu1982/)
' 
' Copyright (c) 2020 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' Apache 2.0 License
' 
' 
' Copyright 2020 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
' 
'     http://www.apache.org/licenses/LICENSE-2.0
' 
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.



' /********************************************************************************/

' Summaries:

'     Class MS2ATopDown
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: IonMatching, MS2Annotation, MS2ATopDown
' 
'         Sub: applySettings, MS2Annotation, MS2AnnotationLoop
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports PlantMAT.Core.Algorithm.InternalCache
Imports PlantMAT.Core.Models
Imports PlantMAT.Core.Models.AnnotationResult

Namespace Algorithm

    ''' <summary>
    ''' This module performs MS2 annotation
    ''' </summary>
    Public Class MS2ATopDown : Inherits PlantMATAlgorithm

        Dim mzPPM As Double
        Dim NoiseFilter As Double

        Sub New(settings As Settings)
            Call MyBase.New(settings)
        End Sub

        Protected Overrides Sub applySettings()
            mzPPM = settings.mzPPM
            NoiseFilter = settings.NoiseFilter
        End Sub

        Public Function MS2Annotation(queries As Query()) As QueryPopulator
            Dim result As Query()

            If queries.All(Function(q) q.Candidates.IsNullOrEmpty) Then
                Console.WriteLine("Please run combinatorial enumeration (MS1) first")
                Return New ArrayPopulator With {.array = queries}
            Else
                Console.WriteLine("Now analyzing ms2 topdown, please wait...")
                result = MS2ATopDown(queries).ToArray
            End If

            Dim cacheFile As String = App.GetAppSysTempFile(".cache", App.PID, "plantmat")

            Console.WriteLine("MS2 annotation finished." & Environment.NewLine & "Continue glycosyl sequencing")
            Console.WriteLine("Now analyzing glycosyl sequencing, please wait...")

            Console.WriteLine($"data cache at location: {cacheFile}.")

            Using writer As New BinaryDataWriter(cacheFile.Open)
                For Each query As Query In New GlycosylSequencing(settings).MS2P(result)
                    Dim json As JsonObject = GetType(Query) _
                        .GetJsonElement(query, New JSONSerializerOptions) _
                        .As(Of JsonObject)

                    Using buffer As MemoryStream = BSON.GetBuffer(json)
                        Call writer.Write(buffer.Length)
                        Call writer.Write(buffer.ToArray)
                    End Using

                    Call json.Dispose()
                    Call Console.WriteLine(query.ToString)
                Next
            End Using

            Console.WriteLine("Glycosyl sequencing finished")
            Console.WriteLine("MS2 annotation finished")

            Return New CacheFilePopulator(cacheFile)
        End Function

        ''' <summary>
        ''' Perform the MS2 annotation and display the results
        ''' </summary>
        ''' <param name="queries"></param>
        ''' <returns></returns>
        Private Function MS2ATopDown(queries As IEnumerable(Of Query)) As IEnumerable(Of Query)
            Return queries _
                .AsParallel _
                .WithDegreeOfParallelism(PublicVSCode.Parallelism) _
                .Select(AddressOf MS2Annotation)
        End Function

        ''' <summary>
        ''' Loop through all candidates for each compound
        ''' </summary>
        ''' <param name="query"></param>
        Private Function MS2Annotation(query As Query) As Query
            For i As Integer = 0 To query.Candidates.Count - 1
                If Not query.Ms2Peaks Is Nothing Then
                    ' Read compound serial number and precuror ion mz
                    Dim IonMZ_crc As Double
                    Dim Rsyb As String
                    Dim precursorIonType As String = query(i).precursor_type

                    ' Find the ion type (pos or neg) based on the setting
                    ' 在二级离子推断注释这里，离子化模式似乎是固定类型的
                    If Right(precursorIonType, 1) = "-" Then
                        IonMZ_crc = e_w - H_w
                        Rsyb = "-H]-"
                    Else
                        IonMZ_crc = H_w - e_w
                        Rsyb = "+H]+"
                    End If

                    Call MS2AnnotationLoop(query, IonMZ_crc, Rsyb, i)
                End If
            Next

            Return query
        End Function

        Private Sub MS2AnnotationLoop(query As Query, IonMZ_crc As Double, Rsyb As String, i As Integer)
            Dim candidate As CandidateResult = query(i)

            ' Read the results from combinatorial enumeration
            Dim AglyN = candidate.Name
            Dim Agly_w = candidate.ExactMass
            Dim Hex_max As Integer = CInt(candidate.Hex)
            Dim HexA_max As Integer = CInt(candidate.HexA)
            Dim dHex_max As Integer = CInt(candidate.dHex)
            Dim Pen_max As Integer = CInt(candidate.Pen)
            Dim Mal_max As Integer = CInt(candidate.Mal)
            Dim Cou_max As Integer = CInt(candidate.Cou)
            Dim Fer_max As Integer = CInt(candidate.Fer)
            Dim Sin_max As Integer = CInt(candidate.Sin)
            Dim DDMP_max As Integer = CInt(candidate.DDMP)

            ' First, predict the ions based on the results from combinatorial enumeration
            Dim prediction As New NeutralLossIonPrediction(AglyN$, Agly_w#, IonMZ_crc, Rsyb) With {
                .Hex_max = Hex_max,
                .HexA_max = HexA_max,
                .dHex_max = dHex_max,
                .Pen_max = Pen_max,
                .Mal_max = Mal_max,
                .Cou_max = Cou_max,
                .Fer_max = Fer_max,
                .Sin_max = Sin_max,
                .DDMP_max = DDMP_max
            }
            Dim pIonList As MzAnnotation() = Nothing

            Call prediction.IonPrediction()
            Call prediction.getResult(pIonList)

            ' Second, compare the predicted ions with the measured
            Dim aIonList As New List(Of IonAnnotation)
            Dim AglyCheck As Boolean = IonMatching(query.Ms2Peaks, pIonList, aIonList)

            ' Fifth, show an asterisk mark if the ions corresponding to the aglycone are found
            candidate.Ms2Anno = New Ms2IonAnnotations With {
                .ions = aIonList.PopAll,
                .aglycone = AglyCheck
            }
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="eIonList">the ms2 matrix data from the raw sample</param>
        ''' <param name="pIonList">product mz ion predicts</param>
        ''' <returns>AglyCheck</returns>
        Private Function IonMatching(eIonList As Ms2Peaks, pIonList As MzAnnotation(), ByRef aIonList As List(Of IonAnnotation)) As Boolean
            ' Initialize the annotated ion list aIonList() to none
            Dim AglyCheck = False
            Dim eIon_n As Integer = eIonList.fragments
            Dim TotalIonInt As Double = eIonList.TotalIonInt

            ' Compare the measured ions eIonList() with the predicted pIonList()
            ' If the mz error is less than the defined ppm and intensity is above the noise filter, then
            ' save the predicted ions in the annotation ion list aIonList()
            For s As Integer = 0 To eIon_n - 1
                Dim eIonMZ As Double = eIonList.mz(s)
                Dim eIonInt As Double = eIonList.into(s)

                For Each t As MzAnnotation In pIonList
                    Dim pIonMZ As Double = t.productMz
                    Dim pIonNM As String = t.annotation

                    If PPMmethod.PPM(eIonMZ, pIonMZ) <= mzPPM Then
                        Dim aIonAbu = eIonInt / TotalIonInt

                        If aIonAbu * 100 >= NoiseFilter Then
                            Call New IonAnnotation With {
                                .productMz = eIonMZ,
                                .ionAbu = aIonAbu * 100,
                                .annotation = pIonNM
                            }.DoCall(AddressOf aIonList.Add)

                            If Left(pIonNM, 1) = "*" Then
                                AglyCheck = True
                            End If
                        End If
                    End If
                Next
            Next

            Return AglyCheck
        End Function
    End Class
End Namespace
