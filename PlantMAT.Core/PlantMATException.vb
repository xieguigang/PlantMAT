Public Class PlantMATException : Inherits InvalidOperationException

    Sub New(message As String)
        Call MyBase.New(message)
    End Sub
End Class
