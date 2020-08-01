Attribute VB_Name = "ThisWorkbook"
Attribute VB_Base = "0{00020819-0000-0000-C000-000000000046}"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = True
Attribute VB_TemplateDerived = False
Attribute VB_Customizable = True
Private Sub Workbook_Open()
   Me.Unprotect
   Sheet1.Activate
   Sheet1.Range("J17").Select
   Sheet1.ScrollArea = "J17"
   Sheet1.Visible = True
   Sheet2.Visible = False
   Sheet3.Visible = False
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
   Sheet3.Visible = False
   Sheet4.Visible = False
   Sheet5.Visible = False
   Me.Protect
   ThisWorkbook.Save
End Sub

Attribute VB_Name = "Sheet5"
Attribute VB_Base = "0{00020820-0000-0000-C000-000000000046}"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = True
Attribute VB_TemplateDerived = False
Attribute VB_Customizable = True

Attribute VB_Name = "Sheet4"
Attribute VB_Base = "0{00020820-0000-0000-C000-000000000046}"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = True
Attribute VB_TemplateDerived = False
Attribute VB_Customizable = True

Attribute VB_Name = "Sheet1"
Attribute VB_Base = "0{00020820-0000-0000-C000-000000000046}"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = True
Attribute VB_TemplateDerived = False
Attribute VB_Customizable = True


Attribute VB_Name = "Settings_Dialog"
Attribute VB_Base = "0{D10AE28D-E1A4-407A-ADE3-C0678DD7D494}{9F1802E0-75FB-475B-BE75-2FFE9DDC3303}"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Attribute VB_TemplateDerived = False
Attribute VB_Customizable = False
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

If cb_InternalAglyconeDatabase.Value = False And _
   tb_ExternalAglyconeDatabase.Value = "[Select external database]" Then
   MsgBox "No database is selected", vbCritical
   Exit Sub
End If

tb11 = tb_NumSugarMin.Value
tb12 = tb_NumSugarMax.Value
tb21 = tb_NumAcidMin.Value
tb22 = tb_NumAcidMax.Value

If IsNumeric(tb11) = False Or Int(Val(tb11)) <> Val(tb11) Or _
   IsNumeric(tb12) = False Or Int(Val(tb12)) <> Val(tb12) Or _
   IsNumeric(tb21) = False Or Int(Val(tb21)) <> Val(tb21) Or _
   IsNumeric(tb22) = False Or Int(Val(tb22)) <> Val(tb22) Or _
   Val(tb11) > Val(tb12) Or Val(tb12) > 6 Or _
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
     .WriteLine ("Internal Aglycone Database: " & cb_InternalAglyconeDatabase.Value)
     .WriteLine ("External Aglycone Database: " & tb_ExternalAglyconeDatabase.Value)
     .WriteLine ("Aglycone Type: " & db_AglyconeType.Value)
     .WriteLine ("Aglycone Source: " & db_AglyconeSource.Value)
     .WriteLine ("Aglycone MW Range: " & tb_AglyconeMWLL.Value & " " & tb_AglyconeMWUL.Value)
     .WriteLine ("Num of Sugar (All): " & tb_NumSugarMin.Value & " " & tb_NumSugarMax.Value)
     .WriteLine ("Num of Acid (All): " & tb_NumAcidMin.Value & " " & tb_NumAcidMax.Value)
     For i = 0 To lb_AddedSugarAcid.ListCount - 1
         NameSA = lb_AddedSugarAcid.List(i, 0)
         TypeSA = lb_AddedSugarAcid.List(i, 1)
         NumSAMin = lb_AddedSugarAcid.List(i, 2)
         NumSAMax = lb_AddedSugarAcid.List(i, 3)
         .WriteLine ("Num of " & TypeSA & " (" & NameSA & "): " & NumSAMin & " " & NumSAMax)
     Next i
     .WriteLine ("Precursor Ion Type: " & db_PrecursorIon.List(db_PrecursorIon.ListIndex, 0))
     .WriteLine ("Precursor Ion MZ: " & db_PrecursorIon.List(db_PrecursorIon.ListIndex, 1))
     .WriteLine ("Precursor Ion N: " & db_PrecursorIon.List(db_PrecursorIon.ListIndex, 2))
     .WriteLine ("Search PPM: " & tb_Searchppm.Value)
     .WriteLine ("Noise Filter: " & tb_NoiseFilter.Value)
     .WriteLine ("m/z PPM: " & tb_Mzppm.Value)
     .WriteLine ("Pattern Prediction: " & cb_PatternPrediction.Value)
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

If IsNumeric(tb11) = False Or Int(Val(tb11)) <> Val(tb11) Or _
   IsNumeric(tb12) = False Or Int(Val(tb12)) <> Val(tb12) Or _
   IsNumeric(tb21) = False Or Int(Val(tb21)) <> Val(tb21) Or _
   IsNumeric(tb22) = False Or Int(Val(tb22)) <> Val(tb22) Or _
   IsNumeric(tb31) = False Or Int(Val(tb31)) <> Val(tb31) Or _
   IsNumeric(tb32) = False Or Int(Val(tb32)) <> Val(tb32) Or _
   Val(tb11) > Val(tb12) Or Val(tb12) > 6 Or _
   Val(tb21) > Val(tb22) Or Val(tb22) > 6 Or _
   Val(tb31) > Val(tb32) Then
   MsgBox "Incorrect min and/or max values", vbCritical, "PlantMAT"
   Exit Sub
End If

With db_SugarAcid
     NameSA = .List(.ListIndex, 0)
     TypeSA = .List(.ListIndex, 1)
End With

If (TypeSA = "Sugar" And Val(tb32) > Val(tb12)) Or _
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

Attribute VB_Name = "MS1_TopDown_Code"
'This module performs combinatorial enumeration

Sub Button_MS1()

'Click to run combintorial enumeration; first check if any MS1 data has been imported
Set Query = ThisWorkbook.Sheets("Query")
If IsNumeric(Query.Range("D4")) = False Or Query.Range("D4") = "" Then
   MsgBox "MS1 data incorrect", vbCritical, "PlantMAT"
   Exit Sub
End If

'Read the parameters in Settings (module: PublicVS_Code)
Call PublicVS_Code.Settings_Check
Call PublicVS_Code.Settings_Reading

'Check the aglycone library is available for use
If InternalAglyconeDatabase = False And Dir(ExternalAglyconeDatabase, vbDirectory) = "" Then
   MsgBox "Can't find external aglycone database", vbCritical, "PlantMAT"
   Exit Sub
End If

'Peform combinatorial enumeration and show the calculation progress (MS1CP)
PublicVS_Code.StartProcessing "Now analyzing, please wait...", "MS1CP"

ThisWorkbook.Save

'Show the message box after the calculation is finished
MsgBox "Substructure prediction finished", vbInformation, "PlantMAT"

End Sub

Sub MS1CP()

Application.ScreenUpdating = False
Application.EnableEvents = False

'Intialize the Query Interface and clear all previous data and results if any
With Query
     .Unprotect
     LastRow = .Range("D" & Rows.Count).End(xlUp).Row
     If LastRow >= 4 Then .Range("G4:" & "Z" & LastRow) = ""
     .Cells.ClearComments
     .DropDowns().Delete
     .ScrollArea = ""
End With

i = 4
Do While Query.Cells(i, 4) <> ""
   If Query.Cells(i, 4) = "..." Then
      Query.Cells(i, 4).EntireRow.Delete
      i = i - 1
   End If
   i = i + 1
Loop

Set Database = ThisWorkbook.Sheets("Library")
Set SMILES = ThisWorkbook.Sheets("SMILES")
With SMILES
     .Unprotect
     LastRow = .Range("D" & Rows.Count).End(xlUp).Row
     If LastRow >= 3 Then .Range("B3:" & "E" & LastRow) = ""
     .ScrollArea = ""
End With

'Run combinatorial enumeration
Pattern_n = 0
Call MS1_CombinatorialPrediction

'Show columns of sugar/acid if any >=1
For j = 8 To 16
    Query.Columns(j).Hidden = True
