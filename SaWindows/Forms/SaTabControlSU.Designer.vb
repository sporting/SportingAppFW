Imports System.Windows.Forms

Namespace SaWindows.Forms
    Partial Public Class SaTabControlSU
        Inherits TabControl

        <System.Diagnostics.DebuggerNonUserCode()>
        Public Sub New(ByVal container As System.ComponentModel.IContainer)
            MyClass.New()

            'Windows.Forms 類別組合設計工具支援所需的程式碼
            If (container IsNot Nothing) Then
                container.Add(Me)
            End If

        End Sub

        <System.Diagnostics.DebuggerNonUserCode()>
        Public Sub New()
            MyBase.New()

            '此為元件設計工具所需的呼叫。
            InitializeComponent()

        End Sub

        'Component 覆寫 Dispose 以清除元件清單。
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

        '為元件設計工具的必要項
        Private components As System.ComponentModel.IContainer

        '注意: 以下為元件設計工具所需的程序
        '您可以使用元件設計工具進行修改。
        '請勿使用程式碼編輯器進行修改。
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            components = New System.ComponentModel.Container()

        End Sub

    End Class

End Namespace