using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace uTaxEmailService
{
    public partial class Service1 : ServiceBase
    {
        Timer _timer = new Timer();
        static string TemplatePath = ConfigurationManager.AppSettings["TemplatePath"];
        static string URL = ConfigurationManager.AppSettings["URL"];
        static string AdminURL = ConfigurationManager.AppSettings["AdminURL"];

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _timer.Stop();
            _timer.Interval = 60000;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Enabled = false;
            try
            {
                SendEmails();
            }
            catch (Exception ex)
            {
                Data.InsertData("insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" + ex.Message + "','" + Guid.Empty + "','WindowsEmailService/timer',GETDATE())");
            }
            finally
            {
                _timer.Enabled = true;
            }
        }

        protected override void OnStop()
        {
        }

        public void SendEmails()
        {
            try
            {
                bool IsActEmailSend = false;
                DataTable dtSet = Data.GetData("select * from uTaxSettings where StatusCode='ACT'");
                if (dtSet.Rows.Count > 0)
                {
                    IsActEmailSend = Convert.ToBoolean(dtSet.Rows[0]["AccountCreation"]);
                }

                //string content = File.ReadAllText(TemplatePath + "\\NewUser.xml");
                DataTable dt = Data.GetData("select * from EmailNotification where IsSent=0");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];

                    //if (dr["EmailTo"].ToString() != "manikishorea@kensium.com")
                    //    continue;

                    if (dr["EmailType"].ToString() == "1")
                    {
                        if (IsActEmailSend)
                        {
                            string content = File.ReadAllText(TemplatePath + "\\NewUser.xml");
                            string[] Paramenters = dr["Parameters"].ToString().Replace("$|$", "$").Split('$');
                            content = content.Replace("$empUserId$", Paramenters[1]).Replace("$empPassword$", Paramenters[2]).Replace("$url$", URL);//.Replace("$xlinkUserId$", Paramenters[3]).Replace("$xlinkPassword$", Paramenters[4])
                            string Subject = dr["EmailSubject"].ToString();
                            int mail = Data.SendEmail(dr["EmailTo"].ToString(), dr["EmailCC"].ToString(), Subject, content);
                            if (mail == 1)
                            {
                                Data.InsertData("update EmailNotification set IsSent=1,SentDate = GETDATE() where Id=" + dr["Id"].ToString());
                                DataTable dttt = Data.GetData("select * from emp_customerlogininformation where empuserid='" + Paramenters[1] + "'");
                                if (dttt.Rows.Count > 0)
                                {
                                    Data.InsertData("update emp_customerinformation set StatusCode ='ACT' where id='" + dttt.Rows[0]["customerofficeid"] + "'");
                                    Data.InsertData("update Officemanagement set StatusCode ='ACT' where customerid='" + dttt.Rows[0]["customerofficeid"] + "'");
                                }
                            }
                        }
                    }
                    else if (dr["EmailType"].ToString() == "2")
                    {
                        string content = File.ReadAllText(TemplatePath + "\\SubsiteCreation.xml");
                        string[] Paramenters = dr["Parameters"].ToString().Replace("$|$", "$").Split('$');
                        content = content.Replace("$Companyname$", Paramenters[1]);
                        int mail = Data.SendEmail(dr["EmailTo"].ToString(), dr["EmailCC"].ToString(), "New Subsite Created", content);
                        if (mail == 1)
                            Data.InsertData("update EmailNotification set IsSent=1,SentDate = GETDATE() where Id=" + dr["Id"].ToString());
                    }
                    else if (dr["EmailType"].ToString() == "3")
                    {
                        string content = File.ReadAllText(TemplatePath + "\\HoldUnhold.xml");
                        string[] Paramenters = dr["Parameters"].ToString().Replace("$|$", "$").Split('$');
                        content = content.Replace("$HoldStatus$", Paramenters[3]).Replace("$UserID$", Paramenters[0]).Replace("$MasterID$", Paramenters[1]).Replace("$ParentUserID$", Paramenters[2]);
                        int mail = Data.SendEmail(dr["EmailTo"].ToString(), dr["EmailCC"].ToString(), dr["EmailSubject"].ToString(), content);
                        if (mail == 1)
                            Data.InsertData("update EmailNotification set IsSent=1,SentDate = GETDATE() where Id=" + dr["Id"].ToString());
                    }
                    else if (dr["EmailType"].ToString() == "4")
                    {
                        string content = File.ReadAllText(TemplatePath + "\\BusinessSoftware.xml");
                        string[] Paramenters = dr["Parameters"].ToString().Replace("$|$", "$").Split('$');
                        content = content.Replace("$UserID$", Paramenters[0]).Replace("$MasterID$", Paramenters[1]).Replace("$swstatus$", Paramenters[2]);
                        int mail = Data.SendEmail(dr["EmailTo"].ToString(), dr["EmailCC"].ToString(), dr["EmailSubject"].ToString(), content);
                        if (mail == 1)
                            Data.InsertData("update EmailNotification set IsSent=1,SentDate = GETDATE() where Id=" + dr["Id"].ToString());
                    }
                    else if (dr["EmailType"].ToString() == "5")
                    {
                        int mail = Data.SendEmail(dr["EmailTo"].ToString(), dr["EmailCC"].ToString(), dr["EmailSubject"].ToString(), dr["EmailContent"].ToString());
                        if (mail == 1)
                            Data.InsertData("update EmailNotification set IsSent=1,SentDate = GETDATE() where Id=" + dr["Id"].ToString());
                    }
                    else if(dr["EmailType"].ToString() == "6")
                    {
                        string content = File.ReadAllText(TemplatePath + "\\HoldUser.xml");
                        string[] Paramenters = dr["Parameters"].ToString().Replace("$|$", "$").Split('$');
                        content = content.Replace("{{ContactName}}", Paramenters[0]).Replace("{{HoldDesc}}", Paramenters[1]);
                        int mail = Data.SendEmail(dr["EmailTo"].ToString(), dr["EmailCC"].ToString(), dr["EmailSubject"].ToString(), content);
                        if (mail == 1)
                            Data.InsertData("update EmailNotification set IsSent=1,SentDate = GETDATE() where Id=" + dr["Id"].ToString());
                    }
                    else if (dr["EmailType"].ToString() == "7")
                    {
                        string content = File.ReadAllText(TemplatePath + "\\NewAdminUser.xml");
                        string[] Paramenters = dr["Parameters"].ToString().Replace("$|$", "$").Split('$');
                        content = content.Replace("$empUserId$", Paramenters[0]).Replace("$empPassword$", Paramenters[1]).Replace("$url$", AdminURL);
                        int mail = Data.SendEmail(dr["EmailTo"].ToString(), dr["EmailCC"].ToString(), dr["EmailSubject"].ToString(), content);
                        if (mail == 1)
                            Data.InsertData("update EmailNotification set IsSent=1,SentDate = GETDATE() where Id=" + dr["Id"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Data.InsertData("insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" + ex.Message + "','" + Guid.Empty + "','WindowsEmailService/SendEmails',GETDATE())");
            }
        }
    }
}
