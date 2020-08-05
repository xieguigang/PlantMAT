#Region "Microsoft.VisualBasic::d26307c6ab412b9ba826634d3deec89d, PlantMAT.Core\PlantMAT.vb"

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

' Module PlantMAT
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: GetConfig, joinMs2Query, ms1Query, MS1TopDown, MS2ATopDown
'               ParseConfig, QueryFromMgf, readLibrary, reportTable
' 
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports PlantMAT.Core.Algorithm
Imports PlantMAT.Core.Models
Imports PlantMAT.Core.Report
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' PlantMAT: A Metabolomics Tool for Predicting the Specialized 
''' Metabolic Potential of a System and for Large-Scale Metabolite 
''' Identifications
''' </summary>
<Package("PlantMAT",
         Category:=APICategories.ResearchTools,
         Cites:="",
         Url:="https://pubs.acs.org/doi/10.1021/acs.analchem.6b00906",
         Description:="
Custom software entitled Plant Metabolite Annotation Toolbox (PlantMAT) 
has been developed to address the number one grand challenge in metabolomics, 
which is the large-scale and confident identification of metabolites. 

PlantMAT uses informed phytochemical knowledge for the prediction of plant 
natural products such as saponins and glycosylated flavonoids through 
combinatorial enumeration of aglycone, glycosyl, and acyl subunits. Many of 
the predicted structures have yet to be characterized and are absent from 
traditional chemical databases, but have a higher probability of being 
present in planta. 
         
PlantMAT allows users to operate an automated and streamlined workflow for 
metabolite annotation from a user-friendly interface within Microsoft Excel, 
a familiar, easily accessed program for chemists and biologists. The 
usefulness of PlantMAT is exemplified using ultrahigh-performance liquid 
chromatography–electrospray ionization quadrupole time-of-flight tandem 
mass spectrometry (UHPLC–ESI-QTOF-MS/MS) metabolite profiling data of 
saponins and glycosylated flavonoids from the model legume Medicago 
truncatula. 

The results demonstrate PlantMAT substantially increases the chemical/metabolic 
space of traditional chemical databases. Ten of the PlantMAT-predicted 
identifications were validated and confirmed through the isolation of the 
compounds using ultrahigh-performance liquid chromatography mass spectrometry 
solid-phase extraction (UHPLC–MS–SPE) followed by de novo structural 
elucidation using 1D/2D nuclear magnetic resonance (NMR). It is further 
demonstrated that PlantMAT enables the dereplication of previously identified 
metabolites and is also a powerful tool for the discovery of structurally 
novel metabolites.
")>
<RTypeExport("precursor", GetType(PrecursorInfo))>
Module PlantMAT

    Sub New()
        Internal.ConsolePrinter.AttachConsoleFormatter(Of Settings)(Function(o) DirectCast(o, Settings).ToString)
        Internal.htmlPrinter.AttachHtmlFormatter(Of Query())(AddressOf Html.GetReportHtml)
    End Sub

    ''' <summary>
    ''' create plantMAT configuration
    ''' 
    ''' if all of the parameter is omit, then you can create a settings 
    ''' model with all configuration set to default values.
    ''' </summary>
    ''' <param name="AglyconeType"></param>
    ''' <param name="AglyconeSource"></param>
    ''' <param name="AglyconeMWRange"></param>
    ''' <param name="NumofSugarAll"></param>
    ''' <param name="NumofAcidAll"></param>
    ''' <param name="NumofSugarHex"></param>
    ''' <param name="NumofSugarHexA"></param>
    ''' <param name="NumofSugardHex"></param>
    ''' <param name="NumofSugarPen"></param>
    ''' <param name="NumofAcidMal"></param>
    ''' <param name="NumofAcidCou"></param>
    ''' <param name="NumofAcidFer"></param>
    ''' <param name="NumofAcidSin"></param>
    ''' <param name="NumofAcidDDMP"></param>
    ''' <param name="PrecursorIonType">
    ''' a character vector of list all precursor types that could be apply 
    ''' for search the ms1 annotation in plantMAT.
    ''' </param>
    ''' <param name="SearchPPM"></param>
    ''' <param name="NoiseFilter"></param>
    ''' <param name="mzPPM"></param>
    ''' <returns></returns>
    <ExportAPI("config")>
    Public Function GetConfig(Optional AglyconeType As db_AglyconeType = db_AglyconeType.All,
                              Optional AglyconeSource As db_AglyconeSource = db_AglyconeSource.All,
                              <RRawVectorArgument(GetType(Double))> Optional AglyconeMWRange As Object = "400,600",
                              <RRawVectorArgument(GetType(Integer))> Optional NumofSugarAll As Object = "0,6",
                              <RRawVectorArgument(GetType(Integer))> Optional NumofAcidAll As Object = "0,1",
                              <RRawVectorArgument(GetType(Integer))> Optional NumofSugarHex As Object = "0,6",
                              <RRawVectorArgument(GetType(Integer))> Optional NumofSugarHexA As Object = "0,6",
                              <RRawVectorArgument(GetType(Integer))> Optional NumofSugardHex As Object = "0,6",
                              <RRawVectorArgument(GetType(Integer))> Optional NumofSugarPen As Object = "0,6",
                              <RRawVectorArgument(GetType(Integer))> Optional NumofAcidMal As Object = "0,1",
                              <RRawVectorArgument(GetType(Integer))> Optional NumofAcidCou As Object = "0,1",
                              <RRawVectorArgument(GetType(Integer))> Optional NumofAcidFer As Object = "0,1",
                              <RRawVectorArgument(GetType(Integer))> Optional NumofAcidSin As Object = "0,1",
                              <RRawVectorArgument(GetType(Integer))> Optional NumofAcidDDMP As Object = "0,1",
                              <RRawVectorArgument(GetType(String))> Optional PrecursorIonType As Object = "[M]+|[M]-|[M+H]+|[M-H]-",
                              Optional SearchPPM As Double = 10,
                              Optional NoiseFilter As Double = 0.05,
                              Optional mzPPM As Double = 15) As Settings

        Return New Settings With {
            .AglyconeMWRange = DirectCast(AglyconeMWRange, Double()),
            .AglyconeSource = AglyconeSource,
            .AglyconeType = AglyconeType,
            .mzPPM = mzPPM,
            .NoiseFilter = NoiseFilter,
            .NumofAcidAll = DirectCast(NumofAcidAll, Integer()),
            .NumofAcidMal = DirectCast(NumofAcidMal, Integer()),
            .NumofAcidDDMP = DirectCast(NumofAcidDDMP, Integer()),
            .NumofAcidSin = DirectCast(NumofAcidSin, Integer()),
            .NumofAcidFer = DirectCast(NumofAcidFer, Integer()),
            .NumofAcidCou = DirectCast(NumofAcidCou, Integer()),
            .NumofSugarAll = DirectCast(NumofSugarAll, Integer()),
            .NumofSugardHex = DirectCast(NumofSugardHex, Integer()),
            .NumofSugarHex = DirectCast(NumofSugarHex, Integer()),
            .NumofSugarHexA = DirectCast(NumofSugarHexA, Integer()),
            .NumofSugarPen = DirectCast(NumofSugarPen, Integer()),
            .PrecursorIonType = PrecursorIonType,
            .SearchPPM = SearchPPM
        }
    End Function

    ''' <summary>
    ''' parse the settings value from a given json string
    ''' </summary>
    ''' <param name="json">settings value in json text format.</param>
    ''' <returns></returns>
    <ExportAPI("parse.config")>
    Public Function ParseConfig(json As String) As Settings
        Return json.LoadJSON(Of Settings)
    End Function

    ''' <summary>
    ''' read ms1 library file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.library")>
    Public Function readLibrary(file As String) As Library()
        Return file.LoadCsv(Of Library)(mute:=True).ToArray
    End Function

    ''' <summary>
    ''' performs combinatorial enumeration, and show the calculation progress (MS1CP)
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <ExportAPI("MS1TopDown")>
    Public Function MS1TopDown(library As Library(), settings As Settings) As MS1TopDown
        Return New MS1TopDown(library, settings)
    End Function

    ''' <summary>
    ''' performs MS2 annotation
    ''' </summary>
    ''' <param name="settings"></param>
    ''' <returns></returns>
    <ExportAPI("MS2ATopDown")>
    Public Function MS2ATopDown(settings As Settings) As MS2ATopDown
        Return New MS2ATopDown(settings)
    End Function

    ''' <summary>
    ''' parse ms1 query data
    ''' </summary>
    ''' <param name="metabolite_list">the input query file content.</param>
    ''' <returns></returns>
    <ExportAPI("query.ms1")>
    Public Function ms1Query(metabolite_list As String()) As Query()
        Return Query.ParseMs1PeakList(file:=metabolite_list)
    End Function

    ''' <summary>
    ''' debug test tools
    ''' </summary>
    ''' <param name="mz">ms1 ``m/z`` value</param>
    ''' <param name="AglyW">exact mass</param>
    ''' <param name="Attn_w"></param>
    ''' <param name="nH2O_w"></param>
    ''' <param name="precursor_type"></param>
    ''' <returns></returns>
    <ExportAPI("ms1.err")>
    Public Function ms1Err(mz As Double, AglyW#, Attn_w#, nH2O_w#, Optional precursor_type$ = "[M+H]+") As Double
        Dim mz1 As Double = AglyW + Attn_w - nH2O_w
        Dim precursor As PrecursorInfo = PublicVSCode.GetPrecursorInfo(precursor_type)

        Return PPMmethod.ppm(mz1 + precursor.adduct, mz)
    End Function

    ''' <summary>
    ''' join ms2 spectra data with the corresponding ms1 query values
    ''' </summary>
    ''' <param name="ms1">the ms1 peak features</param>
    ''' <param name="files">a file path vector of the ms2 spectra matrix list for each ms1 peaks</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("join.ms2")>
    <RApiReturn(GetType(Query))>
    Public Function joinMs2Query(<RRawVectorArgument> ms1 As Object, files As String(), Optional env As Environment = Nothing) As Object
        Dim queries As pipeline = pipeline.TryCreatePipeline(Of Query)(ms1, env)

        If queries.isError Then
            Return queries.getError
        End If

        Dim fileIndex = files.ToDictionary(Function(path) path.BaseName)
        Dim joinIterator =
            Iterator Function() As IEnumerable(Of Query)
                For Each query As Query In queries.populates(Of Query)(env)
                    If fileIndex.ContainsKey(query.PeakNO.ToString) Then
                        query.Ms2Peaks = Ms2Peaks.ParseMs2(fileIndex(query.PeakNO.ToString).ReadAllLines)
                    End If

                    Yield query
                Next
            End Function

        Return joinIterator().ToArray
    End Function

    ''' <summary>
    ''' create plantMAT query from the given mgf ion stream
    ''' </summary>
    ''' <param name="mgf"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("as.query")>
    <RApiReturn(GetType(Query))>
    Public Function QueryFromMgf(<RRawVectorArgument> mgf As Object, Optional env As Environment = Nothing) As Object
        Dim ions As pipeline = pipeline.TryCreatePipeline(Of Ions)(mgf, env)

        If ions.isError Then
            Return ions.getError
        End If

        Return ions.populates(Of Ions)(env) _
            .AsParallel _
            .Select(Function(ion)
                        Return PublicVSCode.QueryFromMgf(ion)
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' read the query result json file
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the json file or the json string text
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("read.query_result")>
    Public Function readResultJSON(file As String) As Query()
        Return file _
            .SolveStream _
            .ParseJson _
            .CreateObject(GetType(Query()))
    End Function

    <ExportAPI("result.json")>
    <RApiReturn(GetType(String))>
    Public Function toResultJSON(<RRawVectorArgument> result As Object, Optional env As Environment = Nothing) As Object
        Dim data As pipeline = pipeline.TryCreatePipeline(Of Query)(result, env)

        If data.isError Then
            Return data.getError
        End If

        Dim raw = data.populates(Of Query)(env).ToArray
        Dim json As String = JSONSerializer.GetJson(raw, maskReadonly:=True)

        Return json
    End Function

    ''' <summary>
    ''' run report table output
    ''' </summary>
    ''' <param name="result"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("report.table")>
    <RApiReturn(GetType(Report.Table))>
    Public Function reportTable(<RRawVectorArgument> result As Object, Optional env As Environment = Nothing) As Object
        Dim data As pipeline = pipeline.TryCreatePipeline(Of Query)(result, env)

        If data.isError Then
            Return data.getError
        End If

        Return data.populates(Of Query)(env) _
            .Select(AddressOf Report.Table.PopulateRows) _
            .IteratesALL _
            .ToArray
    End Function
End Module
