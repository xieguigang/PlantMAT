﻿Namespace Models

    Public Enum NeutralTypes
        NA
        sugar
        acid
        methylate
    End Enum

    Friend Interface INeutralGroupHit

        Property ionName As String
        Property nHit As Integer
        Property type As NeutralTypes

    End Interface

End Namespace