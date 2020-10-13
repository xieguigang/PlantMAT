#Region "Microsoft.VisualBasic::645ce6a975a321e01d19db6fc6162161, PlantMAT.Core\zzz.vb"

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

    ' Class zzz
    ' 
    '     Sub: onLoad
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine

Public Class zzz

    Public Shared Sub onLoad()
        Call GetType(PlantMAT).Assembly _
            .FromAssembly _
            .AppSummary(
                description:="Welcome to the PlantMAT toolkit!",
                SYNOPSIS:=Nothing,
                write:=App.StdOut
            )

        Call Console.WriteLine("You could modify of the PlantMAT parallel by specific argument when running PlantMAT with R#:")
        Call Console.WriteLine("")
        Call Console.WriteLine("    --parallel <n_threads>")
        Call Console.WriteLine("")
    End Sub
End Class
