Imports System.Data.Common
Imports System.Data.SQLite
Imports SportingAppFW.Data.Common.SaEnum
Imports SportingAppFW.Extensions
Imports SportingAppFW.SaWindows.Data
Imports SportingAppFW.Tools

Namespace Data.Common.DB.Sqlite
    Public Class SaSQLiteDBClass
        Inherits SaBaseDBClass

        Private _clock As SaUseTimeClass = New SaUseTimeClass()
        Private _fileName As String = String.Empty
        Public ReadOnly Property FileName As String
            Get
                Return _fileName
            End Get
        End Property

        Private _displayFileName As String = String.Empty
        Public ReadOnly Property DisplayFileName As String
            Get
                Return _displayFileName
            End Get
        End Property

        Public Overrides ReadOnly Property SimpleConnectionString() As String
            Get
                Return DisplayFileName
            End Get
        End Property

        Protected Sub New()
            LogTag = "SaSQLiteDBClass"

            AddHandler DBClosedHandler, AddressOf AfterDBClose
        End Sub

        Private Sub AfterDBClose()
            GC.Collect()
        End Sub

        Public Sub New(ByVal fileName As String, Optional ByVal auto_commit As Boolean = False)
            Me.New(fileName, fileName, auto_commit)
        End Sub


        Public Sub New(ByVal fileName As String, ByVal disFileName As String, Optional ByVal auto_commit As Boolean = False)
            Me.New(String.Format("{0}={1};Version=3", "Data source", fileName))
            _fileName = fileName
            _displayFileName = disFileName

            Logger.SaveLog(LogTag, String.Format("{0} {1}", "Database Connect to: ", fileName))

            _autoCommit = auto_commit
        End Sub

        Private Sub New(ByVal connection_string As String)
            Me.New()
            _db = New SQLiteConnection(connection_string)
            CreateConnection()
        End Sub

        Public Overloads Overrides Function ExecuteSQL(ByVal sql As String, ByVal row As List(Of SaDBParameter), Optional ByVal allDataLoad As Boolean = False) As SaDataTableFN
            CreateConnection()
            sql = sql.Trim()
            ' Logger.SaveLog(LogTag, sql)
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

        Protected Overloads Overrides Sub CreateConnection()
            _connectStatus = ConnectStatus.Connecting
            Try
                DatabaseType = eDatabaseType.SQLite

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

        Public Overrides Function GetDDL(ByVal table As SaTableSettings) As String
            CreateConnection()

            Dim sql As String = "SELECT SQL FROM SQLITE_MASTER WHERE TYPE='table' AND NAME=" + table.TableName.QuotedStr()

            '  Logger.SaveLog(LogTag, sql)
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

            Dim sql As String = "SELECT TBL_NAME AS TABLE_DESC,TBL_NAME AS TABLE_NAME,-1 AS NUM_ROWS FROM SQLITE_MASTER WHERE TYPE='table' ORDER BY TABLE_NAME"

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

        Public Overrides Function DetectTableType(table As String) As ObjectType
            CreateConnection()

            Dim sql As String = "Select TYPE FROM SQLITE_MASTER WHERE NAME=" + table.QuotedStr()

            '   Logger.SaveLog(LogTag, sql)
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
            sql = "Explain query plan " + sql
            ' Logger.SaveLog(LogTag, sql)
            If _db IsNot Nothing Then
                Dim dtb As SaDataTableFN

                _executionStatus = ExecutionStatus.Executing
                Try
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
            Dim sql As String = "Select * FROM SQLITE_MASTER WHERE TBL_NAME=:TABLE_NAME AND TYPE='index'"
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

        Public Overrides Function GetColumns(ByVal table As SaTableSettings) As SaDataTableFN
            CreateConnection()
            Dim sql As String = "PRAGMA table_info(" + table.TableName + ")"
            'Dim params As List(Of SaDBParameter) = New List(Of SaDBParameter)()
            'params.Add(New SaDBParameter(Type.GetType("System.String"), "TABLE_NAME", table.TableName))
            '    Logger.SaveLog(LogTag, sql)
            If _db IsNot Nothing Then
                Dim dtb As SaDataTableFN
                _executionStatus = ExecutionStatus.Executing
                Try
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

        Public Overrides Function GetColumnsTypeCollection(ByVal table As SaTableSettings) As List(Of SaDBColumnType)
            CreateConnection()
            Try
                If _db IsNot Nothing Then
                    Dim dtb As SaDataTableFN = GetColumns(table)
                    If dtb.Rows.Count > 0 Then
                        Dim dataCollection As List(Of SaDBColumnType) = New List(Of SaDBColumnType)()
                        For Each row In dtb.Rows
                            dataCollection.Add(New SaSQLiteDBColumnType(row("name"), row("type")))
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