Imports System.Windows.Forms
Imports System.Drawing
Imports SportingAppFW.Extensions
Imports SportingAppFW.Data.Common
Imports SportingAppFW.SaSystem.SaDelegates

Namespace SaWindows.Forms
    ''' <summary>
    ''' Summary description for MultiColumnComboBox.
    ''' </summary>

    Public Class SaMultiComboBox
        Inherits TextBox

        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private _components As System.ComponentModel.Container = Nothing

        Private _selectedRow As DataRow = Nothing

        Private _displayMember As String = ""

        Private _displayValue As String = ""

        Private _valueMember As String = ""

        Private _value As String = ""

        Private _dataTable As DataTable = Nothing

        Private _columnsToDisplay As List(Of SaColumnNameToDisplay)

        Public Event AfterSelectEvent As AfterSelectEventHandler
        Public Event BeforeDropDownEvent As BeforeDropDownEventHandler

        Private DropDownButton As Button

        Private _MAX_DROP_DOWN_ROWS As Integer = 20

        Private _maxDropDownRows As Integer = _MAX_DROP_DOWN_ROWS
        Public Property MaxDropDownRows As Integer
            Get
                Return _maxDropDownRows
            End Get
            Set(value As Integer)
                If value < _MAX_DROP_DOWN_ROWS Then
                    _maxDropDownRows = _MAX_DROP_DOWN_ROWS
                Else
                    _maxDropDownRows = value
                End If
            End Set
        End Property

        Public Sub New(ByVal container As System.ComponentModel.IContainer)
            MyBase.New
            ''' <summary>
            ''' Required for Windows.Forms Class Composition Designer support
            ''' </summary>
            container.Add(Me)
            Me.InitializeComponent()

            '
            ' TODO: Add any constructor code after InitializeComponent call
            '
        End Sub

        Public Sub New()
            MyBase.New
            Me.InitializeComponent()
        End Sub
#Region "Component Designer generated code"

        Private Sub InitializeComponent()
            Me._components = New System.ComponentModel.Container

            DropDownButton = New Button()
            DropDownButton.Parent = Me
            DropDownButton.Dock = DockStyle.Right
            DropDownButton.Image = My.Resources.if_Dropdown_70492
            DropDownButton.Cursor = Cursors.Hand
            DropDownButton.Width = 25
            DropDownButton.FlatStyle = FlatStyle.Flat
            DropDownButton.FlatAppearance.BorderSize = 0
            DropDownButton.TabStop = False

            AddHandler DropDownButton.Click, AddressOf OnDropDown
        End Sub

