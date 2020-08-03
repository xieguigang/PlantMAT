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
