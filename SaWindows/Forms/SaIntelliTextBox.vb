'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Drawing
Imports System.Windows.Forms
Imports SportingAppFW.Extensions

Namespace SaWindows.Forms

    Partial Public Class SaIntelliTextBox
        Inherits TextBox

        Private Enum TextChangeMode
            CM_GENERAL = 1
            CM_REPLACE = 2
        End Enum

        Private _intellisenseMode As Boolean = False
        Private _phrasesListBox As ListBox
        Private _phrasesListBoxFont As Size

        Private _words As SaWordsPosition
        Private _phrases As SaPhrases
        Private _lastText As String
        Private _fontHeight As Integer
        Private _fontWidth As Integer
        Private _textChangeMode As TextChangeMode = TextChangeMode.CM_GENERAL

        Private _backColor As Color
        Property FocusColorEnabled As Boolean = False
        Property FocusColor As Color = Color.Khaki

        'Property TabIsKey As Boolean = False

        ReadOnly Property PhrasesDictionary() As SaPhrases
            Get
                Return _phrases
            End Get
        End Property

        ReadOnly Property Words() As SaWordsPosition
            Get
                Return _words
            End Get
        End Property

        Public Property IntellisenseMode As Boolean
            Get
                Return _intellisenseMode
            End Get
            Set(value As Boolean)
                _intellisenseMode = value
            End Set
        End Property

        Protected Property IntellisenseShowing As Boolean
            Get
                Return _phrasesListBox.Visible
            End Get
            Set(value As Boolean)
                _phrasesListBox.Visible = value

                If _phrasesListBox.Visible Then
                    _phrasesListBox.Parent = TopForm
                    If TopForm IsNot Nothing Then
                        TopForm.Controls.SetChildIndex(_phrasesListBox, 0)
                    End If
                End If
            End Set
        End Property

        Sub New(ByVal intellisenseMode As Boolean)
            MyBase.New()

            _intellisenseMode = intellisenseMode
        End Sub

        Protected Overrides Sub OnGotFocus(e As EventArgs)
            MyBase.OnGotFocus(e)
            If FocusColorEnabled Then
                _backColor = BackColor
                BackColor = FocusColor
            End If
        End Sub

        Protected Overrides Sub OnLostFocus(e As EventArgs)
            MyBase.OnLostFocus(e)
            If FocusColorEnabled Then
                BackColor = _backColor
            End If

            If IntellisenseShowing Then
                IntellisenseShowing = False
            End If
        End Sub

        Private _mainForm As Form
        Protected Overrides Sub OnFontChanged(e As EventArgs)
            Dim s As Size = TextRenderer.MeasureText("A", Font)
            _fontWidth = s.Width
            _fontHeight = s.Height

            MyBase.OnFontChanged(e)
        End Sub

        Protected Overrides Sub OnTextChanged(e As EventArgs)
            If DesignMode Then
                Exit Sub
            End If
            IntellisenseShowing = False

            If Not _lastText.IsEmpty() Then
                If Math.Abs(_lastText.Length - Text.Length) > 1 Then
                    _lastText = Text
                    Exit Sub
                End If
            End If

            _lastText = Text

            If (SelectionLength = 0) Then
                If _intellisenseMode Then
                    ExtractWords(Text, SelectionStart)
                    If _textChangeMode = TextChangeMode.CM_GENERAL Then
                        If _phrases IsNot Nothing And _words IsNot Nothing Then
                            Dim filterPhrases As List(Of String) = _phrases.KeyStartsWith(_words.Words)
                            If filterPhrases IsNot Nothing AndAlso filterPhrases.Count > 0 Then
                                _phrasesListBox.DataSource = filterPhrases

                                Dim p As Point = getCaretPos(Me)

                                _phrasesListBox.Top = p.Y + _fontHeight
                                _phrasesListBox.Left = p.X + _fontWidth / 2
                                Dim maxlen As Integer = filterPhrases.Select(Of Integer)(Function(s) s.Length).OrderBy(Of Integer)(Function(s) s).Last()
                                _phrasesListBox.Height = (_phrasesListBox.ItemHeight * Math.Max(_phrasesListBox.Items.Count, 1)) + 4
                                _phrasesListBox.Width = maxlen * (_phrasesListBoxFont.Width - 4)
                                _phrasesListBox.HorizontalScrollbar = filterPhrases.Count > 1
                                IntellisenseShowing = True
                            End If
                        End If
                    End If
                End If
            End If
            MyBase.OnTextChanged(e)
        End Sub

        Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
            Try
                If IntellisenseShowing Then
                    If e.KeyCode = Keys.Up Then
                        _phrasesListBox.SelectedIndex = IIf(_phrasesListBox.SelectedIndex > 0, _phrasesListBox.SelectedIndex - 1, 0)
                        e.SuppressKeyPress = False
                        e.Handled = True
                    ElseIf e.KeyCode = Keys.PageUp Then
                        _phrasesListBox.SelectedIndex = IIf(_phrasesListBox.SelectedIndex > 9, _phrasesListBox.SelectedIndex - 10, 0)
                        e.SuppressKeyPress = False
                        e.Handled = True
                    ElseIf e.KeyData = Keys.Down Then
                        _phrasesListBox.SelectedIndex = IIf(_phrasesListBox.SelectedIndex < _phrasesListBox.Items.Count - 1, _phrasesListBox.SelectedIndex + 1, _phrasesListBox.Items.Count - 1)
                        e.SuppressKeyPress = False
                        e.Handled = True
                    ElseIf e.KeyCode = Keys.PageDown Then
                        _phrasesListBox.SelectedIndex = IIf(_phrasesListBox.SelectedIndex < _phrasesListBox.Items.Count - 9, _phrasesListBox.SelectedIndex + 9, _phrasesListBox.Items.Count - 1)
                        e.SuppressKeyPress = False
                        e.Handled = True
                    ElseIf e.KeyCode = Keys.Left OrElse e.KeyCode = Keys.Right Then
                        IntellisenseShowing = False
                        e.SuppressKeyPress = True
                        e.Handled = False
                    ElseIf e.KeyCode = Keys.Home Then
                        _phrasesListBox.SelectedIndex = 0
                        e.SuppressKeyPress = False
                        e.Handled = True
                    ElseIf e.KeyCode = Keys.End Then
                        _phrasesListBox.SelectedIndex = _phrasesListBox.Items.Count - 1
                        e.SuppressKeyPress = False
                        e.Handled = True
                    ElseIf e.KeyData = Keys.Escape Then
                        IntellisenseShowing = False
                        e.SuppressKeyPress = False
                        e.Handled = True
                    ElseIf e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Tab Then
                        ReplacePhrase()
                        e.SuppressKeyPress = True
                        e.Handled = True
                    End If
                End If
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try

            MyBase.OnKeyDown(e)
        End Sub

        'Protected Overrides Function IsInputKey(ByVal keyData As Keys) As Boolean
        '    If TabIsKey Then
        '        If keyData = Keys.Tab Then
        '            Return True
        '        End If
        '    End If
        '    Return MyBase.IsInputKey(keyData)
        'End Function

        Private Sub PhraseListBoxMouseClick(sender As Object, e As MouseEventArgs)
            ReplacePhrase()
        End Sub

        Private Sub ReplacePhrase()
            Dim text As String = _phrasesListBox.SelectedItem.ToString() '+ " "
            Dim newText As String = Me.Text.Substring(0, _words.StartIdx) + text + Me.Text.Substring(_words.StartIdx + _words.Length)
            Dim newCaret As Integer = (newText.Substring(0, _words.StartIdx) + text).Length

            Me.Text = newText
            SelectionStart = newCaret
            IntellisenseShowing = False

            ScrollToCaret()

            _textChangeMode = TextChangeMode.CM_REPLACE
            Try
                OnTextChanged(Nothing)
            Finally
                _textChangeMode = TextChangeMode.CM_GENERAL
            End Try

            If CanSelect Then
                [Select]()
            End If
        End Sub

        '    Protected Overrides Sub OnLostFocus(e As EventArgs)
        '        IntellisenseShowing = False
        '        MyBase.OnLostFocus(e)
        '    End Sub

        '    Protected Overrides Sub OnGotFocus(e As EventArgs)
        '        MyBase.OnGotFocus(e)
        '    End Sub

        Private ReadOnly Property TopForm() As Form
            Get
                If _mainForm IsNot Nothing Then
                    Return _mainForm
                ElseIf Application.OpenForms.Count > 0 Then
                    _mainForm = Application.OpenForms(0)
                    Return _mainForm
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Private Function getCaretPos(ByVal tb As TextBox) As Point
            If TopForm Is Nothing Then
                Return New Point(0, 0)
            End If

            Dim p As Point = New Point()
            p = tb.GetPositionFromCharIndex(tb.SelectionStart - 1)

            Return TopForm.PointToClient(PointToScreen(p))
        End Function


        Private Sub ExtractWords(text As String, selectionStart As Integer)
            _words = Nothing

            If selectionStart <= 0 Then
                Exit Sub
            End If

            Dim headidx As Integer = selectionStart
            Dim c As Char
            While headidx > 0
                c = text.Substring(headidx - 1, 1)

                If c.IsEmpty() OrElse c.Equals("."c) OrElse c.Equals(","c) OrElse Char.IsControl(c) Then
                    Exit While
                End If
                headidx -= 1
            End While

            Dim t As String = text.Substring(headidx, selectionStart - headidx)
            _words = New SaWordsPosition(t, headidx, selectionStart - headidx)
        End Sub

        Public Overloads Function Search(ByVal keyword As String) As Boolean
            Return Search(keyword, 0, False)
        End Function

        Public Overloads Function Search(ByVal keyword As String, ByVal startpos As Integer) As Boolean
            Return Search(keyword, startpos, False)
        End Function

        Private _lastSearchIdx As Integer = -1
        Public Overloads Function Search(ByVal keyword As String, ByVal startpos As Integer, ByVal restart As Boolean) As Boolean
            _lastSearchIdx = Text.IndexOf(keyword, startpos, StringComparison.CurrentCultureIgnoreCase)
            If _lastSearchIdx >= 0 Then
                [Select](_lastSearchIdx, keyword.Length)
                Me.ScrollToCaret()
            ElseIf restart Then
                Search(keyword, 0, False)
            End If
            Return _lastSearchIdx >= 0
        End Function

        Public Sub ReplaceAll(ByVal keyword As String, ByVal toWord As String)
            If keyword = toWord Then
                Exit Sub
            End If

            While Search(keyword, 0, True)
                SelectedText = toWord
            End While
        End Sub

    End Class

End Namespace