'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Data.SQLite
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms
Imports NPOI.HSSF.UserModel
Imports NPOI.SS.UserModel
Imports SportingAppFW.Data.Common
Imports SportingAppFW.SaSystem
Imports SportingAppFW.Tools.SaUtility

Namespace Extensions
    Public Module SaDataGridViewExtension
        <Extension()>
        Public Function ExportToCSV(dgv As DataGridView, ByVal strExportFileName As String, ByVal blnWriteColumnHeaderNames As Boolean, ByVal strDelimiterType As String) As Boolean
            Try
                Dim sr As StreamWriter = IO.File.CreateText(strExportFileName)
                Dim strDelimiter As String = strDelimiterType
                Dim intColumnCount As Integer = 0
                For Each col As DataGridViewColumn In dgv.Columns
                    If col.Visible Then
                        intColumnCount += 1
                    End If
                Next
                Dim strRowData As String = ""
                If blnWriteColumnHeaderNames Then
                    For intX As Integer = 0 To intColumnCount - 1
                        If dgv.Columns(intX).Visible Then
                            strRowData += Replace(dgv.Columns(intX).Name, strDelimiter, "") & IIf(intX < intColumnCount, strDelimiter, "")
                        End If
                    Next intX
                    sr.WriteLine(strRowData)
                End If
                For intX As Integer = 0 To dgv.Rows.Count - 1
                    strRowData = ""
                    For intRowData As Integer = 0 To intColumnCount - 1
                        If dgv.Columns(intRowData).Visible Then
                            strRowData += Replace(NVL(dgv.Rows(intX).Cells(intRowData).Value, String.Empty), strDelimiter, "") &
                    IIf(intRowData < intColumnCount, strDelimiter, "")
                        End If
                    Next intRowData
                    sr.WriteLine(strRowData)
                Next intX
                sr.Close()
                Return True
            Catch ex As Exception
                Return False
            End Try
            Return True
        End Function

        <Extension()>
        Public Function ExportToExcel(dgv As DataGridView, ByVal strExportFileName As String, ByVal sheetName As String) As Boolean
            Try
                '建立Excel 2003檔案
                Dim wb As IWorkbook = New HSSFWorkbook()
                Dim ws As ISheet

                '建立Excel 2007檔案
                ws = wb.CreateSheet(sheetName)

                ws.CreateRow(0) '第一行為欄位名稱
                Dim celli As Integer = 0
                For i As Integer = 0 To dgv.Columns.Count - 1
                    If dgv.Columns(i).Visible Then
                        ws.GetRow(0).CreateCell(celli).SetCellValue(dgv.Columns(i).Name)
                        celli += 1
                    End If
                Next

                For i As Integer = 0 To dgv.Rows.Count - 1
                    ws.CreateRow(i + 1)
                    celli = 0
                    For j As Integer = 0 To dgv.Columns.Count - 1
                        If dgv.Columns(j).Visible Then
                            ws.GetRow(i + 1).CreateCell(j).SetCellValue(dgv.Rows(i).Cells(j).Value.ToString())
                            celli += 1
                        End If
                    Next
                Next
                Using f As FileStream = New FileStream(strExportFileName, FileMode.Create)
                    wb.Write(f)
                End Using
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Export To Sqlite using SaTable, SaFields to implement
        ''' </summary>
        ''' <param name="dgv"></param>
        ''' <param name="strExportFileName"></param>
        ''' <returns></returns>
        <Extension()>
        Public Function ExportToSQLite(dgv As DataGridView, ByVal strExportFileName As String) As Boolean
            If Not IO.File.Exists(strExportFileName) Then
                SQLiteConnection.CreateFile(strExportFileName)
            End If

            Dim tableName As String = "EXPORT_DATA"

            Dim connection_string As String = String.Format("{0}={1};Version=3", "Data source", strExportFileName)
            Dim db As SQLiteConnection = New SQLiteConnection(connection_string)

            Dim obj As Object = GenerateSaFieldsType(dgv)
            Dim saf As SaFields = GenerateSaFields(Nothing, obj, dgv, 0)

            Dim sat As SaTable = New SaTable(db, tableName, saf)

            sat.CreateTableIfNotExists(saf)

            'Transaction will cause outOfMemoryException while the data is too large. One process has max 2GB memory on 32bit mode
            '  Dim transaction As SQLiteTransaction = db.BeginTransaction()
            Try
                Dim affectRows As Integer = 0
                For intX As Integer = 0 To dgv.Rows.Count - 1
                    saf = GenerateSaFields(saf, obj, dgv, intX)
                    affectRows = sat.InsertRow(saf)

                    If affectRows < 0 Then
                        Exit For
                    End If
                Next intX

                If affectRows < 0 Then
                    'transaction.Rollback()

                    Return False
                Else
                    'transaction.Commit()

                    Return True
                End If

            Catch ex As Exception
                'ransaction.Rollback()

                Return False
            Finally
                db.Close()
            End Try

            ' To Release SQLite db file lock
            GC.Collect()
        End Function

        Private Function GenerateSaFieldsType(dgv As DataGridView) As Object
            Dim o As SaObjectBuilder = New SaObjectBuilder()

            Dim Fields As List(Of SaField) = New List(Of SaField)()

            For Each col As DataGridViewColumn In dgv.Columns
                Dim f As SaField = New SaField()
                f.FieldName = col.Name
                If col.ValueType.Equals(GetType(Integer)) Then
                    f.FieldType = GetType(Integer)
                Else
                    f.FieldType = GetType(String)
                End If
                Fields.Add(f)
            Next

            Dim newObj As Object = o.CreateNewObject(Fields, GetType(SaFields))

            Return newObj
        End Function

        Private Function GenerateSaFields(ByRef saf As SaFields, obj As Object, dgv As DataGridView, rowNum As Integer) As SaFields
            Dim t As Type = obj.GetType()

            Dim instance As SaFields = saf

            If saf Is Nothing Then
                instance = Activator.CreateInstance(t)
            End If

            instance.ClearFieldAttribute()

            Dim props() As PropertyInfo = instance.GetType.GetProperties()
            Dim instancePropsCount As Integer = props.Count
            Dim value As String = String.Empty

            For i As Integer = 0 To instancePropsCount - 1
                Dim fieldName As String = props(i).Name
                Dim mInfo As MemberInfo() = Nothing
                Dim pInfo As PropertyInfo = obj.GetType.GetProperty(fieldName)

                If pInfo IsNot Nothing Then
                    value = String.Empty
                    If dgv.Columns.Item(fieldName) IsNot Nothing Then
                        value = dgv.Item(fieldName, rowNum).Value.ToString()
                    End If

                    mInfo = t.GetMember(fieldName)

                    If (value IsNot Nothing And mInfo IsNot Nothing And Not String.IsNullOrEmpty(mInfo(0).ToString())) Then
                        SaObjectBuilder.SetMemberValue(mInfo(0), instance, value)
                    End If
                Else
                    mInfo = t.GetMember(fieldName)

                    If (mInfo IsNot Nothing And Not String.IsNullOrEmpty(mInfo(0).ToString())) Then
                        SaObjectBuilder.SetMemberValue(mInfo(0), instance, Nothing)
                    End If
                End If
            Next
            'objList.Add(instance)

            For Each pro As PropertyInfo In instance.GetType.GetProperties()
                If pro.CanWrite Then
                    instance.AddSaUIFieldAttribute(pro, New SaUIFieldsAttribute(pro.Name, FilterFieldEnum.FilterField))
                    instance.AddSaFieldAttribute(pro, New SaFieldsAttribute())
                End If
            Next

            Return instance
        End Function
    End Module
End Namespace
