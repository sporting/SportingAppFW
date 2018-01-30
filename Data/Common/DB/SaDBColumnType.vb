'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************

Namespace Data.Common.DB
    Public MustInherit Class SaDBColumnType
        Dim _column As String
        Friend Property data_type As String
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
                Return data_type
            End Get
            Set(value As String)
                data_type = value
            End Set
        End Property

        Sub New(ByVal col As String, ByVal type As String)
            _column = col
            data_type = type
        End Sub

        Public MustOverride ReadOnly Property MsType As System.Type
    End Class
End Namespace