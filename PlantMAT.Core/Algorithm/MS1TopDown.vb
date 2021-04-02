﻿#Region "Microsoft.VisualBasic::50143b6cc1e4152e44ead2c3a47e0ee5, PlantMAT.Core\Algorithm\MS1TopDown.vb"

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

'     Class MS1TopDown
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: (+2 Overloads) CombinatorialPrediction, DatabaseSearch, MS1CP, PatternPrediction, RestrictionCheck
'                   RunDatabaseSearch, RunMs1Query
' 
'         Sub: applySettings, PatternPredictionLoop
' 
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports PlantMAT.Core.Models
Imports PlantMAT.Core.Models.AnnotationResult
Imports Info = Microsoft.VisualBasic.Information
Imports WorksheetFunction = Microsoft.VisualBasic.Math.VBMath

Namespace Algorithm

    Public Delegate Function IMS1TopDown(query As Query(), library As Library(), settings As Settings, ionMode As Integer) As Query()

    ''' <summary>
    ''' This module performs combinatorial enumeration
    ''' </summary>
    Public Class MS1TopDown : Inherits PlantMATAlgorithm

        Dim library As Library()
        Dim NumHexMin, NumHexMax, NumHexAMin, NumHexAMax, NumdHexMin, NumdHexMax, NumPenMin, NumPenMax, NumMalMin, NumMalMax, NumCouMin, NumCouMax, NumFerMin, NumFerMax, NumSinMin, NumSinMax, NumDDMPMin, NumDDMPMax As Integer
        Dim NumSugarMin, NumSugarMax, NumAcidMin, NumAcidMax As Integer
        Dim AglyconeType As db_AglyconeType = db_AglyconeType.All
        Dim AglyconeSource As db_AglyconeSource = db_AglyconeSource.All
        Dim SearchPPM As Double
        Dim Precursors As PrecursorInfo()

        Public Sub New(library As Library(), settings As Settings)
            MyBase.New(settings)
            Me.library = library
        End Sub

        Protected Overrides Sub applySettings()
            Const min = 0
            Const max = 1

            NumHexMin = settings.NumofSugarHex(min) : NumHexMax = settings.NumofSugarHex(max)
            NumHexAMin = settings.NumofSugarHexA(min) : NumHexAMax = settings.NumofSugarHexA(max)
            NumdHexMin = settings.NumofSugardHex(min) : NumdHexMax = settings.NumofSugardHex(max)
            NumPenMin = settings.NumofSugarPen(min) : NumPenMax = settings.NumofSugarPen(max)
            NumMalMin = settings.NumofAcidMal(min) : NumMalMax = settings.NumofAcidMal(max)
            NumCouMin = settings.NumofAcidCou(min) : NumCouMax = settings.NumofAcidCou(max)
            NumFerMin = settings.NumofAcidFer(min) : NumFerMax = settings.NumofAcidFer(max)
            NumSinMin = settings.NumofAcidSin(min) : NumSinMax = settings.NumofAcidSin(max)
            NumDDMPMin = settings.NumofAcidDDMP(min) : NumDDMPMax = settings.NumofAcidDDMP(max)

            NumSugarMin = settings.NumofSugarAll(min) : NumSugarMax = settings.NumofSugarAll(max)
            NumAcidMin = settings.NumofAcidAll(min) : NumAcidMax = settings.NumofAcidAll(max)

            AglyconeType = settings.AglyconeType
            AglyconeSource = settings.AglyconeSource

            SearchPPM = settings.SearchPPM
            Precursors = settings _
                .PrecursorIonType _
                .GetPrecursorIons _
                .ToArray
        End Sub

        Public Shared Function MS1CP(query As Query(), library As Library(), settings As Settings, Optional ionMode As Integer = 1) As Query()
            Return New MS1TopDown(library, settings).CombinatorialPrediction(query, ionMode).ToArray
        End Function

        ''' <summary>
        ''' search for given precursor_type
        ''' </summary>
        ''' <param name="queryGroup">
        ''' Should be one element from the result of <see cref="GroupQueryByMz(IEnumerable(Of Query), Double)"/>
        ''' </param>
        Public Function CombinatorialPrediction(queryGroup As IEnumerable(Of Query), ionMode As Integer) As IEnumerable(Of Query)
            Dim precursors As PrecursorInfo()

            If ionMode = 1 Then
                precursors = Me.Precursors.Where(Function(a) a.precursor_type.Last = "+"c).ToArray
            Else
                precursors = Me.Precursors.Where(Function(a) a.precursor_type.Last = "-"c).ToArray
            End If

            Return RunMs1Query(queryGroup, precursors).ToArray
        End Function

        Public Shared Function GroupQueryByMz(queries As IEnumerable(Of Query), Optional ppm As Double = 1) As NamedCollection(Of Query)()
            Return queries.GroupBy(Function(a) a.PrecursorIon, Tolerance.PPM(1)).ToArray
        End Function

        ''' <summary>
        ''' query with the same precursor ion m/z
        ''' </summary>
        ''' <param name="queries"></param>
        ''' <param name="precursors"></param>
        ''' <returns></returns>
        Private Iterator Function RunMs1Query(queries As IEnumerable(Of Query), precursors As PrecursorInfo()) As IEnumerable(Of Query)
            Dim queryList As Query() = queries.ToArray
            Dim candidates As New List(Of CandidateResult)
            Dim precursorIon = Aggregate query As Query In queryList Into Average(query.PrecursorIon)

            For Each type As PrecursorInfo In precursors
                Dim PrecursorIonMZ = type.adduct
                Dim PrecursorIonN = type.M

                For Each item As CandidateResult In CombinatorialPrediction(0, precursorIon, PrecursorIonMZ, PrecursorIonN)
                    item.precursor_type = type.precursor_type
                    ' add the common candidate result
                    candidates.Add(item)
                Next
            Next

            Dim addSMILES As CandidateResult() = PatternPrediction(candidates).ToArray

            For Each query As Query In queryList
                query.Candidates = addSMILES _
                    .Select(Function(c)
                                Return New CandidateResult(c) With {
                                    .SMILES = c.SMILES _
                                        .Select(Function(str)
                                                    Return str.Replace("[*placeholder*]", $"[{query.PeakNO}]")
                                                End Function) _
                                        .ToArray
                                }
                            End Function) _
                    .ToArray

                Yield query
            Next
        End Function

        Private Iterator Function CombinatorialPrediction(rt_e As Double, precursorIon As Double, PrecursorIonMZ As Double, PrecursorIonN As Integer) As IEnumerable(Of CandidateResult)
            Dim M_w = (precursorIon - PrecursorIonMZ) / PrecursorIonN

            ' invali exact mass that calculated from the precursor ion
            If M_w <= 0 OrElse M_w > 2000 Then
                Return
            End If

            ' 暴力枚举的方法来搜索代谢物信息
            For Hex_n = NumHexMin To NumHexMax
                For HexA_n = NumHexAMin To NumHexAMax
                    For dHex_n = NumdHexMin To NumdHexMax
                        For Pen_n = NumPenMin To NumPenMax
                            For Mal_n = NumMalMin To NumMalMax
                                For Cou_n = NumCouMin To NumCouMax
                                    For Fer_n = NumFerMin To NumFerMax
                                        For Sin_n = NumSinMin To NumSinMax
                                            For DDMP_n = NumDDMPMin To NumDDMPMax

                                                For Each checked In RestrictionCheck(rt_e, Hex_n, HexA_n, dHex_n, Pen_n, Mal_n, Cou_n, Fer_n, Sin_n, DDMP_n, M_w, PrecursorIonMZ, PrecursorIonN)
                                                    Yield checked
                                                Next

                                            Next DDMP_n
                                        Next Sin_n
                                    Next Fer_n
                                Next Cou_n
                            Next Mal_n
                        Next Pen_n
                    Next dHex_n
                Next HexA_n
            Next Hex_n
        End Function

        Private Iterator Function RestrictionCheck(RT_E#, Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%, M_w As Double, PrecursorIonMZ As Double, PrecursorIonN As Integer) As IEnumerable(Of CandidateResult)
            Dim Sugar_n = Hex_n + HexA_n + dHex_n + Pen_n
            Dim Acid_n = Mal_n + Cou_n + Fer_n + Sin_n + DDMP_n

            If Sugar_n >= NumSugarMin AndAlso Sugar_n <= NumSugarMax AndAlso Acid_n >= NumAcidMin AndAlso Acid_n <= NumAcidMax Then
                Dim Attn_w = Hex_n * Hex_w + HexA_n * HexA_w + dHex_n * dHex_w + Pen_n * Pen_w + Mal_n * Mal_w + Cou_n * Cou_w + Fer_n * Fer_w + Sin_n * Sin_w + DDMP_n * DDMP_w
                Dim nH2O_w = (Sugar_n + Acid_n) * H2O_w
                Dim Bal = M_w + nH2O_w - Attn_w

                ' "Aglycone MW Range" Then AglyconeMWLL = minValue : AglyconeMWUL = maxValue
                If Bal >= settings.AglyconeMWRange(0) AndAlso Bal <= settings.AglyconeMWRange(1) Then
                    For Each candidate In RunDatabaseSearch(RT_E:=RT_E, M_w#, Attn_w#, nH2O_w#, Sugar_n%, Acid_n%, Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%)
                        candidate.Theoretical_ExactMass = M_w
                        candidate.Theoretical_PrecursorMz = Bal * PrecursorIonN + PrecursorIonMZ

                        Yield candidate
                    Next
                End If

            End If
        End Function

        Private Iterator Function RunDatabaseSearch(RT_E#, M_w#, Attn_w#, nH2O_w#, Sugar_n%, Acid_n%, Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%) As IEnumerable(Of CandidateResult)
            For Each ref As Library In library
                For Each candidate As CandidateResult In DatabaseSearch(
                    xref:=ref.Xref,
                    RT_E:=RT_E,
                    AglyN:=ref.CommonName,
                    AglyT:=ref.Class,
                    AglyO:=ref.Genus,
                    AglyW:=ref.ExactMass,
                    AglyS:=ref.Universal_SMILES,
                    M_w:=M_w,
                    Attn_w:=Attn_w,
                    nH2O_w:=nH2O_w,
                    Hex_n%,
                    HexA_n%,
                    dHex_n%,
                    Pen_n%,
                    Mal_n%,
                    Cou_n%,
                    Fer_n%,
                    Sin_n%,
                    DDMP_n%,
                    Sugar_n%,
                    Acid_n%
                )
                    Yield candidate
                Next
            Next
        End Function

        Private Iterator Function DatabaseSearch(xref$, RT_E#, AglyN$, AglyT$, AglyO$, AglyW#, AglyS$, M_w#, Attn_w#, nH2O_w#, Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%, Sugar_n%, Acid_n%) As IEnumerable(Of CandidateResult)
            If AglyT = AglyconeType.ToString OrElse AglyconeType = db_AglyconeType.All Then
                If AglyO = AglyconeSource.ToString OrElse AglyconeSource = db_AglyconeSource.All Then
                    Dim err1 = Math.Abs((M_w - (AglyW + Attn_w - nH2O_w)) / (AglyW + Attn_w - nH2O_w)) * 1000000

                    If err1 <= SearchPPM Then
                        ' 在这里如何进行保留时间的预测？
                        Dim RT_P = 0

                        Yield New CandidateResult With {
                            .ExactMass = AglyW,
                            .Name = AglyN,
                            .Hex = Hex_n,
                            .HexA = HexA_n,
                            .dHex = dHex_n,
                            .Pen = Pen_n,
                            .Mal = Mal_n,
                            .Err = err1,
                            .SubstructureAgly = AglyS,
                            .Cou = Cou_n,
                            .DDMP = DDMP_n,
                            .Fer = Fer_n,
                            .Sin = Sin_n,
                            .RT = RT_P,
                            .RTErr = RT_P - RT_E,
                            .Acid_n = Acid_n,
                            .Attn_w = Attn_w,
                            .nH2O_w = nH2O_w,
                            .Sugar_n = Sugar_n,
                            .xref = xref
                        }
                    End If

                End If
            End If
        End Function

        Private Iterator Function PatternPrediction(queryCandidates As IEnumerable(Of CandidateResult)) As IEnumerable(Of CandidateResult)
            ' for each candidate result
            For Each candidate As CandidateResult In queryCandidates
                Call PatternPredictionLoop("*placeholder*", candidate:=candidate)

                ' 20201010
                ' 在这里是否需要过滤掉所有smiles字符串结果为空的candidate？
                If Not candidate.SMILES.IsNullOrEmpty Then
                    Yield candidate
                End If
            Next
        End Function

        Const Hex = "C?C(C(C(C(CO)O?)O)O)O"
        Const HexA = "C?C(C(C(C(C(=O)O)O?)O)O)O"
        Const dHex = "C?C(C(C(C(C)O?)O)O)O"
        Const Pen = "C?C(C(C(CO?)O)O)O"
        Const Mal = "C(=O)CC(=O)O"
        Const Cou = "c?ccc(cc?)C=CC(=O)O"
        Const Fer = "COc?cc(ccc?O)C=CC(=O)O"
        Const Sin = "COc?cc(C=CC(=O)O)cc(c?O)OC"
        Const DDMP = "CC?=C(C(=O)CC(O)O?)O"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="peakNO">apply for ``candidate.smiles``: ["*placeholder*"]</param>
        ''' <param name="candidate"></param>
        Private Sub PatternPredictionLoop(peakNO As String, ByRef candidate As CandidateResult)
            ' 1. Find location and number of OH groups in aglycone
            Dim AglyS1 As String, AglyS2 As String
            Dim OH_n As Integer
            Dim n1 As Long, n2 As Long
            Dim AglyN As String

            AglyN = candidate.Name
            AglyS1 = candidate.SubstructureAgly
            AglyS2 = Strings.Replace(AglyS1, "O)", ".)")
            AglyS2 = Strings.Replace(AglyS2, "=.", "=O")

            If Right(AglyS2, 1) = "O" Then
                AglyS2 = Left(AglyS2, Len(AglyS2) - 1) & "."
            End If

            OH_n = 0

            For e As Integer = 1 To Len(AglyS2)
                If Mid(AglyS2, e, 1) = "." Then OH_n = OH_n + 1
            Next e

            If OH_n = 0 Then Return
            If OH_n > 2 Then OH_n = 2

            n1 = 0
            n2 = 0

            For e As Integer = 1 To Len(AglyS1)
                Dim c As String = Mid(AglyS1, e, 1)

                If Info.IsNumeric(c) Then
                    n2 = CInt(c)

                    If n2 > n1 Then
                        n1 = n2
                    End If
                End If
            Next e

            ' 2. Find type and number of sugars/acids
            Dim Sug_n As Long
            Dim Sug As String = Nothing
            Dim Sug_p(,) As String
            Dim l As Integer

            Sug_n = CLng(candidate.GetSug_nStatic.Sum)

            If Sug_n = 0 Then
                Return
            End If

            ReDim Sug_p(1, 0 To CInt(Sug_n))

            l = 1

            Dim candidateSug_nStatic = candidate.GetSug_nStatic.ToArray

            For e As Integer = 3 To 11
                Dim g = CInt(candidateSug_nStatic(CInt(e - 3)))

                If g > 0 Then
                    If e = 3 Then Sug = Hex
                    If e = 4 Then Sug = HexA
                    If e = 5 Then Sug = dHex
                    If e = 6 Then Sug = Pen
                    If e = 7 Then Sug = Mal
                    If e = 8 Then Sug = Cou
                    If e = 9 Then Sug = Fer
                    If e = 10 Then Sug = Sin
                    If e = 11 Then Sug = DDMP

                    For h As Integer = 1 To g
                        Sug_p(1, l) = Sug
                        l = l + 1
                    Next h
                End If
            Next e

            ' 3. Permutate sugars/acids without repetition
            Dim p As Integer
            Dim rng(,) As Long, temp As Long
            Dim temp1 As Long, y() As Long, d As Long

            p = CInt(WorksheetFunction.Permut(Sug_n, Sug_n))

            ' 3.1 Create array
            ReDim rng(0 To p, 0 To Sug_n)

            ' 3.2 Create first row in array (1, 2, 3, ...)
            For c As Integer = 1 To Sug_n
                rng(1, c) = c
            Next c

            For r As Integer = 2 To p
                Dim e As Integer

                ' 3.3 Find the first smaller number rng(r-1,c-1)<rng(r-1,c)
                For c As Integer = Sug_n To 1 Step -1
                    If rng(r - 1, c - 1) < rng(r - 1, c) Then
                        temp = c - 1
                        Exit For
                    End If
                Next c

                ' 3.4 Copy values from previous row
                For c As Integer = Sug_n To 1 Step -1
                    rng(r, c) = rng(r - 1, c)
                Next c

                ' 3.5 Find a larger number than rng(r-1,temp) as far to the right as possible
                For c As Integer = Sug_n To 1 Step -1
                    If rng(r - 1, c) > rng(r - 1, temp) Then
                        temp1 = rng(r - 1, temp)
                        rng(r, temp) = rng(r - 1, c)
                        rng(r, c) = temp1
                        ReDim y(Sug_n - temp)
                        e = 0

                        For d = temp + 1 To Sug_n
                            y(e) = rng(r, d)
                            e = e + 1
                        Next d

                        e = 0

                        For d = Sug_n To temp + 1 Step -1
                            rng(r, d) = y(e)
                            e = e + 1
                        Next d

                        Exit For
                    End If
                Next c
            Next r

            ' 4 Combine sugars/acids
            Dim w As Long, n As Long
            Dim x(,) As String, t(,) As String, u(,) As String

            ReDim x(0 To Sug_n, 0 To Sug_n)
            ReDim t(100000, 0 To OH_n)
            ReDim u(100000, 0 To OH_n)

            w = 1

            For v As Integer = 1 To p

                ' 4.1 Load each group of sugar/acids from permutation
                For e As Integer = 1 To Sug_n
                    x(1, e) = Sug_p(1, rng(v, e))
                Next e

                ' 4.2 Within each group create all possible oligosaccharides
                l = 0
                For e As Integer = 1 To Sug_n
                    Dim h = e + 1
                    For g As Integer = 2 To Sug_n - l
                        x(g, e) = x(g - 1, e) & x(1, h)
                        h = h + 1
                    Next g
                    l = l + 1
                Next e

                ' 4.3 Within each group make all unique combinations of mono- and oligosaccharides
                ' 4.3.1 Make all possible combinations
                n = 1
                For z As Integer = 0 To Sug_n - 1
                    If n > OH_n Then
                        Exit For
                    End If

                    For q As Integer = 1 To Sug_n - z - 1
                        If OH_n = 1 Then
                            GoTo AllSugarConnected
                        End If

                        Dim c As Integer

                        n = 2

                        If z > 0 Then
                            c = 0
                            For s1 As Integer = 1 To z
                                t(w, n) = x(1, q + s1)
                                c = c + 1
                                n = n + 1

                                If n > OH_n - 1 Then
                                    Exit For
                                End If
                            Next
                        End If

                        t(w, 1) = x(q, 1)
                        t(w, n) = x(Sug_n - (q + z), (q + z) + 1)

                        If c < z Then
                            For e As Integer = 1 To OH_n
                                t(w, e) = ""
                            Next e

                            w = w - 1
                        End If

                        n = n + 1
                        w = w + 1
                    Next q
                Next z
