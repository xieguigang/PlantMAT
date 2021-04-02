Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Parallel
Imports Parallel.ThreadTask
Imports PlantMAT.Core.Models
Imports snowFall.Protocol
Imports stdNum = System.Math

Public Module ParallelPipeline

    Private Function MS1CPTask(query As IEnumerable(Of Query), libfile As String, settings As Settings, ionMode As Integer) As Query()
        Dim snowFall As SlaveTask = Host.CreateSlave


    End Function

    <Extension>
    Public Function MS1CP(query As Query(), library As Library(), settings As Settings, Optional ionMode As Integer = 1) As Query()
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

        If App.IsMicrosoftPlatform Then
            runParallel = (Iterator Function() As IEnumerable(Of Query)
                               For Each block As IEnumerable(Of Query) In From group As NamedCollection(Of Query)
                                                                          In Algorithm.MS1TopDown.GroupQueryByMz(query) _
                                                                              .AsParallel _
                                                                              .WithDegreeOfParallelism(PublicVSCode.Parallelism)
                                                                          Select Algorithm.MS1TopDown.MS1CP(group, library, settings, ionMode)
                                   For Each item As Query In block
                                       Yield item
                                   Next
                               Next
                           End Function)()
        Else
            Dim tmp As String = App.GetAppSysTempFile(".table_reflib", App.PID.ToHexString, prefix:="PlantMAT")
            Dim taskList As Func(Of Query())() = Algorithm.MS1TopDown _
                .GroupQueryByMz(query) _
                .Select(Function(p) New Func(Of Query())(Function() MS1CPTask(p, tmp, settings, ionMode))) _
                .ToArray

            Using file As Stream = tmp.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Call Models.Library.WriteToStream(library, file)
            End Using

            runParallel = (Iterator Function() As IEnumerable(Of Query)
                               For Each block As Query() In New ThreadTask(Of Query())(taskList) _
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
