
'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Globalization
Imports System.Runtime.CompilerServices

Namespace Extensions
    Public Module SaDateExtension
        <Extension()>
        Public Function DateToTaiwanCalendarStr(dt As DateTime, Optional seperate As String = "/") As String
            Dim tc As TaiwanCalendar = New TaiwanCalendar()

            Return tc.GetYear(Date.Now).ToString() + seperate + tc.GetMonth(Date.Now).ToString() + seperate + tc.GetDayOfMonth(Date.Now).ToString()
        End Function
    End Module

End Namespace