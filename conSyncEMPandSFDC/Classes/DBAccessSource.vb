Public Class DBAccessSource

    Private Shared _SqlCnx As SqlClient.SqlConnection

    Public Enum EnumDBRetval
        Success = 1
        Failure = -1
    End Enum

#Region " Standard Init Logic"

    Public Shared Sub Init(ByVal DBServer As String, ByVal DBName As String, ByVal DBUsername As String, ByVal DBPwd As String, ByVal IsTrustedConnection As Boolean)
        If _SqlCnx Is Nothing Then
            Dim retu As Boolean
            retu = _OpenDBCnx(DBServer, DBName, DBUsername, DBPwd, IsTrustedConnection)
            If Not retu Then
                Throw New Exception("Unable to open database connection, Please contact your Administrator immediately.")
            End If
        End If
    End Sub

    Private Shared Function _OpenDBCnx(ByVal DBServer As String, ByVal DBName As String, ByVal DBUsername As String, ByVal DBPwd As String, ByVal IsTrustedConnection As Boolean) As Boolean
        If IsTrustedConnection Then
            _SqlCnx = New SqlClient.SqlConnection("Server=" & DBServer & ";Database=" & DBName & ";Trusted_Connection=True")
        Else
            _SqlCnx = New SqlClient.SqlConnection("Server=" & DBServer & ";Database=" & DBName & ";User ID=" & DBUsername & ";Password=" & DBPwd & ";Trusted_Connection=False")
        End If
        Return _OpenDBCnx()
    End Function

    Private Shared Function _OpenDBCnx() As Boolean
        Try
            _SqlCnx.Close()
        Catch ex As Exception
        End Try

        Try
            _SqlCnx.Open()
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

#End Region

    Public Shared Function ExecuteQuery(ByVal strQuery As String) As DataSet
        Dim sqlCmd As SqlClient.SqlCommand
        Dim ds As DataSet = New DataSet
        sqlCmd = New SqlClient.SqlCommand(strQuery, _SqlCnx)
        sqlCmd.CommandType = CommandType.Text
        sqlCmd.CommandTimeout = MyConfig.CommandTimeout
        Dim sqlDA As SqlClient.SqlDataAdapter
        sqlDA = New SqlClient.SqlDataAdapter(sqlCmd)
        ds = New DataSet
        sqlDA.Fill(ds)
        Return ds
    End Function

    Public Shared Function InvokeSP_UpdateSFAccount(ByVal AccountId As String, ByVal FieldName As String, ByVal FieldValue As String) As DataSet
        Dim sqlCmd As SqlClient.SqlCommand
        sqlCmd = New SqlClient.SqlCommand("SF_UpdateAccount", _SqlCnx)
        sqlCmd.CommandType = CommandType.StoredProcedure
        sqlCmd.Parameters.Add(New SqlClient.SqlParameter("@RETURN_VALUE", SqlDbType.Int))
        sqlCmd.Parameters(0).Direction = ParameterDirection.ReturnValue
        sqlCmd.Parameters.Add(New SqlClient.SqlParameter("@id", SqlDbType.NVarChar, 18))
        sqlCmd.Parameters(1).Value = AccountId
        sqlCmd.Parameters.Add(New SqlClient.SqlParameter("@field", SqlDbType.NVarChar, 50))
        sqlCmd.Parameters(2).Value = FieldName
        sqlCmd.Parameters.Add(New SqlClient.SqlParameter("@value", SqlDbType.NVarChar, 200))
        sqlCmd.Parameters(3).Value = FieldValue
        Dim sqlDA As SqlClient.SqlDataAdapter
        sqlDA = New SqlClient.SqlDataAdapter(sqlCmd)
        Dim ds As DataSet
        ds = New DataSet
        sqlDA.Fill(ds)
        Return ds
    End Function

    Public Shared Function InvokeSP_UpdateSFContact(ByVal AccountId As String, ByVal FieldName As String, ByVal FieldValue As String) As DataSet
        Dim sqlCmd As SqlClient.SqlCommand
        sqlCmd = New SqlClient.SqlCommand("SF_UpdateContact", _SqlCnx)
        sqlCmd.CommandType = CommandType.StoredProcedure
        sqlCmd.Parameters.Add(New SqlClient.SqlParameter("@RETURN_VALUE", SqlDbType.Int))
        sqlCmd.Parameters(0).Direction = ParameterDirection.ReturnValue
        sqlCmd.Parameters.Add(New SqlClient.SqlParameter("@id", SqlDbType.NVarChar, 18))
        sqlCmd.Parameters(1).Value = AccountId
        sqlCmd.Parameters.Add(New SqlClient.SqlParameter("@field", SqlDbType.NVarChar, 50))
        sqlCmd.Parameters(2).Value = FieldName
        sqlCmd.Parameters.Add(New SqlClient.SqlParameter("@value", SqlDbType.NVarChar, 500))
        sqlCmd.Parameters(3).Value = FieldValue
        Dim sqlDA As SqlClient.SqlDataAdapter
        sqlDA = New SqlClient.SqlDataAdapter(sqlCmd)
        Dim ds As DataSet
        ds = New DataSet
        sqlDA.Fill(ds)
        Return ds
    End Function

    Public Shared Function InvokeSP_UpdateSFOpportunity(ByVal AccountId As String, ByVal FieldName As String, ByVal FieldValue As String) As DataSet
        Dim sqlCmd As SqlClient.SqlCommand
        sqlCmd = New SqlClient.SqlCommand("SF_UpdateOpportunity", _SqlCnx)
        sqlCmd.CommandType = CommandType.StoredProcedure
        sqlCmd.Parameters.Add(New SqlClient.SqlParameter("@RETURN_VALUE", SqlDbType.Int))
        sqlCmd.Parameters(0).Direction = ParameterDirection.ReturnValue
        sqlCmd.Parameters.Add(New SqlClient.SqlParameter("@id", SqlDbType.NVarChar, 18))
        sqlCmd.Parameters(1).Value = AccountId
        sqlCmd.Parameters.Add(New SqlClient.SqlParameter("@field", SqlDbType.NVarChar, 50))
        sqlCmd.Parameters(2).Value = FieldName
        sqlCmd.Parameters.Add(New SqlClient.SqlParameter("@value", SqlDbType.NVarChar, 500))
        sqlCmd.Parameters(3).Value = FieldValue
        Dim sqlDA As SqlClient.SqlDataAdapter
        sqlDA = New SqlClient.SqlDataAdapter(sqlCmd)
        Dim ds As DataSet
        ds = New DataSet
        sqlDA.Fill(ds)
        Return ds
    End Function

End Class
