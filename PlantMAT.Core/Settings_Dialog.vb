Public Module Settings_Dialog


    'Attribute VB_Name = "Settings_Dialog"
    'Attribute VB_Base = "0{D10AE28D-E1A4-407A-ADE3-C0678DD7D494}{9F1802E0-75FB-475B-BE75-2FFE9DDC3303}"
    'Attribute VB_GlobalNameSpace = False
    'Attribute VB_Creatable = False
    'Attribute VB_PredeclaredId = True
    'Attribute VB_Exposed = False
    'Attribute VB_TemplateDerived = False
    'Attribute VB_Customizable = False
    Private Sub bt_Cancel_Click()

        Unload Settings_Dialog

End Sub

    Private Sub bt_ExternalSugarAcidDatabase_Click()

        DatabaseAddress = Application.GetOpenFilename("Database (*.txt), *.txt", , "Select Sugar/Acid Database")
        If DatabaseAddress <> False Then
            tb_ExternalSugarAcidDatabase.Enabled = True
            tb_ExternalSugarAcidDatabase.Value = DatabaseAddress
            tb_ExternalSugarAcidDatabase.Enabled = False
        End If

    End Sub

    Private Sub bt_OK_Click()

        On Error GoTo ErrorHandler

        Application.ScreenUpdating = False

        If cb_InternalAglyconeDatabase.Value = False And
           tb_ExternalAglyconeDatabase.Value = "[Select external database]" Then
            MsgBox "No database is selected", vbCritical
   Exit Sub
        End If

        tb11 = tb_NumSugarMin.Value
        tb12 = tb_NumSugarMax.Value
        tb21 = tb_NumAcidMin.Value
        tb22 = tb_NumAcidMax.Value

        If IsNumeric(tb11) = False Or Int(Val(tb11)) <> Val(tb11) Or
           IsNumeric(tb12) = False Or Int(Val(tb12)) <> Val(tb12) Or
           IsNumeric(tb21) = False Or Int(Val(tb21)) <> Val(tb21) Or
           IsNumeric(tb22) = False Or Int(Val(tb22)) <> Val(tb22) Or
           Val(tb11) > Val(tb12) Or Val(tb12) > 6 Or
           Val(tb21) > Val(tb22) Or Val(tb22) > 6 Then
            MsgBox "Incorrect min and/or max values", vbCritical, "PlantMAT"
   Exit Sub
        End If

Set Query = ThisWorkbook.Sheets("Query")

PlantMATfolderPath = "C:\Users\" & Environ$("Username") & "\Documents\PlantMAT"
Set fs = CreateObject("Scripting.FileSystemObject")
If Dir(PlantMATfolderPath, vbDirectory) = "" Then Set PlantMATfolder = fs.CreateFolder(PlantMATfolderPath)
Set Settingsfile = fs.CreateTextFile(PlantMATfolderPath & "\Settings.txt", True)
With Settingsfile
            .WriteLine("Internal Aglycone Database: " & cb_InternalAglyconeDatabase.Value)
            .WriteLine("External Aglycone Database: " & tb_ExternalAglyconeDatabase.Value)
            .WriteLine("Aglycone Type: " & db_AglyconeType.Value)
            .WriteLine("Aglycone Source: " & db_AglyconeSource.Value)
            .WriteLine("Aglycone MW Range: " & tb_AglyconeMWLL.Value & " " & tb_AglyconeMWUL.Value)
            .WriteLine("Num of Sugar (All): " & tb_NumSugarMin.Value & " " & tb_NumSugarMax.Value)
            .WriteLine("Num of Acid (All): " & tb_NumAcidMin.Value & " " & tb_NumAcidMax.Value)
            For i = 0 To lb_AddedSugarAcid.ListCount - 1
                NameSA = lb_AddedSugarAcid.List(i, 0)
                TypeSA = lb_AddedSugarAcid.List(i, 1)
                NumSAMin = lb_AddedSugarAcid.List(i, 2)
                NumSAMax = lb_AddedSugarAcid.List(i, 3)
                .WriteLine("Num of " & TypeSA & " (" & NameSA & "): " & NumSAMin & " " & NumSAMax)
            Next i
            .WriteLine("Precursor Ion Type: " & db_PrecursorIon.List(db_PrecursorIon.ListIndex, 0))
            .WriteLine("Precursor Ion MZ: " & db_PrecursorIon.List(db_PrecursorIon.ListIndex, 1))
            .WriteLine("Precursor Ion N: " & db_PrecursorIon.List(db_PrecursorIon.ListIndex, 2))
            .WriteLine("Search PPM: " & tb_Searchppm.Value)
            .WriteLine("Noise Filter: " & tb_NoiseFilter.Value)
            .WriteLine("m/z PPM: " & tb_Mzppm.Value)
            .WriteLine("Pattern Prediction: " & cb_PatternPrediction.Value)
        End With

        Settingsfile.Close
        Unload Settings_Dialog
