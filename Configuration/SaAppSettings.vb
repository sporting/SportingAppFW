Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Linq
Imports System.Text


Namespace Configuration
    Public Class SaAppSettings
        Public Shared Function GetAppSetting(Of T)(ByVal key As String, ByVal defaultValue As T) As T
            Dim val As Object = ConfigurationManager.AppSettings(key)

            If val Is Nothing Then
                Return defaultValue
            End If

            Return CType(val, T)
        End Function

        Public Shared Function GetAppSetting(Of T)(ByVal key As String) As T
            Dim val As Object = ConfigurationManager.AppSettings(key)

            If val Is Nothing Then
                Return Nothing
            End If

            Return CType(val, T)
        End Function

        Public Shared Function GetAppSection(Of T)(ByVal section As String) As T
            Dim val As Object = ConfigurationManager.GetSection(section)

            If val Is Nothing Then
                Return Nothing
            End If

            Return CType(val, T)
        End Function

        Public Shared Function GetConnectionSettings(ByVal name As String) As ConnectionStringSettings
            Return ConfigurationManager.ConnectionStrings(name)
        End Function
    End Class
End Namespace
