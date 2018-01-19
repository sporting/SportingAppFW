'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Runtime.CompilerServices
Imports System.Reflection

Namespace Extensions

    Public Module SaHashSetExtension
        <Extension()>
        Public Function ToDataTable(Of T As Class)(ByVal data As HashSet(Of T)) As DataTable
            Dim result As New DataTable()
            Dim type As Type = GetType(T)
            Dim list = (From p As PropertyInfo In type.GetProperties() Where (p.CanRead AndAlso (p.GetIndexParameters().Length = 0)) Select New With {.[Property] = p, .Column = New DataColumn(p.Name, p.PropertyType)})
            result.Columns.AddRange((From obj In list Select obj.Column).ToArray())
            If (Not data Is Nothing) Then
                result.BeginLoadData()
                Array.ForEach(Of T)(data.ToArray(), Function(item As T) result.Rows.Add((From obj In list Select obj.[Property].GetValue(item, Nothing)).ToArray()))
                result.EndLoadData()
            End If
            result.AcceptChanges()
            list = Nothing
            type = Nothing
            Return result
        End Function
    End Module
End Namespace