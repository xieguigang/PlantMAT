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

Namespace Algorithm
    Public Class IonPrediction

        Public Hex_max%, HexA_max%, dHex_max%, Pen_max%, Mal_max%, Cou_max%, Fer_max%, Sin_max%, DDMP_max%

        ' Initilize all neutral losses and predicted ions pIonList() to none
        Dim pIon_n% = 0
        Dim HexLoss$ = ""
        Dim HexALoss$ = ""
        Dim dHexLoss$ = ""
        Dim PenLoss$ = ""
        Dim MalLoss$ = ""
        Dim CouLoss$ = ""
        Dim FerLoss$ = ""
        Dim SinLoss$ = ""
        Dim DDMPLoss$ = ""
        Dim H2OLoss$ = ""
        Dim CO2Loss$ = ""

        Dim Rsyb$
        Dim IonMZ_crc#
        Dim Agly_w#
        Dim AglyN$

        Dim pIonList(0 To 2, 0 To 1) As Object

        Sub New(AglyN$, Agly_w#, IonMZ_crc#, Rsyb$)
            Me.IonMZ_crc = IonMZ_crc
            Me.Rsyb = Rsyb
            Me.Agly_w = Agly_w
            Me.AglyN = AglyN
        End Sub

        Public Sub getResult(ByRef pIon_n As Integer, ByRef pIonList As Object(,))
            pIon_n = Me.pIon_n
            pIonList = Me.pIonList
        End Sub

        Sub IonPrediction()

            ' Calcualte the total number of glycosyl and acyl groups allowed in the brute iteration
            Dim Total_max = Hex_max + HexA_max + dHex_max + Pen_max + Mal_max + Cou_max + Fer_max + Sin_max + DDMP_max

            ' Calculate the the mass of precursor ion
            Dim MIonMZ = Agly_w + Hex_max * Hex_w + HexA_max * HexA_w + dHex_max * dHex_w + Pen_max * Pen_w +
                 Mal_max * Mal_w + Cou_max * Cou_w + Fer_max * Fer_w + Sin_max * Sin_w + DDMP_max * DDMP_w -
                 Total_max * H2O_w + IonMZ_crc

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

                                                        CO2Loss = CO2Loss + "-CO2"
                                                    Next CO2_n
                                                    CO2Loss = ""
                                                    H2OLoss = H2OLoss + "-H2O"
                                                Next H2O_n
                                                H2OLoss = ""
                                                DDMPLoss = DDMPLoss + "-DDMP"
                                            Next DDMP_n
                                            DDMPLoss = ""
                                            SinLoss = SinLoss + "-Sin"
                                        Next Sin_n
                                        SinLoss = ""
                                        FerLoss = FerLoss + "-Fer"
                                    Next Fer_n
                                    FerLoss = ""
                                    CouLoss = CouLoss + "-Cou"
                                Next Cou_n
                                CouLoss = ""
                                MalLoss = MalLoss + "-Mal"
                            Next Mal_n
                            MalLoss = ""
                            PenLoss = PenLoss + "-Pen"
                        Next Pen_n
                        PenLoss = ""
                        dHexLoss = dHexLoss + "-dHex"
                    Next dHex_n
                    dHexLoss = ""
                    HexALoss = HexALoss + "-HexA"
                Next HexA_n
                HexALoss = ""
                HexLoss = HexLoss + "-Hex"
            Next Hex_n

        End Sub

        Sub LossCombination(Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%, H2O_n%, CO2_n%, MIonMZ#)

            ' Calculate the total number of glycosyl and acyl groups in the predicted neutral loss
            Dim Total_n = Hex_n + HexA_n + dHex_n + Pen_n + Mal_n + Cou_n + Fer_n + Sin_n + DDMP_n

            ' Calculate the mass of the predicte neutral loss
            Dim Loss_w = Hex_n * Hex_w + HexA_n * HexA_w + dHex_n * dHex_w + Pen_n * Pen_w +
                 Mal_n * Mal_w + Cou_n * Cou_w + Fer_n * Fer_w + Sin_n * Sin_w + DDMP_n * DDMP_w -
                 Total_n * H2O_w + H2O_n * H2O_w + CO2_n * CO2_w

            ' Calculate the precuror ion mz based on the calcualted loss mass
            Dim pIonMZ = MIonMZ - Loss_w
            Dim pIonNM As String

            ' Find if the ion is related to the H2O/CO2 loss from aglycone
            If Hex_n = Hex_max And HexA_n = HexA_max And dHex_n = dHex_max And Pen_n = Pen_max And
                Mal_n = Mal_max And Cou_n = Cou_max And Fer_n = Fer_max And Sin_n = Sin_max And DDMP_n = DDMP_max Then

                pIonNM = "[Agly" & H2OLoss & CO2Loss & Rsyb

                If H2OLoss & CO2Loss = "" Or (H2OLoss & CO2Loss = "-H2O-CO2" And
                    (AglyN = "Medicagenic acid" Or AglyN = "Zanhic acid")) Then
                    pIonNM = "*" & pIonNM
                End If
            Else
                pIonNM = "[M" & HexLoss & HexALoss & dHexLoss & PenLoss &
                            MalLoss & CouLoss & FerLoss & SinLoss & DDMPLoss &
                            H2OLoss & CO2Loss & Rsyb
            End If

            ' Save the predicted ion mz to data array pIonList()
            pIon_n = pIon_n + 1
            ReDim Preserve pIonList(0 To 2, 0 To pIon_n)
            pIonList(1, pIon_n) = pIonMZ
            pIonList(2, pIon_n) = pIonNM

        End Sub
    End Class
End Namespace
