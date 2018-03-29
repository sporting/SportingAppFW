
Imports System.Data.SQLite
Imports System.IO
Imports System.Reflection
Imports NPOI.HSSF.UserModel
Imports NPOI.SS.UserModel
Imports NPOI.XSSF.UserModel
Imports SportingAppFW.Data.Common
Imports SportingAppFW.Data.Common.SaEnum
Imports SportingAppFW.SaSystem

Namespace Extensions
    Public Module SaExcelUtility

        Public Function ToSQLite(inputFileName As String, outputFileName As String, Optional firstRowIsHead As Boolean = True, Optional excelVersion As eExcelVerEnum = eExcelVerEnum.evXLS) As Boolean
            If Not File.Exists(outputFileName) Then
                SQLiteConnection.CreateFile(outputFileName)
            End If

            Dim connection_string As String = String.Format("{0}={1};Version=3", "Data source", outputFileName)
            Dim db As SQLiteConnection = New SQLiteConnection(connection_string)

            Dim obj As Object
            Dim saf As SaFields
            Dim sat As SaTable
            Dim headerCols As List(Of SaField) = New List(Of SaField)()
            Dim cellCols As List(Of SaField) = New List(Of SaField)()
            Dim headerRow As IRow
            Dim iStartRows As Integer = 0
            Dim affectRows As Integer = 0
            Dim o As SaObjectBuilder = New SaObjectBuilder()

            Using fs As FileStream = New FileStream(inputFileName, FileMode.Open, FileAccess.Read)
                Dim wb As IWorkbook

                If excelVersion = eExcelVerEnum.evXLSX Then
                    wb = New XSSFWorkbook(fs)
                Else
                    wb = New HSSFWorkbook(fs)
                End If

                Dim iSht As ISheet = Nothing
                For i As Integer = 0 To wb.NumberOfSheets() - 1
                    Try
                        iSht = wb.GetSheetAt(i)

                        headerRow = iSht.GetRow(0)

                        headerCols.Clear()
                        For iCellNum As Integer = headerRow.FirstCellNum To headerRow.LastCellNum - 1
                            If firstRowIsHead Then
                                '第一欄表頭一定是String，除非檢查欄位下每個資料的屬性
                                headerCols.Add(New SaField(headerRow.GetCell(iCellNum).StringCellValue, GetType(String), Nothing))
                            Else
                                headerCols.Add(New SaField("F" + iCellNum, GetType(String), Nothing))
                            End If
                        Next


                        obj = o.CreateNewObject(headerCols, GetType(SaFields))
                        saf = GenerateSaFields(Nothing, obj, headerCols)

                        sat = New SaTable(db, iSht.SheetName, saf)
                        sat.CreateTableIfNotExists(saf)

                        iStartRows = iSht.FirstRowNum + IIf(firstRowIsHead, 1, 0)

                        For iRows As Integer = iStartRows To iSht.LastRowNum
                            cellCols.Clear()

                            For iCellNum As Integer = headerRow.FirstCellNum To headerRow.LastCellNum - 1
                                If iSht.GetRow(iRows).GetCell(iCellNum).CellType = CellType.Numeric Then
                                    If DateUtil.IsCellDateFormatted(iSht.GetRow(iRows).GetCell(iCellNum)) Then
                                        cellCols.Add(New SaField(headerRow.GetCell(iCellNum).StringCellValue, GetType(Integer), iSht.GetRow(iRows).GetCell(iCellNum).DateCellValue))
                                    Else
                                        cellCols.Add(New SaField(headerRow.GetCell(iCellNum).StringCellValue, GetType(Integer), iSht.GetRow(iRows).GetCell(iCellNum).NumericCellValue))
                                    End If
                                Else
                                    cellCols.Add(New SaField(headerRow.GetCell(iCellNum).StringCellValue, GetType(String), iSht.GetRow(iRows).GetCell(iCellNum).StringCellValue))
                                End If
                            Next

                            saf = GenerateSaFields(saf, obj, cellCols)
                            affectRows = sat.InsertRow(saf)

                            If affectRows < 0 Then
                                Exit For
                            End If
                        Next


                        If affectRows < 0 Then
                            Return False
                        End If
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                Next

                Return True
            End Using
        End Function


        Public Function GenerateSaFields(ByVal saf As SaFields, obj As Object, fields As List(Of SaField)) As SaFields
            Dim t As Type = obj.GetType()

            Dim instance As SaFields = saf

            If saf Is Nothing Then
                instance = Activator.CreateInstance(t)
            End If

            instance.ClearFieldAttribute()

            Dim props() As PropertyInfo = instance.GetType.GetProperties()
            Dim instancePropsCount As Integer = props.Count
            Dim value As String
            Dim f As SaField

            For i As Integer = 0 To instancePropsCount - 1
                Dim fieldName As String = props(i).Name
                Dim mInfo As MemberInfo() = Nothing
                Dim pInfo As PropertyInfo = obj.GetType.GetProperty(fieldName)

                If pInfo IsNot Nothing Then
                    value = String.Empty
                    f = fields.Find(Function(x) x.FieldName.Equals(fieldName))
                    If f IsNot Nothing Then
                        value = f.Value
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