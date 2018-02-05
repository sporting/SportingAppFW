'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports SportingAppFW.Extensions
Imports SportingAppFW.Tools

Namespace Data.Common
    ''' <summary>
    ''' Operator enum 
    ''' soSum 加
    ''' soMinus 減
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum SumOperation As Int32
        soSum = 0
        soMinus = 1
    End Enum


    ''' <summary>
    ''' DataSetAggregate
    ''' 1. 縱向合併 DataSet.Tables data : Merge
    ''' 2. 橫向合併 DataSet.Tables data : Concat
    ''' 3. 加入縱向合計
    ''' 4. 數值型資料加總 : SumNumeric
    ''' </summary>
    ''' <remarks></remarks>
    Public NotInheritable Class SaDataSetAggregate
        Implements IDisposable
        Public Sub Dispose() Implements IDisposable.Dispose
            _DataSet.Dispose()
        End Sub

        Private _DataSet As DataSet
        Private _ResultDataTable As DataTable
        Private _ConcatColumnPrefix As String
        Private _ConcatColumnNames As List(Of KeyValuePair(Of String, String))

        Public Property ConcatColumnName(ByVal idxName As String) As String
            Get
                Return _ConcatColumnNames.Find(Function(x) x.Key = idxName).Value
            End Get
            Set(ByVal value As String)
                Dim blnNewKey As Boolean = True

                For Each keyValue As KeyValuePair(Of String, String) In _ConcatColumnNames
                    If keyValue.Key = idxName Then
                        keyValue = New KeyValuePair(Of String, String)(idxName, value)
                        blnNewKey = False
                        Exit For
                    End If
                Next

                If blnNewKey Then
                    _ConcatColumnNames.Add(New KeyValuePair(Of String, String)(idxName, value))
                End If
            End Set
        End Property

        Public Property ConcatColumnPrefix() As String
            Get
                Return _ConcatColumnPrefix
            End Get
            Set(ByVal value As String)
                _ConcatColumnPrefix = value
            End Set
        End Property

        Public Property SourceDataSet() As DataSet
            Get
                Return _DataSet
            End Get
            Set(ByVal value As DataSet)
                _DataSet = value
            End Set
        End Property

        ''' <summary>
        ''' Property: assign to DataSet.Tables(0)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FirstDataTable() As DataTable
            Get
                If _DataSet.Tables.Count > 0 Then
                    Return _DataSet.Tables(0)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Sub New()
            MyBase.New()
            If _DataSet Is Nothing Then
                _DataSet = New DataSet()
            End If

            If _ConcatColumnNames Is Nothing Then
                _ConcatColumnNames = New List(Of KeyValuePair(Of String, String))
            End If
        End Sub

        Public Sub New(ByRef dts As DataSet)
            Me.New()
            _DataSet = dts
        End Sub

        ''' <summary>
        ''' The same with DataTable.Merge
        ''' DataTable 資料串接
        ''' DataSet 裡所有 DataTable Merge
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Merge() As DataTable
            If FirstDataTable Is Nothing Then
                Return Nothing
            End If

            _ResultDataTable = FirstDataTable.Clone()

            For Each dt As DataTable In _DataSet.Tables
                _ResultDataTable.Merge(dt)
            Next

            Return _ResultDataTable
        End Function

        ''' <summary>
        ''' 橫向欄位合併
        ''' </summary>
        ''' <param name="pkField">結合資料的條件 primary key</param>
        ''' <param name="concatField">欲結合的欄位</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Concat(ByVal pkField As String, ByVal concatField As String) As DataTable
            If FirstDataTable Is Nothing Then
                Return Nothing
            End If

            Dim res As DataTable = New DataTable()

            _ResultDataTable = FirstDataTable.Clone()
            res.Columns.Add(New DataColumn(pkField))

            For Each dt As DataTable In _DataSet.Tables
                res = DataTableFullOuterJoin(res, dt, pkField)
            Next

            For Each row As DataRow In res.Rows
                _ResultDataTable.ImportRow(row)
            Next

            Dim pk1 As String = String.Empty
            Dim desDr As DataRow
            Dim newField As String

            For Each dt As DataTable In _DataSet.Tables


                newField = String.Concat(_ConcatColumnPrefix, ConcatColumnName(dt.TableName))

                _ResultDataTable.Columns.Add(New DataColumn(newField, dt.Columns(concatField).DataType))

                pk1 = String.Empty
                For Each dr As DataRow In dt.Rows
                    pk1 = dr(pkField)

                    desDr = _ResultDataTable.Select(String.Concat(pkField, "=", pk1.QuotedStr())).First()

                    If desDr IsNot Nothing Then
                        desDr(newField) = dr(concatField)
                    Else
                        Dim newRow As DataRow
                        newRow = _ResultDataTable.NewRow()
                        newRow(pkField) = pk1
                        newRow(newField) = dr(concatField)
                        _ResultDataTable.Rows.Add(newRow)
                    End If
                Next
            Next

            Return _ResultDataTable
        End Function

        ''' <summary>
        ''' 數值形資料欄位加總
        ''' </summary>
        ''' <param name="pkField"> primary key field </param>
        ''' <returns>result DataTable</returns>
        ''' <remarks></remarks>
        Public Function SumNumeric(ByVal pkField As String, Optional ByVal operation As SumOperation = SumOperation.soSum) As DataTable
            If FirstDataTable Is Nothing Then
                Return Nothing
            End If

            Dim res As DataTable = New DataTable()

            _ResultDataTable = FirstDataTable.Clone()
            res.Columns.Add(New DataColumn(pkField))

            For Each dt As DataTable In _DataSet.Tables
                res = DataTableFullOuterJoin(res, dt, pkField)
            Next

            For Each row As DataRow In res.Rows
                _ResultDataTable.ImportRow(row)
                ' Dim r As DataRow
                '  r = _ResultDataTable.NewRow
                '   r(pkField) = row(pkField)
                '    _ResultDataTable.Rows.Add(r)
            Next

            SumDataTable(pkField, _ResultDataTable, FirstDataTable, SumOperation.soSum)

            For Each dt As DataTable In _DataSet.Tables
                If dt.TableName = FirstDataTable.TableName Then
                    Continue For
                End If
                SumDataTable(pkField, _ResultDataTable, dt, operation)
            Next

            Return _ResultDataTable
        End Function

        Private Sub SumDataTable(ByVal pkField As String, ByRef source As DataTable, ByRef des As DataTable, Optional ByVal operation As SumOperation = SumOperation.soSum)
            Dim item1 As Object = Nothing
            Dim item2 As Object = Nothing
            For Each dc As DataColumn In source.Columns
                For Each dc2 As DataColumn In des.Columns
                    If (dc.ColumnName <> pkField) AndAlso (dc.ColumnName = dc2.ColumnName) AndAlso (dc.DataType Is GetType(Integer) OrElse dc.DataType Is GetType(Double) OrElse dc.DataType Is GetType(Decimal)) Then
                        For Each row As DataRow In source.Rows
                            For Each row2 As DataRow In des.Rows
                                If row(pkField) = row2(pkField) Then
                                    dc.ReadOnly = False
                                    If IsDBNull(row.Item(dc)) AndAlso IsDBNull(row2.Item(dc2)) Then
                                        Continue For
                                    End If

                                    item1 = row.Item(dc)
                                    item2 = row2.Item(dc2)

                                    If operation = SumOperation.soSum Then
                                        row.Item(dc) = SaUtility.NVL(item1, 0) + SaUtility.NVL(item2, 0)
                                    Else
                                        row.Item(dc) = SaUtility.NVL(item1, 0) - SaUtility.NVL(item2, 0)
                                    End If
                                    Exit For
                                End If
                            Next
                        Next
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' primary key field outer join
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="des"></param>
        ''' <param name="pkField"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function DataTableFullOuterJoin(ByRef source As DataTable, ByRef des As DataTable, ByVal pkField As String) As DataTable
            Dim KeyAry As New List(Of String)

            Dim selectedCount As Integer
            For Each dr As DataRow In des.Rows
                selectedCount = source.Select(String.Concat(pkField, "=", dr(pkField).ToString().QuotedStr())).Count()
                If selectedCount <= 0 Then
                    KeyAry.Add(dr(pkField).ToString())
                End If
            Next


            'Dim leftJoin = From sourceRow In sourceDataTable.AsEnumerable() _
            '              Group Join destRow In targetDataTable.AsEnumerable() _
            '             On sourceRow(pkField) Equals destRow(pkField) _
            '            Into Havematch = Any() _
            '           Where Not Havematch _
            '          Select New With {.Field = sourceRow(pkField)}

            '            Dim leftJoin = From sourceRow In sourceDataTable.AsEnumerable() _
            'Group Join destRow In targetDataTable.AsEnumerable() _
            'On sourceRow(pkField) Equals destRow(pkField) _
            'Into ps = Any() _
            'From p In ps.DefaultIfEmpty(sourceDataTable.NewRow())
            'Select New With {.Field = sourceRow(pkField)}

            'Dim rightJoin = From destRow In targetDataTable.AsEnumerable() _
            '               Group Join sourceRow In sourceDataTable.AsEnumerable() _
            '              On destRow(pkField) Equals sourceRow(pkField) _
            '             Into Havematch = Any() _
            '            Where Not Havematch _
            '           Select New With {.Field = destRow(pkField)}

            'Dim fullJoin = leftJoin.Union(rightJoin)

            'Dim dt As DataTable = New DataTable(sourceDataTable.TableName)
            'dt.Columns.Add(New DataColumn(pkField))

            '            For Each f In fullJoin
            '            Dim dr = dt.NewRow
            'dr(pkField) = f.Field.ToString()
            'dt.Rows.Add(dr)
            'Next

            For Each f In KeyAry
                Dim dr = source.NewRow
                dr(pkField) = f
                source.Rows.Add(dr)
            Next

            Return source
        End Function

        Public Sub Clear()
            _DataSet.Tables.Clear()
            _ConcatColumnNames.Clear()
        End Sub
    End Class
End Namespace
