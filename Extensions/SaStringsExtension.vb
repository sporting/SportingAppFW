'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Runtime.CompilerServices

Namespace Extensions
    Public Module SaStringsExtension
        <Extension()>
        Public Function Sort(ByVal SS As String(), Optional ByVal reverse As Boolean = False, Optional ByVal StartIndex As Integer = 0, Optional ByVal Length As Integer = 10) As String()
            Try
                Dim strs = From s As String In SS
                           Select s
                           Order By s.Substring(IIf(StartIndex < 0, 0, IIf(StartIndex >= s.Length, (s.Length - 1).LowerBound(0), StartIndex)), IIf(Length < 1, s.Length, (s.Length - StartIndex).UpperBound(Length)))

                Return IIf(reverse, strs.Reverse().ToArray(), strs.ToArray())
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Return SS
            End Try
        End Function
    End Module
End Namespace