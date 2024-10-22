﻿#Region "Microsoft.VisualBasic::6e958d9242d0c5d62684d6ef1a48de8f, PlantMAT.Core\Algorithm\MS1TopDown.vb"

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

'     Delegate Function
' 
' 
'     Class MS1TopDown
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: (+2 Overloads) CombinatorialPrediction, DatabaseSearch, GroupQueryByMz, MS1CP, PatternPrediction
'                   RunDatabaseSearch, RunMs1Query
' 
'         Sub: applySettings, PatternPredictionLoop
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math
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
        Dim AglyconeType As db_AglyconeType = db_AglyconeType.All
        Dim AglyconeSource As db_AglyconeSource = db_AglyconeSource.All
        Dim SearchPPM As Double
        Dim Precursors As PrecursorInfo()
        Dim aglyconeSet As NamedValue(Of Double)()

        Public Sub New(library As Library(), settings As Settings)
            MyBase.New(settings)

            ' load metabolite reference library
            Me.library = library

            If settings.AglyconeSet.IsNullOrEmpty Then
                Me.aglyconeSet = Nothing
            Else
                Me.aglyconeSet = settings.AglyconeSet _
                    .Select(Function(a)
                                Return New NamedValue(Of Double)(a.name, Val(a.text))
                            End Function) _
                    .ToArray
            End If
        End Sub

        Protected Friend Overrides Sub applySettings()
            AglyconeType = settings.AglyconeType
            AglyconeSource = settings.AglyconeSource

            SearchPPM = settings.SearchPPM
            Precursors = settings _
                .PrecursorIonType _
                .GetPrecursorIons _
                .ToArray
        End Sub

        Public Shared Function MS1CP(query As Query(), library As Library(), settings As Settings, Optional ionMode As Integer = 1) As Query()
            Dim output As New List(Of Query)
            Dim mzList = GroupQueryByMz(query)

            Call Console.WriteLine("Run Annotation with parameters:")
            Call Console.WriteLine(settings.GetXml)

            For Each block As NamedCollection(Of Query) In mzList
                Call New MS1TopDown(library, settings) _
                    .CombinatorialPrediction(block, ionMode) _
                    .DoCall(AddressOf output.AddRange)
            Next

            Return output.ToArray
        End Function

        ''' <summary>
        ''' search for given precursor_type
        ''' </summary>
        ''' <param name="queryGroup">
        ''' Should be one element from the result of <see cref="GroupQueryByMz"/>
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

        Public Shared Function GroupQueryByMz(Of Query As IMs1)(queries As IEnumerable(Of Query), Optional ppm As Double = 1) As NamedCollection(Of Query)()
            ' shuffle is required for avoid the large mz stays in one threads problem
            ' due to the reason of groupBy is always reorder mz in asc orders
            '
            Return queries _
                .GroupBy(Function(a) a.mz, Tolerance.PPM(ppm)) _
                .Shuffles _
                .ToArray
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
                For Each item As CandidateResult In CombinatorialPrediction(0, precursorIon, precursor:=type)
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

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="rt_e"></param>
        ''' <param name="precursorIon">ion m/z of the precursor</param>
        ''' <returns></returns>
        Private Iterator Function CombinatorialPrediction(rt_e As Double, precursorIon As Double, precursor As PrecursorInfo) As IEnumerable(Of CandidateResult)
            Dim neutralLossSearch As New NeutralLossSearch(settings, {})
            Dim PrecursorIonMZ As Double = precursor.adduct
            Dim PrecursorIonN As Double = precursor.M
            Dim M_w As Double = (precursorIon - PrecursorIonMZ) / PrecursorIonN
            Dim search As IEnumerable(Of NamedValue(Of NeutralLoss))

            Call neutralLossSearch.applySettings()

            If aglyconeSet.IsNullOrEmpty Then
                search = neutralLossSearch _
                    .NeutralLosses(precursorIon, precursor) _
                    .Select(Function(loss)
                                Return New NamedValue(Of NeutralLoss)("NA", loss)
                            End Function)
            Else
                search = neutralLossSearch.SearchAny(aglyconeSet, precursorIon, precursor)
            End If

            For Each lossResult As NamedValue(Of NeutralLoss) In search
                Dim neutralLoss As NeutralLoss = lossResult.Value
                Dim Sugar_n As Integer = neutralLoss.Sugar_n
                Dim Acid_n As Integer = neutralLoss.Acid_n
                Dim Attn_w As Double = neutralLoss.Attn_w
                Dim nH2O_w As Double = (Sugar_n + Acid_n) * H2O_w
                Dim Bal As Double = neutralLoss.AglyconeExactMass(M_w)

                For Each candidate In RunDatabaseSearch(RT_E:=rt_e, M_w#, Attn_w#, nH2O_w#, Sugar_n%, Acid_n%, neutralLoss)
                    candidate.Theoretical_ExactMass = M_w
                    candidate.Theoretical_PrecursorMz = Bal * PrecursorIonN + PrecursorIonMZ
                    candidate.AglyconeFamily = lossResult.Name

                    Yield candidate
                Next
            Next
        End Function

        Private Iterator Function RunDatabaseSearch(RT_E#, M_w#, Attn_w#, nH2O_w#, Sugar_n%, Acid_n%, neutralLoss As NeutralLoss) As IEnumerable(Of CandidateResult)
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
                    neutralLoss,
                    Sugar_n%,
                    Acid_n%
                )
                    Yield candidate
                Next
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="xref$"></param>
        ''' <param name="RT_E#"></param>
        ''' <param name="AglyN$"></param>
        ''' <param name="AglyT$"></param>
        ''' <param name="AglyO$"></param>
        ''' <param name="AglyW#"></param>
        ''' <param name="AglyS">Universal SMILES</param>
        ''' <param name="M_w#"></param>
        ''' <param name="Attn_w#"></param>
        ''' <param name="nH2O_w#"></param>
        ''' <param name="neutralLoss"></param>
        ''' <param name="Sugar_n%"></param>
        ''' <param name="Acid_n%"></param>
        ''' <returns></returns>
        Private Iterator Function DatabaseSearch(xref$, RT_E#, AglyN$, AglyT$, AglyO$, AglyW#, AglyS$, M_w#, Attn_w#, nH2O_w#, neutralLoss As NeutralLoss, Sugar_n%, Acid_n%) As IEnumerable(Of CandidateResult)
            If AglyT = AglyconeType.ToString OrElse AglyconeType = db_AglyconeType.All Then
                If AglyO = AglyconeSource.ToString OrElse AglyconeSource = db_AglyconeSource.All Then
                    Dim err1 = Math.Abs((M_w - (AglyW + Attn_w - nH2O_w)) / (AglyW + Attn_w - nH2O_w)) * 1000000

                    If err1 <= SearchPPM Then
                        ' 在这里如何进行保留时间的预测？
                        Dim RT_P = 0

                        Yield New CandidateResult With {
                            .ExactMass = AglyW,
                            .Name = AglyN,
                            .Hex = neutralLoss.Hex,
                            .HexA = neutralLoss.HexA,
                            .dHex = neutralLoss.dHex,
                            .Pen = neutralLoss.Pen,
                            .Mal = neutralLoss.Mal,
                            .Err = err1,
                            .SubstructureAgly = AglyS,
                            .Cou = neutralLoss.Cou,
                            .DDMP = neutralLoss.DDMP,
                            .Fer = neutralLoss.Fer,
                            .Sin = neutralLoss.Sin,
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

        ''' <summary>
        ''' Hexose
        ''' 
        ''' Canonical SMILES: C(C1C(C(C(C(O1)O)O)O)O)O
        ''' </summary>
        Const Hex = "C?C(C(C(C(CO)O?)O)O)O"
        ''' <summary>
        ''' Hexenuronic acid
        ''' 
        ''' 
        ''' </summary>
        Const HexA = "C?C(C(C(C(C(=O)O)O?)O)O)O"
        ''' <summary>
        ''' 6-Deoxy-Hexose
        ''' 
        ''' Canonical SMILES: CC1C(C(C(C(O1)O)O)O)O
        ''' </summary>
        Const dHex = "C?C(C(C(C(C)O?)O)O)O"

        ''' <summary>
        ''' pentose
        ''' 
        ''' Canonical SMILES: C1C(C(C(C(O1)O)O)O)O
        ''' </summary>
        Const Pen = "C?C(C(C(CO?)O)O)O"

        ''' <summary>
        ''' Malonic acid
        ''' 
        ''' Canonical SMILES: C(C(=O)O)C(=O)O
        ''' </summary>
        Const Mal = "C(=O)CC(=O)O"

        ''' <summary>
        ''' Coumarinic acid
        ''' 
        ''' Canonical SMILES: C1=CC=C(C(=C1)C=CC(=O)O)O
        ''' </summary>
        Const Cou = "c?ccc(cc?)C=CC(=O)O"
        ''' <summary>
        ''' Ferulic acid
        ''' 
        ''' Canonical SMILES: COC1=C(C=CC(=C1)C=CC(=O)O)O
        ''' </summary>
        Const Fer = "COc?cc(ccc?O)C=CC(=O)O"
        ''' <summary>
        ''' Sinapinic acid
        ''' 
        ''' Canonical SMILES: COC1=CC(=CC(=C1O)OC)C=CC(=O)O
        ''' </summary>
        Const Sin = "COc?cc(C=CC(=O)O)cc(c?O)OC"
        Const DDMP = "CC?=C(C(=O)CC(O)O?)O"

        ''' <summary>
        ''' work on the SMILES information from the library
        ''' </summary>
        ''' <param name="peakNO">apply for ``candidate.smiles``: ["*placeholder*"]</param>
        ''' <param name="candidate"></param>
        Private Sub PatternPredictionLoop(peakNO As String, ByRef candidate As CandidateResult)
            ' 1. Find location and number of OH groups in aglycone
            Dim AglyS1 As String, AglyS2 As String
            Dim OH_n As Integer
            Dim n1 As Long, n2 As Long
            Dim AglyN As String

            ' this is the metabolite common name
            AglyN = candidate.Name
            ' Universal SMILES
            AglyS1 = candidate.SubstructureAgly
            ' AglyS2 is apply for static of the -OH group
            AglyS2 = Strings.Replace(AglyS1, "O)", ".)")
            AglyS2 = Strings.Replace(AglyS2, "=.", "=O")

            If Right(AglyS2, 1) = "O" Then
                AglyS2 = Left(AglyS2, Len(AglyS2) - 1) & "."
            End If

            OH_n = 0

            For e As Integer = 1 To Len(AglyS2)
                If Mid(AglyS2, e, 1) = "." Then
                    OH_n = OH_n + 1
                End If
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

            ' get totoal all predicted result count of suger
            Sug_n = candidate.GetSug_nStatic.Sum

            If Sug_n = 0 Then
                Return
            End If

            ReDim Sug_p(1, 0 To CInt(Sug_n))

            l = 1

            ' a count vector of [Hex, HexA, dHex, Pen, Mal, Cou, Fer, Sin, DDMP]
            Dim candidateSug_nStatic = candidate.GetSug_nStatic

            For e As Integer = 3 To 11
                Dim g As Integer = candidateSug_nStatic(e - 3)

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
            Next

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
                        Next
                    Next

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
                    Next
                Next
            Next

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
