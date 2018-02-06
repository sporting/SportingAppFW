'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Namespace SaWindows.Forms

    Public Class SaPhrases
        Dim _hash As Hashtable
        Dim _filterhash As List(Of String)

        Dim _limitLen As Integer = 3

        Property TriggerChars As Integer
            Get
                Return _limitLen
            End Get
            Set(value As Integer)
                _limitLen = value
            End Set
        End Property

        Sub New()
            _limitLen = 3
            _hash = New Hashtable()
            _filterhash = New List(Of String)()
        End Sub

        Sub New(ByVal words_to_intellisense As Integer)
            Me.New()

            _limitLen = words_to_intellisense
        End Sub

        Sub New(ByVal words_to_intellisense As Integer, ByVal phrases As IEnumerable(Of String))
            Me.New()

            _limitLen = words_to_intellisense
            AddPhrases(phrases)
        End Sub
        Public Sub AddPhrase(ByVal key As String)
            If key Is Nothing AndAlso key.Length < _limitLen Then
                Exit Sub
            End If

            If _hash.ContainsKey(key) Then
                _hash.Item(key) = _hash.Item(key) + 1
            Else
                _hash.Add(key, 1)
            End If
        End Sub

        Public Sub AddPhrases(ByVal keys As IEnumerable(Of String))
            If keys Is Nothing Then
                Exit Sub
            End If
            Try
                For Each key As String In keys
                    AddPhrase(key)
                Next
            Catch ex As Exception
                Console.WriteLine(ex.Message)
            End Try

        End Sub

        Public Sub Clear()
            _hash.Clear()
        End Sub

        Public Function KeyStartsWith(ByVal prefix As String) As List(Of String)
            If prefix.Length < _limitLen Then
                Return Nothing
            End If

            Dim res = From de In _hash
                      Where CType(de.key, String).StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase)
                      Select New String(de.key)

            _filterhash = res.OrderBy(Of String)(Function(s) s).ToList()

            Return _filterhash
        End Function

    End Class

End Namespace