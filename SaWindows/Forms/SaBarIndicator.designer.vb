Namespace SaWindows.Forms
    <CompilerServices.DesignerGenerated()>
    Partial Class SaBarIndicator
        Inherits System.Windows.Forms.UserControl

        'UserControl 覆寫 Dispose 以清除元件清單。
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        '為 Windows Form 設計工具的必要項
        Private components As System.ComponentModel.IContainer

        '注意: 以下為 Windows Form 設計工具所需的程序
        '可以使用 Windows Form 設計工具進行修改。
        '請不要使用程式碼編輯器進行修改。
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Me.PanelIndicator = New System.Windows.Forms.Panel()
            Me.TBDefineValue = New System.Windows.Forms.TextBox()
            Me.CMSDelItem = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.TSMIDelete = New System.Windows.Forms.ToolStripMenuItem()
            Me.NTBRangeValue = New SportingAppFW.SaWindows.Forms.SaNumericTextBox()
            Me.CMSDelItem.SuspendLayout()
            Me.SuspendLayout()
            '
            'PanelIndicator
            '
            Me.PanelIndicator.BackColor = System.Drawing.Color.Red
            Me.PanelIndicator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.PanelIndicator.ContextMenuStrip = Me.CMSDelItem
            Me.PanelIndicator.Location = New System.Drawing.Point(23, 24)
            Me.PanelIndicator.Name = "PanelIndicator"
            Me.PanelIndicator.Size = New System.Drawing.Size(6, 40)
            Me.PanelIndicator.TabIndex = 0
            '
            'TBDefineValue
            '
            Me.TBDefineValue.BackColor = System.Drawing.Color.Khaki
            Me.TBDefineValue.Location = New System.Drawing.Point(0, 66)
            Me.TBDefineValue.Name = "TBDefineValue"
            Me.TBDefineValue.Size = New System.Drawing.Size(52, 22)
            Me.TBDefineValue.TabIndex = 2
            Me.TBDefineValue.TabStop = False
            Me.TBDefineValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
            '
            'CMSDelItem
            '
            Me.CMSDelItem.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TSMIDelete})
            Me.CMSDelItem.Name = "CMSDelItem"
            Me.CMSDelItem.Size = New System.Drawing.Size(101, 26)
            '
            'TSMIDelete
            '
            Me.TSMIDelete.Name = "TSMIDelete"
            Me.TSMIDelete.Size = New System.Drawing.Size(100, 22)
            Me.TSMIDelete.Text = "移除"
            '
            'NTBRangeValue
            '
            Me.NTBRangeValue.AllowSpace = False
            Me.NTBRangeValue.Location = New System.Drawing.Point(0, 0)
            Me.NTBRangeValue.Name = "NTBRangeValue"
            Me.NTBRangeValue.Size = New System.Drawing.Size(52, 22)
            Me.NTBRangeValue.TabIndex = 1
            Me.NTBRangeValue.TabStop = False
            Me.NTBRangeValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
            '
            'SaBarIndicator
            '
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
            Me.Controls.Add(Me.TBDefineValue)
            Me.Controls.Add(Me.NTBRangeValue)
            Me.Controls.Add(Me.PanelIndicator)
            Me.Name = "SaBarIndicator"
            Me.Size = New System.Drawing.Size(51, 87)
            Me.CMSDelItem.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents PanelIndicator As System.Windows.Forms.Panel
        Friend WithEvents NTBRangeValue As SaNumericTextBox
        Friend WithEvents TBDefineValue As System.Windows.Forms.TextBox
        Friend WithEvents CMSDelItem As Windows.Forms.ContextMenuStrip
        Friend WithEvents TSMIDelete As Windows.Forms.ToolStripMenuItem
    End Class

End Namespace