﻿#Region "Microsoft.VisualBasic::a6c048662710cf1b7d3572d479dbe427, PlantMAT.Core\Report\Table.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
'       Feng Qiu (fengqiu1982 https://sourceforge.net/u/fengqiu1982/)
' 
' Copyright (c) 2020 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' Apache 2.0 License
' 
' 
' Copyright 2020 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
' 
'     http://www.apache.org/licenses/LICENSE-2.0
' 
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.



' /********************************************************************************/

' Summaries:

'     Class Table
' 
'         Properties: [structure], accession, candidate, cou, DDMP
'                     dhex, err, exact_mass, fer, glycosyl1
'                     glycosyl2, glycosyl3, glycosyl4, glycosyl5, hex
'                     hexA, ion1, ion2, ion3, ion4
'                     ion5, mal, mz, peakNO, pen
'                     precursor_type, rt, sin, stats, topMs2
' 
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports PlantMAT.Core.Models
Imports PlantMAT.Core.Models.AnnotationResult

Namespace Report

    Public Class Table

        Public Property peakNO As Integer
        Public Property accession As String
        Public Property mz As Double
        Public Property rt As Double
        Public Property topMs2 As Double()
        Public Property stats As String
        Public Property candidate As String
        Public Property exact_mass As Double
        Public Property precursor_type As String
        Public Property [structure] As String
        Public Property err As Double
        Public Property cou As Integer
        Public Property DDMP As Integer
        Public Property fer As Integer
        Public Property hex As Integer
        Public Property hexA As Integer
        Public Property mal As Integer
        Public Property pen As Integer
        Public Property sin As Integer
        Public Property dhex As Integer

        Public Property ion1 As String
        Public Property ion2 As String
        Public Property ion3 As String
        Public Property ion4 As String
        Public Property ion5 As String

        Public Property glycosyl1 As String
        Public Property glycosyl2 As String
        Public Property glycosyl3 As String
        Public Property glycosyl4 As String
        Public Property glycosyl5 As String

        Public Overrides Function ToString() As String
            Return $"Dim {accession} As {candidate}.{precursor_type}"
        End Function

        Friend Shared Iterator Function PopulateRows(query As Query) As IEnumerable(Of Table)
            If query.Candidates.IsNullOrEmpty Then
                Yield New Table With {
                    .stats = "no hits...",
                    .accession = query.Accession,
                    .candidate = "NA",
                    .mz = query.PrecursorIon,
                    .peakNO = query.PeakNO,
                    .rt = query.RT,
                    .topMs2 = query.Ms2Peaks.GetTopMs2(3)
                }
            Else
                For Each candidate As CandidateResult In query.Candidates
                    Dim ions = candidate.Ms2Anno.ions _
                        .OrderByDescending(Function(a) a.ionAbu) _
                        .Take(5) _
                        .ToArray
                    Dim glycosyl = If(candidate.Glycosyl Is Nothing, {}, candidate.Glycosyl.pResult) _
                        .Where(Function(a) a.best) _
                        .ToArray

                    Yield New Table With {
                        .accession = query.Accession,
                        .candidate = candidate.Name,
                        .cou = candidate.Cou,
                        .DDMP = candidate.DDMP,
                        .dhex = candidate.dHex,
                        .err = candidate.Err,
                        .exact_mass = candidate.ExactMass,
                        .fer = candidate.Fer,
                        .hex = candidate.Hex,
                        .hexA = candidate.HexA,
                        .mal = candidate.Mal,
                        .mz = query.PrecursorIon,
                        .peakNO = query.PeakNO,
                        .pen = candidate.Pen,
                        .precursor_type = candidate.precursor_type,
                        .rt = query.RT,
                        .sin = candidate.Sin,
                        .[structure] = candidate.SubstructureAgly,
                        .stats = $"{query.Candidates.Count} candidates",
                        .topMs2 = query.Ms2Peaks.GetTopMs2(3),
                        .ion1 = ions.ElementAtOrDefault(Scan0).DoCall(AddressOf annotatedIon),
                        .ion2 = ions.ElementAtOrDefault(1).DoCall(AddressOf annotatedIon),
                        .ion3 = ions.ElementAtOrDefault(2).DoCall(AddressOf annotatedIon),
                        .ion4 = ions.ElementAtOrDefault(3).DoCall(AddressOf annotatedIon),
                        .ion5 = ions.ElementAtOrDefault(4).DoCall(AddressOf annotatedIon),
                        .glycosyl1 = glycosyl.ElementAtOrDefault(0)?.struct,
                        .glycosyl2 = glycosyl.ElementAtOrDefault(1)?.struct,
                        .glycosyl3 = glycosyl.ElementAtOrDefault(2)?.struct,
                        .glycosyl4 = glycosyl.ElementAtOrDefault(3)?.struct,
                        .glycosyl5 = glycosyl.ElementAtOrDefault(4)?.struct
                    }
                Next
            End If
        End Function

        Private Shared Function annotatedIon(ion As IonAnnotation) As String
            Return $"{ion.productMz} {ion.annotation}"
        End Function
    End Class
End Namespace
