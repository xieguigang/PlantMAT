#Region "Microsoft.VisualBasic::88582e507b84d319b6f2fe1a1f1152c0, PlantMAT.Core\Models\AnnotationResult\SMILES.vb"

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

    '     Class SMILES
    ' 
    '         Properties: GlycN, GlycS, peakNo, Sequence
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Models.AnnotationResult

    Public Class SMILES

        ''' <summary>
        ''' 2
        ''' </summary>
        ''' <returns></returns>
        Public Property peakNo As Integer
        ''' <summary>
        ''' 3
        ''' </summary>
        ''' <returns></returns>
        Public Property Sequence As String
        ''' <summary>
        ''' 4
        ''' </summary>
        ''' <returns></returns>
        Public Property GlycN As String
        Public Property GlycS As String

    End Class
End Namespace
