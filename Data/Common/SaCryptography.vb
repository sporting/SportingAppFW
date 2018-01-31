'*****************************************************
'* Copyright 2017, SportingApp, all rights reserved. *
'* Author: Shih Peiting                              *
'* mailto: sportingapp@gmail.com                     *
'*****************************************************
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports SportingAppFW.Extensions

Namespace Data.Common
    Public Module SaCryptography
        ''' <summary>
        ''' File MD5
        ''' </summary>
        ''' <param name="file_full_path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FileMD5(ByVal file_full_path As String) As Byte()
            If File.Exists(file_full_path) Then
                Using md = MD5.Create()
                    Using stream = File.OpenRead(file_full_path)
                        Return md.ComputeHash(stream)
                    End Using
                End Using
            End If

            Throw New Exception(file_full_path + " is not exists")
        End Function

        Public Function TextMD5(ByVal s As String) As Byte()
            Using md = MD5.Create()
                Return md.ComputeHash(Encoding.UTF8.GetBytes(s))
            End Using
        End Function

        Public Function TextSHA256(ByVal s As String) As Byte()
            Using sha As SHA256 = New SHA256CryptoServiceProvider()
                Return sha.ComputeHash(Encoding.UTF8.GetBytes(s))
            End Using
        End Function

        ''' <summary>
        ''' 加密字串 (公鑰加密)
        ''' </summary>
        ''' <param name="publickey"></param>
        ''' <param name="content"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RSAEncrypt(ByVal publickey As String, ByVal content As String) As String
            Dim rsa As RSACryptoServiceProvider = New RSACryptoServiceProvider()
            rsa.FromXmlString(publickey)

            Dim encryptString As String = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(content), False))

            Return encryptString
        End Function

        ''' <summary>
        ''' 解密字串 (私鑰解密)
        ''' </summary>
        ''' <param name="privatekey"></param>
        ''' <param name="content"></param>
        ''' <remarks></remarks>
        Public Function RSADecrypt(ByVal privatekey As String, ByVal content As String) As String
            Dim rsa As RSACryptoServiceProvider = New RSACryptoServiceProvider()
            Try
                If content.IsEmpty Then
                    Return String.Empty
                End If
                rsa.FromXmlString(privatekey)

                Dim decryptString = Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(content), False))

                Return decryptString
            Catch e As Exception
                Console.WriteLine(e.Message)
                Return String.Empty
            End Try
        End Function

        ''' <summary>
        ''' SHA1 產生簽章資料
        ''' </summary>
        ''' <param name="privatekey"></param>
        ''' <param name="dataToSign"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RSAhashAnsSign(ByVal privatekey As String, ByVal dataToSign As Byte()) As Byte()
            Dim rsa As RSACryptoServiceProvider = New RSACryptoServiceProvider()
            rsa.FromXmlString(privatekey)
            Return rsa.SignData(dataToSign, New SHA1CryptoServiceProvider())
        End Function

        ''' <summary>
        ''' 驗證簽章資料
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="signature"></param>
        ''' <param name="publicKey"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function VerifySignedData(ByVal data As Byte(), ByVal signature As Byte(), ByVal publicKey As String) As Boolean
            Dim rsa As RSACryptoServiceProvider = New RSACryptoServiceProvider()
            rsa.FromXmlString(publicKey) '需要用到傳送者的公鑰將簽章解密

            Return rsa.VerifyData(data, New SHA1CryptoServiceProvider(), signature)
        End Function

        ''' <summary>
        ''' 對稱式加密
        ''' </summary>
        ''' <param name="plainText"></param>
        ''' <param name="key"></param>
        ''' <returns></returns>
        Public Function TextEncrypt(ByVal plainText As String, ByVal key As String) As String
            Dim AES As New RijndaelManaged()
            Dim MD5 As New MD5CryptoServiceProvider()
            Dim plainTextData As Byte() = Encoding.Unicode.GetBytes(plainText)
            Dim keyData As Byte() = MD5.ComputeHash(Encoding.Unicode.GetBytes(key))
            Dim IVData As Byte() = MD5.ComputeHash(Encoding.Unicode.GetBytes("Super SportingApp"))
            AES.Key = keyData
            AES.IV = IVData
            AES.Padding = PaddingMode.PKCS7
            Dim transform As ICryptoTransform = AES.CreateEncryptor()
            ' Dim transform As ICryptoTransform = AES.CreateEncryptor(keyData, IVData)
            Dim outputData As Byte() = transform.TransformFinalBlock(plainTextData, 0, plainTextData.Length)
            Return Convert.ToBase64String(outputData)
        End Function

        ''' <summary>
        ''' 對稱式解密
        ''' </summary>
        ''' <param name="cipherTextData"></param>
        ''' <param name="key"></param>
        ''' <returns></returns>
        Public Function TextDecrypt(ByVal cipherTextData As Byte(), ByVal key As String) As String
            Dim AES As New RijndaelManaged()
            Dim MD5 As New MD5CryptoServiceProvider()
            Dim keyData As Byte() = MD5.ComputeHash(Encoding.Unicode.GetBytes(key))
            Dim IVData As Byte() = MD5.ComputeHash(Encoding.Unicode.GetBytes("Super SportingApp"))
            AES.Key = keyData
            AES.IV = IVData
            AES.Padding = PaddingMode.PKCS7
            Dim transform As ICryptoTransform = AES.CreateDecryptor()
            ' Dim transform As ICryptoTransform = AES.CreateDecryptor(keyData, IVData)
            Dim outputData As Byte() = transform.TransformFinalBlock(cipherTextData, 0, cipherTextData.Length)
            Return Encoding.Unicode.GetString(outputData)
        End Function

        Public Function TextDecrypt(ByVal encryptData As String, ByVal key As String) As String
            Dim cipherTextData As Byte() = Convert.FromBase64String(encryptData)
            Dim AES As New RijndaelManaged()
            Dim MD5 As New MD5CryptoServiceProvider()
            Dim keyData As Byte() = MD5.ComputeHash(Encoding.Unicode.GetBytes(key))
            Dim IVData As Byte() = MD5.ComputeHash(Encoding.Unicode.GetBytes("Super SportingApp"))
            AES.Key = keyData
            AES.IV = IVData
            AES.Padding = PaddingMode.PKCS7
            Dim transform As ICryptoTransform = AES.CreateDecryptor()
            ' Dim transform As ICryptoTransform = AES.CreateDecryptor(keyData, IVData)
            Dim outputData As Byte() = transform.TransformFinalBlock(cipherTextData, 0, cipherTextData.Length)
            Return Encoding.Unicode.GetString(outputData)
        End Function
    End Module
End Namespace