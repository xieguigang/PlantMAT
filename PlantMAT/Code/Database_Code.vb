#Region "Microsoft.VisualBasic::bb0dc435c52928f0291e256b8041671b, PlantMAT\Code\Database_Code.vb"

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

'Module Database_Code

'    'Sub Button_DataAdd()

'    '    'Show diaglog box for data entry
'    '    Database_Dialog.Show

'    'End Sub

'    Sub Button_DataDelete()

'        'Delete data entry; Ask to confirm before deletion
'        Database = ThisWorkbook.Worksheets("Library")
'        LastRow = Database.Range("B" & Rows.Count).End(xlUp).Row
'        FindRow = ActiveCell.Row
'        If Database.Range("I" & FindRow) = "fqiu" Then
'            MsgBox "Current entry is protected and cannot be deleted", vbInformation, "PlantMAT"
'Else
'            Dim a = MsgBox("Delete current entry?", vbYesNo, "PlantMAT")
'            If a = vbYes Then
'                With Database
'                    .Unprotect
'                    .Rows(FindRow).Delete
'                    .ScrollArea = "B3:B" & CStr(LastRow + 1)
'                    .Protect
'                End With
'                ThisWorkbook.Save
'                MsgBox "Current entry was deleted", vbInformation, "PlantMAT"
'   End If
'        End If

'    End Sub

'    Sub Button_DataExport()

''Export the whole library to CSV file
'Set Database = ThisWorkbook.Sheets("Library")
'i = Database.Range("B" & Rows.Count).End(xlUp).Row
'        Call PublicVS_Code.Export(Database, 2, i, 2, 10)

'    End Sub
'End Module

