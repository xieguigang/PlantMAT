Module Edit_Code


    ''Attribute VB_Name = "Edit_Code"
    'Sub Button_Reset()

    '    'Reset (Clear) all fields in the output display; Ask to confirm before resetting
    '    a = MsgBox("Reset all fields?", vbYesNo, "PlantMAT")

    '    If a = vbYes Then
    '        Application.ScreenUpdating = False
    '    Set Query = ThisWorkbook.Sheets("Query")
    '    With Query
    '            .Unprotect
    '            LastRow = .Range("D" & Rows.Count).End(xlUp).Row
    '            If LastRow >= 4 Then .Range("B4:" & "Z" & LastRow) = ""
    '            .Cells.ClearComments
    '            .DropDowns().Delete
    '            .ScrollArea = ""
    '            For i = 8 To 16
    '                Query.Columns(i).Hidden = True
    '            Next i
    '            .Shapes("bt_MS1").OnAction = Empty()
    '            .Shapes("bt_MS1").DrawingObject.Font.ColorIndex = 16
    '            .Shapes("bt_MS2A").OnAction = Empty()
    '            .Shapes("bt_MS2A").DrawingObject.Font.ColorIndex = 16
    '            Application.Goto.Range("A1"), True
    '        .ScrollArea = "A4:Z4"
    '            .Protect
    '        End With
    '    Set SMILES = ThisWorkbook.Sheets("SMILES")
    '    With SMILES
    '            .Unprotect
    '            LastRow = .Range("B" & Rows.Count).End(xlUp).Row
    '            If LastRow >= 3 Then .Range("B3:" & "E" & LastRow) = ""
    '            .Protect
    '        End With
    '        ThisWorkbook.Save
    '        Query.Activate
    '        Application.ScreenUpdating = True
    '    End If

    'End Sub

    '    Sub Button_Export()

    ''Export the results to CSV file
    '    Set Query = ThisWorkbook.Sheets("Query")

    '    If Query.Range("G4") = "" Then
    '            MsgBox "No results to export", vbCritical, "PlantMAT"
    '        Exit Sub
    '        End If

    '        i = Query.Range("G" & Rows.Count).End(xlUp).Row

    '        Call PublicVS_Code.Export(Query, 2, i, 2, 26)

    '    End Sub
End Module
