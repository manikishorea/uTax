using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using CrossLinkAPIService.Crosslinkapi;

namespace CrossLinkAPIService
{
    public static class Data
    {
        static string sqlConnection = ConfigurationManager.ConnectionStrings["sqlConnection"].ToString();
        static string csrsqlConnection = ConfigurationManager.ConnectionStrings["csrsqlConnection"].ToString();

        public static DataTable GetData(string sql, string Connection = "emp")
        {
            string _sqlConnection = Connection == "emp" ? sqlConnection : csrsqlConnection;
            SqlConnection sqlcon = new SqlConnection(_sqlConnection);
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

        public static void CallDefaultStoreProcedure(string SPName, string CustomerId, string UserId, string BankId)
        {
            try
            {

                using (SqlConnection con = new SqlConnection(sqlConnection))
                {
                    using (SqlCommand cmd = new SqlCommand(SPName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@xCustomerId", SqlDbType.VarChar).Value = CustomerId;
                        cmd.Parameters.Add("@xUserId", SqlDbType.VarChar).Value = UserId;
                        cmd.Parameters.Add("@xBankId", SqlDbType.VarChar).Value = BankId;

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
    }

    public enum Entity
    {
        uTax = 1,
        SO = 2,
        SOME = 3,
        SOME_SS = 4,
        MO = 5,
        MO_SO = 6,
        MO_AE = 7,
        MO_AE_SS = 8,
        SVB = 9,
        SVB_SO = 10,
        SVB_MO = 11,
        SVB_MO_SO = 12,
        SVB_MO_AE = 13,
        SVB_MO_AE_SS = 14,
        SVB_AE = 15,
        SVB_AE_SS = 16
    }

    public enum OnboardStatus
    {
        Not_Started = 1,
        Completed = 2,
        Started_But_Incomplete = 3,
        Partner_Declined = 4,
        Not_Applicable = 5
    }

    public class SubmitAppModel
    {
        public AuthObject Auth { get; set; }
        public int BankAppId { get; set; }
        public int EFINId { get; set; }
        public string AccountId { get; set; }
        public string BankCode { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class CreateNewUserModel
    {
        public string MasterIdentifier { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public bool AddBusinessProduct { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? ParentCustomerId { get; set; }
    }

    public class NewUserEmailRequest
    {
        public Guid CustomerId { get; set; }
        public Guid? UserId { get; set; }
    }
}
