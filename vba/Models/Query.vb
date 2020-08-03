
''' <summary>
''' Query peak object
''' </summary>
Public Class Query

    Public Property id As String
    Public Property precursorMz As Double

    Public Property no_hit As Boolean
    Public Property candidates As New List(Of CandiateSearch)
    Public Property comment As String

End Class

Public Class CandiateSearch

    Public Property substructure_agly As String
    Public Property err As Double

End Class