Next j

For j = 2 To 10
    NameSA = AddedSugarAcid(j, 0)
    If NameSA = "" Then Exit For
    Query.Columns(Query.Range(NameSA).Column).Hidden = False
Next j

'Enable the button for MS2 analysis and lock (protect) all spreadsheets
With Query
     .Shapes("bt_MS2A").OnAction = "Button_MS2Annotation"
     .Shapes("bt_MS2A").DrawingObject.Font.ColorIndex = 1
     Application.Goto .Range("A1"), True
     .ScrollArea = "A4:Z" & CStr(i + 1)
     .Protect
End With

With SMILES
     .ScrollArea = "E3:E" & CStr(Pattern_n + 1)
     .Protect
End With

Application.EnableEvents = True
Application.ScreenUpdating = True

End Sub

Sub MS1_CombinatorialPrediction()

Pattern_n = 0

i = 4
Do While Query.Cells(i, 4) <> ""
   DoEvents
   ErrorCheck = False
   RT_E = Query.Cells(i, 3)
   M_w = (Query.Cells(i, 4) - PrecursorIonMZ) / PrecursorIonN
   i_prev = i
   AllSMILES = ""
   Candidate_n = 0
   ReDim Candidate(0 To 14, 1 To 1)
   
   If M_w <= 0 Or M_w > 2000 Then
      Candidate_n = 0
      GoTo ResultDisplay
   End If
   
   For Hex_n = NumHexMin To NumHexMax
       For HexA_n = NumHexAMin To NumHexAMax
           For dHex_n = NumdHexMin To NumdHexMax
               For Pen_n = NumPenMin To NumPenMax
                   For Mal_n = NumMalMin To NumMalMax
                       For Cou_n = NumCouMin To NumCouMax
                           For Fer_n = NumFerMin To NumFerMax
                               For Sin_n = NumSinMin To NumSinMax
                                   For DDMP_n = NumDDMPMin To NumDDMPMax
                                       
                                       Call MS1_CombinatorialPrediction_RestrictionCheck
                                       
                                   Next DDMP_n
                               Next Sin_n
                           Next Fer_n
                       Next Cou_n
                   Next Mal_n
               Next Pen_n
           Next dHex_n
       Next HexA_n
   Next Hex_n

ResultDisplay:
   Call MS1_CombinatorialPrediciton_ResultDisplay
   
   i = i + 1
Loop

End Sub

Sub MS1_CombinatorialPrediction_RestrictionCheck()

Sugar_n = Hex_n + HexA_n + dHex_n + Pen_n
Acid_n = Mal_n + Cou_n + Fer_n + Sin_n + DDMP_n
                                       
If Sugar_n >= NumSugarMin And Sugar_n <= NumSugarMax And _
   Acid_n >= NumAcidMin And Acid_n <= NumAcidMax Then
                                          
   Attn_w = Hex_n * Hex_w + HexA_n * HexA_w + dHex_n * dHex_w + Pen_n * Pen_w + _
   Mal_n * Mal_w + Cou_n * Cou_w + Fer_n * Fer_w + Sin_n * Sin_w + DDMP_n * DDMP_w
   nH2O_w = (Sugar_n + Acid_n) * H2O_w
   Bal = M_w + nH2O_w - Attn_w
   
   If Bal >= AglyconeMWLL And Bal <= AglyconeMWUL Then
      If InternalAglyconeDatabase = True Then
         Call MS1_CombinatorialPrediciton_InternalDatabase
      Else
         Call MS1_CombinatorialPrediciton_ExternalDatabase
      End If
   End If
   
End If

End Sub

Sub MS1_CombinatorialPrediciton_InternalDatabase()

LastRow = Database.Range("B" & Rows.Count).End(xlUp).Row

For j = 3 To LastRow
    DoEvents
    AglyN = Database.Cells(j, 2)
    AglyT = Database.Cells(j, 3)
    AglyO = Database.Cells(j, 7)
    AglyW = Database.Cells(j, 6)
    AglyS = Database.Cells(j, 8)
    
    Call MS1_CombinatorialPrediciton_DatabaseSearch

Next j

End Sub

Sub MS1_CombinatorialPrediciton_ExternalDatabase()

Dim EachAgly() As String
Open ExternalAglyconeDatabase For Input As #1

Do Until EOF(1)
   DoEvents
   Line Input #1, textLine
   EachAgly = Split(textLine, ",")
   AglyN = EachAgly(0)
   AglyT = EachAgly(1)
   AglyO = EachAgly(2)
   AglyW = Val(EachAgly(4))
   AglyS = EachAgly(5)
   
   Call MS1_CombinatorialPrediciton_DatabaseSearch
   
Loop

Close #1

End Sub

Sub MS1_CombinatorialPrediciton_DatabaseSearch()

If AglyT = AglyconeType Or AglyconeType = "All" Then
   If AglyO = AglyconeSource Or AglyconeSource = "All" Then
      
      Err1 = Abs((M_w - (AglyW + Attn_w - nH2O_w)) / (AglyW + Attn_w - nH2O_w)) * 1000000
   
      If Err1 <= SearchPPM Then
         RT_P = 0
         
         Candidate_n = Candidate_n + 1
         ReDim Preserve Candidate(0 To 14, 1 To Candidate_n)
         Candidate(0, Candidate_n) = AglyW
         Candidate(1, Candidate_n) = AglyS
         Candidate(2, Candidate_n) = AglyN
         Candidate(3, Candidate_n) = Hex_n
         Candidate(4, Candidate_n) = HexA_n
         Candidate(5, Candidate_n) = dHex_n
         Candidate(6, Candidate_n) = Pen_n
         Candidate(7, Candidate_n) = Mal_n
         Candidate(8, Candidate_n) = Cou_n
         Candidate(9, Candidate_n) = Fer_n
         Candidate(10, Candidate_n) = Sin_n
         Candidate(11, Candidate_n) = DDMP_n
         Candidate(12, Candidate_n) = Err1
         Candidate(13, Candidate_n) = RT_P
         Candidate(14, Candidate_n) = CStr(RT_P - RT_E)
      End If
   
   End If
End If

End Sub

Sub MS1_CombinatorialPrediciton_ResultDisplay()

If Candidate_n = 0 Then
   With Query.Cells(i, 7)
        .Value = "No hits"
        .Font.Color = RGB(217, 217, 217)
        .HorizontalAlignment = xlLeft
   End With
Else
   For m = 1 To Candidate_n
       DoEvents
       max_temp = 100
       For n = 1 To Candidate_n
           DoEvents
           If Right(Candidate(14, n), 1) <> "*" And Abs(Val(Candidate(14, n))) < max_temp Then
              max_temp = Abs(Val(Candidate(14, n)))
              k = n
           End If
       Next n
          
       With Query
            If m > 1 Then
               .Cells(i, 4).Offset(1).EntireRow.Insert
               i = i + 1
               .Cells(i, 4) = "..."
            End If
            .Cells(i, 7).AddComment (CStr(Candidate(0, k)))
            .Cells(i, 7).Comment.Shape.TextFrame.AutoSize = True
            For q = 2 To 12
                .Cells(i, q + 5) = Candidate(q, k)
            Next q
            .Range(Cells(i, 7), Cells(i, 20)).Font.Color = RGB(0, 0, 0)
            .Cells(i, 7).HorizontalAlignment = xlLeft
            If max_temp <> RT_E Then
               .Cells(i, 19) = Candidate(13, k)
               .Cells(i, 20) = Candidate(14, k)
                RT_Diff = Abs(Val(Candidate(14, k)))
                If RT_Diff <= 0.5 Then .Cells(i, 20).Font.Color = RGB(118, 147, 60)
                If RT_Diff > 0.5 And RT_Diff <= 1 Then .Cells(i, 20).Font.Color = RGB(255, 192, 0)
                If RT_Diff > 1 Then .Cells(i, 20).Font.Color = RGB(192, 80, 77)
            Else
                If RetentionPrediction = True Then
                   .Range(Cells(i, 19), Cells(i, 20)) = "n/a"
                   .Range(Cells(i, 19), Cells(i, 20)).Font.Color = RGB(217, 217, 217)
                End If
            End If
       End With
       
       Candidate(14, k) = Candidate(14, k) + "*"
       
       If PatternPrediction = True Then Call MS1_CombinatorialPrediciton_PatternPrediction 'Pattern Prediction
   Next m
