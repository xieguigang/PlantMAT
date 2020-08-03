﻿Imports System.IO
Imports System.Text

Public Class Settings

    Public Property InternalAglyconeDatabase As Boolean
    Public Property ExternalAglyconeDatabase As String
    Public Property AglyconeType As db_AglyconeType
    Public Property AglyconeSource As db_AglyconeSource
    Public Property AglyconeMWRange As Integer()
    Public Property NumofSugarAll As Integer()
    Public Property NumofAcidAll As Integer()

    Public Property NumofSugarHex As Integer()
    Public Property NumofSugarHexA As Integer()
    Public Property NumofSugardHex As Integer()
    Public Property NumofSugarPen As Integer()
    Public Property NumofAcidMal As Integer()

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
                .WriteLine("Internal Aglycone Database: " & InternalAglyconeDatabase)
                .WriteLine("External Aglycone Database: " & ExternalAglyconeDatabase)
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
            .InternalAglyconeDatabase = True,
            .ExternalAglyconeDatabase = Nothing,
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
            .PrecursorIonType = "[M-H]-",
            .PrecursorIonMZ = -1.007277,
            .PrecursorIonN = 1,
            .SearchPPM = 10,
            .NoiseFilter = 0.05,
            .mzPPM = 15,
            .PatternPrediction = False
        }
    End Function
End Class
