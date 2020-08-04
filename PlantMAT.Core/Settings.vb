﻿Imports System.IO
Imports System.Text
Imports PlantMAT.Core.Models

Public Class Settings

    Public Property AglyconeType As db_AglyconeType
    Public Property AglyconeSource As db_AglyconeSource
    Public Property AglyconeMWRange As Double()
    Public Property NumofSugarAll As Integer()
    Public Property NumofAcidAll As Integer()

    Public Property NumofSugarHex As Integer()
    Public Property NumofSugarHexA As Integer()
    Public Property NumofSugardHex As Integer()
    Public Property NumofSugarPen As Integer()

    Public Property NumofAcidMal As Integer()
    Public Property NumofAcidCou As Integer()
    Public Property NumofAcidFer As Integer()
    Public Property NumofAcidSin As Integer()
    Public Property NumofAcidDDMP As Integer()

    Public Property PrecursorIonType As String
    Public Property PrecursorIonMZ As Double
    Public Property PrecursorIonN As Integer
    Public Property SearchPPM As Double
    Public Property NoiseFilter As Double
    Public Property mzPPM As Double
    Public Property PatternPrediction As Boolean

    Public Overrides Function ToString() As String
        Dim text As New StringBuilder

        Using Settingsfile As New StringWriter(text)
            With Settingsfile
                .WriteLine("Aglycone Type: " & AglyconeType)
                .WriteLine("Aglycone Source: " & AglyconeSource)
                .WriteLine("Aglycone MW Range: " & AglyconeMWRange.JoinBy(" "))
                .WriteLine("Num of Sugar (All): " & NumofSugarAll.JoinBy(" "))
                .WriteLine("Num of Acid (All): " & NumofAcidAll.JoinBy(" "))

                .WriteLine("Num of Sugar Hex: " & NumofSugarHex.JoinBy(" "))
                .WriteLine("Num of Sugar HexA: " & NumofSugarHexA.JoinBy(" "))
                .WriteLine("Num of Sugar dHex: " & NumofSugardHex.JoinBy(" "))
                .WriteLine("Num of Sugar Pen: " & NumofSugarPen.JoinBy(" "))

                .WriteLine("Num of Acid Mal: " & NumofAcidMal.JoinBy(" "))
                .WriteLine("Num of Acid Cou: " & NumofAcidCou.JoinBy(" "))
                .WriteLine("Num of Acid Fer: " & NumofAcidFer.JoinBy(" "))
                .WriteLine("Num of Acid Sin: " & NumofAcidSin.JoinBy(" "))
                .WriteLine("Num of Acid DDMP: " & NumofAcidDDMP.JoinBy(" "))

                .WriteLine("Precursor Ion Type: " & PrecursorIonType)
                .WriteLine("Precursor Ion MZ: " & PrecursorIonMZ)
                .WriteLine("Precursor Ion N: " & PrecursorIonN)
                .WriteLine("Search PPM: " & SearchPPM)
                .WriteLine("Noise Filter: " & NoiseFilter)
                .WriteLine("m/z PPM: " & mzPPM)
                .WriteLine("Pattern Prediction: " & PatternPrediction)
            End With

            Call Settingsfile.Flush()
        End Using

        Return text.ToString
    End Function

    Public Shared Function DefaultSettings() As Settings
        Return New Settings With {
            .AglyconeType = db_AglyconeType.Triterpene,
            .AglyconeSource = db_AglyconeSource.Medicago,
            .AglyconeMWRange = {400, 600},
            .NumofSugarAll = {0, 6},
            .NumofAcidAll = {0, 1},
            .NumofSugarHex = {0, 6},
            .NumofSugarHexA = {0, 6},
            .NumofSugardHex = {0, 6},
            .NumofSugarPen = {0, 6},
            .NumofAcidMal = {0, 1},
            .NumofAcidCou = {0, 1},
            .NumofAcidFer = {0, 1},
            .NumofAcidSin = {0, 1},
            .NumofAcidDDMP = {0, 1},
            .PrecursorIonType = "[M-H]-",
            .PrecursorIonMZ = -1.007277,
            .PrecursorIonN = 1,
            .SearchPPM = 10,
            .NoiseFilter = 0.05,
            .mzPPM = 15,
            .PatternPrediction = True
        }
    End Function
End Class