AllSugarConnected:
                For e As Integer = 1 To Sug_n
                    t(w, 1) = t(w, 1) + x(1, e)
                Next e

                w = w + 1

            Next v

            ' 4.3.2 Remove all duplicates regardless of order
            Dim s As Integer = 1

            For e As Integer = 1 To w - 1
                Dim c = 0

                For r As Integer = 1 To s - 1
                    c = 0
                    For g As Integer = 1 To OH_n
                        For h As Integer = 1 To OH_n
                            If t(e, g) = u(r, h) Then
                                u(r, h) = u(r, h) + "*"
                                c = c + 1
                                Exit For
                            End If
                        Next h
                    Next g

                    If c = OH_n Then
                        Exit For
                    End If
                Next r

                If c < OH_n Then
                    For g As Integer = 1 To OH_n
                        u(s, g) = t(e, g)
                    Next g
                    s = s + 1
                End If

                For r As Integer = 1 To s - 1
                    For h As Integer = 1 To OH_n
                        If Right(u(r, h), 1) = "*" Then u(r, h) = Left(u(r, h), Len(u(r, h)) - 1)
                    Next h
                Next r
            Next e

            ' 5. Attach each sugar/acid combination to aglycone to create all possible glycosides
            Dim glycN As String
            Dim sugComb As StringBuilder
            Dim sugComb1 As String
            Dim predicted As New List(Of String)

            For e As Integer = 1 To s - 1
                sugComb = New StringBuilder

                For g As Integer = 1 To OH_n
                    If u(e, g) <> "" Then
                        sugComb1 = u(e, g)

                        If InStr(sugComb1, Hex) <> 0 Then sugComb1 = Strings.Replace(sugComb1, Hex, "-Hex")
                        If InStr(sugComb1, HexA) <> 0 Then sugComb1 = Strings.Replace(sugComb1, HexA, "-HexA")
                        If InStr(sugComb1, dHex) <> 0 Then sugComb1 = Strings.Replace(sugComb1, dHex, "-dHex")
                        If InStr(sugComb1, Mal) <> 0 Then sugComb1 = Strings.Replace(sugComb1, Mal, "-Mal")
                        If InStr(sugComb1, Pen) <> 0 Then sugComb1 = Strings.Replace(sugComb1, Pen, "-Pen")
                        If InStr(sugComb1, Cou) <> 0 Then sugComb1 = Strings.Replace(sugComb1, Cou, "-Cou")
                        If InStr(sugComb1, Fer) <> 0 Then sugComb1 = Strings.Replace(sugComb1, Fer, "-Fer")
                        If InStr(sugComb1, Sin) <> 0 Then sugComb1 = Strings.Replace(sugComb1, Sin, "-Sin")
                        If InStr(sugComb1, DDMP) <> 0 Then sugComb1 = Strings.Replace(sugComb1, DDMP, "-DDMP")

                        sugComb = sugComb.Append(", ").Append(sugComb1)
                    Else
                        Exit For
                    End If
                Next g

                ' GlycN = AglyN.Replace("-", "_") & SugComb
                glycN = $"[{peakNO}]" & sugComb.ToString

                sugComb.Clear()
                predicted.Add(glycN)
            Next

            candidate.SMILES = predicted.ToArray
        End Sub
    End Class
End Namespace
