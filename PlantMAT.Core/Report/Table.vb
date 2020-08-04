#Region "Microsoft.VisualBasic::a6c048662710cf1b7d3572d479dbe427, PlantMAT.Core\Report\Table.vb"

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

    '     Class Table
    ' 
    '         Properties: [structure], accession, candidate, cou, DDMP
    '                     dhex, err, exact_mass, fer, glycosyl1
    '                     glycosyl2, glycosyl3, glycosyl4, glycosyl5, hex
    '                     hexA, ion1, ion2, ion3, ion4
    '                     ion5, mal, mz, peakNO, pen
    '                     precursor_type, rt, sin, stats, topMs2
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Report

    Public Class Table

        Public Property peakNO As Integer
        Public Property accession As String
        Public Property mz As Double
        Public Property rt As Double
        Public Property topMs2 As Double()
        Public Property stats As String
        Public Property candidate As String
        Public Property exact_mass As Double
        Public Property precursor_type As String
        Public Property [structure] As String
        Public Property err As Double
        Public Property cou As Integer
        Public Property DDMP As Integer
        Public Property fer As Integer
        Public Property hex As Integer
        Public Property hexA As Integer
        Public Property mal As Integer
        Public Property pen As Integer
        Public Property sin As Integer
        Public Property dhex As Integer

        Public Property ion1 As String
        Public Property ion2 As String
        Public Property ion3 As String
        Public Property ion4 As String
        Public Property ion5 As String

        Public Property glycosyl1 As String
        Public Property glycosyl2 As String
        Public Property glycosyl3 As String
        Public Property glycosyl4 As String
        Public Property glycosyl5 As String

        Public Overrides Function ToString() As String
            Return $"Dim {accession} As {candidate}.{precursor_type}"
        End Function
    End Class
End Namespace
