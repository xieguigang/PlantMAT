Namespace Models.AnnotationResult

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
End Namespace