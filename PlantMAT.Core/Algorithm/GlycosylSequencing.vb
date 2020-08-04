﻿Imports Microsoft.VisualBasic.Linq

Public Class GlycosylSequencing

    Dim PrecursorIonType$
    Dim PrecursorIonMZ#
    Dim PrecursorIonN%
    Dim mzPPM As Double

    Dim settings As Settings

    Sub New(settings As Settings)
        Me.settings = settings
        Me.applySettings()
    End Sub

    Private Sub applySettings()
        mzPPM = settings.mzPPM

        PrecursorIonType = settings.PrecursorIonType
        PrecursorIonMZ = settings.PrecursorIonMZ
        PrecursorIonN = settings.PrecursorIonN
    End Sub

    Public Iterator Function MS2P(queries As IEnumerable(Of Query)) As IEnumerable(Of Query)
        For Each query As Query In queries
            Dim CmpdTag = query.PeakNO
            Dim DHIonMZ = query.PrecursorIon
            Dim MIonMZ#
            Dim Rsyb$

            If Right(PrecursorIonType, 1) = "-" Then
                MIonMZ = ((DHIonMZ - PrecursorIonMZ) / PrecursorIonN) - H_w + e_w
                Rsyb = "-H]-"
            Else
                MIonMZ = ((DHIonMZ - PrecursorIonMZ) / PrecursorIonN) + H_w - e_w
                Rsyb = "+H]+"
            End If

            If Not query.Ms2Peaks Is Nothing Then
                Call MS2P_MS2Prediction(query, MIonMZ)
            End If

            Yield query
        Next
    End Function

    Private Sub MS2P_MS2Prediction(query As Query, MIonMZ As Double)
        'Predict MS2
        For i As Integer = 0 To query.Candidates.Count - 1
            For Each smile As SMILES In query(i).SMILES
                MS2P_MS2PredictionLoop(query, i, smile, MIonMZ).DoCall(AddressOf query(i).Glycosyl.Add)
            Next
        Next
    End Sub

    Private Function MS2P_MS2PredictionLoop(query As Query, i As Integer, smiles As SMILES, MIonMZ As Double) As Glycosyl
        'Find how many structural possibilites for each peak in 'SMILES' sheet
        Dim RS(,) As String

        'Create a combbox for MS2 prediction results of each combination possibility
        Dim combName = "dd_MS2P_" & CStr(i)
        Dim comb As New List(Of String)
        Dim candidate As CandidateResult = query.Candidate(i)

        'Predict MS2 [MSPrediction()] for each structural possibility
        Dim PredNo As Integer
        Dim Pred_n = 0
        Dim Match_n = 0
        Dim Match_m = 0
        Dim GlycN As String
        Dim Lt As String

        ReDim RS(2, 1)

        '  For Each smiles As SMILES In candidate.SMILES
        Pred_n = Pred_n + 1
        GlycN = smiles.GlycN

        Dim Comma_n = 0
        For e = 1 To Len(GlycN)
            Lt = Mid(GlycN, e, 1)
            If Lt = "," Then Comma_n = Comma_n + 1
        Next e

        RS = MS2P_MS2Prediction_IonPredictionMatching(RS, query.Ms2Peaks, Match_m, Match_n, GlycN, MIonMZ)

        Dim temp = ""

        For l = 1 To Len(smiles.Sequence)
            If Mid(smiles.Sequence, l, 1) = "-" Then Exit For
            temp = temp & Mid(smiles.Sequence, l, 1)
        Next l

        PredNo = CInt(Val(temp))

        'Sort RS() in descending order and write new list to combbox and worksheet
        Dim pResult = ""
        Dim Best_n = 0
        Dim u As Integer
        Dim max_real As Integer

        If Match_m > 0 Then
            For t = 1 To Match_n
                Dim max_temp = -1
                For s = 1 To Match_n
                    If Right(RS(1, s), 1) <> "*" And Val(RS(1, s)) > max_temp Then
                        max_temp = CInt(Val(RS(1, s)))
                        u = s
                    End If
                Next s
                RS(1, u) = RS(1, u) + "*"
                If t = 1 Then max_real = max_temp
                max_real = 1
                If max_temp / max_real = 1 Then Best_n = Best_n + 1
                comb.Add(CStr(Format(max_temp / max_real, "0.00")) & " " & RS(2, u))
                pResult = pResult & CStr(Format(max_temp / max_real, "0.00")) & " " & RS(2, u) & "; "
            Next t
        End If

        Return New Glycosyl With {
            .Match_m = Match_m,
            .Pred_n = Pred_n,
            .pResult = pResult,
            .list = comb.ToArray
        }
    End Function

    Private Function MS2P_MS2Prediction_IonPredictionMatching(RS As String(,), eIonList As Ms2Peaks, ByRef Match_m As Integer, ByRef Match_n As Integer, GlycN As String, MIonMZ As Double) As String(,)

        '1. Declare variables and assign mass of [M-H2O]
        Dim m(,) As String, u(,) As String, Lt As String
        Dim Loss1 As Double, pIonList(,) As Double
        Dim pIonMZ As Double, eIonMZ As Double, eIonInt As Double
        ReDim m(20, 20), u(1, 100)

        Dim f1(1, 100) As Double, f2(1, 100) As Double
        Dim w(5, 100) As Double
        Dim SugComb As String = ""

        '2. Read aglyone/sugar/acid combination and store each component to u()
        Dim Comma_n = 0
        Dim g = 1
        For e = 1 To Len(GlycN)
            Lt = Mid(GlycN, e, 1)
            If Lt = "," And Comma_n = 0 Then
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
        Dim NameComponent As String
        Dim NumDash As Double

        '3. Identify each component, calculate mass, and store value to w()
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

        '4. Fragment each sugar chain forward (NL = sugar portions);
        'calualte mass of each fragment (loss), and store value to f1()
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

        '5. Fragment each sugar chain backward (ion = sugar portions);
        'calualte mass of each fragment (loss), and store value to f1()
        Dim h1 = h + 1

        Dim NameSugar As String = ""
        Dim mass As Double
        Dim f1_temp As Double

        For e = 2 To NumComponent
            NameComponent = u(1, e)
            NumDash = 0
            For g = Len(NameComponent) To 1 Step -1
                NameSugar = Mid(NameComponent, g, 1) & NameSugar
                If Mid(NameComponent, g, 1) = "-" Then NumDash = NumDash + 1
                If NameSugar = "-Hex" Then mass = Hex_w
                If NameSugar = "-HexA" Then mass = HexA_w
                If NameSugar = "-dHex" Then mass = dHex_w
                If NameSugar = "-Pen" Then mass = Pen_w
                If NameSugar = "-Mal" Then mass = Mal_w
                If NameSugar = "-Cou" Then mass = Cou_w
                If NameSugar = "-Fer" Then mass = Fer_w
                If NameSugar = "-Sin" Then mass = Sin_w
                If NameSugar = "-DDMP" Then mass = DDMP_w
                If mass <> 0 Then
                    h = h + 1
                    If NumDash = 1 Then f1_temp = mass
                    If NumDash = 2 Then f1(1, h) = f1_temp + mass - H2O_w
                    If NumDash > 2 Then f1(1, h) = f1(1, h - 1) + mass - H2O_w
                    NameSugar = ""
                    mass = 0
                End If
            Next g
        Next e

        For h2 = h1 To h
            f1(1, h2) = MIonMZ - f1(1, h2) + H_w - e_w
        Next h2

        h1 = h + 1
        For e = 2 To NumComponent
            NameComponent = u(1, e)
            NumDash = 0
            For g = 1 To Len(NameComponent)
                NameSugar = Mid(NameComponent, g, 1) + NameSugar
                If Mid(NameComponent, g, 1) = "-" Then NumDash = NumDash + 1
                If NameSugar = "Hex-" Then mass = Hex_w
                If NameSugar = "HexA-" Then mass = HexA_w
                If NameSugar = "dHex-" Then mass = dHex_w
                If NameSugar = "Pen-" Then mass = Pen_w
                If NameSugar = "Mal-" Then mass = Mal_w
                If NameSugar = "Cou-" Then mass = Cou_w
                If NameSugar = "Fer-" Then mass = Fer_w
                If NameSugar = "Sin-" Then mass = Sin_w
                If NameSugar = "DDMP-" Then mass = DDMP_w
                If mass <> 0 Then
                    h = h + 1
                    If NumDash = 1 Then f1_temp = mass
                    If NumDash = 2 Then f1(1, h) = f1_temp + mass - H2O_w
                    If NumDash > 2 Then f1(1, h) = f1(1, h - 1) + mass - H2O_w
                    NameSugar = ""
                    mass = 0
                End If
            Next g
        Next e

        For h2 = h1 To h
            f1(1, h2) = MIonMZ - f1(1, h2) + H_w - e_w
        Next h2

        '6. Remove duplicates (loss with same mass) in array f1() and create a new list to f2()
        g = 1
        f2(1, 1) = f1(1, 1)
        For e = 1 To h
            For s = 1 To g
                If Int(f1(1, e)) = Int(f2(1, s)) Then GoTo NextOne
            Next s
            g = g + 1
            f2(1, g) = f1(1, e)
