Imports System.Runtime.CompilerServices
Imports SMRUCC.Rsharp.RDataSet.Convertor
Imports SMRUCC.Rsharp.RDataSet.Struct
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports PlantMATlib = PlantMAT.Core.Models.Library

Module ParseRData

    Public Function LoadLibrary(path As String, symbol As String) As PlantMATlib()
        Dim rdata As RData = RData.ParseFile(path)
        Dim refR As list = ConvertToR.ToRObject(rdata.object)
        Dim symbolVal As list = refR(symbol)
        Dim data As PlantMATlib() = symbolVal.PopulateReference.ToArray

        Return data
    End Function

    <Extension>
    Private Iterator Function PopulateReference(ref As list) As IEnumerable(Of PlantMATlib)
        For Each compound As list In ref.data.Select(Function(obj) DirectCast(obj, list))
            Dim meta As list = compound("metainfo")
            Dim id As String = meta.getByName("BioDeepID")
            Dim name As String = meta.getByName("name")

        Next
    End Function
End Module
