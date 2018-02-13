'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************

Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports Microsoft.Win32.SafeHandles

Namespace SaWindows.Forms
    Public Enum RearrangeVerticalDirection
        TopDown 'Top to Down
        DownTop 'Down to Top
    End Enum

    Public Enum RearrangeHorizontalDirection
        LeftRight 'Left to Right
        RightLeft 'Right to Left
    End Enum

    Public Enum RearrangeDirectionPriority
        Vertical    'Vertical Direction First, Horizontal Direction Last
        Horizontal  'Horizontal Direction First, Vertical Direction Last
    End Enum

    Public Class SaTabIndexRearrange
        Implements IDisposable
        Private disposed As Boolean = False
        Private handle As SafeHandle = New SafeFileHandle(IntPtr.Zero, True)

        Private _container As Control
        Private _force As Boolean = True
        Private _directionPriority As RearrangeDirectionPriority = RearrangeDirectionPriority.Vertical
        Private _directionHorizontal As RearrangeHorizontalDirection = RearrangeHorizontalDirection.LeftRight
        Private _directionVertical As RearrangeVerticalDirection = RearrangeVerticalDirection.TopDown

        Public Property Container As Control
            Get
                Return _container
            End Get
            Set(value As Control)
                _container = value
            End Set
        End Property

        Public Property Force As Boolean
            Get
                Return _force
            End Get
            Set(value As Boolean)
                _force = value
            End Set
        End Property

        Public Property DirectionPriority As RearrangeDirectionPriority
            Get
                Return _directionPriority
            End Get
            Set(value As RearrangeDirectionPriority)
                _directionPriority = value
            End Set
        End Property

        Public Property DirectionHorizontal As RearrangeHorizontalDirection
            Get
                Return _directionHorizontal
            End Get
            Set(value As RearrangeHorizontalDirection)
                _directionHorizontal = value
            End Set
        End Property

        Public Property DirectionVertical As RearrangeVerticalDirection
            Get
                Return _directionVertical
            End Get
            Set(value As RearrangeVerticalDirection)
                _directionVertical = value
            End Set
        End Property
        Sub New()

        End Sub

        Sub New(container As Control)
            _container = container
        End Sub

        ''' <summary>
        ''' sub container's controls force arrange
        ''' </summary>
        ''' <param name="container"></param>
        ''' <param name="force">sub container include</param>
        Sub New(container As Control, force As Boolean)
            MyClass.New(container)
            _force = force
        End Sub

        Sub New(container As Control, force As Boolean, priority As RearrangeDirectionPriority, verticalDir As RearrangeVerticalDirection, horizontalDir As RearrangeHorizontalDirection)
            MyClass.New(container)
            _force = force
            _directionPriority = priority
            _directionHorizontal = horizontalDir
            _directionVertical = verticalDir
        End Sub

        Public Overloads Sub Doit()
            If _container Is Nothing Then
                Exit Sub
            End If

            DoIt(_container)
        End Sub

        Private Overloads Sub DoIt(container As Control)
            Dim idx As Integer = 0
            Dim orderControls
            If _directionPriority.Equals(RearrangeDirectionPriority.Horizontal) Then
                If _directionHorizontal.Equals(RearrangeHorizontalDirection.LeftRight) Then
                    If _directionVertical.Equals(RearrangeVerticalDirection.TopDown) Then
                        orderControls = From con As Control In container.Controls
                                        Select con
                                        Order By con.Left, con.Top
                    Else
                        orderControls = From con As Control In container.Controls
                                        Select con
                                        Order By con.Left, con.Top Descending
                    End If
                Else
                    If _directionVertical.Equals(RearrangeVerticalDirection.TopDown) Then
                        orderControls = From con As Control In container.Controls
                                        Select con
                                        Order By con.Left Descending, con.Top
                    Else
                        orderControls = From con As Control In container.Controls
                                        Select con
                                        Order By con.Left Descending, con.Top Descending
                    End If
                End If
            Else
                If _directionVertical.Equals(RearrangeVerticalDirection.TopDown) Then
                    If _directionHorizontal.Equals(RearrangeHorizontalDirection.LeftRight) Then
                        orderControls = From con As Control In container.Controls
                                        Select con
                                        Order By con.Top, con.Left
                    Else
                        orderControls = From con As Control In container.Controls
                                        Select con
                                        Order By con.Top, con.Left Descending
                    End If
                Else
                    If _directionHorizontal.Equals(RearrangeHorizontalDirection.LeftRight) Then
                        orderControls = From con As Control In container.Controls
                                        Select con
                                        Order By con.Top Descending, con.Left
                    Else
                        orderControls = From con As Control In container.Controls
                                        Select con
                                        Order By con.Top Descending, con.Left Descending
                    End If
                End If
            End If

            For Each con As Control In orderControls
                If con.TabStop Then
                    con.TabIndex = idx
                    idx += 1
                End If

                If _force Then
                    DoIt(con)
                End If
            Next
        End Sub

        ' Public implementation of Dispose pattern callable by consumers.
        Public Sub Dispose() _
              Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        ' Protected implementation of Dispose pattern.
        Protected Overridable Sub Dispose(disposing As Boolean)
            If disposed Then Return

            If disposing Then
                handle.Dispose()

                ' Free any other managed objects here.
                '
            End If

            ' Free any unmanaged objects here.
            '
            disposed = True
        End Sub
    End Class
End Namespace