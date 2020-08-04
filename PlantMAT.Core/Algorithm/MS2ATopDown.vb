Imports Microsoft.VisualBasic.Language

''' <summary>
''' This module performs MS2 annotation
''' </summary>
Public Class MS2ATopDown

    Dim settings As Settings
    Dim mzPPM As Double
    Dim NoiseFilter As Double
    Dim PrecursorIonType$

    Sub New(settings As Settings)
        Me.settings = settings
        Me.applySettings()
    End Sub

    Private Sub applySettings()
        mzPPM = settings.mzPPM
        NoiseFilter = settings.NoiseFilter
        PrecursorIonType = settings.PrecursorIonType
    End Sub

    Public Function MS2Annotation(queries As Query()) As Query()
        Dim result As Query()

        If queries.All(Function(q) q.Candidates.IsNullOrEmpty) Then
            Console.WriteLine("Please run combinatorial enumeration (MS1) first")
            result = queries
        Else
            Console.WriteLine("Now analyzing ms2 topdown, please wait...")
            result = MS2ATopDown(queries).ToArray

            Console.WriteLine("MS2 annotation finished." & vbNewLine & "Continue glycosyl sequencing")
            Console.WriteLine("Now analyzing glycosyl sequencing, please wait...")
            result = New GlycosylSequencing(settings).MS2P(result).ToArray

            Console.WriteLine("Glycosyl sequencing finished")
            Console.WriteLine("MS2 annotation finished")
        End If

        Return result
    End Function

    Private Iterator Function MS2ATopDown(queries As IEnumerable(Of Query)) As IEnumerable(Of Query)
        For Each query As Query In queries
            'Read compound serial number and precuror ion mz
            Dim CmpdTag = query.PeakNO
            Dim DHIonMZ = query.PrecursorIon
            Dim IonMZ_crc As Double
            Dim Rsyb As String

            'Find the ion type (pos or neg) based on the setting
            If Right(PrecursorIonType, 1) = "-" Then
                IonMZ_crc = e_w - H_w
                Rsyb = "-H]-"
            Else
                IonMZ_crc = H_w - e_w
                Rsyb = "+H]+"
            End If

            'Perform the MS2 annotation and display the results
            Call MS2A_TopDown_MS2Annotation(query, IonMZ_crc, Rsyb)

            Yield query
        Next
    End Function

    ''' <summary>
    ''' Loop through all candidates for each compound
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="IonMZ_crc"></param>
    ''' <param name="Rsyb"></param>
    Private Sub MS2A_TopDown_MS2Annotation(query As Query, IonMZ_crc As Double, Rsyb As String)
        For i As Integer = 0 To query.Candidates.Count - 1
            If Not query.Ms2Peaks Is Nothing Then
                Call MS2A_TopDown_MS2AnnotationLoop(query, IonMZ_crc, Rsyb, i)
            End If
        Next
    End Sub

    Private Sub MS2A_TopDown_MS2AnnotationLoop(query As Query, IonMZ_crc As Double, Rsyb As String, i As Integer)
        Dim candidate As CandidateResult = query.Candidate(i)

        'Read the results from combinatorial enumeration
        Dim AglyN = candidate.Name
        Dim Agly_w = candidate.ExactMass
        Dim Hex_max = candidate.Hex
        Dim HexA_max = candidate.HexA
        Dim dHex_max = candidate.dHex
        Dim Pen_max = candidate.Pen
        Dim Mal_max = candidate.Mal
        Dim Cou_max = candidate.Cou
        Dim Fer_max = candidate.Fer
        Dim Sin_max = candidate.Sin
        Dim DDMP_max = candidate.DDMP

        'First, predict the ions based on the results from combinatorial enumeration
        Dim prediction As New MS2A_TopDown_MS2Annotation_IonPrediction(AglyN$, Agly_w#, IonMZ_crc, Rsyb) With {
                .Hex_max = Hex_max,
            .HexA_max = HexA_max#, .dHex_max = dHex_max#, .Pen_max = Pen_max#, .Mal_max = Mal_max#, .Cou_max = Cou_max#, .Fer_max = Fer_max#, .Sin_max = Sin_max#, .DDMP_max = DDMP_max#}
        Dim pIon_n As Integer = 0
        Dim pIonList As String(,) = Nothing

        Call prediction.MS2A_TopDown_MS2Annotation_IonPrediction()
        Call prediction.getResult(pIon_n, pIonList)

        'Second, compare the predicted ions with the measured
        Dim aIon_n As Integer = 0
        Dim aIonList(0 To 3, 0 To 1) As String
        Dim AglyCheck As Boolean = MS2A_TopDown_MS2Annotation_IonMatching(query, pIon_n, pIonList, aIon_n, aIonList)

        'Third, add a dropdown list for each candidate and show the annotation results in the list
        Dim combName = "dd_MS2A_TopDown_" & CStr(i)
        Dim comb As New List(Of String)

        'Fourth, save the annotation results in the cell
        Dim aResult As String
        aResult = ""

        If aIon_n > 0 Then
            For s = 1 To aIon_n
                Dim aIonMZ = aIonList(1, s)
                Dim aIonAbu = aIonList(2, s)
                Dim aIonNM = aIonList(3, s)
                comb.Add(CStr(Format(aIonAbu, "0.000")) & " " & aIonNM)
                aResult = aResult & CStr(Format(aIonMZ, "0.0000")) & ", " &
                    CStr(Format(aIonAbu * 100, "0.00")) & ", " & aIonNM & "; "
            Next s
        End If

        Dim combText = CStr(aIon_n) & " ions annotated"

        candidate.Ms2Anno = New Ms2IonAnnotations With {
                .title = combText,
                .annotations = comb.ToArray,
                .comment = aResult
            }

        'Fifth, show an asterisk mark if the ions corresponding to the aglycone are found
        If AglyCheck = True Then
            candidate.Ms2Anno.aglycone = True
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="pIon_n%"></param>
    ''' <param name="pIonList"></param>
    ''' <returns>AglyCheck</returns>
    Private Function MS2A_TopDown_MS2Annotation_IonMatching(query As Query, pIon_n%, pIonList As String(,), ByRef aIon_n As Integer, ByRef aIonList As String(,)) As Boolean
        'Initialize the annotated ion list aIonList() to none
        Dim AglyCheck = False
        Dim eIonList = query.Ms2Peaks
        Dim eIon_n = eIonList.mz.Length
        Dim TotalIonInt As Double = eIonList.TotalIonInt

        'Compare the measured ions eIonList() with the predicted pIonList()
        'If the mz error is less than the defined ppm and intensity is above the noise filter, then
        'save the predicted ions in the annotation ion list aIonList()
        For s As Integer = 0 To eIon_n - 1
            Dim eIonMZ = eIonList.mz(s)
            Dim eIonInt = eIonList.into(s)
            For t = 1 To pIon_n
                Dim pIonMZ = pIonList(1, t)
                Dim pIonNM = pIonList(2, t)
                If Math.Abs((eIonMZ - pIonMZ) / pIonMZ) * 1000000 <= mzPPM Then
                    Dim aIonAbu = eIonInt / TotalIonInt
                    If aIonAbu * 100 >= NoiseFilter Then
                        aIon_n = aIon_n + 1
                        ReDim Preserve aIonList(0 To 3, 0 To aIon_n)
                        aIonList(1, aIon_n) = eIonMZ
                        aIonList(2, aIon_n) = aIonAbu
                        aIonList(3, aIon_n) = pIonNM
                        If Left(pIonNM, 1) = "*" Then AglyCheck = True
                    End If
                End If
            Next t
        Next s

        Return AglyCheck
    End Function
End Class

Public Class MS2A_TopDown_MS2Annotation_IonPrediction

    Public Hex_max%, HexA_max%, dHex_max%, Pen_max%, Mal_max%, Cou_max%, Fer_max%, Sin_max%, DDMP_max%

    ' Initilize all neutral losses and predicted ions pIonList() to none
    Dim pIon_n% = 0
    Dim HexLoss$ = ""
    Dim HexALoss$ = ""
    Dim dHexLoss$ = ""
    Dim PenLoss$ = ""
    Dim MalLoss$ = ""
    Dim CouLoss$ = ""
    Dim FerLoss$ = ""
    Dim SinLoss$ = ""
    Dim DDMPLoss$ = ""
    Dim H2OLoss$ = ""
    Dim CO2Loss$ = ""

    Dim Rsyb$
    Dim IonMZ_crc#
    Dim Agly_w#
    Dim AglyN$

    Dim pIonList(0 To 2, 0 To 1) As String

    Sub New(AglyN$, Agly_w#, IonMZ_crc#, Rsyb$)
        Me.IonMZ_crc = IonMZ_crc
        Me.Rsyb = Rsyb
        Me.Agly_w = Agly_w
        Me.AglyN = AglyN
    End Sub

    Public Sub getResult(ByRef pIon_n As Integer, ByRef pIonList As String(,))
        pIon_n = Me.pIon_n
        pIonList = Me.pIonList
    End Sub

    Sub MS2A_TopDown_MS2Annotation_IonPrediction()

        'Calcualte the total number of glycosyl and acyl groups allowed in the brute iteration
        Dim Total_max = Hex_max + HexA_max + dHex_max + Pen_max + Mal_max + Cou_max + Fer_max + Sin_max + DDMP_max

        'Calculate the the mass of precursor ion
        Dim MIonMZ = Agly_w + Hex_max * Hex_w + HexA_max * HexA_w + dHex_max * dHex_w + Pen_max * Pen_w +
                 Mal_max * Mal_w + Cou_max * Cou_w + Fer_max * Fer_w + Sin_max * Sin_w + DDMP_max * DDMP_w -
                 Total_max * H2O_w + IonMZ_crc

        'Do brute force iteration to generate all hypothetical neutral losses
        'as a combination of different glycosyl and acyl groups, and
        'for each predicted neutral loss, calcualte the ion mz
        For Hex_n = 0 To Hex_max
            For HexA_n = 0 To HexA_max
                For dHex_n = 0 To dHex_max
                    For Pen_n = 0 To Pen_max
                        For Mal_n = 0 To Mal_max
                            For Cou_n = 0 To Cou_max
                                For Fer_n = 0 To Fer_max
                                    For Sin_n = 0 To Sin_max
                                        For DDMP_n = 0 To DDMP_max
                                            For H2O_n = 0 To 1
                                                For CO2_n = 0 To 1

                                                    Call MS2A_TopDown_MS2Annotation_IonPrediction_LossCombination(Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%, H2O_n%, CO2_n%, MIonMZ)

                                                    CO2Loss = CO2Loss + "-CO2"
                                                Next CO2_n
                                                CO2Loss = ""
                                                H2OLoss = H2OLoss + "-H2O"
                                            Next H2O_n
                                            H2OLoss = ""
                                            DDMPLoss = DDMPLoss + "-DDMP"
                                        Next DDMP_n
                                        DDMPLoss = ""
                                        SinLoss = SinLoss + "-Sin"
                                    Next Sin_n
                                    SinLoss = ""
                                    FerLoss = FerLoss + "-Fer"
                                Next Fer_n
                                FerLoss = ""
                                CouLoss = CouLoss + "-Cou"
                            Next Cou_n
                            CouLoss = ""
                            MalLoss = MalLoss + "-Mal"
                        Next Mal_n
                        MalLoss = ""
                        PenLoss = PenLoss + "-Pen"
                    Next Pen_n
                    PenLoss = ""
                    dHexLoss = dHexLoss + "-dHex"
                Next dHex_n
                dHexLoss = ""
                HexALoss = HexALoss + "-HexA"
            Next HexA_n
            HexALoss = ""
            HexLoss = HexLoss + "-Hex"
        Next Hex_n

    End Sub

    Sub MS2A_TopDown_MS2Annotation_IonPrediction_LossCombination(Hex_n%, HexA_n%, dHex_n%, Pen_n%, Mal_n%, Cou_n%, Fer_n%, Sin_n%, DDMP_n%, H2O_n%, CO2_n%, MIonMZ#)

        'Calculate the total number of glycosyl and acyl groups in the predicted neutral loss
        Dim Total_n = Hex_n + HexA_n + dHex_n + Pen_n + Mal_n + Cou_n + Fer_n + Sin_n + DDMP_n

        'Calculate the mass of the predicte neutral loss
        Dim Loss_w = Hex_n * Hex_w + HexA_n * HexA_w + dHex_n * dHex_w + Pen_n * Pen_w +
                 Mal_n * Mal_w + Cou_n * Cou_w + Fer_n * Fer_w + Sin_n * Sin_w + DDMP_n * DDMP_w -
                 Total_n * H2O_w + H2O_n * H2O_w + CO2_n * CO2_w

        'Calculate the precuror ion mz based on the calcualted loss mass
        Dim pIonMZ = MIonMZ - Loss_w
        Dim pIonNM As String

        'Find if the ion is related to the H2O/CO2 loss from aglycone
        If Hex_n = Hex_max And HexA_n = HexA_max And dHex_n = dHex_max And Pen_n = Pen_max And
           Mal_n = Mal_max And Cou_n = Cou_max And Fer_n = Fer_max And Sin_n = Sin_max And DDMP_n = DDMP_max Then
            pIonNM = "[Agly" & H2OLoss & CO2Loss & Rsyb
            If H2OLoss & CO2Loss = "" Or (H2OLoss & CO2Loss = "-H2O-CO2" And
               (AglyN = "Medicagenic acid" Or AglyN = "Zanhic acid")) Then
                pIonNM = "*" & pIonNM
            End If
        Else
            pIonNM = "[M" & HexLoss & HexALoss & dHexLoss & PenLoss &
                            MalLoss & CouLoss & FerLoss & SinLoss & DDMPLoss &
                            H2OLoss & CO2Loss & Rsyb
        End If

        'Save the predicted ion mz to data array pIonList()
        pIon_n = pIon_n + 1
        ReDim Preserve pIonList(0 To 2, 0 To pIon_n)
        pIonList(1, pIon_n) = pIonMZ
        pIonList(2, pIon_n) = pIonNM

    End Sub
End Class