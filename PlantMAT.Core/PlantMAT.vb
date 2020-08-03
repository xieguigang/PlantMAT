﻿Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' PlantMAT: A Metabolomics Tool for Predicting the Specialized 
''' Metabolic Potential of a System and for Large-Scale Metabolite 
''' Identifications
''' </summary>
<Package("PlantMAT", Category:=APICategories.ResearchTools, Cites:="", Url:="https://pubs.acs.org/doi/10.1021/acs.analchem.6b00906")>
Module PlantMAT

    Sub New()
        Internal.ConsolePrinter.AttachConsoleFormatter(Of Settings)(Function(o) DirectCast(o, Settings).ToString)
    End Sub

    ''' <summary>
    ''' create plantMAT configuration
    ''' </summary>
    ''' <param name="ExternalAglyconeDatabase"></param>
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
    ''' <param name="PrecursorIonType"></param>
    ''' <param name="PrecursorIonMZ"></param>
    ''' <param name="PrecursorIonN"></param>
    ''' <param name="SearchPPM"></param>
    ''' <param name="NoiseFilter"></param>
    ''' <param name="mzPPM"></param>
    ''' <param name="PatternPrediction"></param>
    ''' <returns></returns>
    <ExportAPI("config")>
    Public Function GetConfig(Optional ExternalAglyconeDatabase As String = Nothing,
                              Optional AglyconeType As db_AglyconeType = db_AglyconeType.All,
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
                              Optional PrecursorIonType As String = "[M-H]-",
                              Optional PrecursorIonMZ As Double = -1.007277,
                              Optional PrecursorIonN As Integer = 1,
                              Optional SearchPPM As Double = 10,
                              Optional NoiseFilter As Double = 0.05,
                              Optional mzPPM As Double = 15,
                              Optional PatternPrediction As Boolean = True) As Settings

        Return New Settings With {
            .InternalAglyconeDatabase = Not ExternalAglyconeDatabase.FileExists,
            .ExternalAglyconeDatabase = ExternalAglyconeDatabase,
            .AglyconeMWRange = AglyconeMWRange,
            .AglyconeSource = AglyconeSource,
            .AglyconeType = AglyconeType,
            .mzPPM = mzPPM,
            .NoiseFilter = NoiseFilter,
            .NumofAcidAll = NumofAcidAll,
            .NumofAcidMal = NumofAcidMal,
            .NumofAcidDDMP = NumofAcidDDMP,
            .NumofAcidSin = NumofAcidSin,
            .NumofAcidFer = NumofAcidFer,
            .NumofAcidCou = NumofAcidCou,
            .NumofSugarAll = NumofSugarAll,
            .NumofSugardHex = NumofSugardHex,
            .NumofSugarHex = NumofSugarHex,
            .NumofSugarHexA = NumofSugarHexA,
            .NumofSugarPen = NumofSugarPen,
            .PatternPrediction = PatternPrediction,
            .PrecursorIonMZ = PrecursorIonMZ,
            .PrecursorIonN = PrecursorIonN,
            .PrecursorIonType = PrecursorIonType,
            .SearchPPM = SearchPPM
        }
    End Function

    ''' <summary>
    ''' read ms1 library file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.library")>
    Public Function readLibrary(file As String) As Library()
        Return file.LoadCsv(Of Library).ToArray
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
    ''' parse ms1 query data
    ''' </summary>
    ''' <param name="metabolite_list">the input query file content.</param>
    ''' <returns></returns>
    <ExportAPI("query.ms1")>
    Public Function ms1Query(metabolite_list As String()) As Query()
        Return Query.ParseMs1PeakList(file:=metabolite_list)
    End Function
End Module