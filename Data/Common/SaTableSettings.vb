'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Namespace Data.Common
    Public Enum ObjectType
        otTable
        otView
    End Enum
    Public Class SaTableSettings
        Dim _name As String

        Dim _type As ObjectType = ObjectType.otTable

        Public Property TableType As ObjectType
            Get
                Return _type
            End Get
            Set(value As ObjectType)
                _type = value
            End Set
        End Property
        Public Property TableName As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
            End Set
        End Property
        Sub New(ByVal name As String)
            _name = name
        End Sub

        Sub New(ByVal name As String, ot As ObjectType)
            _name = name
            _type = ot
        End Sub
    End Class
End Namespace