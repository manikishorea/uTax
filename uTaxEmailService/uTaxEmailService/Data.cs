using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace uTaxEmailService
{
    public static class Data
    {
        static string sqlConnection = ConfigurationManager.ConnectionStrings["sqlConnection"].ToString();
        static string SMTPServer = ConfigurationManager.AppSettings["SMTPServer"].ToString();
        static string SMTPPort = ConfigurationManager.AppSettings["SMTPPort"].ToString();
        static string SMTPUsername = ConfigurationManager.AppSettings["SMTPUsername"].ToString();
        static string SMTPPassword = ConfigurationManager.AppSettings["SMTPPassword"].ToString();

        public static DataTable GetData(string sql)
        {
            SqlConnection sqlcon = new SqlConnection(sqlConnection);
            SqlDataAdapter adap = new SqlDataAdapter(sql, sqlcon);
            DataTable dt = new DataTable();
            adap.Fill(dt);
            return dt;
        }

        public static string InsertData(string sqlQry)
        {
            try
            {
                SqlConnection sqlcon = new SqlConnection(sqlConnection);
                SqlCommand cmd = new SqlCommand(sqlQry, sqlcon);
                sqlcon.Open();
                cmd.ExecuteNonQuery();
                sqlcon.Close();
                sqlcon.Dispose();
                return "";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void CallStoreProcedure(string SPName, string CustomerId, string SalesYear, string RootId)
        {
            try
            {

                using (SqlConnection con = new SqlConnection(sqlConnection))
                {
                    using (SqlCommand cmd = new SqlCommand(SPName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@xCustomerId", SqlDbType.VarChar).Value = CustomerId;
                        cmd.Parameters.Add("@xSalesYear", SqlDbType.VarChar).Value = SalesYear;
                        cmd.Parameters.Add("@xRootParentId", SqlDbType.VarChar).Value = RootId;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Data.InsertData("insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" + ex.ToString() + "','" + Guid.Empty + "','WindowsService/CallStoreProcedure',GETDATE())");
            }
        }

        public static int SendEmail(string ToEmailIDs, string CCEmailIDs, string Subject, string Body)
        {
            string strGUID = Guid.NewGuid().ToString();
            try
            {
                SmtpClient oSMTPClient = new SmtpClient(SMTPServer, Convert.ToInt32(SMTPPort));
                oSMTPClient.EnableSsl = true;
                System.Net.NetworkCredential myCredentials = new System.Net.NetworkCredential(SMTPUsername, SMTPPassword);
                oSMTPClient.Credentials = myCredentials;
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                msg.From = new MailAddress(SMTPUsername);
                msg.To.Add(ToEmailIDs);
                if (CCEmailIDs != "")
                    msg.CC.Add(CCEmailIDs);
                msg.Subject = Subject;
                msg.Body = Body;
                msg.IsBodyHtml = true;
                try
                {
                    oSMTPClient.Send(msg);
                }
                catch (Exception ex)
                {
                    Data.InsertData("insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" + ex.Message + "','" + Guid.Empty + "','WindowsEmailService/DataSendEmail',GETDATE())");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Data.InsertData("insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" + ex.Message + "','" + Guid.Empty + "','WindowsEmailService/DataSendEmail',GETDATE())");
                return 0;
            }
            return 1;
        }
    }
}

//<br/>
//Crosslink account has been created with the following details: <br/>
//<b>Crosslink User Id : </b> $xlinkUserId$ <br/>
//<b>Crosslink Password : </b> $xlinkPassword$ <br/>