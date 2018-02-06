
Imports System.Drawing
Imports System.Windows.Forms

Namespace SaWindows.Forms
    Partial Public Class SaIntelliTextBox
        Inherits TextBox

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
            _phrasesListBox = New ListBox()
            _phrasesListBox.Visible = False
            '_phrasesListBox.Parent = _mainForm

            _phrasesListBox.Top = 0
            _phrasesListBox.Left = 0
            _phrasesListBox.Font = New Font("Consolas", 11, FontStyle.Regular) 'New Font("FixedsysTTF", 11, FontStyle.Regular)
            _phrasesListBox.Cursor = Cursors.Hand
            _phrasesListBox.MaximumSize = New Size(300, 300)
            _phrasesListBox.HorizontalScrollbar = False
            AddHandler _phrasesListBox.MouseClick, AddressOf PhraseListBoxMouseClick

            _phrasesListBoxFont = TextRenderer.MeasureText("A", _phrasesListBox.Font)
            _phrasesListBox.ItemHeight = _phrasesListBoxFont.Height

            Dim s As Size = TextRenderer.MeasureText("A", Font)
            _fontWidth = s.Width
            _fontHeight = s.Height

            _phrases = New SaPhrases()
        End Sub
    End Class

End Namespace