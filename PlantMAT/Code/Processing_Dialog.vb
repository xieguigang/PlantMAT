'Module Processing_Dialog


'    'Attribute VB_Name = "Processing_Dialog"
'    'Attribute VB_Base = "0{80811597-4C57-43F0-9AE9-0C8A34BE5E8F}{A0BE763D-4584-45AF-91A2-118DBE282DA5}"
'    'Attribute VB_GlobalNameSpace = False
'    'Attribute VB_Creatable = False
'    'Attribute VB_PredeclaredId = True
'    'Attribute VB_Exposed = False
'    'Attribute VB_TemplateDerived = False
'    'Attribute VB_Customizable = False
'    Private Sub UserForm_Activate()

'        Me.Repaint                               'Refresh the UserForm
'        Application.Run Macro_to_Process         'Run the macro
'        Unload Me                                'Unload the UserForm

'    End Sub

'    Private Sub UserForm_Initialize()

'        lblmessage.Caption = Processing_Message  'Change the Label Caption

'    End Sub

'    Private Sub UserForm_QueryClose(Cancel As Integer, CloseMode As Integer)

'        If CloseMode = vbFormControlMenu Then
'            Cancel = True
'        End If

'    End Sub
'End Module
