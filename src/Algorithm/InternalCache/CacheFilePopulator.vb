﻿#Region "Microsoft.VisualBasic::13c18ec3435df5dfc9891a1c05791fbe, PlantMAT.Core\Algorithm\InternalCache\CacheFilePopulator.vb"

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

'     Class CacheFilePopulator
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: GetQueries, ToString
' 
'         Sub: Delete
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports PlantMAT.Core.Models

Namespace Algorithm.InternalCache

    ''' <summary>
    ''' use cache file for solve memory problem
    ''' </summary>
    Public Class CacheFilePopulator : Inherits QueryPopulator

        ReadOnly cacheFile As String

        Sub New(cache As String)
            cacheFile = cache
        End Sub

        Public Overrides Function ToString() As String
            Return $"cache: {cacheFile}"
        End Function

        Public Sub Delete()
            Call cacheFile.DeleteFile

            If Not cacheFile.FileExists Then
                Call Console.WriteLine($"cache file `{cacheFile}` cleanup!")
            End If
        End Sub

        Public Overrides Iterator Function GetQueries() As IEnumerable(Of Query)
            Using reader As New BinaryDataReader(cacheFile.Open)
                Do While Not reader.EndOfStream
                    Dim size As Long = reader.ReadInt64
                    Dim buffer As Byte() = reader.ReadBytes(size)
                    Dim json As JsonObject = BSON.Load(buffer)

                    Erase buffer

                    Yield json.CreateObject(Of Query)(decodeMetachar:=False)
                Loop
            End Using
        End Function
    End Class

End Namespace
