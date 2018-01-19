'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.IO
Imports SportingAppFW.Data.Common

Namespace Tools
    Public Module SaFileUtility
        Function SyncFilesInDirectory(ByVal strDirectory As String, ByVal destDirectory As String, ByVal Optional includeSubDirectory As Boolean = True) As Boolean
            If Not Directory.Exists(strDirectory) Then
                Return False
            End If

            Try
                If Not Directory.Exists(destDirectory) Then
                    Directory.CreateDirectory(destDirectory)
                End If

                Dim targetpath As String
                Dim result As Boolean = True

                For Each file As String In Directory.GetFiles(strDirectory)
                    targetpath = Path.Combine(destDirectory, Path.GetFileName(file))

                    result = result And SyncFile(file, targetpath)
                Next


                If includeSubDirectory Then
                    Dim subDir As String
                    For Each dir As String In Directory.GetDirectories(strDirectory)
                        subDir = Path.GetDirectoryName(dir)
                        targetpath = Path.Combine(destDirectory, subDir)
                        result = result + SyncFilesInDirectory(dir, targetpath, includeSubDirectory)
                    Next
                End If

                Return result
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Return False
            End Try

        End Function

        Function SyncFile(ByVal srcFullFileName As String, ByVal destFullFileName As String) As Boolean
            If Not File.Exists(srcFullFileName) Then
                Return False
            End If

            Try
                Dim destDirectory As String = Path.GetDirectoryName(destFullFileName)

                If Not Directory.Exists(destDirectory) Then
                    Directory.CreateDirectory(destDirectory)
                End If

                'If forceCopy Then
                '    If File.Exists(targetpath) Then
                '        File.Delete(targetpath)
                '    End If
                'End If

                If File.Exists(destFullFileName) Then
                    Dim sourceMD5 As Byte() = FileMD5(srcFullFileName)
                    Dim targetMD5 As Byte() = FileMD5(destFullFileName)

                    If sourceMD5.SequenceEqual(targetMD5) Then
                        Return True
                    End If
                End If

                If File.Exists(destFullFileName) Then
                    File.Delete(destFullFileName)
                End If
                File.Copy(srcFullFileName, destFullFileName, True)
                Return True
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Return False
            End Try
        End Function

        'Caution!!!!!
        Sub RemoveAllFilesWithExtension(dir As String, ext As String)
            If Not Directory.Exists(dir) Then
                Try
                    For Each f As String In Directory.GetFiles(dir)
                        If Path.GetExtension(f) = ext Then
                            File.Delete(f)
                        End If
                    Next
                Catch ex As Exception

                End Try
            End If
        End Sub
    End Module

End Namespace