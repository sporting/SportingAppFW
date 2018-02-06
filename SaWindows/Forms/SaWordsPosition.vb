'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Namespace SaWindows.Forms
    Public Class SaWordsPosition
        Dim _text As String
        Dim _startIdx As Integer
        Dim _length As Integer

        Public ReadOnly Property Words() As String
            Get
                Return _text
            End Get
        End Property

        Public ReadOnly Property StartIdx() As Integer
            Get
                Return _startIdx
            End Get
        End Property
        Public ReadOnly Property Length() As Integer
            Get
                Return _length
            End Get
        End Property
        Sub New(ByVal text As String, ByVal startIdx As Integer, ByVal length As Integer)
            _text = text
            _startIdx = startIdx
            _length = length
        End Sub
    End Class

End Namespace