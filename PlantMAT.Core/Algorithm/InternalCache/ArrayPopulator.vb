Imports PlantMAT.Core.Models

Namespace Algorithm.InternalCache

    Public Class ArrayPopulator : Inherits QueryPopulator

        Public Property array As Query()

        Public Overrides Function ToString() As String
            Return $"memory_cache: {array.Length} queries"
        End Function

        Public Overrides Function GetQueries() As IEnumerable(Of Query)
            Return array
        End Function
    End Class
End Namespace