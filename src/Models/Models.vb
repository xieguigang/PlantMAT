#Region "Microsoft.VisualBasic::335379dd641dfdced09aaee71f3b29db, PlantMAT.Core\Models\Models.vb"

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

    '     Enum db_AglyconeType
    ' 
    '         All, Lipid, Polyphenol, Steroid, Triterpene
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum db_AglyconeSource
    ' 
    '         All, Arabidopsis, Asparagus, Glycine, Glycyrrhiza
    '         Medicago, Solanum
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class db_SugarAcid
    ' 
    '         Properties: NameSA, TypeSA
    ' 
    '     Class db_PrecursorIon
    ' 
    '         Properties: Adducts, IonType, M
    ' 
    '     Class lb_AddedSugarAcid
    ' 
    '         Properties: NameSA, NumSAMax, NumSAMin, TypeSA
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Models

    Public Enum db_AglyconeType
        All
        Polyphenol
        Triterpene
        Steroid
        Lipid
    End Enum

    Public Enum db_AglyconeSource
        All
        Medicago
        Arabidopsis
        Asparagus
        Glycine
        Glycyrrhiza
        Solanum
    End Enum

    Public Class db_SugarAcid
        Public Property NameSA As String
        Public Property TypeSA As String
    End Class

    Public Class db_PrecursorIon
        Public Property IonType As String
        Public Property Adducts As Double
        Public Property M As Integer
    End Class

    Public Class lb_AddedSugarAcid
        Public Property NameSA As String
        Public Property TypeSA As String
        Public Property NumSAMin As String
        Public Property NumSAMax As String

        Default Public Property ListAccessor(i As Integer) As String
            Get
                Select Case i
                    Case 0 : Return NameSA
                    Case 1 : Return TypeSA
                    Case 2 : Return NumSAMin
                    Case 3 : Return NumSAMax
                    Case Else
                        Throw New InvalidOperationException
                End Select
            End Get
            Set
                Select Case i
                    Case 0 : NameSA = Value
                    Case 1 : TypeSA = Value
                    Case 2 : NumSAMin = Value
                    Case 3 : NumSAMax = Value
                    Case Else
                        Throw New InvalidOperationException
                End Select
            End Set
        End Property
    End Class

End Namespace
