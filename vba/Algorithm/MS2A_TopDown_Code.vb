﻿
''' <summary>
''' This module performs MS2 annotation
''' </summary>
Module MS2A_TopDown_Code

    Sub Button_MS2Annotation(MS2FilePath As String)

        'Click to run MS2 annotation; first browse to folder where stores MS2 data
        'Dim SelectedFolder As FileDialog
        'SelectedFolder = Application.FileDialog(msoFileDialogFolderPicker)
        'SelectedFolder.Title = "Select MS2 Folder"
        'SelectedFolder.AllowMultiSelect = False
        'SelectedFolder.Show
        'If SelectedFolder.SelectedItems.Count = 0 Then Exit Sub
        'MS2FilePath = SelectedFolder.SelectedItems(1) + "\"
        SingleQ = False

        'Peform MS2 annotation and show the calculation progress (MS2A)
        'After finished, ask whether to continue MS2 prediction for glycosyl sequencing (MS2P)
        '   PublicVS_Code.Query = ThisWorkbook.Sheets("Query")
        '  SMILES = ThisWorkbook.Sheets("SMILES")
        If PublicVS_Code.Query.Cells(4, 7) <> "" Or PublicVS_Code.Query.Cells(4, 22) <> "" Then
            PublicVS_Code.StartProcessing("Now analyzing, please wait...", AddressOf MS2A_TopDown)
            If SMILES.Cells(4, 2) <> "" Then
                Console.WriteLine("MS2 annotation finished." & vbNewLine & "Continue glycosyl sequencing")

                PublicVS_Code.StartProcessing("Now analyzing, please wait...", AddressOf MS2P_Code.MS2P)
                Console.WriteLine("Glycosyl sequencing finished")

            Else
                Console.WriteLine("MS2 annotation finished")
            End If
        Else
            Console.WriteLine("Please run combinatorial enumeration (MS1) first")
        End If

        ' ThisWorkbook.Save

    End Sub

    Sub MS2A_TopDown()

        'Application.ScreenUpdating = False
        'Application.EnableEvents = False

        Dim dd As Object

        'Clear all previous results in the output display
        With PublicVS_Code.Query
            Call .Unprotect
            LastRow = .Range("D" & Rows.Count).End(xlUp).Row
            If LastRow >= 4 Then
                Call .Range("V4:" & "W" & LastRow).ClearContents
                For Each dd In .DropDowns()
                    If Left(dd.Name, 7) = "dd_MS2A" Then dd.Delete
                Next dd
            End If
            .ScrollArea = ""
        End With

        'Read the parameters in Settings (module: PublicVS_Code)
        Call PublicVS_Code.Settings_Check()
        Call PublicVS_Code.Settings_Reading()

        i = 4

        'Loop through all compounds and do MS2 annotation for each
        Do While PublicVS_Code.Query.Cells(i, 4) <> ""
            ' DoEvents

            'Skip the compound if there are no hits from combinatorial enumeration
            Do While PublicVS_Code.Query.Cells(i, 7) = "No hits"
                i = i + 1
            Loop

            'If this is the last + 1 cell, then exit the loop
            If PublicVS_Code.Query.Cells(i, 4) = "" Then Exit Do

            'Read compound serial number and precuror ion mz
            With PublicVS_Code.Query
                CmpdTag = .Cells(i, 2)
                DHIonMZ = .Cells(i, 4)
            End With

            'Find the ion type (pos or neg) based on the setting
            If Right(PrecursorIonType, 1) = "-" Then
                IonMZ_crc = e_w - H_w
                Rsyb = "-H]-"
            Else
                IonMZ_crc = H_w - e_w
                Rsyb = "+H]+"
            End If

            'Perform the MS2 annotation and display the results
            If SingleQ = True Then
                Call MS2A_TopDown_MS2Annotation()
            Else
                Call MS2File_Searching()

                If FileCheck = False And PublicVS_Code.Query.Cells(i, 4) <> "" Then
                    With PublicVS_Code.Query
                        If ErrorCheck = True Then
                            .Cells(i, 22) = "Data error"
                        Else
                            .Cells(i, 22) = "Data not found"
                        End If
                        .Cells(i, 22).HorizontalAlignment = xlLeft
                        .Cells(i, 22).Font.Color = RGB(217, 217, 217)
                    End With
                    i = i + 1
                    Do While PublicVS_Code.Query.Cells(i, 4) = "..."
                        i = i + 1
                    Loop
                Else
                    Call MS2A_TopDown_MS2Annotation()
                End If
            End If
        Loop

        'Go to the top of spreadsheet and lock (protect) the spreadsheet
        With PublicVS_Code.Query
            Application.Goto.Range("A1"), True
     .ScrollArea = "A4:Z" & CStr(i + 1)
            Call .Protect
        End With

        'Application.EnableEvents = True
        'Application.ScreenUpdating = True

    End Sub

    Sub MS2File_Searching()

        'On Error GoTo ErrorHandler


        Dim MS2FileName As String

        FileCheck = False
        ErrorCheck = False


        MS2FileName = CStr(CmpdTag) & ".txt"

        'Find MS2 data for each compound and read into data array eIonList()
        For Each file As String In MS2FilePath.ListDirectory


            If InStr(file, MS2FileName) = 1 Then
                FileCheck = True
                eIon_n = 0
                TotalIonInt = 0
                ReDim eIonList(1 To 2, 1 To 1)
                For Each lineText As String In (MS2FilePath & "/" & MS2FileName).IterateAllLines

                    Dim eIon = Strings.Split(CStr(lineText), Chr(9))
                    DaughterIonMZ = eIon(0)
                    DaughterIonInt = eIon(1)
                    TotalIonInt = TotalIonInt + DaughterIonInt
                    If DaughterIonMZ = 0 Then Exit Sub

                    eIon_n = eIon_n + 1
                    ReDim Preserve eIonList(1 To 2, 1 To eIon_n)
                    eIonList(1, eIon_n) = DaughterIonMZ
                    eIonList(2, eIon_n) = DaughterIonInt
                Next
            End If

        Next

    End Sub

    Sub MS2A_TopDown_MS2Annotation()

        'Loop through all candidates for each compound
        Do While True
            ' DoEvents

            'Read the results from combinatorial enumeration
            With PublicVS_Code.Query
                AglyN = .Cells(i, 7)
                Agly_w = Val(.Cells(i, 7).Comment.Text)
                Hex_max = .Cells(i, 8)
                HexA_max = .Cells(i, 9)
                dHex_max = .Cells(i, 10)
                Pen_max = .Cells(i, 11)
                Mal_max = .Cells(i, 12)
                Cou_max = .Cells(i, 13)
                Fer_max = .Cells(i, 14)
                Sin_max = .Cells(i, 15)
                DDMP_max = .Cells(i, 16)
            End With

            'First, predict the ions based on the results from combinatorial enumeration
            Call MS2A_TopDown_MS2Annotation_IonPrediction()

            'Second, compare the predicted ions with the measured
            Call MS2A_TopDown_MS2Annotation_IonMatching()

            'Third, add a dropdown list for each candidate and show the annotation results in the list
            With PublicVS_Code.Query.Cells(i, 23)
                comb = PublicVS_Code.Query.DropDowns.Add(.Left, .Top, .Width, .Height)
                comb.Name = "dd_MS2A_TopDown_" & CStr(i)
            End With

            'Fourth, save the annotation results in the cell
            Dim aResult As String
            aResult = ""

            If aIon_n > 0 Then
                For s = 1 To aIon_n
                    aIonMZ = aIonList(1, s)
                    aIonAbu = aIonList(2, s)
                    aIonNM = aIonList(3, s)
                    comb.AddItem(CStr(Format(aIonAbu, "0.000")) & " " & aIonNM)
                    aResult = aResult & CStr(Format(aIonMZ, "0.0000")) & ", " &
                    CStr(Format(aIonAbu * 100, "0.00")) & ", " & aIonNM & "; "
                Next s
            End If

            comb.Text = CStr(aIon_n) & " ions annotated"

            'Fifth, show an asterisk mark if the ions corresponding to the aglycone are found
            With PublicVS_Code.Query
                If AglyCheck = True Then
                    .Cells(i, 22) = "*"
                    .Cells(i, 22).HorizontalAlignment = xlCenter
                    .Cells(i, 22).Font.Color = RGB(118, 147, 60)
                End If
                If aIon_n > 0 Then
                    .Cells(i, 23) = CStr(aIon_n) & " ions annotated: " & Left(aResult, Len(aResult) - 2)
                    .Cells(i, 23).Font.Color = RGB(255, 255, 255)
                    .Cells(i, 23).HorizontalAlignment = xlFill
                End If
            End With

            i = i + 1

            'If the last candidate has been analyzed, then exit the loop and go to the next compound
            If PublicVS_Code.Query.Cells(i, 4) <> "..." Then Exit Sub
        Loop

    End Sub

    Sub MS2A_TopDown_MS2Annotation_IonPrediction()

        'Calcualte the total number of glycosyl and acyl groups allowed in the brute iteration
        Dim Total_max = Hex_max + HexA_max + dHex_max + Pen_max + Mal_max + Cou_max + Fer_max + Sin_max + DDMP_max

        'Calculate the the mass of precursor ion
        MIonMZ = Agly_w + Hex_max * Hex_w + HexA_max * HexA_w + dHex_max * dHex_w + Pen_max * Pen_w +
                 Mal_max * Mal_w + Cou_max * Cou_w + Fer_max * Fer_w + Sin_max * Sin_w + DDMP_max * DDMP_w -
                 Total_max * H2O_w + IonMZ_crc

        'Initilize all neutral losses and predicted ions pIonList() to none
        HexLoss = ""
        HexALoss = ""
        dHexLoss = ""
        PenLoss = ""
        MalLoss = ""
        CouLoss = ""
        FerLoss = ""
        SinLoss = ""
        DDMPLoss = ""
        H2OLoss = ""
        CO2Loss = ""

        pIon_n = 0
        ReDim pIonList(1 To 2, 1 To 1)

        'Do brute force iteration to generate all hypothetical neutral losses
        'as a combination of different glycosyl and acyl groups, and
        'for each predicted neutral loss, calcualte the ion mz
        For Hex_n = 0 To Hex_max
            For HexA_n = 0 To HexA_max
                For dHex_n = 0 To dHex_max
                    For Pen_n = 0 To Pen_max
                        For Mal_n = 0 To Mal_max
                            For Cou_n = 0 To Cou_max
                                For Fer_n = 0 To Fer_max
                                    For Sin_n = 0 To Sin_max
                                        For DDMP_n = 0 To DDMP_max
                                            For H2O_n = 0 To 1
                                                For CO2_n = 0 To 1

                                                    Call MS2A_TopDown_MS2Annotation_IonPrediction_LossCombination()

                                                    CO2Loss = CO2Loss + "-CO2"
                                                Next CO2_n
                                                CO2Loss = ""
                                                H2OLoss = H2OLoss + "-H2O"
                                            Next H2O_n
                                            H2OLoss = ""
                                            DDMPLoss = DDMPLoss + "-DDMP"
                                        Next DDMP_n
                                        DDMPLoss = ""
                                        SinLoss = SinLoss + "-Sin"
                                    Next Sin_n
                                    SinLoss = ""
                                    FerLoss = FerLoss + "-Fer"
                                Next Fer_n
                                FerLoss = ""
                                CouLoss = CouLoss + "-Cou"
                            Next Cou_n
                            CouLoss = ""
                            MalLoss = MalLoss + "-Mal"
                        Next Mal_n
                        MalLoss = ""
                        PenLoss = PenLoss + "-Pen"
                    Next Pen_n
                    PenLoss = ""
                    dHexLoss = dHexLoss + "-dHex"
                Next dHex_n
                dHexLoss = ""
                HexALoss = HexALoss + "-HexA"
            Next HexA_n
            HexALoss = ""
            HexLoss = HexLoss + "-Hex"
        Next Hex_n

    End Sub

    Sub MS2A_TopDown_MS2Annotation_IonPrediction_LossCombination()

        'Calculate the total number of glycosyl and acyl groups in the predicted neutral loss
        Dim Total_n = Hex_n + HexA_n + dHex_n + Pen_n + Mal_n + Cou_n + Fer_n + Sin_n + DDMP_n

        'Calculate the mass of the predicte neutral loss
        Dim Loss_w = Hex_n * Hex_w + HexA_n * HexA_w + dHex_n * dHex_w + Pen_n * Pen_w +
                 Mal_n * Mal_w + Cou_n * Cou_w + Fer_n * Fer_w + Sin_n * Sin_w + DDMP_n * DDMP_w -
                 Total_n * H2O_w + H2O_n * H2O_w + CO2_n * CO2_w

        'Calculate the precuror ion mz based on the calcualted loss mass
        Dim pIonMZ = MIonMZ - Loss_w
        Dim pIonNM As String

        'Find if the ion is related to the H2O/CO2 loss from aglycone
        If Hex_n = Hex_max And HexA_n = HexA_max And dHex_n = dHex_max And Pen_n = Pen_max And
           Mal_n = Mal_max And Cou_n = Cou_max And Fer_n = Fer_max And Sin_n = Sin_max And DDMP_n = DDMP_max Then
            pIonNM = "[Agly" & H2OLoss & CO2Loss & Rsyb
            If H2OLoss & CO2Loss = "" Or (H2OLoss & CO2Loss = "-H2O-CO2" And
               (AglyN = "Medicagenic acid" Or AglyN = "Zanhic acid")) Then
                pIonNM = "*" & pIonNM
            End If
        Else
            pIonNM = "[M" & HexLoss & HexALoss & dHexLoss & PenLoss &
                            MalLoss & CouLoss & FerLoss & SinLoss & DDMPLoss &
                            H2OLoss & CO2Loss & Rsyb
        End If

        'Save the predicted ion mz to data array pIonList()
        pIon_n = pIon_n + 1
        ReDim Preserve pIonList(1 To 2, 1 To pIon_n)
        pIonList(1, pIon_n) = pIonMZ
        pIonList(2, pIon_n) = pIonNM

    End Sub

    Sub MS2A_TopDown_MS2Annotation_IonMatching()

        'Initialize the annotated ion list aIonList() to none
        aIon_n = 0
        AglyCheck = False
        ReDim aIonList(1 To 3, 1 To 1)

        'Compare the measured ions eIonList() with the predicted pIonList()
        'If the mz error is less than the defined ppm and intensity is above the noise filter, then
        'save the predicted ions in the annotation ion list aIonList()
        For s = 1 To eIon_n
            Dim eIonMZ = eIonList(1, s)
            Dim eIonInt = eIonList(2, s)
            For t = 1 To pIon_n
                Dim pIonMZ = pIonList(1, t)
                Dim pIonNM = pIonList(2, t)
                If Math.Abs((eIonMZ - pIonMZ) / pIonMZ) * 1000000 <= mzPPM Then
                    Dim aIonAbu = eIonInt / TotalIonInt
                    If aIonAbu * 100 >= NoiseFilter Then
                        aIon_n = aIon_n + 1
                        ReDim Preserve aIonList(1 To 3, 1 To aIon_n)
                        aIonList(1, aIon_n) = eIonMZ
                        aIonList(2, aIon_n) = aIonAbu
                        aIonList(3, aIon_n) = pIonNM
                        If Left(pIonNM, 1) = "*" Then AglyCheck = True
                    End If
                End If
            Next t
        Next s

    End Sub
End Module