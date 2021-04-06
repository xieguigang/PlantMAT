#Region "Microsoft.VisualBasic::b1ab9f25b5f166c50cd9bb48c8878474, PlantMAT.Core\Algorithm\InternalCache.vb"

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

'     Class QueryPopulator
' 
' 
' 
'     Class ArrayPopulator
' 
'         Properties: array
' 
'         Function: GetQueries, ToString
' 
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

Imports System.IO
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Parallel.IpcStream
Imports PlantMAT.Core.Models

Namespace Algorithm.InternalCache

    Public MustInherit Class QueryPopulator

        Public MustOverride Function GetQueries() As IEnumerable(Of Query)

    End Class

    Public Class ArrayPopulator : Inherits QueryPopulator

        Public Property array As Query()

        Public Overrides Function ToString() As String
            Return $"memory_cache: {array.Length} queries"
        End Function

        Public Overrides Function GetQueries() As IEnumerable(Of Query)
            Return array
        End Function
    End Class

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

                    Yield json.CreateObject(Of Query)
                Loop
            End Using
        End Function
    End Class

    Public Class CacheFileWriter : Implements IDisposable

        ReadOnly cacheFile As String
        ReadOnly bin As BinaryDataWriter

        Private disposedValue As Boolean

        Sub New(cache As String)
            cacheFile = cache
            bin = New BinaryDataWriter(cache.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False))
        End Sub

        Sub New(cache As SocketRef)
            Call Me.New(cache.address)
        End Sub

        Public Sub AddQuery(query As Query)
            Dim json As JsonObject = GetType(Query) _
                .GetJsonElement(query, New JSONSerializerOptions) _
                .As(Of JsonObject)

            Using buffer As MemoryStream = BSON.GetBuffer(json)
                Call bin.Write(buffer.Length)
                Call bin.Write(buffer.ToArray)
            End Using

            Call json.Dispose()
        End Sub

        Public Overrides Function ToString() As String
            Return $"cache: {cacheFile}"
        End Function

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: 释放托管状态(托管对象)
                    Call bin.Flush()
                    Call bin.Dispose()
                End If

                ' TODO: 释放未托管的资源(未托管的对象)并替代终结器
                ' TODO: 将大型字段设置为 null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
        ' Protected Overrides Sub Finalize()
        '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class

End Namespace

