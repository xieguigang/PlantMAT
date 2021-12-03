Imports SMRUCC.Rsharp.RData
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports PlantMATlib = PlantMAT.Core.Models.Library

Module ParseRData

    Public Function LoadLibrary(path As String, symbol As String) As PlantMATlib()
        Dim rdata As RData = RData.ParseFile(path)
        Dim refR As list = ConvertToR.ToRObject(rdata.object)
    End Function
End Module
