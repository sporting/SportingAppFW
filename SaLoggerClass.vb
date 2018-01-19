'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************

Imports System.IO
Imports SportingAppFW.Data.Common

Namespace Logger
    Public Class SaLoggerClass
        Private Shared _instance As SaLoggerClass
        Private Shared _lock As Object = New Object()

        Dim _path As String

        Private Sub New(Optional ByVal path As String = "")
            _path = path
        End Sub

        Public Overloads Sub SaveLog(ByVal catelog As String, ByVal log As String, Optional ByVal eventype As TraceEventType = TraceEventType.Information)
            SaveLog(String.Format("[{0}] {1}", catelog, log), eventype)
        End Sub

        Public Overloads Sub SaveLog(ByVal log As String, Optional ByVal eventype As TraceEventType = TraceEventType.Information)
            Dim filepath As String = Path.Combine(_path, FRAMEWORK_NAMESPACE_LOG) '指定Log資料夾的路徑與應用程式的路徑相同
            If Not Directory.Exists(filepath) Then
                Directory.CreateDirectory(filepath)
            End If

            With My.Application.Log.DefaultFileLogWriter
                .AutoFlush = True
                '.IncludeHostName = True
                .BaseFileName = "Log"    'Log檔的檔名
                .CustomLocation = filepath   '自訂Log檔的存放路徑
                .AutoFlush = True        'Log檔寫完後自動清除緩衝
                .LogFileCreationSchedule = Logging.LogFileCreationScheduleOption.Daily '設定一天產生一個Log檔
            End With

            My.Application.Log.WriteEntry(String.Format("{0}: {1}", FormatDateTime(Now, DateFormat.GeneralDate), log), eventype)

            '#If DEBUG Then
            Console.WriteLine(log)
            '#End If
        End Sub

        Public Shared Function GetInstance(Optional ByVal path As String = ".") As SaLoggerClass
            If _instance Is Nothing Then
                SyncLock _lock
                    If _instance Is Nothing Then
                        _instance = New SaLoggerClass(path)
                    End If
                End SyncLock
            End If
            Return _instance
        End Function

    End Class

End Namespace