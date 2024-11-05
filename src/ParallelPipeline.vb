#Region "Microsoft.VisualBasic::711ac2ee952da1c06d0fa34d866f02c2, PlantMAT.Core\ParallelPipeline.vb"

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

' Module ParallelPipeline
' 
'     Function: MS1CP, MS1CPTask
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Darwinism.HPC.Parallel
Imports Darwinism.HPC.Parallel.IpcStream
Imports Darwinism.HPC.Parallel.ThreadTask
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports PlantMAT.Core.Models
Imports snowFall.Protocol
Imports stdNum = System.Math

Public Module ParallelPipeline

    Private Function MS1CPTask(query As IEnumerable(Of NamedCollection(Of Query)),
                               libfile As SocketRef,
                               settings As Settings,
                               ionMode As Integer,
                               verbose As Boolean,
                               debugPort As Integer?) As Query()

        Dim snowFall As SlaveTask = Host.CreateSlave(debugPort:=debugPort)
        Dim api As New Algorithm.IMS1TopDown(AddressOf Algorithm.MS1TopDown.MS1CP)
        Dim allPip As Query() = query _
            .Select(Function(p) p.AsEnumerable) _
            .IteratesALL _
            .ToArray
        Dim result As Query() = snowFall.RunTask(Of Query())(api, allPip, libfile, settings, ionMode)

        Return result
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="library"></param>
    ''' <param name="settings"></param>
    ''' <param name="ionMode"></param>
    ''' <param name="verbose"></param>
    ''' <param name="debugPort">debug of parallel library</param>
    ''' <param name="sequenceMode">debug of algorithm</param>
    ''' <returns></returns>
    <Extension>
    Public Function MS1CP(query As Query(), library As Library(), settings As Settings,
                          Optional ionMode As Integer = 1,
                          Optional verbose As Boolean = False,
                          Optional debugPort As Integer? = Nothing,
                          Optional sequenceMode As Boolean = False) As Query()

        Dim result As New List(Of Query)(query.Length)
        Dim start = App.NanoTime
        Dim elapse As Double
        Dim speed As Double
        Dim ETA As TimeSpan
        Dim runParallel As IEnumerable(Of Query)

        ' Run combinatorial enumeration
        Console.WriteLine("Now analyzing, please wait...")
        Console.WriteLine("Peform combinatorial enumeration and show the calculation progress (MS1CP)")
        Console.WriteLine($" --> {query.Length} queries...")

        If sequenceMode Then
            ' debug of the algorithm
            runParallel = Algorithm.MS1TopDown.MS1CP(query, library, settings, ionMode)
        ElseIf App.IsMicrosoftPlatform Then
            runParallel = (Iterator Function() As IEnumerable(Of Query)
                               For Each block As IEnumerable(Of Query) In From group As NamedCollection(Of Query)
                                                                          In Algorithm.MS1TopDown.GroupQueryByMz(query) _
                                                                              .AsParallel _
                                                                              .WithDegreeOfParallelism(PublicVSCode.Parallelism)
                                                                          Select Algorithm.MS1TopDown.MS1CP(group.ToArray, library, settings, ionMode)
                                   For Each item As Query In block
                                       Yield item
                                   Next
                               Next
                           End Function)()
        Else
            Dim socket As SocketRef = SocketRef.WriteBuffer(library)
            Dim mzList = Algorithm.MS1TopDown.GroupQueryByMz(query)
            Dim size As Integer = stdNum.Max(mzList.Length / (PublicVSCode.Parallelism + 1), 1)
            Dim taskList As Func(Of Query())() = mzList _
                .Split(size) _
                .Select(Function(p) New Func(Of Query())(Function() MS1CPTask(p, socket, settings, ionMode, verbose, debugPort:=debugPort))) _
                .ToArray

            Call Console.WriteLine($"Run parallel with {size} task elements in {taskList.Length} task queue!")

            runParallel = (Iterator Function() As IEnumerable(Of Query)
                               For Each block As Query() In New ThreadTask(Of Query())(taskList, debugMode:=Not debugPort Is Nothing) _
                                   .WithDegreeOfParallelism(PublicVSCode.Parallelism) _
                                   .RunParallel

                                   For Each item As Query In block
                                       Yield item
                                   Next
                               Next
                           End Function)()
        End If

        For Each item As Query In runParallel
            If item.Candidates.Length = 0 Then
                result.Add(Nothing)
            Else
                result.Add(item)
            End If

            elapse = (App.NanoTime - start) / TimeSpan.TicksPerMillisecond / 1000
            speed = result.Count / elapse
            ETA = TimeSpan.FromSeconds((query.Length - result.Count) / speed)

            Console.WriteLine($"[{result.Count}/{query.Length}] [{speed.ToString("F3").PadRight(3, "0")} query/sec, ETA {ETA.FormatTime}] {item.ToString} [{stdNum.Round(result.Count / query.Length * 100)}% done!]")
        Next

        ' Show the message box after the calculation is finished
        Console.WriteLine("Substructure prediction finished")

        Return result.Where(Function(a) Not a Is Nothing).ToArray
    End Function
End Module
