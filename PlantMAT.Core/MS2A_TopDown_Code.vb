Module MS2A_TopDown_Code
    

Attribute VB_Name = "MS2A_TopDown_Code"
    'This module performs MS2 annotation

    Sub Button_MS2Annotation()

        'Click to run MS2 annotation; first browse to folder where stores MS2 data
        Dim SelectedFolder As FileDialog
Set SelectedFolder = Application.FileDialog(msoFileDialogFolderPicker)
SelectedFolder.Title = "Select MS2 Folder"
        SelectedFolder.AllowMultiSelect = False
        SelectedFolder.Show
        If SelectedFolder.SelectedItems.Count = 0 Then Exit Sub
        MS2FilePath = SelectedFolder.SelectedItems(1) + "\"
        SingleQ = False

'Peform MS2 annotation and show the calculation progress (MS2A)
'After finished, ask whether to continue MS2 prediction for glycosyl sequencing (MS2P)
Set Query = ThisWorkbook.Sheets("Query")
Set SMILES = ThisWorkbook.Sheets("SMILES")
If Query.Cells(4, 7) <> "" Or Query.Cells(4, 22) <> "" Then
            PublicVS_Code.StartProcessing "Now analyzing, please wait...", "MS2A_TopDown"
   If SMILES.Cells(4, 2) <> "" Then
                a = MsgBox("MS2 annotation finished." & vbNewLine & "Continue glycosyl sequencing?", vbYesNo, "PlantMAT")
                If a = vbYes Then
                    PublicVS_Code.StartProcessing "Now analyzing, please wait...", "MS2P_Code.MS2P"
         MsgBox "Glycosyl sequencing finished", vbInformation, "PlantMAT"
      End If
            Else
                MsgBox "MS2 annotation finished", vbInformation, "PlantMAT"
   End If
        Else
            a = MsgBox("Please run combinatorial enumeration (MS1) first", vbInformation, "PlantMAT")
            Exit Sub
        End If

        ThisWorkbook.Save

    End Sub

    Sub MS2A_TopDown()

        Application.ScreenUpdating = False
        Application.EnableEvents = False

        Dim dd As Object

        'Clear all previous results in the output display
        With Query
            .Unprotect
            LastRow = .Range("D" & Rows.Count).End(xlUp).Row
            If LastRow >= 4 Then
                .Range("V4:" & "W" & LastRow).ClearContents
                For Each dd In .DropDowns()
                    If Left(dd.Name, 7) = "dd_MS2A" Then dd.Delete
                Next dd
            End If
            .ScrollArea = ""
        End With

        'Read the parameters in Settings (module: PublicVS_Code)
        Call PublicVS_Code.Settings_Check
        Call PublicVS_Code.Settings_Reading

        i = 4

        'Loop through all compounds and do MS2 annotation for each
        Do While Query.Cells(i, 4) <> ""
            DoEvents

            'Skip the compound if there are no hits from combinatorial enumeration
            Do While Query.Cells(i, 7) = "No hits"
                i = i + 1
            Loop

            'If this is the last + 1 cell, then exit the loop
            If Query.Cells(i, 4) = "" Then Exit Do

            'Read compound serial number and precuror ion mz
            With Query
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
                    Do While Query.Cells(i, 4) = "..."
                        i = i + 1
                    Loop
                Else
                    Call MS2A_TopDown_MS2Annotation()
                End If
            End If
        Loop

        'Go to the top of spreadsheet and lock (protect) the spreadsheet
        With Query
            Application.Goto.Range("A1"), True
     .ScrollArea = "A4:Z" & CStr(i + 1)
            .Protect
        End With

        Application.EnableEvents = True
        Application.ScreenUpdating = True

    End Sub

    Sub MS2File_Searching()

        On Error GoTo ErrorHandler

        Dim File As Variant
        Dim MS2FileName As String

        FileCheck = False
        ErrorCheck = False

        File = Dir(MS2FilePath)
        MS2FileName = CStr(CmpdTag) + ".txt"

        'Find MS2 data for each compound and read into data array eIonList()
        While (File <> "")
            DoEvents

            If InStr(File, MS2FileName) = 1 Then
                FileCheck = True
                eIon_n = 0
                TotalIonInt = 0
                ReDim eIonList(1 To 2, 1 To 1)
                Open MS2FilePath + MS2FileName For Input As #1

        While Not EOF(1)
                    DoEvents

                    Line Input #1, LineText
            eIon = Split(CStr(LineText), Chr(9))
                    DaughterIonMZ = eIon(0)
                    DaughterIonInt = eIon(1)
                    TotalIonInt = TotalIonInt + DaughterIonInt
                    If DaughterIonMZ = 0 Then Exit Sub

                    eIon_n = eIon_n + 1
                    ReDim Preserve eIonList(1 To 2, 1 To eIon_n)
                    eIonList(1, eIon_n) = DaughterIonMZ
                    eIonList(2, eIon_n) = DaughterIonInt
        Wend

        Close #1
    End If

            File = Dir()
Wend

Exit Sub

        'If error is found, go to ErrorHandler
ErrorHandler:
        FileCheck = False
        ErrorCheck = True
        Close #1

End Sub

    Sub MS2A_TopDown_MS2Annotation()

        'Loop through all candidates for each compound
        Do While True
            DoEvents

            'Read the results from combinatorial enumeration
            With Query
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
            With Query.Cells(i, 23)
        Set comb = Query.DropDowns.Add(.Left, .Top, .Width, .Height)
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
                    comb.AddItem CStr(Format(aIonAbu, "0.000")) & " " & aIonNM
          aResult = aResult & CStr(Format(aIonMZ, "0.0000")) & ", " &
                    CStr(Format(aIonAbu * 100, "0.00")) & ", " & aIonNM & "; "
                Next s
            End If

            comb.Text = CStr(aIon_n) & " ions annotated"

            'Fifth, show an asterisk mark if the ions corresponding to the aglycone are found
            With Query
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
            If Query.Cells(i, 4) <> "..." Then Exit Sub
        Loop

    End Sub

    Sub MS2A_TopDown_MS2Annotation_IonPrediction()

        'Calcualte the total number of glycosyl and acyl groups allowed in the brute iteration
        Total_max = Hex_max + HexA_max + dHex_max + Pen_max + Mal_max + Cou_max + Fer_max + Sin_max + DDMP_max

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
        Total_n = Hex_n + HexA_n + dHex_n + Pen_n + Mal_n + Cou_n + Fer_n + Sin_n + DDMP_n

        'Calculate the mass of the predicte neutral loss
        Loss_w = Hex_n * Hex_w + HexA_n * HexA_w + dHex_n * dHex_w + Pen_n * Pen_w +
                 Mal_n * Mal_w + Cou_n * Cou_w + Fer_n * Fer_w + Sin_n * Sin_w + DDMP_n * DDMP_w -
                 Total_n * H2O_w + H2O_n * H2O_w + CO2_n * CO2_w

        'Calculate the precuror ion mz based on the calcualted loss mass
        pIonMZ = MIonMZ - Loss_w

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
            eIonMZ = eIonList(1, s)
            eIonInt = eIonList(2, s)
            For t = 1 To pIon_n
                pIonMZ = pIonList(1, t)
                pIonNM = pIonList(2, t)
                If Abs((eIonMZ - pIonMZ) / pIonMZ) * 1000000 <= mzPPM Then
                    aIonAbu = eIonInt / TotalIonInt
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
