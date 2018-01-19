'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Windows.Forms
Imports SportingAppFW.Components.Data
Imports SportingAppFW.Data.Common

Namespace Components.WinForm
    Partial Public Class SaDataGridViewFN
        Inherits DataGridView

        Private _dataTable As SaDataTableFN
        Private _bindingSource As BindingSource


        Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
            'If e.Control And e.KeyCode = Keys.End Then
            '    LoadData(True)
            'End If
            MyBase.OnKeyDown(e)
        End Sub

        Protected Overrides Sub OnDataSourceChanged(e As EventArgs)
            If DataSource IsNot Nothing Then
                If DataSource.GetType().Equals(GetType(BindingSource)) OrElse DataSource.GetType().IsSubclassOf(GetType(BindingSource)) Then
                    _bindingSource = CType(DataSource, BindingSource)
                    AddHandler _bindingSource.DataSourceChanged, AddressOf BindingSourceDataSourceChanged

                    If _bindingSource IsNot Nothing AndAlso _bindingSource.DataSource IsNot Nothing AndAlso _bindingSource.DataSource.GetType().Equals(GetType(SaDataTableFN)) Then
                        _dataTable = CType(_bindingSource.DataSource, SaDataTableFN)
                    End If
                ElseIf DataSource.GetType().Equals(GetType(SaDataTableFN)) Then
                    _dataTable = CType(DataSource, SaDataTableFN)
                End If
            End If
            MyBase.OnDataSourceChanged(e)
        End Sub

        Protected Overrides Sub OnColumnHeaderMouseClick(e As DataGridViewCellMouseEventArgs)
            If Columns(e.ColumnIndex).SortMode <> DataGridViewColumnSortMode.NotSortable Then
                LoadData(True)
            End If
            MyBase.OnColumnHeaderMouseClick(e)
        End Sub

        Protected Overrides Sub OnScroll(e As ScrollEventArgs)
            MyBase.OnScroll(e)
            If e.ScrollOrientation = ScrollOrientation.VerticalScroll Then
                If e.NewValue > 0 And e.NewValue > RowCount - FN_LIMIT_ROWS_TO_LOAD - Me.DisplayedRowCount(True) Then
                    If _dataTable IsNot Nothing Then
                        If Not _dataTable.IsLoadComplete Then
                            If e.NewValue - e.OldValue > Me.DisplayedRowCount(True) Then
                                LoadData(True)
                            Else
                                LoadData(False)
                            End If
                        End If
                    End If
                End If
            End If
        End Sub


        Private Sub BindingSourceDataSourceChanged(sender As Object, e As EventArgs)
            If _bindingSource.DataSource IsNot Nothing AndAlso _bindingSource.DataSource.GetType().Equals(GetType(SaDataTableFN)) Then
                _dataTable = CType(_bindingSource.DataSource, SaDataTableFN)
            End If
        End Sub

        Private Sub LoadData(ByVal fillit As Boolean)
            If _dataTable IsNot Nothing Then
                Dim cur As Cursor = Cursor

                Cursor = Cursors.WaitCursor

                If _bindingSource IsNot Nothing Then
                    _bindingSource.SuspendBinding()
                End If
                Try
                    _dataTable.FillByDataAdapter(fillit)
                Finally
                    If _bindingSource IsNot Nothing Then
                        _bindingSource.ResumeBinding()
                    End If

                    Cursor = cur
                End Try
            End If
        End Sub


    End Class

End Namespace
