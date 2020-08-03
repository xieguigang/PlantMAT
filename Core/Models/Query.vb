Public Class Query

    Public Property PeakNO As String
    Public Property PrecursorIon As Double

    Public Shared Function ParseMs1PeakList(file As IEnumerable(Of String)) As Query

    End Function

End Class

Public Class CandidateResult

    Public Property SubstructureAgly As String
    Public Property Hex As String
    Public Property HexA As String
    Public Property dHex As String
    Public Property Pen As String
    Public Property Mal As String
    Public Property Err As Double

End Class