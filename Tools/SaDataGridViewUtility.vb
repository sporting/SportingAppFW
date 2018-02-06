
Imports System.Drawing
Imports System.Reflection
Imports System.Windows.Forms
Imports SportingAppFW.Data.Common
Imports SportingAppFW.SaWindows.Forms

Namespace Tools
    Public Module SaDataGridViewUtility
        ''' <summary>
        ''' 依據 SaFields 設定的屬性設定 DataGridView
        ''' </summary>
        ''' <param name="gv"></param>
        ''' <param name="saf"></param>
        ''' <param name="dbconn"></param>
        Public Sub SetGridViewHeaderByAttribute(ByRef gv As DataGridView, ByVal saf As SaFields, ByVal dbconn As IDbConnection)
            gv.AutoGenerateColumns = False
            gv.Columns.Clear()

            For Each attri As KeyValuePair(Of PropertyInfo, SaUIFieldsAttribute) In saf.SaUIFieldsAttributes
                If attri.Value.CustomControl = CustomControlEnum.TextBox Then
                    Dim textCln As DataGridViewTextBoxColumn = New DataGridViewTextBoxColumn()
                    textCln.HeaderText = attri.Value.Caption
                    textCln.ReadOnly = attri.Value.ReadMode
                    If textCln.ReadOnly Then
                        textCln.DefaultCellStyle.BackColor = SystemColors.ControlDark
                    End If
                    textCln.Resizable = DataGridViewTriState.True
                    textCln.DataPropertyName = attri.Key.Name
                    textCln.Name = attri.Key.Name

                    textCln.Visible = attri.Value.FieldVisible

                    gv.Columns.Add(textCln)
                ElseIf attri.Value.CustomControl = CustomControlEnum.DateTimePicker Then
                    Dim calCln As SaCalendarColumn = New SaCalendarColumn()
                    calCln.HeaderText = attri.Value.Caption
                    calCln.ReadOnly = attri.Value.ReadMode
                    If calCln.ReadOnly Then
                        calCln.DefaultCellStyle.BackColor = SystemColors.ControlDark
                    End If
                    calCln.Resizable = DataGridViewTriState.True
                    calCln.DataPropertyName = attri.Key.Name
                    calCln.Name = attri.Key.Name
                    calCln.MinimumWidth = 100

                    calCln.Visible = attri.Value.FieldVisible

                    gv.Columns.Add(calCln)
                ElseIf attri.Value.CustomControl = CustomControlEnum.CheckBox Then
                    Dim checkCln As DataGridViewCheckBoxColumn = New DataGridViewCheckBoxColumn()
                    checkCln.HeaderText = attri.Value.Caption
                    checkCln.ReadOnly = attri.Value.ReadMode
                    If checkCln.ReadOnly Then
                        checkCln.DefaultCellStyle.BackColor = SystemColors.ControlDark
                    End If
                    checkCln.Resizable = DataGridViewTriState.True
                    checkCln.DataPropertyName = attri.Key.Name
                    checkCln.Name = attri.Key.Name
                    checkCln.TrueValue = "Y"
                    checkCln.FalseValue = "N"
                    checkCln.Visible = attri.Value.FieldVisible

                    gv.Columns.Add(checkCln)
                ElseIf attri.Value.CustomControl = CustomControlEnum.MultiComboBox Then
                    Dim saTab As SaTable = Activator.CreateInstance(attri.Value.ComboBoxSetting.DataSourceClass, dbconn)
                    Dim dt As DataTable = saTab.SelectAll()

                    Dim cmbCln As SaMultiComboBoxColumn = New SaMultiComboBoxColumn()
                    cmbCln.DataSource = dt
                    cmbCln.Member = attri.Value.ComboBoxSetting.ValueMember
                    cmbCln.ColToDisplay = attri.Value.ComboBoxSetting.ColumnsToDisplay
                    cmbCln.PassMemeber = attri.Value.ComboBoxSetting.PassToTargets

                    cmbCln.HeaderText = attri.Value.Caption
                    cmbCln.ReadOnly = attri.Value.ReadMode
                    If cmbCln.ReadOnly Then
                        cmbCln.DefaultCellStyle.BackColor = SystemColors.ControlDark
                    End If
                    cmbCln.Resizable = DataGridViewTriState.True
                    cmbCln.DataPropertyName = attri.Key.Name
                    cmbCln.Name = attri.Key.Name
                    cmbCln.MinimumWidth = 100

                    cmbCln.Visible = attri.Value.FieldVisible

                    gv.Columns.Add(cmbCln)
                End If
            Next
        End Sub
    End Module

End Namespace