Namespace Data.Common.DB
    ''' <summary>
    ''' DB Table to ORM Class
    ''' </summary>

    Public NotInheritable Class SaMSSqlDBTypeMappingClass
        Dim typeMap As Dictionary(Of String, System.Type) = New Dictionary(Of String, System.Type)()
        Private Shared _instance As SaMSSqlDBTypeMappingClass = Nothing
        Private Shared lock As Object = New Object()

        Private Sub New()
            'https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
            typeMap.Add("BIGINT", Type.GetType("System.Int64"))
            typeMap.Add("BINARY", Type.GetType("System.Byte[]"))
            typeMap.Add("BIT", Type.GetType("System.Boolean"))
            typeMap.Add("CHAR", Type.GetType("System.String"))
            typeMap.Add("DATE", Type.GetType("System.DateTime"))
            typeMap.Add("DATETIME", Type.GetType("System.DateTime"))
            typeMap.Add("DATETIME2", Type.GetType("System.DateTime"))
            typeMap.Add("DATETIMEOFFSET", Type.GetType("System.DateTimeOffset"))
            typeMap.Add("DECIMAL", Type.GetType("System.Decimal"))
            typeMap.Add("FILESTREAM ATTRIBUTE (VARBINARY(MAX))", Type.GetType("System.Byte[]"))
            typeMap.Add("FLOAT", Type.GetType("System.Double"))
            typeMap.Add("IMAGE", Type.GetType("System.Byte[]"))
            typeMap.Add("INT", Type.GetType("System.Int32"))
            typeMap.Add("MONEY", Type.GetType("System.Decimal"))
            typeMap.Add("NCHAR", Type.GetType("System.String"))
            typeMap.Add("NTEXT", Type.GetType("System.String"))
            typeMap.Add("NUMERIC", Type.GetType("System.Decimal"))
            typeMap.Add("NVARCHAR", Type.GetType("System.String"))
            typeMap.Add("REAL", Type.GetType("System.Single"))
            typeMap.Add("ROWVERSION", Type.GetType("System.Byte[]"))
            typeMap.Add("SMALLDATETIME", Type.GetType("System.DateTime"))
            typeMap.Add("SMALLINT", Type.GetType("System.Int16"))
            typeMap.Add("SMALLMONEY", Type.GetType("System.Decimal"))
            typeMap.Add("SQL_VARIANT", Type.GetType("System.Object *"))
            typeMap.Add("TEXT", Type.GetType("System.String"))
            typeMap.Add("TIME", Type.GetType("System.TimeSpan"))
            typeMap.Add("TIMESTAMP", Type.GetType("System.Byte[]"))
            typeMap.Add("TINYINT", Type.GetType("System.Byte"))
            typeMap.Add("UNIQUEIDENTIFIER", Type.GetType("System.Guid"))
            typeMap.Add("VARBINARY", Type.GetType("System.Byte[]"))
            typeMap.Add("VARCHAR", Type.GetType("System.String"))
            typeMap.Add("XML", Type.GetType("System.Xml"))
        End Sub

        Public Shared ReadOnly Property Instance() As SaMSSqlDBTypeMappingClass
            Get
                If _instance Is Nothing Then
                    SyncLock lock
                        If _instance Is Nothing Then
                            _instance = New SaMSSqlDBTypeMappingClass()
                        End If
                    End SyncLock
                End If
                Return _instance
            End Get
        End Property

        Public Function Mapping(ByVal dtype As String) As System.Type
            If typeMap.ContainsKey(dtype.ToUpper()) Then
                Return typeMap(dtype.ToUpper())
            Else
                Return Type.GetType("System.String")
            End If
        End Function
    End Class
End Namespace