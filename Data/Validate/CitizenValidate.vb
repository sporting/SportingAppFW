Imports System.Text.RegularExpressions
Namespace Data.Validate
    Public Class CitizenValidate
        Public Shared Function IDNumberValidate(ByVal id As String) As Boolean
            Dim pattern As String = "^[A-Z]{1}[12]{1}[0-9]{8}$"
            Dim rgx As New Regex(pattern, RegexOptions.IgnoreCase)
            Return rgx.IsMatch(id)
        End Function
    End Class

End Namespace