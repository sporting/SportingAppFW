Namespace Data.Common.DB
    ''' <summary>
    ''' DB Table to ORM Class
    ''' </summary>
    Public NotInheritable Class SaSQLiteDBTypeMappingClass
        Dim typeMap As Dictionary(Of String, System.Type) = New Dictionary(Of String, System.Type)()
        Private Shared _instance As SaSQLiteDBTypeMappingClass = Nothing
        Private Shared lock As Object = New Object()

        Private Sub New()
            'https://www.devart.com/dotconnect/sqlite/docs/DataTypeMapping.html
            typeMap.Add("BOOLEAN", Type.GetType("System.Boolean"))
            typeMap.Add("TINYINT", Type.GetType("System.Byte"))
            typeMap.Add("INT16", Type.GetType("System.Int16"))
            typeMap.Add("INT32", Type.GetType("System.Int32"))
            typeMap.Add("INT64", Type.GetType("System.Int64"))

            typeMap.Add("REAL", Type.GetType("System.Double"))
            typeMap.Add("NUMERIC", Type.GetType("System.Decimal"))
            typeMap.Add("DATETIME", Type.GetType("System.DateTime"))

            typeMap.Add("DATETIMEOFFSET", Type.GetType("System.DateTimeOffset"))
            typeMap.Add("TIME", Type.GetType("System.TimeSpan"))
            typeMap.Add("TEXT", Type.GetType("System.String"))
            typeMap.Add("BLOB", Type.GetType("System.Byte[]"))
            typeMap.Add("GUID", Type.GetType("System.Guid"))
        End Sub

        Public Shared ReadOnly Property Instance() As SaSQLiteDBTypeMappingClass
            Get
                If _instance Is Nothing Then
                    SyncLock lock
                        If _instance Is Nothing Then
                            _instance = New SaSQLiteDBTypeMappingClass()
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