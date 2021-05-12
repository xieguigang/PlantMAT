Namespace Models

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
End Namespace