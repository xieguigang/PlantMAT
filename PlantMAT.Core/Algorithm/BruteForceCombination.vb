Imports PlantMAT.Core.Models
Imports Microsoft.VisualBasic.Linq

Namespace Algorithm

    Public Delegate Function Iteration(Of T)(neutralLoss As NeutralLoss) As T
    Public Delegate Sub Finalize(last As NeutralGroupHit)

    Public Class BruteForceCombination : Inherits PlantMATAlgorithm

        ReadOnly _defines As NeutralGroup()
        ReadOnly _finalize As Finalize

        Public Sub New(defines As NeutralGroup(), settings As Settings, Optional finalize As Finalize = Nothing)
            MyBase.New(settings)

            Me._defines = defines
            Me._finalize = finalize
        End Sub

        Protected Friend Overrides Sub applySettings()

        End Sub

        Public Iterator Function BruteForceIterations(Of T)(Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%, iteration As Iteration(Of T)) As IEnumerable(Of T)
            If _defines.IsNullOrEmpty Then
                Return
            Else
                Dim loess As New NeutralLoss With {
                    .Cou = Cou_n,
                    .DDMP = DDMP_n,
                    .dHex = dHex_n,
                    .Fer = Fer_n,
                    .Hex = Hex_n,
                    .HexA = HexA_n,
                    .Mal = Mal_n,
                    .Pen = Pen_n,
                    .Sin = Sin_n,
                    .externals = {}
                }

                For Each item As T In BruteForceIterations(defines:=_defines, loess:=loess, iteration:=iteration)
                    Yield item
                Next
            End If
        End Function

        Private Iterator Function BruteForceIterations(Of T)(defines As NeutralGroup(), loess As NeutralLoss, iteration As Iteration(Of T)) As IEnumerable(Of T)
            Dim external As NeutralGroup = defines(Scan0)
            Dim pop As NeutralGroup() = defines.Skip(1).ToArray
            Dim it As NeutralGroupHit = NeutralGroupHit.FromDefine(external)

            loess = New NeutralLoss With {
                .Cou = loess.Cou,
                .DDMP = loess.DDMP,
                .dHex = loess.dHex,
                .Fer = loess.Fer,
                .Hex = loess.Hex,
                .HexA = loess.HexA,
                .Mal = loess.Mal,
                .Pen = loess.Pen,
                .Sin = loess.Sin,
                .externals = NeutralGroupHit.CopyVector(loess.externals).JoinIterates(it).ToArray
            }

            If pop.Length = 0 Then
                For i As Integer = external.min To external.max
                    it.nHit = i

                    Yield iteration(loess)
                Next
            Else
                For i As Integer = external.min To external.max
                    it.nHit = i

                    For Each item In BruteForceIterations(pop, loess, iteration)
                        Yield item
                    Next
                Next
            End If

            If Not _finalize Is Nothing Then
                Call _finalize(loess.externals.Last)
            End If
        End Function
    End Class
End Namespace