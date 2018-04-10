Imports System.Reflection
Imports SportingAppFW.Extensions
Imports SportingAppFW.SaSystem.SaDelegates
Imports SportingAppFW.SaWindows.Data

Namespace Data.Common
    Public Class SaTable
        Inherits SaTableSettings
        Private _db As IDbConnection
        Private _fields As SaFields

        Public Property Data As SaDataTableFN
        Public ReadOnly Property DBConnection As IDbConnection
            Get
                Return _db
            End Get
        End Property

        Public Property Fields As SaFields
            Get
                Return _fields
            End Get
            Set(value As SaFields)
                _fields = value
            End Set
        End Property
        Public Sub DBOpen()
            If _db IsNot Nothing Then
                If _db.State = ConnectionState.Closed Then
                    _db.Open()
                End If
            End If
        End Sub

        Public Sub DBClose()
            If _db IsNot Nothing Then
                If _db.State = ConnectionState.Open Then
                    _db.Close()
                End If
            End If
        End Sub

        Sub New(ByVal db As IDbConnection, ByVal table As String, ByVal fields As SaFields)
            MyBase.New(table)
            _db = db
            _fields = fields

            DBOpen()
        End Sub

        Public Function CreateTableIfNotExists(ByVal fields As SaFields) As Boolean
            Try
                Dim sql As String = "CREATE TABLE IF NOT EXISTS {0} ({1})"
                Dim segments As List(Of String) = New List(Of String)()
                Dim prikeys As List(Of String) = New List(Of String)()
                Dim tmp As String

                'For Each pro As PropertyInfo In fields.GetType().GetProperties()
                For Each attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In fields.SaFielsdAttributes
                    tmp = String.Format("{0} {1}", attri.Key.Name, DecideDBTypeText(attri.Key))
                    If attri.Value.PrimaryKey And Not attri.Value.AutoIncrement Then
                        prikeys.Add(attri.Key.Name)
                    ElseIf attri.Value.PrimaryKey And attri.Value.AutoIncrement Then
                        tmp = String.Format("{0} {1}", tmp, "PRIMARY KEY AUTOINCREMENT")
                    End If

                    If attri.Value.HasDefaultValue Then
                        tmp = String.Format("{0} DEFAULT '{1}'", tmp, attri.Value.DefaultValue)
                    End If

                    segments.Add(tmp)
                Next

                If prikeys.Count > 0 Then
                    segments.Add(String.Format("PRIMARY KEY ({0})", String.Join(", ", prikeys.ToArray())))
                End If

                Dim cmd As IDbCommand = _db.CreateCommand()
                'DBOpen()
                cmd.CommandText = String.Format(sql, TableName, String.Join(", ", segments.ToArray()))
                Dim res = cmd.ExecuteScalar()
                'DBClose()

                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Function DecideDBTypeText(pro As PropertyInfo) As String
            If pro.PropertyType Is GetType(String) Then
                Return "TEXT"
            ElseIf pro.PropertyType Is GetType(Integer) Then
                Return "Integer"
            End If

            Return "TEXT"
        End Function

        Public Function IsTableEmpty() As Boolean
            Dim cmd As IDbCommand = _db.CreateCommand()
            'DBOpen()
            cmd.CommandText = String.Format("SELECT COUNT(*) FROM {0} LIMIT 1", TableName)
            Dim res = cmd.ExecuteScalar()
            'DBClose()

            Return (res IsNot Nothing) AndAlso (res = "0")
        End Function

        Public Function SelectAll() As SaDataTableFN
            'DBOpen()

            Dim dt As SaDataTableFN

            dt = New SaDataTableFN(String.Format("SELECT * FROM {0} ORDER BY 1", TableName), _db)
            dt.FillByDataAdapter(True)

            'DBClose()
            Return dt
        End Function

        Public Function GetDataReader() As IDataReader
            Dim cmd As IDbCommand = _db.CreateCommand()
            'DBOpen()
            cmd.CommandText = String.Format("SELECT * FROM {0}", TableName)
            Dim datareader As IDataReader = cmd.ExecuteReader()

            'DBClose()
            Return datareader
        End Function

        Public Function SelectWhere(ByVal wheresql As String) As SaDataTableFN
            'DBOpen()

            Dim dt As SaDataTableFN
            If wheresql.IsEmpty Then
                dt = New SaDataTableFN(String.Format("SELECT * FROM {0}", TableName), _db)
            Else
                dt = New SaDataTableFN(String.Format("SELECT * FROM {0} WHERE {1}", TableName, wheresql), _db)
            End If

            dt.FillByDataAdapter(True)

            'DBClose()

            Return dt
        End Function

        Public Function NewRowByField(ByVal dt As DataTable, ByVal fields As SaFields) As DataRow
            Dim row As DataRow = dt.NewRow()
            'For Each field As KeyValuePair(Of String, Object) In fields.NameValues
            For Each attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In fields.SaFielsdAttributes
                row(attri.Key.Name) = attri.Key.GetValue(fields, Nothing)
            Next

            Return row
        End Function
        Public Function DeleteRows(ByVal multikeyvaluefiels() As SaFields) As Integer
            Dim affectrowscnt As Integer
            For Each row In multikeyvaluefiels
                affectrowscnt = affectrowscnt + DeleteRow(row)
            Next
            Return affectrowscnt
        End Function
        Public Function DeleteRow(ByVal keyvaluefields As SaFields) As Integer
            Try
                Dim wherefilter As List(Of String) = New List(Of String)()
                Dim proval As Object

                For Each attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In keyvaluefields.SaFielsdAttributes
                    If attri.Value.PrimaryKey Then
                        proval = attri.Key.GetValue(keyvaluefields, Nothing)
                        If proval Is Nothing Then
                            wherefilter.Add(String.Format("({0} IS NULL", attri.Key.Name))
                        Else
                            wherefilter.Add(String.Format("({0} = {1})", attri.Key.Name, proval.ToString().QuotedStr()))
                        End If
                    End If
                Next


                ' all fields are key
                If wherefilter.Count = 0 Then
                    For Each attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In keyvaluefields.SaFielsdAttributes
                        proval = attri.Key.GetValue(keyvaluefields, Nothing)
                        If proval Is Nothing Then
                            wherefilter.Add(String.Format("({0} IS NULL", attri.Key.Name))
                        Else
                            wherefilter.Add(String.Format("({0} = {1})", attri.Key.Name, proval.ToString().QuotedStr()))
                        End If
                    Next
                End If

                Dim cmd As IDbCommand = _db.CreateCommand()
                'DBOpen()

                cmd.CommandText = String.Format("DELETE FROM {0} WHERE {1}", TableName, String.Join(" AND ", wherefilter.ToArray()))

                Dim affectrowscnt As Integer
                affectrowscnt = cmd.ExecuteNonQuery()

                'DBClose()

                Return affectrowscnt
            Catch ex As Exception
                Return 0
            End Try

        End Function


        Public Function UpdateRows(ByVal multikeyvaluefiels As SaFields(), ByVal multivaluefields As SaFields()) As Integer
            Dim affectrowscnt As Integer = 0
            For idx As Integer = 0 To multikeyvaluefiels.Length
                affectrowscnt = affectrowscnt + UpdateRow(multikeyvaluefiels(idx), multivaluefields(idx))
            Next

            Return affectrowscnt
        End Function

        Public Function UpdateRow(ByVal keyvaluefiels As SaFields, ByVal valuefields As SaFields) As Integer
            Try
                Dim valueparams As List(Of KeyValuePair(Of String, String)) = New List(Of KeyValuePair(Of String, String))()
                Dim wherefilter As List(Of String) = New List(Of String)()
                Dim proval As Object

                For Each attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In valuefields.SaFielsdAttributes
                    proval = attri.Key.GetValue(valuefields, Nothing)
                    If attri.Value.AutoDateTime Then
                        valueparams.Add(New KeyValuePair(Of String, String)(attri.Key.Name, Now.ToString("yyyy-MM-dd HH:mm:ss")))
                    ElseIf proval IsNot Nothing Then
                        valueparams.Add(New KeyValuePair(Of String, String)(attri.Key.Name, proval.ToString()))
                    End If
                Next

                'For Each pro As PropertyInfo In keyvaluefiels.GetType().GetProperties()
                For Each attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In keyvaluefiels.SaFielsdAttributes
                    proval = attri.Key.GetValue(keyvaluefiels, Nothing)
                    If attri.Value.PrimaryKey Then
                        If proval Is Nothing Then
                            wherefilter.Add(String.Format("({0} IS NULL)", attri.Key.Name))
                        Else
                            wherefilter.Add(String.Format("({0} = {1})", attri.Key.Name, proval.ToString().QuotedStr()))
                        End If
                    End If
                Next

                ' all fields are key
                If wherefilter.Count = 0 Then
                    For Each attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In keyvaluefiels.SaFielsdAttributes
                        proval = attri.Key.GetValue(keyvaluefiels, Nothing)
                        If proval Is Nothing Then
                            wherefilter.Add(String.Format("({0} IS NULL)", attri.Key.Name))
                        Else
                            wherefilter.Add(String.Format("({0} = {1})", attri.Key.Name, proval.ToString().QuotedStr()))
                        End If
                    Next
                End If

                Dim fields As List(Of String) = New List(Of String)()

                For Each val As KeyValuePair(Of String, String) In valueparams
                    fields.Add(String.Format("{0} = {1}", val.Key, val.Value.QuotedStr()))
                Next

                Dim cmd As IDbCommand = _db.CreateCommand()
                'DBOpen()

                cmd.CommandText = String.Format("UPDATE {0} SET {1} WHERE {2}", TableName, String.Join(", ", fields.ToArray()), String.Join(" AND ", wherefilter.ToArray()))

                Dim affectrowscnt As Integer
                affectrowscnt = cmd.ExecuteNonQuery()

                'DBClose()

                Return affectrowscnt
            Catch ex As Exception
                Return 0
            End Try

        End Function

        Public Sub InsertRows(ByVal multivaluefields As SaFields())
            Dim affectrowscnt As Integer = 0
            For Each row As SaFields In multivaluefields
                affectrowscnt = affectrowscnt + InsertRow(row)
            Next
        End Sub

        Public Function InsertRow(ByVal valuefields As SaFields) As Integer
            Try
                'Dim fieldsType As Type = valuefields.GetType()
                'Dim valueparams As List(Of KeyValuePair(Of String, String)) = New List(Of KeyValuePair(Of String, String))()
                'Dim attri As SaFieldsAttribute

                Dim fields As List(Of String) = New List(Of String)()
                Dim values As List(Of String) = New List(Of String)()

                '    For Each pro As PropertyInfo In fieldsType.GetProperties()
                For Each attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In valuefields.SaFielsdAttributes
                    If attri.Value.AutoDateTime Then
                        'valueparams.Add(New KeyValuePair(Of String, String)(pro.Name, Now.ToString("yyyy-MM-dd HH:mm:ss")))
                        fields.Add(attri.Key.Name)
                        values.Add(Now.ToString("yyyy-MM-dd HH:mm:ss").QuotedStr())
                    ElseIf attri.Key.GetValue(valuefields, Nothing) IsNot Nothing Then
                        ' valueparams.Add(New KeyValuePair(Of String, String)(pro.Name, pro.GetValue(valuefields, Nothing).ToString()))
                        fields.Add(attri.Key.Name)
                        values.Add(attri.Key.GetValue(valuefields, Nothing).ToString().QuotedStr())
                    End If
                Next

                'For Each val As KeyValuePair(Of String, String) In valueparams
                '    fields.Add(val.Key)
                '    values.Add(val.Value.QuotedStr())
                'Next

                Dim cmd As IDbCommand = _db.CreateCommand()
                'DBOpen()

                cmd.CommandText = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", TableName, String.Join(",", fields.ToArray()), String.Join(",", values.ToArray()))

                Dim affectrowscnt As Integer
                affectrowscnt = cmd.ExecuteNonQuery()

                'DBClose()

                Return affectrowscnt
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Return -1
            End Try
        End Function

        Public Overloads Function LoadData() As SaDataTableFN
            Data = SelectAll()
            AddHandler Data.RowChanged, AddressOf RowChanged
            AddHandler Data.RowChanging, AddressOf RowChanging
            AddHandler Data.ColumnChanged, AddressOf ColumnChanged
            AddHandler Data.ColumnChanging, AddressOf ColumnChanging

            Return Data
        End Function
        Public Event ColumnChangedStaticValueHandler As ColumnChangedStaticValue
        Private Sub ColumnChanged(sender As Object, e As DataColumnChangeEventArgs)
            RaiseEvent ColumnChangedStaticValueHandler(sender, e)
        End Sub
        Public Event ColumnChangingStaticValueHandler As ColumnChangingStaticValue
        Private Sub ColumnChanging(sender As Object, e As DataColumnChangeEventArgs)
            RaiseEvent ColumnChangingStaticValueHandler(sender, e)
        End Sub
        Public Event RowChangingStaticValueHandler As RowChangingStaticValue
        Private Sub RowChanging(sender As Object, e As DataRowChangeEventArgs)
            RaiseEvent RowChangingStaticValueHandler(sender, e.Row)
        End Sub

        Public Overloads Function LoadData(ByVal wheresql As String) As SaDataTableFN
            Data = SelectWhere(wheresql)
            AddHandler Data.RowChanged, AddressOf RowChanged
            AddHandler Data.RowChanging, AddressOf RowChanging
            AddHandler Data.ColumnChanged, AddressOf ColumnChanged
            AddHandler Data.ColumnChanging, AddressOf ColumnChanging

            Return Data
        End Function

        Public Event RowChangedStaticValueHandler As RowChangedStaticValue

        Private Sub RowChanged(sender As Object, e As DataRowChangeEventArgs)
            RemoveHandler Data.RowChanged, AddressOf RowChanged
            Try
                'For Each pro As PropertyInfo In Fields.GetType().GetProperties
                For Each attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In Fields.SaFielsdAttributes
                    If attri.Key.CanWrite Then
                        If attri.Value.AutoDateTime Then
                            e.Row().Item(attri.Key.Name) = Now.ToString("yyyy-MM-dd HH:mm:ss")
                        End If
                    End If
                Next

                RaiseEvent RowChangedStaticValueHandler(sender, e.Row)
            Finally
                AddHandler Data.RowChanged, AddressOf RowChanged
            End Try
        End Sub

        Private _filtered As Boolean
        Public Property Filtered As Boolean
            Get
                Return _filtered
            End Get
            Set(value As Boolean)
                _filtered = value
                If Not _filtered Then
                    If Data IsNot Nothing Then
                        Data.DefaultView.RowFilter = String.Empty
                    End If
                End If
            End Set
        End Property
        Public Sub FilterDefaultView(ByVal words As String)
            Dim filterTexts As List(Of String) = New List(Of String)()
            If Data IsNot Nothing Then
                For Each attri As KeyValuePair(Of PropertyInfo, SaUIFieldsAttribute) In Fields.SaUIFieldsAttributes
                    If attri.Key.CanRead Then
                        If attri.Value.FilterField Then
                            filterTexts.Add("(" + attri.Key.Name + " LIKE '%" + words + "%')")
                        End If
                    End If
                Next

                Data.DefaultView.RowFilter = String.Join(" OR ", filterTexts.ToArray())
            End If
        End Sub

        Public Sub FilterDefaultView(ByVal parentFilter As String, ByVal words As String)
            Dim filterTexts As List(Of String) = New List(Of String)()
            If Data IsNot Nothing Then
                If Not words.IsEmpty Then
                    For Each attri As KeyValuePair(Of PropertyInfo, SaUIFieldsAttribute) In Fields.SaUIFieldsAttributes
                        If attri.Key.CanRead Then
                            If attri.Value.FilterField Then
                                filterTexts.Add("(" + attri.Key.Name + " LIKE '%" + words + "%')")
                            End If
                        End If
                    Next
                End If

                If filterTexts.Count > 0 Then
                    Data.DefaultView.RowFilter = IIf(parentFilter.IsEmpty, String.Empty, parentFilter + " AND (") + String.Join(" OR ", filterTexts.ToArray()) + ")"
                Else
                    Data.DefaultView.RowFilter = IIf(parentFilter.IsEmpty, String.Empty, parentFilter)
                End If
            End If
        End Sub

        Public Function GetMappingFieldValue(ByVal fieldname As String, ByVal fieldvalue As String) As Hashtable
            Dim key As String
            Dim sqlwhere As String
            Dim ht As Hashtable = New Hashtable()

            For Each attri As KeyValuePair(Of PropertyInfo, SaUIFieldsAttribute) In Fields.SaUIFieldsAttributes
                If attri.Key.Name.Equals(fieldname) Then
                    If attri.Value.CustomControl = CustomControlEnum.MultiComboBox Then
                        key = attri.Value.ComboBoxSetting.ValueMember
                        sqlwhere = String.Format("{0}={1}", key, fieldvalue.QuotedStr())
                        Dim saTab As SaTable = Activator.CreateInstance(attri.Value.ComboBoxSetting.DataSourceClass, _db)
                        Dim saTable As SaDataTableFN = saTab.SelectWhere(sqlwhere)
                        If saTable.Rows.Count > 0 Then
                            For Each p As SaPassColumnToTarget In attri.Value.ComboBoxSetting.PassToTargets
                                ht.Add(p.SourceColumnName, saTable.Rows(0).Item(p.SourceColumnName))
                            Next
                        End If
                        Exit For
                    End If
                End If
            Next
            Return ht
        End Function
    End Class

End Namespace