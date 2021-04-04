#Region "Microsoft.VisualBasic::ea4e9f219648a787e5e9bf272ac8b85b, PlantMAT.Core\Algorithm\IonPrediction.vb"

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

'     Class IonPrediction
' 
'         Constructor: (+1 Overloads) Sub New
'         Sub: getResult, IonPrediction, LossCombination
' 
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.Linq

Namespace Algorithm

    ''' <summary>
    ''' Ms2 ion fragment prediction for natural products.
    ''' </summary>
    Public Class NeutralLossIonPrediction : Implements IDisposable

        Public Hex_max%, HexA_max%, dHex_max%, Pen_max%, Mal_max%, Cou_max%, Fer_max%, Sin_max%, DDMP_max%

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

        ReadOnly pIonList As New List(Of MzAnnotation)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="AglyN">the metabolite common name</param>
        ''' <param name="Agly_w">the exact mass</param>
        ''' <param name="IonMZ_crc">precursor type components, value should be ``"-H]-"`` or ``"+H]+"``</param>
        Sub New(AglyN$, Agly_w#, IonMZ_crc As MzAnnotation)
            Me.IonMZ_crc = IonMZ_crc.productMz
            Me.Rsyb = IonMZ_crc.annotation
            Me.Agly_w = Agly_w
            Me.AglyN = AglyN
        End Sub

        Public Sub getResult(ByRef result As MzAnnotation())
            result = pIonList.ToArray
        End Sub

        ''' <summary>
        ''' 根据中性丢失的数量组合来生成预测的m/z值以及对应的注释
        ''' </summary>
        Sub IonPrediction()

            ' Calcualte the total number of glycosyl and acyl groups allowed in the brute iteration
            Dim Total_max = Hex_max + HexA_max + dHex_max + Pen_max + Mal_max + Cou_max + Fer_max + Sin_max + DDMP_max

            ' Calculate the the mass of precursor ion
            Dim MIonMZ = Agly_w + Hex_max * Hex_w + HexA_max * HexA_w + dHex_max * dHex_w + Pen_max * Pen_w +
                 Mal_max * Mal_w + Cou_max * Cou_w + Fer_max * Fer_w + Sin_max * Sin_w + DDMP_max * DDMP_w -
                 Total_max * H2O_w + IonMZ_crc

            ' 0 -> 0 for循环会执行一次

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

                                                        Call LossCombination(Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%, H2O_n%, CO2_n%, MIonMZ)

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
        ''' <param name="Hex_n%"></param>
        ''' <param name="HexA_n%"></param>
        ''' <param name="dHex_n%"></param>
        ''' <param name="Pen_n%"></param>
        ''' <param name="Mal_n%"></param>
        ''' <param name="Cou_n%"></param>
        ''' <param name="Fer_n%"></param>
        ''' <param name="Sin_n%"></param>
        ''' <param name="DDMP_n%"></param>
        ''' <param name="H2O_n%"></param>
        ''' <param name="CO2_n%"></param>
        ''' <param name="MIonMZ#"></param>
        ''' <remarks>
        ''' 根据数量的组合预测计算出不同的二级碎片m/z，以及添加上对应的中性丢失注释
        ''' </remarks>
        Sub LossCombination(Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%, H2O_n%, CO2_n%, MIonMZ#)

            ' Calculate the total number of glycosyl and acyl groups in the predicted neutral loss
            ' n * H2O
            Dim Total_n = Hex_n + HexA_n + dHex_n + Pen_n + Mal_n + Cou_n + Fer_n + Sin_n + DDMP_n

            ' Calculate the mass of the predicte neutral loss
            Dim Loss_w = Hex_n * Hex_w + HexA_n * HexA_w + dHex_n * dHex_w + Pen_n * Pen_w +
                 Mal_n * Mal_w + Cou_n * Cou_w + Fer_n * Fer_w + Sin_n * Sin_w + DDMP_n * DDMP_w -
                 Total_n * H2O_w + H2O_n * H2O_w + CO2_n * CO2_w

            ' Calculate the precuror ion mz based on the calcualted loss mass
            Dim pIonMZ As Double = MIonMZ - Loss_w
            Dim pIonNM As String

            ' Find if the ion is related to the H2O/CO2 loss from aglycone
            If Hex_n = Hex_max AndAlso HexA_n = HexA_max AndAlso dHex_n = dHex_max AndAlso Pen_n = Pen_max AndAlso
                Mal_n = Mal_max AndAlso Cou_n = Cou_max AndAlso Fer_n = Fer_max AndAlso Sin_n = Sin_max AndAlso DDMP_n = DDMP_max Then

                pIonNM = $"[Agly{H2OLoss}{CO2Loss}{Rsyb}"

                If $"{H2OLoss}{CO2Loss}" = "" OrElse ($"{H2OLoss}{CO2Loss}" = "-H2O-CO2" AndAlso (AglyN = "Medicagenic acid" OrElse AglyN = "Zanhic acid")) Then
                    pIonNM = "*" & pIonNM
                End If
            Else
                pIonNM = {"[M",
                    HexLoss, HexALoss, dHexLoss, PenLoss,
                    MalLoss, CouLoss, FerLoss, SinLoss, DDMPLoss,
                    H2OLoss, CO2Loss, Rsyb
                }.JoinBy("")
            End If

            ' Save the predicted ion mz to data array pIonList()
            Call New MzAnnotation With {
                .productMz = pIonMZ,
                .annotation = pIonNM
            }.DoCall(AddressOf pIonList.Add)
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
