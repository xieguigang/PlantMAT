#Region "Microsoft.VisualBasic::057cab86566ae00e5b2f37f06004326a, PlantMAT.Core\Algorithm\MS2ATopDown.vb"

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

Imports Microsoft.VisualBasic.Language
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

        Public Function MS2Annotation(queries As Query()) As Query()
            Dim result As Query()

            If queries.All(Function(q) q.Candidates.IsNullOrEmpty) Then
                Console.WriteLine("Please run combinatorial enumeration (MS1) first")
                result = queries
            Else
                Console.WriteLine("Now analyzing ms2 topdown, please wait...")
                result = MS2ATopDown(queries).ToArray

                Console.WriteLine("MS2 annotation finished." & vbNewLine & "Continue glycosyl sequencing")
                Console.WriteLine("Now analyzing glycosyl sequencing, please wait...")
                result = New GlycosylSequencing(settings).MS2P(result).ToArray

                Console.WriteLine("Glycosyl sequencing finished")
                Console.WriteLine("MS2 annotation finished")
            End If

            Return result
        End Function

        Private Iterator Function MS2ATopDown(queries As IEnumerable(Of Query)) As IEnumerable(Of Query)
            For Each query As Query In queries
                ' Perform the MS2 annotation and display the results
                Call MS2Annotation(query)

                Yield query
            Next
        End Function

        ''' <summary>
        ''' Loop through all candidates for each compound
        ''' </summary>
        ''' <param name="query"></param>
        Private Sub MS2Annotation(query As Query)
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
        End Sub

        Private Sub MS2AnnotationLoop(query As Query, IonMZ_crc As Double, Rsyb As String, i As Integer)
            Dim candidate As CandidateResult = query.Candidate(i)

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
            Dim prediction As New IonPrediction(AglyN$, Agly_w#, IonMZ_crc, Rsyb) With {
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
            Dim pIon_n As Integer = 0
            Dim pIonList As Object(,) = Nothing

            Call prediction.IonPrediction()
            Call prediction.getResult(pIon_n, pIonList)

            ' Second, compare the predicted ions with the measured
            Dim aIon_n As Integer = 0
            Dim aIonList(0 To 3, 0 To 1) As Object
            Dim AglyCheck As Boolean = IonMatching(query, pIon_n, pIonList, aIon_n, aIonList)

            ' Third, add a dropdown list for each candidate and show the annotation results in the list
            Dim combName = "dd_MS2A_TopDown_" & CStr(i)
            Dim comb As New List(Of String)

            ' Fourth, save the annotation results in the cell
            Dim aResult As String = ""

            If aIon_n > 0 Then
                For s As Integer = 1 To aIon_n
                    Dim aIonMZ = aIonList(1, s)
                    Dim aIonAbu As Double = DirectCast(aIonList(2, s), Double)
                    Dim aIonNM As String = DirectCast(aIonList(3, s), String)

                    comb.Add(CStr(Format(aIonAbu, "0.000")) & " " & aIonNM)
                    aResult = aResult & CStr(Format(aIonMZ, "0.0000")) & ", " & CStr(Format(aIonAbu * 100, "0.00")) & ", " & aIonNM & "; "
                Next s
            End If

            Dim combText = CStr(aIon_n) & " ions annotated"

            candidate.Ms2Anno = New Ms2IonAnnotations With {
                .title = combText,
                .annotations = comb.ToArray,
                .comment = aResult
            }

            ' Fifth, show an asterisk mark if the ions corresponding to the aglycone are found
            If AglyCheck = True Then
                candidate.Ms2Anno.aglycone = True
            End If
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="pIon_n"></param>
        ''' <param name="pIonList"></param>
        ''' <returns>AglyCheck</returns>
        Private Function IonMatching(query As Query, pIon_n%, pIonList As Object(,), ByRef aIon_n As Integer, ByRef aIonList As Object(,)) As Boolean
            ' Initialize the annotated ion list aIonList() to none
            Dim AglyCheck = False
            Dim eIonList = query.Ms2Peaks
            Dim eIon_n = eIonList.mz.Length
            Dim TotalIonInt As Double = eIonList.TotalIonInt

            ' Compare the measured ions eIonList() with the predicted pIonList()
            ' If the mz error is less than the defined ppm and intensity is above the noise filter, then
            ' save the predicted ions in the annotation ion list aIonList()
            For s As Integer = 0 To eIon_n - 1
                Dim eIonMZ As Double = eIonList.mz(s)
                Dim eIonInt As Double = eIonList.into(s)

                For t As Integer = 1 To pIon_n
                    Dim pIonMZ As Double = DirectCast(pIonList(1, t), Double)
                    Dim pIonNM As String = DirectCast(pIonList(2, t), String)

                    If Math.Abs((eIonMZ - pIonMZ) / pIonMZ) * 1000000 <= mzPPM Then
                        Dim aIonAbu = eIonInt / TotalIonInt

                        If aIonAbu * 100 >= NoiseFilter Then
                            aIon_n = aIon_n + 1
                            ReDim Preserve aIonList(0 To 3, 0 To aIon_n)
                            aIonList(1, aIon_n) = eIonMZ
                            aIonList(2, aIon_n) = aIonAbu
                            aIonList(3, aIon_n) = pIonNM

                            If Left(pIonNM, 1) = "*" Then
                                AglyCheck = True
                            End If
                        End If
                    End If
                Next t
            Next s

            Return AglyCheck
        End Function
    End Class
End Namespace