End If

End Sub

Sub MS1_CombinatorialPrediciton_PatternPrediction()

'1. Find location and number of OH groups in aglycone
Dim AglyS1 As String, AglyS2 As String
Dim Hex As String, HexA As String, dHex As String, Pen As String
Dim Cou As String, Fer As String, Sin As String, Mal As String
Dim e As Long, OH_n As Long
Dim n1 As Long, n2 As Long

AglyN = Candidate(2, k)
AglyS1 = Candidate(1, k)
AglyS2 = Replace(AglyS1, "O)", ".)")
AglyS2 = Replace(AglyS2, "=.", "=O")
If Right(AglyS2, 1) = "O" Then AglyS2 = Left(AglyS2, Len(AglyS2) - 1) & "."

OH_n = 0
For e = 1 To Len(AglyS2)
    If Mid(AglyS2, e, 1) = "." Then OH_n = OH_n + 1
Next e

If OH_n = 0 Then Exit Sub
If OH_n > 2 Then OH_n = 2

n1 = 0
n2 = 0
For e = 1 To Len(AglyS1)
    If IsNumeric(Mid(AglyS1, e, 1)) Then
       n2 = CInt(Mid(AglyS1, e, 1))
       If n2 > n1 Then n1 = n2
    End If
Next e

'2. Find type and number of sugars/acids
Dim Sug_n As Long
Dim Sug, Sug_p() As String
Dim g As Long, h As Long, l As Long

For e = 3 To 11
    Sug_n = Sug_n + Candidate(e, k)
Next e
If Sug_n = 0 Then Exit Sub

ReDim Sug_p(1, 1 To Sug_n)
l = 1

Hex = "C?C(C(C(C(CO)O?)O)O)O"
HexA = "C?C(C(C(C(C(=O)O)O?)O)O)O"
dHex = "C?C(C(C(C(C)O?)O)O)O"
Pen = "C?C(C(C(CO?)O)O)O"
Mal = "C(=O)CC(=O)O"
Cou = "c?ccc(cc?)C=CC(=O)O"
Fer = "COc?cc(ccc?O)C=CC(=O)O"
Sin = "COc?cc(C=CC(=O)O)cc(c?O)OC"
DDMP = "CC?=C(C(=O)CC(O)O?)O"

For e = 3 To 11
    g = Candidate(e, k)
    If g > 0 Then
       If e = 3 Then Sug = Hex
       If e = 4 Then Sug = HexA
       If e = 5 Then Sug = dHex
       If e = 6 Then Sug = Pen
       If e = 7 Then Sug = Mal
       If e = 8 Then Sug = Cou
       If e = 9 Then Sug = Fer
       If e = 10 Then Sug = Sin
       If e = 11 Then Sug = DDMP
       For h = 1 To g
           Sug_p(1, l) = Sug
           l = l + 1
       Next h
    End If
Next e

'3. Permutate sugars/acids without repetition
Dim c As Long, r As Long, p As Long
Dim rng() As Long, temp As Long
Dim temp1 As Long, y() As Long, d As Long

p = WorksheetFunction.Permut(Sug_n, Sug_n)

'3.1 Create array
ReDim rng(1 To p, 1 To Sug_n)


'3.2 Create first row in array (1, 2, 3, ...)
For c = 1 To Sug_n
    rng(1, c) = c
Next c
For r = 2 To p

'3.3 Find the first smaller number rng(r-1,c-1)<rng(r-1,c)
    For c = Sug_n To 1 Step -1
        If rng(r - 1, c - 1) < rng(r - 1, c) Then
           temp = c - 1
           Exit For
        End If
    Next c
    
'3.4 Copy values from previous row
    For c = Sug_n To 1 Step -1
        rng(r, c) = rng(r - 1, c)
    Next c
    
'3.5 Find a larger number than rng(r-1,temp) as far to the right as possible
    For c = Sug_n To 1 Step -1
        If rng(r - 1, c) > rng(r - 1, temp) Then
           temp1 = rng(r - 1, temp)
           rng(r, temp) = rng(r - 1, c)
           rng(r, c) = temp1
           ReDim y(Sug_n - temp)
           e = 0
           For d = temp + 1 To Sug_n
               y(e) = rng(r, d)
               e = e + 1
           Next d
           e = 0
           For d = Sug_n To temp + 1 Step -1
               rng(r, d) = y(e)
               e = e + 1
           Next d
           Exit For
        End If
    Next c
Next r

'4 Combine sugars/acids
Dim z As Long, q As Long, s As Long
Dim v As Long, w As Long, n As Long
Dim x() As String, t() As String, u() As String
ReDim x(1 To Sug_n, 1 To Sug_n)
ReDim t(100000, 1 To OH_n)
ReDim u(100000, 1 To OH_n)

w = 1

For v = 1 To p

'4.1 Load each group of sugar/acids from permutation
    For e = 1 To Sug_n
        x(1, e) = Sug_p(1, rng(v, e))
    Next e
    
'4.2 Within each group create all possible oligosaccharides
    l = 0
    For e = 1 To Sug_n
        h = e + 1
        For g = 2 To Sug_n - l
            x(g, e) = x(g - 1, e) + x(1, h)
            h = h + 1
        Next g
        l = l + 1
    Next e
    
'4.3 Within each group make all unique combinations of mono- and oligosaccharides
'4.3.1 Make all possible combinations
    n = 1
    For z = 0 To Sug_n - 1
        If n > OH_n Then Exit For
        For q = 1 To Sug_n - z - 1
            If OH_n = 1 Then GoTo AllSugarConnected
            n = 2
            If z > 0 Then
               c = 0
               For s = 1 To z
                   t(w, n) = x(1, q + s)
                   c = c + 1
                   n = n + 1
                   If n > OH_n - 1 Then Exit For
               Next s
            End If
            t(w, 1) = x(q, 1)
            t(w, n) = x(Sug_n - (q + z), (q + z) + 1)
            If c < z Then
               For e = 1 To OH_n
                   t(w, e) = ""
               Next e
               w = w - 1
            End If
            n = n + 1
            w = w + 1
        Next q
    Next z
AllSugarConnected:
    For e = 1 To Sug_n
        t(w, 1) = t(w, 1) + x(1, e)
    Next e
    w = w + 1
    
Next v

'4.3.2 Remove all duplicates regardless of order
s = 1
c = 0
For e = 1 To w - 1
    For r = 1 To s - 1
        c = 0
        For g = 1 To OH_n
            For h = 1 To OH_n
                If t(e, g) = u(r, h) Then
                   u(r, h) = u(r, h) + "*"
                   c = c + 1
                   Exit For
                End If
            Next h
        Next g
        If c = OH_n Then Exit For
    Next r
    If c < OH_n Then
       For g = 1 To OH_n
           u(s, g) = t(e, g)
       Next g
       s = s + 1
    End If
    For r = 1 To s - 1
        For h = 1 To OH_n
            If Right(u(r, h), 1) = "*" Then u(r, h) = Left(u(r, h), Len(u(r, h)) - 1)
        Next h
    Next r
Next e

'5. Attach each sugar/acid combination to aglycone to create all possible glycosides
Dim GlycS As String, GlycN As String
Dim SugComb As String, SugComb1 As String
Dim n3 As Long

