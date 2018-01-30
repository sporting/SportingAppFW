Namespace Data.Common.DB
    Public Class SaSQLiteDBColumnType
        Inherits SaDBColumnType
        Public Overrides ReadOnly Property MsType As System.Type
            Get
                Return SaSQLiteDBTypeMappingClass.Instance.Mapping(data_type)
            End Get
        End Property

        Sub New(ByVal col As String, ByVal type As String)
            MyBase.New(col, type)
        End Sub
    End Class
End Namespace