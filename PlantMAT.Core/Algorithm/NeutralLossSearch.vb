#Region "Microsoft.VisualBasic::e8a9006e5b4a7809c043ce4295cdf795, PlantMAT.Core\Algorithm\NeutralLossSearch.vb"

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

'     Class NeutralLossSearch
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: NeutralLosses, RestrictionCheck
' 
'         Sub: applySettings
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.Language
Imports PlantMAT.Core.Models

Namespace Algorithm

    Public Class NeutralLossSearch : Inherits PlantMATAlgorithm

#Region "Search Space"
        Dim NumHexMin, NumHexMax, NumHexAMin, NumHexAMax, NumdHexMin, NumdHexMax, NumPenMin, NumPenMax, NumMalMin, NumMalMax, NumCouMin, NumCouMax, NumFerMin, NumFerMax, NumSinMin, NumSinMax, NumDDMPMin, NumDDMPMax As Integer
        Dim NumSugarMin, NumSugarMax, NumAcidMin, NumAcidMax As Integer
#End Region

        ReadOnly externalDefines As NeutralGroup()

        Public Sub New(settings As Settings, externalDefines As NeutralGroup())
            MyBase.New(settings)

            Me.externalDefines = externalDefines
        End Sub

        Protected Friend Overrides Sub applySettings()
            Const min = 0
            Const max = 1

            NumHexMin = settings.NumofSugarHex(min) : NumHexMax = settings.NumofSugarHex(max)
            NumHexAMin = settings.NumofSugarHexA(min) : NumHexAMax = settings.NumofSugarHexA(max)
            NumdHexMin = settings.NumofSugardHex(min) : NumdHexMax = settings.NumofSugardHex(max)
            NumPenMin = settings.NumofSugarPen(min) : NumPenMax = settings.NumofSugarPen(max)
            NumMalMin = settings.NumofAcidMal(min) : NumMalMax = settings.NumofAcidMal(max)
            NumCouMin = settings.NumofAcidCou(min) : NumCouMax = settings.NumofAcidCou(max)
            NumFerMin = settings.NumofAcidFer(min) : NumFerMax = settings.NumofAcidFer(max)
            NumSinMin = settings.NumofAcidSin(min) : NumSinMax = settings.NumofAcidSin(max)
            NumDDMPMin = settings.NumofAcidDDMP(min) : NumDDMPMax = settings.NumofAcidDDMP(max)

            NumSugarMin = settings.NumofSugarAll(min) : NumSugarMax = settings.NumofSugarAll(max)
            NumAcidMin = settings.NumofAcidAll(min) : NumAcidMax = settings.NumofAcidAll(max)
        End Sub

        ''' <summary>
        ''' Do brute force iteration to generate all hypothetical neutral losses
        ''' </summary>
        ''' <param name="precursorIon">
        ''' The ms1 precursor ion its m/z value
        ''' </param>
        ''' <param name="precursor">
        ''' The precursor type information
        ''' </param>
        ''' <returns></returns>
        Public Iterator Function NeutralLosses(precursorIon As Double, precursor As PrecursorInfo) As IEnumerable(Of NeutralLoss)
            Dim PrecursorIonMZ As Double = precursor.adduct
            Dim PrecursorIonN As Double = precursor.M
            Dim M_w As Double = (precursorIon - PrecursorIonMZ) / PrecursorIonN
            Dim checkLoss As New Value(Of NeutralLoss)
            Dim combination As New BruteForceCombination(externalDefines, NumSugarMax:=NumSugarMax, NumAcidMax:=NumAcidMax)

            ' invali exact mass that calculated from the precursor ion
            If M_w <= 0 OrElse M_w > 2000 Then
                Return
            End If

            ' 暴力枚举的方法来搜索代谢物信息
            For Hex_n As Integer = NumHexMin To NumHexMax
                For HexA_n As Integer = NumHexAMin To NumHexAMax
                    For dHex_n As Integer = NumdHexMin To NumdHexMax
                        For Pen_n As Integer = NumPenMin To NumPenMax
                            For Mal_n As Integer = NumMalMin To NumMalMax
                                For Cou_n As Integer = NumCouMin To NumCouMax
                                    For Fer_n As Integer = NumFerMin To NumFerMax
                                        For Sin_n As Integer = NumSinMin To NumSinMax
                                            For DDMP_n As Integer = NumDDMPMin To NumDDMPMax

                                                For Each check As NeutralLoss In combination.BruteForceIterations(
                                                    Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%,
 _
                                                    iteration:=Function(loss)
                                                                   If RestrictionCheck(neutralLoss:=loss, M_w:=M_w) Then
                                                                       Return loss
                                                                   Else
                                                                       Return Nothing
                                                                   End If
                                                               End Function)

                                                    If Not (checkLoss = check) Is Nothing Then
                                                        Yield checkLoss
                                                    End If
                                                Next

                                            Next DDMP_n
                                        Next Sin_n
                                    Next Fer_n
                                Next Cou_n
                            Next Mal_n
                        Next Pen_n
                    Next dHex_n
                Next HexA_n
            Next Hex_n
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="neutralLoss"></param>
        ''' <param name="M_w">exact mass</param>
        ''' <returns></returns>
        Private Function RestrictionCheck(neutralLoss As NeutralLoss, M_w As Double) As Boolean
            Dim Sugar_n As Integer = neutralLoss.Sugar_n
            Dim Acid_n As Integer = neutralLoss.Acid_n

            If Sugar_n >= NumSugarMin AndAlso Sugar_n <= NumSugarMax AndAlso Acid_n >= NumAcidMin AndAlso Acid_n <= NumAcidMax Then
                Dim Attn_w As Double = neutralLoss.Attn_w
                Dim nH2O_w As Double = (Sugar_n + Acid_n) * H2O_w
                Dim Bal As Double = neutralLoss.AglyconeExactMass(M_w)

                ' "Aglycone MW Range" Then AglyconeMWLL = minValue : AglyconeMWUL = maxValue
                If settings.AglyconeExactMassInRange(Bal) Then
                    Return True
                End If
            End If

            Return False
        End Function
    End Class
End Namespace
