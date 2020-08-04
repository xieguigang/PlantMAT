#Region "Microsoft.VisualBasic::693f12eb199d4cb500b31123cee14117, PlantMAT.Core\Algorithm\MS1TopDown.vb"

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
    '                   RunDatabaseSearch
    ' 
    '         Sub: applySettings, PatternPredictionLoop
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.Linq
Imports PlantMAT.Core.Models
Imports PlantMAT.Core.Models.AnnotationResult
Imports WorksheetFunction = Microsoft.VisualBasic.Math.VBMath

Namespace Algorithm

    ''' <summary>
    ''' This module performs combinatorial enumeration
    ''' </summary>
    Public Class MS1TopDown

        Dim library As Library()
        Dim settings As Settings
        Dim NumHexMin, NumHexMax, NumHexAMin, NumHexAMax, NumdHexMin, NumdHexMax, NumPenMin, NumPenMax, NumMalMin, NumMalMax, NumCouMin, NumCouMax, NumFerMin, NumFerMax, NumSinMin, NumSinMax, NumDDMPMin, NumDDMPMax As Integer
        Dim NumSugarMin, NumSugarMax, NumAcidMin, NumAcidMax As Integer
        Dim AglyconeType As db_AglyconeType = db_AglyconeType.All
        Dim AglyconeSource As db_AglyconeSource = db_AglyconeSource.All
        Dim SearchPPM As Double
        Dim Precursors As PrecursorInfo()

        Sub New(library As Library(), settings As Settings)
            Me.library = library
            Me.settings = settings

            Call applySettings()
        End Sub

        Private Sub applySettings()
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

        Public Function MS1CP(query As Query(), Optional ionMode As Integer = 1) As Query()
            Dim result As Query()

            ' Run combinatorial enumeration
            Console.WriteLine("Now analyzing, please wait...")
            ' Peform combinatorial enumeration and show the calculation progress (MS1CP)
            result = CombinatorialPrediction(query, ionMode).ToArray
            ' Show the message box after the calculation is finished
            Console.WriteLine("Substructure prediction finished")

            Return result
        End Function

        ''' <summary>
        ''' search for given precursor_type
        ''' </summary>
        ''' <param name="queries"></param>
        Private Function CombinatorialPrediction(queries As IEnumerable(Of Query), ionMode As Integer) As IEnumerable(Of Query)
            Dim precursors As PrecursorInfo()

            If ionMode = 1 Then
                precursors = Me.Precursors.Where(Function(a) a.precursor_type.Last = "+"c).ToArray
            Else
                precursors = Me.Precursors.Where(Function(a) a.precursor_type.Last = "-"c).ToArray
            End If

            Return queries _
                .AsParallel _
                .Select(Function(query)
                            For Each type As PrecursorInfo In precursors
                                Dim PrecursorIonMZ = type.adduct
                                Dim PrecursorIonN = type.M
                                Dim result = CombinatorialPrediction(query, PrecursorIonMZ, PrecursorIonN)

                                For Each item As CandidateResult In result
                                    item.precursor_type = type.precursor_type
                                    query.Candidates.Add(item)
                                Next
                            Next

                            Return PatternPrediction(query)
                        End Function)
        End Function

        Private Function CombinatorialPrediction(query As Query, PrecursorIonMZ As Double, PrecursorIonN As Integer) As IEnumerable(Of CandidateResult)
            Dim ErrorCheck = False
            Dim RT_E = query.PeakNO
            Dim M_w = (query.PrecursorIon - PrecursorIonMZ) / PrecursorIonN
            Dim Candidate As New List(Of CandidateResult)

            If M_w <= 0 Or M_w > 2000 Then
                Return query
            End If

            For Hex_n = NumHexMin To NumHexMax
                For HexA_n = NumHexAMin To NumHexAMax
                    For dHex_n = NumdHexMin To NumdHexMax
                        For Pen_n = NumPenMin To NumPenMax
                            For Mal_n = NumMalMin To NumMalMax
                                For Cou_n = NumCouMin To NumCouMax
                                    For Fer_n = NumFerMin To NumFerMax
                                        For Sin_n = NumSinMin To NumSinMax
                                            For DDMP_n = NumDDMPMin To NumDDMPMax

                                                Call RestrictionCheck(RT_E, Hex_n, HexA_n, dHex_n, Pen_n, Mal_n, Cou_n, Fer_n, Sin_n, DDMP_n, M_w).DoCall(AddressOf Candidate.AddRange)

                                            Next DDMP_n
                                        Next Sin_n
                                    Next Fer_n
                                Next Cou_n
                            Next Mal_n
                        Next Pen_n
                    Next dHex_n
                Next HexA_n
            Next Hex_n

            Return Candidate
        End Function

        Private Iterator Function RestrictionCheck(RT_E#, Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%, M_w As Double) As IEnumerable(Of CandidateResult)
            Dim Sugar_n = Hex_n + HexA_n + dHex_n + Pen_n
            Dim Acid_n = Mal_n + Cou_n + Fer_n + Sin_n + DDMP_n

            If Sugar_n >= NumSugarMin And Sugar_n <= NumSugarMax And Acid_n >= NumAcidMin And Acid_n <= NumAcidMax Then
                Dim Attn_w = Hex_n * Hex_w + HexA_n * HexA_w + dHex_n * dHex_w + Pen_n * Pen_w + Mal_n * Mal_w + Cou_n * Cou_w + Fer_n * Fer_w + Sin_n * Sin_w + DDMP_n * DDMP_w
                Dim nH2O_w = (Sugar_n + Acid_n) * H2O_w
                Dim Bal = M_w + nH2O_w - Attn_w

                ' "Aglycone MW Range" Then AglyconeMWLL = minValue : AglyconeMWUL = maxValue
                If Bal >= settings.AglyconeMWRange(0) And Bal <= settings.AglyconeMWRange(1) Then
                    For Each candidate In RunDatabaseSearch(RT_E:=RT_E, M_w#, Attn_w#, nH2O_w#, Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%)
                        Yield candidate
                    Next
                End If

            End If
        End Function

        Private Iterator Function RunDatabaseSearch(RT_E#, M_w#, Attn_w#, nH2O_w#, Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%) As IEnumerable(Of CandidateResult)
            For Each ref As Library In library
                For Each candidate As CandidateResult In DatabaseSearch(
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
                    DDMP_n%
                )
                    Yield candidate
                Next
            Next
        End Function

        Private Iterator Function DatabaseSearch(RT_E#, AglyN$, AglyT$, AglyO$, AglyW#, AglyS$, M_w#, Attn_w#, nH2O_w#, Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%) As IEnumerable(Of CandidateResult)
            If AglyT = AglyconeType.ToString Or AglyconeType = db_AglyconeType.All Then
                If AglyO = AglyconeSource.ToString Or AglyconeSource = db_AglyconeSource.All Then
                    Dim Err1 = Math.Abs((M_w - (AglyW + Attn_w - nH2O_w)) / (AglyW + Attn_w - nH2O_w)) * 1000000

                    If Err1 <= SearchPPM Then
                        Dim RT_P = 0

                        Yield New CandidateResult With {
                            .ExactMass = AglyW,
                            .Name = AglyN,
                            .Hex = Hex_n,
                            .HexA = HexA_n,
                            .dHex = dHex_n,
                            .Pen = Pen_n,
                            .Mal = Mal_n,
                            .Err = Err1,
                            .SubstructureAgly = AglyS,
                            .Cou = Cou_n,
                            .DDMP = DDMP_n,
                            .Fer = Fer_n,
                            .Sin = Sin_n,
                            .RT = RT_P,
                            .RTErr = RT_P - RT_E
                        }
                    End If

                End If
            End If
        End Function

        Private Function PatternPrediction(query As Query) As Query
            For m As Integer = 0 To query.Candidates.Count - 1
                Call PatternPredictionLoop(query.PeakNO, query(m), m)
            Next

            Return query
        End Function

        Private Sub PatternPredictionLoop(peakNO As Integer, candidate As CandidateResult, m As Integer)
            ' 1. Find location and number of OH groups in aglycone
            Dim AglyS1 As String, AglyS2 As String
            Dim Hex As String, HexA As String, dHex As String, Pen As String
            Dim Cou As String, Fer As String, Sin As String, Mal As String
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
                If Information.IsNumeric(Mid(AglyS1, e, 1)) Then
                    n2 = CInt(Mid(AglyS1, e, 1))

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

            Hex = "C?C(C(C(C(CO)O?)O)O)O"
            HexA = "C?C(C(C(C(C(=O)O)O?)O)O)O"
            dHex = "C?C(C(C(C(C)O?)O)O)O"
            Pen = "C?C(C(C(CO?)O)O)O"
            Mal = "C(=O)CC(=O)O"
            Cou = "c?ccc(cc?)C=CC(=O)O"
            Fer = "COc?cc(ccc?O)C=CC(=O)O"
            Sin = "COc?cc(C=CC(=O)O)cc(c?O)OC"
            Dim DDMP = "CC?=C(C(=O)CC(O)O?)O"

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
                        x(g, e) = x(g - 1, e) + x(1, h)
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
            Dim s = 1

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
            Dim GlycS As String, GlycN As String
            Dim SugComb As String, SugComb1 As String
            Dim n3 As Long

            For e As Integer = 1 To CInt(s) - 1
                GlycS = AglyS2
                n3 = n1
                SugComb = ""

                For g As Integer = 1 To OH_n
                    If u(e, g) <> "" Then
                        SugComb1 = u(e, g)

                        If InStr(SugComb1, Hex) <> 0 Then SugComb1 = Strings.Replace(SugComb1, Hex, "-Hex")
                        If InStr(SugComb1, HexA) <> 0 Then SugComb1 = Strings.Replace(SugComb1, HexA, "-HexA")
                        If InStr(SugComb1, dHex) <> 0 Then SugComb1 = Strings.Replace(SugComb1, dHex, "-dHex")
                        If InStr(SugComb1, Mal) <> 0 Then SugComb1 = Strings.Replace(SugComb1, Mal, "-Mal")
                        If InStr(SugComb1, Pen) <> 0 Then SugComb1 = Strings.Replace(SugComb1, Pen, "-Pen")
                        If InStr(SugComb1, Cou) <> 0 Then SugComb1 = Strings.Replace(SugComb1, Cou, "-Cou")
                        If InStr(SugComb1, Fer) <> 0 Then SugComb1 = Strings.Replace(SugComb1, Fer, "-Fer")
                        If InStr(SugComb1, Sin) <> 0 Then SugComb1 = Strings.Replace(SugComb1, Sin, "-Sin")
                        If InStr(SugComb1, DDMP) <> 0 Then SugComb1 = Strings.Replace(SugComb1, DDMP, "-DDMP")

                        SugComb = SugComb + ", " + SugComb1
                    Else
                        Exit For
                    End If
                Next g

                GlycN = AglyN + SugComb

                Call New SMILES With {
                    .Sequence = CStr(m) + "-" + CStr(e),
                    .GlycN = GlycN,
                    .GlycS = GlycS,
                    .peakNo = peakNO
                }.DoCall(AddressOf candidate.SMILES.Add)
            Next e
        End Sub
    End Class
End Namespace
