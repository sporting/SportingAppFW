Imports System.Data.Common
Imports System.Text.RegularExpressions
Imports Oracle.DataAccess.Client
Imports SportingAppFW.Components.Data
Imports SportingAppFW.Data.Common.SaEnum
Imports SportingAppFW.Extensions
Imports SportingAppFW.Extensions.SaStringExtension
Imports SportingAppFW.Tools

Namespace Data.Common.DB.Oracle
    Public Class SaOracleDBClass
        Inherits SaBaseDBClass
        Public ReadOnly Property DBHost() As String
            Get
                Dim res As String = String.Empty
                If _db IsNot Nothing Then
                    Dim connection_string = _db.ConnectionString
                    Dim pattern As String = "\(HOST[\s]?=[\s]?(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}|[\w|\-]+|[\w|\.]+)\)"
                    Dim r As Regex = New Regex(pattern, RegexOptions.IgnoreCase)
                    Dim m As Match = r.Match(connection_string)
                    res = connection_string
                    If m.Success Then
                        res = m.Groups(1).ToString().Trim()
                    End If
                End If
                Return res
            End Get
        End Property

        Public ReadOnly Property ServiceName() As String
            Get
                Dim res As String = String.Empty
                If _db IsNot Nothing Then
                    Dim connection_string = _db.ConnectionString
                    Dim pattern As String = "\(SERVICE_NAME[\s]?=[\s]?([\w|\.]+)\)"
                    Dim r As Regex = New Regex(pattern, RegexOptions.IgnoreCase)
                    Dim m As Match = r.Match(connection_string)
                    res = connection_string
                    If m.Success Then
                        res = m.Groups(1).ToString()
                    Else
                        pattern = "\(SID[\s]?=[\s]?([\w|\.]+)\)"
                        r = New Regex(pattern, RegexOptions.IgnoreCase)
                        m = r.Match(connection_string)
                        If m.Success Then
                            res = m.Groups(1).ToString()
                        End If
                    End If
                End If
                Return res
            End Get
        End Property

        Public Overrides ReadOnly Property SimpleConnectionString() As String
            Get
                Return DBHost + " " + ServiceName
            End Get
        End Property

        Private _clock As SaUseTimeClass = New SaUseTimeClass()

        Protected Sub New()
            LogTag = "SaOracleDBClass"
        End Sub


        Public Sub New(ByVal datasource As String, ByVal userid As String, ByVal passwd As String, Optional ByVal auto_commit As Boolean = False)
            Me.New(String.Format("Data Source={0};User Id={1};Password={2}", datasource, userid, passwd), auto_commit)
        End Sub

        Public Sub New(ByVal connection_string As String, Optional ByVal auto_commit As Boolean = False)
            Me.New()

            Logger.SaveLog(LogTag, String.Format("{0} {1}", "Database Connect to: ", connection_string))

            _autoCommit = auto_commit

            _db = New OracleConnection(connection_string)
            CreateConnection()
        End Sub


        Protected Overloads Overrides Sub CreateConnection()
            _connectStatus = ConnectStatus.Connecting
            Try
                DatabaseType = eDatabaseType.Oracle

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


        Public Overrides Function GetDDL(ByVal table As SaTableSettings) As String
            CreateConnection()

            Dim sql As String = "SELECT DBMS_METADATA.GET_DDL('" + IIf(table.TableType = ObjectType.otTable, "TABLE", "VIEW") + "', " + table.TableName.QuotedStr() + ", USER) FROM DUAL"

            Logger.SaveLog(LogTag, sql)
            If _db IsNot Nothing Then

                _executionStatus = ExecutionStatus.Executing
                Try
                    Dim dad As DbDataAdapter = SaDataTableFN.CreateAdapter(sql, _db)
                    Dim r = dad.SelectCommand.ExecuteScalar()

                    If r IsNot Nothing Then
                        Return r.ToString()
                    Else
                        Return String.Empty
                    End If

                Finally
                    _executionStatus = ExecutionStatus.Idle
                End Try
            End If
            DBMessage(Me, "No Connection")
            Return Nothing
        End Function

        Public Overrides Function GetTables() As SaDataTableFN
            CreateConnection()
            'Dim sql As String = "SELECT TABLE_NAME||' ('||NVL(NUM_ROWS,0)||')' AS TABLE_DESC, TABLE_NAME,NUM_ROWS FROM ALL_TABLES WHERE OWNER = USER ORDER BY TABLE_NAME"
            Dim sql As String = "SELECT * FROM ( " _
                    + " SELECT TABLE_NAME||' ('||NVL(NUM_ROWS,0)||')' AS TABLE_DESC, TABLE_NAME,NUM_ROWS FROM ALL_TABLES WHERE OWNER = USER UNION ALL " _
                    + " SELECT VIEW_NAME||' (?)' AS TABLE_DESC, VIEW_NAME AS TABLE_NAME,-1 NUM_ROWS " _
                    + " From ALL_VIEWS " _
                    + " WHERE OWNER=USER) " _
                    + " ORDER BY TABLE_NAME "

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

        Public Overrides Function DetectTableType(table As String) As ObjectType
            CreateConnection()

            Dim sql As String = "SELECT OBJECT_TYPE FROM ALL_OBJECTS WHERE OWNER=USER AND OBJECT_NAME=" + table.QuotedStr()

            Logger.SaveLog(LogTag, sql)
            If _db IsNot Nothing Then

                _executionStatus = ExecutionStatus.Executing
                Try
                    Dim dad As DbDataAdapter = SaDataTableFN.CreateAdapter(sql, _db)
                    Dim r = dad.SelectCommand.ExecuteScalar()

                    If r IsNot Nothing Then
                        Return IIf(r.ToString().ToUpper() = "TABLE", ObjectType.otTable, ObjectType.otView)
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
            CreateConnection()
            sql = sql.Trim()
            sql = "Explain plan for " + sql
            Logger.SaveLog(LogTag, sql)
            If _db IsNot Nothing Then
                Dim dtb As SaDataTableFN

                _executionStatus = ExecutionStatus.Executing
                Try
                    Dim dbCommand As DbCommand = _db.CreateCommand()
                    dbCommand.CommandText = sql
                    dbCommand.ExecuteNonQuery()

                    sql = "SELECT * FROM TABLE(DBMS_XPLAN.DISPLAY)"

                    dtb = New SaDataTableFN(sql, _db)
                    dtb.FillByDataAdapter(True)

                    Return dtb
                Finally
                    _executionStatus = ExecutionStatus.Idle
                End Try
            End If
            DBMessage(Me, "No Connection")
            Return Nothing
        End Function

        Public Overrides Function GetIndexes(ByVal table As SaTableSettings) As SaDataTableFN
            CreateConnection()
            Dim sql As String = "SELECT * FROM USER_IND_COLUMNS WHERE TABLE_NAME=:TABLE_NAME"
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

        Public Overrides Function GetColumns(ByVal table As SaTableSettings) As SaDataTableFN
            CreateConnection()
            Dim sql As String = "SELECT * FROM USER_TAB_COLUMNS WHERE TABLE_NAME=:TABLE_NAME"
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


        Public Overrides Function GetColumnsTypeCollection(ByVal table As SaTableSettings) As List(Of SaDBColumnType)
            CreateConnection()
            Try
                If _db IsNot Nothing Then
                    Dim dtb As SaDataTableFN = GetColumns(table)
                    If dtb.Rows.Count > 0 Then
                        Dim dataCollection As List(Of SaDBColumnType) = New List(Of SaDBColumnType)()
                        For Each row In dtb.Rows
                            dataCollection.Add(New SaOracleDBColumnType(row("COLUMN_NAME"), row("DATA_TYPE")))
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