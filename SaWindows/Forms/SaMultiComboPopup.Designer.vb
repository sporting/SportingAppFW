<CompilerServices.DesignerGenerated()>
Partial Class SaMultiComboPopup
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
    '請勿使用程式碼編輯器進行修改。
    <System.Diagnostics.DebuggerStepThrough()>
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
        Me.lstvMyView.Size = New System.Drawing.Size(303, 132)
        Me.lstvMyView.TabIndex = 1
        Me.lstvMyView.UseCompatibleStateImageBehavior = False
        Me.lstvMyView.View = System.Windows.Forms.View.Details
        '
        'MultiColumnComboPopup
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lstvMyView)
        Me.Name = "MultiColumnComboPopup"
        Me.Size = New System.Drawing.Size(303, 132)
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents lstvMyView As Windows.Forms.ListView
End Class

