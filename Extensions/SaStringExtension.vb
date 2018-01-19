'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CompilerServices

Namespace Extensions
    ''' <summary>
    ''' for string type extension method
    ''' </summary>
    ''' <remarks></remarks>
    Public Module SaStringExtension
        <Extension()>
        Public Function QuotedStr(ByVal S As String) As String
            Dim I As Integer
            Dim Result As String = S
            For I = Len(Result) - 1 To 0 Step -1
                If Result.Chars(I) = "'" Then
                    Result = Result.Insert(I, "'")
                End If
            Next
            Result = "'" + Result + "'"

            Return Result
        End Function

        ''' <summary>
        ''' True 表示式 : true, y, 1
        ''' </summary>
        ''' <param name="S"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension()>
        Public Function IsTrue(ByVal S As String) As Boolean
            Return Operators.CompareString(S, "true", True) = 0 _
            OrElse Operators.CompareString(S, "y", True) = 0 _
            OrElse Operators.CompareString(S, "1", True) = 0
        End Function

        <Extension()>
        Public Function IsEmpty(ByVal S As String) As Boolean
            Return S Is Nothing OrElse S.Trim() = String.Empty
        End Function

        <Extension()>
        Public Function IsInt(ByVal S As String) As Boolean
            Dim res As Integer
            Return Integer.TryParse(S, res)
        End Function

        <Extension()>
        Public Function ToInt(ByVal S As String) As Integer
            Dim res As Integer
            If Integer.TryParse(S, res) Then
                Return res
            End If
            Return Nothing
        End Function

        <Extension()>
        Public Function ToInt(ByVal S As String, ByVal defaultInt As Integer) As Integer
            Dim res As Integer
            If Integer.TryParse(S, res) Then
                Return res
            End If
            Return defaultInt
        End Function

        <Extension()>
        Public Function IsDecimal(ByVal S As String) As Boolean
            Dim res As Decimal
            Return Decimal.TryParse(S, res)
        End Function

        <Extension()>
        Public Function ToDecimal(ByVal S As String) As Decimal
            Dim res As Decimal
            If Decimal.TryParse(S, res) Then
                Return res
            End If
            Return Nothing
        End Function

        <Extension()>
        Public Function ToDecimal(ByVal S As String, ByVal defaultDecimal As Decimal) As Decimal
            Dim res As Decimal
            If Decimal.TryParse(S, res) Then
                Return res
            End If
            Return defaultDecimal
        End Function

        <Extension()>
        Public Function IsDouble(ByVal S As String) As Double
            Dim res As Double
            Return Double.TryParse(S, res)
        End Function

        <Extension()>
        Public Function ToDouble(ByVal S As String) As Double
            Dim res As Double
            If Double.TryParse(S, res) Then
                Return res
            End If
            Return Nothing
        End Function

        <Extension()>
        Public Function ToDouble(ByVal S As String, ByVal defaultDouble As Double) As Double
            Dim res As Double
            If Double.TryParse(S, res) Then
                Return res
            End If
            Return defaultDouble
        End Function

        <Extension()>
        Public Function ParserHexToInt(S As String) As Byte()
            Try
                S = S.Trim()
                If S.IsEmpty Then
                    Return Nothing
                End If

                Dim apdu_str As String() = S.Split(" ")
                If apdu_str.Length <= 1 Then
                    apdu_str = S.Split("-")
                End If
                Dim apdu_buffers = From t As String In apdu_str
                                   Select Convert.ToByte(Convert.ToInt16(t.Trim(), 16))

                Return apdu_buffers.ToArray()
            Catch
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' 80 CA 04 00 90 FE
        ''' 80 09 CA 04 55 FB
        ''' string transfer to byte
        ''' </summary>
        ''' <param name="S"></param>
        ''' <returns></returns>
        <Extension()>
        Public Function StringToBytes(S As String) As Byte()
            Dim ls As String() = S.Split(Environment.NewLine)
            Dim bs As List(Of Byte) = New List(Of Byte)()

            For Each l As String In ls
                bs.AddRange(ParserHexToInt(l.Trim()))
            Next

            Return bs.ToArray()
        End Function
    End Module
End Namespace