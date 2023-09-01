Imports System.Runtime.CompilerServices
Imports SMRUCC.Rsharp.RDataSet.Convertor
Imports SMRUCC.Rsharp.RDataSet.Struct
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports PlantMATlib = PlantMAT.Core.Models.Library
Imports REnv = SMRUCC.Rsharp.Runtime

Module ParseRData

    <Extension>
    Public Iterator Function PopulateReference(ref As list) As IEnumerable(Of PlantMATlib)
        For Each compound As list In ref.data.Select(Function(obj) DirectCast(obj, list))
            Dim meta As list = compound("metainfo")
            Dim id As String = REnv.single(meta.getByName("BioDeepID"))
            Dim name As String = REnv.single(meta.getByName("name"))
            Dim SMILES As String = REnv.single(meta.getByName("SMILES"))
            Dim formula As String = REnv.single(meta.getByName("formula"))
            Dim exactmass As Double = CDbl(REnv.single(meta.getByName("exact_mass")))
            Dim type As String = REnv.single(meta.getByName("class"))

            Yield New PlantMATlib With {
                .CommonName = name,
                .Xref = id,
                .Universal_SMILES = SMILES,
                .ExactMass = exactmass,
                .Formula = formula,
                .[Date] = Now,
                .Editor = "BioDeep",
                .Type = type,
                .[Class] = .Type,
                .Genus = .Type
            }
        Next
    End Function
End Module
