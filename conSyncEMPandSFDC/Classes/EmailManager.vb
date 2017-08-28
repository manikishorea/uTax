Imports System.Net.Mail

Public Class EmailManager

    Public Shared Function SendEmail(Subject As String, Body As String) As Integer
        Dim strGUID As String = Guid.NewGuid().ToString()
        Try
            Dim oSMTPClient As New SmtpClient(MyConfig.SMTPServer, Convert.ToInt32(MyConfig.SMTPPort))
            oSMTPClient.EnableSsl = True
            Dim myCredentials As New System.Net.NetworkCredential(MyConfig.SMTPUserName, MyConfig.SMTPPassword)
            oSMTPClient.Credentials = myCredentials
            Dim msg As New System.Net.Mail.MailMessage()
            msg.From = New MailAddress(MyConfig.SMTPUserName)
            msg.[To].Add(MyConfig.ToEmail)
            msg.Subject = Subject
            msg.Body = Body
            msg.IsBodyHtml = True
            Try
                oSMTPClient.Send(msg)
            Catch ex As Exception
                Return 0
            End Try
        Catch ex As Exception
            Return 0
        End Try
        Return 1
    End Function

End Class
