Namespace Data.Common
    Public Enum ReadModeEnum
        Read
        ReadWrite
    End Enum

    Public Enum FilterFieldEnum
        FilterField
        SystemField
    End Enum

    Public Enum CustomControlEnum
        TextBox
        CheckBox
        DateTimePicker
        MultiComboBox
    End Enum

    Public Enum VisibleEnum
        Visible
        NotVisible
    End Enum

    Public Class SaUIFieldsAttribute
        Inherits Attribute
        Public Property Caption As String
        Public Property ReadMode As Boolean
        Public Property FilterField As Boolean
        Public Property CustomControl As CustomControlEnum
        Public Property FieldVisible As Boolean = True
        ' Lookup datasource for ComboBox setting
        Public Property ComboBoxSetting As SaComboBoxSetting

        Sub New(ByVal displayName As String, ByVal filter As FilterFieldEnum, Optional ByVal mode As ReadModeEnum = ReadModeEnum.ReadWrite)
            Caption = displayName
            ReadMode = mode = ReadModeEnum.Read
            FilterField = filter = FilterFieldEnum.FilterField
            CustomControl = CustomControlEnum.TextBox
        End Sub
        Sub New(ByVal displayName As String, ByVal filter As FilterFieldEnum, ByVal visible As VisibleEnum, Optional ByVal mode As ReadModeEnum = ReadModeEnum.ReadWrite)
            Caption = displayName
            ReadMode = mode = ReadModeEnum.Read
            FilterField = filter = FilterFieldEnum.FilterField
            CustomControl = CustomControlEnum.TextBox
            FieldVisible = visible = VisibleEnum.Visible
        End Sub
        Sub New(ByVal displayName As String, ByVal filter As FilterFieldEnum, ByVal cont As CustomControlEnum, Optional ByVal mode As ReadModeEnum = ReadModeEnum.ReadWrite)
            Caption = displayName
            ReadMode = mode = ReadModeEnum.Read
            FilterField = filter = FilterFieldEnum.FilterField
            CustomControl = cont
        End Sub
        Sub New(ByVal displayName As String, ByVal filter As FilterFieldEnum, ByVal cont As CustomControlEnum, ByVal dtType As Type, ByVal valuemember As String, ByVal displaycolumns As String, ByVal displaynames As String, ByVal passmembers As String, ByVal passto As String, Optional ByVal mode As ReadModeEnum = ReadModeEnum.ReadWrite)
            Caption = displayName
            ReadMode = mode = ReadModeEnum.Read
            FilterField = filter = FilterFieldEnum.FilterField
            CustomControl = cont
            ComboBoxSetting = New SaComboBoxSetting()
            ComboBoxSetting.DataSourceClass = dtType
            ComboBoxSetting.ValueMember = valuemember
            Dim lst As List(Of SaColumnNameToDisplay) = New List(Of SaColumnNameToDisplay)()

            Dim colnames As String() = displaycolumns.Split(";")
            Dim displaytexts As String() = displaynames.Split(";")

            For i As Integer = 0 To colnames.Count - 1
                lst.Add(New SaColumnNameToDisplay(colnames(i), displaytexts(i)))
            Next

            ComboBoxSetting.ColumnsToDisplay = lst

            Dim passlst As List(Of SaPassColumnToTarget) = New List(Of SaPassColumnToTarget)()

            Dim pass As String() = passmembers.Split(";")
            Dim passtotargets As String() = passto.Split(";")

            For i As Integer = 0 To pass.Count - 1
                passlst.Add(New SaPassColumnToTarget(pass(i), passtotargets(i)))
            Next

            ComboBoxSetting.PassToTargets = passlst
        End Sub

        Sub New(ByVal displayName As String, ByVal filter As FilterFieldEnum, ByVal cont As CustomControlEnum, ByVal dtType As Type, ByVal filterWhere As String, ByVal valuemember As String, ByVal displaycolumns As String, ByVal displaynames As String, ByVal passmembers As String, ByVal passto As String, Optional ByVal mode As ReadModeEnum = ReadModeEnum.ReadWrite)
            Me.New(displayName, filter, cont, dtType, valuemember, displaycolumns, displaynames, passmembers, passto, mode)
            ComboBoxSetting.FilterWhere = filterWhere
        End Sub
    End Class

End Namespace