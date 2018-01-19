'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.ComponentModel
Imports System.Windows.Forms
Imports SportingAppFW.Components.Data
Imports SportingAppFW.Data.Common

Namespace Components.WinForm
    Partial Public Class SaBindingSourceFN
        Inherits BindingSource

        Private _dataTable As SaDataTableFN

        Protected Overrides Sub OnDataSourceChanged(e As EventArgs)
            If DataSource IsNot Nothing AndAlso DataSource.GetType().Equals(GetType(SaDataTableFN)) Then
                _dataTable = CType(DataSource, SaDataTableFN)
            End If
            MyBase.OnDataSourceChanged(e)
        End Sub

        Protected Overrides Sub OnPositionChanged(e As EventArgs)
            MyBase.OnPositionChanged(e)
            If Position > Count - FN_LIMIT_ROWS_TO_LOAD Then
                If _dataTable IsNot Nothing Then
                    _dataTable.FillByDataAdapter(False)
                End If
            End If
        End Sub
    End Class

End Namespace