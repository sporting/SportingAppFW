'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Namespace Tools
    Public Class SaUseTimeClass
        Dim _stopwatch As Stopwatch

        Sub New()
            _stopwatch = New Stopwatch()
        End Sub

        Public Sub StartCal()
            _stopwatch.Reset()
            _stopwatch.Start()
        End Sub

        Public Function StopCal() As String
            If _stopwatch.IsRunning Then
                _stopwatch.Stop()

                Dim ts As TimeSpan = _stopwatch.Elapsed
                Dim elapsedTime As String = String.Format("Elapsed Time: {0:00}:{1:00}:{2:00}.{3:00}",
                                        ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10)
                Return elapsedTime
            Else
                Throw New System.Exception("Must call StartCal() before StopCal()")
            End If
        End Function

    End Class

End Namespace