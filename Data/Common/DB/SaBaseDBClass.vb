Imports System.ComponentModel
Imports System.Data.Common
Imports System.Text.RegularExpressions
Imports SportingAppFW.Data.Common.SaEnum
Imports SportingAppFW.Extensions
Imports SportingAppFW.SaSystem.Logger
Imports SportingAppFW.SaWindows.Data

Namespace Data.Common.DB
    Public MustInherit Class SaBaseDBClass

        Protected Friend _db As DbConnection
        Protected Friend _transaction As DbTransaction = Nothing
        Protected Friend _autoCommit As Boolean = False
        Protected Friend _connectStatus As ConnectStatus = ConnectStatus.NotConnect
        Private _logger As SaLoggerClass = SaLoggerClass.GetInstance(WorkingDirectory)

        Private _dbType As eDatabaseType

        Private _logTag As String = "SaBaseDBClass"

        Public ReadOnly Property db As DbConnection
            Get
                Return _db
            End Get
        End Property


        Public Event DBMessageHandler As DBStatusMessage
        Protected Friend Sub DBMessage(ByVal sender As Object, ByVal msg As String)
            RaiseEvent DBMessageHandler(sender, msg)
        End Sub

        Public Property DatabaseType() As eDatabaseType
            Get
                Return _dbType
            End Get
            Set(value As eDatabaseType)
                _dbType = value
            End Set
        End Property

        Protected Friend Overridable Property LogTag As String
            Get
                Return _logTag
            End Get
            Set(value As String)
                _logTag = value
            End Set
        End Property

        Protected Friend ReadOnly Property Logger() As SaLoggerClass
            Get
                Return _logger
            End Get
        End Property
        Public ReadOnly Property AutoCommit As Boolean
            Get
                Return _autoCommit
            End Get
        End Property

        ''' <summary>
        ''' DB Connection status
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum ConnectStatus As Integer
            FailConnect
            NotConnect
            Connecting
            Connected
        End Enum

        Public Enum ExecutionStatus As Integer
            Idle
            Executing
        End Enum

        Public Function Clone() As Object
            Return MemberwiseClone() 'Shallow copy  'only non-static property can be copy
        End Function

        Public Event DBOpenedHandler As DBOpenedMessage
        Public Sub DBOpen()
            If _db IsNot Nothing Then
                If _db.State <> ConnectionState.Open Then
                    _db.Open()

                    RaiseEvent DBOpenedHandler()
                End If
            End If
        End Sub

        Public Event DBClosingHandler As DBClosingMessage
        Public Event DBClosedHandler As DBClosedMessage

        Public Sub DBClose()
            If _db IsNot Nothing Then
                If _db.State <> ConnectionState.Closed Then

                    Dim e As CancelEventArgs = New CancelEventArgs()
                    RaiseEvent DBClosingHandler(e)

                    If Not e.Cancel Then
                        _db.Close()
                    End If
                End If

                RaiseEvent DBClosedHandler()
            End If
        End Sub

        Public Sub StartTransaction()
            If _db IsNot Nothing Then
                If _transaction Is Nothing Then
                    '   Logger.SaveLog(_logTag, "Start Transaction")
                    _transaction = _db.BeginTransaction(IsolationLevel.ReadCommitted)
                End If
            End If
        End Sub

        Public Sub Commit()
            If _db IsNot Nothing Then
                If _transaction IsNot Nothing Then
                    '  Logger.SaveLog(_logTag, "Start Commit")
                    _transaction.Commit()
                    _transaction = Nothing
                End If
            End If
        End Sub

        Public Sub Rollback()
            If _db IsNot Nothing Then
                If _transaction IsNot Nothing Then
                    ' Logger.SaveLog(_logTag, "Start Rollback")
                    _transaction.Rollback()
                    _transaction = Nothing
                End If
            End If
        End Sub


        Public ReadOnly Property DBConnectStatus() As ConnectStatus
            Get
                Return _connectStatus
            End Get
        End Property

        Protected Friend _executionStatus As ExecutionStatus = ExecutionStatus.Idle

        Public ReadOnly Property SQLExecuteStatus() As ExecutionStatus
            Get
                Return _executionStatus
            End Get
        End Property


        Public ReadOnly Property IsInTransaction() As Boolean
            Get
                Return _transaction IsNot Nothing
            End Get
        End Property

        Public Overridable ReadOnly Property SimpleConnectionString() As String
            Get
                Return String.Empty
            End Get
        End Property

        Public Shared Function IsSelectSQL(ByVal sql As String) As Boolean
            If sql.IsEmpty Then
                Return False
            End If

            sql = sql.Trim()

            Dim pattern As String = "^select[\w|\W]*$"
            Dim r As Regex = New Regex(pattern, RegexOptions.IgnoreCase)
            Dim m As Match = r.Match(sql)

            Return m.Success
        End Function

        Public Shared Function IsCommitSQL(ByVal sql As String) As Boolean
            If sql.IsEmpty Then
                Return False
            End If

            sql = sql.Trim()

            Dim pattern As String = "^commit$"
            Dim r As Regex = New Regex(pattern, RegexOptions.IgnoreCase)
            Dim m As Match = r.Match(sql)

            Return m.Success
        End Function

        Public Shared Function IsRollbackSQL(ByVal sql As String) As Boolean
            If sql.IsEmpty Then
                Return False
            End If

            sql = sql.Trim()

            Dim pattern As String = "^rollback$"
            Dim r As Regex = New Regex(pattern, RegexOptions.IgnoreCase)
            Dim m As Match = r.Match(sql)

            Return m.Success
        End Function

        Protected MustOverride Sub CreateConnection()

        Public MustOverride Function GetIndexes(ByVal table As SaTableSettings) As SaDataTableFN

        Public MustOverride Function GetColumns(ByVal table As SaTableSettings) As SaDataTableFN

        Public MustOverride Function GetColumnsTypeCollection(ByVal table As SaTableSettings) As List(Of SaDBColumnType)

        Public MustOverride Function ExecutionPlan(sql As String) As SaDataTableFN

        Public MustOverride Function DetectTableType(table As String) As ObjectType

        Public MustOverride Function GetTables() As SaDataTableFN

        Public MustOverride Function GetDDL(ByVal table As SaTableSettings) As String

        Public MustOverride Overloads Function ExecuteSQL(ByVal sql As String, Optional ByVal allDataLoad As Boolean = False) As SaDataTableFN

        Public MustOverride Overloads Function ExecuteSQL(ByVal sql As String, ByVal row As List(Of SaDBParameter), Optional ByVal allDataLoad As Boolean = False) As SaDataTableFN

        Public Overridable Overloads Function ExecuteSQLFetchFirstData(ByVal sql As String) As Object
            Dim dtb As SaDataTableFN = ExecuteSQL(sql)
            If dtb IsNot Nothing Then
                If dtb.Rows.Count > 0 Then
                    Return dtb.Rows(0).Item(0)
                End If
            End If

            Return Nothing
        End Function

        Public Overridable Overloads Function ExecuteSQLFetchFirstData(ByVal sql As String, ByVal row As List(Of SaDBParameter)) As Object
            Dim dtb As SaDataTableFN = ExecuteSQL(sql, row, True)
            If dtb IsNot Nothing Then
                If dtb.Rows.Count > 0 Then
                    Return dtb.Rows(0).Item(0)
                End If
            End If

            Return Nothing
        End Function

        Public Overridable Overloads Function ExecuteSQLFetchFirstRow(ByVal sql As String, ByVal row As List(Of SaDBParameter)) As DataRow
            Dim dtb As SaDataTableFN = ExecuteSQL(sql, row, True)
            If dtb IsNot Nothing Then
                If dtb.Rows.Count > 0 Then
                    Return dtb.Rows(0)
                End If
            End If

            Return Nothing
        End Function

        Public Overridable Overloads Function ExecuteSQLFetchFirstRow(ByVal sql As String) As DataRow
            Dim dtb As SaDataTableFN = ExecuteSQL(sql, True)
            If dtb IsNot Nothing Then
                If dtb.Rows.Count > 0 Then
                    Return dtb.Rows(0)
                End If
            End If

            Return Nothing
        End Function
    End Class

End Namespace