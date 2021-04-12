Imports PlantMAT.Core.Models
Imports Microsoft.VisualBasic.Linq
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType

Namespace Algorithm

    Public Delegate Function Iteration(Of T)(neutralLoss As NeutralLoss) As T
    Public Delegate Sub Finalize(last As NeutralGroupHit)

    Public Class BruteForceCombination

        ReadOnly _defines As NeutralGroup()
        ReadOnly _finalize As Finalize

        Dim NumSugarMax, NumAcidMax As Integer
        Dim MaxAglyconeExactMass As Double

        Public Sub New(defines As NeutralGroup(), NumSugarMax%, NumAcidMax%, MaxAglyconeExactMass#, Optional finalize As Finalize = Nothing)
            Me.NumAcidMax = NumAcidMax
            Me.NumSugarMax = NumSugarMax
            Me._defines = defines
            Me._finalize = finalize
            Me.MaxAglyconeExactMass = MaxAglyconeExactMass
        End Sub

        Public Iterator Function BruteForceIterations(Of T)(Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%,
                                                            iteration As Iteration(Of T),
                                                            Optional M_w# = Nothing) As IEnumerable(Of T)
            Yield New NeutralLoss With {
                .Cou = Cou_n,
                .DDMP = DDMP_n,
                .dHex = dHex_n,
                .externals = {},
                .Fer = Fer_n,
                .Hex = Hex_n,
                .HexA = HexA_n,
                .Mal = Mal_n,
                .Pen = Pen_n,
                .Sin = Sin_n
            }.DoCall(Function(x) iteration(x))

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

                For Each item As T In BruteForceIterations(defines:=_defines, loss:=loess, iteration:=iteration, M_w:=M_w)
                    Yield item
                Next
            End If
        End Function

        Private Iterator Function BruteForceIterations(Of T)(defines As NeutralGroup(), loss As NeutralLoss, M_w#, iteration As Iteration(Of T)) As IEnumerable(Of T)
            Dim external As NeutralGroup = defines(Scan0)
            Dim pop As NeutralGroup() = defines.Skip(1).ToArray
            Dim it As NeutralGroupHit = NeutralGroupHit.FromDefine(external)

            loss = New NeutralLoss With {
                .Cou = loss.Cou,
                .DDMP = loss.DDMP,
                .dHex = loss.dHex,
                .Fer = loss.Fer,
                .Hex = loss.Hex,
                .HexA = loss.HexA,
                .Mal = loss.Mal,
                .Pen = loss.Pen,
                .Sin = loss.Sin,
                .externals = NeutralGroupHit _
                    .CopyVector(loss.externals) _
                    .JoinIterates(it) _
                    .ToArray
            }

            If pop.Length = 0 Then
                For i As Integer = external.min To external.max
                    it.nHit = i

                    Yield iteration(loss)
                Next
            Else
                For i As Integer = external.min To external.max
                    it.nHit = i

                    If loss.Sugar_n > NumSugarMax OrElse loss.Acid_n > NumAcidMax Then
                        Exit For
                    ElseIf loss.AglyconeExactMass(M_w) > MaxAglyconeExactMass Then
                        Exit For
                    Else
                        Yield iteration(loss)
                    End If

                    For Each item In BruteForceIterations(pop, loss, M_w, iteration)
                        Yield item
                    Next
                Next
            End If

            If Not _finalize Is Nothing Then
                Call _finalize(loss.externals.Last)
            End If
        End Function
    End Class
End Namespace