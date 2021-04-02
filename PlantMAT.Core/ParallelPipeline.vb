Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports PlantMAT.Core.Models
Imports stdNum = System.Math

Public Module ParallelPipeline

    <Extension>
    Public Function MS1CP(query As Query(), library As Library(), settings As Settings, Optional ionMode As Integer = 1) As Query()
        Dim result As New List(Of Query)(query.Length)
        Dim start = App.NanoTime
        Dim elapse As Double
        Dim speed As Double
        Dim ETA As TimeSpan

        ' Run combinatorial enumeration
        Console.WriteLine("Now analyzing, please wait...")
        Console.WriteLine("Peform combinatorial enumeration and show the calculation progress (MS1CP)")
        Console.WriteLine($" --> {query.Length} queries...")

        Dim runParallel = From group As NamedCollection(Of Query)
                          In Algorithm.MS1TopDown.GroupQueryByMz(query) _
                              .AsParallel _
                              .WithDegreeOfParallelism(PublicVSCode.Parallelism)
                          Select New Algorithm.MS1TopDown(library, settings).CombinatorialPrediction(group, ionMode)

        For Each item As Query In runParallel.IteratesALL
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
