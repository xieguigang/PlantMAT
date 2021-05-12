Imports PlantMAT.Core.Models
Imports Microsoft.VisualBasic.Language

Namespace Algorithm

    Public Class NeutralLossBuilder

        ReadOnly groups As New Dictionary(Of String, NeutralGroup)

        Sub New(groups As IEnumerable(Of NeutralGroup))
            Me.groups = groups.ToDictionary(Function(element) element.ionName)
        End Sub

        Public Function CreateLoss(lossList As Dictionary(Of String, Integer)) As NeutralLoss
            Dim hits As New List(Of NeutralGroupHit)

            For Each lossElement As KeyValuePair(Of String, Integer) In lossList
                hits += NeutralGroupHit.FromDefine(groups(lossElement.Key))
                hits.Last.nHit = lossElement.Value
            Next

            Return New NeutralLoss With {.externals = hits.ToArray}
        End Function
    End Class
End Namespace