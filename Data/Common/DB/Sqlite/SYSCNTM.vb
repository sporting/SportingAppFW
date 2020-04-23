Imports SportingAppFW.Extensions
Imports SportingAppFW.SaWindows.Data

Namespace Data.Common.DB.Sqlite
    Public Class SYSCNTM
        Inherits SaTable
        Implements ISaTable

        Sub New(ByRef db As IDbConnection)
            MyBase.New(db, "SYSCNTM", New SYSCNTMFields())
            AddHandler Me.RowChangedStaticValueHandler, AddressOf CustomRowChangedStaticValue

            Initialize()
        End Sub

        Private Sub CustomRowChangedStaticValue(sender As Object, ByRef row As DataRow)

        End Sub

        Public Sub CreateTable() Implements ISaTable.CreateTable
            CreateTableIfNotExists(Fields)
        End Sub

        Public Sub Initialize() Implements ISaTable.Initialize
            CreateTable()
        End Sub

        Public Function GetNextVal(ByVal leadingCode As String, Optional ByVal formatLength As Integer = 2) As String
            Dim today As String = Now.ToString("yyyyMMdd")
            Dim dt As SaDataTableFN = SelectWhere(String.Format("{0}={1} AND RESET_DATE={2}", "COUNTER_ID", leadingCode.QuotedStr(), today.QuotedStr()))
            Dim nextVal As String
            If dt IsNot Nothing AndAlso dt.Rows.Count = 1 Then
                Dim val As Integer = CType(dt.Rows(0).Item("CURRENT_VAL"), Integer) + 1

                UpdateRow(New SYSCNTMFields(leadingCode, today, 0), New SYSCNTMFields(leadingCode, today, val))

                nextVal = String.Format("{0}{1}{" + formatLength.ToString() + ":00}", leadingCode, today, val)
            Else
                InsertRow(New SYSCNTMFields(leadingCode, today, 1))

                nextVal = String.Format("{0}{1}{" + formatLength.ToString() + ":00}", leadingCode, today, 1)
            End If
            Return nextVal
        End Function

    End Class

    Public Class SYSCNTMFields
        Inherits SaFields
        <SaFieldsAttribute(PrimaryKeyEnum.IsPKey)>
        <SaUIFieldsAttribute("前置代碼", FilterFieldEnum.FilterField)>
        Public Property COUNTER_ID As String
        <SaFieldsAttribute(PrimaryKeyEnum.IsPKey)>
        <SaUIFieldsAttribute("重置日期", FilterFieldEnum.FilterField)>
        Public Property RESET_DATE As String
        <SaFieldsAttribute()>
        <SaUIFieldsAttribute("目前號碼", FilterFieldEnum.SystemField)>
        Public Property CURRENT_VAL As Integer

        Sub New()

        End Sub
        Sub New(ByVal id As String, ByVal resetdate As String, ByVal currval As Integer)
            COUNTER_ID = id
            RESET_DATE = resetdate
            CURRENT_VAL = currval
        End Sub
    End Class
End Namespace