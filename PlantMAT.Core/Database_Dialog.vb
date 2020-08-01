Module Database_Dialog

    'Attribute VB_Name = "Database_Dialog"
    'Attribute VB_Base = "0{AD5148FF-1199-476B-92B3-A258F724E907}{778D62AA-BC1C-4728-BE7B-C76FB436C57A}"
    'Attribute VB_GlobalNameSpace = False
    'Attribute VB_Creatable = False
    'Attribute VB_PredeclaredId = True
    'Attribute VB_Exposed = False
    'Attribute VB_TemplateDerived = False
    'Attribute VB_Customizable = False
    Private Sub Add_Click()

        On Error GoTo ErrorHandler

        Dim Database As Worksheet
        Dim DatabaseDialog As UserForm
Set Database = ThisWorkbook.Worksheets("Library")
Set DatabaseDialog = Database_Dialog
LastRow = Database.Range("B" & Rows.Count).End(xlUp).Row
        NewRow = LastRow + 1

        With DatabaseDialog
            CommonName = .TextBox_CommonName.Text
     Class = .ComboBox_Class.Text
     SubClass = .ComboBox_Type.Text
     Genus = .ComboBox_Genus.Text
     Formula = .TextBox_Formula.Text
     SMILESt = .TextBox_SMILES.Text
     Editor = .TextBox_Editor.Text
     DMY = .TextBox_Date.Text
End With

        If CommonName = "" Or Class = "" Or SubClass = "" Or _
   Genus = "" Or Formula = "" Or SMILESt = "" Or _
   Editor = "" Or DMY = "" Then
   MsgBox "All fields are required", vbInformation, "PlantMAT"
   Exit Sub
        End If

        Dim pos(1 To 6) As Integer, num(1 To 6) As Integer

pos(1) = InStr(Formula, "C")
pos(2) = InStr(Formula, "H")
pos(3) = InStr(Formula, "O")
pos(4) = InStr(Formula, "N")
pos(5) = InStr(Formula, "P")
pos(6) = InStr(Formula, "S")

For i = 1 To 6
        If pos(i) <> 0 Then
        For j = 1 To Len(Formula) - 1
        If Val(Mid(Formula, pos(i) + 1, j)) = 0 Then Exit For
       Next j
        If j = 1 Then
          num(i) = 1
       Else
          num(i) = Val(Mid(Formula, pos(i) + 1, j - 1))
       End If
        End If
        Next i
        
ExactMass = num(1) * 12 + num(2) * 1.007825 + num(3) * 15.99491 + _
            num(4) * 14.00307 + num(5) * 30.97376 + num(6) * 31.97207

With Database
        .Unprotect
        .Range("B" & NewRow) = CommonName
        .Range("C" & NewRow) = Class
     .Range("D" & NewRow) = SubClass
        .Range("E" & NewRow) = Formula
        .Range("F" & NewRow) = ExactMass
        .Range("G" & NewRow) = Genus
        .Range("H" & NewRow) = SMILESt
        .Range("I" & NewRow) = Editor
        .Range("J" & NewRow) = DMY
        .ScrollArea = "B3:B" & CStr(LastRow + 2)
        .Protect
        End With

ThisWorkbook.Save

With DatabaseDialog
        .TextBox_CommonName.Text = ""
        .ComboBox_Class.Text = ""
        .ComboBox_Type.Text = ""
        .ComboBox_Genus.Text = ""
        .TextBox_Formula.Text = ""
        .TextBox_SMILES.Text = ""
        .TextBox_Editor.Text = ""
        .TextBox_Date.Text = Date
        End With

a = MsgBox("Success. Add next one?", vbYesNo, "PlantMAT")
If a = vbNo Then Database_Dialog.Hide

Exit Sub

ErrorHandler:
MsgBox "Data incorrect", vbCritical, "PlantMAT"

End Sub

        Private Sub Cancel_Click()

            Database_Dialog.Hide

        End Sub

        Private Sub UserForm_Initialize()

            Dim ComboBoxClass As ComboBox
            Dim ComboBoxGenus As ComboBox

Set ComboBoxClass = Database_Dialog.ComboBox_Class
With ComboBoxClass
                .AddItem "Polyphenol"
     .AddItem "Triterpene"
     .AddItem "Lipid"
End With

Set ComboBoxGenus = Database_Dialog.ComboBox_Genus
With ComboBoxGenus
                .AddItem "Arabidopsis"
     .AddItem "Medicago"
End With

            Database_Dialog.TextBox_Date = Date

        End Sub
End Module
