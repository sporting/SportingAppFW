Namespace SaSystem
    Public Module SaDelegates
        Public Delegate Sub BeforeUpdate(ByVal sender As Object)
        Public Delegate Sub AfterUpdate(ByVal sender As Object)
        Public Delegate Sub UpdateError(ByVal sender As Object, ByVal ex As Exception)
        Public Delegate Sub AfterSelectEventHandler()
        Public Delegate Sub BeforeDropDownEventHandler(ByVal sender As Object)
        Public Delegate Sub AfterRowSelectEventHandler(ByVal sender As Object, ByVal SelectedRow As DataRow)
        Public Delegate Sub ColumnChangedStaticValue(ByVal sender As Object, ByRef e As DataColumnChangeEventArgs)
        Public Delegate Sub ColumnChangingStaticValue(ByVal sender As Object, ByRef e As DataColumnChangeEventArgs)
        Public Delegate Sub RowChangingStaticValue(ByVal sender As Object, ByRef row As DataRow)
        Public Delegate Sub RowChangedStaticValue(ByVal sender As Object, ByRef row As DataRow)
    End Module

End Namespace