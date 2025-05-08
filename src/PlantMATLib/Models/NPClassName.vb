Imports Microsoft.VisualBasic.Serialization.JSON

Public Class NPClassName

    Public Property name As String
    Public Property note As String
    Public Property subclass As Dictionary(Of String, NPClassName)

    Public Overrides Function ToString() As String
        Return $"[{name}] {note}"
    End Function

    Public Shared Function ParseJSON(str As String) As Dictionary(Of String, NPClassName)
        Return str.LoadJSON(Of Dictionary(Of String, NPClassName))
    End Function

End Class
