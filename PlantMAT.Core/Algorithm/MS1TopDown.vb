Imports Microsoft.VisualBasic.Linq
Imports WorksheetFunction = Microsoft.VisualBasic.Math.VBMath

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
    End Sub

    Public Function MS1CP(query As Query()) As Query()
        Dim result As Query()

        ' Run combinatorial enumeration
        Console.WriteLine("Now analyzing, please wait...")
        ' Peform combinatorial enumeration and show the calculation progress (MS1CP)
        result = MS1_CombinatorialPrediction(query, settings.PrecursorIonMZ, settings.PrecursorIonN).ToArray
        ' Show the message box after the calculation is finished
        Console.WriteLine("Substructure prediction finished")

        Return result
    End Function

    ''' <summary>
    ''' search for given precursor_type
    ''' </summary>
    ''' <param name="queries"></param>
    ''' <param name="PrecursorIonMZ">adducts</param>
    ''' <param name="PrecursorIonN">M</param>
    Private Function MS1_CombinatorialPrediction(queries As IEnumerable(Of Query), PrecursorIonMZ As Double, PrecursorIonN As Integer) As IEnumerable(Of Query)
        Return queries _
            .AsParallel _
            .Select(Function(query)
                        Return MS1_CombinatorialPrediction(query, PrecursorIonMZ, PrecursorIonN)
                    End Function)
    End Function

    Private Function MS1_CombinatorialPrediction(query As Query, PrecursorIonMZ As Double, PrecursorIonN As Integer) As Query
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

                                            Call MS1_CombinatorialPrediction_RestrictionCheck(RT_E, Hex_n, HexA_n, dHex_n, Pen_n, Mal_n, Cou_n, Fer_n, Sin_n, DDMP_n, M_w).DoCall(AddressOf Candidate.AddRange)

                                        Next DDMP_n
                                    Next Sin_n
                                Next Fer_n
                            Next Cou_n
                        Next Mal_n
                    Next Pen_n
                Next dHex_n
            Next HexA_n
        Next Hex_n

        query.Candidates = Candidate

        Return MS1_CombinatorialPrediciton_PatternPrediction(query)
    End Function

    Private Iterator Function MS1_CombinatorialPrediction_RestrictionCheck(RT_E#, Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%, M_w As Double) As IEnumerable(Of CandidateResult)
        Dim Sugar_n = Hex_n + HexA_n + dHex_n + Pen_n
        Dim Acid_n = Mal_n + Cou_n + Fer_n + Sin_n + DDMP_n

        If Sugar_n >= NumSugarMin And Sugar_n <= NumSugarMax And
   Acid_n >= NumAcidMin And Acid_n <= NumAcidMax Then

            Dim Attn_w = Hex_n * Hex_w + HexA_n * HexA_w + dHex_n * dHex_w + Pen_n * Pen_w +
   Mal_n * Mal_w + Cou_n * Cou_w + Fer_n * Fer_w + Sin_n * Sin_w + DDMP_n * DDMP_w
            Dim nH2O_w = (Sugar_n + Acid_n) * H2O_w
            Dim Bal = M_w + nH2O_w - Attn_w

            ' "Aglycone MW Range" Then AglyconeMWLL = minValue : AglyconeMWUL = maxValue
            If Bal >= settings.AglyconeMWRange(0) And Bal <= settings.AglyconeMWRange(1) Then
                For Each candidate In MS1_CombinatorialPrediciton_InternalDatabase(RT_E:=RT_E, M_w#, Attn_w#, nH2O_w#, Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%)
                    Yield candidate
                Next
            End If

        End If
    End Function

    Private Iterator Function MS1_CombinatorialPrediciton_InternalDatabase(RT_E#, M_w#, Attn_w#, nH2O_w#, Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%) As IEnumerable(Of CandidateResult)
        For Each ref As Library In library
            For Each candidate In MS1_CombinatorialPrediciton_DatabaseSearch(RT_E:=RT_E, AglyN:=ref.CommonName,
            AglyT:=ref.Class,
            AglyO:=ref.Genus,
            AglyW:=ref.ExactMass,
            AglyS:=ref.Universal_SMILES, M_w:=M_w, Attn_w:=Attn_w, nH2O_w:=nH2O_w, Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%)
                Yield candidate
            Next
        Next
    End Function

    Private Iterator Function MS1_CombinatorialPrediciton_DatabaseSearch(RT_E#, AglyN$, AglyT$, AglyO$, AglyW#, AglyS$, M_w#, Attn_w#, nH2O_w#, Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%) As IEnumerable(Of CandidateResult)
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

    Private Function MS1_CombinatorialPrediciton_PatternPrediction(query As Query) As Query
        For m As Integer = 0 To query.Candidates.Count - 1
            Dim candidate As CandidateResult = query(m)

            Call MS1_CombinatorialPrediciton_PatternPrediction(query.PeakNO, candidate, m)
        Next

        Return query
    End Function

    Private Sub MS1_CombinatorialPrediciton_PatternPrediction(peakNO As Integer, candidate As CandidateResult, m As Integer)
        ' 1. Find location and number of OH groups in aglycone
        Dim AglyS1 As String, AglyS2 As String
        Dim Hex As String, HexA As String, dHex As String, Pen As String
        Dim Cou As String, Fer As String, Sin As String, Mal As String
        Dim OH_n As Long
        Dim n1 As Long, n2 As Long
        Dim AglyN As String

        AglyN = candidate.Name
        AglyS1 = candidate.SubstructureAgly
        AglyS2 = Strings.Replace(AglyS1, "O)", ".)")
        AglyS2 = Strings.Replace(AglyS2, "=.", "=O")
        If Right(AglyS2, 1) = "O" Then AglyS2 = Left(AglyS2, Len(AglyS2) - 1) & "."

        OH_n = 0
        For e As Integer = 1 To Len(AglyS2)
            If Mid(AglyS2, e, 1) = "." Then OH_n = OH_n + 1
        Next e

        If OH_n = 0 Then Exit Sub
        If OH_n > 2 Then OH_n = 2

        n1 = 0
        n2 = 0
        For e As Integer = 1 To Len(AglyS1)
            If Information.IsNumeric(Mid(AglyS1, e, 1)) Then
                n2 = CInt(Mid(AglyS1, e, 1))
                If n2 > n1 Then n1 = n2
            End If
        Next e

        ' 2. Find type and number of sugars/acids
        Dim Sug_n As Long
        Dim Sug As String = Nothing
        Dim Sug_p(,) As String
        Dim g As Long, h As Long, l As Long

        Sug_n = CLng(candidate.GetSug_nStatic.Sum)

        If Sug_n = 0 Then
            Return
        End If

        ReDim Sug_p(1, 0 To Sug_n)
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
            g = CLng(candidateSug_nStatic(CInt(e - 3)))
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
                For h = 1 To g
                    Sug_p(1, l) = Sug
                    l = l + 1
                Next h
            End If
        Next e

        ' 3. Permutate sugars/acids without repetition
        Dim c As Long, r As Long, p As Long
        Dim rng(,) As Long, temp As Long
        Dim temp1 As Long, y() As Long, d As Long

        p = WorksheetFunction.Permut(Sug_n, Sug_n)

        ' 3.1 Create array
        ReDim rng(0 To p, 0 To Sug_n)

        ' 3.2 Create first row in array (1, 2, 3, ...)
        For c = 1 To Sug_n
            rng(1, c) = c
        Next c

        For r = 2 To p
            Dim e As Integer

            ' 3.3 Find the first smaller number rng(r-1,c-1)<rng(r-1,c)
            For c = Sug_n To 1 Step -1
                If rng(r - 1, c - 1) < rng(r - 1, c) Then
                    temp = c - 1
                    Exit For
                End If
            Next c

            ' 3.4 Copy values from previous row
            For c = Sug_n To 1 Step -1
                rng(r, c) = rng(r - 1, c)
            Next c

            ' 3.5 Find a larger number than rng(r-1,temp) as far to the right as possible
            For c = Sug_n To 1 Step -1
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
        Dim z As Long, q As Long, s As Long
        Dim v As Long, w As Long, n As Long
        Dim x(,) As String, t(,) As String, u(,) As String

        ReDim x(0 To Sug_n, 0 To Sug_n)
        ReDim t(100000, 0 To OH_n)
        ReDim u(100000, 0 To OH_n)

        w = 1

        For v = 1 To p

            ' 4.1 Load each group of sugar/acids from permutation
            For e As Integer = 1 To Sug_n
                x(1, e) = Sug_p(1, rng(v, e))
            Next e

            ' 4.2 Within each group create all possible oligosaccharides
            l = 0
            For e As Integer = 1 To Sug_n
                h = e + 1
                For g = 2 To Sug_n - l
                    x(g, e) = x(g - 1, e) + x(1, h)
                    h = h + 1
                Next g
                l = l + 1
            Next e

            ' 4.3 Within each group make all unique combinations of mono- and oligosaccharides
            ' 4.3.1 Make all possible combinations
            n = 1
            For z = 0 To Sug_n - 1
                If n > OH_n Then Exit For
                For q = 1 To Sug_n - z - 1
                    If OH_n = 1 Then
                        GoTo AllSugarConnected
                    End If
                    n = 2
                    If z > 0 Then
                        c = 0
                        For s = 1 To z
                            t(w, n) = x(1, q + s)
                            c = c + 1
                            n = n + 1
                            If n > OH_n - 1 Then Exit For
                        Next s
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
        s = 1
        c = 0
        For e As Integer = 1 To w - 1
            For r = 1 To s - 1
                c = 0
                For g = 1 To OH_n
                    For h = 1 To OH_n
                        If t(e, g) = u(r, h) Then
                            u(r, h) = u(r, h) + "*"
                            c = c + 1
                            Exit For
                        End If
                    Next h
                Next g
                If c = OH_n Then Exit For
            Next r
            If c < OH_n Then
                For g = 1 To OH_n
                    u(s, g) = t(e, g)
                Next g
                s = s + 1
            End If
            For r = 1 To s - 1
                For h = 1 To OH_n
                    If Right(u(r, h), 1) = "*" Then u(r, h) = Left(u(r, h), Len(u(r, h)) - 1)
                Next h
            Next r
        Next e

        ' 5. Attach each sugar/acid combination to aglycone to create all possible glycosides
        Dim GlycS As String, GlycN As String
        Dim SugComb As String, SugComb1 As String
        Dim n3 As Long

        For e As Integer = 1 To s - 1
            GlycS = AglyS2
            n3 = n1
            SugComb = ""
            For g = 1 To OH_n
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

            candidate.SMILES.Add(New SMILES With {.Sequence = CStr(m) + "-" + CStr(e), .GlycN = GlycN, .GlycS = GlycS, .peakNo = peakNO})
        Next e
    End Sub
End Class
