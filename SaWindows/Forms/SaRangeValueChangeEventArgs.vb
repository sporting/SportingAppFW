' Author: Nick Shih
' 2018/2/27
Imports System.ComponentModel
Namespace SaWindows.Forms
    <Description("SaBarIndicator ValueChanged Event Arguments")>
    Public Class SaRangeValueChangeEventArgs
        Inherits EventArgs
        Public RangeValue As Integer
        Public DefineValue As String

        Sub New(ByVal val As Integer, ByVal val2 As String)
            RangeValue = val
            DefineValue = val2
        End Sub
    End Class
End Namespace