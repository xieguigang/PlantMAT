#Region "Microsoft.VisualBasic::9dd0e800106748b7050de52c422cc35a, PlantMAT.Core\Models\Query.vb"

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

'     Class Query
' 
'         Properties: Accession, Candidates, Ms2Peaks, PeakNO, PrecursorIon
'                     RT
' 
'         Constructor: (+2 Overloads) Sub New
'         Function: ParseMs1PeakList, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports PlantMAT.Core.Models.AnnotationResult

Namespace Models

    <KnownType(GetType(CandidateResult))>
    Public Class Query : Implements IMs1

        ''' <summary>
        ''' 一般为保留时间取整数
        ''' </summary>
        ''' <returns></returns>
        Public Property PeakNO As Integer
        Public Property RT As Double Implements IMs1.rt
        Public Property PrecursorIon As Double Implements IMs1.mz
        Public Property Candidates As CandidateResult()
        Public Property Ms2Peaks As Ms2Peaks
        ''' <summary>
        ''' a unique guid string of current query object
        ''' </summary>
        ''' <returns></returns>
        Public Property Accession As String

        <SoapIgnore>
        <XmlIgnore>
        <ScriptIgnore>
        Default Public ReadOnly Property Candidate(i As Integer) As CandidateResult
            Get
                Return _Candidates(i)
            End Get
        End Property

        Sub New()
        End Sub

        ''' <summary>
        ''' clone query object
        ''' </summary>
        ''' <param name="clone"></param>
        Sub New(clone As Query)
            Me.Accession = clone.Accession

            If Not clone.Ms2Peaks Is Nothing Then
                Me.Ms2Peaks = New Ms2Peaks With {
                    .into = clone.Ms2Peaks.into.ToArray,
                    .mz = clone.Ms2Peaks.mz.ToArray
                }
            End If

            Me.PeakNO = clone.PeakNO
            Me.PrecursorIon = clone.PrecursorIon
            Me.RT = clone.RT
            Me.Candidates = clone.Candidates _
                .SafeQuery _
                .Select(Function(a) New CandidateResult(a)) _
                .ToArray
        End Sub

        Public Overrides Function ToString() As String
            Dim candidateNames$

            If Candidates.Count = 0 Then
                candidateNames = "no hits"
            Else
                candidateNames = Candidates.Take(6).Select(Function(c) c.Name).JoinBy(", ")
                candidateNames = If(candidateNames.Length > 64, candidateNames.Substring(0, 63) & "...", candidateNames)
            End If

            Return $"[#{PeakNO}] {PrecursorIon.ToString("F4")} {candidateNames}"
        End Function

        Public Shared Function ParseMs1PeakList(file As IEnumerable(Of String)) As Query()
            Return file _
                .Select(Function(line) line.StringSplit("\s+")) _
                .Select(Function(tokens)
                            Return New Query With {
                                .PeakNO = Integer.Parse(tokens(Scan0)),
                                .PrecursorIon = Val(tokens(1))
                            }
                        End Function) _
                .OrderBy(Function(q) q.PeakNO) _
                .ToArray
        End Function

    End Class
End Namespace
