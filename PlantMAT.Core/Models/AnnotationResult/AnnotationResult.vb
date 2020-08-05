#Region "Microsoft.VisualBasic::a3a0d1898adae33b166d1447011145bd, PlantMAT.Core\Models\AnnotationResult\AnnotationResult.vb"

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

    '     Class CandidateResult
    ' 
    '         Properties: Cou, DDMP, dHex, Err, ExactMass
    '                     Fer, Glycosyl, Hex, HexA, Mal
    '                     Ms2Anno, Name, Pen, precursor_type, RT
    '                     RTErr, Sin, SMILES, SubstructureAgly
    ' 
    '         Function: GetSug_nStatic
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Models.AnnotationResult

    Public Class CandidateResult

        Public Property precursor_type As String

        ''' <summary>
        ''' 0
        ''' </summary>
        ''' <returns></returns>
        Public Property ExactMass As Double
        ''' <summary>
        ''' 1
        ''' </summary>
        ''' <returns></returns>
        Public Property SubstructureAgly As String
        ''' <summary>
        ''' 2
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String
        ''' <summary>
        ''' 3
        ''' </summary>
        ''' <returns></returns>
        Public Property Hex As Double
        ''' <summary>
        ''' 4
        ''' </summary>
        ''' <returns></returns>
        Public Property HexA As Double
        ''' <summary>
        ''' 5
        ''' </summary>
        ''' <returns></returns>
        Public Property dHex As Double
        ''' <summary>
        ''' 6
        ''' </summary>
        ''' <returns></returns>
        Public Property Pen As Double
        ''' <summary>
        ''' 7
        ''' </summary>
        ''' <returns></returns>
        Public Property Mal As Double
        ''' <summary>
        ''' 8
        ''' </summary>
        ''' <returns></returns>
        Public Property Cou As Double
        ''' <summary>
        ''' 9
        ''' </summary>
        ''' <returns></returns>
        Public Property Fer As Double
        ''' <summary>
        ''' 10
        ''' </summary>
        ''' <returns></returns>
        Public Property Sin As Double
        ''' <summary>
        ''' 11
        ''' </summary>
        ''' <returns></returns>
        Public Property DDMP As Double
        ''' <summary>
        ''' 12
        ''' </summary>
        ''' <returns></returns>
        Public Property Err As Double
        ''' <summary>
        ''' 13
        ''' </summary>
        ''' <returns></returns>
        Public Property RT As Double
        ''' <summary>
        ''' 14
        ''' </summary>
        ''' <returns></returns>
        Public Property RTErr As Double

        Public Property SMILES As New List(Of SMILES)

        Public Property Ms2Anno As Ms2IonAnnotations
        Public Property Glycosyl As Glycosyl

        Public Function GetSug_nStatic() As Double()
            ' 3 - 11
            Return {Hex, HexA, dHex, Pen, Mal, Cou, Fer, Sin, DDMP}
        End Function

    End Class
End Namespace
