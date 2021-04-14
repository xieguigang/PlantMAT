#Region "Microsoft.VisualBasic::b8629e28472caa4c6269a675eb77a340, PlantMAT.Core\Algorithm\MS2ATopDown.vb"

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
'         Function: IonMatching, (+2 Overloads) MS2Annotation, MS2ATopDown
' 
'         Sub: applySettings, MS2AnnotationLoop
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Parallel.IpcStream
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
        Dim ionMode As String

        Sub New(settings As Settings, ionMode As Integer)
            Call MyBase.New(settings)

            Me.ionMode = If(ionMode > 0, "+", "-")
        End Sub

        Protected Friend Overrides Sub applySettings()
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

            Dim cacheFile As SocketRef = SocketRef.CreateReference

            Console.WriteLine("MS2 annotation finished." & Environment.NewLine & "Continue glycosyl sequencing")
            Console.WriteLine("Now analyzing glycosyl sequencing, please wait...")

            Console.WriteLine($"data cache at location: {cacheFile}.")

            Using writer As New CacheFileWriter(cacheFile)
                For Each query As Query In New GlycosylSequencing(settings).MS2P(result)
                    Call writer.AddQuery(query)
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
                    Call MS2AnnotationLoop(query, IonMZ_crc.GetIonMZ_crc(ionMode), i)
                End If
            Next

            Return query
        End Function

        Private Sub MS2AnnotationLoop(query As Query, IonMZ_crc As MzAnnotation, i As Integer)
            Dim candidate As CandidateResult = query(i)
            Dim pIonList As MzAnnotation() = Nothing

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
            Using insilicons As New NeutralLossIonPrediction(query.PrecursorIon, AglyN$, Agly_w#, IonMZ_crc, {}, PublicVSCode.GetPrecursorInfo(candidate.precursor_type))

                Call insilicons.SetPredictedMax(
                    Hex_max:=Hex_max,
                    HexA_max:=HexA_max,
                    dHex_max:=dHex_max,
                    Pen_max:=Pen_max,
                    Mal_max:=Mal_max,
                    Cou_max:=Cou_max,
                    Fer_max:=Fer_max,
                    Sin_max:=Sin_max,
                    DDMP_max:=DDMP_max
                )

                Call insilicons.IonPrediction()
                Call insilicons.getResult(pIonList)
            End Using

            ' Second, compare the predicted ions with the measured
            Dim aIonList As New List(Of IonAnnotation)
            Dim AglyCheck As Boolean = IonMatching(query.Ms2Peaks, pIonList, aIonList, Tolerance.PPM(mzPPM), NoiseFilter)

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
        ''' <returns>a logical result value of AglyCheck</returns>
        Public Shared Function IonMatching(eIonList As Ms2Peaks,
                                           pIonList As MzAnnotation(),
                                           ByRef aIonList As List(Of IonAnnotation),
                                           tolerance As Tolerance,
                                           noiseFilter As Double) As Boolean

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

                    If tolerance(eIonMZ, pIonMZ) Then
                        Dim aIonAbu = eIonInt / TotalIonInt

                        If aIonAbu * 100 >= noiseFilter Then
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
