Imports System.Drawing
Imports System.Windows.Forms
Imports SportingAppFW.Data.Common
Imports SportingAppFW.SaSystem.SaDelegates

Namespace Components.WinForm

    ''' <summary>
    ''' Summary description for MultiColumnComboPopup.
    ''' </summary>
    Public Class SaMultiComboPopup
        Inherits Form

        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.Container = Nothing

        Private _selectedRow As DataRow = Nothing

        Private _InputRows() As DataRow = Nothing

        Private dataTable As DataTable = Nothing

        Private mCols As Integer = 0

        Private mRows As Integer = 0

        Private columnsToDisplay As List(Of SaColumnNameToDisplay) = Nothing

        Private _maxDropDownRows As Integer
        Public Property MaxDropDownRows As Integer
            Get
                Return _maxDropDownRows
            End Get
            Set(value As Integer)
                _maxDropDownRows = value
                SetHeight()
            End Set
        End Property

        Public Event AfterRowSelectEvent As AfterRowSelectEventHandler

        Public Sub New()
            MyBase.New
            Me.InitializeComponent()
            Me.mCols = 4
            Me.mRows = 0
            Me.InitializeGridProperties()
        End Sub

        Public Sub New(ByVal dtable As DataTable, ByRef selRow As DataRow, ByVal colsToDisplay As List(Of SaColumnNameToDisplay))
            MyBase.New
            Me.InitializeComponent()
            Me.dataTable = dtable
            Me._selectedRow = selRow

            Me.columnsToDisplay = colsToDisplay
            If (Not (Me.columnsToDisplay) Is Nothing) Then
                Me.SetDataTable(Me.dataTable, Me.columnsToDisplay)
            Else
                Me.SetDataTable(Me.dataTable)
            End If

            SetSelected()
        End Sub

        Private Sub SetSelected()

            For Each item As ListViewItem In lstvMyView.Items
                If item.Text = SelectedRow.Item(0).ToString() Then
                    item.Selected = True
                    Exit For
                End If
            Next
        End Sub

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If (Not (Me.components) Is Nothing) Then
                    Me.components.Dispose()
                End If

            End If

            MyBase.Dispose(disposing)
        End Sub
#Region "Windows Form Designer generated code"

        Private Sub InitializeComponent()
            Me.lstvMyView = New System.Windows.Forms.ListView()
            Me.SuspendLayout()
            '
            'lstvMyView
            '
            Me.lstvMyView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.lstvMyView.Dock = System.Windows.Forms.DockStyle.Fill
            Me.lstvMyView.FullRowSelect = True
            Me.lstvMyView.GridLines = True
            Me.lstvMyView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
            Me.lstvMyView.Location = New System.Drawing.Point(0, 0)
            Me.lstvMyView.MultiSelect = False
            Me.lstvMyView.Name = "lstvMyView"
            Me.lstvMyView.Size = New System.Drawing.Size(317, 88)
            Me.lstvMyView.TabIndex = 1
            Me.lstvMyView.UseCompatibleStateImageBehavior = False
            Me.lstvMyView.View = System.Windows.Forms.View.Details
            '
            'SaMultiComboPopup
            '
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 15)
            Me.ClientSize = New System.Drawing.Size(317, 88)
            Me.ControlBox = False
            Me.Controls.Add(Me.lstvMyView)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
            Me.KeyPreview = True
            Me.Name = "SaMultiComboPopup"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
            Me.ResumeLayout(False)

        End Sub
#End Region
#Region "Functions"

        Private Sub InitializeGridProperties()
            Me.lstvMyView.Items.Clear()
            Me.lstvMyView.Columns.Clear()
            'Add Data Headers
            Dim i As Integer = 0
            For Each col As SaColumnNameToDisplay In columnsToDisplay
                If i >= Me.mCols Then
                    Exit For
                End If

                Me.lstvMyView.Columns.Add(col.DisplayText, -1, HorizontalAlignment.Left)
                i = (i + 1)
            Next

            'Add Empty Values
            i = 0
            Do While (i < Me.mRows)
                Dim item As ListViewItem = New ListViewItem("")
                Me.lstvMyView.Items.Add(item)
                Dim j As Integer = 0
                Do While (j < Me.mCols)
                    item.SubItems.Add(" ")
                    j = (j + 1)
                Loop

                i = (i + 1)
            Loop
        End Sub


        Public Sub SetCellValue(ByVal Row As Integer, ByVal Column As Integer, ByVal ItemValue As String)
            If Me.IsValidRowAndColumn(Row, Column) Then
                If (Column = 0) Then
                    Me.lstvMyView.Items(Row).Text = ItemValue
                Else
                    Me.lstvMyView.Items(Row).SubItems(Column).Text = ItemValue
                End If

            End If

        End Sub

        Public Function GetCellValue(ByVal Row As Integer, ByVal Column As Integer) As String
            If Me.IsValidRowAndColumn(Row, Column) Then
                If (Column = 0) Then
                    Return Me.lstvMyView.Items(Row).Text
                Else
                    Return Me.lstvMyView.Items(Row).SubItems(Column).Text
                End If

            Else
                Return Nothing
            End If

        End Function

        Public Overloads Sub SetFullRow(ByVal Row As Integer, ByVal ItemValues As List(Of String))
            If Me.IsValidRow(Row) Then
                If (ItemValues.Count > 0) Then
                    Dim i As Integer = 0
                    Do While (i < ItemValues.Count)
                        Me.SetCellValue(Row, i, ItemValues(i))
                        i = (i + 1)
                    Loop

                Else
                    MessageBox.Show(Me, "SetFullRow: Empty Values Sent", "Grid Error")
                End If

            End If

        End Sub

        Public Overloads Sub SetFullRow(ByVal Row As Integer, ByVal drow As DataRow)
            If Me.IsValidRow(Row) Then
                If (Me.mCols > 0) Then
                    Dim i As Integer = 0
                    Do While (i < Me.mCols)
                        Me.SetCellValue(Row, i, CType(drow(i), String))
                        i = (i + 1)
                    Loop

                Else
                    MessageBox.Show(Me, "SetFullRow: Empty Values Sent", "Grid Error")
                End If

            End If

        End Sub

        Public Sub SetFullColumn(ByVal Column As Integer, ByVal ItemValues As List(Of String))
            If Me.IsValidColumn(Column) Then
                If (ItemValues.Count > 0) Then
                    Dim i As Integer = 0
                    Do While (i < Me.lstvMyView.Items.Count)
                        Me.SetCellValue(i, Column, ItemValues(i))
                        i = (i + 1)
                    Loop

                Else
                    MessageBox.Show(Me, "SetFullColumn: Empty Values Sent", "Grid Error")
                End If

            End If

        End Sub

        Public Function GetFullRow(ByVal Row As Integer) As List(Of String)
            Dim ItemValues As List(Of String) = New List(Of String)()
            If Me.IsValidRow(Row) Then
                Dim i As Integer = 0
                Do While (i < Me.lstvMyView.Items.Count)
                    ItemValues.Add(Me.GetCellValue(Row, i))
                    i = (i + 1)
                Loop

                Return ItemValues
            Else
                Return Nothing
            End If

        End Function

        Public Function GetFullColumn(ByVal Column As Integer) As List(Of String)
            Dim ItemValues As List(Of String) = New List(Of String)()
            If Me.IsValidColumn(Column) Then
                Dim i As Integer = 0
                Do While (i < Me.lstvMyView.Items.Count)
                    ItemValues.Add(Me.GetCellValue(i, Column))
                    i = (i + 1)
                Loop

                Return ItemValues
            Else
                Return Nothing
            End If

        End Function

        Public Sub AddItems(ByVal ItemValues As List(Of List(Of String)))
            Me.lstvMyView.BeginUpdate()
            If (ItemValues.Count > 0) Then
                Me.mRows = ItemValues.Count
                Me.mCols = ItemValues(0).Count
                Me.InitializeGridProperties()
                Dim i As Integer = 0
                Do While (i < Me.mRows)
                    Me.SetFullRow(i, ItemValues(i))
                    i = (i + 1)
                Loop

                Me.lstvMyView.EndUpdate()
            Else
                MessageBox.Show(Me, "AddItems: Empty Values Sent", "Grid Error")
            End If
        End Sub

        Private Sub SetHeight(ByVal rows As Integer)
            If lstvMyView.Items.Count > 0 Then
                Dim height As Integer = lstvMyView.GetItemRect(0).Height * (rows + 2)
                Dim max_height As Integer = lstvMyView.GetItemRect(0).Height * (MaxDropDownRows + 2)
                Me.Height = Math.Min(max_height, height)
            End If
        End Sub

        Public Sub SetHeight()
            SetHeight(lstvMyView.Items.Count)
        End Sub

        Public Function GetItems() As List(Of List(Of String))
            Dim ItemValues As List(Of List(Of String)) = New List(Of List(Of String))()
            Dim i As Integer = 0
            Do While (i < Me.lstvMyView.Items.Count)
                ItemValues.Add(Me.GetFullRow(i))
                i = (i + 1)
            Loop

            Return ItemValues
        End Function

        'Public Sub SetColumnHeaderNames(ByVal ColumnNames As List(Of String))
        '    Dim i As Integer = 0
        '    Do While (i < ColumnNames.Count)
        '        If (i >= Me.lstvMyView.Columns.Count) Then
        '            Me.lstvMyView.Columns.Add(ColumnNames(i), 25, HorizontalAlignment.Center)
        '            Me.lstvMyView.Columns(i).Width = (ColumnNames(i).Length * CType(Font.SizeInPoints, Integer))
        '        Else
        '            Me.lstvMyView.Columns(i).Text = ColumnNames(i)
        '            Me.lstvMyView.Columns(i).Width = (ColumnNames(i).Length * CType(Font.SizeInPoints, Integer))
        '        End If

        '        i = (i + 1)
        '    Loop

        'End Sub

        Public Sub SetColumnWidths(ByVal Widths() As Integer)
            Dim i As Integer = 0
            Do While (i < Widths.Length)
                If (i >= Me.lstvMyView.Columns.Count) Then
                    Me.lstvMyView.Columns.Add("", (Widths(i) * CType(Me.lstvMyView.Font.SizeInPoints, Integer)), HorizontalAlignment.Center)
                Else
                    Me.lstvMyView.Columns(i).Width = (Widths(i) * CType(Font.SizeInPoints, Integer))
                End If

                i = (i + 1)
            Loop

        End Sub

        Public Sub SetColumnWidth(ByVal Column As Integer, ByVal ColWidth As Integer)
            If Me.IsValidColumn(Column) Then
                Me.lstvMyView.Columns(Column).Width = (ColWidth * CType(Font.SizeInPoints, Integer))
            End If

        End Sub

        Public Overloads Sub SetDataTable(ByVal dtable As DataTable)
            Dim dcc As DataColumnCollection = dtable.Columns
            columnsToDisplay = New List(Of SaColumnNameToDisplay)()
            'Dim drc As DataRowCollection = dtable.Rows
            'Dim data As List(Of List(Of String)) = New List(Of List(Of String))()

            'Dim item As List(Of String)
            'Dim i As Integer = 0

            For Each col As DataColumn In dcc
                columnsToDisplay.Add(New SaColumnNameToDisplay(col.ToString(), col.ToString()))
            Next

            SetDataTable(dtable, columnsToDisplay)
            'For Each dr As DataRow In drc
            '    i = 0
            '    item = New List(Of String)()
            '    For Each col As DataColumn In dcc
            '        item.Add(col.ToString())
            '    Next

            '    data.Add(item)
            'Next

            'Me.AddItems(data)
        End Sub

        Public Overloads Sub SetDataTable(ByVal dtable As DataTable, ByVal colsToDisplay As List(Of SaColumnNameToDisplay))
            Dim drc As DataRowCollection = dtable.Rows
            Dim data As List(Of List(Of String)) = New List(Of List(Of String))()
            Dim item As List(Of String)
            Dim i As Integer = 0

            For Each dr As DataRow In drc
                i = 0
                item = New List(Of String)()
                For Each col As SaColumnNameToDisplay In colsToDisplay
                    item.Add(dr(col.Name).ToString)
                Next

                data.Add(item)
            Next
            Me.AddItems(data)
        End Sub

        Private Function IsValidRowAndColumn(ByVal Row As Integer, ByVal Column As Integer) As Boolean
            If ((Column < 0) OrElse (Row < 0)) Then
                MessageBox.Show(Me, "Row or Column Cannot be Negative!", "Grid Error")
                Return False
            ElseIf (Row > Me.lstvMyView.Items.Count) Then
                MessageBox.Show(Me, "SetCellValue: Row Out Of Range", "Grid Error")
                Return False
            ElseIf (Column > Me.lstvMyView.Columns.Count) Then
                MessageBox.Show(Me, ("SetCellValue: Column Out Of Range " + Column.ToString), "Grid Error")
                Return False
            Else
                Return True
            End If

        End Function

        Private Function IsValidRow(ByVal Row As Integer) As Boolean
            If (Row < 0) Then
                MessageBox.Show(Me, "Row Cannot be Negative!", "Grid Error")
                Return False
            ElseIf (Row > Me.lstvMyView.Items.Count) Then
                MessageBox.Show(Me, "Row Out Of Range", "Grid Error")
                Return False
            Else
                Return True
            End If

        End Function

        Private Function IsValidColumn(ByVal Column As Integer) As Boolean
            If (Column < 0) Then
                MessageBox.Show(Me, "Column Cannot be Negative!", "Grid Error")
                Return False
            ElseIf (Column > Me.lstvMyView.Columns.Count) Then
                MessageBox.Show(Me, ("Column Out Of Range " + Column.ToString), "Grid Error")
                Return False
            Else
                Return True
            End If

        End Function
#End Region
        Private Sub lstvMyView_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles lstvMyView.KeyDown
            If (e.KeyData = Keys.Enter) Then
                If (Me.lstvMyView.SelectedItems.Count > 0) Then
                    Me._selectedRow = Me.dataTable.Rows(Me.lstvMyView.SelectedIndices(0))
                    RaiseEvent AfterRowSelectEvent(Me, Me.SelectedRow)
                End If

                Me.Close()
            End If

        End Sub

        Private Sub lstvMyView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstvMyView.Click
            If (Me.lstvMyView.SelectedItems.Count > 0) Then
                Me._selectedRow = Me.dataTable.Rows(Me.lstvMyView.SelectedIndices(0))
                RaiseEvent AfterRowSelectEvent(Me, Me.SelectedRow)
            End If

            Me.Close()
        End Sub
        Private Sub gridValue_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            '
        End Sub


        Private Sub MultiColumnComboPopup_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Deactivate
            Me.Close()
        End Sub

        Private Sub MultiColumnComboPopup_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Leave
            Me.Close()
        End Sub

        Private Sub MultiColumnComboPopup_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
            If (e.KeyData = Keys.Escape) Then
                Me.Close()
            End If

        End Sub

        Private WithEvents lstvMyView As ListView

        Public WriteOnly Property Table As DataTable
            Set
                Me.dataTable = Value
                If (Me.dataTable Is Nothing) Then
                    Return
                End If

                Me.SetDataTable(Me.dataTable)
            End Set
        End Property

        Public ReadOnly Property SelectedRow As DataRow
            Get
                Return Me._selectedRow
            End Get
        End Property

        Public ReadOnly Property Cols As Integer
            Get
                Me.mCols = Me.lstvMyView.Columns.Count
                Return Me.mCols
            End Get
        End Property

        Public ReadOnly Property Rows As Integer
            Get
                Me.mRows = Me.lstvMyView.Items.Count
                Return Me.mRows
            End Get
        End Property


    End Class


End Namespace