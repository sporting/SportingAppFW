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
                If Integer.TryParse(NTBRangeValue.Text, val) Then
                    Return val
                End If

                Return 0
            End Get
            Set(ByVal value As Integer)
                NTBRangeValue.Text = value.ToString()
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


    End Class

End Namespace