Module ErrorHandler

    ' Attribute VB_Name = "ErrorHandler"
    Sub ErrorHanlder()

        'If data error is found in the peak list, then show warning message and
        'reset all fields in the output display
        MsgBox "Import failed. Please check your data and try again.", vbCritical, "PlantMAT"

    Application.ScreenUpdating = False
    Set Query = ThisWorkbook.Sheets("Query")
    With Query
            .Unprotect
            LastRow = .Range("B" & Rows.Count).End(xlUp).Row
            If LastRow >= 4 Then .Range("B4:" & "Z" & LastRow) = ""
            .Cells.ClearComments
            .DropDowns().Delete
            .ScrollArea = ""
            For i = 8 To 16
                Query.Columns(i).Hidden = True
            Next i
            .Shapes("bt_MS1").OnAction = Empty()
            .Shapes("bt_MS1").DrawingObject.Font.ColorIndex = 16
            .Shapes("bt_MS2A").OnAction = Empty()
            .Shapes("bt_MS2A").DrawingObject.Font.ColorIndex = 16
            Application.Goto.Range("A1"), True
        .ScrollArea = "A4:Z4"
            .Protect
        End With
    Set SMILES = ThisWorkbook.Sheets("SMILES")
    With SMILES
            .Unprotect
            LastRow = .Range("B" & Rows.Count).End(xlUp).Row
            If LastRow >= 3 Then .Range("B3:" & "E" & LastRow) = ""
            .Protect
        End With
        ThisWorkbook.Save
        Query.Activate
        Application.ScreenUpdating = True

    End Sub
End Module