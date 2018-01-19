Namespace Data.Common
    Public Class SaColumnNameToDisplay
        Public Property Name As String
        Public Property DisplayText As String

        Sub New(ByVal na As String, ByVal text As String)
            Name = na
            DisplayText = text
        End Sub
    End Class

    Public Class SaPassColumnToTarget
        Public Property SourceColumnName As String
        Public Property TargetColumnName As String
        Sub New(ByVal src As String, ByVal tar As String)
            SourceColumnName = src
            TargetColumnName = tar
        End Sub
    End Class

End Namespace