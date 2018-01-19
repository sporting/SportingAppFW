'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.Runtime.CompilerServices
Imports System.Reflection

Namespace Extensions
    Public Module SaTypeExtension
        <Extension()>
        Public Function IsNumericType(ByVal t As Type) As Boolean
            Return t.Equals(Type.GetType("System.Decimal")) _
                    OrElse t.Equals(Type.GetType("System.Double")) _
                    OrElse t.Equals(Type.GetType("System.Int32")) _
                    OrElse t.Equals(Type.GetType("System.Int64")) _
                    OrElse t.Equals(Type.GetType("System.Int16")) _
                    OrElse t.Equals(Type.GetType("System.Single")) _
                    OrElse t.Equals(Type.GetType("System.UInt32")) _
                    OrElse t.Equals(Type.GetType("System.UInt64")) _
                    OrElse t.Equals(Type.GetType("System.UInt16"))
        End Function

    End Module
End Namespace