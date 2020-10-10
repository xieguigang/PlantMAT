#Region "Microsoft.VisualBasic::2df5597c1f01b2082918a61d3c1b1fab, PlantMAT.Core\Models\AnnotationResult\Ms2IonAnnotations.vb"

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

    '     Class Ms2IonAnnotations
    ' 
    '         Properties: aglycone, ions, title
    ' 
    '     Class IonAnnotation
    ' 
    '         Properties: annotation, ionAbu, productMz
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Models.AnnotationResult

    Public Class Ms2IonAnnotations

        Public Property ions As IonAnnotation()
        Public Property aglycone As Boolean

    End Class

    Public Class IonAnnotation
        Public Property productMz As Double
        Public Property ionAbu As Double
        Public Property annotation As String

    End Class
End Namespace
