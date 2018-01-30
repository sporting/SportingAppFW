Namespace Data.Common
    Public Class SaComboBoxSetting
        Public Property DataSourceClass As Type
        Public Property ValueMember As String
        Public Property ColumnsToDisplay As List(Of SaColumnNameToDisplay)
        Public Property PassToTargets As List(Of SaPassColumnToTarget)

        Public Property FilterWhere As String
    End Class
End Namespace