NextOne:
        Next e

        '7. Create ion list based on possible sugar/acid losses in f2() and store value to pIonList()
        ReDim pIonList(g, 4)
        For e = 1 To g
            h = 1
            For x = 0 To 1
                For y = 0 To 1
                    If x + y > 2 Then Exit For
                    pIonList(e, h) = MIonMZ - f2(1, e) - x * H2O_w - y * CO2_w
                    h = h + 1
                Next y
            Next x
        Next e

        Dim eIon_n = eIonList.mz.Length
        Dim TotalIonInt As Double = eIonList.TotalIonInt

        '8. Compare pIonList() with eIonlist(), calculate raw score, and save result to RS()
        Dim RawScore As Double = 0
        For e = 1 To g
            For h = 1 To 4
                pIonMZ = pIonList(e, h)
                For s = 0 To eIon_n - 1
                    eIonMZ = eIonList.mz(s)
                    If Math.Abs(pIonMZ - eIonMZ) / pIonMZ * 1000000 < mzPPM Then
                        eIonInt = eIonList.into(s)
                        RawScore = RawScore + Math.Log10(100000 * eIonInt / TotalIonInt)
                        GoTo NextPriIon
                    End If
                Next s
            Next h
NextPriIon:
        Next e

        If RawScore > 0 Then Match_m = Match_m + 1
        Match_n = Match_n + 1
        ReDim Preserve RS(2, Match_n)
        RS(1, Match_n) = CStr(RawScore)
        RS(2, Match_n) = SugComb

        Return RS
    End Function
End Class