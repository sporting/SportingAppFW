Imports System.ComponentModel

Namespace Data.Common
    Public Module SaDelegates

        Public Class AfterQueryEventArgs
            Inherits EventArgs
            'Dim _sql As String
            'Public ReadOnly Property sql As String
            '    Get
            '        Return _sql
            '    End Get
            'End Property
            'Sub New(ByVal sql As String)
            '    _sql = sql
            'End Sub
            Sub New()

            End Sub
        End Class

        Public Delegate Sub AfterQuery(ByVal sender As Object, ByVal e As AfterQueryEventArgs)
        Public Delegate Sub EachWorkCompleteEvent(ByVal sender As Object, ByVal e As SaEachWorkCompleteEventArgs)
        Public Delegate Sub removeSelf(ByVal sender As Object, ByVal e As EventArgs)
        Public Delegate Sub DBStatusMessage(ByVal sender As Object, ByVal msg As String)
        Public Delegate Sub DBOpenedMessage()
        Public Delegate Sub DBClosingMessage(ByRef e As CancelEventArgs)
        Public Delegate Sub DBClosedMessage()
        Public Delegate Sub StatusMessage(ByVal sender As Object, ByVal msg As String)
        Public Delegate Sub InfoMessage(ByVal sender As Object, ByVal info As String)
    End Module

End Namespace