Application.ScreenUpdating = True

        Exit Sub

ErrorHandler:
        MsgBox "Data incorrect", vbCritical, "PlantMAT"

End Sub

    Private Sub bt_ExternalAglyconeDatabase_Click()

        DatabaseAddress = Application.GetOpenFilename("Database (*.csv), *.csv", , "Select Aglycone Database")
        If DatabaseAddress <> False Then
            tb_ExternalAglyconeDatabase.Enabled = True
            tb_ExternalAglyconeDatabase.Value = DatabaseAddress
            tb_ExternalAglyconeDatabase.Enabled = False
        End If

    End Sub

    Private Sub bt_RemoveSugarAcid_Click()

        With lb_AddedSugarAcid
            For i = .ListCount - 1 To 0 Step -1
                If .Selected(i) Then
                    .RemoveItem i
            Exit For
                End If
            Next i
        End With

    End Sub

    Private Sub cb_InternalAglyconeDatabase_Click()

        If cb_InternalAglyconeDatabase.Value = False Then
            bt_ExternalAglyconeDatabase.Enabled = True
        Else
            tb_ExternalAglyconeDatabase.Value = "[Select external database]"
            bt_ExternalAglyconeDatabase.Enabled = False
        End If

    End Sub

    Private Sub cb_InternalSugarAcidDatabase_Click()

        If cb_InternalSugarAcidDatabase.Value = False Then
            bt_ExternalSugarAcidDatabase.Enabled = True
        Else
            tb_ExternalSugarAcidDatabase.Value = "[Select external database]"
            bt_ExternalSugarAcidDatabase.Enabled = False
        End If

    End Sub

    Private Sub bt_AddSugarAcid_Click()

        If db_SugarAcid.Value = "Sugar/Acid" Then
            MsgBox "Please select sugar/acid", vbCritical, "PlantMAT"
   Exit Sub
        End If

        tb11 = tb_NumSugarMin.Value
        tb12 = tb_NumSugarMax.Value
        tb21 = tb_NumAcidMin.Value
        tb22 = tb_NumAcidMax.Value
        tb31 = tb_NumSAMin.Value
        tb32 = tb_NumSAMax.Value

        If IsNumeric(tb11) = False Or Int(Val(tb11)) <> Val(tb11) Or
           IsNumeric(tb12) = False Or Int(Val(tb12)) <> Val(tb12) Or
           IsNumeric(tb21) = False Or Int(Val(tb21)) <> Val(tb21) Or
           IsNumeric(tb22) = False Or Int(Val(tb22)) <> Val(tb22) Or
           IsNumeric(tb31) = False Or Int(Val(tb31)) <> Val(tb31) Or
           IsNumeric(tb32) = False Or Int(Val(tb32)) <> Val(tb32) Or
           Val(tb11) > Val(tb12) Or Val(tb12) > 6 Or
           Val(tb21) > Val(tb22) Or Val(tb22) > 6 Or
           Val(tb31) > Val(tb32) Then
            MsgBox "Incorrect min and/or max values", vbCritical, "PlantMAT"
   Exit Sub
        End If

        With db_SugarAcid
            NameSA = .List(.ListIndex, 0)
            TypeSA = .List(.ListIndex, 1)
        End With

        If (TypeSA = "Sugar" And Val(tb32) > Val(tb12)) Or
           (TypeSA = "Acid" And Val(tb32) > Val(tb22)) Then
            MsgBox "Incorrect min and/or max values", vbCritical, "PlantMAT"
   Exit Sub
        End If

        With lb_AddedSugarAcid
            For i = 0 To .ListCount - 1
                If .List(i, 0) = NameSA Then
                    .List(i, 2) = tb31
                    .List(i, 3) = tb32
                    Exit Sub
                End If
            Next i
            .AddItem
            .List(.ListCount - 1, 0) = NameSA
            .List(.ListCount - 1, 1) = TypeSA
            .List(.ListCount - 1, 2) = tb31
            .List(.ListCount - 1, 3) = tb32
        End With

    End Sub

    Private Sub UserForm_Initialize()

        With db_AglyconeType
            .AddItem "All"
     .AddItem "Polyphenol"
     .AddItem "Triterpene"
     .AddItem "Steroid"
     .AddItem "Lipid"
