﻿#Region "Microsoft.VisualBasic::0b33ccf8474b201f8e3496d116c01a07, PlantMAT.Core\Algorithm\GlycosylSequencing.vb"

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

    '     Class GlycosylSequencing
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: IonPredictionMatching, MS2P
    ' 
    '         Sub: applySettings, MS2Prediction
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports PlantMAT.Core.Models
Imports PlantMAT.Core.Models.AnnotationResult

Namespace Algorithm

    Public Class GlycosylSequencing : Inherits PlantMATAlgorithm

        Dim mzPPM As Double

        Sub New(settings As Settings)
            Call MyBase.New(settings)
        End Sub

        Protected Friend Overrides Sub applySettings()
            mzPPM = settings.mzPPM
        End Sub

        Public Function MS2P(queries As IEnumerable(Of Query)) As IEnumerable(Of Query)
            Return queries _
                .AsParallel _
                .WithDegreeOfParallelism(PublicVSCode.Parallelism) _
                .Select(Function(queryClone As Query)
                            ' fix of the reference problem
                            ' do object clone for break reference
                            Dim query As New Query(queryClone)

                            If Not query.Ms2Peaks Is Nothing Then
                                Call MS2Prediction(query)
                            End If

                            Return query
                        End Function)
        End Function

        Private Sub MS2Prediction(query As Query)
            Dim DHIonMZ As Double = query.PrecursorIon

            ' Predict MS2
            For i As Integer = 0 To query.Candidates.Length - 1
                Dim MIonMZ#
                Dim precursor As PrecursorInfo = PublicVSCode.GetPrecursorInfo(query(i).precursor_type)

                If precursor.precursor_type.Last = "-"c Then
                    MIonMZ = ((DHIonMZ - precursor.adduct) / precursor.M) - H_w + e_w
                Else
                    MIonMZ = ((DHIonMZ - precursor.adduct) / precursor.M) + H_w - e_w
                End If

                ' Find how many structural possibilites for each peak in 'SMILES' sheet
                Dim RS As New List(Of GlycosylPredition)
                Dim Pred_n = 0
                Dim Match_n = 0
                Dim Match_m = 0

                ' Predict MS2 [MSPrediction()] for each structural possibility
                For Each GlycN As String In query(i).SMILES.SafeQuery
                    Dim candidate As CandidateResult = query(i)

                    ' For Each smiles As SMILES In candidate.SMILES
                    Pred_n = Pred_n + 1
                    IonPredictionMatching(query.Ms2Peaks, Match_m, GlycN, MIonMZ).DoCall(AddressOf RS.Add)
                Next

                If RS.Count > 0 Then
                    query(i).Glycosyl = New Glycosyl With {
                        .Match_m = Match_m,
                        .Pred_n = Pred_n,
                        .pResult = RS.ToArray,
                        .Match_n = Match_n
                    }
                End If
            Next
        End Sub

        Private Function IonPredictionMatching(eIonList As Ms2Peaks, ByRef Match_m As Integer, GlycN As String, MIonMZ As Double) As GlycosylPredition
            ' 1. Declare variables and assign mass of [M-H2O]
            Dim m(,) As String, u(,) As String, Lt As String
            Dim Loss1 As Double, pIonList(,) As Double
            Dim pIonMZ As Double, eIonMZ As Double, eIonInt As Double

            ReDim m(20, 20), u(1, 100)

            Dim f1(1, 100) As Double, f2(1, 100) As Double
            Dim SugComb As String = ""

            ' 2. Read aglyone/sugar/acid combination and store each component to u()
            Dim Comma_n = 0
            Dim g = 1

            For e As Integer = 1 To Len(GlycN)
                Lt = Mid(GlycN, e, 1)
                If Lt = "," AndAlso Comma_n = 0 Then
                    SugComb = Right(GlycN, Len(GlycN) - e - 1)
                    Comma_n = Comma_n + 1
                End If
                If Lt <> "," Then
                    u(1, g) = u(1, g) + Lt
                Else
                    e = e + 1
                    g = g + 1
                End If
            Next e

            Dim NumComponent = g
            Dim nameComponent As String
            Dim numDash As Integer
            Dim w(Math.Max(g, 6), 100) As Double

            ' 3. Identify each component, calculate mass, and store value to w()
            Lt = ""
            For e = 2 To g
                Dim s = 1
                For h12 = Len(u(1, e)) To 1 Step -1
                    Lt = Mid(u(1, e), h12, 1)
                    If Lt <> "-" Then
                        m(e - 1, s) = Lt + m(e - 1, s)

                        If m(e - 1, s) = "Hex" Then w(e - 1, s) = Hex_w - H2O_w
                        If m(e - 1, s) = "HexA" Then w(e - 1, s) = HexA_w - H2O_w
                        If m(e - 1, s) = "dHex" Then w(e - 1, s) = dHex_w - H2O_w
                        If m(e - 1, s) = "Pen" Then w(e - 1, s) = Pen_w - H2O_w
                        If m(e - 1, s) = "Mal" Then w(e - 1, s) = Mal_w - H2O_w
                        If m(e - 1, s) = "Cou" Then w(e - 1, s) = Cou_w - H2O_w
                        If m(e - 1, s) = "Fer" Then w(e - 1, s) = Fer_w - H2O_w
                        If m(e - 1, s) = "Sin" Then w(e - 1, s) = Sin_w - H2O_w
                        If m(e - 1, s) = "DDMP" Then w(e - 1, s) = DDMP_w - H2O_w
                    Else
                        w(e - 1, s) = w(e - 1, s) + w(e - 1, s - 1)
                        s = s + 1
                    End If
                Next h12
            Next e

            ' 4. Fragment each sugar chain forward (NL = sugar portions);
            ' calualte mass of each fragment (loss), and store value to f1()
            Dim h = 0

            For c1 = 1 To 5
                For c1f = 1 To 100
                    If w(c1, c1f) = 0 Then Exit For
                    h = h + 1
                    f1(1, h) = w(c1, c1f)
                    Loss1 = f1(1, h)
                    For c2 = c1 + 1 To 5
                        For c2f = 1 To 100
                            If w(c2, c2f) = 0 Then Exit For
                            h = h + 1
                            f1(1, h) = Loss1 + w(c2, c2f)
                            Dim Loss2 = f1(1, h)
                            For c3 = c2 + 1 To 5
                                For c3f = 1 To 100
                                    If w(c3, c3f) = 0 Then Exit For
                                    h = h + 1
                                    f1(1, h) = Loss2 + w(c3, c3f)
                                    Dim Loss3 = f1(1, h)
                                    For c4 = c3 + 1 To 5
                                        For c4f = 1 To 100
                                            If w(c4, c4f) = 0 Then Exit For
                                            h = h + 1
                                            f1(1, h) = Loss3 + w(c4, c4f)
                                            Dim Loss4 = f1(1, h)
                                            For c5 = c4 + 1 To 5
                                                For c5f = 1 To 100
                                                    If w(c5, c5f) = 0 Then Exit For
                                                    h = h + 1
                                                    f1(1, h) = Loss4 + w(c5, c5f)
                                                Next c5f
                                            Next c5
                                        Next c4f
                                    Next c4
                                Next c3f
                            Next c3
                        Next c2f
                    Next c2
                Next c1f
            Next c1

            ' 5. Fragment each sugar chain backward (ion = sugar portions);
            ' calualte mass of each fragment (loss), and store value to f1()
            Dim h1 = h + 1

            Dim nameSugar As String = ""
            Dim mass As Double
            Dim f1_temp As Double
            Dim c As String

            For e = 2 To NumComponent
                nameComponent = u(1, e)
                numDash = 0

                For g = Len(nameComponent) To 1 Step -1

                    c = Mid(nameComponent, g, 1)
                    nameSugar = c & nameSugar

                    If c = "-" Then numDash = numDash + 1
                    If nameSugar = "-Hex" Then mass = Hex_w
                    If nameSugar = "-HexA" Then mass = HexA_w
                    If nameSugar = "-dHex" Then mass = dHex_w
                    If nameSugar = "-Pen" Then mass = Pen_w
                    If nameSugar = "-Mal" Then mass = Mal_w
                    If nameSugar = "-Cou" Then mass = Cou_w
                    If nameSugar = "-Fer" Then mass = Fer_w
                    If nameSugar = "-Sin" Then mass = Sin_w
                    If nameSugar = "-DDMP" Then mass = DDMP_w

                    If mass <> 0 Then
                        h = h + 1
                        If numDash = 1 Then f1_temp = mass
                        If numDash = 2 Then f1(1, h) = f1_temp + mass - H2O_w
                        If numDash > 2 Then f1(1, h) = f1(1, h - 1) + mass - H2O_w
                        nameSugar = ""
                        mass = 0
                    End If
                Next g
            Next e

            For h2 = h1 To h
                f1(1, h2) = MIonMZ - f1(1, h2) + H_w - e_w
            Next h2

            h1 = h + 1

            For e = 2 To NumComponent
                nameComponent = u(1, e)
                numDash = 0

                For g = 1 To Len(nameComponent)

                    c = Mid(nameComponent, g, 1)
                    nameSugar = c & nameSugar

                    If c = "-" Then numDash = numDash + 1
                    If nameSugar = "Hex-" Then mass = Hex_w
                    If nameSugar = "HexA-" Then mass = HexA_w
                    If nameSugar = "dHex-" Then mass = dHex_w
                    If nameSugar = "Pen-" Then mass = Pen_w
                    If nameSugar = "Mal-" Then mass = Mal_w
                    If nameSugar = "Cou-" Then mass = Cou_w
                    If nameSugar = "Fer-" Then mass = Fer_w
                    If nameSugar = "Sin-" Then mass = Sin_w
                    If nameSugar = "DDMP-" Then mass = DDMP_w

                    If mass <> 0 Then

                        h = h + 1

                        If numDash = 1 Then f1_temp = mass
                        If numDash = 2 Then f1(1, h) = f1_temp + mass - H2O_w
                        If numDash > 2 Then f1(1, h) = f1(1, h - 1) + mass - H2O_w

                        nameSugar = ""
                        mass = 0
                    End If
                Next g
            Next e

            For h2 = h1 To h
                f1(1, h2) = MIonMZ - f1(1, h2) + H_w - e_w
            Next h2

            ' 6. Remove duplicates (loss with same mass) in array f1() and create a new list to f2()
            g = 1
            f2(1, 1) = f1(1, 1)

            For e As Integer = 1 To h
                For s = 1 To g
                    If Int(f1(1, e)) = Int(f2(1, s)) Then
                        GoTo NextOne
                    End If
                Next s
                g = g + 1
                f2(1, g) = f1(1, e)
