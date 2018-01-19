'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************

Namespace Data.Common
    Public Class SaDBColumnType
        Dim _column As String
        Dim _data_type As String
        Dim _ms_type As System.Type

        Public Property Column As String
            Get
                Return _column
            End Get
            Set(value As String)
                _column = value
            End Set
        End Property

        Public Property DataType As String
            Get
                Return _data_type
            End Get
            Set(value As String)
                _data_type = value
            End Set
        End Property

        Public ReadOnly Property MsType As System.Type
            Get
                Return SaDBTypeMappingClass.Instance.Mapping(_data_type)
            End Get
        End Property

        Sub New(ByVal col As String, ByVal type As String)
            _column = col
            _data_type = type
        End Sub
    End Class
End Namespace