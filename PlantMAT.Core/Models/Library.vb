#Region "Microsoft.VisualBasic::71859404a4796f150bbbd0f0a382c567, PlantMAT.Core\Models\Library.vb"

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
    '                     Formula, Genus, Type, Universal_SMILES
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace Models

    Public Class Library

        <Column("Common Name")>
        Public Property CommonName As String
        Public Property [Class] As String
        Public Property Type As String
        Public Property Formula As String

        <Column("Exact Mass")>
        Public Property ExactMass As Double
        Public Property Genus As String

        <Column("Universal SMILES")>
        Public Property Universal_SMILES As String
        Public Property Editor As String
        Public Property [Date] As Date

        Public Overrides Function ToString() As String
            Return CommonName
        End Function

    End Class

End Namespace