For e = 1 To s - 1
    GlycS = AglyS2
    n3 = n1
    SugComb = ""
    For g = 1 To OH_n
        If u(e, g) <> "" Then
           SugComb1 = u(e, g)
           If InStr(SugComb1, Hex) <> 0 Then SugComb1 = Replace(SugComb1, Hex, "-Hex")
           If InStr(SugComb1, HexA) <> 0 Then SugComb1 = Replace(SugComb1, HexA, "-HexA")
           If InStr(SugComb1, dHex) <> 0 Then SugComb1 = Replace(SugComb1, dHex, "-dHex")
           If InStr(SugComb1, Mal) <> 0 Then SugComb1 = Replace(SugComb1, Mal, "-Mal")
           If InStr(SugComb1, Pen) <> 0 Then SugComb1 = Replace(SugComb1, Pen, "-Pen")
           If InStr(SugComb1, Cou) <> 0 Then SugComb1 = Replace(SugComb1, Cou, "-Cou")
           If InStr(SugComb1, Fer) <> 0 Then SugComb1 = Replace(SugComb1, Fer, "-Fer")
           If InStr(SugComb1, Sin) <> 0 Then SugComb1 = Replace(SugComb1, Sin, "-Sin")
           If InStr(SugComb1, DDMP) <> 0 Then SugComb1 = Replace(SugComb1, DDMP, "-DDMP")
           SugComb = SugComb + ", " + SugComb1
        Else
           Exit For
        End If
    Next g
    
    GlycN = AglyN + SugComb
    
    With SMILES
         .Cells(Pattern_n + 3, 2) = Query.Cells(i_prev, 2)
         .Cells(Pattern_n + 3, 3) = CStr(m) + "-" + CStr(e)
         .Cells(Pattern_n + 3, 4) = GlycN
    End With
    
    Pattern_n = Pattern_n + 1
Next e

End Sub



Attribute VB_Name = "Sheet2"
Attribute VB_Base = "0{00020820-0000-0000-C000-000000000046}"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = True
Attribute VB_TemplateDerived = False
Attribute VB_Customizable = True
Sub bt_Database()

Sheet3.Activate

End Sub

Sub bt_SingleQuery()

Sheet4.Activate
Call SingleQ_Code.Button_SingleQ

End Sub

Sub bt_BatchImport()

Sheet4.Activate
Call Import_Code.Button_Import

End Sub


Attribute VB_Name = "Sheet3"
Attribute VB_Base = "0{00020820-0000-0000-C000-000000000046}"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = True
Attribute VB_TemplateDerived = False
Attribute VB_Customizable = True

Attribute VB_Name = "Database_Dialog"
Attribute VB_Base = "0{AD5148FF-1199-476B-92B3-A258F724E907}{778D62AA-BC1C-4728-BE7B-C76FB436C57A}"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Attribute VB_TemplateDerived = False
Attribute VB_Customizable = False
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

Attribute VB_Name = "Edit_Code"
Sub Button_Reset()

'Reset (Clear) all fields in the output display; Ask to confirm before resetting
    a = MsgBox("Reset all fields?", vbYesNo, "PlantMAT")

    If a = vbYes Then
        Application.ScreenUpdating = False
        Set Query = ThisWorkbook.Sheets("Query")
        With Query
            .Unprotect
            LastRow = .Range("D" & Rows.Count).End(xlUp).Row
            If LastRow >= 4 Then .Range("B4:" & "Z" & LastRow) = ""
            .Cells.ClearComments
            .DropDowns().Delete
            .ScrollArea = ""
            For i = 8 To 16
                Query.Columns(i).Hidden = True
            Next i
            .Shapes("bt_MS1").OnAction = Empty
            .Shapes("bt_MS1").DrawingObject.Font.ColorIndex = 16
            .Shapes("bt_MS2A").OnAction = Empty
            .Shapes("bt_MS2A").DrawingObject.Font.ColorIndex = 16
            Application.Goto .Range("A1"), True
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
    End If

End Sub

Sub Button_Export()

'Export the results to CSV file
    Set Query = ThisWorkbook.Sheets("Query")

    If Query.Range("G4") = "" Then
        MsgBox "No results to export", vbCritical, "PlantMAT"
        Exit Sub
    End If

    i = Query.Range("G" & Rows.Count).End(xlUp).Row

    Call PublicVS_Code.Export(Query, 2, i, 2, 26)

End Sub

Attribute VB_Name = "Processing_Dialog"
Attribute VB_Base = "0{80811597-4C57-43F0-9AE9-0C8A34BE5E8F}{A0BE763D-4584-45AF-91A2-118DBE282DA5}"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
Attribute VB_TemplateDerived = False
Attribute VB_Customizable = False
Private Sub UserForm_Activate()

Me.Repaint                               'Refresh the UserForm
Application.Run Macro_to_Process         'Run the macro
Unload Me                                'Unload the UserForm

End Sub

Private Sub UserForm_Initialize()

lblmessage.Caption = Processing_Message  'Change the Label Caption

End Sub

Private Sub UserForm_QueryClose(Cancel As Integer, CloseMode As Integer)

If CloseMode = vbFormControlMenu Then
   Cancel = True
End If

End Sub

Attribute VB_Name = "Database_Code"
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
     .Shapes("bt_MS1").OnAction = Empty
     .Shapes("bt_MS1").DrawingObject.Font.ColorIndex = 16
     .Shapes("bt_MS2A").OnAction = Empty
     .Shapes("bt_MS2A").DrawingObject.Font.ColorIndex = 16
     .Protect
End With

ThisWorkbook.Save

MsgBox "Annotation finished", vbInformation, "PlantMAT"

Exit Sub

ErrorHandler:
MsgBox "Data incorrect", vbCritical, "PlantMAT"

End Sub

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
      Call MS2A_TopDown_MS2Annotation
   Else
      Call MS2File_Searching
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
         Call MS2A_TopDown_MS2Annotation
      End If
   End If
Loop

'Go to the top of spreadsheet and lock (protect) the spreadsheet
With Query
     Application.Goto .Range("A1"), True
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
    
    File = Dir
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
   Call MS2A_TopDown_MS2Annotation_IonPrediction
   
   'Second, compare the predicted ions with the measured
   Call MS2A_TopDown_MS2Annotation_IonMatching
   
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
          aResult = aResult & CStr(Format(aIonMZ, "0.0000")) & ", " & _
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
MIonMZ = Agly_w + Hex_max * Hex_w + HexA_max * HexA_w + dHex_max * dHex_w + Pen_max * Pen_w + _
         Mal_max * Mal_w + Cou_max * Cou_w + Fer_max * Fer_w + Sin_max * Sin_w + DDMP_max * DDMP_w - _
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
                                            
                                            Call MS2A_TopDown_MS2Annotation_IonPrediction_LossCombination
                                            
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
Loss_w = Hex_n * Hex_w + HexA_n * HexA_w + dHex_n * dHex_w + Pen_n * Pen_w + _
         Mal_n * Mal_w + Cou_n * Cou_w + Fer_n * Fer_w + Sin_n * Sin_w + DDMP_n * DDMP_w - _
         Total_n * H2O_w + H2O_n * H2O_w + CO2_n * CO2_w

'Calculate the precuror ion mz based on the calcualted loss mass
pIonMZ = MIonMZ - Loss_w

'Find if the ion is related to the H2O/CO2 loss from aglycone
If Hex_n = Hex_max And HexA_n = HexA_max And dHex_n = dHex_max And Pen_n = Pen_max And _
   Mal_n = Mal_max And Cou_n = Cou_max And Fer_n = Fer_max And Sin_n = Sin_max And DDMP_n = DDMP_max Then
   pIonNM = "[Agly" & H2OLoss & CO2Loss & Rsyb
   If H2OLoss & CO2Loss = "" Or (H2OLoss & CO2Loss = "-H2O-CO2" And _
      (AglyN = "Medicagenic acid" Or AglyN = "Zanhic acid")) Then
      pIonNM = "*" & pIonNM
   End If
Else
   pIonNM = "[M" & HexLoss & HexALoss & dHexLoss & PenLoss & _
                   MalLoss & CouLoss & FerLoss & SinLoss & DDMPLoss & _
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

Attribute VB_Name = "MS2P_Code"
Sub MS2P()

Application.ScreenUpdating = False
Application.EnableEvents = False

Dim dd As Object

