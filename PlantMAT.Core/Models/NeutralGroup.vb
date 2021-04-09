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

        Public Shared Function CopyVector(vec As IEnumerable(Of NeutralGroupHit)) As NeutralGroupHit()
            Return vec.Select(Function(n) n.Clone).ToArray
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
    End Class

End Namespace