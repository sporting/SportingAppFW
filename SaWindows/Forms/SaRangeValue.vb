' Author: Nick Shih
' 2018/2/27
Imports System.ComponentModel
Namespace SaWindows.Forms
    <Description("SaMultiRangeBar RangeValue Structure")>
    Public Structure SaRangeValue
        Dim MinValue As Integer
        Dim MaxValue As Integer
        Dim DefineValue As String

        Sub New(ByVal iVal As Integer, ByVal xVal As Integer, ByVal val As String)
            MinValue = iVal
            MaxValue = xVal
            DefineValue = val
        End Sub
    End Structure
End Namespace