With Query
     .Unprotect
     LastRow = .Range("D" & Rows.Count).End(xlUp).Row
     If LastRow >= 4 Then
        .Range("Y4:" & "Z" & LastRow).ClearContents
        For Each dd In .DropDowns()
            If Left(dd.Name, 7) = "dd_MS2P" Then dd.Delete
        Next dd
     End If
     .ScrollArea = ""
End With

Call PublicVS_Code.Settings_Check
Call PublicVS_Code.Settings_Reading

i = 4

Do While Query.Cells(i, 4) <> ""
   DoEvents
   
   Do While Query.Cells(i, 7) = "No hits"
      i = i + 1
      k = k + 1
   Loop
   
   If Query.Cells(i, 4) = "" Then Exit Do
   
   k = 1
   
   With Query
        CmpdTag = .Cells(i, 2)
        DHIonMZ = .Cells(i, 4)
   End With
   
   If Right(PrecursorIonType, 1) = "-" Then
      MIonMZ = ((DHIonMZ - PrecursorIonMZ) / PrecursorIonN) - H_w + e_w
      Rsyb = "-H]-"
   Else
      MIonMZ = ((DHIonMZ - PrecursorIonMZ) / PrecursorIonN) + H_w - e_w
      Rsyb = "+H]+"
   End If
   
   If SingleQ = True Then
      Call MS2P_MS2Prediction
   Else
      Call MS2File_Searching
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
         k = k + 1
         Do While Query.Cells(i, 4) = "..."
            i = i + 1
            k = k + 1
         Loop
      Else
         Call MS2P_MS2Prediction
      End If
   End If
Loop

With Query
     Application.Goto .Range("A1"), True
     .ScrollArea = "A4:Z" & CStr(i + 1)
     .Protect
End With

Application.EnableEvents = True
Application.ScreenUpdating = True

End Sub

Sub MS2P_MS2Prediction()
 
'Find how many structural possibilites for each peak in 'SMILES' sheet
r = 3

Do While True
   PeakNo = SMILES.Cells(r, 2)
   If PeakNo = 0 Or PeakNo = CmpdTag Then Exit Do
   r = r + 1
Loop

'Predict MS2
Do While True
   DoEvents
      
   AglyMass = Val(Query.Cells(i, 7).Comment.Text)
                        
   'Create a combbox for MS2 prediction results of each combination possibility
   With Query.Cells(i, 26)
        Set comb = Query.DropDowns.Add(.Left, .Top, .Width, .Height)
        comb.Name = "dd_MS2P_" & CStr(i)
   End With
                        
   'Predict MS2 [MSPrediction()] for each structural possibility
   PredNo = k
   Pred_n = 0
   Match_n = 0
   Match_m = 0
   ReDim RS(2, 1)
   
   With SMILES
        Do While PeakNo = CmpdTag And PredNo = k
           DoEvents
                      
           Pred_n = Pred_n + 1
           GlycN = .Cells(r, 4)
                                
           Comma_n = 0
           For e = 1 To Len(GlycN)
               Lt = Mid(GlycN, e, 1)
               If Lt = "," Then Comma_n = Comma_n + 1
           Next e
    
           Call MS2P_MS2Prediction_IonPredictionMatching
                                
           r = r + 1
           PeakNo = .Cells(r, 2)
           temp = ""
                 
           For l = 1 To Len(.Cells(r, 3))
               If Mid(.Cells(r, 3), l, 1) = "-" Then Exit For
               temp = temp + Mid(.Cells(r, 3), l, 1)
           Next l
                 
           PredNo = Val(temp)
        Loop
   End With
   
   'Sort RS() in descending order and write new list to combbox and worksheet
   pResult = ""
   Best_n = 0
                        
   If Match_m > 0 Then
      For t = 1 To Match_n
          max_temp = -1
          For s = 1 To Match_n
              If Right(RS(1, s), 1) <> "*" And Val(RS(1, s)) > max_temp Then
                 max_temp = Val(RS(1, s))
                 u = s
              End If
          Next s
          RS(1, u) = RS(1, u) + "*"
          If t = 1 Then max_real = max_temp
          max_real = 1
          If max_temp / max_real = 1 Then Best_n = Best_n + 1
          comb.AddItem CStr(Format(max_temp / max_real, "0.00")) & " " & RS(2, u)
          pResult = pResult & CStr(Format(max_temp / max_real, "0.00")) & " " & RS(2, u) & "; "
      Next t
   End If
   
   comb.Text = CStr(Match_m) & "/" & CStr(Pred_n) & " candidates"
                        
   With Query
        If .Cells(i, 22) = "*" Then
           .Cells(i, 25) = "*"
           .Cells(i, 25).HorizontalAlignment = xlCenter
           .Cells(i, 25).Font.Color = RGB(118, 147, 60)
        End If
        If Match_n > 0 And Match_m > 0 Then
           .Cells(i, 26) = CStr(Match_m) & "/" & CStr(Pred_n) & " candidates: " & _
                           Left(pResult, Len(pResult) - 2)
           .Cells(i, 26).Font.Color = RGB(255, 255, 255)
           .Cells(i, 26).HorizontalAlignment = xlFill
        End If
   End With
   
   i = i + 1
   k = k + 1
                     
   If Query.Cells(i, 4) <> "..." Then Exit Sub
Loop

End Sub

Sub MS2P_MS2Prediction_IonPredictionMatching()

'1. Declare variables and assign mass of [M-H2O]
Dim m() As String, u() As String, Lt As String
Dim n1() As Double, n2() As Double, w() As Double
Dim Loss As Double, Loss1 As Double, pIonList() As Double
Dim pIonMZ As Double, eIonMZ As Double, eIonInt As Double
ReDim m(20, 20), u(1, 100)
ReDim f1(1, 100), f2(1, 100), w(5, 100)

'2. Read aglyone/sugar/acid combination and store each component to u()
Comma_n = 0
g = 1
For e = 1 To Len(GlycN)
    Lt = Mid(GlycN, e, 1)
    If Lt = "," And Comma_n = 0 Then
       SugComb = Right(GlycN, Len(GlycN) - e - 1)
       Comma_n = Comma_n + 1
    End If
    If Lt <> "," Then
       u(1, g) = u(1, g) + Lt
    Else
       e = e + 1
       g = g + 1
    End If
Next e

NumComponent = g

'3. Identify each component, calculate mass, and store value to w()
Lt = ""
For e = 2 To g
    s = 1
    For h = Len(u(1, e)) To 1 Step -1
        Lt = Mid(u(1, e), h, 1)
        If Lt <> "-" Then
           m(e - 1, s) = Lt + m(e - 1, s)
           If m(e - 1, s) = "Hex" Then w(e - 1, s) = Hex_w - H2O_w
           If m(e - 1, s) = "HexA" Then w(e - 1, s) = HexA_w - H2O_w
           If m(e - 1, s) = "dHex" Then w(e - 1, s) = dHex_w - H2O_w
           If m(e - 1, s) = "Pen" Then w(e - 1, s) = Pen_w - H2O_w
           If m(e - 1, s) = "Mal" Then w(e - 1, s) = Mal_w - H2O_w
           If m(e - 1, s) = "Cou" Then w(e - 1, s) = Cou_w - H2O_w
           If m(e - 1, s) = "Fer" Then w(e - 1, s) = Fer_w - H2O_w
           If m(e - 1, s) = "Sin" Then w(e - 1, s) = Sin_w - H2O_w
           If m(e - 1, s) = "DDMP" Then w(e - 1, s) = DDMP_w - H2O_w
        Else
           w(e - 1, s) = w(e - 1, s) + w(e - 1, s - 1)
           s = s + 1
        End If
    Next h
Next e

