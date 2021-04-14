#Region "Microsoft.VisualBasic::19f3d754942ed24973624556aae4c62e, PlantMAT.Core\Algorithm\NeutralLoss.vb"

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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports PlantMAT.Core.Models

Namespace Algorithm

    Public Class NeutralLoss

        <MessagePackMember(0)> <XmlAttribute> Public Property Hex As Integer
        <MessagePackMember(1)> <XmlAttribute> Public Property HexA As Integer
        <MessagePackMember(2)> <XmlAttribute> Public Property dHex As Integer
        <MessagePackMember(3)> <XmlAttribute> Public Property Pen As Integer
        <MessagePackMember(4)> <XmlAttribute> Public Property Mal As Integer
        <MessagePackMember(5)> <XmlAttribute> Public Property Cou As Integer
        <MessagePackMember(6)> <XmlAttribute> Public Property Fer As Integer
        <MessagePackMember(7)> <XmlAttribute> Public Property Sin As Integer
        <MessagePackMember(8)> <XmlAttribute> Public Property DDMP As Integer
        <MessagePackMember(9)>
        Public Property externals As NeutralGroupHit()

        Public ReadOnly Property Sugar_n As Integer
            Get
                Return Hex + HexA + dHex + Pen + nCount(externals, type:=NeutralTypes.suger)
            End Get
        End Property

        Public ReadOnly Property Acid_n As Integer
            Get
                Return Mal + Cou + Fer + Sin + DDMP + nCount(externals, type:=NeutralTypes.acid)
            End Get
        End Property

        Public Shared Function nMax(Hex%, HexA%, dHex%, Pen%, Mal%, Cou%, Fer%, Sin%, DDMP%, externals As NeutralGroup()) As (sugarMax%, acidMax%)
            Dim sugarMax = Hex + HexA + dHex + Pen + nCount(externals, type:=NeutralTypes.suger)
            Dim acidMax = Mal + Cou + Fer + Sin + DDMP + nCount(externals, type:=NeutralTypes.acid)

            Return (sugarMax, acidMax)
        End Function

        ''' <summary>
        ''' Sum(X * n(X))
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Attn_w As Double
            Get
                Return Hex * Hex_w + HexA * HexA_w + dHex * dHex_w + Pen * Pen_w + Mal * Mal_w + Cou * Cou_w + Fer * Fer_w + Sin * Sin_w + DDMP * DDMP_w + weightPlus()
            End Get
        End Property

        Public ReadOnly Property nH2O As Integer
            Get
                Return (Sugar_n + Acid_n)
            End Get
        End Property

        Public ReadOnly Property nH2O_w As Double
            Get
                Return (Sugar_n + Acid_n) * H2O_w
            End Get
        End Property

        Private Function weightPlus() As Double
            Return Aggregate item In externals.SafeQuery Into Sum(item.MassTotal)
        End Function

        Friend Shared Function nCount(Of T As INeutralGroupHit)(externals As T(), type As NeutralTypes) As Integer
            Return Aggregate item In externals.SafeQuery Where item.type = type Into Sum(item.nHit)
        End Function

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

        Friend Function SetExternalCount(counts As IEnumerable(Of NeutralGroupHit)) As NeutralLoss
            Me.externals = NeutralGroupHit.CopyVector(counts)
            Return Me
        End Function

        ''' <summary>
        ''' AglyconeExactMass = exactMass + nH2O_w - Attn_w
        ''' </summary>
        ''' <param name="exactMass"></param>
        ''' <returns></returns>
        Public Function AglyconeExactMass(exactMass As Double) As Double
            Return exactMass + nH2O_w - Attn_w
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="aglyconeExactMass"></param>
        ''' <param name="exactMass"></param>
        ''' <returns>
        ''' returns the value of ``nH2O_w - Attn_w``
        ''' </returns>
        Public Shared Function TargetMass(aglyconeExactMass As Double, exactMass As Double) As Double
            ' aglyconeExactMass = exactMass + nH2O_w - Attn_w
            ' nH2O_w - Attn_w = aglyconeExactMass - exactMass
            Return aglyconeExactMass - exactMass
        End Function

        Public Overrides Function ToString() As String
            Return $"X + {nH2O_w} - {Attn_w}"
        End Function

    End Class
End Namespace
