﻿Imports System.Reflection
Imports SportingAppFW.Extensions
Imports SportingAppFW.Tools.SaAttributeUtility


Namespace Data.Common
    Public Class SaFields
        Inherits Object

        ' hashtable with propertyinfo, SaFieldsAttribute
        Private _fieldAttributes As List(Of KeyValuePair(Of PropertyInfo, SaFieldsAttribute)) = New List(Of KeyValuePair(Of PropertyInfo, SaFieldsAttribute))()
        Private _fieldUIAttributes As List(Of KeyValuePair(Of PropertyInfo, SaUIFieldsAttribute)) = New List(Of KeyValuePair(Of PropertyInfo, SaUIFieldsAttribute))()

        Private _primaryKeys As String()
        Private _names As String()

        ReadOnly Property SaFielsdAttributes As List(Of KeyValuePair(Of PropertyInfo, SaFieldsAttribute))
            Get
                Return _fieldAttributes
            End Get
        End Property

        ReadOnly Property SaUIFieldsAttributes As List(Of KeyValuePair(Of PropertyInfo, SaUIFieldsAttribute))
            Get
                Return _fieldUIAttributes
            End Get
        End Property
        Public Shared Function GetSaFieldsAttribute(ByVal pro As PropertyInfo) As SaFieldsAttribute
            Return CType(GetAttribute(pro, "SaFieldsAttribute"), SaFieldsAttribute)
        End Function

        Public Shared Function GetSaUIFieldsAttribute(ByVal pro As PropertyInfo) As SaUIFieldsAttribute
            Return CType(GetAttribute(pro, "SaUIFieldsAttribute"), SaUIFieldsAttribute)
        End Function

        ReadOnly Property Names As String()
            Get
                Return _names
            End Get
        End Property

        ReadOnly Property Values As String()
            Get
                Dim val = From attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In _fieldAttributes
                          Select attri.Key.GetValue(Me, Nothing).ToString().QuotedStr()


                If val.Count < 1 Then
                    Return New String() {}
                Else
                    Return val.ToArray()
                End If
            End Get
        End Property

        Public Function GetPropertyValue(ByVal propertyName As String) As Object
            For Each attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In _fieldAttributes
                If attri.Key.Name = propertyName Then
                    Dim res = attri.Key.GetValue(Me, Nothing)
                    If res IsNot Nothing Then
                        Return res
                    Else
                        Return attri.Value.DefaultValue
                    End If
                End If
            Next
            Return Nothing
        End Function

        Sub New()
            Try
                For Each pro As PropertyInfo In Me.GetType.GetProperties()
                    Dim attri As SaFieldsAttribute = CType(GetAttribute(pro, "SaFieldsAttribute"), SaFieldsAttribute)
                    Dim attri2 As SaUIFieldsAttribute = CType(GetAttribute(pro, "SaUIFieldsAttribute"), SaUIFieldsAttribute)
                    If attri IsNot Nothing Then
                        _fieldAttributes.Add(New KeyValuePair(Of PropertyInfo, SaFieldsAttribute)(pro, attri))

                        If pro.CanWrite Then
                            If attri.HasDefaultValue Then
                                pro.SetValue(Me, attri.DefaultValue, Nothing)
                            Else
                                pro.SetValue(Me, Nothing, Nothing)
                            End If
                        End If
                    End If

                    If attri2 IsNot Nothing Then
                        _fieldUIAttributes.Add(New KeyValuePair(Of PropertyInfo, SaUIFieldsAttribute)(pro, attri2))
                    End If
                Next

                '所有欄位名稱
                Dim val = From attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In _fieldAttributes
                          Select attri.Key.Name

                If val.Count < 1 Then
                    _names = New String() {}
                Else
                    _names = val.ToArray()
                End If

                '所有 primary keys
                Dim attris = From attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In _fieldAttributes
                             Where attri.Value.PrimaryKey
                             Select attri.Key.Name

                _primaryKeys = attris.ToArray()

            Catch ex As Exception
                Console.WriteLine(String.Format("{0} {1}", "SaFields", ex.Message))
            End Try
        End Sub
        Public Function GetPrimaryKeys() As String()
            Return _primaryKeys
        End Function

        Public Function GetPrimaryKeyValueSqls() As String()
            Dim attris = From attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In _fieldAttributes
                         Where attri.Value.PrimaryKey
                         Select String.Format("({0} = {1})", attri.Key.Name, attri.Key.GetValue(Me, Nothing).ToString().QuotedStr())

            If attris.Count = 0 Then
                attris = From attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In _fieldAttributes
                         Where attri.Value.PrimaryKey
                         Select CType(IIf(attri.Key.GetValue(Me, Nothing) Is Nothing, String.Format("({0} IS NULL)", attri.Key.Name), String.Format("({0} = {1})", attri.Key.Name, attri.Key.GetValue(Me, Nothing).ToString().QuotedStr())), String)
            End If

            Return attris.ToArray()
        End Function

        Public Function GetValueSqls() As String()
            Dim attris = From attri As KeyValuePair(Of PropertyInfo, SaFieldsAttribute) In _fieldAttributes
                         Where attri.Value.AutoDateTime OrElse attri.Key.GetValue(Me, Nothing) IsNot Nothing
                         Select CType(IIf(attri.Value.AutoDateTime, String.Format("{0} = {1}", attri.Key.Name, Now.ToString("yyyy-MM-dd HH:mm:ss").QuotedStr()), String.Format("{0} = {1}", attri.Key.Name, attri.Key.GetValue(Me, Nothing).ToString().QuotedStr())), String)

            Return attris.ToArray()
        End Function

        Public Sub ClearFieldAttribute()
            _fieldAttributes.Clear()
            _fieldUIAttributes.Clear()
        End Sub

        Public Sub AddSaFieldAttribute(pro As PropertyInfo, attri As SaFieldsAttribute)
            _fieldAttributes.Add(New KeyValuePair(Of PropertyInfo, SaFieldsAttribute)(pro, attri))
        End Sub

        Public Sub AddSaUIFieldAttribute(pro As PropertyInfo, attri As SaUIFieldsAttribute)
            _fieldUIAttributes.Add(New KeyValuePair(Of PropertyInfo, SaUIFieldsAttribute)(pro, attri))
        End Sub

    End Class

End Namespace