'4. Fragment each sugar chain forward (NL = sugar portions);
'calualte mass of each fragment (loss), and store value to f1()
h = 0
For c1 = 1 To 5
    For c1f = 1 To 100
        If w(c1, c1f) = 0 Then Exit For
        h = h + 1
        f1(1, h) = w(c1, c1f)
        Loss1 = f1(1, h)
        For c2 = c1 + 1 To 5
            For c2f = 1 To 100
                If w(c2, c2f) = 0 Then Exit For
                h = h + 1
                f1(1, h) = Loss1 + w(c2, c2f)
                Loss2 = f1(1, h)
                For c3 = c2 + 1 To 5
                    For c3f = 1 To 100
                        If w(c3, c3f) = 0 Then Exit For
                        h = h + 1
                        f1(1, h) = Loss2 + w(c3, c3f)
                        Loss3 = f1(1, h)
                        For c4 = c3 + 1 To 5
                            For c4f = 1 To 100
                                If w(c4, c4f) = 0 Then Exit For
                                h = h + 1
                                f1(1, h) = Loss3 + w(c4, c4f)
                                Loss4 = f1(1, h)
                                For c5 = c4 + 1 To 5
                                    For c5f = 1 To 100
                                        If w(c5, c5f) = 0 Then Exit For
                                        h = h + 1
                                        f1(1, h) = Loss4 + w(c5, c5f)
                                    Next c5f
                                Next c5
                            Next c4f
                        Next c4
                    Next c3f
                Next c3
            Next c2f
        Next c2
    Next c1f
Next c1

'5. Fragment each sugar chain backward (ion = sugar portions);
'calualte mass of each fragment (loss), and store value to f1()
h1 = h + 1
For e = 2 To NumComponent
    NameComponent = u(1, e)
    NumDash = 0
    For g = Len(NameComponent) To 1 Step -1
        NameSugar = Mid(NameComponent, g, 1) + NameSugar
        If Mid(NameComponent, g, 1) = "-" Then NumDash = NumDash + 1
        If NameSugar = "-Hex" Then Mass = Hex_w
        If NameSugar = "-HexA" Then Mass = HexA_w
        If NameSugar = "-dHex" Then Mass = dHex_w
        If NameSugar = "-Pen" Then Mass = Pen_w
        If NameSugar = "-Mal" Then Mass = Mal_w
        If NameSugar = "-Cou" Then Mass = Cou_w
        If NameSugar = "-Fer" Then Mass = Fer_w
        If NameSugar = "-Sin" Then Mass = Sin_w
        If NameSugar = "-DDMP" Then Mass = DDMP_w
        If Mass <> 0 Then
            h = h + 1
            If NumDash = 1 Then f1_temp = Mass
            If NumDash = 2 Then f1(1, h) = f1_temp + Mass - H2O_w
            If NumDash > 2 Then f1(1, h) = f1(1, h - 1) + Mass - H2O_w
            NameSugar = ""
            Mass = 0
        End If
    Next g
Next e

For h2 = h1 To h
    f1(1, h2) = MIonMZ - f1(1, h2) + H_w - e_w
Next h2

h1 = h + 1
For e = 2 To NumComponent
    NameComponent = u(1, e)
    NumDash = 0
    For g = 1 To Len(NameComponent)
        NameSugar = Mid(NameComponent, g, 1) + NameSugar
        If Mid(NameComponent, g, 1) = "-" Then NumDash = NumDash + 1
        If NameSugar = "Hex-" Then Mass = Hex_w
        If NameSugar = "HexA-" Then Mass = HexA_w
        If NameSugar = "dHex-" Then Mass = dHex_w
        If NameSugar = "Pen-" Then Mass = Pen_w
        If NameSugar = "Mal-" Then Mass = Mal_w
        If NameSugar = "Cou-" Then Mass = Cou_w
        If NameSugar = "Fer-" Then Mass = Fer_w
        If NameSugar = "Sin-" Then Mass = Sin_w
        If NameSugar = "DDMP-" Then Mass = DDMP_w
        If Mass <> 0 Then
            h = h + 1
            If NumDash = 1 Then f1_temp = Mass
            If NumDash = 2 Then f1(1, h) = f1_temp + Mass - H2O_w
            If NumDash > 2 Then f1(1, h) = f1(1, h - 1) + Mass - H2O_w
            NameSugar = ""
            Mass = 0
        End If
    Next g
Next e

For h2 = h1 To h
    f1(1, h2) = MIonMZ - f1(1, h2) + H_w - e_w
Next h2

'6. Remove duplicates (loss with same mass) in array f1() and create a new list to f2()
g = 1
f2(1, 1) = f1(1, 1)
For e = 1 To h
    For s = 1 To g
        If Int(f1(1, e)) = Int(f2(1, s)) Then GoTo NextOne
    Next s
    g = g + 1
    f2(1, g) = f1(1, e)
NextOne:
Next e

'7. Create ion list based on possible sugar/acid losses in f2() and store value to pIonList()
ReDim pIonList(g, 4)
For e = 1 To g
    h = 1
    For x = 0 To 1
        For y = 0 To 1
            If x + y > 2 Then Exit For
            pIonList(e, h) = MIonMZ - f2(1, e) - x * H2O_w - y * CO2_w
            h = h + 1
        Next y
    Next x
Next e

'8. Compare pIonList() with eIonlist(), calculate raw score, and save result to RS()
RawScore = 0
For e = 1 To g
    For h = 1 To 4
        pIonMZ = pIonList(e, h)
        For s = 1 To eIon_n
            eIonMZ = eIonList(1, s)
            If Abs(pIonMZ - eIonMZ) / pIonMZ * 1000000 < mzPPM Then
               eIonInt = eIonList(2, s)
               RawScore = RawScore + WorksheetFunction.Log10(100000 * eIonInt / TotalIonInt)
               GoTo NextPriIon
            End If
        Next s
    Next h
NextPriIon:
Next e

If RawScore > 0 Then Match_m = Match_m + 1
Match_n = Match_n + 1
ReDim Preserve RS(2, Match_n)
RS(1, Match_n) = CStr(RawScore)
RS(2, Match_n) = SugComb

End Sub


Attribute VB_Name = "PublicVS_Code"
Public Processing_Message As String
Public Macro_to_Process As String
Public ErrorCheck As Boolean
Public Database As Worksheet, Query As Worksheet, SMILES As Worksheet
Public ListFile As Variant
Public fs As Object, BatchFile As Object
Public MS2FilePath As String
Public i As Long, i_prev As Long, j As Long, k As Long, m As Long, CmpdTag As Integer
Public Candidate() As Variant, Candidate_n As Long, Pattern_n As Long
Public IonType As String, IonMZ_crc As Double, Rsyb As String, GlycN As String, SugComb As String
Public DHIonMZ As Double, MIonMZ As Double, DaughterIonMZ As Double, DaughterIonInt As Double, TotalIonInt As Double
Public eIonList() As Double, eIon_n As Long, pIonList() As Variant, pIon_n As Long, aIonList() As Variant, aIon_n As Long
Public AglyN As String, AglyT As String, AglyO As String, AglyW As Double, AglyS As String
Public Agly() As Variant, Agly_w As Double, Agly_n As Long, aAgly() As Variant, aAgly_n As Long
Public M_w As Double, H_w As Double, e_w As Double, RT_E As Double, RT_P As Variant
Public Hex_w As Double, HexA_w As Double, dHex_w As Double, Pen_w As Double
Public Mal_w As Double, Cou_w As Double, Fer_w As Double, Sin_w As Double, DDMP_w As Double
Public H2O_w As Double, nH2O_w As Double, CO2_w As Double, Attn_w As Double, Bal As Double
Public Hex_n As Long, HexA_n As Long, dHex_n As Long, Pen_n As Long, CO2_n As Long, H2O_n As Long
Public Mal_n As Long, Cou_n As Long, Fer_n As Long, Sin_n As Long, DDMP_n As Long
Public Hex_max As Long, HexA_max As Long, dHex_max As Long, Pen_max As Long
Public Mal_max As Long, Cou_max As Long, Fer_max As Long, Sin_max As Long, DDMP_max As Long
Public HexLoss As String, HexALoss As String, dHexLoss As String, PenLoss As String, CO2Loss As String, H2OLoss As String
Public MalLoss As String, CouLoss As String, FerLoss As String, SinLoss As String, DDMPLoss As String
Public AglyMass As Double, AglyCheck As Boolean, SingleQ As Boolean, FileCheck As Boolean
Public RawScore As Double, RS() As String, Match_n As Long, Match_m As Long
Public comb As Object, SMILESfile As Object, SMILESfolderName As String, SMILESfolderPath As String
Public Subs() As Variant, Subs_n As Long, pNL() As Double, pNL_n As Long, aNL() As String, aNL_n As Long

