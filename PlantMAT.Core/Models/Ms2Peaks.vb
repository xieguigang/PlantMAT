Namespace Models

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
End Namespace