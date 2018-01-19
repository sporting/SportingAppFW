'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Runtime.CompilerServices
Imports System.Reflection

Namespace Extensions
    ''' <summary>
    ''' DataTable to List(Of ORM Entity Class)
    ''' </summary>
    Public Module SaDataTableExtensionM
        <Extension()>
        Public Function ToList(Of T As Class)(ByVal dtb As DataTable) As List(Of T)
            Dim result As List(Of T) = New List(Of T)()
            Dim obj As T
            Dim type As Type = GetType(T)

            For Each row As DataRow In dtb.Rows
                obj = GetType(T).GetConstructor(New System.Type() {}).Invoke(New Object() {})
                For Each p As PropertyInfo In type.GetProperties()
                    If p.CanRead() Then
                        For Each col As DataColumn In dtb.Columns
                            If col.ColumnName = p.Name Then
                                p.SetValue(obj, row(col), Nothing)
                                Exit For
                            End If
                        Next
                    End If
                Next
                result.Add(obj)
            Next

            Return result
        End Function

        <Extension()>
        Public Function ToDataTableFN(ByVal dtb As DataTable) As Components.Data.SaDataTableFN
            'control privilege transfer to DataTableFN
            Return Components.Data.SaDataTableFN.ToDataTableFN(dtb)
        End Function

        ''' <summary>
        ''' Implement row to column datatable
        ''' 實現行轉列
        ''' </summary>
        ''' <param name="dtb">source datatable</param>
        ''' <param name="beHeaderColumn">column value to be new column's header</param>
        ''' <param name="beValueColumn">column value to be new column's value</param>
        ''' <returns></returns>
        <Extension()>
        Public Function RowToColumn(ByVal dtb As DataTable, ByVal beHeaderColumn As DataColumn, ByVal beValueColumn As DataColumn) As DataTable
            Dim new_dt As DataTable = New DataTable()
            new_dt.Columns.Clear()
            Try
                For Each col As DataColumn In dtb.Columns
                    If col.ColumnName <> beHeaderColumn.ColumnName _
                    And col.ColumnName <> beValueColumn.ColumnName Then
                        new_dt.Columns.Add(New DataColumn(col.ColumnName, col.DataType, col.Expression, col.ColumnMapping))
                    End If
                Next

                Dim query = From row As DataRow In dtb.Rows
                            Select row(beHeaderColumn)
                            Distinct

                ' add new column to new datatable except beHeaderColumn and beValueColumn 
                For Each cell As Object In query.OrderBy(Of Object)(Function(r) r.ToString())
                    If Not cell.ToString().IsEmpty Then
                        new_dt.Columns.Add(New DataColumn(dtb.NewColumnNameBySeq(cell.ToString(), cell.ToString(), 1), beValueColumn.DataType, beValueColumn.Expression, beValueColumn.ColumnMapping))
                    End If
                Next

                For Each row As DataRow In dtb.Rows
                    Dim filterStr As String = String.Empty
                    For Each col As DataColumn In row.Table.Columns
                        If col.ColumnName <> beHeaderColumn.ColumnName _
                            And col.ColumnName <> beValueColumn.ColumnName Then
                            If filterStr.IsEmpty() Then
                                filterStr = col.ColumnName + " = " + row.Item(col.ColumnName).ToString().QuotedStr()
                            Else
                                filterStr = filterStr + " AND " + col.ColumnName + " = " + row.Item(col.ColumnName).ToString().QuotedStr()
                            End If
                        End If
                    Next

                    Dim tmpRow As DataRow() = new_dt.Select(filterStr)
                    If tmpRow.Count > 0 Then
                        Dim new_row As DataRow = tmpRow(0)

                        Dim v = new_row(row(beHeaderColumn))
                        Dim new_col_name As String = beHeaderColumn.ColumnName
                        Dim i As Integer = 1
                        While Not v.ToString().IsEmpty()
                            new_col_name = beHeaderColumn.ColumnName + "_" + i.ToString()
                            v = new_row(row(new_col_name))
                            i += 1
                        End While

                        new_row(row(new_col_name)) = row(beValueColumn)
                    Else
                        Dim new_row As DataRow = new_dt.NewRow()

                        For Each col As DataColumn In row.Table.Columns
                            If col.ColumnName <> beHeaderColumn.ColumnName _
                                And col.ColumnName <> beValueColumn.ColumnName Then
                                new_row.Item(col.ColumnName) = row.Item(col.ColumnName)
                            End If
                        Next

                        Dim v = new_row(row(beHeaderColumn))
                        Dim new_col_name As String = beHeaderColumn.ColumnName
                        Dim i As Integer = 1
                        While Not v.ToString().IsEmpty()
                            new_col_name = beHeaderColumn.ColumnName + "_" + i.ToString()
                            v = new_row(row(new_col_name))
                            i += 1
                        End While

                        new_row(row(new_col_name)) = row(beValueColumn)
                        new_dt.Rows.Add(new_row)
                    End If
                Next
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try
            Return new_dt
        End Function

        ''' <summary>
        ''' recursive check new column name
        ''' check column name exist or not, if exist then create new one
        ''' </summary>
        ''' <param name="dtb">original datatable</param>
        ''' <param name="columnName">original column name</param>
        ''' <param name="newName">new column name</param>
        ''' <param name="startIndex">start sequence</param>
        ''' <returns></returns>
        <Extension()>
        Private Function NewColumnNameBySeq(ByVal dtb As DataTable, ByVal columnName As String, ByVal newName As String, ByVal startIndex As Integer) As String
            Dim new_name As String = newName
            If dtb.Columns.Contains(newName) Then
                startIndex += 1
                new_name = columnName + "_" + startIndex.ToString()
                Return NewColumnNameBySeq(dtb, columnName, new_name, startIndex)
            Else
                Return new_name
            End If
        End Function
    End Module
End Namespace