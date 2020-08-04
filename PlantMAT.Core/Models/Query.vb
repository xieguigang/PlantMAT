Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Class Query

    ''' <summary>
    ''' 一般为保留时间取整数
    ''' </summary>
    ''' <returns></returns>
    Public Property PeakNO As Integer
    Public Property PrecursorIon As Double
    Public Property Candidates As New List(Of CandidateResult)
    Public Property Ms2Peaks As Ms2Peaks

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

Public Class Ms2Peaks

    Public Property mz As Double()
    Public Property into As Double()

    Public ReadOnly Property TotalIonInt As Double
        Get
            Return into.Sum
        End Get
    End Property

    Public Shared Function ParseMs2(file As IEnumerable(Of String)) As Ms2Peaks
        Dim raw As Double()() = file _
            .Select(Function(line)
                        Return line _
                            .StringSplit("\s+") _
                            .Select(AddressOf Val) _
                            .ToArray
                    End Function) _
            .ToArray
        Dim mz = raw.Select(Function(a) a(Scan0)).ToArray
        Dim into = raw.Select(Function(a) a(1)).ToArray

        Return New Ms2Peaks With {
            .mz = mz,
            .into = into
        }
    End Function

End Class
