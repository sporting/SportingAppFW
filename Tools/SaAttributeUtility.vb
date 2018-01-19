Imports System.Reflection

Namespace Tools
    Public Module SaAttributeUtility
        Public Function GetAttribute(ByRef pro As PropertyInfo, ByVal name As String) As Object
            Dim attributes As Object() = pro.GetCustomAttributes(False)
            For Each attribute As Object In attributes
                If attribute.GetType.Name = name Then
                    Return attribute
                End If
            Next
            Return Nothing
        End Function
    End Module

End Namespace