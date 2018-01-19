Imports System.Data.Common
Imports SportingAppFW.Data.Common.SaEnum

Namespace Components
    Public MustInherit Class SaDBConnection
        Inherits DbConnection
        Public Shared Function DetectDatabaseType(ByRef dbconn As DbConnection) As eDatabaseType
            If dbconn Is Nothing Then
                Exit Function
            End If

            Dim str As String = Strings.UCase(dbconn.[GetType]().Name.ToString())
            If (String.Compare(str, "SQLCONNECTION", False) = 0) Then
                Return eDatabaseType.MSSql
            ElseIf (String.Compare(str, "ORACLECONNECTION", False) = 0) Then
                Return eDatabaseType.Oracle
            ElseIf (String.Compare(str, "SQLITECONNECTION", False) = 0) Then
                Return eDatabaseType.SQLite
            Else
                Return eDatabaseType.UnKnown
            End If
        End Function
    End Class

End Namespace