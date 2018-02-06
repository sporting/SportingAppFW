'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Drawing
Imports System.Windows.Forms

Namespace SaWindows.Forms
    Partial Public Class SaTabControlSU
        Inherits TabControl

        Private _tabHeaderVisible As Boolean = True
        Private _tabAddButton As Boolean = False

        Public Property TabHeaderVisible As Boolean
            Get
                Return _tabHeaderVisible
            End Get
            Set(value As Boolean)
                If value Then
                    MyBase.Appearance = TabAppearance.Normal
                    MyBase.ItemSize = New Size(58, 18)
                    MyBase.SizeMode = TabSizeMode.Normal
                Else
                    MyBase.Appearance = TabAppearance.FlatButtons
                    MyBase.ItemSize = New Size(0, 1)
                    MyBase.SizeMode = TabSizeMode.Fixed
                End If

                _tabHeaderVisible = value
            End Set
        End Property

        Public Overloads Property Appearance As TabAppearance
            Get
                Return MyBase.Appearance
            End Get
            Set(value As TabAppearance)
                If TabHeaderVisible Then
                    MyBase.Appearance = value
                End If
            End Set
        End Property

        Public Overloads Property SizeMode As TabSizeMode
            Get
                Return MyBase.SizeMode
            End Get
            Set(value As TabSizeMode)
                If TabHeaderVisible Then
                    MyBase.SizeMode = value
                End If
            End Set
        End Property

        Public Overloads Property ItemSize As Size
            Get
                Return MyBase.ItemSize
            End Get
            Set(value As Size)
                If TabHeaderVisible Then
                    MyBase.ItemSize = value
                End If
            End Set
        End Property

        'Protected Overrides Sub OnDrawItem(e As DrawItemEventArgs)
        '    MyBase.OnDrawItem(e)

        '    Dim tabTextArea As RectangleF = Rectangle.Empty

        '    For nIndex As Integer = 0 To Me.TabCount - 1
        '        tabTextArea = Me.GetTabRect(nIndex)
        '        If (nIndex <> Me.SelectedIndex) Then
        '            Using bmp As Bitmap = New Bitmap(My.Resources.if_Gnome_Window_Close_32_55183)
        '                e.Graphics.DrawImage(bmp, tabTextArea.X + tabTextArea.Width - 16, 5, 13, 13)
        '            End Using
        '        Else
        '            Dim br As LinearGradientBrush = New LinearGradientBrush(tabTextArea, SystemColors.ControlLightLight, SystemColors.Control, LinearGradientMode.Vertical)
        '            e.Graphics.FillRectangle(br, tabTextArea)

        '            Using bmp As Bitmap = New Bitmap(My.Resources.if_Gnome_Window_Close_32_55183)
        '                e.Graphics.DrawImage(bmp, tabTextArea.X + tabTextArea.Width - 16, 5, 13, 13)
        '            End Using

        '            br.Dispose()
        '        End If

        '        Dim str As String = Me.TabPages(nIndex).Text
        '        Dim strFormat As StringFormat = New StringFormat()
        '        strFormat.Alignment = StringAlignment.Center
        '        Using brush As SolidBrush = New SolidBrush(Me.TabPages(nIndex).ForeColor)
        '            e.Graphics.DrawString(str, Me.Font, brush, tabTextArea, strFormat)
        '        End Using
        '    Next
        'End Sub


        'Private _addTabPage As TabPage


        'Public Property TabAddButtonVisible As Boolean
        '    Get
        '        Return _tabAddButton
        '    End Get
        '    Set(value As Boolean)
        '        _tabAddButton = value
        '        If value Then
        '            _addTabPage = New TabPage("+")
        '            Me.TabPages.Add(_addTabPage)
        '            AddHandler Me.TabIndexChanged, AddressOf AddNewTab
        '        Else
        '            If _addTabPage IsNot Nothing Then
        '                Me.TabPages.Remove(_addTabPage)
        '                _addTabPage = Nothing
        '            End If
        '        End If
        '    End Set
        'End Property

        'Public Delegate Sub BeforeAddNewTabEventHandler(ByVal sender As Object, ByVal e As EventArgs)
        'Public Delegate Sub AfterAddNewTabEventHandler(ByVal sender As Object, ByVal e As EventArgs)

        'Public Event BeforeTabAddButtonClick As BeforeAddNewTabEventHandler
        'Public Event AfterTabAddButtonClick As AfterAddNewTabEventHandler

        'Private Sub AddNewTab(ByVal sender As Object, ByVal e As EventArgs)
        '    If _tabAddButton Then
        '        If Me.TabCount > 0 Then
        '            RaiseEvent BeforeTabAddButtonClick(sender, e)

        '            Me.TabPages.Insert(Me.TabCount - 1, "NewTab")

        '            RaiseEvent AfterTabAddButtonClick(sender, e)
        '        End If
        '    End If
        'End Sub
    End Class

End Namespace