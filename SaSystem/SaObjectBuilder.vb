Imports System.Reflection
Imports System.Reflection.Emit

''' <summary>
''' 參考 SaDataGridViewExtension.ExportToSQLite
''' SaTable、SaFields 需事先有定義好的類別結構
''' 若有事先無法定義的類別結構，例如 DataGridView Select 出來的資料是不定結構
''' 可用 ObjectBuilder 動態建立類別結構
''' </summary>

Namespace SaSystem
    Public Class SaObjectBuilder
        Public Property objType As Type
        Public Sub New()
            objType = Nothing
        End Sub

        Public Function CreateNewObject(Fields As List(Of SaField), Optional baseType As Type = Nothing) As Object
            objType = CompileResultType(Fields, baseType)
            Dim myObject = Activator.CreateInstance(objType)
            Return myObject
        End Function

        Public Function getObjectList() As IList
            Dim listType As Type = GetType(List(Of)).MakeGenericType(objType)

            Return CType(Activator.CreateInstance(listType), IList)
        End Function

        Public Shared Function GetCompareToMethod(genericInstance As Object, sortExpression As String)
            Dim genericType As Type = genericInstance.GetType()
            Dim sortExpressionValue As Object = genericType.GetProperty(sortExpression).GetValue(genericInstance, Nothing)
            Dim sortExpressionType As Type = sortExpressionValue.GetType()
            Dim compareToMethodOfSortExpressionType As MethodInfo = sortExpressionType.GetMethod("CompareTo", New Type() {sortExpressionType})

            Return compareToMethodOfSortExpressionType
        End Function

        Public Shared Function CompileResultType(Fields As List(Of SaField), Optional baseType As Type = Nothing) As Type
            Dim tb As TypeBuilder = GetTypeBuilder(baseType)
            Dim constructor As ConstructorBuilder = tb.DefineDefaultConstructor(MethodAttributes.Public Or MethodAttributes.SpecialName Or MethodAttributes.RTSpecialName)
            For Each field In Fields
                CreateProperty(tb, field.FieldName, field.FieldType)
            Next

            Dim objectType As Type = tb.CreateType()
            Return objectType

        End Function

        Private Shared Function GetTypeBuilder(baseType As Type) As TypeBuilder
            Dim typeSignature As String = "MyDynamicType"
            Dim an = New AssemblyName(typeSignature)
            Dim ab As AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run)
            Dim mb As ModuleBuilder = ab.DefineDynamicModule("MainModule")
            Dim tb As TypeBuilder = mb.DefineType(typeSignature, TypeAttributes.Public Or TypeAttributes.Class Or TypeAttributes.AutoClass Or TypeAttributes.AnsiClass Or TypeAttributes.BeforeFieldInit Or TypeAttributes.AutoLayout, baseType)
            Return tb
        End Function

        Private Shared Sub CreateProperty(tb As TypeBuilder, propertyName As String, propertyType As Type)
            Dim fieldBuilder As FieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private)
            Dim pb As PropertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, Nothing)
            Dim getPropMthBldr As MethodBuilder = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public + MethodAttributes.SpecialName Or MethodAttributes.HideBySig, propertyType, Type.EmptyTypes)
            Dim getIl As ILGenerator = getPropMthBldr.GetILGenerator()
            getIl.Emit(OpCodes.Ldarg_0)
            getIl.Emit(OpCodes.Ldfld, fieldBuilder)
            getIl.Emit(OpCodes.Ret)

            Dim setPropMthdBldr As MethodBuilder = tb.DefineMethod("set_" + propertyName, MethodAttributes.Public Or MethodAttributes.RTSpecialName Or MethodAttributes.HideBySig, Nothing, {propertyType})

            Dim setIl As ILGenerator = setPropMthdBldr.GetILGenerator()
            Dim modifyProperty As Label = setIl.DefineLabel()
            Dim exitSet As Label = setIl.DefineLabel()

            setIl.MarkLabel(modifyProperty)
            setIl.Emit(OpCodes.Ldarg_0)
            setIl.Emit(OpCodes.Ldarg_1)
            setIl.Emit(OpCodes.Stfld, fieldBuilder)

            setIl.Emit(OpCodes.Nop)
            setIl.MarkLabel(exitSet)
            setIl.Emit(OpCodes.Ret)

            pb.SetGetMethod(getPropMthBldr)
            pb.SetSetMethod(setPropMthdBldr)
        End Sub

        Public Shared Sub SetMemberValue(member As MemberInfo, target As Object, value As Object)
            Select Case member.MemberType
                Case MemberTypes.Field
                    If CType(member, PropertyInfo).CanWrite Then
                        CType(member, FieldInfo).SetValue(target, value)
                    End If
                Case MemberTypes.Property
                    If CType(member, PropertyInfo).CanWrite Then
                        CType(member, PropertyInfo).SetValue(target, value, Nothing)
                    End If
                Case Else
                    Throw New ArgumentException("MemberInfo must be if type FieldInfo or PropertyInfo", "member")
            End Select
        End Sub

    End Class

    Public Class SaField
        Public FieldName As String
        Public FieldType As Type
        Public Value As Object

        Public Sub New(name As String, t As Type, val As Object)
            FieldName = name
            FieldType = t
            Value = val
        End Sub

        Public Sub New()

        End Sub
    End Class


End Namespace