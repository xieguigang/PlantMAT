Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace Models

    Public Enum NeutralTypes
        NA
        sugar
        acid
        methylate
    End Enum

    Friend Interface INeutralGroupHit

        Property ionName As String
        Property nHit As Integer
        Property type As NeutralTypes

    End Interface

End Namespace