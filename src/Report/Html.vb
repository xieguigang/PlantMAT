#Region "Microsoft.VisualBasic::0eb3272e0dd0c626d8c5177b0a3fd2e3, PlantMAT.Core\Report\Html.vb"

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

    '     Module Html
    ' 
    '         Function: getBlankHtml, GetReportHtml
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports PlantMAT.Core.Models
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]

Namespace Report

    Module Html

        Private Function getBlankHtml() As ScriptBuilder
            Return New ScriptBuilder(
                <html>
                    <head>
                        <meta charset="UTF-8"/>
                        <meta http-equiv="X-UA-Compatible" content="ie=edge"/>
                        <meta name='renderer' content='webkit'/>
                        <meta name="viewport" content="width=device-width, initial-scale=1, minimum-scale=1, maximum-scale=1, user-scalable=no"/>

                        <title>{$title}</title>

                        <link rel="stylesheet" href="https://cdn.biodeep.cn/styles/bootstrap-3.3.7/dist/css/bootstrap.min.css"/>
                        <link rel="stylesheet" href="https://cdn.biodeep.cn/styles/font-awesome-4.7.0/css/font-awesome.min.css"/>

                        <link rel="icon" href="https://cdn.biodeep.cn/favicon.ico" type="image/x-icon"/>

                        <script type="text/javascript" src="https://cdn.biodeep.cn/vendor/jquery-3.2.1.min.js"></script>
                        <script type="text/javascript" src="https://cdn.biodeep.cn/styles/bootstrap-3.3.7/dist/js/bootstrap.min.js"></script>
                    </head>
                    <body>
                        {$report}
                    </body>
                </html>)
        End Function

        Public Function GetReportHtml(result As Query(), args As list, env As Environment) As String
            Dim html As ScriptBuilder = getBlankHtml()

            Return html.ToString
        End Function
    End Module
End Namespace
