#Region "Microsoft.VisualBasic::d7e3aaa70812bd8a162f45319804c1e4, PlantMAT\Code\Enter_Code.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    '       Feng Qiu (fengqiu1982 https://sourceforge.net/u/fengqiu1982/)
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
