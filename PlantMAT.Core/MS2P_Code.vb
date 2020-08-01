Module MS2P_Code


    ' Attribute VB_Name = "MS2P_Code"
    Sub MS2P()

        Application.ScreenUpdating = False
        Application.EnableEvents = False

        Dim dd As Object

        With Query
            .Unprotect
            LastRow = .Range("D" & Rows.Count).End(xlUp).Row
            If LastRow >= 4 Then
                .Range("Y4:" & "Z" & LastRow).ClearContents
                For Each dd In .DropDowns()
                    If Left(dd.Name, 7) = "dd_MS2P" Then dd.Delete
                Next dd
            End If
            .ScrollArea = ""
        End With

        Call PublicVS_Code.Settings_Check
        Call PublicVS_Code.Settings_Reading

        i = 4

        Do While Query.Cells(i, 4) <> ""
            DoEvents

            Do While Query.Cells(i, 7) = "No hits"
                i = i + 1
                k = k + 1
            Loop

            If Query.Cells(i, 4) = "" Then Exit Do

            k = 1

            With Query
                CmpdTag = .Cells(i, 2)
                DHIonMZ = .Cells(i, 4)
            End With

            If Right(PrecursorIonType, 1) = "-" Then
                MIonMZ = ((DHIonMZ - PrecursorIonMZ) / PrecursorIonN) - H_w + e_w
                Rsyb = "-H]-"
            Else
                MIonMZ = ((DHIonMZ - PrecursorIonMZ) / PrecursorIonN) + H_w - e_w
                Rsyb = "+H]+"
            End If

            If SingleQ = True Then
                Call MS2P_MS2Prediction()
            Else
                Call MS2File_Searching()

                If FileCheck = False And Query.Cells(i, 4) <> "" Then
                    With Query
                        If ErrorCheck = True Then
                            .Cells(i, 22) = "Data error"
                        Else
                            .Cells(i, 22) = "Data not found"
                        End If
                        .Cells(i, 22).HorizontalAlignment = xlLeft
                        .Cells(i, 22).Font.Color = RGB(217, 217, 217)
                    End With
                    i = i + 1
                    k = k + 1
                    Do While Query.Cells(i, 4) = "..."
                        i = i + 1
                        k = k + 1
                    Loop
                Else
                    Call MS2P_MS2Prediction()
                End If
            End If
        Loop

        With Query
            Application.Goto.Range("A1"), True
     .ScrollArea = "A4:Z" & CStr(i + 1)
            .Protect
        End With

        Application.EnableEvents = True
        Application.ScreenUpdating = True

    End Sub

    Sub MS2P_MS2Prediction()

        'Find how many structural possibilites for each peak in 'SMILES' sheet
        r = 3

        Do While True
            PeakNo = SMILES.Cells(r, 2)
            If PeakNo = 0 Or PeakNo = CmpdTag Then Exit Do
            r = r + 1
        Loop

        'Predict MS2
        Do While True
            DoEvents

            AglyMass = Val(Query.Cells(i, 7).Comment.Text)

            'Create a combbox for MS2 prediction results of each combination possibility
            With Query.Cells(i, 26)
        Set comb = Query.DropDowns.Add(.Left, .Top, .Width, .Height)
        comb.Name = "dd_MS2P_" & CStr(i)
            End With

            'Predict MS2 [MSPrediction()] for each structural possibility
            PredNo = k
            Pred_n = 0
            Match_n = 0
            Match_m = 0
            ReDim RS(2, 1)

            With SMILES
                Do While PeakNo = CmpdTag And PredNo = k
                    DoEvents

                    Pred_n = Pred_n + 1
                    GlycN = .Cells(r, 4)

                    Comma_n = 0
                    For e = 1 To Len(GlycN)
                        Lt = Mid(GlycN, e, 1)
                        If Lt = "," Then Comma_n = Comma_n + 1
                    Next e

                    Call MS2P_MS2Prediction_IonPredictionMatching()

                    r = r + 1
                    PeakNo = .Cells(r, 2)
                    temp = ""

                    For l = 1 To Len(.Cells(r, 3))
                        If Mid(.Cells(r, 3), l, 1) = "-" Then Exit For
                        temp = temp + Mid(.Cells(r, 3), l, 1)
                    Next l

                    PredNo = Val(temp)
                Loop
            End With

            'Sort RS() in descending order and write new list to combbox and worksheet
            pResult = ""
            Best_n = 0

            If Match_m > 0 Then
                For t = 1 To Match_n
                    max_temp = -1
                    For s = 1 To Match_n
                        If Right(RS(1, s), 1) <> "*" And Val(RS(1, s)) > max_temp Then
                            max_temp = Val(RS(1, s))
                            u = s
                        End If
                    Next s
                    RS(1, u) = RS(1, u) + "*"
                    If t = 1 Then max_real = max_temp
                    max_real = 1
                    If max_temp / max_real = 1 Then Best_n = Best_n + 1
                    comb.AddItem CStr(Format(max_temp / max_real, "0.00")) & " " & RS(2, u)
          pResult = pResult & CStr(Format(max_temp / max_real, "0.00")) & " " & RS(2, u) & "; "
                Next t
            End If

            comb.Text = CStr(Match_m) & "/" & CStr(Pred_n) & " candidates"

            With Query
                If .Cells(i, 22) = "*" Then
                    .Cells(i, 25) = "*"
                    .Cells(i, 25).HorizontalAlignment = xlCenter
                    .Cells(i, 25).Font.Color = RGB(118, 147, 60)
                End If
                If Match_n > 0 And Match_m > 0 Then
                    .Cells(i, 26) = CStr(Match_m) & "/" & CStr(Pred_n) & " candidates: " &
                                    Left(pResult, Len(pResult) - 2)
                    .Cells(i, 26).Font.Color = RGB(255, 255, 255)
                    .Cells(i, 26).HorizontalAlignment = xlFill
                End If
            End With

            i = i + 1
            k = k + 1

            If Query.Cells(i, 4) <> "..." Then Exit Sub
        Loop

    End Sub

    Sub MS2P_MS2Prediction_IonPredictionMatching()

        '1. Declare variables and assign mass of [M-H2O]
        Dim m() As String, u() As String, Lt As String
        Dim n1() As Double, n2() As Double, w() As Double
        Dim Loss As Double, Loss1 As Double, pIonList() As Double
        Dim pIonMZ As Double, eIonMZ As Double, eIonInt As Double
        ReDim m(20, 20), u(1, 100)
        ReDim f1(1, 100), f2(1, 100), w(5, 100)

        '2. Read aglyone/sugar/acid combination and store each component to u()
        Comma_n = 0
        g = 1
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

        NumComponent = g

        '3. Identify each component, calculate mass, and store value to w()
        Lt = ""
        For e = 2 To g
            s = 1
            For h = Len(u(1, e)) To 1 Step -1
                Lt = Mid(u(1, e), h, 1)
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
            Next h
        Next e

        '4. Fragment each sugar chain forward (NL = sugar portions);
        'calualte mass of each fragment (loss), and store value to f1()
        h = 0
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
                        Loss2 = f1(1, h)
                        For c3 = c2 + 1 To 5
                            For c3f = 1 To 100
                                If w(c3, c3f) = 0 Then Exit For
                                h = h + 1
                                f1(1, h) = Loss2 + w(c3, c3f)
                                Loss3 = f1(1, h)
                                For c4 = c3 + 1 To 5
                                    For c4f = 1 To 100
                                        If w(c4, c4f) = 0 Then Exit For
                                        h = h + 1
                                        f1(1, h) = Loss3 + w(c4, c4f)
                                        Loss4 = f1(1, h)
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
        h1 = h + 1
        For e = 2 To NumComponent
            NameComponent = u(1, e)
            NumDash = 0
            For g = Len(NameComponent) To 1 Step -1
                NameSugar = Mid(NameComponent, g, 1) + NameSugar
                If Mid(NameComponent, g, 1) = "-" Then NumDash = NumDash + 1
                If NameSugar = "-Hex" Then Mass = Hex_w
                If NameSugar = "-HexA" Then Mass = HexA_w
                If NameSugar = "-dHex" Then Mass = dHex_w
                If NameSugar = "-Pen" Then Mass = Pen_w
                If NameSugar = "-Mal" Then Mass = Mal_w
                If NameSugar = "-Cou" Then Mass = Cou_w
                If NameSugar = "-Fer" Then Mass = Fer_w
                If NameSugar = "-Sin" Then Mass = Sin_w
                If NameSugar = "-DDMP" Then Mass = DDMP_w
                If Mass <> 0 Then
                    h = h + 1
                    If NumDash = 1 Then f1_temp = Mass
                    If NumDash = 2 Then f1(1, h) = f1_temp + Mass - H2O_w
                    If NumDash > 2 Then f1(1, h) = f1(1, h - 1) + Mass - H2O_w
                    NameSugar = ""
                    Mass = 0
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
                If NameSugar = "Hex-" Then Mass = Hex_w
                If NameSugar = "HexA-" Then Mass = HexA_w
                If NameSugar = "dHex-" Then Mass = dHex_w
                If NameSugar = "Pen-" Then Mass = Pen_w
                If NameSugar = "Mal-" Then Mass = Mal_w
                If NameSugar = "Cou-" Then Mass = Cou_w
                If NameSugar = "Fer-" Then Mass = Fer_w
                If NameSugar = "Sin-" Then Mass = Sin_w
                If NameSugar = "DDMP-" Then Mass = DDMP_w
                If Mass <> 0 Then
                    h = h + 1
                    If NumDash = 1 Then f1_temp = Mass
                    If NumDash = 2 Then f1(1, h) = f1_temp + Mass - H2O_w
                    If NumDash > 2 Then f1(1, h) = f1(1, h - 1) + Mass - H2O_w
                    NameSugar = ""
                    Mass = 0
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

        '8. Compare pIonList() with eIonlist(), calculate raw score, and save result to RS()
        RawScore = 0
        For e = 1 To g
            For h = 1 To 4
                pIonMZ = pIonList(e, h)
                For s = 1 To eIon_n
                    eIonMZ = eIonList(1, s)
                    If Abs(pIonMZ - eIonMZ) / pIonMZ * 1000000 < mzPPM Then
                        eIonInt = eIonList(2, s)
                        RawScore = RawScore + WorksheetFunction.Log10(100000 * eIonInt / TotalIonInt)
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

    End Sub
End Module
