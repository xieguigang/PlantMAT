Public Class Query

    Public Property PeakNO As String
    Public Property PrecursorIon As Double
    Public Property Candidates As New List(Of CandidateResult)

    Default Public ReadOnly Property Candidate(i As Integer) As CandidateResult
        Get
            Return _Candidates(i)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"[{PeakNO}] {PrecursorIon} {If(Candidates.Count = 0, "no hits", Candidates.Take(6).Select(Function(c) c.Name).JoinBy(", ")) & "..."}"
    End Function

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

    ''' <summary>
    ''' 0
    ''' </summary>
    ''' <returns></returns>
    Public Property ExactMass As Double
    ''' <summary>
    ''' 1
    ''' </summary>
    ''' <returns></returns>
    Public Property SubstructureAgly As String
    ''' <summary>
    ''' 2
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String
    ''' <summary>
    ''' 3
    ''' </summary>
    ''' <returns></returns>
    Public Property Hex As Double
    ''' <summary>
    ''' 4
    ''' </summary>
    ''' <returns></returns>
    Public Property HexA As Double
    ''' <summary>
    ''' 5
    ''' </summary>
    ''' <returns></returns>
    Public Property dHex As Double
    ''' <summary>
    ''' 6
    ''' </summary>
    ''' <returns></returns>
    Public Property Pen As Double
    ''' <summary>
    ''' 7
    ''' </summary>
    ''' <returns></returns>
    Public Property Mal As Double
    ''' <summary>
    ''' 8
    ''' </summary>
    ''' <returns></returns>
    Public Property Cou As Double
    ''' <summary>
    ''' 9
    ''' </summary>
    ''' <returns></returns>
    Public Property Fer As Double
    ''' <summary>
    ''' 10
    ''' </summary>
    ''' <returns></returns>
    Public Property Sin As Double
    ''' <summary>
    ''' 11
    ''' </summary>
    ''' <returns></returns>
    Public Property DDMP As Double
    ''' <summary>
    ''' 12
    ''' </summary>
    ''' <returns></returns>
    Public Property Err As Double
    ''' <summary>
    ''' 13
    ''' </summary>
    ''' <returns></returns>
    Public Property RT As Double
    ''' <summary>
    ''' 14
    ''' </summary>
    ''' <returns></returns>
    Public Property RTErr As Double

    Public Property SMILES As New List(Of String())

    Public Function GetSug_nStatic() As Double()
        ' 3 - 11
        Return {Hex, HexA, dHex, Pen, Mal, Cou, Fer, Sin, DDMP}
    End Function

End Class