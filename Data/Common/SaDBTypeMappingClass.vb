'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Namespace Data.Common
    ''' <summary>
    ''' DB Table to ORM Class
    ''' </summary>
    Public NotInheritable Class SaDBTypeMappingClass
        Dim typeMap As Dictionary(Of String, System.Type) = New Dictionary(Of String, System.Type)()
        Private Shared _instance As SaDBTypeMappingClass = Nothing
        Private Shared lock As Object = New Object()

        Private Sub New()
            'https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/oracle-data-type-mappings
            typeMap.Add("BFILE", Type.GetType("System.Byte[]"))
            typeMap.Add("BLOB", Type.GetType("System.Byte[]"))
            typeMap.Add("CHAR", Type.GetType("System.String"))
            typeMap.Add("CLOB", Type.GetType("System.String"))
            typeMap.Add("DATE", Type.GetType("System.DateTime"))
            typeMap.Add("FLOAT", Type.GetType("System.Decimal"))
            typeMap.Add("INTEGER", Type.GetType("System.Decimal"))
            typeMap.Add("LONG", Type.GetType("System.String"))
            typeMap.Add("LONG RAW", Type.GetType("System.Byte[]"))
            typeMap.Add("NCHAR", Type.GetType("System.String"))
            typeMap.Add("NCLOB", Type.GetType("System.String"))
            typeMap.Add("NUMBER", Type.GetType("System.Decimal"))
            typeMap.Add("NVARCHAR2", Type.GetType("System.String"))
            typeMap.Add("RAW", Type.GetType("System.Byte[]"))
            typeMap.Add("ROWID", Type.GetType("System.String"))
            typeMap.Add("TIMESTAMP", Type.GetType("System.DateTime"))
            typeMap.Add("VARCHAR2", Type.GetType("System.String"))

        End Sub

        Public Shared ReadOnly Property Instance() As SaDBTypeMappingClass
            Get
                If _instance Is Nothing Then
                    SyncLock lock
                        If _instance Is Nothing Then
                            _instance = New SaDBTypeMappingClass()
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
