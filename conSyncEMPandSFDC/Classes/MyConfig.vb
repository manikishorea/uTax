Friend Class MyConfig

#Region " Private Variables "
    Private Shared _DestDBServerName As String
    Private Shared _DestDBName As String
    Private Shared _DestDBUsername As String
    Private Shared _DestDBPassword As String
    Private Shared _DestDBTrustedConnection As Boolean
    Private Shared _SourceDBServerName As String
    Private Shared _SourceDBName As String
    Private Shared _SourceDBUsername As String
    Private Shared _SourceDBPassword As String
    Private Shared _SourceDBTrustedConnection As Boolean
    Private Shared _IsInit As Boolean = False
    Private Shared _SMTPServer As String
    Private Shared _SMTPPort As String
    Private Shared _SMTPUsername As String
    Private Shared _SMTPPassword As String
    Private Shared _ToEmail As String
    Private Shared _CommandTimeout As Integer
    Private Shared _LinkedDBName As String

#End Region

#Region " Public Methods "
    Public Shared Sub Setup()
        _DestDBServerName = Configuration.ConfigurationManager.AppSettings("DestDBServerName")
        _DestDBName = Configuration.ConfigurationManager.AppSettings("DestDBName")
        _DestDBUsername = Configuration.ConfigurationManager.AppSettings("DestDBUsername")
        _DestDBPassword = Configuration.ConfigurationManager.AppSettings("DestDBPassword")
        _DestDBTrustedConnection = Configuration.ConfigurationManager.AppSettings("DestDBTrustedConnection")
        _SourceDBServerName = Configuration.ConfigurationManager.AppSettings("SourceDBServerName")
        _SourceDBName = Configuration.ConfigurationManager.AppSettings("SourceDBName")
        _SourceDBUsername = Configuration.ConfigurationManager.AppSettings("SourceDBUsername")
        _SourceDBPassword = Configuration.ConfigurationManager.AppSettings("SourceDBPassword")
        _SourceDBTrustedConnection = Configuration.ConfigurationManager.AppSettings("SourceDBTrustedConnection")
        _SMTPServer = Configuration.ConfigurationManager.AppSettings("SMTPServer")
        _SMTPPort = Configuration.ConfigurationManager.AppSettings("SMTPPort")
        _SMTPUsername = Configuration.ConfigurationManager.AppSettings("SMTPUsername")
        _SMTPPassword = Configuration.ConfigurationManager.AppSettings("SMTPPassword")
        _ToEmail = Configuration.ConfigurationManager.AppSettings("ToEmail")
        _CommandTimeout = CType(Configuration.ConfigurationManager.AppSettings("CommandTimeout"), Integer)
        _LinkedDBName = Configuration.ConfigurationManager.AppSettings("LinkedDBName")
        _IsInit = True
    End Sub
#End Region

#Region " Public Readonly Configuration Values "

    Public Shared ReadOnly Property DestDBTrustedConnection() As String
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _DestDBTrustedConnection
        End Get
    End Property

    Public Shared ReadOnly Property SourceDBTrustedConnection() As String
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _SourceDBTrustedConnection
        End Get
    End Property

    Public Shared ReadOnly Property DestDBServerName() As String
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _DestDBServerName
        End Get
    End Property

    Public Shared ReadOnly Property DestDBName() As String
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _DestDBName
        End Get
    End Property

    Public Shared ReadOnly Property DestDBUsername() As String
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _DestDBUsername
        End Get
    End Property

    Public Shared ReadOnly Property DestDBPassword() As String
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _DestDBPassword
        End Get
    End Property

    Public Shared ReadOnly Property SourceDBServerName() As String
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _SourceDBServerName
        End Get
    End Property

    Public Shared ReadOnly Property SourceDBName() As String
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _SourceDBName
        End Get
    End Property

    Public Shared ReadOnly Property SourceDBUsername() As String
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _SourceDBUsername
        End Get
    End Property

    Public Shared ReadOnly Property SourceDBPassword() As String
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _SourceDBPassword
        End Get
    End Property

    Public Shared ReadOnly Property SMTPServer() As String
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _SMTPServer
        End Get
    End Property

    Public Shared ReadOnly Property SMTPPort() As String
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _SMTPPort
        End Get
    End Property

    Public Shared ReadOnly Property SMTPUserName() As String
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _SMTPUsername
        End Get
    End Property

    Public Shared ReadOnly Property SMTPPassword() As String
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _SMTPPassword
        End Get
    End Property

    Public Shared ReadOnly Property ToEmail() As String
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _ToEmail
        End Get
    End Property

    Public Shared ReadOnly Property CommandTimeout() As Integer
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _CommandTimeout
        End Get
    End Property

    Public Shared ReadOnly Property LinkedDBName() As Integer
        Get
            If Not _IsInit Then
                Throw New Exception("Please Invoke the Setup method before accessing configuration values")
            End If
            Return _LinkedDBName
        End Get
    End Property

#End Region

End Class