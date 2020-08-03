Public Enum db_AglyconeType
    All
    Polyphenol
    Triterpene
    Steroid
    Lipid
End Enum

Public Enum db_AglyconeSource
    All
    Medicago
    Arabidopsis
    Asparagus
    Glycine
    Glycyrrhiza
    Solanum
End Enum

Public Class db_SugarAcid
    Public Property NameSA As String
    Public Property TypeSA As String
End Class

Public Class db_PrecursorIon
    Public Property IonType As String
    Public Property Adducts As Double
    Public Property M As Integer
End Class

Public Class lb_AddedSugarAcid
    Public Property NameSA As String
    Public Property TypeSA As String
    Public Property NumSAMin As String
    Public Property NumSAMax As String

    Default Public Property ListAccessor(i As Integer) As String
        Get
            Select Case i
                Case 0 : Return NameSA
                Case 1 : Return TypeSA
                Case 2 : Return NumSAMin
                Case 3 : Return NumSAMax
                Case Else
                    Throw New InvalidOperationException
            End Select
        End Get
        Set
            Select Case i
                Case 0 : NameSA = Value
                Case 1 : TypeSA = Value
                Case 2 : NumSAMin = Value
                Case 3 : NumSAMax = Value
                Case Else
                    Throw New InvalidOperationException
            End Select
        End Set
    End Property
End Class