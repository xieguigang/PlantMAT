Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine

Public Class zzz

    Const info As String =
        "Custom software entitled Plant Metabolite Annotation Toolbox (PlantMAT)" & vbCrLf &
        "has been developed to address the number one grand challenge in metabolomics," & vbCrLf &
        "which is the large-scale and confident identification of metabolites." & vbCrLf & vbCrLf &
        "PlantMAT uses informed phytochemical knowledge for the prediction of plant" & vbCrLf &
        "natural products such as saponins and glycosylated flavonoids through " & vbCrLf &
        "combinatorial enumeration of aglycone, glycosyl, and acyl subunits. " & vbCrLf & vbCrLf &
        "Many of the predicted structures have yet to be characterized and are " & vbCrLf &
        "absent from traditional chemical databases, but have a higher probability " & vbCrLf &
        "of being present in planta. PlantMAT allows users to operate an automated " & vbCrLf &
        "and streamlined workflow for metabolite annotation from a user-friendly " & vbCrLf &
        "interface within Microsoft Excel, a familiar, easily accessed program for " & vbCrLf &
        "chemists and biologists. " & vbCrLf & vbCrLf &
        "The usefulness of PlantMAT is exemplified using ultrahigh-performance liquid " & vbCrLf &
        "chromatography–electrospray ionization quadrupole time-of-flight tandem" & vbCrLf &
        "mass spectrometry (UHPLC–ESI-QTOF-MS/MS) metabolite profiling data of saponins" & vbCrLf &
        "and glycosylated flavonoids from the model legume Medicago truncatula. " & vbCrLf & vbCrLf &
        "The results demonstrate PlantMAT substantially increases the chemical/metabolic " & vbCrLf &
        "space of traditional chemical databases. Ten of the PlantMAT-predicted " & vbCrLf &
        "identifications were validated and confirmed through the isolation of the" & vbCrLf &
        "compounds using ultrahigh-performance liquid chromatography mass spectrometry" & vbCrLf &
        "solid-phase extraction (UHPLC–MS–SPE) followed by de novo structural" & vbCrLf &
        "elucidation using 1D/2D nuclear magnetic resonance (NMR). " & vbCrLf & vbCrLf &
        "It is further demonstrated that PlantMAT enables the dereplication of " & vbCrLf &
        "previously identified metabolites and is also a powerful tool for the" & vbCrLf &
        "discovery of structurally novel metabolites." & vbCrLf & vbCrLf & vbCrLf

    Public Shared Sub onLoad()
        Call GetType(PlantMAT).Assembly _
            .FromAssembly _
            .AppSummary(
                description:=info,
                SYNOPSIS:=Nothing,
                write:=App.StdOut
            )
    End Sub
End Class
