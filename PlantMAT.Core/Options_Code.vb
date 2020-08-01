Module Options_Code


    ' Attribute VB_Name = "Options_Code"
    Sub Button_Options()

        Call PublicVS_Code.Settings_Check()
        Call PublicVS_Code.Settings_Reading()

        With Settings_Dialog
            .cb_InternalAglyconeDatabase.Value = InternalAglyconeDatabase
            .tb_ExternalAglyconeDatabase.Value = ExternalAglyconeDatabase
            .db_AglyconeType.Value = AglyconeType
            .db_AglyconeSource.Value = AglyconeSource
            .tb_AglyconeMWLL.Value = AglyconeMWLL
            .tb_AglyconeMWUL.Value = AglyconeMWUL
            .tb_NumSugarMin.Value = AddedSugarAcid(0, 2)
            .tb_NumSugarMax.Value = AddedSugarAcid(0, 3)
            .tb_NumAcidMin.Value = AddedSugarAcid(1, 2)
            .tb_NumAcidMax.Value = AddedSugarAcid(1, 3)
            For j = 2 To i - 1
                With .lb_AddedSugarAcid
                    .AddItem
                    .List(j - 2, 0) = AddedSugarAcid(j, 0)
                    .List(j - 2, 1) = AddedSugarAcid(j, 1)
                    .List(j - 2, 2) = AddedSugarAcid(j, 2)
                    .List(j - 2, 3) = AddedSugarAcid(j, 3)
                End With
            Next j
            .db_PrecursorIon.Value = PrecursorIonType
            .tb_Searchppm.Value = SearchPPM
            .tb_NoiseFilter.Value = NoiseFilter
            .tb_Mzppm.Value = mzPPM
            .cb_PatternPrediction.Value = PatternPrediction
        End With

        Settings_Dialog.Show

    End Sub
End Module
