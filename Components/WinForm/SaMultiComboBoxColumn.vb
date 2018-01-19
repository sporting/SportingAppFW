Imports System.Runtime.CompilerServices
Imports System.Windows.Forms
Imports SportingAppFW.Data.Common
Imports SportingAppFW.Extensions

Namespace Components.WinForm
    Public Class SaMultiComboBoxColumn
        Inherits DataGridViewColumn

        Private _dt As DataTable
        Private _member As String
        Private _coltodisplay As List(Of SaColumnNameToDisplay)
        Private _passMember As List(Of SaPassColumnToTarget)
        Private _filterWhere As String

        Public Property FilterWhere As String
            Get
                Return _filterWhere
            End Get
            Set(value As String)
                _filterWhere = value
                _cmbCell.FilterWhere = _filterWhere
            End Set
        End Property

        Public Property DataSource As DataTable
            Get
                Return _dt
            End Get
            Set(value As DataTable)
                _dt = value
                _cmbCell.DataSource = _dt
            End Set
        End Property

        Public Property Member As String
            Get
                Return _member
            End Get
            Set(value As String)
                _member = value
                _cmbCell.Member = _member
            End Set
        End Property
        Public Property ColToDisplay As List(Of SaColumnNameToDisplay)
            Get
                Return _coltodisplay
            End Get
            Set(value As List(Of SaColumnNameToDisplay))
                _coltodisplay = value
                _cmbCell.ColToDisplay = _coltodisplay
            End Set
        End Property

        Public Property PassMemeber As List(Of SaPassColumnToTarget)
            Get
                Return PassMember
            End Get
            Set(value As List(Of SaPassColumnToTarget))
                PassMember = value
                _cmbCell.PassMember = PassMember
            End Set
        End Property

        Private _cmbCell As SaMultiComboBoxCell

        Public Sub New()
            MyBase.New()
            _cmbCell = New SaMultiComboBoxCell()
            CellTemplate = _cmbCell
        End Sub

        Public Overrides Property CellTemplate() As DataGridViewCell
            Get
                Return MyBase.CellTemplate
            End Get
            Set(ByVal value As DataGridViewCell)

                ' Ensure that the cell used for the template is a CalendarCell.
                If (value IsNot Nothing) AndAlso
                Not value.GetType().IsAssignableFrom(GetType(SaMultiComboBoxCell)) _
                Then
                    Throw New InvalidCastException("Must be a SaMultiComboBoxCell")
                End If
                MyBase.CellTemplate = value

            End Set
        End Property

        Public Property PassMember As List(Of SaPassColumnToTarget)
            Get
                Return _passMember
            End Get
            Set(value As List(Of SaPassColumnToTarget))
                _passMember = value
            End Set
        End Property
    End Class


    Public Class SaMultiComboBoxCell
        Inherits DataGridViewTextBoxCell

        Private _dt As DataTable
        Private _member As String
        Private _coltodisplay As List(Of SaColumnNameToDisplay)
        Private _passMember As List(Of SaPassColumnToTarget)

        Private _filterWhere As String

        Public Property FilterWhere As String
            Get
                Return _filterWhere
            End Get
            Set(value As String)
                _filterWhere = value
            End Set
        End Property
        Public Property DataSource As DataTable
            Get
                Return _dt
            End Get
            Set(value As DataTable)
                _dt = value
            End Set
        End Property

        Public Property Member As String
            Get
                Return _member
            End Get
            Set(value As String)
                _member = value
            End Set
        End Property
        Public Property ColToDisplay As List(Of SaColumnNameToDisplay)
            Get
                Return _coltodisplay
            End Get
            Set(value As List(Of SaColumnNameToDisplay))
                _coltodisplay = value
            End Set
        End Property

        Public Property PassMember As List(Of SaPassColumnToTarget)
            Get
                Return _passMember
            End Get
            Set(value As List(Of SaPassColumnToTarget))
                _passMember = value
            End Set
        End Property

        Public Sub New()

        End Sub

        Public Overrides Function Clone() As Object
            Dim objectValue As SaMultiComboBoxCell = TryCast(MyBase.Clone(), SaMultiComboBoxCell)
            objectValue.DataSource = RuntimeHelpers.GetObjectValue(Me.DataSource)
            objectValue.Member = Me.Member
            objectValue.ColToDisplay = Me.ColToDisplay
            objectValue.PassMember = Me.PassMember
            objectValue.FilterWhere = Me.FilterWhere
            Return objectValue
        End Function

        Public Overrides Sub InitializeEditingControl(ByVal rowIndex As Integer,
        ByVal initialFormattedValue As Object,
        ByVal dataGridViewCellStyle As DataGridViewCellStyle)

            ' Set the value of the editing control to the current cell value.
            MyBase.InitializeEditingControl(rowIndex, initialFormattedValue,
            dataGridViewCellStyle)

            Dim ctl As SaMultiComboBoxEditingControl =
            CType(DataGridView.EditingControl, SaMultiComboBoxEditingControl)

            ctl.DataSource = DataSource
            ctl.DisplayMember = Member
            ctl.ValueMember = Member
            ctl.ColumnsToDisplay = ColToDisplay
            ctl.PassMember = PassMember
            ctl.FilterWhere = FilterWhere

            ' Use the default row value when Value property is null.
            If (Me.RowIndex < 0 OrElse Me.Value Is Nothing OrElse IsDBNull(Me.Value)) Then
                ctl.Text = CType(Me.DefaultNewRowValue, String)
            Else
                ctl.Text = CType(Me.Value, String)
            End If
        End Sub

        Public Overrides ReadOnly Property EditType() As Type
            Get
                ' Return the type of the editing control that SaMultiComboBoxCell uses.
                Return GetType(SaMultiComboBoxEditingControl)
            End Get
        End Property

        Public Overrides ReadOnly Property ValueType() As Type
            Get
                ' Return the type of the value that SaMultiComboBoxCell contains.
                Return GetType(String)
            End Get
        End Property

        Public Overrides ReadOnly Property DefaultNewRowValue() As Object
            Get
                ' Use the current date and time as the default value.

                Return String.Empty
            End Get
        End Property

    End Class


    Class SaMultiComboBoxEditingControl
        Inherits SaMultiComboBox
        Implements IDataGridViewEditingControl

        Private dataGridViewControl As DataGridView
        Private valueIsChanged As Boolean = False
        Private rowIndexNum As Integer
        Private _filterWhere As String
        Public Property FilterWhere As String
            Get
                Return _filterWhere
            End Get
            Set(value As String)
                _filterWhere = value
            End Set
        End Property
        Private _passMember As List(Of SaPassColumnToTarget)

        Property PassMember As List(Of SaPassColumnToTarget)
            Get
                Return _passMember
            End Get
            Set(value As List(Of SaPassColumnToTarget))
                _passMember = value
            End Set
        End Property

        Public Sub New()
            AddHandler Me.BeforeDropDownEvent, AddressOf DataGridViewBeforeDropDown
        End Sub

        Private Sub DataGridViewBeforeDropDown(sender As Object)
            If Not FilterWhere.IsEmpty Then
                Dim setWhere As String = FilterWhere
                For Each col As DataGridViewColumn In dataGridViewControl.Columns
                    setWhere = setWhere.Replace("{" + col.Name + "}", dataGridViewControl.Rows(rowIndexNum).Cells(col.Name).Value)
                Next
                DataSource.DefaultView.RowFilter = setWhere
            End If
        End Sub

        Public Property EditingControlFormattedValue() As Object _
        Implements IDataGridViewEditingControl.EditingControlFormattedValue

            Get
                Return Me.Value
            End Get

            Set(ByVal value As Object)
                Try
                    Me.Value = value
                Catch
                    Me.Value = value
                End Try
            End Set

        End Property

        Public Function GetEditingControlFormattedValue(ByVal context _
        As DataGridViewDataErrorContexts) As Object _
        Implements IDataGridViewEditingControl.GetEditingControlFormattedValue

            Return Me.Value

        End Function

        Public Sub ApplyCellStyleToEditingControl(ByVal dataGridViewCellStyle As _
        DataGridViewCellStyle) _
        Implements IDataGridViewEditingControl.ApplyCellStyleToEditingControl

            Me.Font = dataGridViewCellStyle.Font
            Me.ForeColor = dataGridViewCellStyle.ForeColor
            Me.BackColor = dataGridViewCellStyle.BackColor

        End Sub

        Public Property EditingControlRowIndex() As Integer _
        Implements IDataGridViewEditingControl.EditingControlRowIndex

            Get
                Return rowIndexNum
            End Get
            Set(ByVal value As Integer)
                rowIndexNum = value
            End Set

        End Property

        Public Function EditingControlWantsInputKey(ByVal key As Keys,
        ByVal dataGridViewWantsInputKey As Boolean) As Boolean _
        Implements IDataGridViewEditingControl.EditingControlWantsInputKey

            ' Let the DateTimePicker handle the keys listed.
            Select Case key And Keys.KeyCode
                Case Keys.Left, Keys.Up, Keys.Down, Keys.Right,
                Keys.Home, Keys.End, Keys.PageDown, Keys.PageUp

                    Return True

                Case Else
                    Return Not dataGridViewWantsInputKey
            End Select

        End Function

        Public Sub PrepareEditingControlForEdit(ByVal selectAll As Boolean) _
        Implements IDataGridViewEditingControl.PrepareEditingControlForEdit

            ' No preparation needs to be done.

        End Sub

        Public ReadOnly Property RepositionEditingControlOnValueChange() _
        As Boolean Implements _
        IDataGridViewEditingControl.RepositionEditingControlOnValueChange

            Get
                Return False
            End Get

        End Property

        Public Property EditingControlDataGridView() As DataGridView _
        Implements IDataGridViewEditingControl.EditingControlDataGridView

            Get
                Return dataGridViewControl
            End Get
            Set(ByVal value As DataGridView)
                dataGridViewControl = value
            End Set

        End Property

        Public Property EditingControlValueChanged() As Boolean _
        Implements IDataGridViewEditingControl.EditingControlValueChanged

            Get
                Return valueIsChanged
            End Get
            Set(ByVal value As Boolean)
                valueIsChanged = value
            End Set

        End Property

        Public ReadOnly Property EditingControlCursor() As Cursor _
        Implements IDataGridViewEditingControl.EditingPanelCursor

            Get
                Return MyBase.Cursor
            End Get

        End Property

        Protected Overrides Sub OnTextChanged(e As EventArgs)
            ' Notify the DataGridView that the contents of the cell have changed.
            'If dataGridViewControl IsNot Nothing AndAlso PassMember IsNot Nothing Then
            '    For Each pass As SaPassColumnToTarget In PassMember
            '      dataGridViewControl.Rows.Item(EditingControlRowIndex).Cells(pass.TargetColumnName).Value = SelectedRow(pass.SourceColumnName)
            '    Next
            'End If

            '  If Not valueIsChanged Then
            valueIsChanged = True
                Me.EditingControlDataGridView.NotifyCurrentCellDirty(True)
                MyBase.OnTextChanged(e)
          '  End If
        End Sub

    End Class
End Namespace