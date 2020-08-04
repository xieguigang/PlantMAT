Module PublicVSCode

    Public db_SugarAcid As db_SugarAcid()
    Public db_PrecursorIon As db_PrecursorIon()

    Public Const Hex_w = 180.06338828,
        HexA_w = 194.04265285,
        dHex_w = 164.06847364,
        Pen_w = 150.05282357,
        Mal_w = 104.01095871,
        Cou_w = 164.04734422,
        Fer_w = 194.05790893,
        Sin_w = 224.06847364,
        DDMP_w = 144.04225873,
        CO2_w = 43.98982928,
        H2O_w = 18.01056471,
        H_w = 1.00782504,
        e_w = 0.00054858

    Sub New()

        Dim SugarAcidList(0 To 8, 0 To 2) As String
        SugarAcidList(0, 0) = "Hex"
        SugarAcidList(1, 0) = "HexA"
        SugarAcidList(2, 0) = "dHex"
        SugarAcidList(3, 0) = "Pen"
        SugarAcidList(4, 0) = "Mal"
        SugarAcidList(5, 0) = "Cou"
        SugarAcidList(6, 0) = "Fer"
        SugarAcidList(7, 0) = "Sin"
        SugarAcidList(8, 0) = "DDMP"

        SugarAcidList(0, 1) = "Sugar"
        SugarAcidList(1, 1) = "Sugar"
        SugarAcidList(2, 1) = "Sugar"
        SugarAcidList(3, 1) = "Sugar"
        SugarAcidList(4, 1) = "Acid"
        SugarAcidList(5, 1) = "Acid"
        SugarAcidList(6, 1) = "Acid"
        SugarAcidList(7, 1) = "Acid"
        SugarAcidList(8, 1) = "Acid"

        PublicVSCode.db_SugarAcid = SugarAcidList _
            .RowIterator _
            .Select(Function(row)
                        Return New db_SugarAcid With {.NameSA = row(0), .TypeSA = row(1)}
                    End Function) _
            .ToArray

        Dim IonTypeList(0 To 8, 0 To 2) As String
        IonTypeList(0, 0) = "[M-H]-"
        IonTypeList(1, 0) = "[M+Na-2H]-"
        IonTypeList(2, 0) = "[M+FA-H]-"
        IonTypeList(3, 0) = "[M+Hac-H]-"
        IonTypeList(4, 0) = "[2M-H]-"
        IonTypeList(5, 0) = "[2M+FA-H]-"
        IonTypeList(6, 0) = "[2M+Hac-H]-"
        IonTypeList(7, 0) = "[M+H]+"
        IonTypeList(8, 0) = "[M+Na]+"
        IonTypeList(0, 1) = "-1.007277"
        IonTypeList(1, 1) = "20.974666"
        IonTypeList(2, 1) = "44.998202"
        IonTypeList(3, 1) = "59.013852"
        IonTypeList(4, 1) = "-1.007277"
        IonTypeList(5, 1) = "44.998202"
        IonTypeList(6, 1) = "59.013852"
        IonTypeList(7, 1) = "1.007277"
        IonTypeList(8, 1) = "22.989220"
        IonTypeList(0, 2) = "1"
        IonTypeList(1, 2) = "1"
        IonTypeList(2, 2) = "1"
        IonTypeList(3, 2) = "1"
        IonTypeList(4, 2) = "2"
        IonTypeList(5, 2) = "2"
        IonTypeList(6, 2) = "2"
        IonTypeList(7, 2) = "1"
        IonTypeList(8, 2) = "1"

        PublicVSCode.db_PrecursorIon = IonTypeList _
            .RowIterator _
            .Select(Function(row)
                        Return New db_PrecursorIon With {
                            .IonType = row(0),
                            .Adducts = Val(row(1)),
                            .M = Integer.Parse(row(2))
                        }
                    End Function) _
            .ToArray
    End Sub
End Module
