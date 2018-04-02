'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Data.Common
Imports System.Runtime.Serialization
Imports SportingAppFW.Data.Common.SaEnum
Imports SportingAppFW.SaSystem.SaDelegates
' DataTable Filled by command if it's necessary

Namespace SaWindows.Data
    ''' <summary>
    ''' use data adapter to fetch data from database when necessary
    ''' </summary>
    <Serializable>
    Public Class SaDataTableFN
        Inherits DataTable
        Private _dataAdapter As DbDataAdapter
        Private _cmdBuilder As DbCommandBuilder
        Private _connection As DbConnection
        Private _loadedRows As Integer = 0
        Private _perFetchRows As Integer = 100
        Private _isLoadComplete As Boolean
        Private _isLoading As Boolean


        Public ReadOnly Property Adapter As DbDataAdapter
            Get
                Return _dataAdapter
            End Get
        End Property

        ''' <summary>
        ''' a statement about fetching data from database
        ''' </summary>
        ''' <returns>True: Loading   False: Idle</returns>
        ReadOnly Property IsLoading As Boolean
            Get
                Return _isLoading
            End Get
        End Property

        ''' <summary>
        ''' a statement about all data is downloaded or not
        ''' </summary>
        ''' <returns>True/False</returns>
        ReadOnly Property IsLoadComplete As Boolean
            Get
                Return _isLoadComplete
            End Get
        End Property

        ''' <summary>
        ''' how many rows to fetch per times
        ''' </summary>
        ''' <returns></returns>
        Property PerFetchRows As Integer
            Get
                Return _perFetchRows
            End Get
            Set(value As Integer)
                If value > 0 Then
                    _perFetchRows = value
                Else
                    Throw New Exception("PerFetchRows must be more than 0")
                End If
            End Set
        End Property

        Private _recordCount As Integer

        ''' <summary>
        ''' if data is not loading finish yet, recordcount will return -1
        ''' else return all data recordcount
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RecordCount() As Integer
            Get
                Return _recordCount
            End Get
        End Property

        Sub New()
            MyBase.New()
        End Sub

        Private Sub SetAdapter()
            _dataAdapter.FillSchema(Me, SchemaType.Source)
            Try
                _cmdBuilder = CreateCommandBuilder(_dataAdapter, _connection)
                _dataAdapter.InsertCommand = _cmdBuilder.GetInsertCommand
                If (PrimaryKey.Count > 0) Then
                    _dataAdapter.UpdateCommand = _cmdBuilder.GetUpdateCommand
                    _dataAdapter.DeleteCommand = _cmdBuilder.GetDeleteCommand
                End If
            Catch ex As Exception
                Console.WriteLine(String.Format("{0} {1}", TableName, ex.Message))
            End Try
        End Sub

        Public Event BeforeUpdateHandler As BeforeUpdate
        Public Event AfterUpdateHandler As AfterUpdate
        Public Event UpdateErrorHandler As UpdateError

        Sub Update()
            Try
                RaiseEvent BeforeUpdateHandler(Me)
                _dataAdapter.Update(Me)
                RaiseEvent AfterUpdateHandler(Me)
            Catch ex As Exception
                RaiseEvent UpdateErrorHandler(Me, ex)
                Console.WriteLine(String.Format("{0} {1}", TableName, ex.Message))
            End Try
        End Sub

        Sub New(ByVal sql As String, ByVal row As List(Of SaDBParameter), ByVal connection As DbConnection)
            MyBase.New()
            _connection = connection
            _dataAdapter = CreateAdapter(sql, row, connection)
            SetAdapter()
        End Sub

        Sub New(ByVal sql As String, ByVal connection As DbConnection)
            MyBase.New()
            _connection = connection
            _dataAdapter = CreateAdapter(sql, connection)
            SetAdapter()
        End Sub

        Sub New(ByVal tableName As String, ByVal sql As String, ByVal row As List(Of SaDBParameter), ByVal connection As DbConnection)
            MyBase.New(tableName)
            _connection = connection
            _dataAdapter = CreateAdapter(sql, row, connection)
            SetAdapter()
        End Sub

        Sub New(ByVal tableName As String, ByVal sql As String, ByVal connection As DbConnection)
            MyBase.New(tableName)
            _connection = connection
            _dataAdapter = CreateAdapter(sql, connection)
            SetAdapter()
        End Sub

        Sub New(ByVal Info As SerializationInfo, ByVal context As StreamingContext, ByVal sql As String, ByVal row As List(Of SaDBParameter), ByVal connection As DbConnection)
            MyBase.New(Info, context)
            _connection = connection
            _dataAdapter = CreateAdapter(sql, row, connection)
            SetAdapter()
        End Sub
        Sub New(ByVal Info As SerializationInfo, ByVal context As StreamingContext, ByVal sql As String, ByVal connection As DbConnection)
            MyBase.New(Info, context)
            _connection = connection
            _dataAdapter = CreateAdapter(sql, connection)
            SetAdapter()
        End Sub

        Sub New(ByVal tableName As String, ByVal tableNamespace As String, ByVal sql As String, ByVal row As List(Of SaDBParameter), ByVal connection As DbConnection)
            MyBase.New(tableName, tableNamespace)
            _connection = connection
            _dataAdapter = CreateAdapter(sql, row, connection)
            SetAdapter()
        End Sub

        Sub New(ByVal tableName As String, ByVal tableNamespace As String, ByVal sql As String, ByVal connection As DbConnection)
            MyBase.New(tableName, tableNamespace)
            _connection = connection
            _dataAdapter = CreateAdapter(sql, connection)
            SetAdapter()
        End Sub

        Public Overloads Shared Function CreateAdapter(ByVal strSelectSQL As String, ByVal dbconn As IDbConnection) As System.Data.Common.DbDataAdapter
            Dim dbDataAdapter As System.Data.Common.DbDataAdapter
            If (dbconn Is Nothing) Then
                Return Nothing
            End If

            Dim edt As eDatabaseType = SaDBConnection.DetectDatabaseType(dbconn)

            If edt = eDatabaseType.MSSql Then
                dbDataAdapter = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateDataAdapter()
            ElseIf edt = eDatabaseType.Oracle Then
                dbDataAdapter = DbProviderFactories.GetFactory("Oracle.DataAccess.Client").CreateDataAdapter()
            ElseIf edt = eDatabaseType.SQLite Then
                dbDataAdapter = DbProviderFactories.GetFactory("System.Data.SQLite").CreateDataAdapter()
            ElseIf edt = eDatabaseType.OleDB Then
                dbDataAdapter = DbProviderFactories.GetFactory("System.Data.OleDb").CreateDataAdapter()
            Else
                dbDataAdapter = Nothing
            End If

            dbDataAdapter.SelectCommand = DirectCast(dbconn.CreateCommand(), DbCommand)
            dbDataAdapter.SelectCommand.CommandText = strSelectSQL
            dbDataAdapter.SelectCommand = CreateParameters(dbDataAdapter.SelectCommand, strSelectSQL, Nothing)
            Return dbDataAdapter

        End Function

        Public Overloads Shared Function CreateAdapter(ByVal strSelectSQL As String, ByVal row As List(Of SaDBParameter), ByVal dbconn As IDbConnection) As DbDataAdapter
            Dim dbDataAdapter As DbDataAdapter
            If (dbconn Is Nothing) Then
                Return Nothing
            End If

            Dim edt As eDatabaseType = SaDBConnection.DetectDatabaseType(dbconn)

            If edt = eDatabaseType.MSSql Then
                dbDataAdapter = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateDataAdapter()
            ElseIf edt = eDatabaseType.Oracle Then
                dbDataAdapter = DbProviderFactories.GetFactory("Oracle.DataAccess.Client").CreateDataAdapter()
            ElseIf edt = eDatabaseType.SQLite Then
                dbDataAdapter = DbProviderFactories.GetFactory("System.Data.SQLite").CreateDataAdapter()
            ElseIf edt = eDatabaseType.OleDB Then
                dbDataAdapter = DbProviderFactories.GetFactory("System.Data.OleDb").CreateDataAdapter()
            Else
                dbDataAdapter = Nothing
            End If

            dbDataAdapter.SelectCommand = DirectCast(dbconn.CreateCommand(), DbCommand)

            dbDataAdapter.SelectCommand = CreateParameters(dbDataAdapter.SelectCommand, strSelectSQL, row)

            Return dbDataAdapter

        End Function

        Public Shared Function CreateCommandBuilder(ByRef dad As IDataAdapter, ByRef dbconn As IDbConnection) As DbCommandBuilder
            Dim dbcmdBuilder As DbCommandBuilder
            If (dbconn Is Nothing) Then
                Return Nothing
            End If

            Dim edt As eDatabaseType = SaDBConnection.DetectDatabaseType(dbconn)

            If edt = eDatabaseType.MSSql Then
                dbcmdBuilder = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommandBuilder()
            ElseIf edt = eDatabaseType.Oracle Then
                dbcmdBuilder = DbProviderFactories.GetFactory("Oracle.DataAccess.Client").CreateCommandBuilder()
            ElseIf edt = eDatabaseType.SQLite Then
                dbcmdBuilder = DbProviderFactories.GetFactory("System.Data.SQLite").CreateCommandBuilder()
            ElseIf edt = eDatabaseType.OleDB Then
                dbcmdBuilder = DbProviderFactories.GetFactory("System.Data.OleDb").CreateCommandBuilder()
            Else
                dbcmdBuilder = Nothing
            End If

            If dbcmdBuilder IsNot Nothing Then
                dbcmdBuilder.DataAdapter = dad
            End If

            Return dbcmdBuilder
        End Function


        Public Shared Function CreateParameters(ByRef dbCmd As DbCommand, ByVal sql As String, ByVal row As List(Of SaDBParameter)) As DbCommand
            If row IsNot Nothing Then
                For Each param In row
                    Dim dbParameter As DbParameter = dbCmd.CreateParameter()
                    dbParameter.DbType = param.DType
                    dbParameter.ParameterName = param.Key
                    dbParameter.Value = param.Value
                    dbCmd.Parameters.Add(dbParameter)
                Next
            End If

            dbCmd.CommandText = sql

            Return dbCmd
        End Function
        Public Sub StopFetchByRows()
            _isLoadComplete = True
            _loadedRows = Rows.Count
            _recordCount = Rows.Count
        End Sub

        Public Overloads Sub FillByDataAdapter(ByVal fillit As Boolean)
            FillByDataAdapter(_perFetchRows, fillit)
        End Sub

        Public Overloads Sub FillByDataAdapter()
            FillByDataAdapter(True)
        End Sub

        Public Overloads Sub FillByDataAdapter(ByVal fetchRows As Integer, Optional fillAll As Boolean = False)
            If _dataAdapter Is Nothing OrElse _isLoadComplete OrElse _isLoading Then
                Exit Sub
            End If
            _isLoading = True

            Try
                Me.BeginLoadData()
                If fillAll Then
                    Clear()
                    _dataAdapter.Fill(Me)
                    _isLoadComplete = True
                    _loadedRows = Rows.Count
                    _recordCount = Rows.Count
                Else
                    Dim affectRows As Integer = _dataAdapter.Fill(_loadedRows, fetchRows, Me)
                    _isLoadComplete = IIf(affectRows = 0, True, False)
                    _loadedRows = _loadedRows + affectRows
                    _recordCount = IIf(fetchRows <> affectRows OrElse affectRows = 0, _loadedRows, -1)
                End If
                Me.EndLoadData()
            Finally
                _isLoading = False
            End Try
        End Sub

        Public Shared Function ToDataTableFN(ByVal dtb As DataTable) As SaDataTableFN
            Dim dt As SaDataTableFN = New SaDataTableFN()
            For Each col As DataColumn In dtb.Columns
                Dim c As DataColumn = New DataColumn(col.ColumnName, col.DataType, col.Expression, col.ColumnMapping)
                dt.Columns.Add(c)
            Next

            For Each row As DataRow In dtb.Rows
                dt.ImportRow(row)
            Next

            dt.StopFetchByRows()

            Return dt
        End Function

    End Class

End Namespace