Public InternalAglyconeDatabase As Boolean, ExternalAglyconeDatabase As String, SubstructureDatabase As String
Public AglyconeType As String, AglyconeSource As String, AglyconeMWLL As Double, AglyconeMWUL As Double
Public AddedSugarAcid() As Variant
Public NumSugarMin As Integer, NumSugarMax As Integer, NumAcidMin As Integer, NumAcidMax As Integer
Public NumHexMin As Integer, NumHexMax As Integer, NumHexAMin As Integer, NumHexAMax As Integer
Public NumdHexMin As Integer, NumdHexMax As Integer, NumPenMin As Integer, NumPenMax As Integer
Public NumMalMin As Integer, NumMalMax As Integer, NumCouMin As Integer, NumCouMax As Integer
Public NumFerMin As Integer, NumFerMax As Integer, NumSinMin As Integer, NumSinMax As Integer
Public NumDDMPMin As Integer, NumDDMPMax As Integer
Public PrecursorIonType As String, PrecursorIonMZ As Double, PrecursorIonN As Integer
Public SearchPPM As Double, NoiseFilter As Double, mzPPM As Double
Public PatternPrediction As Boolean

Sub Settings_Check()

On Error GoTo ErrorHandler

PlantMATfolder = "C:\Users\" & Environ$("Username") & "\Documents\PlantMAT"
Set fs = CreateObject("Scripting.FileSystemObject")
If Dir(PlantMATfolder, vbDirectory) = "" Then
   Set PlantMATfolder = fs.CreateFolder(PlantMATfolder)
End If
If Dir(PlantMATfolder & "\Settings.txt", vbDirectory) = "" Then
   Set Settingsfile = fs.CreateTextFile(PlantMATfolder & "\Settings.txt", True)
   With Settingsfile
        .WriteLine ("Internal Aglycone Database: True")
        .WriteLine ("External Aglycone Database: [Select external database]")
        .WriteLine ("Aglycone Type: Triterpene")
        .WriteLine ("Aglycone Source: Medicago")
        .WriteLine ("Aglycone MW Range: 400 600")
        .WriteLine ("Num of Sugar (All): 0 6")
        .WriteLine ("Num of Acid (All): 0 1")
        .WriteLine ("Num of Sugar (Hex): 0 6")
        .WriteLine ("Num of Sugar (HexA): 0 6")
        .WriteLine ("Num of Sugar (dHex): 0 6")
        .WriteLine ("Num of Sugar (Pen): 0 6")
        .WriteLine ("Num of Acid (Mal): 0 1")
        .WriteLine ("Precursor Ion Type: [M-H]-")
        .WriteLine ("Precursor Ion MZ: -1.007277")
        .WriteLine ("Precursor Ion N: 1")
        .WriteLine ("Search PPM: 10")
        .WriteLine ("Noise Filter: 0.05")
        .WriteLine ("m/z PPM: 15")
        .WriteLine ("Pattern Prediction: False")
    End With
    Settingsfile.Close
End If

Exit Sub

ErrorHandler:
MsgBox "PlantMAT is incompatiable with the current Excel or OS", vbCritical, "PlantMAT"
ThisWorkbook.Close

End Sub

Sub Settings_Reading()

Hex_w = 180.06338828
HexA_w = 194.04265285
dHex_w = 164.06847364
Pen_w = 150.05282357
Mal_w = 104.01095871
Cou_w = 164.04734422
Fer_w = 194.05790893
Sin_w = 224.06847364
DDMP_w = 144.04225873
CO2_w = 43.98982928
H2O_w = 18.01056471
H_w = 1.00782504
e_w = 0.00054858

NumSugarMin = 0
NumSugarMax = 0
NumHexMin = 0
NumHexMax = 0
NumHexAMin = 0
NumHexAMax = 0
NumdHexMin = 0
NumdHexMax = 0
NumPenMin = 0
NumPenMax = 0
NumMalMin = 0
NumMalMax = 0
NumCouMin = 0
NumCouMax = 0
NumFerMin = 0
NumFerMax = 0
NumSinMin = 0
NumSinMax = 0
NumDDMPMin = 0
NumDDMPMax = 0

Settingsfile = "C:\Users\" & Environ$("Username") & "\Documents\PlantMAT\Settings.txt"
Open Settingsfile For Input As #1

i = 0
ReDim AddedSugarAcid(0 To 10, 0 To 3)

Do Until EOF(1)
   Line Input #1, textLine
   posColon = InStr(textLine, ":")
   lenTextline = Len(textLine)
   txtTitle = Left(textLine, posColon - 1)
   txtValue = Right(textLine, lenTextline - posColon - 1)
   posSpace = InStr(txtValue, " ")
   If posSpace <> 0 Then
      lenValue = Len(txtValue)
      minValue = Left(txtValue, posSpace - 1)
      maxValue = Right(txtValue, lenValue - posSpace)
   End If
   If txtTitle = "Internal Aglycone Database" Then InternalAglyconeDatabase = txtValue
   If txtTitle = "External Aglycone Database" Then ExternalAglyconeDatabase = txtValue
   If txtTitle = "Aglycone Type" Then AglyconeType = txtValue
   If txtTitle = "Aglycone Source" Then AglyconeSource = txtValue
   If txtTitle = "Aglycone MW Range" Then AglyconeMWLL = minValue: AglyconeMWUL = maxValue
   If Left(txtTitle, 3) = "Num" Then
      If Left(txtTitle, 12) = "Num of Sugar" Then
         TypeSA = "Sugar"
         NameSA = Mid(txtTitle, 15, Len(txtTitle) - 15)
      Else
         TypeSA = "Acid"
         NameSA = Mid(txtTitle, 14, Len(txtTitle) - 14)
      End If
      AddedSugarAcid(i, 0) = NameSA
      AddedSugarAcid(i, 1) = TypeSA
      AddedSugarAcid(i, 2) = minValue
      AddedSugarAcid(i, 3) = maxValue
      If NameSA = "All" And TypeSA = "Sugar" Then NumSugarMin = minValue: NumSugarMax = maxValue
      If NameSA = "All" And TypeSA = "Acid" Then NumAcidMin = minValue: NumAcidMax = maxValue
      If NameSA = "Hex" Then NumHexMin = minValue: NumHexMax = maxValue
      If NameSA = "HexA" Then NumHexAMin = minValue: NumHexAMax = maxValue
      If NameSA = "dHex" Then NumdHexMin = minValue: NumdHexMax = maxValue
      If NameSA = "Pen" Then NumPenMin = minValue: NumPenMax = maxValue
      If NameSA = "Mal" Then NumMalMin = minValue: NumMalMax = maxValue
      If NameSA = "Cou" Then NumCouMin = minValue: NumCouMax = maxValue
      If NameSA = "Fer" Then NumFerMin = minValue: NumFerMax = maxValue
      If NameSA = "Sin" Then NumSinMin = minValue: NumSinMax = maxValue
      If NameSA = "DDMP" Then NumDDMPMin = minValue: NumDDMPMax = maxValue
      i = i + 1
   End If
   If txtTitle = "Precursor Ion Type" Then PrecursorIonType = txtValue
   If txtTitle = "Precursor Ion MZ" Then PrecursorIonMZ = txtValue
   If txtTitle = "Precursor Ion N" Then PrecursorIonN = txtValue
   If txtTitle = "Search PPM" Then SearchPPM = txtValue
   If txtTitle = "Noise Filter" Then NoiseFilter = txtValue
   If txtTitle = "m/z PPM" Then mzPPM = txtValue
   If txtTitle = "Pattern Prediction" Then PatternPrediction = txtValue
Loop

Close #1

End Sub

Sub StartProcessing(msg As String, code As String)

