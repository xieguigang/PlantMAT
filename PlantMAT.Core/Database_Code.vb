Module Database_Code


    ' Attribute VB_Name = "Database_Code"
    Sub Button_DataAdd()

        'Show diaglog box for data entry
        Database_Dialog.Show

    End Sub

    Sub Button_DataDelete()

'Delete data entry; Ask to confirm before deletion
Set Database = ThisWorkbook.Worksheets("Library")
LastRow = Database.Range("B" & Rows.Count).End(xlUp).Row
        FindRow = ActiveCell.Row
        If Database.Range("I" & FindRow) = "fqiu" Then
            MsgBox "Current entry is protected and cannot be deleted", vbInformation, "PlantMAT"
Else
            a = MsgBox("Delete current entry?", vbYesNo, "PlantMAT")
            If a = vbYes Then
                With Database
                    .Unprotect
                    .Rows(FindRow).Delete
                    .ScrollArea = "B3:B" & CStr(LastRow + 1)
                    .Protect
                End With
                ThisWorkbook.Save
                MsgBox "Current entry was deleted", vbInformation, "PlantMAT"
   End If
        End If

    End Sub

    Sub Button_DataExport()

'Export the whole library to CSV file
Set Database = ThisWorkbook.Sheets("Library")
i = Database.Range("B" & Rows.Count).End(xlUp).Row
        Call PublicVS_Code.Export(Database, 2, i, 2, 10)

    End Sub
End Module
