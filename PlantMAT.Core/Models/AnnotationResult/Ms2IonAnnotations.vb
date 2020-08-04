#Region "Microsoft.VisualBasic::25778540372c5cd2433a396e5aabd585, PlantMAT.Core\Models\AnnotationResult\Ms2IonAnnotations.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    '       Feng Qiu (fengqiu1982)
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

    '     Class Ms2IonAnnotations
    ' 
    '         Properties: aglycone, annotations, comment, title
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Models.AnnotationResult

    Public Class Ms2IonAnnotations

        Public Property title As String
        Public Property annotations As String()
        Public Property comment As String
        Public Property aglycone As Boolean

    End Class
End Namespace
