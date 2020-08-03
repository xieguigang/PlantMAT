Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Public Class Library

    <Column("Common Name")>
    Public Property CommonName As String
    Public Property [Class] As String
    Public Property Type As String
    Public Property Formula As String

    <Column("Exact Mass")>
    Public Property ExactMass As Double
    Public Property Genus As String

    <Column("Universal SMILES")>
    Public Property Universal_SMILES As String
    Public Property Editor As String
    Public Property [Date] As Date

    Public Overrides Function ToString() As String
        Return CommonName
    End Function

End Class
