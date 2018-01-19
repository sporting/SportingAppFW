'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.IO
Imports System.Security.Permissions
Imports SportingAppFW.Extensions.SaStringExtension

Namespace Tools

    Public Module SaProcessOps
        ''' <summary>
        ''' 呼叫外部程式
        ''' </summary>
        ''' <param name="program">程式名稱</param>
        ''' <param name="argvs">傳入參數</param>
        ''' <param name="workingDir">工作目錄</param>
        ''' <param name="hidden">隱藏視窗</param>
        ''' <param name="useShell">使用作業系統 Shell</param>
        ''' <param name="waitProcessFinish">要不要等候 Process 結束</param>
        ''' <param name="waitTimeOut">設定 timeout 時間(毫秒) (-1 表示無限 wait)</param>
        ''' <returns>process 成功/失敗</returns>
        <PermissionSet(SecurityAction.LinkDemand)>
        Public Function CallExternalProgram(ByVal program As String, Optional ByVal argvs As String() = Nothing, Optional ByVal workingDir As String = "", Optional ByVal hidden As Boolean = False, Optional ByVal useShell As Boolean = True, Optional ByVal waitProcessFinish As Boolean = False, Optional ByVal waitTimeOut As Integer = -1) As Boolean
            Dim argv_string As String = String.Empty
            If argvs IsNot Nothing Then
                argv_string = String.Join(" ", argvs.ToArray())
            End If

            Dim startInfo As New ProcessStartInfo(program)
            If hidden Then
                startInfo.WindowStyle = ProcessWindowStyle.Hidden
                startInfo.CreateNoWindow = True
            End If
            startInfo.Arguments = argv_string

            If workingDir.IsEmpty Then
                workingDir = Path.GetDirectoryName(program)
            End If

            startInfo.WorkingDirectory = workingDir
            startInfo.UseShellExecute = useShell

            Dim p As Process = Process.Start(startInfo)

            If waitProcessFinish Then
                'Wait for the process window to complete loading.
                'p.WaitForInputIdle()
                'Wait for the process to exit.
                If waitTimeOut > 0 Then
                    p.WaitForExit(waitTimeOut)
                    'HasExited is true if the application closed before the time-out.
                    If p.HasExited = False Then
                        'Process is still running.
                        'Test to see if process is hung up.
                        If p.Responding Then
                            'Process was responding; close the main window.
                            p.CloseMainWindow()
                        Else
                            'Process was not responding; force the process to close.
                            p.Kill()
                        End If
                        Return False
                    End If
                Else
                    p.WaitForExit()
                End If
            End If

            Return True
        End Function
    End Module
End Namespace
