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
    Public Module SaBytesExtension

        <Extension()>
        Public Function IsValidImage(ByVal bytes As Byte()) As Boolean
            Try
                Using ms = New MemoryStream(bytes)
                    Image.FromStream(ms)
                End Using
            Catch ex As ArgumentException
                Return False
            End Try
            Return True
        End Function


        <Extension()>
        Public Function ToImage(ByVal bytes As Byte()) As Image
            Dim img As Image
            Try
                Using ms = New MemoryStream(bytes)
                    img = Image.FromStream(ms)
                End Using
            Catch ex As ArgumentException
                Return Nothing
            End Try
            Return img
        End Function

        <Extension()>
        Public Function TrimEnd(bytes As Byte(), bi As Integer) As Byte()
            Dim i As Integer = bytes.Length - 1
            While bytes(i) = bi
                i = i - 1
            End While

            Dim temp(i + 1) As Byte
            Array.Copy(bytes, temp, i + 1)
            Return temp
        End Function

        <Extension()>
        Public Function Cut(bytes As Byte(), startpos As Integer, length As Integer) As Byte()
            Dim temp(length) As Byte
            Array.Copy(bytes, startpos, temp, 0, length)
            Return temp
        End Function

        ''' <summary>
        ''' default input bytes is Int
        ''' </summary>
        ''' <param name="bytes"></param>
        ''' <returns></returns>
        <Extension()>
        Public Function BytesToStrings(bytes As Byte(), Optional isHex As Boolean = False) As String()
            Dim line As String
            Dim sb As List(Of String) = New List(Of String)()
            If isHex Then
                Dim newbytes = From b As Byte In bytes
                               Select Convert.ToInt16(b, 16)
                line = Encoding.Default.GetString(newbytes)
            Else
                line = Encoding.Default.GetString(bytes)
            End If

            For Each s As String In line.Split(vbFormFeed)
                sb.Add(s)
            Next

            Return sb.ToArray()
        End Function

        ''' <summary>
        ''' String is Hex to Int
        ''' </summary>
        ''' <param name="bs"></param>
        ''' <returns></returns>
        <Extension()>
        Public Function BytesToStrings(bs As String) As String()
            Return StringToBytes(bs).BytesToStrings()
        End Function

    End Module

End Namespace