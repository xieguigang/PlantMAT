#Region "Microsoft.VisualBasic::0ed2776c18a7db47dc82bc9b36a36996, PlantMAT.Core\Models\Ms2Peaks.vb"

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

'     Class Ms2Peaks
' 
'         Properties: fragments, into, mz, TotalIonInt
' 
'         Function: ParseMs2
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Namespace Models

    ''' <summary>
    ''' [mz, into] matrix
    ''' </summary>
    Public Class Ms2Peaks

        Public Property mz As Double()
        Public Property into As Double()

        Public ReadOnly Property TotalIonInt As Double
            Get
                Return into.Sum
            End Get
        End Property

        Public ReadOnly Property fragments As Integer
            Get
                Return mz.Length
            End Get
        End Property

        Public Function GetMs2() As ms2()
            Return mz _
                .Select(Function(mzi, i)
                            Return New ms2 With {
                                .mz = mzi,
                                .intensity = into(i)
                            }
                        End Function) _
                .ToArray
        End Function

        Public Shared Function ParseMs2(file As IEnumerable(Of String)) As Ms2Peaks
            Dim raw As Double()() = file _
                .Select(Function(line)
                            Return line _
                                .StringSplit("\s+") _
                                .Select(AddressOf Val) _
                                .ToArray
                        End Function) _
                .ToArray
            Dim mz = raw.Select(Function(a) a(Scan0)).ToArray
            Dim into = raw.Select(Function(a) a(1)).ToArray

            Return New Ms2Peaks With {
                .mz = mz,
                .into = into
            }
        End Function
    End Class
End Namespace
