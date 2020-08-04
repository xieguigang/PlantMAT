﻿Public Class CandidateResult

    ''' <summary>
    ''' 0
    ''' </summary>
    ''' <returns></returns>
    Public Property ExactMass As Double
    ''' <summary>
    ''' 1
    ''' </summary>
    ''' <returns></returns>
    Public Property SubstructureAgly As String
    ''' <summary>
    ''' 2
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String
    ''' <summary>
    ''' 3
    ''' </summary>
    ''' <returns></returns>
    Public Property Hex As Double
    ''' <summary>
    ''' 4
    ''' </summary>
    ''' <returns></returns>
    Public Property HexA As Double
    ''' <summary>
    ''' 5
    ''' </summary>
    ''' <returns></returns>
    Public Property dHex As Double
    ''' <summary>
    ''' 6
    ''' </summary>
    ''' <returns></returns>
    Public Property Pen As Double
    ''' <summary>
    ''' 7
    ''' </summary>
    ''' <returns></returns>
    Public Property Mal As Double
    ''' <summary>
    ''' 8
    ''' </summary>
    ''' <returns></returns>
    Public Property Cou As Double
    ''' <summary>
    ''' 9
    ''' </summary>
    ''' <returns></returns>
    Public Property Fer As Double
    ''' <summary>
    ''' 10
    ''' </summary>
    ''' <returns></returns>
    Public Property Sin As Double
    ''' <summary>
    ''' 11
    ''' </summary>
    ''' <returns></returns>
    Public Property DDMP As Double
    ''' <summary>
    ''' 12
    ''' </summary>
    ''' <returns></returns>
    Public Property Err As Double
    ''' <summary>
    ''' 13
    ''' </summary>
    ''' <returns></returns>
    Public Property RT As Double
    ''' <summary>
    ''' 14
    ''' </summary>
    ''' <returns></returns>
    Public Property RTErr As Double

    Public Property SMILES As New List(Of SMILES)

    Public Property Ms2Anno As Ms2IonAnnotations
    Public Property Glycosyl As New List(Of Glycosyl)

    Public Function GetSug_nStatic() As Double()
        ' 3 - 11
        Return {Hex, HexA, dHex, Pen, Mal, Cou, Fer, Sin, DDMP}
    End Function

End Class

Public Class Ms2IonAnnotations

    Public Property title As String
    Public Property annotations As String()
    Public Property comment As String
    Public Property aglycone As Boolean

End Class

Public Class Glycosyl

    Public Property title As String
    Public Property pResult As String
    Public Property Match_m As Integer
    Public Property Pred_n As Integer
    Public Property list As String()

    Public Overrides Function ToString() As String
        Return CStr(Match_m) & "/" & CStr(Pred_n) & " candidates"
    End Function

End Class

Public Class SMILES

    ''' <summary>
    ''' 2
    ''' </summary>
    ''' <returns></returns>
    Public Property peakNo As Integer
    ''' <summary>
    ''' 3
    ''' </summary>
    ''' <returns></returns>
    Public Property Sequence As String
    ''' <summary>
    ''' 4
    ''' </summary>
    ''' <returns></returns>
    Public Property GlycN As String
    Public Property GlycS As String

End Class