NextOne:
            Next e

            ' 7. Create ion list based on possible sugar/acid losses in f2() and store value to pIonList()
            ReDim pIonList(g, 4)

            For e = 1 To g
                h = 1
                For x = 0 To 1
                    For y = 0 To 1
                        If x + y > 2 Then
                            Exit For
                        End If
                        pIonList(e, h) = MIonMZ - f2(1, e) - x * H2O_w - y * CO2_w
                        h = h + 1
                    Next y
                Next x
            Next e

            Dim eIon_n = eIonList.mz.Length
            Dim TotalIonInt As Double = eIonList.TotalIonInt

            ' 8. Compare pIonList() with eIonlist(), calculate raw score, and save result to RS()
            Dim RawScore As Double = 0

            For e = 1 To g
                For h = 1 To 4
                    pIonMZ = pIonList(e, h)
                    For s = 0 To eIon_n - 1
                        eIonMZ = eIonList.mz(s)
                        If PPMmethod.PPM(pIonMZ, eIonMZ) < mzPPM Then
                            eIonInt = eIonList.into(s)
                            RawScore = RawScore + Math.Log10(100000 * eIonInt / TotalIonInt)
                            GoTo NextPriIon
                        End If
                    Next s
                Next h
NextPriIon:
            Next e

            If RawScore > 0 Then
                Match_m = Match_m + 1
            End If

            Return New GlycosylPredition With {
                .score = RawScore,
                .struct = SugComb
            }
        End Function
    End Class
End Namespace
