Imports PlantMAT.Core.Models
Imports Microsoft.VisualBasic.Language

Namespace Algorithm

    Public Class NeutralLossBuilder

        ReadOnly groups As New Dictionary(Of String, NeutralGroup)
        ReadOnly strict As Boolean = False

        Sub New(groups As IEnumerable(Of NeutralGroup), Optional strict As Boolean = False)
            Me.groups = groups.ToDictionary(Function(element) element.ionName)
            Me.strict = strict
        End Sub

        Public Function CreateLoss(lossList As Dictionary(Of String, Integer)) As NeutralLoss
            Dim hits As New List(Of NeutralGroupHit)

            For Each lossElement As KeyValuePair(Of String, Integer) In lossList
                If Not groups.ContainsKey(lossElement.Key) Then
                    Call $"missing neutral element group: {lossElement.Key}!".Warning

                    If strict Then
                        Throw New KeyNotFoundException($"missing neutral element group: {lossElement.Key}!")
                    Else
                        Return Nothing
                    End If
                End If

                hits += NeutralGroupHit.FromDefine(groups(lossElement.Key))
                hits.Last.nHit = lossElement.Value
            Next

            Return New NeutralLoss With {.externals = hits.ToArray}
        End Function
    End Class
End Namespace