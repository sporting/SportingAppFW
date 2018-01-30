'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Namespace Data.Common

    Module SaConstant
        ''' <summary>
        ''' FN component series, fetch rows from database each time
        ''' </summary>
        Public ReadOnly FN_LIMIT_ROWS_TO_LOAD As Integer = 10

        Public ReadOnly FRAMEWORK_NAMESPACE As String = "SportingApp"

        Public ReadOnly FRAMEWORK_NAMESPACE_LOG As String = "SportingAppLog"

        Public ReadOnly WorkingDirectory As String = CurDir()
    End Module

End Namespace