Namespace Data.Common
    Public Enum PrimaryKeyEnum
        IsPKey
        IsNotPKey
    End Enum

    Public Enum AutoDateTimeEnum
        Auto
        NotAuto
    End Enum

    Public Enum AllowNullEnum
        Allow
        NotAllow
    End Enum

    Public Enum AutoIncrementEnum
        Auto
        NotAuto
    End Enum


    Public Class SaFieldsAttribute
        Inherits Attribute

        Public ReadOnly Property GeneralField As Boolean
        Public ReadOnly Property PrimaryKey As Boolean
        Public ReadOnly Property AutoDateTime As Boolean

        Public ReadOnly Property HasDefaultValue As Boolean
        Public ReadOnly Property DefaultValue As Object
        Public ReadOnly Property AllowNull As Boolean
        Public ReadOnly Property AutoIncrement As Boolean

        Sub New()
            Me.New(False, Nothing, AutoDateTimeEnum.NotAuto, AllowNullEnum.NotAllow, PrimaryKeyEnum.IsNotPKey, AutoIncrementEnum.NotAuto)
        End Sub
        Sub New(ByVal isPrimaryKey As PrimaryKeyEnum, Optional ByVal isAutoIncrement As AutoIncrementEnum = AutoIncrementEnum.NotAuto)
            Me.New(False, Nothing, AutoDateTimeEnum.NotAuto, AllowNullEnum.NotAllow, isPrimaryKey, isAutoIncrement)
        End Sub
        Sub New(ByVal isPrimaryKey As PrimaryKeyEnum, ByVal leadingCode As String, Optional ByVal isAutoIncrement As AutoIncrementEnum = AutoIncrementEnum.NotAuto)
            Me.New(False, Nothing, AutoDateTimeEnum.NotAuto, AllowNullEnum.NotAllow, isPrimaryKey, isAutoIncrement)
        End Sub
        Sub New(ByVal isAutoDateTime As AutoDateTimeEnum)
            Me.New(False, Nothing, isAutoDateTime, AllowNullEnum.NotAllow, PrimaryKeyEnum.IsNotPKey, AutoIncrementEnum.NotAuto)
        End Sub
        Sub New(ByVal isAllowNull As AllowNullEnum)
            Me.New(False, Nothing, AutoDateTimeEnum.NotAuto, isAllowNull, PrimaryKeyEnum.IsNotPKey, AutoIncrementEnum.NotAuto)
        End Sub
        Sub New(ByVal hasDefValue As Boolean, ByVal defValue As Object)
            Me.New(hasDefValue, defValue, AutoDateTimeEnum.NotAuto, AllowNullEnum.NotAllow, PrimaryKeyEnum.IsNotPKey, AutoIncrementEnum.NotAuto)
        End Sub
        Sub New(ByVal hasDefValue As Boolean, ByVal defValue As Object, ByVal isAutoDateTime As AutoDateTimeEnum, ByVal isAllowNull As AllowNullEnum, ByVal isPrimaryKey As PrimaryKeyEnum, Optional ByVal isAutoIncrement As AutoIncrementEnum = AutoIncrementEnum.NotAuto)
            HasDefaultValue = hasDefValue

            If hasDefValue Then
                DefaultValue = defValue
            Else
                DefaultValue = Nothing
            End If

            AutoDateTime = isAutoDateTime = AutoDateTimeEnum.Auto
            AllowNull = isAllowNull = AllowNullEnum.Allow
            PrimaryKey = isPrimaryKey = PrimaryKeyEnum.IsPKey
            AutoIncrement = isAutoIncrement = AutoIncrementEnum.Auto
        End Sub

    End Class


End Namespace