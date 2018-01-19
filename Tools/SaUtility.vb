'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Reflection

Namespace Tools
    Public Module SaUtility
        ''' <summary>
        ''' the same with oracle function NVL
        ''' if Obj is null then return defaultVal
        ''' </summary>
        ''' <param name="Obj"></param>
        ''' <param name="defaultVal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function NVL(ByRef Obj As Object, ByVal defaultVal As Object) As Object
            Return IIf((Obj Is Nothing) OrElse (IsDBNull(Obj)), defaultVal, Obj)
        End Function

        ''' <summary>
        ''' Nothing or DBNull return True
        ''' </summary>
        ''' <param name="Obj"></param>
        ''' <returns></returns>
        Public Function IsNull(ByRef Obj As Object) As Boolean
            Return Obj Is Nothing OrElse IsDBNull(Obj)
        End Function

        Public Function GetAssemblyPath(ByRef t As Type) As String
            ' GetType(UsersComponent.CCSN002)

            Return t.Assembly.Location
        End Function

    End Module
End Namespace