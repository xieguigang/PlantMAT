Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime

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
    ''' performs combinatorial enumeration
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <ExportAPI("MS1TopDown")>
    Public Function MS1TopDown(library As Library(), settings As Settings) As MS1TopDown
        Return New MS1TopDown(library, settings)
    End Function


End Module
