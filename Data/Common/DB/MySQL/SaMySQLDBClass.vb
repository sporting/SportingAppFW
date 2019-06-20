Imports System.Data.Common
Imports SportingAppFW.Data.Common.SaEnum
Imports SportingAppFW.Extensions
Imports SportingAppFW.SaWindows.Data
Imports SportingAppFW.Tools

Namespace Data.Common.DB.MySQL
    Public Class SaMySQLDBClass
        Inherits SaBaseDBClass

        Private _datasource As String
        Private _clock As SaUseTimeClass = New SaUseTimeClass()

        Protected Sub New()
            LogTag = "SaMySQLDBClass"
        End Sub

        Public Overrides ReadOnly Property SimpleConnectionString() As String
            Get
                Return _datasource
            End Get
        End Property

        Public Sub New(ByVal server As String, ByVal db As String, ByVal uid As String, ByVal pwd As String, Optional ByVal port As Integer = 3306)
            Me.New(String.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};", server, port, db, uid, pwd))

            Logger.SaveLog(LogTag, String.Format("{0} {1} {2}", "Database Connect to: ", server, db))

            _datasource = server
        End Sub

        Public Sub New(ByVal connection_string As String)
            Me.New()
            Throw New NotImplementedException("MySQL Connection must use .net framework 4.5.2")
            '_db = New (connection_string)
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

        Public Overloads Function ExecuteSQL(ByVal databaseName As String, ByVal sql As String, ByVal row As List(Of SaDBParameter), Optional ByVal allDataLoad As Boolean = False) As SaDataTableFN
            ChangeDatabase(databaseName)
            Return ExecuteSQL(sql, row, allDataLoad)
        End Function

        Public Overloads Overrides Function ExecuteSQL(ByVal sql As String, ByVal row As List(Of SaDBParameter), Optional ByVal allDataLoad As Boolean = False) As SaDataTableFN
            CreateConnection()
            sql = sql.Trim()
            '  Logger.SaveLog(LogTag, sql)
            If _db IsNot Nothing Then
                _executionStatus = ExecutionStatus.Executing
                Try
                    Dim dtb As SaDataTableFN
                    If IsSelectSQL(sql) Then
                        _clock.StartCal()

                        dtb = New SaDataTableFN(sql, row, _db)

                        dtb.FillByDataAdapter(allDataLoad)

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

        Public Overloads Function ExecuteSQL(ByVal databaseName As String, ByVal sql As String, Optional ByVal allDataLoad As Boolean = False) As SaDataTableFN
            ChangeDatabase(databaseName)
            Return ExecuteSQL(sql, allDataLoad)
        End Function

        Public Overloads Overrides Function ExecuteSQL(ByVal sql As String, Optional ByVal allDataLoad As Boolean = False) As SaDataTableFN
            CreateConnection()
            sql = sql.Trim()
            ' Logger.SaveLog(LogTag, sql)
            If _db IsNot Nothing Then
                Dim dtb As SaDataTableFN

                _executionStatus = ExecutionStatus.Executing
                Try
                    If IsSelectSQL(sql) Then
                        _clock.StartCal()
                        dtb = New SaDataTableFN(sql, _db)

                        dtb.FillByDataAdapter(allDataLoad)

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
            Dim sql As String = "SELECT schema_name FROM information_schema.schemata"

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
            Dim sql As String = "SELECT CONCAT(TABLE_SCHEMA,'.',TABLE_NAME,' (',IFNULL(TABLE_ROWS,0),')') AS TABLE_DESC,TABLE_ROWS,TABLE_SCHEMA FROM information_schema.tables "

            ' Logger.SaveLog(LogTag, sql)
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

            Dim sql As String = "select table_type from information_schema.TABLES where table_name =" + table.QuotedStr()

            ' Logger.SaveLog(LogTag, sql)
            If _db IsNot Nothing Then

                _executionStatus = ExecutionStatus.Executing
                Try
                    Dim dad As DbDataAdapter = SaDataTableFN.CreateAdapter(sql, _db)
                    Dim r = dad.SelectCommand.ExecuteScalar()

                    If r IsNot Nothing Then
                        Return IIf(r.ToString().ToUpper() = "BASE TABLE", ObjectType.otTable, ObjectType.otView)
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
            Dim sql As String = "SELECT TABLE_SCHEMA,TABLE_NAME,INDEX_SCHEMA, INDEX_NAME,SEQ_IN_INDEX,COLUMN_NAME,NULLABLE FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_NAME=@TABLE_NAME ORDER BY TABLE_SCHEMA,TABLE_NAME,INDEX_SCHEMA,INDEX_NAME"
            Dim params As List(Of SaDBParameter) = New List(Of SaDBParameter)()
            params.Add(New SaDBParameter(Type.GetType("System.String"), "TABLE_NAME", table.TableName))
            ' Logger.SaveLog(LogTag, sql)
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
            Dim sql As String = "SELECT TABLE_SCHEMA,TABLE_NAME,COLUMN_NAME,IS_NULLABLE,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH FROM information_schema.columns WHERE TABLE_NAME=@TABLE_NAME"
            Dim params As List(Of SaDBParameter) = New List(Of SaDBParameter)()
            params.Add(New SaDBParameter(Type.GetType("System.String"), "TABLE_NAME", table.TableName))
            '  Logger.SaveLog(LogTag, sql)
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
                            dataCollection.Add(New SaMySQLDBColumnType(row("COLUMN_NAME"), row("DATA_TYPE")))
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