Processing_Message = msg
Macro_to_Process = code
Processing_Dialog.Show

End Sub

Sub Export(ExportRange As Worksheet, FirstRow As Long, LastRow As Long, FirstCol As Long, LastCol As Long)

On Error GoTo ErrorHandler

Dim ExportFile As String
ExportFile = Application.GetSaveAsFilename(fileFilter:="csv Files (*.csv), *.csv")
If ExportFile = "False" Then Exit Sub

Open ExportFile For Output As #1
For r = FirstRow To LastRow
    For c = FirstCol To LastCol
        Data = ExportRange.Cells(r, c).Value
        If IsNumeric(Data) = True Then Data = CStr(Data)
        If Data = "" Then Data = ""
        If c <> LastCol Then
           Write #1, Data;
        Else
           Write #1, Data
        End If
    Next c
Next r
Close #1

MsgBox "Data were exported to " & ExportFile, vbInformation, "PlantMAT"

Exit Sub

ErrorHandler:
MsgBox "Export failed", vbCritical, "PlantMAT"

End Sub

Sub Printout(PrintRange As Worksheet, TableHeader As String, FirstCell As String, LastRow As Long, LastCol As Long)

PrintRange.DisplayAutomaticPageBreaks = False
With PrintRange.PageSetup
     .PrintArea = PrintRange.Range(FirstCell & ":" & PrintRange.Cells(LastRow, LastCol).Address).Address
     .PrintTitleRows = TableHeader
     .Orientation = xlLandscape
     .Zoom = False
     .FitToPagesWide = 1
     .FitToPagesTall = False
End With
PrintRange.Printout

MsgBox "Data were sent to printer", vbInformation, "PlantMAT"

End Sub

Attribute VB_Name = "Import_Code"
'This module imports the peak list into Query Interface

Sub Button_Import()

'Click to select the peal list in TXT file
ListFile = Application.GetOpenFilename("Text Files (*.txt), *.txt", , "Select Peak List")
If ListFile = "False" Then Exit Sub

If Sheet4.Range("B4") <> "" Then
   a = MsgBox("Importing new data will delete" & vbNewLine & "all previous results! Continue?", vbYesNo, "PlantMAT")
   If a = vbNo Then Exit Sub
End If

'Import the data into Query Interface and show the importing progress (Data_Importing)
PublicVS_Code.StartProcessing "Now importing, please wait...", "Data_Importing"

ThisWorkbook.Save

'If no error is found, show the number of imported peaks in message box
If ErrorCheck = False Then MsgBox CStr(i - 4) & " peaks were imported", vbInformation, "PlantMAT"

End Sub

Sub Data_Importing()

On Error GoTo ErrorHandler

Application.ScreenUpdating = False
Application.EnableEvents = False

'Intialize the Query Interface (Clear all previous data and results if any)
Set Query = ThisWorkbook.Sheets("Query")
With Query
     .Unprotect
     .Cells.ClearComments
     LastRow = .Range("D" & Rows.Count).End(xlUp).Row
     If LastRow >= 4 Then .Range("B4:" & "Z" & LastRow) = ""
     .DropDowns().Delete
     .ScrollArea = ""
     For i = 8 To 16
         Query.Columns(i).Hidden = True
     Next i
End With

'Read peak list in TXT into data array and copy to Query Interface
i = 4
Open ListFile For Input As #1
Do Until EOF(1)
    DoEvents
    Line Input #1, LineText
    ePeak = Split(CStr(LineText), Chr(9))
    With Query
        .Cells(i, 2) = i - 3
        .Cells(i, 4) = ePeak(1) + 0
        .Range(Cells(i, 2), Cells(i, 5)).Font.Color = RGB(0, 0, 255)
    End With
    If i = 503 Then Exit Do
    i = i + 1
Loop
Close #1

'Enable the buttons for MS1 and MS2 analysis
With Query
     .Shapes("bt_MS1").OnAction = "Button_MS1"
     .Shapes("bt_MS1").DrawingObject.Font.ColorIndex = 1
     .Shapes("bt_MS2A").OnAction = "Button_MS2Annotation"
     .Shapes("bt_MS2A").DrawingObject.Font.ColorIndex = 1
     Application.Goto .Range("A1"), True
     .ScrollArea = "A4:Z" & CStr(i + 2)
     .Protect
End With

Application.EnableEvents = True
Application.ScreenUpdating = True

Exit Sub

'If error is found, go to ErrorHandler
ErrorHandler:
ErrorCheck = True
Close #1
Processing_Dialog.Hide
Call ErrorHandler.ErrorHanlder

End Sub

Attribute VB_Name = "Options_Code"
Sub Button_Options()

Call PublicVS_Code.Settings_Check
Call PublicVS_Code.Settings_Reading

With Settings_Dialog
     .cb_InternalAglyconeDatabase.Value = InternalAglyconeDatabase
     .tb_ExternalAglyconeDatabase.Value = ExternalAglyconeDatabase
     .db_AglyconeType.Value = AglyconeType
     .db_AglyconeSource.Value = AglyconeSource
     .tb_AglyconeMWLL.Value = AglyconeMWLL
     .tb_AglyconeMWUL.Value = AglyconeMWUL
     .tb_NumSugarMin.Value = AddedSugarAcid(0, 2)
     .tb_NumSugarMax.Value = AddedSugarAcid(0, 3)
     .tb_NumAcidMin.Value = AddedSugarAcid(1, 2)
     .tb_NumAcidMax.Value = AddedSugarAcid(1, 3)
     For j = 2 To i - 1
         With .lb_AddedSugarAcid
              .AddItem
              .List(j - 2, 0) = AddedSugarAcid(j, 0)
              .List(j - 2, 1) = AddedSugarAcid(j, 1)
              .List(j - 2, 2) = AddedSugarAcid(j, 2)
              .List(j - 2, 3) = AddedSugarAcid(j, 3)
         End With
     Next j
     .db_PrecursorIon.Value = PrecursorIonType
     .tb_Searchppm.Value = SearchPPM
     .tb_NoiseFilter.Value = NoiseFilter
     .tb_Mzppm.Value = mzPPM
     .cb_PatternPrediction.Value = PatternPrediction
End With

Settings_Dialog.Show

End Sub

Attribute VB_Name = "SingleQ_Code"
Sub Button_SingleQ()

Call PublicVS_Code.Settings_Reading

If PatternPrediction = False Then
   With SingleQ_Dialog.cb_MS2Prediction
        .Value = False
        .Enabled = False
   End With
End If

SingleQ_Dialog.Show

End Sub

Attribute VB_Name = "Enter_Code"
Sub Button_SystemEnter()

'Click to enter and initialize all interfaces
    Application.ScreenUpdating = False
    ThisWorkbook.Unprotect

    With Sheet2
        .Visible = True
        .Activate
        .Range("C8").Select
        .ScrollArea = "C8"
    End With

    Sheet1.Visible = False

    With Sheet3
        LastRow = .Range("B" & Rows.Count).End(xlUp).Row
        .ScrollArea = "B3:B" & CStr(LastRow + 2)
        .Visible = True
    End With
    
    Dim comb As Object
    With Sheet4
        .Unprotect
        For Each comb In .DropDowns()
            n = comb.ListCount
            comb.Text = CStr(n) & " results"
        Next
        LastRow = .Range("D" & Rows.Count).End(xlUp).Row
        .ScrollArea = "A4:Z" & CStr(LastRow + 2)
        .Protect
    End With
    Sheet4.Visible = True
    
    Sheet5.Visible = False

    Sheet2.Activate

    ThisWorkbook.Protect
    ThisWorkbook.Save
    Application.ScreenUpdating = True

End Sub


Attribute VB_Name = "ErrorHandler"
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
        .Shapes("bt_MS1").OnAction = Empty
        .Shapes("bt_MS1").DrawingObject.Font.ColorIndex = 16
        .Shapes("bt_MS2A").OnAction = Empty
        .Shapes("bt_MS2A").DrawingObject.Font.ColorIndex = 16
        Application.Goto .Range("A1"), True
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