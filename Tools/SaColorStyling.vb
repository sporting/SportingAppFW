'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************

Imports System.Drawing
Imports System.Threading

Namespace Tools

    Public Class SaColorStyling
        Public Enum Theme As Integer
            'https://color.adobe.com
            PearLemonFizz = 0
            SkinTone = 1
            SweetSugar = 2
            Seaman = 3
            FlatDesign = 4
            BeachTime = 5
        End Enum

        Sub New()

        End Sub

        ''' <summary>
        ''' Return random color
        ''' </summary>
        ''' <param name="red_low"></param>
        ''' <param name="red_high"></param>
        ''' <param name="green_low"></param>
        ''' <param name="green_high"></param>
        ''' <param name="blue_low"></param>
        ''' <param name="blue_high"></param>
        ''' <returns></returns>
        Public Shared Function RandomColor(Optional ByVal red_low As Integer = 0, Optional ByVal red_high As Integer = 255, Optional ByVal green_low As Integer = 0, Optional ByVal green_high As Integer = 255, Optional ByVal blue_low As Integer = 0, Optional ByVal blue_high As Integer = 255) As Color
            Dim rnd1 As New Random
            Dim mColor As Color

            Try
                mColor = Color.FromArgb(rnd1.Next(red_low, red_high + 1), rnd1.Next(green_low, green_high + 1), rnd1.Next(blue_low, blue_high + 1))
            Catch ex As ArgumentException
                Console.Write(ex.Message)
            End Try

            Return mColor
        End Function

        ''' <summary>
        ''' Get the default Theme
        ''' </summary>
        ''' <param name="choosetheme"></param>
        ''' <returns></returns>
        Public Shared Function GetTheme(ByVal choosetheme As Theme) As Color()
            Dim colors As Color() = {}

            Select Case choosetheme
                Case Theme.PearLemonFizz
                    colors = {Color.FromArgb(4, 191, 191), Color.FromArgb(4, 191, 191), Color.FromArgb(247, 233, 103), Color.FromArgb(169, 207, 84), Color.FromArgb(88, 143, 39)}
                Case Theme.SkinTone
                    colors = {Color.FromArgb(255, 226, 197), Color.FromArgb(255, 238, 221), Color.FromArgb(255, 221, 170), Color.FromArgb(255, 196, 132), Color.FromArgb(255, 221, 153)}
                Case Theme.SweetSugar
                    colors = {Color.FromArgb(254, 67, 101), Color.FromArgb(252, 157, 154), Color.FromArgb(249, 205, 173), Color.FromArgb(200, 200, 169), Color.FromArgb(131, 175, 155)}
                Case Theme.Seaman
                    colors = {Color.FromArgb(48, 57, 79), Color.FromArgb(255, 67, 76), Color.FromArgb(106, 206, 235), Color.FromArgb(237, 232, 223), Color.FromArgb(255, 251, 237)}
                Case Theme.FlatDesign
                    colors = {Color.FromArgb(202, 45, 36), Color.FromArgb(168, 61, 70), Color.FromArgb(46, 68, 94), Color.FromArgb(22, 72, 89), Color.FromArgb(0, 67, 87)}
                Case Theme.BeachTime
                    colors = {Color.FromArgb(150, 206, 180), Color.FromArgb(255, 238, 173), Color.FromArgb(255, 111, 105), Color.FromArgb(255, 204, 92), Color.FromArgb(170, 216, 176)}
            End Select

            Return colors
        End Function

        ''' <summary>
        ''' Random Get Theme by Known Themes
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetRandomTheme() As Color()
            Dim rnd1 As New Random
            Dim values() As Integer = CType([Enum].GetValues(GetType(Theme)), Integer())
            Dim selected As Integer = rnd1.Next(0, values.Length)
            Return GetTheme(CType(selected, Theme))
        End Function

        'Public Shared Function GetBestLuminosityContrast(ByVal source As Color) As Color
        '    Dim R, G, B As Double
        '    Dim light1 As Double = 0.2126 * Math.Pow(source.R / 255, 2.2) + 0.7152 * Math.Pow(source.G / 255, 2.2) + 0.0722 * Math.Pow(source.B / 255, 2.2)
        '    Dim light2 As Double = 0
        '    If light1 > 0.5 Then
        '        light2 = (light1 - 0.2) / 5
        '    Else
        '        light2 = 5 * light1 + 0.2
        '    End If

        '    If light2 < 0.0722 Then
        '        R = 0
        '        G = 0
        '        B = 1 - ((0.0722 - light2) / 0.0722)
        '        B = (Math.Log(B) / Math.Log(2.2)) * 255
        '    ElseIf light2 < 0.2126 Then
        '        G = 0
        '        B = ((0.2126 - light2) / (0.2126 - 0.0722))
        '        R = 1 - B

        '        B = (Math.Log(B) / Math.Log(2.2)) * 255
        '        R = (Math.Log(R) / Math.Log(2.2)) * 255
        '    ElseIf light2 < 0.7152 Then
        '        B = 0
        '        R = ((0.7152 - light2) / (0.7152 - 0.2126))
        '        G = 1 - R

        '        R = (Math.Log(R) / Math.Log(2.2)) * 255
        '        G = (Math.Log(G) / Math.Log(2.2)) * 255
        '    ElseIf light2 >= 0.7152 Then
        '        G = 255
        '        If light2 - 0.7152 > 0.2126 Then
        '            R = 255
        '            B = light2 - 0.7152 - 0.2126
        '            B = (Math.Log(B) / Math.Log(2.2)) * 255
        '        Else
        '            R = 0
        '            B = light2 - 0.7152
        '            B = (Math.Log(B) / Math.Log(2.2)) * 255
        '        End If
        '    End If

        '    Return Color.FromArgb(R, G, B)
        'End Function

        Public Shared Function TryRandomContrastColor(ByVal source As Color) As Color
            Dim matchCount As Integer
            Dim maxMatchCount As Integer = 0
            Dim contrastColor As Color
            Dim chooseColor As Color = Color.Black

            For i As Integer = 0 To 1000
                Thread.Sleep(10)
                matchCount = 0
                contrastColor = RandomColor()
                matchCount = matchCount + IIf(LuminosityDistance(source, contrastColor) > 5, 1, 0)
                matchCount = matchCount + IIf(ColorDifference(source, contrastColor) > 500, 1, 0)
                matchCount = matchCount + IIf(BrightnessDifference(source, contrastColor) > 125, 1, 0)
                matchCount = matchCount + IIf(PythagoreanDistance(source, contrastColor) > 250, 1, 0)

                If maxMatchCount < matchCount Then
                    chooseColor = contrastColor
                    maxMatchCount = matchCount

                    If maxMatchCount = 4 Then
                        Return chooseColor
                    End If
                End If
            Next

            Return chooseColor
        End Function

        ''' <summary>
        '''  Luminosity Contrast distance calculator
        ''' </summary>
        ''' <param name="color1"></param>
        ''' <param name="color2"></param>
        ''' <returns>bigger than "5" for best readability</returns>
        ''' reference: https://www.splitbrain.org/blog/2008-09/18-calculating_color_contrast_with_php
        Private Shared Function LuminosityDistance(ByVal color1 As Color, ByVal color2 As Color) As Double
            Dim light1 = 0.2126 * Math.Pow(color1.R / 255, 2.2) + 0.7152 * Math.Pow(color1.G / 255, 2.2) + 0.0722 * Math.Pow(color1.B / 255, 2.2)
            Dim light2 = 0.2126 * Math.Pow(color2.R / 255, 2.2) + 0.7152 * Math.Pow(color2.G / 255, 2.2) + 0.0722 * Math.Pow(color2.B / 255, 2.2)
            If light1 > light2 Then
                Return (light1 + 0.05) / (light2 + 0.05)
            Else
                Return (light2 + 0.05) / (light1 + 0.05)
            End If
        End Function

        ''' <summary>
        ''' Color difference calculate
        ''' </summary>
        ''' <param name="color1"></param>
        ''' <param name="color2"></param>
        ''' <returns>higher than "500" is recommended for good readability</returns>
        ''' reference: https://www.splitbrain.org/blog/2008-09/18-calculating_color_contrast_with_php
        Private Shared Function ColorDifference(ByVal color1 As Color, ByVal color2 As Color) As Double
            Return CType(Math.Max(color1.R, color2.R), Double) - CType(Math.Min(color1.R, color2.R), Double) + CType(Math.Max(color1.G, color2.G), Double) - CType(Math.Min(color1.G, color2.G), Double) + CType(Math.Max(color1.B, color2.B), Double) - CType(Math.Min(color1.B, color2.B), Double)
        End Function

        ''' <summary>
        ''' Brightness difference calculate
        ''' </summary>
        ''' <param name="color1"></param>
        ''' <param name="color2"></param>
        ''' <returns>more than "125" is recommended</returns>
        ''' reference: https://www.splitbrain.org/blog/2008-09/18-calculating_color_contrast_with_php
        Private Shared Function BrightnessDifference(ByVal color1 As Color, ByVal color2 As Color) As Double
            Dim br1 = (299 + color1.R + 587 * color1.G + 114 * color1.B) / 1000
            Dim br2 = (299 + color2.R + 587 * color2.G + 114 * color2.B) / 1000

            Return Math.Abs(br1 - br2)
        End Function

        ''' <summary>
        ''' Pythagorean Distance
        ''' </summary>
        ''' <param name="color1"></param>
        ''' <param name="color2"></param>
        ''' <returns>threshold of "250"</returns>
        '''  reference: https://www.splitbrain.org/blog/2008-09/18-calculating_color_contrast_with_php
        Private Shared Function PythagoreanDistance(ByVal color1 As Color, ByVal color2 As Color) As Double
            Dim rd = CType(color1.R, Double) - CType(color2.R, Double)
            Dim gd = CType(color1.G, Double) - CType(color2.G, Double)
            Dim bd = CType(color1.B, Double) - CType(color2.B, Double)

            Return Math.Sqrt(rd * rd + gd * gd + bd * bd)
        End Function
    End Class

End Namespace