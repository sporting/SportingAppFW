Namespace Data.Common.DB.Ole
    ''' <summary>
    ''' DB Table to ORM Class
    ''' </summary>

    Public NotInheritable Class SaOleDBTypeMappingClass
        Dim typeMap As Dictionary(Of String, System.Type) = New Dictionary(Of String, System.Type)()
        Private Shared _instance As SaOleDBTypeMappingClass = Nothing
        Private Shared lock As Object = New Object()

        Private Sub New()
            'https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ole-db-data-type-mappings
            typeMap.Add("DBTYPE_I8", Type.GetType("System.Int64"))
            typeMap.Add("DBTYPE_BYTES", Type.GetType("System.Byte[]"))
            typeMap.Add("DBTYPE_BOOL", Type.GetType("System.Boolean"))
            typeMap.Add("DBTYPE_BSTR", Type.GetType("System.String"))
            typeMap.Add("DBTYPE_STR", Type.GetType("System.String"))
            typeMap.Add("DBTYPE_CY", Type.GetType("System.Decimal"))
            typeMap.Add("DBTYPE_DATE", Type.GetType("System.DateTime"))
            typeMap.Add("DBTYPE_DBDATE", Type.GetType("System.DateTime"))
            typeMap.Add("DBTYPE_DBTIME", Type.GetType("System.DateTime"))
            typeMap.Add("DBTYPE_DBTIMESTAMP", Type.GetType("System.DateTime"))
            typeMap.Add("DBTYPE_DECIMAL", Type.GetType("System.Decimal"))
            typeMap.Add("DBTYPE_R8", Type.GetType("System.Double"))
            typeMap.Add("DBTYPE_ERROR", Type.GetType("System.ExternalException"))
            typeMap.Add("DBTYPE_FILETIME", Type.GetType("System.DateTime"))
            typeMap.Add("DBTYPE_GUID", Type.GetType("System.Guid"))
            typeMap.Add("DBTYPE_IDISPATCH", Type.GetType("System.Object"))
            typeMap.Add("DBTYPE_I4", Type.GetType("System.Int32"))

            typeMap.Add("DBTYPE_IUNKNOWN", Type.GetType("System.Object"))
            typeMap.Add("DBTYPE_NUMERIC", Type.GetType("System.Decimal"))
            typeMap.Add("DBTYPE_PROPVARIANT", Type.GetType("System.Object"))
            typeMap.Add("DBTYPE_R4", Type.GetType("System.Single"))
            typeMap.Add("DBTYPE_I2", Type.GetType("System.Int16"))
            typeMap.Add("DBTYPE_I1", Type.GetType("System.Byte"))
            typeMap.Add("DBTYPE_UI8", Type.GetType("System.UInt64"))
            typeMap.Add("DBTYPE_UI4", Type.GetType("System.UInt32"))

            typeMap.Add("DBTYPE_UI2", Type.GetType("System.UInt16"))
            typeMap.Add("DBTYPE_UI1", Type.GetType("System.Byte"))
            typeMap.Add("DBTYPE_VARIANT", Type.GetType("System.Object"))
            typeMap.Add("DBTYPE_WSTR", Type.GetType("System.String"))
        End Sub

        Public Shared ReadOnly Property Instance() As SaOleDBTypeMappingClass
            Get
                If _instance Is Nothing Then
                    SyncLock lock
                        If _instance Is Nothing Then
                            _instance = New SaOleDBTypeMappingClass()
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