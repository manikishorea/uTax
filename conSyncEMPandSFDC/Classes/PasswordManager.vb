Imports System.Text
Imports System.Security.Cryptography
Imports System.Reflection
Imports System.ComponentModel
Imports System.IO

Public Class PasswordManager

  Private Shared key As Byte() = New Byte(7) {1, 2, 3, 4, 5, 6, _
   7, 8}
  Private Shared iv As Byte() = New Byte(7) {1, 2, 3, 4, 5, 6, _
   7, 8}

  Private Shared PasswordKey As String = "PAS$WO&3"
  Private Shared PasswordIV As String = "PAS$WO&3"
  ''' <summary>
  ''' This method is used to hash the input string based on a Key specified
  ''' </summary>
  ''' <param name="inputString"></param>
  ''' <param name="Key"></param>
  ''' <param name="hasher"></param>
  ''' <returns></returns>
  Public Shared Function HashText(inputString As String, Key As String) As String
    Try
      Dim textWithSaltBytes As Byte() = Encoding.UTF8.GetBytes(String.Concat(inputString, Key))
      Dim hasher As HashAlgorithm = New SHA1CryptoServiceProvider()
      Dim hashedBytes As Byte() = hasher.ComputeHash(textWithSaltBytes)
      hasher.Clear()
      Return Convert.ToBase64String(hashedBytes)
    Finally
    End Try
  End Function

  ''' <summary>
  ''' To get a random 8 digit alphanumeric string
  ''' </summary>
  ''' <returns>random 8 digit alphanumeric string</returns>
  Public Shared Function GetRandomString() As String
    Try
      Return Guid.NewGuid().ToString().Substring(0, 8)
    Finally
    End Try
  End Function


  ''' <summary>
  ''' 
  ''' </summary>
  ''' <param name="text"></param>
  ''' <returns></returns>
  Public Shared Function CryptText(text As String) As String
    Dim KeySaltBytes As Byte() = Encoding.UTF8.GetBytes(PasswordKey)
    Dim VISaltBytes As Byte() = Encoding.UTF8.GetBytes(PasswordIV)

    Dim algorithm As SymmetricAlgorithm = DES.Create()
    Dim transform As ICryptoTransform = algorithm.CreateEncryptor(KeySaltBytes, VISaltBytes)
    Dim inputbuffer As Byte() = Encoding.Unicode.GetBytes(text)
    Dim outputBuffer As Byte() = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length)
    Return Convert.ToBase64String(outputBuffer)
  End Function


  ''' <summary>
  ''' 
  ''' </summary>
  ''' <param name="text"></param>
  ''' <returns></returns>
  Public Shared Function DecryptText(text As String) As String
    Dim KeySaltBytes As Byte() = Encoding.UTF8.GetBytes(PasswordKey)
    Dim VISaltBytes As Byte() = Encoding.UTF8.GetBytes(PasswordIV)

    Dim algorithm As SymmetricAlgorithm = DES.Create()
    Dim transform As ICryptoTransform = algorithm.CreateDecryptor(KeySaltBytes, VISaltBytes)
    Dim inputbuffer As Byte() = Convert.FromBase64String(text)
    Dim outputBuffer As Byte() = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length)
    Return Encoding.Unicode.GetString(outputBuffer)
  End Function

  ''' <summary>
  ''' 
  ''' </summary>
  ''' <param name="text"></param>
  ''' <returns></returns> 
  Public Shared Function Crypt(text As String) As String
    Dim algorithm As SymmetricAlgorithm = DES.Create()
    Dim transform As ICryptoTransform = algorithm.CreateEncryptor(key, iv)
    Dim inputbuffer As Byte() = Encoding.Unicode.GetBytes(text)
    Dim outputBuffer As Byte() = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length)
    Return Convert.ToBase64String(outputBuffer)
  End Function

  ''' <summary>
  ''' 
  ''' </summary>
  ''' <param name="text"></param>
  ''' <returns></returns> 
  Public Shared Function Decrypt(text As String) As String
    Dim algorithm As SymmetricAlgorithm = DES.Create()
    Dim transform As ICryptoTransform = algorithm.CreateDecryptor(key, iv)
    Dim inputbuffer As Byte() = Convert.FromBase64String(text)
    Dim outputBuffer As Byte() = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length)
    Return Encoding.Unicode.GetString(outputBuffer)
  End Function

  ''' <summary>
  ''' 
  ''' </summary>
  ''' <param name="value"></param>
  ''' <returns></returns>
  Public Shared Function GetEnumDescription(value As [Enum]) As String
    Dim fi As FieldInfo = value.[GetType]().GetField(value.ToString())

    Dim attributes As DescriptionAttribute() = DirectCast(fi.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())

    If attributes IsNot Nothing AndAlso attributes.Length > 0 Then
      Return attributes(0).Description
    Else
      Return value.ToString()
    End If
  End Function

  ''' <summary>
  ''' 
  ''' </summary>
  Private Shared KEY_192 As Byte() = {42, 16, 93, 156, 78, 4, _
   218, 32, 15, 167, 44, 80, _
   26, 250, 155, 112, 2, 94, _
   11, 204, 119, 35, 184, 197}

  ''' <summary>
  ''' 
  ''' </summary>
  Private Shared IV_192 As Byte() = {55, 103, 246, 79, 36, 99, _
   167, 3, 42, 5, 62, 83, _
   184, 7, 209, 13, 145, 23, _
   200, 58, 173, 10, 121, 222}
  '    //TRIPLE DES encryption
  '    private static string EncryptTripleDES(string value)
  '    {
  '        TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider();
  '        MemoryStream ms = new MemoryStream();
  '        CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateEncryptor(KEY_192, IV_192), CryptoStreamMode.Write);
  '        StreamWriter sw = new StreamWriter(cs);

  '        sw.Write(value);
  '        sw.Flush();
  '        cs.FlushFinalBlock();
  '        ms.Flush();

  '        //convert back to a string
  '        return Convert.ToBase64String(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
  '    }

  'TRIPLE DES decryption 
  Private Shared Function DecryptTripleDES(value As String) As String
    Dim cryptoProvider As New TripleDESCryptoServiceProvider()
    'convert from string to byte array
    Dim buffer As Byte() = Convert.FromBase64String(value)
    Dim ms As New MemoryStream(buffer)
    Dim cs As New CryptoStream(ms, cryptoProvider.CreateDecryptor(KEY_192, IV_192), CryptoStreamMode.Read)
    Dim sr As New StreamReader(cs)
    Return sr.ReadToEnd()
  End Function

  ''' <summary>
  ''' 
  ''' </summary>
  ''' <param name="ActivationKey"></param>
  ''' <returns></returns>
  Public Shared Function GetAttributesFromKey(ActivationKey As String) As String()
    Dim DecriptActivationKey As String = DecryptTripleDES(ActivationKey)
    Dim ActivationKeysWithAttributes As String() = DecriptActivationKey.Split("^"c)
    Dim ArrayAttributes As String() = Nothing
    Dim stringDeCriptedAttributes As String = Nothing
    Dim ArrayDeCriptedAttributes As String() = Nothing
    Dim AttributeKey As String = String.Empty
    If ActivationKeysWithAttributes.Length > 1 Then
      ActivationKey = ActivationKeysWithAttributes(0).ToString()
      AttributeKey = ActivationKeysWithAttributes(1).ToString()
    End If
    If Not String.IsNullOrEmpty(AttributeKey) Then
      ArrayAttributes = AttributeKey.Split("*"c)
      For Each itemAttribute As String In ArrayAttributes
        If String.IsNullOrEmpty(stringDeCriptedAttributes) Then
          stringDeCriptedAttributes = DecryptTripleDES(itemAttribute)
        Else
          stringDeCriptedAttributes += Convert.ToString("*") & DecryptTripleDES(itemAttribute)

        End If
      Next

      If Not String.IsNullOrEmpty(stringDeCriptedAttributes) Then
        ArrayDeCriptedAttributes = stringDeCriptedAttributes.Split("*"c)


      End If
    End If
    Return ArrayDeCriptedAttributes

  End Function

End Class
