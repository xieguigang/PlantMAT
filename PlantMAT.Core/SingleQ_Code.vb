Module SingleQ_Code


    ' Attribute VB_Name = "SingleQ_Code"
    Sub Button_SingleQ()

        Call PublicVS_Code.Settings_Reading()

        If PatternPrediction = False Then
            With SingleQ_Dialog.cb_MS2Prediction
                .Value = False
                .Enabled = False
            End With
        End If

        SingleQ_Dialog.Show

    End Sub

End Module
