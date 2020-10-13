﻿Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports PlantMAT.Core.Models

Namespace Algorithm.InternalCache

    Public MustInherit Class QueryPopulator

        Public MustOverride Function GetQueries() As IEnumerable(Of Query)

    End Class

    Public Class ArrayPopulator : Inherits QueryPopulator

        Public Property array As Query()

        Public Overrides Function ToString() As String
            Return $"memory_cache: {array.Length} queries"
        End Function

        Public Overrides Function GetQueries() As IEnumerable(Of Query)
            Return array
        End Function
    End Class

    ''' <summary>
    ''' use cache file for solve memory problem
    ''' </summary>
    Public Class CacheFilePopulator : Inherits QueryPopulator

        ReadOnly cacheFile As String

        Sub New(cache As String)
            cacheFile = cache
        End Sub

        Public Overrides Function ToString() As String
            Return $"cache: {cacheFile}"
        End Function

        Public Sub Delete()
            Call cacheFile.DeleteFile

            If Not cacheFile.FileExists Then
                Call Console.WriteLine($"cache file `{cacheFile}` cleanup!")
            End If
        End Sub

        Public Overrides Iterator Function GetQueries() As IEnumerable(Of Query)
            Using reader As New BinaryDataReader(cacheFile.Open)
                Do While Not reader.EndOfStream
                    Dim size As Long = reader.ReadInt64
                    Dim buffer As Byte() = reader.ReadBytes(size)
                    Dim json As JsonObject = BSON.Load(buffer)

                    Yield json.CreateObject(Of Query)
                Loop
            End Using
        End Function
    End Class

End Namespace