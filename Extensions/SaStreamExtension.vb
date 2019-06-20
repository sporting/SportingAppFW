'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text

Namespace Extensions
    Public Module SaStreamExtension

        <Extension()>
        Public Function CopyTo(ByVal input As Stream, ByVal output As Stream) As Boolean
            Dim buffer As Byte() = New Byte(16383) {}
            Dim bytesRead As Integer

            While (bytesRead = input.Read(buffer, 0, buffer.Length)) > 0
                output.Write(buffer, 0, bytesRead)
            End While
        End Function

    End Module

End Namespace