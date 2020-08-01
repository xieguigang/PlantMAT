Module ThisWorkbook
    '    Attribute VB_Name = "ThisWorkbook"
    'Attribute VB_Base = "0{00020819-0000-0000-C000-000000000046}"
    'Attribute VB_GlobalNameSpace = False
    'Attribute VB_Creatable = False
    'Attribute VB_PredeclaredId = True
    'Attribute VB_Exposed = True
    'Attribute VB_TemplateDerived = False
    'Attribute VB_Customizable = True
    Private Sub Workbook_Open()
        Me.Unprotect
        Sheet1.Activate
        Sheet1.Range("J17").Select
        Sheet1.ScrollArea = "J17"
        Sheet1.Visible = True
        Sheet2.Visible = False
        Database_Dialog.Visible = False
        Sheet4.Visible = False
        Sheet5.Visible = False
        Me.Protect
        Call PublicVS_Code.Settings_Check

        Application.ExecuteExcel4Macro "SHOW.TOOLBAR(""Ribbon"",False)"
   Application.DisplayFormulaBar = False
        Application.DisplayStatusBar = False
        Application.DisplayAlerts = False
        Application.Cursor = xlNorthwestArrow
        ActiveWindow.DisplayGridlines = False
        ActiveWindow.DisplayHeadings = False
    End Sub

    Private Sub Workbook_Activate()
        Application.ExecuteExcel4Macro "SHOW.TOOLBAR(""Ribbon"",False)"
   Application.DisplayFormulaBar = False
        Application.DisplayStatusBar = False
        Application.DisplayAlerts = False
        Application.Cursor = xlNorthwestArrow
        ActiveWindow.DisplayGridlines = False
        ActiveWindow.DisplayHeadings = False
    End Sub

    Private Sub Workbook_Deactivate()
        Application.ExecuteExcel4Macro "SHOW.TOOLBAR(""Ribbon"",True)"
   Application.DisplayFormulaBar = True
        Application.DisplayStatusBar = True
        Application.DisplayAlerts = True
        Application.Cursor = xlDefault
        ActiveWindow.DisplayGridlines = True
        ActiveWindow.DisplayHeadings = True
    End Sub

    Private Sub Workbook_BeforeSave(ByVal SaveAsUI As Boolean, Cancel As Boolean)
        SaveAsUI = False
    End Sub

    Private Sub Workbook_BeforeClose(Cancel As Boolean)
        Me.Unprotect
        Sheet1.Visible = True
        Sheet2.Visible = False
        Database_Dialog.Visible = False
        Sheet4.Visible = False
        Sheet5.Visible = False
        Me.Protect
        ThisWorkbook.Save
    End Sub
End Module