End With

        With db_AglyconeSource
            .AddItem "All"
     .AddItem "Medicago"
     .AddItem "Arabidopsis"
     .AddItem "Asparagus"
     .AddItem "Glycine"
     .AddItem "Glycyrrhiza"
     .AddItem "Solanum"
End With

        Dim SugarAcidList(0 To 8, 0 To 2)
        SugarAcidList(0, 0) = "Hex"
        SugarAcidList(1, 0) = "HexA"
        SugarAcidList(2, 0) = "dHex"
        SugarAcidList(3, 0) = "Pen"
        SugarAcidList(4, 0) = "Mal"
        SugarAcidList(5, 0) = "Cou"
        SugarAcidList(6, 0) = "Fer"
        SugarAcidList(7, 0) = "Sin"
        SugarAcidList(8, 0) = "DDMP"
        SugarAcidList(0, 1) = "Sugar"
        SugarAcidList(1, 1) = "Sugar"
        SugarAcidList(2, 1) = "Sugar"
        SugarAcidList(3, 1) = "Sugar"
        SugarAcidList(4, 1) = "Acid"
        SugarAcidList(5, 1) = "Acid"
        SugarAcidList(6, 1) = "Acid"
        SugarAcidList(7, 1) = "Acid"
        SugarAcidList(8, 1) = "Acid"

        With db_SugarAcid
            .List() = SugarAcidList
        End With

        Dim IonTypeList(0 To 8, 0 To 2)
        IonTypeList(0, 0) = "[M-H]-"
        IonTypeList(1, 0) = "[M+Na-2H]-"
        IonTypeList(2, 0) = "[M+FA-H]-"
        IonTypeList(3, 0) = "[M+Hac-H]-"
        IonTypeList(4, 0) = "[2M-H]-"
        IonTypeList(5, 0) = "[2M+FA-H]-"
        IonTypeList(6, 0) = "[2M+Hac-H]-"
        IonTypeList(7, 0) = "[M+H]+"
        IonTypeList(8, 0) = "[M+Na]+"
        IonTypeList(0, 1) = "-1.007277"
        IonTypeList(1, 1) = "20.974666"
        IonTypeList(2, 1) = "44.998202"
        IonTypeList(3, 1) = "59.013852"
        IonTypeList(4, 1) = "-1.007277"
        IonTypeList(5, 1) = "44.998202"
        IonTypeList(6, 1) = "59.013852"
        IonTypeList(7, 1) = "1.007277"
        IonTypeList(8, 1) = "22.989220"
        IonTypeList(0, 2) = "1"
        IonTypeList(1, 2) = "1"
        IonTypeList(2, 2) = "1"
        IonTypeList(3, 2) = "1"
        IonTypeList(4, 2) = "2"
        IonTypeList(5, 2) = "2"
        IonTypeList(6, 2) = "2"
        IonTypeList(7, 2) = "1"
        IonTypeList(8, 2) = "1"

        With db_PrecursorIon
            .List() = IonTypeList
        End With

    End Sub
End Module