#End Region

        Private Function FirstSimilar(ByVal s As String) As DataRow
            If _dataTable IsNot Nothing Then
                Dim drs As DataRow() = _dataTable.Select(String.Format("{0} LIKE {1}", DisplayMember, (s + "%").QuotedStr()))
                If drs.Count > 0 Then
                    Return drs(0)
                End If
            End If
            Return Nothing
        End Function

        Private Function FirstEqual(ByVal s As String) As DataRow
            If _dataTable IsNot Nothing Then
                Dim drs As DataRow() = _dataTable.Select(String.Format("{0} = {1}", DisplayMember, s.QuotedStr()))
                If drs.Count > 0 Then
                    Return drs(0)
                End If
            End If
            Return Nothing
        End Function

        'Auto Match Words
        Protected Overrides Sub OnKeyPress(e As KeyPressEventArgs)
            If Not Char.IsControl(e.KeyChar) Then
                Dim dr As DataRow = FirstSimilar((Me.Text.Substring(0, Me.SelectionStart) + e.KeyChar + "%"))
                If dr IsNot Nothing Then
                    Me._selectedRow = dr
                    Me._displayValue = dr(Me._displayMember).ToString
                    Me._value = dr(Me.ValueMember).ToString
                    Dim startpos As Integer = Me.SelectionStart
                    Me.Text = Me._displayValue

                    Me.Select(startpos + 1, Me.TextLength)
                    e.Handled = True
                End If
            End If

            MyBase.OnKeyPress(e)
        End Sub

        Private Function IsMatch() As Boolean
            Dim dr As DataRow = FirstEqual(Me.Text)
            Return dr IsNot Nothing
        End Function

        Protected Overrides Sub OnLostFocus(e As EventArgs)
            If Not DropDownButton.Focused And Not Me.Text.IsEmpty Then
                If Not IsMatch() Then
                    Me.Focus()
                    Exit Sub
                End If
            End If
            MyBase.OnLostFocus(e)
        End Sub

        Private Sub OnDropDown(sender As Object, e As EventArgs)
            RaiseEvent BeforeDropDownEvent(sender)

            Dim parent As Form = Me.FindForm
            'Dim parent As Control = Me.Parent
            If (Me._dataTable IsNot Nothing) Then
                If Not IsMatch() Then
                    Me.Text = String.Empty
                End If

                If (Me._selectedRow IsNot Nothing) Then
                    Try
                        _selectedRow(Me._displayMember) = Me._displayValue
                        _selectedRow(Me.ValueMember) = Me._value
                    Catch e2 As Exception
                        MessageBox.Show(e2.Message, "Error")
                    End Try

                End If

                Dim popup As SaMultiComboPopup = New SaMultiComboPopup(Me._dataTable, Me._selectedRow, Me._columnsToDisplay)
                AddHandler popup.AfterRowSelectEvent, AddressOf Me.MultiColumnComboBox_AfterSelectEvent
                popup.MaxDropDownRows = MaxDropDownRows
                '    Dim locationOnScreen As Point = parent.PointToScreen(New Point((Me.Parent.Left + Me.Left), (parent.Top + Me.Bottom)))
                'Dim locationOnScreen As Point = parent.PointToClient(Me.Parent.PointToScreen(New Point(Me.Left, (parent.Top + Me.Height))))
                Dim locationOnScreen As Point = PointToClient(PointToScreen(MousePosition))
                popup.Location = locationOnScreen

                popup.Show()

                RaiseEvent AfterSelectEvent()
            End If
        End Sub

        Private Sub MultiColumnComboBox_AfterSelectEvent(ByVal sender As Object, ByVal drow As DataRow)
            Try
                If (drow IsNot Nothing) Then
                    Me._selectedRow = drow
                    Me._displayValue = drow(Me._displayMember).ToString
                    Me._value = drow(Me.ValueMember).ToString
                    Me.Text = Me._displayValue
                End If

            Catch exp As Exception
                MessageBox.Show(Me, exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End Sub


        Public ReadOnly Property SelectedRow As DataRow
            Get
                Return Me._selectedRow
            End Get
        End Property

        Public ReadOnly Property DisplayValue As String
            Get
                Return Me._displayValue
            End Get
        End Property

        Public Property DisplayMember As String
            Get
                Return Me._displayMember
            End Get
            Set
                Me._displayMember = Value
            End Set
        End Property

        Public Property ValueMember As String
            Get
                Return Me._valueMember
            End Get
            Set(value As String)
                Me._valueMember = value
            End Set
        End Property

        Public Property Value As String
            Get
                Return Me._value
            End Get
            Set(value As String)
                Dim dr As DataRow = FirstEqual(value)
                If dr IsNot Nothing Then
                    Me._selectedRow = dr
                    Me._displayValue = dr(Me._displayMember).ToString
                    Me._value = dr(Me.ValueMember).ToString
                    Me.Text = Me._displayValue
                Else
                    Me._selectedRow = Nothing
                    Me._displayValue = String.Empty
                    Me._value = String.Empty
                    Me.Text = Me.DisplayValue
                End If
            End Set
        End Property

        Public Property DataSource As DataTable
            Get
                Return Me._dataTable
            End Get
            Set
                Me._dataTable = Value
                If (Me._dataTable Is Nothing) Then
                    Return
                End If

                Me._selectedRow = Me._dataTable.NewRow

                'If Not _displayMember.IsEmpty Then
                '    _selectedRow(Me._displayMember) = Me._displayValue
                'End If

                'If Not ValueMember.IsEmpty Then
                '    _selectedRow(Me.ValueMember) = Me._value
                'End If

                'Me.Text = Me._displayValue
            End Set
        End Property


        Public Property ColumnsToDisplay As List(Of SaColumnNameToDisplay)
            Get
                Return Me._columnsToDisplay
            End Get
            Set(value As List(Of SaColumnNameToDisplay))
                Me._columnsToDisplay = value
            End Set
        End Property

    End Class
End Namespace