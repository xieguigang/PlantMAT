﻿Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace Models

    Public Enum NeutralTypes
        NA
        suger
        acid
        ' methylate
    End Enum

    Public Class NeutralGroup : Implements INeutralGroupHit

        Public Property name As String
        Public Property formula As String
        Public Property ionName As String Implements INeutralGroupHit.ionName
        Public Property type As NeutralTypes Implements INeutralGroupHit.type
        Public Property min As Integer
        Public Property max As Integer Implements INeutralGroupHit.nHit

        Public Overrides Function ToString() As String
            Return ionName
        End Function

    End Class

    Friend Interface INeutralGroupHit
        Property ionName As String
        Property nHit As Integer
        Property type As NeutralTypes
    End Interface

    Public Class NeutralGroupHit : Implements INeutralGroupHit

        <MessagePackMember(0)> Public Property ionName As String Implements INeutralGroupHit.ionName
        <MessagePackMember(1)> Public Property formula As String
        <MessagePackMember(2)> Public Property exact_mass As Double
        <MessagePackMember(3)> Public Property type As NeutralTypes Implements INeutralGroupHit.type
        <MessagePackMember(4)> Public Property nHit As Integer Implements INeutralGroupHit.nHit

        Public ReadOnly Property MassTotal As Double
            Get
                Return nHit * exact_mass
            End Get
        End Property

        Public Function Clone() As NeutralGroupHit
            Return New NeutralGroupHit With {
                .ionName = ionName,
                .exact_mass = exact_mass,
                .formula = formula,
                .type = type,
                .nHit = nHit
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"{ionName} [{nHit}] {MassTotal}"
        End Function

        Public Shared Function CopyVector(vec As IEnumerable(Of NeutralGroupHit)) As NeutralGroupHit()
            Return vec.Select(Function(n) n.Clone).ToArray
        End Function

        Public Shared Function FromDefine(define As NeutralGroup) As NeutralGroupHit
            Return New NeutralGroupHit With {
                .ionName = define.ionName,
                .formula = define.formula,
                .type = define.type,
                .nHit = 0,
                .exact_mass = FormulaScanner.EvaluateExactMass(.formula)
            }
        End Function
    End Class

End Namespace