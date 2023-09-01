#Region "Microsoft.VisualBasic::1a87091b3b4086faddb6170e521c688f, PlantMAT.Core\Models\Library.vb"

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

    '     Class Library
    ' 
    '         Properties: [Class], [Date], CommonName, Editor, ExactMass
    '                     Formula, Genus, Type, Universal_SMILES, Xref
    ' 
    '         Function: LoadStream, ToString, WriteToStream
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Namespace Models

    Public Class Library

        ''' <summary>
        ''' the unique reference id of current library item
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(0)> Public Property Xref As String

        <Column("Common Name")>
        <MessagePackMember(1)> Public Property CommonName As String
        <MessagePackMember(2)> Public Property [Class] As String
        <MessagePackMember(3)> Public Property Type As String
        <MessagePackMember(4)> Public Property Formula As String

        <Column("Exact Mass")>
        <MessagePackMember(5)> Public Property ExactMass As Double
        <MessagePackMember(6)> Public Property Genus As String

        ''' <summary>
        ''' Universal SMILES
        ''' </summary>
        ''' <returns></returns>
        <Column("Universal SMILES")>
        <MessagePackMember(7)> Public Property Universal_SMILES As String
        <MessagePackMember(8)> Public Property Editor As String
        <MessagePackMember(9)> Public Property [Date] As Date

        Public Overrides Function ToString() As String
            Return CommonName
        End Function

        Public Shared Function LoadStream(file As Stream) As Library()
            Return MsgPackSerializer.Deserialize(Of Library())(file)
        End Function

        Public Shared Function WriteToStream([lib] As IEnumerable(Of Library), file As Stream) As Boolean
            Call MsgPackSerializer.SerializeObject([lib].ToArray, file)
            Return True
        End Function

    End Class
End Namespace
