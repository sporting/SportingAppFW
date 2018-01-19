'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Runtime.CompilerServices

Namespace Extensions
    Public Module SaIntExtension
        <Extension()>
        Public Function LowerBound(i As Int16, minInt As Integer) As Integer
            Return Math.Max(i, minInt)
        End Function


        <Extension()>
        Public Function UpperBound(i As Int16, maxInt As Integer) As Integer
            Return Math.Min(i, maxInt)
        End Function

        <Extension()>
        Public Function LowerBound(i As Integer, minInt As Integer) As Integer
            Return Math.Max(i, minInt)
        End Function


        <Extension()>
        Public Function UpperBound(i As Integer, maxInt As Integer) As Integer
            Return Math.Min(i, maxInt)
        End Function

        <Extension()>
        Public Function LowerBound(i As Int64, minInt As Integer) As Integer
            Return Math.Max(i, minInt)
        End Function


        <Extension()>
        Public Function UpperBound(i As Int64, maxInt As Integer) As Integer
            Return Math.Min(i, maxInt)
        End Function


        <Extension()>
        Public Function LowerBound(i As Decimal, minInt As Integer) As Integer
            Return Math.Max(i, minInt)
        End Function


        <Extension()>
        Public Function UpperBound(i As Decimal, maxInt As Integer) As Integer
            Return Math.Min(i, maxInt)
        End Function


        <Extension()>
        Public Function LowerBound(i As Double, minInt As Integer) As Integer
            Return Math.Max(i, minInt)
        End Function


        <Extension()>
        Public Function UpperBound(i As Double, maxInt As Integer) As Integer
            Return Math.Min(i, maxInt)
        End Function

    End Module

End Namespace