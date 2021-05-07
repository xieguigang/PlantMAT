#Region "Microsoft.VisualBasic::246ad58724fc9026cc406ecde2ec3b55, PlantMAT.Core\Models\AnnotationResult\AnnotationResult.vb"

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
'         Properties: Acid_n, Attn_w, Cou, DDMP, dHex
'                     Err, ExactMass, Fer, Glycosyl, Hex
'                     HexA, Mal, Ms2Anno, Name, nH2O_w
'                     Pen, precursor_type, RT, RTErr, Sin
'                     SMILES, SubstructureAgly, Sugar_n, Theoretical_ExactMass, Theoretical_PrecursorMz
'                     xref
' 
'         Constructor: (+2 Overloads) Sub New
'         Function: GetSug_nStatic
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq

Namespace Models.AnnotationResult

    Public Class CandidateResult

        Public Property xref As String
        Public Property precursor_type As String

        ''' <summary>
        ''' 0
        ''' </summary>
        ''' <returns></returns>
        Public Property ExactMass As Double
        Public Property Theoretical_ExactMass As Double
        Public Property Theoretical_PrecursorMz As Double

        ''' <summary>
        ''' 1 Universal SMILES from the library information
        ''' </summary>
        ''' <returns></returns>
        Public Property SubstructureAgly As String
        ''' <summary>
        ''' 2 metabolite common name
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String
        ''' <summary>
        ''' 3
        ''' </summary>
        ''' <returns></returns>
        Public Property Hex As Integer
        ''' <summary>
        ''' 4
        ''' </summary>
        ''' <returns></returns>
        Public Property HexA As Integer
        ''' <summary>
        ''' 5
        ''' </summary>
        ''' <returns></returns>
        Public Property dHex As Integer
        ''' <summary>
        ''' 6
        ''' </summary>
        ''' <returns></returns>
        Public Property Pen As Integer
        ''' <summary>
        ''' 7
        ''' </summary>
        ''' <returns></returns>
        Public Property Mal As Integer
        ''' <summary>
        ''' 8
        ''' </summary>
        ''' <returns></returns>
        Public Property Cou As Integer
        ''' <summary>
        ''' 9
        ''' </summary>
        ''' <returns></returns>
        Public Property Fer As Integer
        ''' <summary>
        ''' 10
        ''' </summary>
        ''' <returns></returns>
        Public Property Sin As Integer
        ''' <summary>
        ''' 11
        ''' </summary>
        ''' <returns></returns>
        Public Property DDMP As Integer
        ''' <summary>
        ''' 12 mass error on ms1 ion
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

        Public Property Sugar_n As Integer
        Public Property Acid_n As Integer

        Public Property Attn_w As Double
        Public Property nH2O_w As Double

        Public Property SMILES As String()
        Public Property Ms2Anno As Ms2IonAnnotations
        Public Property Glycosyl As Glycosyl

        Sub New()
        End Sub

        Sub New(clone As CandidateResult)
            Me.Acid_n = clone.Acid_n
            Me.Attn_w = clone.Attn_w
            Me.Cou = clone.Cou
            Me.DDMP = clone.DDMP
            Me.dHex = clone.dHex
            Me.Err = clone.Err
            Me.ExactMass = clone.ExactMass
            Me.Fer = clone.Fer

            If Not clone.Glycosyl Is Nothing Then
                Me.Glycosyl = New Glycosyl With {
                    .Match_m = clone.Glycosyl.Match_m,
                    .Match_n = clone.Glycosyl.Match_n,
                    .Pred_n = clone.Glycosyl.Match_n,
                    .pResult = clone.Glycosyl.pResult _
                        .SafeQuery _
                        .Select(Function(a)
                                    Return New GlycosylPredition With {
                                        .score = a.score,
                                        .struct = a.struct
                                    }
                                End Function) _
                        .ToArray
                }
            End If

            Me.Hex = clone.Hex
            Me.HexA = clone.HexA
            Me.Mal = clone.Mal
            Me.Ms2Anno = clone.Ms2Anno
            Me.Name = clone.Name
            Me.nH2O_w = clone.nH2O_w
            Me.Pen = clone.Pen
            Me.precursor_type = clone.precursor_type
            Me.RT = clone.RT
            Me.RTErr = clone.RTErr
            Me.Sin = clone.Sin
            Me.SMILES = clone.SMILES.SafeQuery.ToArray
            Me.SubstructureAgly = clone.SubstructureAgly
            Me.Sugar_n = clone.Sugar_n
            Me.Theoretical_ExactMass = clone.Theoretical_ExactMass
            Me.Theoretical_PrecursorMz = clone.Theoretical_PrecursorMz
            Me.xref = clone.xref
        End Sub

        ''' <summary>
        ''' returns a count vector in order of
        ''' 
        ''' ```
        ''' Hex, HexA, dHex, Pen, Mal, Cou, Fer, Sin, DDMP
        ''' ```
        ''' </summary>
        ''' <returns></returns>
        Public Function GetSug_nStatic() As Integer()
            ' 3 - 11
            Return {Hex, HexA, dHex, Pen, Mal, Cou, Fer, Sin, DDMP}
        End Function

    End Class
End Namespace
