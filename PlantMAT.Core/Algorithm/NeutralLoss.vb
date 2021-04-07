﻿#Region "Microsoft.VisualBasic::19f3d754942ed24973624556aae4c62e, PlantMAT.Core\Algorithm\NeutralLoss.vb"

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

    '     Class NeutralLoss
    ' 
    '         Properties: Acid_n, Attn_w, nH2O_w, Sugar_n
    ' 
    '         Function: AglyconeExactMass, SetLoess, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Algorithm

    Public Class NeutralLoss

        Public Property Hex As Integer
        Public Property HexA As Integer
        Public Property dHex As Integer
        Public Property Pen As Integer
        Public Property Mal As Integer
        Public Property Cou As Integer
        Public Property Fer As Integer
        Public Property Sin As Integer
        Public Property DDMP As Integer

        Public ReadOnly Property Sugar_n As Integer
            Get
                Return Hex + HexA + dHex + Pen
            End Get
        End Property

        Public ReadOnly Property Acid_n As Integer
            Get
                Return Mal + Cou + Fer + Sin + DDMP
            End Get
        End Property

        Public ReadOnly Property Attn_w As Double
            Get
                Return Hex * Hex_w + HexA * HexA_w + dHex * dHex_w + Pen * Pen_w + Mal * Mal_w + Cou * Cou_w + Fer * Fer_w + Sin * Sin_w + DDMP * DDMP_w
            End Get
        End Property

        Public ReadOnly Property nH2O_w As Double
            Get
                Return (Sugar_n + Acid_n) * H2O_w
            End Get
        End Property

        Friend Function SetLoess(Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%) As NeutralLoss
            Hex = Hex_n
            HexA = HexA_n
            dHex = dHex_n
            Pen = Pen_n
            Mal = Mal_n
            Cou = Cou_n
            Fer = Fer_n
            Sin = Sin_n
            DDMP = DDMP_n

            Return Me
        End Function

        Public Function AglyconeExactMass(exactMass As Double) As Double
            Return exactMass + nH2O_w - Attn_w
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace
