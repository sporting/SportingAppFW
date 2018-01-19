'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Runtime.CompilerServices
Namespace Extensions
    Public Module SaCharExtension
        <Extension()>
        Public Function Repeat(ByVal C As Char, ByVal times As Integer) As String
            Dim s As String = String.Empty
            For i As Integer = 1 To times
                s = s + C
            Next
            Return s
        End Function

        <Extension()>
        Public Function IsEmpty(ByVal C As Char) As Boolean
            Return C.Equals("") Or C.Equals(" "c)
        End Function
    End Module
End Namespace