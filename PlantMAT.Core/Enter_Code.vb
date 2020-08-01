'Module Enter_Code

'    ' Attribute VB_Name = "Enter_Code"
'    Sub Button_SystemEnter()

'        'Click to enter and initialize all interfaces
'        Application.ScreenUpdating = False
'        ThisWorkbook.Unprotect

'        With Sheet2
'            .Visible = True
'            .Activate
'            .Range("C8").Select
'            .ScrollArea = "C8"
'        End With

'        Sheet1.Visible = False

'        With Sheet3
'            LastRow = .Range("B" & Rows.Count).End(xlUp).Row
'            .ScrollArea = "B3:B" & CStr(LastRow + 2)
'            .Visible = True
'        End With

'        Dim comb As Object
'        With Sheet4
'            .Unprotect
'            For Each comb In .DropDowns()
'                n = comb.ListCount
'                comb.Text = CStr(n) & " results"
'            Next
'            LastRow = .Range("D" & Rows.Count).End(xlUp).Row
'            .ScrollArea = "A4:Z" & CStr(LastRow + 2)
'            .Protect
'        End With
'        Sheet4.Visible = True

'        Sheet5.Visible = False

'        Sheet2.Activate

'        ThisWorkbook.Protect
'        ThisWorkbook.Save
'        Application.ScreenUpdating = True

'    End Sub

'End Module
