Namespace Data.Common.DB.MySQL
    ''' <summary>
    ''' DB Table to ORM Class
    ''' </summary>

    Public NotInheritable Class SaMySQLDBTypeMappingClass
        Dim typeMap As Dictionary(Of String, System.Type) = New Dictionary(Of String, System.Type)()
        Private Shared _instance As SaMySQLDBTypeMappingClass = Nothing
        Private Shared lock As Object = New Object()

        Private Sub New()
            '.NET Data Types    MySQL Data Types
            'System.Boolean     Boolean, bit(1)
            'System.Byte      tinyint unsigned
            'System.Byte[]	   binary, varbinary, blob, longblob
            'System.DateTime   DateTime
            'System.Decimal    Decimal
            'System.Double     Double
            'System.Guid       Char(36)
            'System.Int16      smallint
            'System.Int32    Int
            'System.Int64    bigint
            'System.SByte    tinyint
            'System.Single   float
            'System.String   Char, varchar, Text, longtext
            'System.TimeSpan time
            typeMap.Add("Boolean", Type.GetType("System.Boolean"))
            typeMap.Add("tinyint", Type.GetType("System.Byte"))
            typeMap.Add("unsigned", Type.GetType("System.Byte"))
            typeMap.Add("binary", Type.GetType("System.Byte[]"))
            typeMap.Add("varbinary", Type.GetType("System.Byte[]"))
            typeMap.Add("blob", Type.GetType("System.Byte[]"))
            typeMap.Add("longblob", Type.GetType("System.Byte[]"))
            typeMap.Add("DateTime", Type.GetType("System.DateTime"))
            typeMap.Add("Decimal", Type.GetType("System.Decimal"))
            typeMap.Add("bit(1)", Type.GetType("System.Boolean"))
            typeMap.Add("Double", Type.GetType("System.Double"))
            typeMap.Add("Char(36)", Type.GetType("System.Guid"))
            typeMap.Add("smallint", Type.GetType("System.Int16"))
            typeMap.Add("Int", Type.GetType("System.Int32"))
            typeMap.Add("bigint", Type.GetType("System.Int64"))
            typeMap.Add("tinyint", Type.GetType("System.SByte"))
            typeMap.Add("float", Type.GetType("System.Single"))
            typeMap.Add("Char", Type.GetType("System.String"))
            typeMap.Add("varchar", Type.GetType("System.String"))
            typeMap.Add("Text", Type.GetType("System.String"))
            typeMap.Add("longtext", Type.GetType("System.String"))
            typeMap.Add("time", Type.GetType("System.TimeSpan"))
        End Sub

        Public Shared ReadOnly Property Instance() As SaMySQLDBTypeMappingClass
            Get
                If _instance Is Nothing Then
                    SyncLock lock
                        If _instance Is Nothing Then
                            _instance = New SaMySQLDBTypeMappingClass()
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