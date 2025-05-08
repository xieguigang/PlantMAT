#Region "Microsoft.VisualBasic::73b0c58768e6e77c8cf370f395b61829, PlantMAT.Core\Report\Table.vb"

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
    '         Properties: [structure], accession, Acid_n, aglycone, Attn_w
    '                     candidate, cou, DDMP, dhex, err
    '                     exact_mass, fer, glycosyl1, glycosyl2, glycosyl3
    '                     glycosyl4, glycosyl5, hex, hexA, ion1
    '                     ion2, ion3, ion4, ion5, mal
    '                     mz, nH2O_w, peakNO, pen, precursor_type
    '                     rt, sin, stats, Sugar_n, theoretical_precursor
    '                     topMs2, xref
    ' 
    '         Function: annotatedIon, FlatTableRow, glycosylSeq, PopulateRows, ToString
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
        Public Property xref As String
        Public Property candidate As String

#Region "这两个都是根据原始数据计算出来的理论值"
        Public Property exact_mass As Double
        Public Property theoretical_precursor As Double
#End Region

        Public Property precursor_type As String
        Public Property [structure] As String

#Region "在库之中的分子的基础上增加的"
        Public Property Attn_w As Double
        Public Property nH2O_w As Double
        ''' <summary>
        ''' 理论的<see cref="theoretical_precursor"/>与实际的<see cref="mz"/>之间的ppm误差值
        ''' </summary>
        ''' <returns></returns>
        Public Property err As Double

        Public Property Sugar_n As Integer
        Public Property Acid_n As Integer
        Public Property cou As Integer
        Public Property DDMP As Integer
        Public Property fer As Integer
        Public Property hex As Integer
        Public Property hexA As Integer
        Public Property mal As Integer
        Public Property pen As Integer
        Public Property sin As Integer
        Public Property dhex As Integer
#End Region

        Public Property aglycone As Boolean

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
                    .topMs2 = query.Ms2Peaks.GetTopMs2(3),
                    .xref = "NA"
                }
            Else
                For Each candidate As CandidateResult In query.Candidates
                    Yield FlatTableRow(query, candidate)
                Next
            End If
        End Function

        Private Shared Function FlatTableRow(query As Query, candidate As CandidateResult) As Table
            Dim ions = If(candidate.Ms2Anno Is Nothing, {}, candidate.Ms2Anno.ions) _
                        .OrderByDescending(Function(a) a.ionAbu) _
                        .Take(5) _
                        .ToArray
            Dim glycosyl = If(candidate.Glycosyl Is Nothing, {}, candidate.Glycosyl.pResult) _
                .OrderByDescending(Function(gly) gly.score) _
                .ToArray

            Return New Table With {
                .accession = query.Accession,
                .candidate = candidate.Name,
                .cou = candidate.Cou,
                .DDMP = candidate.DDMP,
                .dhex = candidate.dHex,
                .err = candidate.Err,
                .exact_mass = candidate.Theoretical_ExactMass,
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
                .glycosyl1 = glycosyl.ElementAtOrDefault(0).DoCall(AddressOf glycosylSeq),
                .glycosyl2 = glycosyl.ElementAtOrDefault(1).DoCall(AddressOf glycosylSeq),
                .glycosyl3 = glycosyl.ElementAtOrDefault(2).DoCall(AddressOf glycosylSeq),
                .glycosyl4 = glycosyl.ElementAtOrDefault(3).DoCall(AddressOf glycosylSeq),
                .glycosyl5 = glycosyl.ElementAtOrDefault(4).DoCall(AddressOf glycosylSeq),
                .aglycone = If(candidate.Ms2Anno Is Nothing, False, candidate.Ms2Anno.aglycone),
                .Sugar_n = candidate.Sugar_n,
                .nH2O_w = candidate.nH2O_w,
                .Attn_w = candidate.Attn_w,
                .Acid_n = candidate.Acid_n,
                .xref = candidate.xref,
                .theoretical_precursor = candidate.Theoretical_PrecursorMz
            }
        End Function

        Private Shared Function glycosylSeq(glycosyl As GlycosylPredition) As String
            If glycosyl Is Nothing Then
                Return ""
            Else
                ' fix of the [#NAME?] display bugs
                ' in excel
                Return "X" & glycosyl.struct
            End If
        End Function

        Private Shared Function annotatedIon(ion As IonAnnotation) As String
            If ion Is Nothing Then
                Return ""
            Else
                Return $"{ion.productMz} {ion.annotation}"
            End If
        End Function
    End Class
End Namespace
