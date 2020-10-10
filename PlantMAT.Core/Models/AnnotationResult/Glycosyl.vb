#Region "Microsoft.VisualBasic::3c7a6a6c4d528b9b0ae7ad4927fa6f38, PlantMAT.Core\Models\AnnotationResult\Glycosyl.vb"

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

    '     Class Glycosyl
    ' 
    '         Properties: Best_n, Match_m, Match_n, Pred_n, pResult
    ' 
    '         Function: ToString
    ' 
    '     Class GlycosylPredition
    ' 
    '         Properties: score, struct
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Models.AnnotationResult

    Public Class Glycosyl

        Public Property pResult As GlycosylPredition()
        Public Property Match_m As Integer
        Public Property Pred_n As Integer
        Public Property Match_n As Integer

        Public Overrides Function ToString() As String
            Return CStr(Match_m) & "/" & CStr(Pred_n) & " candidates"
        End Function

    End Class

    Public Class GlycosylPredition

        Public Property score As Double
        Public Property struct As String

    End Class
End Namespace
