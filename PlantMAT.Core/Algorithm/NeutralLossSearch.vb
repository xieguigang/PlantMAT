Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType

Namespace Algorithm

    Public Class NeutralLossSearch : Inherits PlantMATAlgorithm

#Region "Search Space"
        Dim NumHexMin, NumHexMax, NumHexAMin, NumHexAMax, NumdHexMin, NumdHexMax, NumPenMin, NumPenMax, NumMalMin, NumMalMax, NumCouMin, NumCouMax, NumFerMin, NumFerMax, NumSinMin, NumSinMax, NumDDMPMin, NumDDMPMax As Integer
        Dim NumSugarMin, NumSugarMax, NumAcidMin, NumAcidMax As Integer
#End Region

        Public Sub New(settings As Settings)
            MyBase.New(settings)
        End Sub

        Protected Friend Overrides Sub applySettings()
            Const min = 0
            Const max = 1

            NumHexMin = settings.NumofSugarHex(min) : NumHexMax = settings.NumofSugarHex(max)
            NumHexAMin = settings.NumofSugarHexA(min) : NumHexAMax = settings.NumofSugarHexA(max)
            NumdHexMin = settings.NumofSugardHex(min) : NumdHexMax = settings.NumofSugardHex(max)
            NumPenMin = settings.NumofSugarPen(min) : NumPenMax = settings.NumofSugarPen(max)
            NumMalMin = settings.NumofAcidMal(min) : NumMalMax = settings.NumofAcidMal(max)
            NumCouMin = settings.NumofAcidCou(min) : NumCouMax = settings.NumofAcidCou(max)
            NumFerMin = settings.NumofAcidFer(min) : NumFerMax = settings.NumofAcidFer(max)
            NumSinMin = settings.NumofAcidSin(min) : NumSinMax = settings.NumofAcidSin(max)
            NumDDMPMin = settings.NumofAcidDDMP(min) : NumDDMPMax = settings.NumofAcidDDMP(max)

            NumSugarMin = settings.NumofSugarAll(min) : NumSugarMax = settings.NumofSugarAll(max)
            NumAcidMin = settings.NumofAcidAll(min) : NumAcidMax = settings.NumofAcidAll(max)
        End Sub

        ''' <summary>
        ''' Do brute force iteration to generate all hypothetical neutral losses
        ''' </summary>
        ''' <param name="precursorIon">
        ''' The ms1 precursor ion its m/z value
        ''' </param>
        ''' <param name="precursor">
        ''' The precursor type information
        ''' </param>
        ''' <returns></returns>
        Public Iterator Function NeutralLosses(precursorIon As Double, precursor As PrecursorInfo) As IEnumerable(Of NeutralLoss)
            Dim PrecursorIonMZ As Double = precursor.adduct
            Dim PrecursorIonN As Double = precursor.M
            Dim M_w = (precursorIon - PrecursorIonMZ) / PrecursorIonN
            Dim neutralLoss As New NeutralLoss

            ' invali exact mass that calculated from the precursor ion
            If M_w <= 0 OrElse M_w > 2000 Then
                Return
            End If

            ' 暴力枚举的方法来搜索代谢物信息
            For Hex_n = NumHexMin To NumHexMax
                For HexA_n = NumHexAMin To NumHexAMax
                    For dHex_n = NumdHexMin To NumdHexMax
                        For Pen_n = NumPenMin To NumPenMax
                            For Mal_n = NumMalMin To NumMalMax
                                For Cou_n = NumCouMin To NumCouMax
                                    For Fer_n = NumFerMin To NumFerMax
                                        For Sin_n = NumSinMin To NumSinMax
                                            For DDMP_n = NumDDMPMin To NumDDMPMax

                                                If RestrictionCheck(
                                                    neutralLoss:=neutralLoss.SetLoess(Hex_n, HexA_n, dHex_n, Pen_n, Mal_n, Cou_n, Fer_n, Sin_n, DDMP_n),
                                                    M_w:=M_w
                                                ) Then

                                                    Yield New NeutralLoss With {
                                                        .Cou = Cou_n,
                                                        .DDMP = DDMP_n,
                                                        .dHex = dHex_n,
                                                        .Fer = Fer_n,
                                                        .Hex = Hex_n,
                                                        .HexA = HexA_n,
                                                        .Mal = Mal_n,
                                                        .Pen = Pen_n,
                                                        .Sin = Sin_n
                                                    }
                                                End If

                                            Next DDMP_n
                                        Next Sin_n
                                    Next Fer_n
                                Next Cou_n
                            Next Mal_n
                        Next Pen_n
                    Next dHex_n
                Next HexA_n
            Next Hex_n
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="neutralLoss"></param>
        ''' <param name="M_w">exact mass</param>
        ''' <returns></returns>
        Private Function RestrictionCheck(neutralLoss As NeutralLoss, M_w As Double) As Boolean
            Dim Sugar_n As Integer = neutralLoss.Sugar_n
            Dim Acid_n As Integer = neutralLoss.Acid_n

            If Sugar_n >= NumSugarMin AndAlso Sugar_n <= NumSugarMax AndAlso Acid_n >= NumAcidMin AndAlso Acid_n <= NumAcidMax Then
                Dim Attn_w As Double = neutralLoss.Attn_w
                Dim nH2O_w = (Sugar_n + Acid_n) * H2O_w
                Dim Bal = neutralLoss.AglyconeExactMass(M_w)

                ' "Aglycone MW Range" Then AglyconeMWLL = minValue : AglyconeMWUL = maxValue
                If settings.AglyconeExactMassInRange(Bal) Then
                    Return True
                End If
            End If

            Return False
        End Function
    End Class
End Namespace