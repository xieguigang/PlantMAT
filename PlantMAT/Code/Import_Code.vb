#Region "Microsoft.VisualBasic::4d3e0ebc698c9480bb1ed0b2ea2d5e21, PlantMAT\Code\Import_Code.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    '       Feng Qiu (fengqiu1982)
    ' 
    ' Copyright (c) 2020 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' Apache 2.0 License
    ' 
    ' 
    ' Copyright 2020 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' Licensed under the Apache License, Version 2.0 (the "License");
    ' you may not use this file except in compliance with the License.
    ' You may obtain a copy of the License at
    ' 
    '     http://www.apache.org/licenses/LICENSE-2.0
    ' 
    ' Unless required by applicable law or agreed to in writing, software
    ' distributed under the License is distributed on an "AS IS" BASIS,
    ' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    ' See the License for the specific language governing permissions and
    ' limitations under the License.



    ' /********************************************************************************/

    ' Summaries:

    ' 
    ' /********************************************************************************/

#End Region

'Module Import_Code

'    ' Attribute VB_Name = "Import_Code"
'    'This module imports the peak list into Query Interface

'    Sub Button_Import()

'        'Click to select the peal list in TXT file
'        ListFile = Application.GetOpenFilename("Text Files (*.txt), *.txt", , "Select Peak List")
'        If ListFile = "False" Then Exit Sub

'        If Sheet4.Range("B4") <> "" Then
'            a = MsgBox("Importing new data will delete" & vbNewLine & "all previous results! Continue?", vbYesNo, "PlantMAT")
'            If a = vbNo Then Exit Sub
'        End If

'        'Import the data into Query Interface and show the importing progress (Data_Importing)
'        PublicVS_Code.StartProcessing "Now importing, please wait...", "Data_Importing"

'ThisWorkbook.Save

'        'If no error is found, show the number of imported peaks in message box
'        If ErrorCheck = False Then MsgBox CStr(i - 4) & " peaks were imported", vbInformation, "PlantMAT"

'End Sub

'    Sub Data_Importing()

'        On Error GoTo ErrorHandler

'        Application.ScreenUpdating = False
'        Application.EnableEvents = False

''Intialize the Query Interface (Clear all previous data and results if any)
'Set Query = ThisWorkbook.Sheets("Query")
'With Query
'            .Unprotect
'            .Cells.ClearComments
'            LastRow = .Range("D" & Rows.Count).End(xlUp).Row
'            If LastRow >= 4 Then .Range("B4:" & "Z" & LastRow) = ""
'            .DropDowns().Delete
'            .ScrollArea = ""
'            For i = 8 To 16
'                Query.Columns(i).Hidden = True
'            Next i
'        End With

'        'Read peak list in TXT into data array and copy to Query Interface
'        i = 4
'        Open ListFile For Input As #1
'Do Until EOF(1)
'            DoEvents
'            Line Input #1, LineText
'    ePeak = Split(CStr(LineText), Chr(9))
'            With Query
'                .Cells(i, 2) = i - 3
'                .Cells(i, 4) = ePeak(1) + 0
'                .Range(Cells(i, 2), Cells(i, 5)).Font.Color = RGB(0, 0, 255)
'            End With
'            If i = 503 Then Exit Do
'            i = i + 1
'        Loop
'        Close #1

''Enable the buttons for MS1 and MS2 analysis
'With Query
'            .Shapes("bt_MS1").OnAction = "Button_MS1"
'            .Shapes("bt_MS1").DrawingObject.Font.ColorIndex = 1
'            .Shapes("bt_MS2A").OnAction = "Button_MS2Annotation"
'            .Shapes("bt_MS2A").DrawingObject.Font.ColorIndex = 1
'            Application.Goto.Range("A1"), True
'     .ScrollArea = "A4:Z" & CStr(i + 2)
'            .Protect
'        End With

'        Application.EnableEvents = True
'        Application.ScreenUpdating = True

'        Exit Sub

'        'If error is found, go to ErrorHandler
'ErrorHandler:
'        ErrorCheck = True
'        Close #1
'Processing_Dialog.Hide
'        Call ErrorHandler.ErrorHanlder

'    End Sub
'End Module

