Namespace Report

    Public Class Table

        Public Property peakNO As Integer
        Public Property accession As String
        Public Property mz As Double
        Public Property rt As Double
        Public Property topMs2 As Double()
        Public Property stats As String
        Public Property candidate As String
        Public Property exact_mass As Double
        Public Property precursor_type As String
        Public Property [structure] As String
        Public Property err As Double
        Public Property cou As Integer
        Public Property DDMP As Integer
        Public Property fer As Integer
        Public Property hex As Integer
        Public Property hexA As Integer
        Public Property mal As Integer
        Public Property pen As Integer
        Public Property sin As Integer
        Public Property dhex As Integer

        Public Property ion1 As String
        Public Property ion2 As String
        Public Property ion3 As String
        Public Property ion4 As String
        Public Property ion5 As String

        Public Property glycosyl1 As String
        Public Property glycosyl2 As String
        Public Property glycosyl3 As String
        Public Property glycosyl4 As String
        Public Property glycosyl5 As String

        Public Overrides Function ToString() As String
            Return $"Dim {accession} As {candidate}.{precursor_type}"
        End Function
    End Class
End Namespace