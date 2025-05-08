#Region "Microsoft.VisualBasic::78167a32100a36ab2c3ebaa6ca889ce3, PlantMAT.Core\zzz.vb"

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
Imports SMRUCC.Rsharp.Runtime.Interop

<Assembly: RPackageModule()>

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

    ' Cyanidin 3-glucoside: C1=CC(=C(C=C1C2=[O+]C3=CC(=CC(=C3C=C2OC4C(C(C(C(O4)CO)O)O)O)O)O)O)O.[Cl-]
    '                       C1=CC(=C(C=C1C2=[O+]C3=CC(=CC(=C3C=C2O ->   C4C(C(C(C(O4)CO)O)O)O    <- )O)O)O)O.[Cl-]
    ' Cyanidin:             C1=CC(=C(C=C1C2=[O+]C3=CC(=CC(=C3C=C2O                                  )O)O)O)O
    ' Glucoside:                                                     C(C1C(C(C(C(O1)O)O)O)O)O
    ' D-Glucose:                                                     C(C1C(C(C(C(O1)O)O)O)O)O

End Class
