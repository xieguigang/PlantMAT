﻿#Region "Microsoft.VisualBasic::e8a9006e5b4a7809c043ce4295cdf795, PlantMAT.Core\Algorithm\NeutralLossSearch.vb"

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

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports PlantMAT.Core.Models
Imports std = System.Math

Namespace Algorithm

    Public Class NeutralLossSearch : Inherits PlantMATAlgorithm

#Region "Search Space"
        Dim NumHexMin, NumHexMax As Integer
        Dim NumHexAMin, NumHexAMax As Integer
        Dim NumdHexMin, NumdHexMax As Integer
        Dim NumPenMin, NumPenMax As Integer
        Dim NumMalMin, NumMalMax As Integer
        Dim NumCouMin, NumCouMax As Integer
        Dim NumFerMin, NumFerMax As Integer
        Dim NumSinMin, NumSinMax As Integer
        Dim NumDDMPMin, NumDDMPMax As Integer
        Dim NumSugarMin, NumSugarMax As Integer
        Dim NumAcidMin, NumAcidMax As Integer
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
            Dim combination As BruteForceCombination = createAlgorithm()

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
                                                    M_w:=M_w,
                                                    iteration:=Function(loss)
                                                                   If RestrictionCheck(neutralLoss:=loss, M_w:=M_w) Then
                                                                       loss = New NeutralLoss With {
                                                                           .Cou = loss.Cou,
                                                                           .DDMP = loss.DDMP,
                                                                           .dHex = loss.dHex,
                                                                           .Fer = loss.Fer,
                                                                           .Hex = loss.Hex,
                                                                           .HexA = loss.HexA,
                                                                           .Mal = loss.Mal,
                                                                           .Pen = loss.Pen,
                                                                           .Sin = loss.Sin,
                                                                           .externals = NeutralGroupHit.CopyVector(loss.externals)
                                                                       }

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

        Private Function createAlgorithm() As BruteForceCombination
            Return New BruteForceCombination(externalDefines, NumSugarMax:=NumSugarMax, NumAcidMax:=NumAcidMax, settings.AglyconeMWRange(0))
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="aglycones">custom aglycone candidates</param>
        ''' <param name="precursorIon"></param>
        ''' <param name="precursor"></param>
        ''' <returns></returns>
        Public Iterator Function SearchAny(aglycones As IEnumerable(Of NamedValue(Of Double)), precursorIon As Double, precursor As PrecursorInfo) As IEnumerable(Of NamedValue(Of NeutralLoss))
            Dim PrecursorIonMZ As Double = precursor.adduct
            Dim PrecursorIonN As Double = precursor.M
            Dim M_w As Double = (precursorIon - PrecursorIonMZ) / PrecursorIonN
            Dim checkLoss As New Value(Of NamedValue(Of NeutralLoss))
            Dim combination As BruteForceCombination = createAlgorithm()
            Dim allAglycones As NamedValue(Of Double)() = aglycones.ToArray
            Dim totalMass As Dictionary(Of String, Double) = allAglycones _
                .ToDictionary(Function(a) a.Name,
                              Function(a)
                                  Return NeutralLoss.TargetMass(a.Value, M_w)
                              End Function)

            ' invali exact mass that calculated from the precursor ion
            If M_w <= 0 OrElse M_w > 2000 Then
                Return
            End If

            ' 暴力枚举的方法来搜索代谢物信息
            For Hex_n As Integer = NumHexMin To NumHexMax

                If Not combination.CheckUpBound(M_w, Hex_n, 0, 0, 0, 0, 0, 0, 0, 0) Then
                    Exit For
                End If

                For HexA_n As Integer = NumHexAMin To NumHexAMax

                    If Not combination.CheckUpBound(M_w, Hex_n, HexA_n, 0, 0, 0, 0, 0, 0, 0) Then
                        Exit For
                    End If

                    For dHex_n As Integer = NumdHexMin To NumdHexMax

                        If Not combination.CheckUpBound(M_w, Hex_n, HexA_n, dHex_n, 0, 0, 0, 0, 0, 0) Then
                            Exit For
                        End If

                        For Pen_n As Integer = NumPenMin To NumPenMax

                            If Not combination.CheckUpBound(M_w, Hex_n, HexA_n, dHex_n, Pen_n, 0, 0, 0, 0, 0) Then
                                Exit For
                            End If

                            For Mal_n As Integer = NumMalMin To NumMalMax

                                If Not combination.CheckUpBound(M_w, Hex_n, HexA_n, dHex_n, Pen_n, Mal_n, 0, 0, 0, 0) Then
                                    Exit For
                                End If

                                For Cou_n As Integer = NumCouMin To NumCouMax

                                    If Not combination.CheckUpBound(M_w, Hex_n, HexA_n, dHex_n, Pen_n, Mal_n, Cou_n, 0, 0, 0) Then
                                        Exit For
                                    End If

                                    For Fer_n As Integer = NumFerMin To NumFerMax

                                        If Not combination.CheckUpBound(M_w, Hex_n, HexA_n, dHex_n, Pen_n, Mal_n, Cou_n, Fer_n, 0, 0) Then
                                            Exit For
                                        End If

                                        For Sin_n As Integer = NumSinMin To NumSinMax

                                            If Not combination.CheckUpBound(M_w, Hex_n, HexA_n, dHex_n, Pen_n, Mal_n, Cou_n, Fer_n, Sin_n, 0) Then
                                                Exit For
                                            End If

                                            For DDMP_n As Integer = NumDDMPMin To NumDDMPMax

                                                If Not combination.CheckUpBound(M_w, Hex_n, HexA_n, dHex_n, Pen_n, Mal_n, Cou_n, Fer_n, Sin_n, DDMP_n) Then
                                                    Exit For
                                                End If

                                                For Each check As NamedValue(Of NeutralLoss) In combination.BruteForceIterations(
                                                    Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%,
                                                                                                                              _
                                                    M_w:=M_w,
                                                    iteration:=Function(loss)
                                                                   For Each type As NamedValue(Of Double) In allAglycones
                                                                       If TargetMassRestrictionCheck(loss, totalMass(type.Name)) Then
                                                                           loss = New NeutralLoss With {
                                                                               .Cou = loss.Cou,
                                                                               .DDMP = loss.DDMP,
                                                                               .dHex = loss.dHex,
                                                                               .Fer = loss.Fer,
                                                                               .Hex = loss.Hex,
                                                                               .HexA = loss.HexA,
                                                                               .Mal = loss.Mal,
                                                                               .Pen = loss.Pen,
                                                                               .Sin = loss.Sin,
                                                                               .externals = NeutralGroupHit.CopyVector(loss.externals)
                                                                           }

                                                                           Return New NamedValue(Of NeutralLoss)(type.Name, loss)
                                                                       End If
                                                                   Next

                                                                   Return Nothing
                                                               End Function)

                                                    If Not (checkLoss = check).IsEmpty Then
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
        ''' <param name="totalMass">
        ''' <see cref="NeutralLoss.TargetMass(Double, Double)"/>
        ''' </param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function TargetMassRestrictionCheck(neutralLoss As NeutralLoss, totalMass As Double) As Boolean
            ' 20210414
            ' 0.1 da is good
            ' too small will loss too much true result
            Return std.Abs(totalMass - (neutralLoss.nH2O_w - neutralLoss.Attn_w)) <= 0.1
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
                If Bal > 0 AndAlso settings.AglyconeExactMassInRange(Bal) Then
                    Return True
                End If
            End If

            Return False
        End Function
    End Class
End Namespace
