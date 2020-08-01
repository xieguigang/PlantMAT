Module SingleQ_Dialog
    

Attribute VB_Name = "SingleQ_Dialog"
Attribute VB_Base = "0{F7FDB3D4-9318-45DB-B934-5D5DDEC837BA}{384E67FB-16E7-4966-BAE3-45E74A92352C}"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Attribute VB_TemplateDerived = False
Attribute VB_Customizable = False
    Private Sub bt_Cancel_Click()

        Unload SingleQ_Dialog

End Sub

    Private Sub bt_OK_Click()

        On Error GoTo ErrorHandler

        Application.ScreenUpdating = False
        Application.EnableEvents = False

        With SingleQ_Dialog
            MZ = .tb_MZ.Value
            MS2Data = .tb_MS2.Value
            IDApproach = "TopDown"
            MS2Annotation = .cb_MS2Annotation.Value
            MS2Prediction = .cb_MS2Prediction.Value
        End With

        If IsNumeric(MZ) = False Or MZ = "" Or IsNumeric(MZ) = False Or Val(MZ) <= 0 Then
            MsgBox "Data incorrect", vbCritical, "PlantMAT"
   Exit Sub
        End If

        If MS2Data = "" Then
            If IDApproach = "BottomUp" Or MS2Annotation = True Or MS2Prediction = True Then
                MsgBox "MS/MS data empty", vbCritical, "PlantMAT"
      Exit Sub
            End If
        End If

        If MS2Data <> "" Then
            SingleQ = True
            TotalIonInt = 0
            Dim EachLine() As String
            Dim EachIon() As String
            EachLine = Split(MS2Data, Chr(10))
            eIon_n = UBound(EachLine) + 1
            ReDim eIonList(1 To 2, 1 To eIon_n)
            For i = 0 To eIon_n - 1
                EachIon = Split(EachLine(i), " ")
                eIonList(1, i + 1) = Val(EachIon(0))
                eIonList(2, i + 1) = Val(EachIon(1))
                TotalIonInt = TotalIonInt + Val(EachIon(1))
            Next i
        End If

        Unload SingleQ_Dialog

Set Query = ThisWorkbook.Sheets("Query")
With Query
            .Unprotect
            .Cells.ClearComments
            LastRow = .Range("D" & Rows.Count).End(xlUp).Row
            If LastRow >= 4 Then .Range("B4:" & "Z" & LastRow) = ""
            .DropDowns().Delete
            .Cells(4, 2) = 1
            .Cells(4, 3) = RT
            .Cells(4, 4) = MZ
            .Cells(4, 5) = Formula
            .Protect
        End With

        If IDApproach = "TopDown" Then
            PublicVS_Code.StartProcessing "Now analyzing, please wait...", "MS1_TopDown_Code.MS1CP"
Else
            PublicVS_Code.StartProcessing "Now analyzing, please wait...", "MS2A_BottomUp_Code.MS2A_BottomUp"
End If

        If MS2Annotation = True Then
            PublicVS_Code.StartProcessing "Now analyzing, please wait...", "MS2A_TopDown_Code.MS2A_TopDown"
End If

        If MS2Prediction = True Then
            PublicVS_Code.StartProcessing "Now analyzing, please wait...", "MS2P_Code.MS2P"
End If

        With Query
            .Unprotect
            .Shapes("bt_MS1").OnAction = Empty()
            .Shapes("bt_MS1").DrawingObject.Font.ColorIndex = 16
            .Shapes("bt_MS2A").OnAction = Empty()
            .Shapes("bt_MS2A").DrawingObject.Font.ColorIndex = 16
            .Protect
        End With

        ThisWorkbook.Save

        MsgBox "Annotation finished", vbInformation, "PlantMAT"

Exit Sub

ErrorHandler:
        MsgBox "Data incorrect", vbCritical, "PlantMAT"

End Sub
End Module
