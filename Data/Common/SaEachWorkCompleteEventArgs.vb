Namespace Data.Common
    Public Class SaEachWorkCompleteEventArgs
        Inherits EventArgs
        Dim _cols As String()
        Public ReadOnly Property ColumnNames As String()
            Get
                Return _cols
            End Get
        End Property

        Sub New(ByVal columns As String())
            _cols = columns
        End Sub
    End Class

End Namespace