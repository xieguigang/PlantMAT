Module PublicVS_Code



    ' Attribute VB_Name = "PublicVS_Code"
    Public Processing_Message As String
    Public Macro_to_Process As String
    Public ErrorCheck As Boolean
    Public Database As List(Of Library), Query As Worksheet, SMILES As Worksheet
    Public ListFile As Variant
    Public fs As Object, BatchFile As Object
    Public MS2FilePath As String
    Public i As Long, i_prev As Long, j As Long, k As Long, m As Long, CmpdTag As Integer
    Public Candidate As String(,), Candidate_n As Long, Pattern_n As Long
    Public IonType As String, IonMZ_crc As Double, Rsyb As String, GlycN As String, SugComb As String
    Public DHIonMZ As Double, MIonMZ As Double, DaughterIonMZ As Double, DaughterIonInt As Double, TotalIonInt As Double
    Public eIonList(,) As Double, eIon_n As Long, pIonList() As Variant, pIon_n As Long, aIonList() As Variant, aIon_n As Long
    Public AglyN As String, AglyT As String, AglyO As String, AglyW As Double, AglyS As String
    Public Agly() As Variant, Agly_w As Double, Agly_n As Long, aAgly() As Variant, aAgly_n As Long
    Public M_w As Double, H_w As Double, e_w As Double, RT_E As Double, RT_P As Variant
    Public Hex_w As Double, HexA_w As Double, dHex_w As Double, Pen_w As Double
    Public Mal_w As Double, Cou_w As Double, Fer_w As Double, Sin_w As Double, DDMP_w As Double
    Public H2O_w As Double, nH2O_w As Double, CO2_w As Double, Attn_w As Double, Bal As Double
    Public Hex_n As Long, HexA_n As Long, dHex_n As Long, Pen_n As Long, CO2_n As Long, H2O_n As Long
    Public Mal_n As Long, Cou_n As Long, Fer_n As Long, Sin_n As Long, DDMP_n As Long
    Public Hex_max As Long, HexA_max As Long, dHex_max As Long, Pen_max As Long
    Public Mal_max As Long, Cou_max As Long, Fer_max As Long, Sin_max As Long, DDMP_max As Long
    Public HexLoss As String, HexALoss As String, dHexLoss As String, PenLoss As String, CO2Loss As String, H2OLoss As String
    Public MalLoss As String, CouLoss As String, FerLoss As String, SinLoss As String, DDMPLoss As String
    Public AglyMass As Double, AglyCheck As Boolean, SingleQ As Boolean, FileCheck As Boolean
    Public RawScore As Double, RS() As String, Match_n As Long, Match_m As Long
    Public comb As Object, SMILESfile As Object, SMILESfolderName As String, SMILESfolderPath As String
    Public Subs() As Variant, Subs_n As Long, pNL() As Double, pNL_n As Long, aNL() As String, aNL_n As Long

    Public InternalAglyconeDatabase As Boolean, ExternalAglyconeDatabase As String, SubstructureDatabase As String
    Public AglyconeType As String, AglyconeSource As String, AglyconeMWLL As Double, AglyconeMWUL As Double
    Public AddedSugarAcid() As Variant
    Public NumSugarMin As Integer, NumSugarMax As Integer, NumAcidMin As Integer, NumAcidMax As Integer
    Public NumHexMin As Integer, NumHexMax As Integer, NumHexAMin As Integer, NumHexAMax As Integer
    Public NumdHexMin As Integer, NumdHexMax As Integer, NumPenMin As Integer, NumPenMax As Integer
    Public NumMalMin As Integer, NumMalMax As Integer, NumCouMin As Integer, NumCouMax As Integer
    Public NumFerMin As Integer, NumFerMax As Integer, NumSinMin As Integer, NumSinMax As Integer
    Public NumDDMPMin As Integer, NumDDMPMax As Integer
    Public PrecursorIonType As String, PrecursorIonMZ As Double, PrecursorIonN As Integer
    Public SearchPPM As Double, NoiseFilter As Double, mzPPM As Double
    Public PatternPrediction As Boolean

    Public Property lb_AddedSugarAcid As List(Of lb_AddedSugarAcid)

    Public Property db_PrecursorIonListIndex As Integer

    Public Property db_SugarAcid As db_SugarAcid()
    Public Property db_AglyconeType As String()
    Public Property db_AglyconeSource As String()
    Public Property db_PrecursorIon As db_PrecursorIon()

    Sub Settings_Check()

        '        On Error GoTo ErrorHandler

        Dim PlantMATfolder = "C:\Users\" & Environ$("Username") & "\Documents\PlantMAT"


        Using Settingsfile = (PlantMATfolder & "\Settings.txt").OpenWriter
            With Settingsfile
                .WriteLine("Internal Aglycone Database: True")
                .WriteLine("External Aglycone Database: [Select external database]")
                .WriteLine("Aglycone Type: Triterpene")
                .WriteLine("Aglycone Source: Medicago")
                .WriteLine("Aglycone MW Range: 400 600")
                .WriteLine("Num of Sugar (All): 0 6")
                .WriteLine("Num of Acid (All): 0 1")
                .WriteLine("Num of Sugar (Hex): 0 6")
                .WriteLine("Num of Sugar (HexA): 0 6")
                .WriteLine("Num of Sugar (dHex): 0 6")
                .WriteLine("Num of Sugar (Pen): 0 6")
                .WriteLine("Num of Acid (Mal): 0 1")
                .WriteLine("Precursor Ion Type: [M-H]-")
                .WriteLine("Precursor Ion MZ: -1.007277")
                .WriteLine("Precursor Ion N: 1")
                .WriteLine("Search PPM: 10")
                .WriteLine("Noise Filter: 0.05")
                .WriteLine("m/z PPM: 15")
                .WriteLine("Pattern Prediction: False")
            End With
        End Using

    End Sub

    Sub Settings_Reading()

        Hex_w = 180.06338828
        HexA_w = 194.04265285
        dHex_w = 164.06847364
        Pen_w = 150.05282357
        Mal_w = 104.01095871
        Cou_w = 164.04734422
        Fer_w = 194.05790893
        Sin_w = 224.06847364
        DDMP_w = 144.04225873
        CO2_w = 43.98982928
        H2O_w = 18.01056471
        H_w = 1.00782504
        e_w = 0.00054858

        NumSugarMin = 0
        NumSugarMax = 0
        NumHexMin = 0
        NumHexMax = 0
        NumHexAMin = 0
        NumHexAMax = 0
        NumdHexMin = 0
        NumdHexMax = 0
        NumPenMin = 0
        NumPenMax = 0
        NumMalMin = 0
        NumMalMax = 0
        NumCouMin = 0
        NumCouMax = 0
        NumFerMin = 0
        NumFerMax = 0
        NumSinMin = 0
        NumSinMax = 0
        NumDDMPMin = 0
        NumDDMPMax = 0

        Dim Settingsfile = "C:\Users\" & Environ$("Username") & "\Documents\PlantMAT\Settings.txt"

        i = 0
        ReDim AddedSugarAcid(0 To 10, 0 To 3)

        For Each textLine As String In Settingsfile.IterateAllLines
            Dim posColon = InStr(textLine, ":")
            Dim lenTextline = Len(textLine)
            Dim txtTitle = Left(textLine, posColon - 1)
            Dim txtValue = Right(textLine, lenTextline - posColon - 1)
            Dim posSpace = InStr(txtValue, " ")
            Dim lenValue, minValue, maxValue As Integer
            Dim typeSA, nameSA As String

            If posSpace <> 0 Then
                lenValue = Len(txtValue)
                minValue = Left(txtValue, posSpace - 1)
                maxValue = Right(txtValue, lenValue - posSpace)
            End If
            If txtTitle = "Internal Aglycone Database" Then InternalAglyconeDatabase = txtValue
            If txtTitle = "External Aglycone Database" Then ExternalAglyconeDatabase = txtValue
            If txtTitle = "Aglycone Type" Then AglyconeType = txtValue
            If txtTitle = "Aglycone Source" Then AglyconeSource = txtValue
            If txtTitle = "Aglycone MW Range" Then AglyconeMWLL = minValue : AglyconeMWUL = maxValue
            If Left(txtTitle, 3) = "Num" Then
                If Left(txtTitle, 12) = "Num of Sugar" Then
                    TypeSA = "Sugar"
                    NameSA = Mid(txtTitle, 15, Len(txtTitle) - 15)
                Else
                    TypeSA = "Acid"
                    NameSA = Mid(txtTitle, 14, Len(txtTitle) - 14)
                End If
                AddedSugarAcid(i, 0) = NameSA
                AddedSugarAcid(i, 1) = TypeSA
                AddedSugarAcid(i, 2) = minValue
                AddedSugarAcid(i, 3) = maxValue
                If NameSA = "All" And TypeSA = "Sugar" Then NumSugarMin = minValue : NumSugarMax = maxValue
                If NameSA = "All" And TypeSA = "Acid" Then NumAcidMin = minValue : NumAcidMax = maxValue
                If NameSA = "Hex" Then NumHexMin = minValue : NumHexMax = maxValue
                If NameSA = "HexA" Then NumHexAMin = minValue : NumHexAMax = maxValue
                If NameSA = "dHex" Then NumdHexMin = minValue : NumdHexMax = maxValue
                If NameSA = "Pen" Then NumPenMin = minValue : NumPenMax = maxValue
                If NameSA = "Mal" Then NumMalMin = minValue : NumMalMax = maxValue
                If NameSA = "Cou" Then NumCouMin = minValue : NumCouMax = maxValue
                If NameSA = "Fer" Then NumFerMin = minValue : NumFerMax = maxValue
                If NameSA = "Sin" Then NumSinMin = minValue : NumSinMax = maxValue
                If NameSA = "DDMP" Then NumDDMPMin = minValue : NumDDMPMax = maxValue
                i = i + 1
            End If
            If txtTitle = "Precursor Ion Type" Then PrecursorIonType = txtValue
            If txtTitle = "Precursor Ion MZ" Then PrecursorIonMZ = txtValue
            If txtTitle = "Precursor Ion N" Then PrecursorIonN = txtValue
            If txtTitle = "Search PPM" Then SearchPPM = txtValue
            If txtTitle = "Noise Filter" Then NoiseFilter = txtValue
            If txtTitle = "m/z PPM" Then mzPPM = txtValue
            If txtTitle = "Pattern Prediction" Then PatternPrediction = txtValue
        Next
    End Sub

    Sub StartProcessing(msg As String, code As Action)

        Processing_Message = msg
        Macro_to_Process = code.Method.Name

        Call code()
    End Sub

    Sub Export(ExportRange As Worksheet, FirstRow As Long, LastRow As Long, FirstCol As Long, LastCol As Long)
        Dim ExportFile As String = Application.GetSaveAsFilename(fileFilter:="csv Files (*.csv), *.csv")
        If ExportFile = "False" Then Exit Sub

        Using save = ExportFile.OpenWriter
            For r = FirstRow To LastRow
                For c = FirstCol To LastCol
                    Dim Data As String = ExportRange.Cells(r, c).Value
                    If IsNumeric(Data) = True Then Data = CStr(Data)
                    If Data = "" Then Data = ""
                    If c <> LastCol Then
                        save.Write(Data)
                    Else
                        save.Write(Data)
                    End If
                Next c
            Next r
        End Using

        Console.WriteLine("Data were exported to " & ExportFile)

    End Sub

End Module
