Module Sheet2


    'Attribute VB_Name = "Sheet2"
    'Attribute VB_Base = "0{00020820-0000-0000-C000-000000000046}"
    'Attribute VB_GlobalNameSpace = False
    'Attribute VB_Creatable = False
    'Attribute VB_PredeclaredId = True
    'Attribute VB_Exposed = True
    'Attribute VB_TemplateDerived = False
    'Attribute VB_Customizable = True
    Sub bt_Database()

        Database_Dialog.Activate

    End Sub

    Sub bt_SingleQuery()

        Sheet4.Activate
        Call SingleQ_Code.Button_SingleQ

    End Sub

    Sub bt_BatchImport()

        Sheet4.Activate
        Call Import_Code.Button_Import

    End Sub
End Module
