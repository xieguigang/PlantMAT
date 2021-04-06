
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Algorithm

    Public Class NeutralLoss

        Public Hex%, HexA%, dHex%, Pen%, Mal%, Cou%, Fer%, Sin%, DDMP%

        Public ReadOnly Property Sugar_n As Integer
            Get
                Return Hex + HexA + dHex + Pen
            End Get
        End Property

        Public ReadOnly Property Acid_n As Integer
            Get
                Return Mal + Cou + Fer + Sin + DDMP
            End Get
        End Property

        Public ReadOnly Property Attn_w As Double
            Get
                Return Hex * Hex_w + HexA * HexA_w + dHex * dHex_w + Pen * Pen_w + Mal * Mal_w + Cou * Cou_w + Fer * Fer_w + Sin * Sin_w + DDMP * DDMP_w
            End Get
        End Property

        Public ReadOnly Property nH2O_w As Double
            Get
                Return (Sugar_n + Acid_n) * H2O_w
            End Get
        End Property

        Friend Function SetLoess(Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%) As NeutralLoss
            Hex = Hex_n
            HexA = HexA_n
            dHex = dHex_n
            Pen = Pen_n
            Mal = Mal_n
            Cou = Cou_n
            Fer = Fer_n
            Sin = Sin_n
            DDMP = DDMP_n

            Return Me
        End Function

        Public Function AglyconeExactMass(exactMass As Double) As Double
            Return exactMass + nH2O_w - Attn_w
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace