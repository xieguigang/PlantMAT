#Region "Microsoft.VisualBasic::f8401a83199e13aa50bf83272d81b73b, PlantMAT.Core\Algorithm\NeutralLossIonPrediction.vb"

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

'     Class NeutralLossIonPrediction
' 
'         Constructor: (+1 Overloads) Sub New
'         Sub: (+2 Overloads) Dispose, getResult, IonPrediction, LossCombination
' 
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Linq
Imports PlantMAT.Core.Models

Namespace Algorithm

    ''' <summary>
    ''' Ms2 ion fragment prediction for natural products.
    ''' </summary>
    Public Class NeutralLossIonPrediction
        Implements IDisposable

        ''' <summary>
        ''' predicted result
        ''' </summary>
        Dim Hex_max%, HexA_max%, dHex_max%, Pen_max%, Mal_max%, Cou_max%, Fer_max%, Sin_max%, DDMP_max%
        Dim NumSugarMax%, NumAcidMax%

        ' Initilize all neutral losses and predicted ions pIonList() to none
        Dim HexLoss As New StringBuilder
        Dim HexALoss As New StringBuilder
        Dim dHexLoss As New StringBuilder
        Dim PenLoss As New StringBuilder
        Dim MalLoss As New StringBuilder
        Dim CouLoss As New StringBuilder
        Dim FerLoss As New StringBuilder
        Dim SinLoss As New StringBuilder
        Dim DDMPLoss As New StringBuilder
        Dim H2OLoss As New StringBuilder
        Dim CO2Loss As New StringBuilder

        Dim Rsyb$
        Dim IonMZ_crc#
        Dim Agly_w#
        Dim AglyN$

        Private disposedValue As Boolean

        ReadOnly pIonList As New Dictionary(Of String, MzAnnotation)
        ReadOnly externals As NeutralGroup()
        ReadOnly externalLoss As Dictionary(Of String, StringBuilder)
        ReadOnly maxnExternals As Dictionary(Of String, Integer)
        ReadOnly precursorMz As Double
        ReadOnly M_w As Double

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="AglyN">the metabolite common name</param>
        ''' <param name="Agly_w">the exact mass</param>
        ''' <param name="IonMZ_crc">precursor type components, value should be ``"-H]-"`` or ``"+H]+"``</param>
        Sub New(precursorMz As Double, AglyN$, Agly_w#, IonMZ_crc As MzAnnotation, externals As NeutralGroup(), precursor As PrecursorInfo)
            Me.IonMZ_crc = IonMZ_crc.productMz
            Me.Rsyb = IonMZ_crc.annotation
            Me.Agly_w = Agly_w
            Me.AglyN = AglyN
            Me.externals = externals.Where(Function(ng) ng.max > 0).ToArray
            Me.externalLoss = externals.ToDictionary(Function(a) a.aglycone, Function(any) New StringBuilder)
            Me.maxnExternals = externals.ToDictionary(Function(a) a.aglycone, Function(a) a.max)
            Me.precursorMz = precursorMz
            Me.M_w = (precursorMz - precursor.adduct) / precursor.M
        End Sub

        ''' <summary>
        ''' set max number from the MS1TopDown analysis
        ''' </summary>
        ''' <param name="Hex_max%"></param>
        ''' <param name="HexA_max%"></param>
        ''' <param name="dHex_max%"></param>
        ''' <param name="Pen_max%"></param>
        ''' <param name="Mal_max%"></param>
        ''' <param name="Cou_max%"></param>
        ''' <param name="Fer_max%"></param>
        ''' <param name="Sin_max%"></param>
        ''' <param name="DDMP_max%"></param>
        ''' <returns></returns>
        Public Function SetPredictedMax(Hex_max%, HexA_max%, dHex_max%, Pen_max%, Mal_max%, Cou_max%, Fer_max%, Sin_max%, DDMP_max%) As NeutralLossIonPrediction
            Me.Hex_max = Hex_max%
            Me.HexA_max = HexA_max%
            Me.dHex_max = dHex_max%
            Me.Pen_max = Pen_max%
            Me.Mal_max = Mal_max%
            Me.Cou_max = Cou_max%
            Me.Fer_max = Fer_max%
            Me.Sin_max = Sin_max%
            Me.DDMP_max = DDMP_max%

            With NeutralLoss.nMax(Hex_max%, HexA_max%, dHex_max%, Pen_max%, Mal_max%, Cou_max%, Fer_max%, Sin_max%, DDMP_max%, externals)
                NumAcidMax = .acidMax
                NumSugarMax = .sugarMax
            End With

            Return Me
        End Function

        Public Sub getResult(ByRef result As MzAnnotation())
            result = pIonList.Values.ToArray
        End Sub

        ''' <summary>
        ''' 根据中性丢失的数量组合来生成预测的m/z值以及对应的注释
        ''' </summary>
        Public Sub IonPrediction()

            ' Calcualte the total number of glycosyl and acyl groups allowed in the brute iteration
            Dim Total_max As Integer = Hex_max + HexA_max + dHex_max + Pen_max + Mal_max + Cou_max + Fer_max + Sin_max + DDMP_max + (Aggregate item In externals Into Sum(item.max))
            Dim TotalExternalMass As Double = Aggregate item As NeutralGroup
                                              In externals
                                              Let exactMass As Double = FormulaScanner.ScanFormula(item.formula).ExactMass
                                              Into Sum(item.max * exactMass)

            ' Calculate the the mass of precursor ion
            Dim MIonMZ As Double = Agly_w + Hex_max * Hex_w + HexA_max * HexA_w + dHex_max * dHex_w + Pen_max * Pen_w +
                 Mal_max * Mal_w + Cou_max * Cou_w + Fer_max * Fer_w + Sin_max * Sin_w + DDMP_max * DDMP_w -
                 Total_max * H2O_w + IonMZ_crc +
                 TotalExternalMass

            Dim combination As New BruteForceCombination(externals, NumSugarMax, NumAcidMax, Double.MinValue, Sub(last As NeutralGroupHit) Call externalLoss(last.aglycone).Clear())

            ' 0 -> 0 for循环会执行一次

            pIonList($"[M]{Rsyb.Last}") = New MzAnnotation With {
                .productMz = MIonMZ,
                .annotation = $"[M]{Rsyb.Last}"
            }

            If Mal_max > 0 Then
                pIonList("[Mal]+") = New MzAnnotation With {.productMz = Mal_w, .annotation = "[Mal]+"}
            End If
            If Cou_max > 0 Then
                pIonList("[Cou]+") = New MzAnnotation With {.productMz = Cou_w, .annotation = "[Cou]+"}
            End If
            If Fer_max > 0 Then
                pIonList("[Fer]+") = New MzAnnotation With {.productMz = Fer_w, .annotation = "[Fer]+"}
            End If
            If Sin_max > 0 Then
                pIonList("[Sin]+") = New MzAnnotation With {.productMz = Sin_w, .annotation = "[Sin]+"}
            End If
            If DDMP_max > 0 Then
                pIonList("[DDMP]+") = New MzAnnotation With {.productMz = DDMP_w, .annotation = "[DDMP]+"}
            End If

            For Each acid In externals.Where(Function(x) x.type = NeutralTypes.acid)
                If acid.max > 0 Then
                    pIonList($"[{acid.aglycone}]+") = New MzAnnotation With {
                        .productMz = FormulaScanner.EvaluateExactMass(acid.formula),
                        .annotation = $"[{acid.aglycone}]+"
                    }
                End If
            Next

            ' Do brute force iteration to generate all hypothetical neutral losses
            ' as a combination of different glycosyl and acyl groups, and
            ' for each predicted neutral loss, calcualte the ion mz
            For Hex_n = 0 To Hex_max
                For HexA_n = 0 To HexA_max
                    For dHex_n = 0 To dHex_max
                        For Pen_n = 0 To Pen_max
                            For Mal_n = 0 To Mal_max
                                For Cou_n = 0 To Cou_max
                                    For Fer_n = 0 To Fer_max
                                        For Sin_n = 0 To Sin_max
                                            For DDMP_n = 0 To DDMP_max
                                                For H2O_n = 0 To 1
                                                    For CO2_n = 0 To 1

                                                        Dim nH2O = H2O_n
                                                        Dim nCO2 = CO2_n

                                                        For Each check In combination.BruteForceIterations(
                                                            Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%, _
 _
                                                            M_w:=M_w,
                                                            iteration:=Function(neutralLoss)
                                                                           Call LossCombination(neutralLoss, nH2O, nCO2, MIonMZ)

                                                                           If neutralLoss.externals.Length > 0 Then
                                                                               Dim lastAglycone As NeutralGroupHit = neutralLoss _
                                                                                  .externals _
                                                                                  .Last

                                                                               If lastAglycone.nHit > 0 Then
                                                                                   Call externalLoss(lastAglycone.aglycone) _
                                                                                       .Append("-") _
                                                                                       .Append(lastAglycone.aglycone)
                                                                               End If
                                                                           End If

                                                                           Return Nothing
                                                                       End Function)

                                                            ' do nothing
                                                        Next

                                                        CO2Loss.Append("-CO2")
                                                    Next CO2_n
                                                    CO2Loss.Clear()
                                                    H2OLoss.Append("-H2O")
                                                Next H2O_n
                                                H2OLoss.Clear()
                                                DDMPLoss.Append("-DDMP")
                                            Next DDMP_n
                                            DDMPLoss.Clear()
                                            SinLoss.Append("-Sin")
                                        Next Sin_n
                                        SinLoss.Clear()
                                        FerLoss.Append("-Fer")
                                    Next Fer_n
                                    FerLoss.Clear()
                                    CouLoss.Append("-Cou")
                                Next Cou_n
                                CouLoss.Clear()
                                MalLoss.Append("-Mal")
                            Next Mal_n
                            MalLoss.Clear()
                            PenLoss.Append("-Pen")
                        Next Pen_n
                        PenLoss.Clear()
                        dHexLoss.Append("-dHex")
                    Next dHex_n
                    dHexLoss.Clear()
                    HexALoss.Append("-HexA")
                Next HexA_n
                HexALoss.Clear()
                HexLoss.Append("-Hex")
            Next Hex_n
        End Sub

        ''' <summary>
        ''' productMz = exactMass - neutral_loss
        ''' </summary>
        ''' <param name="H2O_n%"></param>
        ''' <param name="CO2_n%"></param>
        ''' <param name="MIonMZ#"></param>
        ''' <remarks>
        ''' 根据数量的组合预测计算出不同的二级碎片m/z，以及添加上对应的中性丢失注释
        ''' </remarks>
        Private Sub LossCombination(neutralLoess As NeutralLoss, H2O_n%, CO2_n%, MIonMZ#)
            ' Calculate the total number of glycosyl and acyl groups in the predicted neutral loss
            ' n * H2O
            Dim Total_n = neutralLoess.Acid_n + neutralLoess.Sugar_n
            ' Calculate the mass of the predicte neutral loss
            Dim Loss_w = neutralLoess.Attn_w - Total_n * H2O_w + H2O_n * H2O_w + CO2_n * CO2_w

            ' Calculate the precuror ion mz based on the calcualted loss mass
            Dim pIonMZ As Double = MIonMZ - Loss_w
            Dim pIonNM As String

            If pIonMZ > precursorMz Then
                Return
            End If

            ' Find if the ion is related to the H2O/CO2 loss from aglycone
            If neutralLoess.Hex = Hex_max AndAlso
                neutralLoess.HexA = HexA_max AndAlso
                neutralLoess.dHex = dHex_max AndAlso
                neutralLoess.Pen = Pen_max AndAlso
                neutralLoess.Mal = Mal_max AndAlso
                neutralLoess.Cou = Cou_max AndAlso
                neutralLoess.Fer = Fer_max AndAlso
                neutralLoess.Sin = Sin_max AndAlso
                neutralLoess.DDMP = DDMP_max AndAlso
                ((Not neutralLoess.externals.IsNullOrEmpty) AndAlso neutralLoess.externals.All(Function(a) maxnExternals(a.aglycone) = a.nHit)) Then

                Dim part As String = $"{H2OLoss}{CO2Loss}"

                pIonNM = $"[Agly{part}{Rsyb}"

                If part = "" OrElse (part = "-H2O-CO2" AndAlso (AglyN = "Medicagenic acid" OrElse AglyN = "Zanhic acid")) Then
                    pIonNM = "*" & pIonNM
                End If
            Else
                pIonNM = {"[M",
                    HexLoss, HexALoss, dHexLoss, PenLoss,
                    MalLoss, CouLoss, FerLoss, SinLoss, DDMPLoss
                }.JoinIterates(externalLoss.Values) _
                 .JoinIterates({H2OLoss, CO2Loss, Rsyb}) _
                 .JoinBy("")
            End If

            ' Save the predicted ion mz to data array pIonList()
            pIonList(pIonNM) = New MzAnnotation With {
                .productMz = pIonMZ,
                .annotation = pIonNM
            }
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    Call pIonList.Clear()

                    Call HexLoss.Clear()
                    Call HexALoss.Clear()
                    Call dHexLoss.Clear()
                    Call PenLoss.Clear()
                    Call MalLoss.Clear()
                    Call CouLoss.Clear()
                    Call FerLoss.Clear()
                    Call SinLoss.Clear()
                    Call DDMPLoss.Clear()
                    Call H2OLoss.Clear()
                    Call CO2Loss.Clear()

                    For Each item In externalLoss
                        item.Value.Clear()
                    Next

                    externalLoss.Clear()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ' TODO: set large fields to null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
        ' Protected Overrides Sub Finalize()
        '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace
