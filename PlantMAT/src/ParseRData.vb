Imports SMRUCC.Rsharp.RData
Imports PlantMATlib = PlantMAT.Core.Models.Library

Module ParseRData

    Public Function LoadLibrary(path As String, symbol As String) As PlantMATlib()
        Dim rdata As RData = RData.ParseFile(path)

    End Function
End Module
