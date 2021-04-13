Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace Algorithm

    Public Class IonMZ_crc

#Region "Find the ion type (pos or neg) based on the setting"

        ' 在二级离子推断注释这里，离子化模式似乎是固定类型的

        ''' <summary>
        ''' [M+H]+/[M-H]-
        ''' </summary>
        Shared ReadOnly IonMZ_crc As New Dictionary(Of String, MzAnnotation) From {
            {"+", New MzAnnotation With {.annotation = "+H]+", .productMz = H_w - e_w}},
            {"-", New MzAnnotation With {.annotation = "-H]-", .productMz = e_w - H_w}}
        }
        ''' <summary>
        ''' [M]+/[M]-
        ''' </summary>
        Shared ReadOnly IonMZ_crc2 As New Dictionary(Of String, MzAnnotation) From {
            {"+", New MzAnnotation With {.annotation = "]+", .productMz = -e_w}},
            {"-", New MzAnnotation With {.annotation = "]-", .productMz = e_w}}
        }

#End Region

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="precursorType">
        ''' [M]+/[M]-/+/-
        ''' </param>
        ''' <returns></returns>
        Public Shared Function GetIonMZ_crc(precursorType As String) As MzAnnotation
            If precursorType = "[M]+" OrElse precursorType = "[M]-" Then
                Return IonMZ_crc2(precursorType.Last)
            Else
                Return IonMZ_crc(precursorType.Last)
            End If
        End Function

    End Class
End Namespace