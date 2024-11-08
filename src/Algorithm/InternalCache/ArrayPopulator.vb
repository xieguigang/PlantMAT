﻿#Region "Microsoft.VisualBasic::8fb348004bfdab881608e057cda2b1e0, PlantMAT.Core\Algorithm\InternalCache\ArrayPopulator.vb"

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

    '     Class ArrayPopulator
    ' 
    '         Properties: array
    ' 
    '         Function: GetQueries, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports PlantMAT.Core.Models

Namespace Algorithm.InternalCache

    Public Class ArrayPopulator : Inherits QueryPopulator

        Public Property array As Query()

        Public Overrides Function ToString() As String
            Return $"memory_cache: {array.Length} queries"
        End Function

        Public Overrides Function GetQueries() As IEnumerable(Of Query)
            Return array
        End Function
    End Class
End Namespace
