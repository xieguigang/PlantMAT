Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Linq

Namespace Models

    Public Structure NeutralGroup

        Public Property name As String
        Public Property formula As String
        Public Property aglycone As String
        Public Property is_acid As Boolean
        Public Property min As Integer
        Public Property max As Integer

    End Structure

    Public Class NeutralGroupHit

        Public Property aglycone As String
        Public Property formula As String
        Public Property exact_mass As Double
        Public Property is_acid As Boolean
        Public Property nHit As Integer

        Public ReadOnly Property MassTotal As Double
            Get
                Return nHit * exact_mass
            End Get
        End Property

        Public Function Clone() As NeutralGroupHit
            Return New NeutralGroupHit With {
                .aglycone = aglycone,
                .exact_mass = exact_mass,
                .formula = formula,
                .is_acid = is_acid,
                .nHit = nHit
            }
        End Function

        Public Overrides Function ToString() As String
            Return aglycone
        End Function

        Public Shared Function FromDefine(define As NeutralGroup) As NeutralGroupHit
            Return New NeutralGroupHit With {
                .aglycone = define.aglycone,
                .formula = define.formula,
                .is_acid = define.is_acid,
                .nHit = 0,
                .exact_mass = FormulaScanner.ScanFormula(.formula).ExactMass
            }
        End Function

        Public Shared Iterator Function BruteForceIterations(Of T)(defines As NeutralGroup(), iteration As Func(Of NeutralGroupHit(), T)) As IEnumerable(Of T)
            If defines.IsNullOrEmpty Then
                Return
            Else
                For Each item In BruteForceIterations(defines, loops:={}, iteration:=iteration)
                    Yield item
                Next
            End If
        End Function

        Private Shared Iterator Function BruteForceIterations(Of T)(defines As NeutralGroup(), loops As NeutralGroupHit(), iteration As Func(Of NeutralGroupHit(), T)) As IEnumerable(Of T)
            Dim external As NeutralGroup = defines(Scan0)
            Dim pop As NeutralGroup() = defines.Skip(1).ToArray
            Dim it As NeutralGroupHit = NeutralGroupHit.FromDefine(external)

            loops = loops.JoinIterates(it).ToArray

            If pop.Length = 0 Then
                For i As Integer = external.min To external.max
                    it.nHit = i

                    Yield iteration(loops)
                Next
            Else
                For i As Integer = external.min To external.max
                    it.nHit = i

                    For Each item In BruteForceIterations(pop, loops, iteration)
                        Yield item
                    Next
                Next
            End If
        End Function
    End Class

End Namespace