Public Class Query

    Public Property PeakNO As String
    Public Property PrecursorIon As Double

    Public Shared Function ParseMs1PeakList(file As IEnumerable(Of String)) As Query()
        Return file _
            .Select(Function(line) line.StringSplit("\s+")) _
            .Select(Function(tokens)
                        Return New Query With {
                            .PeakNO = tokens(Scan0),
                            .PrecursorIon = Val(tokens(1))
                        }
                    End Function) _
            .ToArray
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