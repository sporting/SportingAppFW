Imports System.Data.Common
Imports System.Data.SqlClient
Imports SportingAppFW.Components.Data
Imports SportingAppFW.Data.Common.SaEnum
Imports SportingAppFW.Extensions
Imports SportingAppFW.Tools

Namespace Data.Common.DB.MSSql
    Public Class SaMSSqlDBClass
        Inherits SaBaseDBClass

        Private _datasource As String
        Private _clock As SaUseTimeClass = New SaUseTimeClass()

        Protected Sub New()
            LogTag = "SaMSSqlDBClass"
        End Sub

        Public Overrides ReadOnly Property SimpleConnectionString() As String
            Get
                Return _datasource
            End Get
        End Property

        Public Sub New(ByVal datasource As String, ByVal userid As String, ByVal passwd As String, Optional ByVal auto_commit As Boolean = False)
            Me.New(String.Format("Data Source={0};Persist Security Info=True;User Id={1};Password={2}", datasource, userid, passwd), auto_commit)

            Logger.SaveLog(LogTag, String.Format("{0} {1}", "Database Connect to: ", datasource))

            _datasource = datasource
        End Sub

        Public Sub New(ByVal connection_string As String, Optional ByVal auto_commit As Boolean = False)
            Me.New()

            _autoCommit = auto_commit

            _db = New SqlConnection(connection_string)
            CreateConnection()
        End Sub


        Protected Overloads Overrides Sub CreateConnection()
            _connectStatus = ConnectStatus.Connecting
            Try
                DatabaseType = eDatabaseType.MSSql

                If _db IsNot Nothing Then
                    DBOpen()
                    _connectStatus = ConnectStatus.Connected
                Else
                    _connectStatus = ConnectStatus.NotConnect
                End If
            Catch ex As Exception
                _connectStatus = ConnectStatus.FailConnect
                _db = Nothing
                Logger.SaveLog(LogTag, ex.Message)
            End Try
        End Sub

        Public Overloads Function ExecuteSQL(ByVal databaseName As String, ByVal sql As String, ByVal row As List(Of SaDBParameter)) As SaDataTableFN
            ChangeDatabase(databaseName)
            Return ExecuteSQL(sql, row)
        End Function

        Public Overloads Overrides Function ExecuteSQL(ByVal sql As String, ByVal row As List(Of SaDBParameter)) As SaDataTableFN
            CreateConnection()
            sql = sql.Trim()
            Logger.SaveLog(LogTag, sql)
            If _db IsNot Nothing Then
                _executionStatus = ExecutionStatus.Executing
                Try
                    Dim dtb As SaDataTableFN
                    If IsSelectSQL(sql) Then
                        _clock.StartCal()

                        dtb = New SaDataTableFN(sql, row, _db)

                        dtb.FillByDataAdapter(False)

                        DBMessage(Me, _clock.StopCal())

                        Return dtb
                    Else
                        Try
                            _clock.StartCal()
                            If Not _autoCommit Then
                                StartTransaction()
                            End If

                            If IsCommitSQL(sql) Then
                                Commit()

                                DBMessage(Me, "Commit success" + _clock.StopCal())
                            ElseIf IsRollbackSQL(sql) Then
                                Rollback()

                                DBMessage(Me, "Rollback success" + _clock.StopCal())
                            Else
                                Dim dbCommand As DbCommand = Nothing

                                dbCommand = SaDataTableFN.CreateParameters(_db.CreateCommand(), sql, row)
                                Dim affectrows As Integer = dbCommand.ExecuteNonQuery()
                                DBMessage(Me, "affect rows: " + affectrows.ToString() + " " + _clock.StopCal())
                            End If

                        Finally
                            'DBClose()
                        End Try
                        Return Nothing
                    End If
                Catch ex As Exception
                    DBMessage(Me, ex.Message)
                    Return Nothing
                Finally
                    _executionStatus = ExecutionStatus.Idle
                End Try
            End If
            DBMessage(Me, "No Connection")
            Return Nothing
        End Function

        Public Overloads Function ExecuteSQL(ByVal databaseName As String, ByVal sql As String) As SaDataTableFN
            ChangeDatabase(databaseName)
            Return ExecuteSQL(sql)
        End Function

        Public Overloads Overrides Function ExecuteSQL(ByVal sql As String) As SaDataTableFN
            CreateConnection()
            sql = sql.Trim()
            Logger.SaveLog(LogTag, sql)
            If _db IsNot Nothing Then
                Dim dtb As SaDataTableFN

                _executionStatus = ExecutionStatus.Executing
                Try
                    If IsSelectSQL(sql) Then
                        _clock.StartCal()
                        dtb = New SaDataTableFN(sql, _db)

                        dtb.FillByDataAdapter(False)

                        DBMessage(Me, _clock.StopCal())

                        Return dtb
                    Else
                        Try
                            _clock.StartCal()
                            If Not _autoCommit Then
                                StartTransaction()
                            End If

                            If IsCommitSQL(sql) Then
                                Commit()

                                DBMessage(Me, "Commit success" + _clock.StopCal())
                            ElseIf IsRollbackSQL(sql) Then
                                Rollback()

                                DBMessage(Me, "Rollback success" + _clock.StopCal())
                            Else
                                Dim dbCommand As DbCommand = Nothing

                                dbCommand = SaDataTableFN.CreateParameters(_db.CreateCommand(), sql, Nothing)
                                Dim affectrows As Integer = dbCommand.ExecuteNonQuery()
                                DBMessage(Me, "affect rows: " + affectrows.ToString() + " " + _clock.StopCal())
                            End If

                        Finally
                            'DBClose()
                        End Try

                        Return Nothing
                    End If
                Catch ex As Exception
                    DBMessage(Me, ex.Message)
                    Return Nothing
                Finally
                    _executionStatus = ExecutionStatus.Idle
                End Try
            End If
            DBMessage(Me, "No Connection")
            Return Nothing
        End Function

        Public Overloads Function GetDDL(ByVal databaseName As String, ByVal table As SaTableSettings) As String
            ChangeDatabase(databaseName)
            Return GetDDL(table)
        End Function
        Public Overrides Function GetDDL(ByVal table As SaTableSettings) As String
            DBMessage(Me, "No Implement")
            Return Nothing
        End Function

        Public Function ChangeDatabase(databaseName As String) As Boolean
            Try
                DBOpen()
                _db.ChangeDatabase(databaseName)
                Return True
            Catch ex As Exception
                DBMessage(Me, ex.Message)
                Return False
            End Try
        End Function
        Public Function GetDatabases() As SaDataTableFN
            CreateConnection()
            '+ "       QUOTENAME(SCHEMA_NAME(sOBJ.schema_id)) + '.' + QUOTENAME(sOBJ.name) AS [TableName] " _
            Dim sql As String = "SELECT name FROM master.dbo.sysdatabases where HAS_DBACCESS(name) =1"

            Logger.SaveLog(LogTag, sql)
            If _db IsNot Nothing Then

                _executionStatus = ExecutionStatus.Executing
                Try
                    Dim dtb As SaDataTableFN = New SaDataTableFN(sql, _db)
                    dtb.FillByDataAdapter(True)

                    Return dtb
                Finally
                    _executionStatus = ExecutionStatus.Idle
                End Try
            End If
            DBMessage(Me, "No Connection")
            Return Nothing
        End Function

        Public Overloads Function GetTables(ByVal databaseName As String) As SaDataTableFN
            ChangeDatabase(databaseName)
            Return GetTables()
        End Function

        Public Overloads Overrides Function GetTables() As SaDataTableFN
            CreateConnection()
            '+ "       QUOTENAME(SCHEMA_NAME(sOBJ.schema_id)) + '.' + QUOTENAME(sOBJ.name) AS [TableName] " _
            Dim sql As String = "WITH SubSet AS " _
                + " ( " _
                + " SELECT " _
                + "       sOBJ.name AS [TableName] " _
                + "       , SUM(sPTN.Rows) AS [RowCount] " _
                + "       , SCHEMA_NAME(sOBJ.schema_id) AS [SchemaName] " _
                + " FROM  " _
                + "       sys.objects AS sOBJ " _
                + "       INNER JOIN sys.partitions AS sPTN " _
                + "             On sOBJ.object_id = sPTN.object_id " _
                + " WHERE " _
                + "       sOBJ.type = 'U' " _
                + "       And sOBJ.is_ms_shipped = 0x0 " _
                + "       And index_id < 2 " _
                + " Group BY  " _
                + "       sOBJ.schema_id " _
                + "       , sOBJ.name " _
                + " ) " _
                + " Select [SchemaName]+'.'+[TableName]+' ('+(CAST([RowCount] as VARCHAR(max)))+')' AS [TABLE_DESC], [TableName] AS [TABLE_NAME],[RowCount] AS [NUM_ROWS] FROM SubSet "

            Logger.SaveLog(LogTag, sql)
            If _db IsNot Nothing Then

                _executionStatus = ExecutionStatus.Executing
                Try
                    Dim dtb As SaDataTableFN = New SaDataTableFN(sql, _db)
                    dtb.FillByDataAdapter(True)

                    Return dtb
                Finally
                    _executionStatus = ExecutionStatus.Idle
                End Try
            End If
            DBMessage(Me, "No Connection")
            Return Nothing
        End Function

        Public Overloads Function DetectTableType(ByVal databaseName As String, ByVal table As String) As ObjectType
            ChangeDatabase(databaseName)
            Return DetectTableType(table)
        End Function

        Public Overrides Function DetectTableType(table As String) As ObjectType
            CreateConnection()

            Dim sql As String = "SELECT TYPE_DESC FROM SYS.OBJECTS WHERE NAME=" + table.QuotedStr()

            Logger.SaveLog(LogTag, sql)
            If _db IsNot Nothing Then

                _executionStatus = ExecutionStatus.Executing
                Try
                    Dim dad As DbDataAdapter = SaDataTableFN.CreateAdapter(sql, _db)
                    Dim r = dad.SelectCommand.ExecuteScalar()

                    If r IsNot Nothing Then
                        Return IIf(r.ToString().ToUpper() = "USER_TABLE", ObjectType.otTable, ObjectType.otView)
                    Else
                        Return ObjectType.otTable
                    End If
                Finally
                    _executionStatus = ExecutionStatus.Idle
                End Try
            End If
            DBMessage(Me, "No Connection")
            Return Nothing
        End Function

        Public Overrides Function ExecutionPlan(sql As String) As SaDataTableFN
            DBMessage(Me, "No Implement")
            Return Nothing
        End Function

        Public Overloads Function GetIndexes(ByVal databaseName As String, ByVal table As SaTableSettings) As SaDataTableFN
            ChangeDatabase(databaseName)
            Return GetIndexes(table)
        End Function
        Public Overrides Function GetIndexes(ByVal table As SaTableSettings) As SaDataTableFN
            CreateConnection()
            Dim sql As String = "SELECT B.OBJECT_ID,B.NAME AS INDEX_NAME,D.NAME AS COLUMN_NAME,E.NAME AS DATA_TYPE, D.MAX_LENGTH,D.PRECISION FROM SYS.objects A, SYS.INDEXES B,SYS.INDEX_COLUMNS C,SYS.COLUMNS D,SYS.TYPES E WHERE A.OBJECT_ID=B.OBJECT_ID AND A.NAME=@TABLE_NAME AND B.OBJECT_ID=C.OBJECT_ID AND B.INDEX_ID=C.INDEX_ID AND C.OBJECT_ID=D.OBJECT_ID AND C.COLUMN_ID=D.COLUMN_ID AND D.user_type_id=E.user_type_id ORDER BY A.NAME,B.NAME,D.NAME"
            Dim params As List(Of SaDBParameter) = New List(Of SaDBParameter)()
            params.Add(New SaDBParameter(Type.GetType("System.String"), "TABLE_NAME", table.TableName))
            Logger.SaveLog(LogTag, sql)
            If _db IsNot Nothing Then
                Dim dtb As SaDataTableFN

                _executionStatus = ExecutionStatus.Executing
                Try
                    dtb = New SaDataTableFN(sql, params, _db)

                    dtb.FillByDataAdapter(True)

                    Return dtb
                Finally
                    _executionStatus = ExecutionStatus.Idle
                End Try
            End If
            DBMessage(Me, "No Connection")
            Return Nothing
        End Function

        Public Overloads Overrides Function GetColumns(ByVal table As SaTableSettings) As SaDataTableFN
            CreateConnection()
            Dim sql As String = "SELECT D.NAME AS COLUMN_NAME,D.COLUMN_ID,E.NAME AS DATA_TYPE,D.MAX_LENGTH,D.PRECISION FROM SYS.objects A,SYS.COLUMNS D,SYS.TYPES E WHERE A.NAME=@TABLE_NAME AND A.OBJECT_ID=D.OBJECT_ID  AND D.user_type_id=E.user_type_id ORDER BY D.COLUMN_ID"
            Dim params As List(Of SaDBParameter) = New List(Of SaDBParameter)()
            params.Add(New SaDBParameter(Type.GetType("System.String"), "TABLE_NAME", table.TableName))
            Logger.SaveLog(LogTag, sql)
            If _db IsNot Nothing Then
                Dim dtb As SaDataTableFN
                _executionStatus = ExecutionStatus.Executing
                Try
                    dtb = New SaDataTableFN(sql, params, _db)
                    dtb.FillByDataAdapter(True)

                    Return dtb
                Finally
                    _executionStatus = ExecutionStatus.Idle
                End Try
            End If
            DBMessage(Me, "No Connection")
            Return Nothing
        End Function

        Public Overloads Function GetColumns(ByVal databaseName As String, ByVal table As SaTableSettings) As SaDataTableFN
            ChangeDatabase(databaseName)
            Return GetColumns(table)
        End Function

        Public Overloads Function GetColumnsTypeCollection(ByVal databaseName As String, ByVal table As SaTableSettings) As List(Of SaDBColumnType)
            ChangeDatabase(databaseName)
            Return GetColumnsTypeCollection(table)
        End Function

        Public Overrides Function GetColumnsTypeCollection(ByVal table As SaTableSettings) As List(Of SaDBColumnType)
            CreateConnection()
            Try
                If _db IsNot Nothing Then
                    Dim dtb As SaDataTableFN = GetColumns(table)
                    If dtb.Rows.Count > 0 Then
                        Dim dataCollection As List(Of SaDBColumnType) = New List(Of SaDBColumnType)()
                        For Each row In dtb.Rows
                            dataCollection.Add(New SaMSSqlDBColumnType(row("COLUMN_NAME"), row("DATA_TYPE")))
                        Next

                        Return dataCollection
                    Else
                        Return Nothing
                    End If
                Else
                    DBMessage(Me, "No Connection")
                End If
            Catch ex As Exception
                DBMessage(Me, ex.Message)
            Finally
            End Try

            Return Nothing
        End Function
    End Class

End Namespace