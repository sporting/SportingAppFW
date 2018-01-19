'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Runtime.CompilerServices
Imports System.Reflection
Imports System.Text

Namespace Extensions

    Public Module SaIEnumerableExtension
        <Extension()>
        Public Function StringJoin(Of T)(ByVal collection As IEnumerable(Of T), ByVal seperator As String) As String
            Dim strList As List(Of String) = New List(Of String)()

            For Each iter In collection
                strList.Add(iter.ToString())
            Next

            Return String.Join(seperator, strList.ToArray())
        End Function

    End Module
End Namespace
