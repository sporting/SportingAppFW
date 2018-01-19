'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************

Imports System
Imports System.Data
Namespace Data.Common
    Public NotInheritable Class SaDataTypeMapping
        Dim typeMap As Dictionary(Of Type, DbType) = New Dictionary(Of Type, DbType)()
        Private Shared _instance As SaDataTypeMapping = Nothing
        Private Shared lock As Object = New Object()

        Private Sub New()
            'https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
            typeMap.Add(Type.GetType("System.Single"), DbType.Single)
            typeMap.Add(Type.GetType("System.Decimal"), DbType.Decimal)
            typeMap.Add(Type.GetType("System.Double"), DbType.Double)
            typeMap.Add(Type.GetType("System.Int16"), DbType.Int16)
            typeMap.Add(Type.GetType("System.Int32"), DbType.Int32)
            typeMap.Add(Type.GetType("System.Int64"), DbType.Int64)
            typeMap.Add(Type.GetType("System.Boolean"), DbType.Boolean)
            typeMap.Add(Type.GetType("System.String"), DbType.String)
            typeMap.Add(Type.GetType("System.Char[]"), DbType.String)
            typeMap.Add(Type.GetType("System.Byte"), DbType.Byte)
            typeMap.Add(Type.GetType("System.Byte[]"), DbType.Binary)
            typeMap.Add(Type.GetType("System.DateTime"), DbType.DateTime)
            typeMap.Add(Type.GetType("System.DateTimeOffset"), DbType.DateTimeOffset)
            typeMap.Add(Type.GetType("System.Object"), DbType.Object)
            typeMap.Add(Type.GetType("System.TimeSpan"), DbType.Time)
            typeMap.Add(Type.GetType("System.Guid"), DbType.Guid)
        End Sub

        Public Shared ReadOnly Property Instance() As SaDataTypeMapping
            Get
                If _instance Is Nothing Then
                    SyncLock lock
                        If _instance Is Nothing Then
                            _instance = New SaDataTypeMapping()
                        End If
                    End SyncLock
                End If
                Return _instance
            End Get
        End Property

        Public Function Mapping(ByVal dtype As Type) As DbType
            If typeMap.ContainsKey(dtype) Then
                Return typeMap(dtype)
            Else
                Return DbType.String
            End If
        End Function
    End Class
End Namespace