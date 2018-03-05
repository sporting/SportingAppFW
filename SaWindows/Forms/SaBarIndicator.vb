' Author: Nick Shih
' 2018/2/27
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace SaWindows.Forms
    Public Class SaBarIndicator
        <Description("Publish Indicator Bar Color")>
        Public Property BarColor() As Color
            Get
                Return PanelIndicator.BackColor
            End Get
            Set(ByVal value As Color)
                PanelIndicator.BackColor = value
            End Set
        End Property

        <Description("Publish RangeInput Color")>
        Public Property RangeColor() As Color
            Get
                Return NTBRangeValue.BackColor
            End Get
            Set(ByVal value As Color)
                NTBRangeValue.BackColor = value
            End Set
        End Property

        <Description("Publish RangeInput Value")>
        Public Property RangeValue() As Integer
            Get
                Dim val As Integer
                If Integer.TryParse(NTBRangeValue.Text.Replace(",", ""), val) Then
                    Return val
                End If

                Return 0
            End Get
            Set(ByVal value As Integer)
                If IsCurrency Then
                    Dim s As Integer = NTBRangeValue.SelectionStart
                    Dim oriPos As Integer = NTBRangeValue.Text.Substring(0, s).Count(Function(c) c = ",")
                    Dim newPos As Integer = value.ToString("N0").Substring(0, s).Count(Function(c) c = ",")
                    Dim cartPosOffSet As Integer = newPos - oriPos
                    NTBRangeValue.Text = value.ToString("N0")
                    NTBRangeValue.SelectionStart = s + cartPosOffSet
                Else
                    NTBRangeValue.Text = value.ToString()
                End If
            End Set
        End Property

        <Description("Publish DefineInput Color")>
        Public Property DefineColor() As Color
            Get
                Return TBDefineValue.BackColor
            End Get
            Set(ByVal value As Color)
                TBDefineValue.BackColor = value
            End Set
        End Property

        <Description("Publish DefineInput Value")>
        Public Property DefineValue() As String
            Get
                Return TBDefineValue.Text
            End Get
            Set(ByVal value As String)
                TBDefineValue.Text = value
            End Set
        End Property

        <Description("Publish Indicator Bar Center Point")>
        Public ReadOnly Property IndicatorCenter() As Point
            Get
                Return New Point((PanelIndicator.Width \ 2) + PanelIndicator.Left, (PanelIndicator.Height \ 2) + PanelIndicator.Top)
            End Get
        End Property


        Public Delegate Sub IndMouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)

        <Description("Publish Bar MouseDown Event")>
        Public Event IndMouseDownHandler As IndMouseDown

        Public Delegate Sub IndMouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)

        <Description("Publish Bar MouseUp Event")>
        Public Event IndMouseUpHandler As IndMouseUp

        Public Delegate Sub IndMouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)

        <Description("Publish Bar MouseMove Event")>
        Public Event IndMouseMoveHandler As IndMouseDown

        Public Delegate Sub IndRangeValueChanged(ByVal sender As Object, ByVal e As EventArgs)

        <Description("Publish Bar Value Change Event")>
        Public Event IndRangeValueChangedHandler As IndRangeValueChanged


        Public Delegate Sub IndDelete(ByVal sender As Object, ByVal e As EventArgs)
        <Description("Publish Context Menu Click Event")>
        Public Event IndDeleteHandler As IndDelete

        Public Property IsCurrency() As Boolean
            Get
                Return NTBRangeValue.IsCurrency
            End Get
            Set(ByVal value As Boolean)
                NTBRangeValue.IsCurrency = value
            End Set
        End Property

        Sub New()

            ' 此為 Windows Form 設計工具所需的呼叫。
            InitializeComponent()

            ' 在 InitializeComponent() 呼叫之後加入任何初始設定。

            SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.BackColor = Color.Transparent

            AddHandler PanelIndicator.MouseDown, AddressOf PanelIndMouseDown
            AddHandler PanelIndicator.MouseUp, AddressOf PanelIndMouseUp
            AddHandler PanelIndicator.MouseMove, AddressOf PanelIndMouseMove
            AddHandler NTBRangeValue.TextChanged, AddressOf ValueChange
            AddHandler TBDefineValue.TextChanged, AddressOf ValueChange

            AddHandler TSMIDelete.Click, AddressOf DeleteIndicator
        End Sub

        Protected Sub ValueChange(ByVal sender As Object, ByVal e As EventArgs)
            RaiseEvent IndRangeValueChangedHandler(Me, New SaRangeValueChangeEventArgs(RangeValue, DefineValue))
        End Sub

        Protected Sub PanelIndMouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
            RaiseEvent IndMouseDownHandler(Me, e)
        End Sub

        Protected Sub PanelIndMouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
            RaiseEvent IndMouseUpHandler(Me, e)
        End Sub

        Protected Sub PanelIndMouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
            RaiseEvent IndMouseMoveHandler(Me, e)
        End Sub

        Private Sub DeleteIndicator(ByVal sender As System.Object, ByVal e As System.EventArgs)
            RaiseEvent IndDeleteHandler(Me, e)
        End Sub
    End Class

End Namespace