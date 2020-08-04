#Region "Microsoft.VisualBasic::9b3c7ec5740c59459e43a71e2319ae0d, PlantMAT\Code\ErrorHandler.vb"

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

'Module ErrorHandler

'    ' Attribute VB_Name = "ErrorHandler"
'    Sub ErrorHanlder()

'        'If data error is found in the peak list, then show warning message and
'        'reset all fields in the output display
'        MsgBox "Import failed. Please check your data and try again.", vbCritical, "PlantMAT"

'    Application.ScreenUpdating = False
'    Set Query = ThisWorkbook.Sheets("Query")
'    With Query
'            .Unprotect
'            LastRow = .Range("B" & Rows.Count).End(xlUp).Row
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

'    End Sub
'End Module
