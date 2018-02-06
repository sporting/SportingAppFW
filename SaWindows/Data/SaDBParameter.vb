Imports SportingAppFW.Data.Common

Namespace SaWindows.Data
    Public Class SaDBParameter
        Private _key As String = String.Empty
        Private _value As Object = Nothing
        Private _dtype As DbType

        Public Property Key() As String
            Get
                Return _key
            End Get
            Set(ByVal value As String)
                _key = value
            End Set
        End Property

        Public Property Value() As Object
            Get
                Return _value
            End Get
            Set(ByVal value As Object)
                _value = value
            End Set
        End Property

        Public Property DType() As System.Data.DbType
            Get
                Return _dtype
            End Get
            Set(ByVal value As System.Data.DbType)
                _dtype = value
            End Set
        End Property

        Public Sub New(ByVal dbtype As System.Data.DbType, ByVal key As String, ByVal value As Object)
            _key = key
            _value = value
            _dtype = dbtype
        End Sub

        Public Sub New(ByVal dbtype As System.Type, ByVal key As String, ByVal value As Object)
            Dim instance As SaDataTypeMapping = SaDataTypeMapping.Instance
            _key = key
            _value = value
            _dtype = instance.Mapping(dbtype)
        End Sub
    End Class
End Namespace