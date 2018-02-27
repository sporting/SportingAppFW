' Author: Nick Shih
' 2018/2/27
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Namespace SaWindows.Forms
    Public Class SaMultiRangeBar
        Protected StraightPen As Pen

        Private _penWidth As Integer = 2

        Private _barColor As Color = Color.Red

        <Description("Set the indicator bar color")>
        Public Property BarColor() As Color
            Get
                Return _barColor
            End Get
            Set(ByVal value As Color)
                _barColor = value

                If DesignMode Then
                    For Each ind As SaBarIndicator In _indicators
                        ind.BarColor = value

                        Me.Invalidate()
                    Next
                End If
            End Set
        End Property

        Private _rangeColor As Color = SystemColors.Window

        <Description("Set the indicator input color")>
        Public Property BarRangeColor() As Color
            Get
                Return _rangeColor
            End Get
            Set(ByVal value As Color)
                _rangeColor = value

                If DesignMode Then
                    For Each ind As SaBarIndicator In _indicators
                        ind.RangeColor = value

                        Me.Invalidate()
                    Next
                End If
            End Set
        End Property

        Private _defineColor As Color = Color.Khaki

        <Description("Set the indicator self-define input color")>
        Public Property BarDefineColor() As Color
            Get
                Return _defineColor
            End Get
            Set(ByVal value As Color)
                _defineColor = value

                If DesignMode Then
                    For Each ind As SaBarIndicator In _indicators
                        ind.DefineColor = value

                        Me.Invalidate()
                    Next
                End If
            End Set
        End Property

        <Description("Get all the range values")>
        Public ReadOnly Property SaRangeValues() As List(Of SaRangeValue)
            Get
                Dim ls As List(Of SaRangeValue) = New List(Of SaRangeValue)()
                Dim a = From ind As SaBarIndicator In _indicators
                        Select ind
                        Order By ind.RangeValue

                Dim lastMin As Integer = Minimum
                For i As Integer = 0 To a.Count - 1
                    ls.Add(New SaRangeValue(lastMin, a(i).RangeValue, a(i).DefineValue))
                    lastMin = a(i).RangeValue
                Next

                Return ls
            End Get
        End Property

        Private _enableMultiRangeIndicator As Boolean = True

        <Description("Allow Multiple Indicators")>
        Public Property EnableMultiRangeIndicator() As Boolean
            Get
                Return _enableMultiRangeIndicator
            End Get
            Set(ByVal value As Boolean)
                _enableMultiRangeIndicator = value
            End Set
        End Property

        Public Property PenWidth() As Integer
            Get
                Return _penWidth
            End Get
            Set(ByVal value As Integer)
                _penWidth = value

                Me.Invalidate()
            End Set
        End Property

        Private _minimum As Integer = 0

        'Range Bar min value
        <Description("Set the indicator minimun value")>
        Public Property Minimum() As Integer
            Get
                Return _minimum
            End Get
            Set(ByVal value As Integer)
                _minimum = value

                Me.Invalidate()
            End Set
        End Property

        Private _maximum As Integer = 100

        'Range Bar max value
        <Description("Set the indicator maximun value")>
        Public Property Maximum() As Integer
            Get
                Return _maximum
            End Get
            Set(ByVal value As Integer)
                _maximum = value

                Me.Invalidate()
            End Set
        End Property


        Private _indicators As List(Of SaBarIndicator)

        <Description("Get all the indicators")>
        Public ReadOnly Property Indicators() As List(Of SaBarIndicator)
            Get
                Return _indicators
            End Get
        End Property


        Private ReadOnly Property LeftOffset() As Integer
            Get
                Dim w As Integer = Me.Width
                Dim minfw As Size = TextRenderer.MeasureText(Minimum.ToString(), Me.Font)
                Return minfw.Width
            End Get
        End Property

        Private ReadOnly Property RightOffset() As Integer
            Get
                Dim maxfw As Size = TextRenderer.MeasureText(Maximum.ToString(), Me.Font)
                Return maxfw.Width
            End Get
        End Property

        Private ReadOnly Property LineWidth() As Integer
            Get
                Return Me.Width - LeftOffset - RightOffset
            End Get
        End Property

        Sub New()
            ' 此為 Windows Form 設計工具所需的呼叫。
            InitializeComponent()

            ' 在 InitializeComponent() 呼叫之後加入任何初始設定。
            SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            SetStyle(ControlStyles.SupportsTransparentBackColor, True)
            Me.BackColor = Color.Transparent

            _indicators = New List(Of SaBarIndicator)()
            _indicators.Add(NewIndicator())

            AddHandler DoubleClick, AddressOf SaMultiRangeBarDoubleClick
            AddHandler Application.Idle, AddressOf SaMultiRangeBarOnIdle

            Me.Invalidate()
        End Sub

        Protected Sub SaMultiRangeBarDoubleClick(ByVal sender As Object, ByVal e As EventArgs)
            If EnableMultiRangeIndicator Then
                Dim ind As SaBarIndicator = NewIndicator()

                _indicators.Add(ind)

                IndicatorValueChanged(ind, New SaRangeValueChangeEventArgs(ind.RangeValue, ind.DefineValue))
            End If
        End Sub

        Protected Function NewIndicator(Optional ByVal val As Integer = -1) As SaBarIndicator
            Dim ind As SaBarIndicator = New SaBarIndicator()
            ind.RangeColor = BarRangeColor
            ind.DefineColor = BarDefineColor
            ind.BarColor = BarColor

            AddHandler ind.IndMouseDownHandler, AddressOf IndicatorMouseDown
            AddHandler ind.IndMouseMoveHandler, AddressOf IndicatorMouseMove
            AddHandler ind.IndMouseUpHandler, AddressOf IndicatorMouseUp
            AddHandler ind.IndRangeValueChangedHandler, AddressOf IndicatorValueChanged

            If val < 0 Then
                val = PointToClient(MousePosition).X * (Maximum - Minimum) / (Width - ind.IndicatorCenter.X)
            End If

            val = ValBetweenMinimumMaximum(val)

            ind.RangeValue = val

            ind.Parent = Me
            Me.Controls.Add(ind)

            Return ind
        End Function

        Private Function ValBetweenMinimumMaximum(ByVal val As Integer) As Integer
            val = Math.Max(val, Minimum)
            If val < Minimum Then
                val = Minimum
            ElseIf val > Maximum Then
                val = Maximum
            End If

            Return val
        End Function

        Protected Sub SaMultiRangeBarOnIdle(ByVal sender As Object, ByVal e As EventArgs)
            'Dim p As Point
            For Each ind As SaBarIndicator In _indicators
                'p = New Point(((ind.RangeValue - _minimum) / (_maximum - _minimum)) * LineWidth + LeftOffset, Me.Height / 2)

                ind.Left = ((ind.RangeValue - _minimum) / (_maximum - _minimum)) * LineWidth + LeftOffset - ind.IndicatorCenter.X
                ind.Top = Me.Height / 2 - ind.IndicatorCenter.Y
            Next
        End Sub

        Dim _startDrag As Boolean = False
        Protected Sub IndicatorMouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
            If e.Button = Windows.Forms.MouseButtons.Left Then
                _startDrag = True
            End If
        End Sub

        Protected Sub IndicatorMouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
            If _startDrag Then
                Dim pointedValue As Integer = e.X * (Maximum - Minimum) / (Width - CType(sender, SaBarIndicator).IndicatorCenter.X)
                Dim val As Integer = CType(sender, SaBarIndicator).RangeValue + pointedValue

                val = ValBetweenMinimumMaximum(val)
                CType(sender, SaBarIndicator).RangeValue = val
            End If
        End Sub

        Protected Sub IndicatorMouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
            _startDrag = False
        End Sub

        Public Delegate Sub ValueInvalidate(ByVal sender As Object, ByVal e As SaRangeValueChangeEventArgs)

        <Description("Trigger when indicator value change")>
        Public Event ValueInvalidateHandler As ValueInvalidate

        Protected Sub IndicatorValueChanged(ByVal sender As Object, ByVal e As SaRangeValueChangeEventArgs)
            RaiseEvent ValueInvalidateHandler(sender, e)
        End Sub

        Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(e)

            '請在此處加入您自訂的繪製程式碼
            Dim w As Integer = Me.Width
            Dim h As Integer = Me.Height
            Dim maxfw As Size = TextRenderer.MeasureText(Maximum.ToString(), Me.Font)
            Dim minfw As Size = TextRenderer.MeasureText(Minimum.ToString(), Me.Font)

            StraightPen = New Pen(Color.DarkGray, _penWidth)
            StraightPen.Alignment = PenAlignment.Center

            e.Graphics.DrawLine(StraightPen, LeftOffset, h \ 2, w - RightOffset, h \ 2)

            e.Graphics.DrawString(Minimum.ToString(), Me.Font, Brushes.Black, LeftOffset - minfw.Width, (h \ 2) - (minfw.Height \ 2))
            e.Graphics.DrawString(Maximum.ToString(), Me.Font, Brushes.Black, w - RightOffset, (h \ 2) - (maxfw.Height \ 2))
        End Sub

    End Class
End Namespace