using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using CrossLinkAPIService.Crosslinkapi;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Globalization;
using System.Net.Http;
using System.Web.Script.Serialization;

namespace CrossLinkAPIService
{
    public partial class Service1 : ServiceBase
    {
        Timer _timer = new Timer();
        CrosslinkWS17SoapClient _apiObj = new CrosslinkWS17SoapClient();
        DataTable dtSalesYear = new DataTable();
        DataTable dtBankData = new DataTable();
        static string ReadyState = ConfigurationManager.AppSettings["ReadyState"];
        static string SubmitState = ConfigurationManager.AppSettings["SubmitState"];
        static string ApprovedState = ConfigurationManager.AppSettings["ApprovedState"];
        static string RejectedState = ConfigurationManager.AppSettings["RejectedState"];
        static string PendingState = ConfigurationManager.AppSettings["PendingState"];
        public string DeniedState = ConfigurationManager.AppSettings["DeniedState"];
        public string CancelledState = ConfigurationManager.AppSettings["CancelledState"];
        static string TPGBank = ConfigurationManager.AppSettings["TPGBank"];
        static string RABank = ConfigurationManager.AppSettings["RABank"];
        static string RBBank = ConfigurationManager.AppSettings["RBBank"];
        static string SVB_MO_AE_SS = ConfigurationManager.AppSettings["SVB_MO_AE_SS"];
        static string SVB_AE_SS = ConfigurationManager.AppSettings["SVB_AE_SS"];
        static string MO_AE_SS = ConfigurationManager.AppSettings["MO_AE_SS"];
        static string SOME_SS = ConfigurationManager.AppSettings["SOME_SS"];

        string sSource;
        string sLog;

        //UserID: 19464
        //Password: D09A1G1

        public Service1()
        {
            InitializeComponent();
            sSource = "CrossLink API Service";
            sLog = "Application";

            //if (!EventLog.SourceExists(sSource))
            //    EventLog.CreateEventSource(sSource, sLog);
        }

        public void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Enabled = false;
            try
            {
                //SubmitBankApplications();
                UpdateBankStatus();
                //UpdateEnrollmentData();
                //CreateNewUsers();
                UpdateOnboardStatus();
            }
            catch (Exception ex)
            {
                Data.InsertData("insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" + ex.Message + "','" + Guid.Empty + "',WindowsService/timer,GETDATE())");
            }
            finally
            {
                _timer.Enabled = true;
            }
        }

        protected override void OnStart(string[] args)
        {
            _timer.Stop();
            _timer.Interval = 180000;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            dtSalesYear = getActiceSalesYear();
            dtBankData = getBankData();
            _timer.Start();
        }

        protected override void OnStop()
        {
        }

        public DataTable getActiceSalesYear()
        {
            DataTable dt = new DataTable();
            dt = Data.GetData("select * from SalesYearMaster where ApplicableFromDate <= GETDATE() and ApplicableToDate >=GETDATE()");
            return dt;
        }

        public DataTable getBankData()
        {
            DataTable dt = new DataTable();
            dt = Data.GetData("select * from BankMaster where StatusCode = 'ACT'");
            return dt;
        }

        public void SubmitBankApplications()
        {

            //Data.InsertData("insert into ExceptionLog (ExceptionMessage,CreatedDateTime) values('started',GETDATE())");

            DataTable dt = Data.GetData(@"select c.EFIN,b.BankId,bm.BankCode,b.CustomerId,l.CrossLinkUserId,l.CrossLinkPassword,l.MasterIdentifier,b.Id,c.ParentId
                                        from BankEnrollment b 
                                        join emp_CustomerInformation c on b.CustomerId = c.Id 
                                        join emp_CustomerLoginInformation l on c.Id = l.CustomerOfficeId
                                        join BankMaster bm on b.BankId = bm.Id where b.StatusCode = '" + ReadyState + "' and b.IsActive=1");
            foreach (DataRow appln in dt.Rows)
            {
                try
                {
                    string UserId = appln["MasterIdentifier"].ToString(); //ConfigurationManager.AppSettings["UserID"];//
                    string Password = appln["CrossLinkPassword"].ToString();//ConfigurationManager.AppSettings["Password"]; //
                    string bankCode = appln["BankCode"].ToString();
                    Guid bankId = new Guid(appln["BankId"].ToString());
                    int efin = Convert.ToInt32(appln["EFIN"]);
                    Guid CustId = new Guid(appln["CustomerId"].ToString());
                    int LoginUserId = Convert.ToInt32(appln["CrossLinkUserId"]);
                    Guid EnrollmentId = new Guid(appln["Id"].ToString());
                    string ParentId = appln["ParentId"].ToString();

                    string _accesskey = _apiObj.getAccessKey(UserId, Password);
                    if (_accesskey != "")
                    {
                        AuthObject _objAuth = new AuthObject();
                        _objAuth.AccessKey = _accesskey;
                        _objAuth.UserID = UserId;

                        // checking the Authentication
                        XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                        if (isValid.success)
                        {
                            LogEvent("Updating the application for Customer " + CustId + ", bank " + bankCode, EventLogEntryType.Information, CustId, 1, bankId, EnrollmentId);

                            int efinid = _apiObj.getEFINID(efin);

                            var efinres = UpdateEfinObject(_objAuth, efin, efinid, UserId, CustId, ParentId, LoginUserId, bankId, EnrollmentId, bankCode);
                            if (efinres.success)
                            {
                                XlinkResponse result = new XlinkResponse();

                                // getting the latest App by bank
                                var latestbankApp = _apiObj.getLatestAppbyBank(_objAuth, efinid, bankCode);
                                if (bankCode == TPGBank)
                                {
                                    // updating the SBTPG bank
                                    result = UpdateTPGBankApp(CustId, _objAuth, efinid, latestbankApp.BankAppID, latestbankApp, LoginUserId, bankId, EnrollmentId);
                                }
                                else if (bankCode == RABank)
                                {
                                    // update RA Owner object
                                    var ownerres = UpdateRAOwnerObject(CustId.ToString(), latestbankApp.BankAppID, _objAuth, LoginUserId);
                                    if (ownerres.success)
                                        // updating Refund Advantage Bank
                                        result = UpdateRABankApp(CustId, _objAuth, efinid, latestbankApp.BankAppID, latestbankApp, LoginUserId, bankId, EnrollmentId);
                                }
                                else if (bankCode == RBBank)
                                {
                                    // updating republic Bank
                                    result = UpdateRBBankApp(CustId, _objAuth, efinid, latestbankApp.BankAppID, latestbankApp, LoginUserId, bankId, EnrollmentId);
                                }

                                if (result.success)
                                {
                                    var banks = _apiObj.getBankApps(_objAuth, efinid);
                                    var bankappId = banks.Where(x => x.BankID == bankCode).Select(x => x.BankAppID).FirstOrDefault();

                                    LogEvent("Subitting the application for Customer " + CustId + ", bank " + bankCode, EventLogEntryType.Information, CustId, 1, bankId, EnrollmentId);

                                    string accountId = string.IsNullOrEmpty(latestbankApp.AccountID) ? UserId : latestbankApp.AccountID;

                                    string xml = getsubmitBankAppXml(_objAuth, bankappId, efinid, accountId, bankCode, accountId);

                                    //Submitting the bank Application
                                    var df = _apiObj.submitBankApplication(_objAuth, bankappId, efinid, accountId, bankCode, accountId);
                                    if (df.success)
                                    {
                                        Data.InsertData("Update BankEnrollment set StatusCode = '" + SubmitState + "' where CustomerId = '" + CustId + "' and IsActive=1");
                                        LogStatus("Submitted the Application", EnrollmentId);
                                        LogEvent("Submitted the application", EventLogEntryType.SuccessAudit, CustId, 1, bankId, EnrollmentId, xml);
                                    }
                                    else
                                    {
                                        LogEvent("Submission failed :: " + string.Join(",", df.message.ToArray()), EventLogEntryType.FailureAudit, CustId, 0, bankId, EnrollmentId, xml);
                                    }
                                }
                            }
                        }
                        else
                        {
                            LogEvent("Authentication Failed :: " + string.Join(",", isValid.message.ToArray()), EventLogEntryType.FailureAudit, CustId, 0, bankId, EnrollmentId);
                        }
                    }
                    else
                    {
                        LogEvent("Accesskey not found ", EventLogEntryType.FailureAudit, CustId, 0, bankId, EnrollmentId);
                    }
                }
                catch (Exception ex)
                {
                    Data.InsertData("insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" + ex.Message + "','" + Guid.Empty + "','WindowsService/SubmitBankApplication',GETDATE())");
                }
            }
        }

        public XlinkResponse UpdateRAOwnerObject(string CustomerId, int AppId, AuthObject auth, int UserId)
        {
            var owners = _apiObj.getRefundAdvantageOwners(auth, AppId);
            foreach (var owner in owners)
            {
                XlinkResponse delres = _apiObj.deleteOwner(auth, owner);
            }

            DataTable dtcust = Data.GetData("select Id from BankEnrollmentForRA where CustomerId ='" + CustomerId + "' and StatusCode='ACT'");
            if (dtcust.Rows.Count > 0)
            {
                DataTable dt = Data.GetData("select * from BankEnrollmentEFINOwnersForRA where BankEnrollmentRAId='" + dtcust.Rows[0][0] + "'");
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        RefundAdvantageOwnerObject _onwerObj = new RefundAdvantageOwnerObject();
                        _onwerObj.Address = item["Address"].ToString();
                        _onwerObj.City = item["City"].ToString();
                        _onwerObj.DOB = DateTime.ParseExact(item["DateofBirth"].ToString(), "MM'/'dd'/'yyyy", CultureInfo.InvariantCulture);
                        _onwerObj.FirstName = item["FirstName"].ToString();
                        _onwerObj.IdNumber = item["IDNumber"].ToString();
                        _onwerObj.IdState = item["IDState"].ToString();
                        _onwerObj.LastName = item["LastName"].ToString();
                        _onwerObj.PercentOwned = Convert.ToInt32(item["PercentageOwned"]);
                        _onwerObj.Phone = item["HomePhone"].ToString().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                        _onwerObj.Refund_Advantage_BankAppID = AppId;
                        //_onwerObj.Refund_Advantage_Business_OwnerID = UserId;
                        _onwerObj.SSN = item["SSN"].ToString();
                        _onwerObj.State = item["StateId"].ToString();
                        _onwerObj.Zip = item["ZipCode"].ToString();

                        string xml = getRAOwnerObjecyXml(_onwerObj);

                        XlinkResponse res = _apiObj.updateRefundAdvantageOwner(auth, _onwerObj, false);
                        if (!res.success)
                        {
                            LogEvent("RAOwner Update failed :: " + string.Join(",", res.message.ToArray()), EventLogEntryType.FailureAudit, new Guid(CustomerId), 0, Guid.Empty, new Guid(dtcust.Rows[0][0].ToString()), xml);
                            return res;
                        }
                        else
                            LogEvent("RAOwner Updated for CustId :: " + CustomerId + " " + string.Join(",", res.message.ToArray()), EventLogEntryType.FailureAudit, new Guid(CustomerId), 1, Guid.Empty, new Guid(dtcust.Rows[0][0].ToString()), xml);

                    }
                    return new XlinkResponse() { success = true };
                }
                else
                    return new XlinkResponse() { success = false };
            }
            else
                return new XlinkResponse() { success = false };
        }

        private string getRAOwnerObjecyXml(RefundAdvantageOwnerObject _onwerObj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(RefundAdvantageOwnerObject));
            StringWriter sww = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, _onwerObj);
                var xml = sww.ToString();
                return xml;
            }
        }

        public XlinkResponse UpdateEfinObject(AuthObject auth, int Efin, int EfinId, string AccountId, Guid CustomerId, string ParentId, int UserId, Guid BankId, Guid EnrollmentId, string BankCode)
        {
            string Qry = "";
            if (BankCode == TPGBank)
                Qry = "select * from BankEnrollmentForTPG where CustomerId='" + CustomerId + "' and StatusCode='ACT'";
            else if (BankCode == RABank)
                Qry = "select * from BankEnrollmentForRA where CustomerId='" + CustomerId + "' and StatusCode='ACT'";
            else
                Qry = "select * from BankEnrollmentForRB where CustomerId='" + CustomerId + "' and StatusCode='ACT'";

            DataTable bankEnrollInfo = Data.GetData(Qry);
            if (bankEnrollInfo.Rows.Count > 0)
            {
                var info = bankEnrollInfo.Rows[0];
                EfinObject _objefin = GetUpdateEfinObject(BankCode, info, AccountId, Efin, EfinId, ParentId, UserId);

                string xml = getEfinObjecyXml(_objefin);
                XlinkResponse isValid = _apiObj.validateEfinObject(_objefin, false);
                if (!isValid.success)
                {
                    if (isValid.message != null)
                    {
                        LogEvent("Efin Update failed :: " + string.Join(",", isValid.message.ToArray()), EventLogEntryType.FailureAudit, CustomerId, 0, BankId, EnrollmentId, xml);
                    }
                    return isValid;
                }
                XlinkResponse res = _apiObj.updateEFIN(auth, _objefin, AccountId, 0, false);
                if (!res.success)
                {
                    if (res.message != null)
                    {
                        LogEvent("Efin Update failed :: " + string.Join(",", res.message.ToArray()), EventLogEntryType.FailureAudit, CustomerId, 0, BankId, EnrollmentId, xml);
                    }
                    return isValid;
                }
                return res;
            }
            else
                return new XlinkResponse() { success = false };
        }

        public EfinObject GetUpdateEfinObject(string bank, DataRow bankEnrollInfo, string AccountId, int Efin, int EfinId, string ParentId, int UserId)
        {
            var info = bankEnrollInfo;

            #region "TPG"

            if (bank == TPGBank)
            {
                EfinObject _objefin = new EfinObject();
                _objefin.AccountID = AccountId;
                _objefin.AcctName = info["AccountName"].ToString();
                _objefin.AcctType = info["OfficeAccountType"].ToString();
                _objefin.Address = info["EFINOwnerAddress"].ToString();
                _objefin.AgreePEIDate = DateTime.Now;
                _objefin.AgreePEITerms = true;
                _objefin.BankName = info["BankName"].ToString();
                _objefin.City = info["EFINOwnerCity"].ToString();
                _objefin.Company = info["CompanyName"].ToString();
                _objefin.CreatedBy = AccountId;
                _objefin.CreatedDate = DateTime.Now;
                _objefin.DAN = info["OfficeDAN"].ToString();
                _objefin.DOB = Convert.ToDateTime(info["EFINOwnerDOB"]);
                _objefin.Efin = Efin;
                _objefin.EfinID = EfinId;
                _objefin.EFINType = string.IsNullOrEmpty(ParentId) ? "M" : "S";
                _objefin.EIN = info["EFINOwnerEIN"].ToString();
                _objefin.Email = info["EFINOwnerEmail"].ToString();
                _objefin.FivePlus = false;
                _objefin.FName = info["EFINOwnerFirstName"].ToString();
                _objefin.IDNumber = info["OwnerIdNumber"].ToString();
                _objefin.IDState = info["OwnerIdState"].ToString();
                _objefin.LName = info["EFINOwnerLastName"].ToString();
                _objefin.Mobile = info["OwnerMobile"].ToString().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                _objefin.Phone = info["EFINOwnerTelephone"].ToString().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                _objefin.RTN = info["OfficeRTN"].ToString();
                _objefin.SBFeeAll = info["FeeOnAll"].ToString();
                _objefin.SelectedBank = "S";
                _objefin.SSN = info["EFINOwnerSSN"].ToString();
                _objefin.State = info["EFINOwnerState"].ToString();
                _objefin.Title = info["OwnerTitle"].ToString();
                _objefin.UpdatedBy = AccountId;
                _objefin.UpdatedDate = DateTime.Now;
                _objefin.UserID = UserId;
                _objefin.Zip = info["EFINOwnerZip"].ToString();

                return _objefin;
            }

            #endregion

            #region "RA"

            else if (bank == RABank)
            {
                EfinObject _objefin = new EfinObject();
                _objefin.AccountID = AccountId;
                _objefin.AcctName = info["AccountName"].ToString();
                _objefin.AcctType = info["BankAccountType"].ToString();
                _objefin.Address = info["OwnerAddress"].ToString();
                _objefin.AgreePEIDate = DateTime.Now;
                _objefin.AgreePEITerms = true;
                _objefin.BankName = info["BankName"].ToString();
                _objefin.City = info["OwnerCity"].ToString();
                _objefin.Company = info["EROOfficeName"].ToString();
                _objefin.CreatedBy = AccountId;
                _objefin.CreatedDate = DateTime.Now;
                _objefin.DAN = info["BankAccountNumber"].ToString();
                _objefin.DOB = Convert.ToDateTime(info["OwnerDOB"]);
                _objefin.Efin = Efin;
                _objefin.EfinID = EfinId;
                _objefin.EFINType = string.IsNullOrEmpty(ParentId) ? "M" : "S";
                _objefin.EIN = info["BusinessEIN"].ToString();
                _objefin.Email = info["OwnerEmail"].ToString();
                _objefin.FivePlus = false;
                _objefin.FName = info["OwnerFirstName"].ToString();
                _objefin.IDNumber = info["OwnerStateIssuedIdNumber"].ToString();
                _objefin.IDState = info["OwnerIssuingState"].ToString();
                _objefin.LName = info["OwnerLastName"].ToString();
                _objefin.Mobile = info["OwnerCellPhone"].ToString().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                _objefin.Phone = info["OwnerHomePhone"].ToString().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                _objefin.RTN = info["BankRoutingNumber"].ToString();
                _objefin.SBFeeAll = info["SbFeeall"].ToString();
                _objefin.SelectedBank = "V";
                _objefin.SSN = info["OwnerSSN"].ToString();
                _objefin.State = info["OwnerState"].ToString();
                _objefin.Title = info["OwnerTitle"].ToString();
                _objefin.UpdatedBy = AccountId;
                _objefin.UpdatedDate = DateTime.Now;
                _objefin.UserID = UserId;
                _objefin.Zip = info["OwnerZipCode"].ToString();

                return _objefin;
            }

            #endregion

            #region "RB"

            else if (bank == RBBank)
            {
                EfinObject _objefin = new EfinObject();
                _objefin.AccountID = AccountId;
                _objefin.AcctName = info["CheckingAccountName"].ToString();
                _objefin.AcctType = info["BankAccountType"].ToString();
                _objefin.Address = info["EFINOwnerAddress"].ToString();
                _objefin.AgreePEIDate = DateTime.Now;
                _objefin.AgreePEITerms = true;
                _objefin.BankName = info["BankName"].ToString();
                _objefin.City = info["EFINOwnerCity"].ToString();
                _objefin.Company = info["OfficeName"].ToString();
                _objefin.CreatedBy = AccountId;
                _objefin.CreatedDate = DateTime.Now;
                _objefin.DAN = info["BankAccountNumber"].ToString();
                _objefin.DOB = Convert.ToDateTime(info["EFINOwnerDOB"]);
                _objefin.Efin = Efin;
                _objefin.EfinID = EfinId;
                _objefin.EFINType = string.IsNullOrEmpty(ParentId) ? "M" : "S";
                _objefin.EIN = info["EFINOwnerEIN"].ToString();
                _objefin.Email = info["EFINOwnerEmail"].ToString();
                _objefin.FivePlus = false;
                _objefin.FName = info["EFINOwnerFirstName"].ToString();
                _objefin.IDNumber = info["EFINOwnerIDNumber"].ToString();
                _objefin.IDState = info["EFINOwnerIDState"].ToString();
                _objefin.LName = info["EFINOwnerLastName"].ToString();
                _objefin.Mobile = info["EFINOwnerMobile"].ToString().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                _objefin.Phone = info["EFINOwnerPhone"].ToString().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                _objefin.RTN = info["BankRoutingNumber"].ToString();
                _objefin.SBFeeAll = info["SBFeeonAll"].ToString();
                _objefin.SelectedBank = "R";
                _objefin.SSN = info["EFINOwnerSSN"].ToString();
                _objefin.State = info["EFINOwnerState"].ToString();
                _objefin.Title = info["EFINOwnerTitle"].ToString();
                _objefin.UpdatedBy = AccountId;
                _objefin.UpdatedDate = DateTime.Now;
                _objefin.UserID = UserId;
                _objefin.Zip = info["EFINOwnerZip"].ToString();

                return _objefin;
            }

            #endregion

            else
                return null;
        }

        public XlinkResponse UpdateTPGBankApp(Guid CustomerId, AuthObject auth, int EfinId, int AppId, AppObject latestbankApp, int UserId, Guid BankId, Guid EnrollmentId)
        {
            try
            {
                DataTable bankEnrollInfo = Data.GetData("select * from BankEnrollmentForTPG where CustomerId='" + CustomerId + "' and StatusCode='ACT'");
                if (bankEnrollInfo.Rows.Count > 0)
                {
                    DateTime? dt = null;

                    var info = bankEnrollInfo.Rows[0];
                    SBTPGAppObject appObj = new SBTPGAppObject();
                    //appObj.AgreeDate = latestbankApp.AgreeDate == null ? dt : latestbankApp.AgreeDate;
                    appObj.Delivered = latestbankApp.Delivered.HasValue ? latestbankApp.Delivered.Value : false;
                    appObj.DeliveredDate = latestbankApp.DeliveredDate.HasValue ? latestbankApp.DeliveredDate.Value : dt;
                    appObj.CompanyName = info["CompanyName"].ToString();
                    appObj.Deleted = false;
                    appObj.EfinID = EfinId;
                    appObj.RalBankLY = info["BankUsedLastYear"].ToString();
                    appObj.ManagerEmail = info["ManagerEmail"].ToString();
                    appObj.ManagerFName = info["ManagerFirstName"].ToString();
                    appObj.ManagerLName = info["ManagerLastName"].ToString();
                    appObj.OfficeAddr = info["OfficeAddress"].ToString();
                    appObj.OfficeCity = info["OfficeCity"].ToString();
                    appObj.OfficeFax = info["OfficeFAX"].ToString();
                    appObj.OfficePhone = info["OfficeTelephone"].ToString().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    appObj.OfficeState = info["OfficeState"].ToString();
                    appObj.OfficeZip = info["OfficeZip"].ToString();
                    appObj.OwnerAddr = info["EFINOwnerAddress"].ToString();
                    appObj.OwnerCity = info["EFINOwnerCity"].ToString();
                    if (!string.IsNullOrEmpty(info["EFINOwnerDOB"].ToString()))
                        appObj.OwnerDOB = Convert.ToDateTime(info["EFINOwnerDOB"].ToString());
                    appObj.OwnerEIN = info["EFINOwnerEIN"].ToString();
                    appObj.OwnerEmail = info["EFINOwnerEmail"].ToString();
                    appObj.OwnerFName = info["EFINOwnerFirstName"].ToString();
                    appObj.OwnerLName = info["EFINOwnerLastName"].ToString();
                    appObj.OwnerPhone = info["EFINOwnerTelephone"].ToString().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    appObj.OwnerSSN = info["EFINOwnerSSN"].ToString();
                    appObj.OwnerState = info["EFINOwnerState"].ToString();
                    appObj.OwnerZip = info["EFINOwnerZip"].ToString();
                    appObj.SBTPGBankAppID = AppId;
                    appObj.ShipAddress = info["ShippingAddress"].ToString();
                    appObj.ShipCity = info["ShippingCity"].ToString();
                    appObj.ShipState = info["ShippingState"].ToString();
                    appObj.ShipZip = info["ShippingZip"].ToString();
                    appObj.UpdatedBy = "RIVEDG";
                    appObj.UpdatedDate = DateTime.Now;
                    appObj.UserID = UserId;
                    appObj.Sent = latestbankApp.Sent.HasValue ? latestbankApp.Sent.Value : false;
                    appObj.SentDate = latestbankApp.SentDate.HasValue ? latestbankApp.SentDate.Value : dt;
                    appObj.EROTranFee = Convert.ToDecimal(info["AddonFee"]);
                    appObj.SBPrepFee = Convert.ToDecimal(info["ServiceBureauFee"]);
                    appObj.DocPrepFee = Convert.ToDecimal(info["DocPrepFee"]);
                    appObj.SuperSBid = 0;
                    appObj.CheckPrint = "D";
                    appObj.Hidden = false;
                    appObj.AgreeBank = Convert.ToBoolean(info["AgreeBank"]);
                    appObj.AgreeDate = DateTime.Now;
                    if (!string.IsNullOrEmpty(info["PriorYearEFIN"].ToString()))
                        appObj.EFINLY = Convert.ToInt32(info["PriorYearEFIN"].ToString());
                    if (!string.IsNullOrEmpty(info["PriorYearVolume"].ToString()))
                        appObj.VolumeLY = Convert.ToInt32(info["PriorYearVolume"].ToString());
                    else
                        appObj.VolumeLY = 0;
                    if (!string.IsNullOrEmpty(info["PriorYearFund"].ToString()))
                        appObj.BankProductsLY = Convert.ToInt32(info["PriorYearFund"]);
                    else
                        appObj.BankProductsLY = 0;

                    string xml = getSBTPGXML(appObj);
                    var IsValid = _apiObj.validateSBTPGApp(appObj, false);
                    if (!IsValid.success)
                    {
                        if (IsValid.message != null)
                        {
                            LogEvent("Updating App Failed :: " + string.Join(",", IsValid.message.ToArray()), EventLogEntryType.FailureAudit, CustomerId, 0, BankId, EnrollmentId, xml);
                        }
                        return IsValid;
                    }
                    XlinkResponse response = _apiObj.updateSBTPGApp(auth, appObj, false);
                    if (response.success)
                    {
                        LogEvent("Updated", EventLogEntryType.SuccessAudit, CustomerId, 1, BankId, EnrollmentId, xml);
                        LogStatus("App Updated", EnrollmentId);
                    }
                    else
                        LogEvent(string.Join(",", response.message.ToArray()), EventLogEntryType.FailureAudit, CustomerId, 0, BankId, EnrollmentId, xml);
                    return response;
                }
                else
                {
                    LogEvent("The record is not availble in TPG.", EventLogEntryType.FailureAudit, CustomerId, 0, BankId, EnrollmentId);
                    return new XlinkResponse() { success = false };
                }
            }
            catch (Exception ex)
            {
                LogEvent(ex.Message + ":: TPG.", EventLogEntryType.Error, CustomerId, 0, BankId, EnrollmentId);
                return new XlinkResponse() { success = false };
            }
        }

        public XlinkResponse UpdateRABankApp(Guid CustomerId, AuthObject auth, int EfinId, int AppId, AppObject latestbankApp, int UserId, Guid BankId, Guid EnrollmentId)
        {
            try
            {
                DataTable bankEnrollInfo = Data.GetData("select * from BankEnrollmentForRA where CustomerId='" + CustomerId + "' and StatusCode='ACT'");
                if (bankEnrollInfo.Rows.Count > 0)
                {
                    DateTime? dt = null;
                    var info = bankEnrollInfo.Rows[0];
                    RefundAdvantageAppObject appObj = new RefundAdvantageAppObject();
                    appObj.Deleted = false;
                    appObj.AgreeBank = true;
                    appObj.AgreeDate = DateTime.Now;
                    appObj.EfinID = EfinId;
                    //appObj.ClientLastYear = info["IsLastYearClient"].ToString() == "1" ? true : false;
                    appObj.CorporationType = info["CorporationType"].ToString();
                    if (!string.IsNullOrEmpty(info["ExpectedCurrentYearVolume"].ToString()))
                        appObj.CurrentYearVolume = Convert.ToInt32(info["ExpectedCurrentYearVolume"].ToString());
                    appObj.RefundAdvantageBankAppID = AppId;
                    appObj.EFINAddressCity = info["OwnerCity"].ToString();
                    appObj.EFINAddressState = info["OwnerState"].ToString();
                    appObj.EFINAddressStreet = info["OwnerAddress"].ToString();
                    appObj.EFINAddressZip = info["OwnerZipCode"].ToString();
                    appObj.EFINOwnerHomePhone = info["OwnerHomePhone"].ToString();
                    appObj.EFINOwnerIDNumber = info["OwnerStateIssuedIdNumber"].ToString();
                    appObj.EFINOwnerIDState = info["OwnerIssuingState"].ToString();
                    appObj.MailingAddressCity = info["EROMailingCity"].ToString();
                    appObj.MailingAddressState = info["EROMailingState"].ToString();
                    appObj.MailingAddressStreet = info["EROMaillingAddress"].ToString();
                    appObj.MailingAddressZip = info["EROMailingZipcode"].ToString();
                    appObj.OfficeAddressCity = info["EROOfficeAddress"].ToString();
                    appObj.OfficeAddressState = info["EROOfficeState"].ToString();
                    appObj.OfficeAddressStreet = info["EROOfficeAddress"].ToString();
                    appObj.OfficeAddressZip = info["EROOfficeZipCoce"].ToString();
                    appObj.OfficeName = info["EROOfficeName"].ToString();
                    appObj.OfficePhone = info["EROOfficePhone"].ToString().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    appObj.PriorYearBank = info["PreviousBankName"].ToString();
                    if (!string.IsNullOrEmpty(info["PreviousYearVolume"].ToString()))
                        appObj.PriorYearVolume = Convert.ToInt32(info["PreviousYearVolume"]);
                    appObj.ShippingAddressCity = info["EROShippingCity"].ToString();
                    appObj.ShippingAddressState = info["EROShippingState"].ToString();
                    appObj.ShippingAddressStreet = info["EROShippingAddress"].ToString();
                    appObj.ShippingAddressZip = info["EROShippingZip"].ToString();
                    appObj.UpdatedBy = "RIVEDG";
                    appObj.UpdatedDate = DateTime.Now;
                    appObj.UserID = UserId;
                    appObj.Sent = latestbankApp.Sent.HasValue ? latestbankApp.Sent.Value : false;
                    appObj.SentDate = latestbankApp.SentDate.HasValue ? latestbankApp.SentDate.Value : dt;
                    appObj.SystemHold = latestbankApp.IsSystemHold.HasValue ? latestbankApp.IsSystemHold.Value : false;
                    appObj.ContactPhone = info["MainContactPhone"].ToString().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    appObj.ContactFirstName = info["MainContactFirstName"].ToString();
                    appObj.ContactLastName = info["MainContactLastName"].ToString();
                    appObj.EFilingFee = Convert.ToDecimal(info["ElectronicFee"]);
                    appObj.EROTranFee = Convert.ToDecimal(info["TransmissionAddon"]);
                    appObj.ExperienceFiling = Convert.ToInt32(info["NoofYearsExperience"]);
                    appObj.LegalIssues = Convert.ToBoolean(info["LegalIssues"]);
                    if (!string.IsNullOrEmpty(info["SbFee"].ToString()))
                        appObj.SBFee = Convert.ToDecimal(info["SbFee"]);
                    appObj.Sent = true;
                    appObj.StateofIncorporation = info["StateOfIncorporation"].ToString();
                    appObj.TextMessage = Convert.ToBoolean(info["TextMessages"]);


                    string xml = getRefundAdvantageXML(appObj);
                    XlinkResponse response = _apiObj.updateRefundAdvantageApp(auth, appObj, false);
                    if (response.success)
                    {
                        LogEvent("Updated the RA application for Customer " + CustomerId, EventLogEntryType.SuccessAudit, CustomerId, 1, BankId, EnrollmentId, xml);
                        LogStatus("App Updated", EnrollmentId);
                    }
                    else
                        LogEvent(string.Join(",", response.message.ToArray()), EventLogEntryType.FailureAudit, CustomerId, 0, BankId, EnrollmentId, xml);
                    return response;
                }
                else
                {
                    LogEvent("The record is not availble in RA", EventLogEntryType.FailureAudit, CustomerId, 0, BankId, EnrollmentId);
                    return new XlinkResponse() { success = false };
                }
            }
            catch (Exception ex)
            {
                LogEvent(ex.Message + ":: RA", EventLogEntryType.Error, CustomerId, 0, BankId, EnrollmentId);
                return new XlinkResponse() { success = false };
            }
        }

        public XlinkResponse UpdateRBBankApp(Guid CustomerId, AuthObject auth, int EfinId, int AppId, AppObject latestbankApp, int UserId, Guid BankId, Guid EnrollmentId)
        {
            try
            {
                DataTable bankEnrollInfo = Data.GetData("select * from BankEnrollmentForRB where CustomerId='" + CustomerId + "' and StatusCode='ACT'");
                if (bankEnrollInfo.Rows.Count > 0)
                {
                    DateTime? dt = null;
                    var info = bankEnrollInfo.Rows[0];
                    RepublicAppObject appObj = new RepublicAppObject();
                    appObj.ActualNumberBankProducts = Convert.ToInt32(info["ActualNoofBankProductsLastYear"].ToString());
                    //if (!string.IsNullOrEmpty(info["EROReadTAndC"].ToString()))
                    //{
                    //    int EROReadTAndC = Convert.ToInt32(info["EROReadTAndC"]);
                    //    appObj.AgreeBank = EROReadTAndC == 0 ? false : true;
                    //}
                    appObj.AdvertisingInd = info["AdvertisingApproval"].ToString();
                    appObj.AntiVirusInd = info["AntivirusRequired"].ToString();
                    appObj.BankProductFacilitator = info["PreviousBankProductFacilitator"].ToString();
                    appObj.CheckCardStorageInd = info["IsLockedStore_Checks"].ToString();

                    appObj.AgreeBank = true;
                    appObj.AgreeDate = DateTime.Now;
                    appObj.Delivered = latestbankApp.Delivered.HasValue ? latestbankApp.Delivered.Value : false;
                    appObj.DeliveredDate = latestbankApp.DeliveredDate.HasValue ? latestbankApp.DeliveredDate.Value : dt;
                    appObj.CellPhoneNumber = info["CellPhoneNumber"].ToString().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    appObj.ComplianceWithLawInd = info["IsAsPerComplainceLaw"].ToString();
                    appObj.Deleted = false;
                    appObj.DebitProgramInd = info["ProductsOffering"].ToString();
                    appObj.DocumentAccessInd = info["IsLimitAccess"].ToString();
                    appObj.DocumentStorageInd = info["IsLockedStore_Documents"].ToString();
                    appObj.EfinID = EfinId;
                    if (!string.IsNullOrEmpty(info["EFINOwnerDOB"].ToString()))
                        appObj.EFINOwnerDOB = Convert.ToDateTime(info["EFINOwnerDOB"].ToString());
                    appObj.EfinOwnerFirstName = info["EFINOwnerFirstName"].ToString();
                    appObj.EfinOwnerLastName = info["EFINOwnerLastName"].ToString();
                    appObj.EfinOwnerSSN = info["EFINOwnerSSN"].ToString();
                    appObj.EIN = info["BusinessEIN"].ToString();
                    appObj.EmailAddress = info["EmailAddress"].ToString();
                    appObj.FaxNumber = info["FAXNumber"].ToString();
                    appObj.FirewallInd = info["HasFirewall"].ToString();
                    appObj.FulfillmentShippingCity = info["FulfillmentShippingCity"].ToString();
                    appObj.FulfillmentShippingState = info["FulfillmentShippingState"].ToString();
                    appObj.FulfillmentShippingStreet = info["FulfillmentShippingAddress"].ToString();
                    appObj.FulfillmentShippingZip = info["FulfillmentShippingZip"].ToString();
                    appObj.IRSTransmittingOfficeInd = info["IsOfficeTransmit"].ToString();
                    if (!string.IsNullOrEmpty(info["NoofBankProductsLastYear"].ToString()))
                        appObj.LastYearBankProducts = Convert.ToInt32(info["NoofBankProductsLastYear"]);
                    appObj.LegalEntityStatusInd = info["LegarEntityStatus"].ToString();
                    appObj.LLCMembershipRegistration = info["LLCMembershipRegistration"].ToString();
                    appObj.LoginPassInd = info["LoginAccesstoEmployees"].ToString();
                    appObj.MailingAddress = info["MailingAddress"].ToString();
                    appObj.MailingCity = info["MailingCity"].ToString();
                    appObj.MailingState = info["MailingState"].ToString();
                    appObj.MailingZip = info["MailingZip"].ToString();
                    appObj.MasterID = latestbankApp.Master.HasValue ? latestbankApp.Master.Value : 0;
                    appObj.MultiOffice = info["IsMultiOffice"].ToString();
                    if (!string.IsNullOrEmpty(info["NoofPersoneel"].ToString()))
                        appObj.NumOfPersonnel = Convert.ToInt32(info["NoofPersoneel"].ToString());
                    appObj.OfficeContactFirstName = info["OfficeContactFirstName"].ToString();
                    appObj.OfficeContactLastName = info["OfficeContactLastName"].ToString();
                    appObj.OfficeContactSSN = info["OfficeContactSSN"].ToString();
                    appObj.OfficeDoorInd = info["IsLocked_Office"].ToString();
                    if (!string.IsNullOrEmpty(info["OfficeManagerDOB"].ToString()))
                        appObj.OfficeManagerDOB = Convert.ToDateTime(info["OfficeManagerDOB"].ToString());
                    appObj.OfficeManagerFirstName = info["OfficeManagerFirstName"].ToString();
                    appObj.OfficeManagerLastName = info["OfficeManageLastName"].ToString();
                    appObj.OfficeManagerSSN = info["OfficeManagerSSN"].ToString();
                    appObj.OfficeName = info["OfficeName"].ToString();
                    appObj.OfficePhoneNumber = info["OfficePhoneNumber"].ToString().Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    appObj.OfficePhysicalCity = info["OfficePhysicalCity"].ToString();
                    appObj.OfficePhysicalState = info["OfficePhysicalState"].ToString();
                    appObj.OfficePhysicalStreet = info["OfficePhysicalAddress"].ToString();
                    appObj.OfficePhysicalZip = info["OfficePhysicalZip"].ToString();
                    appObj.OwnerAddress = info["OwnerAddress"].ToString();
                    appObj.OwnerCity = info["OwnerCity"].ToString();
                    if (!string.IsNullOrEmpty(info["OnwerDOB"].ToString()))
                        appObj.OwnerDOB = Convert.ToDateTime(info["OnwerDOB"].ToString());
                    appObj.OwnerFirstName = info["OwnerFirstName"].ToString();
                    appObj.OwnerHomePhone = info["OwnerHomePhone"].ToString();
                    appObj.OwnerLastName = info["OwnerLastName"].ToString();
                    appObj.OwnerSSN = info["OwnerSSN"].ToString();
                    appObj.OwnerState = info["OwnerState"].ToString();
                    appObj.OwnerZip = info["OwnerZip"].ToString();
                    appObj.PreviousViolationFineInd = info["ConsumerLending"].ToString();
                    appObj.ProductTrainingInd = info["OnlineTraining"].ToString();
                    appObj.PTINInd = info["IsPTIN"].ToString();
                    appObj.RepublicBankAppID = AppId;
                    appObj.SensitiveDocumentDestInd = info["PlantoDispose"].ToString();
                    appObj.UpdatedBy = "RIVEDG";
                    appObj.UpdatedDate = DateTime.Now;
                    appObj.UserID = UserId;
                    appObj.Sent = latestbankApp.Sent.HasValue ? latestbankApp.Sent.Value : false;
                    appObj.SentDate = latestbankApp.SentDate.HasValue ? latestbankApp.SentDate.Value : dt;
                    appObj.SystemHold = latestbankApp.IsSystemHold.HasValue ? latestbankApp.IsSystemHold.Value : false;
                    appObj.WebsiteAddress = info["WebsiteAddress"].ToString();
                    appObj.WirelessInd = info["PasswordRequired"].ToString();
                    appObj.YearsInBusiness = Convert.ToInt32(info["YearsinBusiness"]);
                    appObj.LastYearBankProducts = Convert.ToInt32(info["NoofBankProductsLastYear"]);
                    appObj.TaxPrepLicensing = info["IsAsPerProcessLaw"].ToString();
                    appObj.SupportedOsInd = info["SupportOS"].ToString();
                    appObj.SBPrepFee = Convert.ToDecimal(info["SBFee"]);
                    appObj.EROTranFee = Convert.ToDecimal(info["TransimissionAddon"]);
                    appObj.CardProgram = info["PrepaidCardProgram"].ToString();


                    string xml = getRepublicXML(appObj);
                    XlinkResponse response = _apiObj.updateRepublicApp(auth, appObj, false);
                    if (response.success)
                    {
                        LogEvent("Updated", EventLogEntryType.SuccessAudit, CustomerId, 1, BankId, EnrollmentId, xml);
                        LogStatus("App Updated", EnrollmentId);
                    }
                    else
                        LogEvent(string.Join(",", response.message.ToArray()), EventLogEntryType.FailureAudit, CustomerId, 0, BankId, EnrollmentId, xml);
                    return response;
                }
                else
                {
                    LogEvent("The record is not availble in RB.", EventLogEntryType.FailureAudit, CustomerId, 0, BankId, EnrollmentId);
                    return new XlinkResponse() { success = false };
                }
            }
            catch (Exception ex)
            {
                LogEvent(ex.Message + ":: RB. CustId : " + CustomerId, EventLogEntryType.Error, CustomerId, 0, BankId, EnrollmentId);
                return new XlinkResponse() { success = false };
            }
        }

        public void LogEvent(string Message, EventLogEntryType type, Guid CustomerId, int Status, Guid BankId, Guid EnrollmentId, string xml = "")
        {
            Data.InsertData("insert into BankEnrollmentHistory (CustomerId,BankId,EnrollmentId,Status,CreatedDate,Message,Paramaeters) values ('" + CustomerId.ToString() + "','" + BankId.ToString() + "','" + EnrollmentId.ToString() + "','" + Status + "',GETDATE(),'" + Message + "','" + xml + "')");
            //EventLog.WriteEntry(sSource, Message, type);
        }

        public void LogStatus(string Status, Guid EnrollmentId)
        {
            Data.InsertData("insert into BankEnrollmentStatus (Id,EnrollmentId,Status,CreatedDate) values (NEWID(),'" + EnrollmentId.ToString() + "','" + Status + "',GETDATE())");
        }

        public string getSBTPGXML(SBTPGAppObject app)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(SBTPGAppObject));
            StringWriter sww = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, app);
                var xml = sww.ToString();
                return xml;
            }
        }

        private string getRefundAdvantageXML(RefundAdvantageAppObject appObj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(RefundAdvantageAppObject));
            StringWriter sww = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, appObj);
                var xml = sww.ToString();
                return xml;
            }
        }

        private string getRepublicXML(RepublicAppObject appObj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(RepublicAppObject));
            StringWriter sww = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, appObj);
                var xml = sww.ToString();
                return xml;
            }
        }

        private string getAppXML(List<AppObject> appObj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(List<AppObject>));
            StringWriter sww = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, appObj);
                var xml = sww.ToString();
                return xml;
            }
        }

        private string getLatestAppXML(AppObject appObj)
        {
            try
            {

                XmlSerializer xsSubmit = new XmlSerializer(typeof(AppObject));
                StringWriter sww = new StringWriter();
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, appObj);
                    var xml = sww.ToString();
                    return xml;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string getsubmitBankAppXml(AuthObject _objAuth, int bankappId, int efinid, string accountId, string bankCode, string updatedby)
        {
            SubmitAppModel objModel = new SubmitAppModel();
            objModel.AccountId = accountId;
            objModel.Auth = _objAuth;
            objModel.BankAppId = bankappId;
            objModel.BankCode = bankCode;
            objModel.EFINId = efinid;
            objModel.UpdatedBy = updatedby;

            XmlSerializer xsSubmit = new XmlSerializer(typeof(SubmitAppModel));
            StringWriter sww = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, objModel);
                var xml = sww.ToString();
                return xml;
            }
        }

        private string getEfinObjecyXml(EfinObject obj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(EfinObject));
            StringWriter sww = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, obj);
                var xml = sww.ToString();
                return xml;
            }
        }

        public void UpdateBankStatus()
        {
            DataTable dt = Data.GetData(@"select c.EFIN,b.BankId,bm.BankCode,b.CustomerId,l.CrossLinkUserId,l.CrossLinkPassword,l.MasterIdentifier,b.Id,c.ParentId,c.EntityId,
                                        l.CLAccountId,l.CLAccountPassword,l.CLLogin,c.PrimaryEmail,c.CompanyName
                                        from BankEnrollment b 
                                        join emp_CustomerInformation c on b.CustomerId = c.Id 
                                        join emp_CustomerLoginInformation l on c.Id = l.CustomerOfficeId
                                        join BankMaster bm on b.BankId = bm.Id where b.StatusCode in ('" + SubmitState + "','" + PendingState + "')");
            foreach (DataRow appln in dt.Rows)
            {
                try
                {
                    string UserId = appln["MasterIdentifier"].ToString(); //"RIVEDG";//
                    string Password = string.IsNullOrEmpty(appln["CrossLinkPassword"].ToString()) ? "" : PasswordManager.DecryptText(appln["CrossLinkPassword"].ToString()); //"xlink";//
                    string bankCode = appln["BankCode"].ToString();
                    Guid bankId = new Guid(appln["BankId"].ToString());
                    int efin = Convert.ToInt32(appln["EFIN"].ToString());
                    Guid CustId = new Guid(appln["CustomerId"].ToString());
                    int LoginUserId = 0;
                    Guid EnrollmentId = new Guid(appln["Id"].ToString());
                    string ParentId = appln["ParentId"].ToString();
                    string EmailId = appln["PrimaryEmail"].ToString();
                    string CompanyName = appln["CompanyName"].ToString();
                    string rootparentid = ParentId;
                    bool isRoot = true;

                    if (!string.IsNullOrEmpty(ParentId))
                    {
                        while (isRoot)
                        {
                            DataTable dt1 = Data.GetData("select * from emp_CustomerInformation where Id = '" + rootparentid + "'");
                            if (dt1.Rows.Count > 0)
                            {
                                if (string.IsNullOrEmpty(dt1.Rows[0]["ParentId"].ToString()))
                                    isRoot = false;
                                else
                                    rootparentid = dt1.Rows[0]["ParentId"].ToString();
                            }
                            else
                                isRoot = false;
                        }
                    }
                    if (rootparentid == CustId.ToString() || string.IsNullOrEmpty(rootparentid))
                        rootparentid = null;

                    if (string.IsNullOrEmpty(appln["CLAccountId"].ToString()))
                    {
                        bool isMso = false;
                        if (rootparentid != null)
                        {
                            DataTable dtrp = Data.GetData("select IsMSOUser from emp_CustomerInformation where Id='" + rootparentid + "'");
                            if (dtrp.Rows.Count > 0)
                                if (!string.IsNullOrEmpty(dtrp.Rows[0][0].ToString()))
                                    if (Convert.ToBoolean(dtrp.Rows[0][0]))
                                        isMso = true;
                        }

                        if (isMso)
                        {
                            DataTable parentdt = Data.GetData("select * from emp_CustomerLoginInformation where CustomerOfficeId='" + rootparentid + "'");
                            if (parentdt.Rows.Count > 0)
                            {
                                UserId = parentdt.Rows[0]["MasterIdentifier"].ToString();
                                Password = PasswordManager.DecryptText(parentdt.Rows[0]["CrossLinkPassword"].ToString());
                                LoginUserId = Convert.ToInt32(parentdt.Rows[0]["CrossLinkUserId"].ToString());
                            }
                        }
                        else if (appln["EntityId"].ToString() == SVB_MO_AE_SS || appln["EntityId"].ToString() == SVB_AE_SS || appln["EntityId"].ToString() == MO_AE_SS || appln["EntityId"].ToString() == SOME_SS)
                        {
                            DataTable parentdt = Data.GetData("select * from emp_CustomerLoginInformation where CustomerOfficeId='" + ParentId + "'");
                            if (parentdt.Rows.Count > 0)
                            {
                                UserId = parentdt.Rows[0]["MasterIdentifier"].ToString();
                                Password = PasswordManager.DecryptText(parentdt.Rows[0]["CrossLinkPassword"].ToString());
                                LoginUserId = Convert.ToInt32(parentdt.Rows[0]["CrossLinkUserId"].ToString());
                            }
                        }
                        else
                            LoginUserId = Convert.ToInt32(appln["CrossLinkUserId"]);

                        DataTable dtxlink = Data.GetData("select * from UtaxCrosslinkDetails where CLAccountId = '" + UserId + "' and StatusCode='ACT'");
                        if (dtxlink.Rows.Count > 0)
                        {
                            UserId = dtxlink.Rows[0]["CLLogin"].ToString();
                            Password = PasswordManager.DecryptText(dtxlink.Rows[0]["CLAccountPassword"].ToString());
                        }
                        else
                        {
                            UserId = ConfigurationManager.AppSettings["UserID"];
                            Password = ConfigurationManager.AppSettings["Password"];
                        }
                        //LoginUserId = Convert.ToInt32(appln["CrossLinkUserId"]);
                    }
                    else
                    {
                        UserId = appln["CLAccountId"].ToString();
                        Password = appln["CLAccountPassword"].ToString();
                        LoginUserId = Convert.ToInt32(appln["CrossLinkUserId"]);
                    }


                    string _accesskey = _apiObj.getAccessKey(UserId, Password);
                    if (_accesskey != "")
                    {
                        AuthObject _objAuth = new AuthObject();
                        _objAuth.AccessKey = _accesskey;
                        _objAuth.UserID = UserId; //"RIVEDG";//

                        // checking the Authentication
                        XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                        if (isValid.success)
                        {
                            //efinid = _apiObj.getEFINID(efin);
                            int efinid = _apiObj.getEFINbyEFIN(_objAuth, efin, UserId, 0).EfinID;
                            var latestbankApp = _apiObj.getLatestAppbyBank(_objAuth, efinid, bankCode);

                            //if (latestbankApp.Response.success)
                            //{

                            if (latestbankApp.RegisteredDescription.Contains("Cancel"))
                            {
                                Data.InsertData("update BankEnrollment set StatusCode='" + CancelledState + "',UpdatedDate=GETDATE() where Id='" + EnrollmentId.ToString() + "'");
                                LogEvent("Bank App Cancelled", EventLogEntryType.SuccessAudit, CustId, 1, bankId, EnrollmentId, getLatestAppXML(latestbankApp));
                                LogStatus("Cancelled", EnrollmentId);
                                if (!string.IsNullOrEmpty(EmailId))
                                    Data.InsertData(@"insert into EmailNotification(EmailType,EmailTo,EmailSubject,EmailContent,Parameters,IsSent,CreatedDate) values(5,'"
                                                    + EmailId + "','site bank status has changed to Cancelled','<p>" + CompanyName.Replace("'", "''") + " site bank status has been cancelled.</p><br/>" +
                                                    "EFIN: " + efin + "<br/>User ID: " + LoginUserId + "<br/>Master ID: " + UserId + "','',0,GETDATE())");
                                Data.CallStoreProcedure("OfficeManagementGridSP", CustId.ToString(), CustId.ToString(), rootparentid);
                            }
                            else
                            {
                                switch (latestbankApp.Registered)
                                {
                                    case "A":
                                        Data.InsertData("update BankEnrollment set StatusCode='" + ApprovedState + "',UpdatedDate=GETDATE() where Id='" + EnrollmentId.ToString() + "'");
                                        LogEvent("Bank App Approved", EventLogEntryType.SuccessAudit, CustId, 1, bankId, EnrollmentId, getLatestAppXML(latestbankApp));
                                        LogStatus("Approved", EnrollmentId);
                                        SetDefaultBank(CustId, bankId);
                                        Data.CallStoreProcedure("OfficeManagementGridSP", CustId.ToString(), CustId.ToString(), rootparentid);
                                        break;
                                    case "X":
                                        Data.InsertData("update BankEnrollment set StatusCode='" + RejectedState + "',UpdatedDate=GETDATE() where Id='" + EnrollmentId.ToString() + "'");
                                        LogEvent("Bank App Rejected", EventLogEntryType.SuccessAudit, CustId, 1, bankId, EnrollmentId, getLatestAppXML(latestbankApp));
                                        LogStatus("Rejected", EnrollmentId);
                                        Data.CallStoreProcedure("OfficeManagementGridSP", CustId.ToString(), CustId.ToString(), rootparentid);
                                        break;
                                    case "P":
                                        Data.InsertData("update BankEnrollment set StatusCode='" + PendingState + "',UpdatedDate=GETDATE() where Id='" + EnrollmentId.ToString() + "'");
                                        LogEvent("Bank App Pending", EventLogEntryType.SuccessAudit, CustId, 1, bankId, EnrollmentId, getLatestAppXML(latestbankApp));
                                        LogStatus("Pending", EnrollmentId);
                                        Data.CallStoreProcedure("OfficeManagementGridSP", CustId.ToString(), CustId.ToString(), rootparentid);
                                        break;
                                    case "D":
                                        Data.InsertData("update BankEnrollment set StatusCode='" + DeniedState + "',UpdatedDate=GETDATE() where Id='" + EnrollmentId.ToString() + "'");
                                        LogEvent("Bank App Denied", EventLogEntryType.SuccessAudit, CustId, 1, bankId, EnrollmentId, getLatestAppXML(latestbankApp));
                                        LogStatus("Denied", EnrollmentId);
                                        Data.CallStoreProcedure("OfficeManagementGridSP", CustId.ToString(), CustId.ToString(), rootparentid);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            //}

                            //var res = _apiObj.GetAllAppStats(_objAuth, efinid, latestbankApp.BankAppID, latestbankApp.BankID, null, null);

                        }
                    }
                }
                catch (Exception ex)
                {
                    Data.InsertData("insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" + ex.ToString() + "','" + Guid.Empty + "','WindowsService/UpdateBankStatus',GETDATE())");
                }
            }
        }

        public void UpdateEnrollmentData()
        {
            #region RA Bank
            DataTable dt = Data.GetData(@"select c.EFIN,b.CustomerId,l.CrossLinkUserId,l.CrossLinkPassword,l.MasterIdentifier,c.ParentId,b.Id
                                        from BankEnrollmentForRA b 
                                        join emp_CustomerInformation c on b.CustomerId = c.Id 
                                        join emp_CustomerLoginInformation l on c.Id = l.CustomerOfficeId
                                        where b.IsUpdated = 1 and b.IsActive=1");
            DataTable dtbank = Data.GetData("select Id from BankMaster where BankCode = '" + RABank + "'");
            foreach (DataRow appln in dt.Rows)
            {
                try
                {
                    DataTable dtenroll = Data.GetData("select Id from BankEnrollment where CustomerId = '" + appln["CustomerId"].ToString() + "' and IsActive=1");
                    string UserId = appln["MasterIdentifier"].ToString(); //ConfigurationManager.AppSettings["UserID"];//
                    string Password = PasswordManager.DecryptText(appln["CrossLinkPassword"].ToString());//ConfigurationManager.AppSettings["Password"]; //
                    string bankCode = RABank;
                    Guid bankId = new Guid(dtbank.Rows[0][0].ToString());
                    int efin = Convert.ToInt32(appln["EFIN"]);
                    Guid CustId = new Guid(appln["CustomerId"].ToString());
                    int LoginUserId = Convert.ToInt32(appln["CrossLinkUserId"]);
                    Guid EnrollmentId = new Guid(dtenroll.Rows[0][0].ToString());
                    string ParentId = appln["ParentId"].ToString();
                    string Id = appln["Id"].ToString();

                    string _accesskey = _apiObj.getAccessKey(UserId, Password);
                    if (_accesskey != "")
                    {
                        AuthObject _objAuth = new AuthObject();
                        _objAuth.AccessKey = _accesskey;
                        _objAuth.UserID = UserId;

                        // checking the Authentication
                        XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                        if (isValid.success)
                        {
                            LogEvent("Updating the application for Customer " + CustId + ", bank " + bankCode, EventLogEntryType.Information, CustId, 1, bankId, EnrollmentId);

                            int efinid = _apiObj.getEFINID(efin);

                            var efinres = UpdateEfinObject(_objAuth, efin, efinid, UserId, CustId, ParentId, LoginUserId, bankId, EnrollmentId, bankCode);
                            if (efinres.success)
                            {
                                XlinkResponse result = new XlinkResponse();

                                // getting the latest App by bank
                                var latestbankApp = _apiObj.getLatestAppbyBank(_objAuth, efinid, bankCode);
                                if (bankCode == RABank)
                                {
                                    // update RA Owner object
                                    var ownerres = UpdateRAOwnerObject(CustId.ToString(), latestbankApp.BankAppID, _objAuth, LoginUserId);
                                    if (ownerres.success)
                                    { // updating Refund Advantage Bank
                                        result = UpdateRABankApp(CustId, _objAuth, efinid, latestbankApp.BankAppID, latestbankApp, LoginUserId, bankId, EnrollmentId);
                                        if (result.success)
                                        {
                                            Data.InsertData("update BankEnrollmentForRA set IsUpdated=0 where Id='" + Id + "'");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            LogEvent("Authentication Failed " + string.Join(",", isValid.message.ToArray()), EventLogEntryType.FailureAudit, CustId, 0, bankId, EnrollmentId);
                        }
                    }
                    else
                    {
                        LogEvent("Accesskey not found", EventLogEntryType.FailureAudit, CustId, 0, bankId, EnrollmentId);
                    }
                }
                catch (Exception ex)
                {
                    Data.InsertData("insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" + ex.ToString() + "','" + Guid.Empty + "','WindowsService/UpdateEnrollmentData',GETDATE())");
                }
            }
            #endregion

            #region RB Bank
            DataTable dtrb = Data.GetData(@"select c.EFIN,b.CustomerId,l.CrossLinkUserId,l.CrossLinkPassword,l.MasterIdentifier,c.ParentId,b.Id
                                        from BankEnrollmentForRB b 
                                        join emp_CustomerInformation c on b.CustomerId = c.Id 
                                        join emp_CustomerLoginInformation l on c.Id = l.CustomerOfficeId
                                        where b.IsUpdated = 1 and b.IsActive=1");
            DataTable dtrabank = Data.GetData("select Id from BankMaster where BankCode = '" + RBBank + "'");
            foreach (DataRow appln in dtrb.Rows)
            {
                try
                {
                    DataTable dtenroll = Data.GetData("select Id from BankEnrollment where CustomerId = '" + appln["CustomerId"].ToString() + "' and IsActive=1");
                    string UserId = appln["MasterIdentifier"].ToString();//ConfigurationManager.AppSettings["UserID"];//
                    string Password = PasswordManager.DecryptText(appln["CrossLinkPassword"].ToString());//ConfigurationManager.AppSettings["Password"]; //
                    string bankCode = RBBank;
                    Guid bankId = new Guid(dtrabank.Rows[0][0].ToString());
                    int efin = Convert.ToInt32(appln["EFIN"]);
                    Guid CustId = new Guid(appln["CustomerId"].ToString());
                    int LoginUserId = Convert.ToInt32(appln["CrossLinkUserId"]);
                    Guid EnrollmentId = new Guid(dtenroll.Rows[0][0].ToString());
                    string ParentId = appln["ParentId"].ToString();
                    string Id = appln["Id"].ToString();

                    string _accesskey = _apiObj.getAccessKey(UserId, Password);
                    if (_accesskey != "")
                    {
                        AuthObject _objAuth = new AuthObject();
                        _objAuth.AccessKey = _accesskey;
                        _objAuth.UserID = UserId;

                        // checking the Authentication
                        XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                        if (isValid.success)
                        {
                            LogEvent("Updating the application for Customer " + CustId + ", bank " + bankCode, EventLogEntryType.Information, CustId, 1, bankId, EnrollmentId);

                            int efinid = _apiObj.getEFINID(efin);

                            var efinres = UpdateEfinObject(_objAuth, efin, efinid, UserId, CustId, ParentId, LoginUserId, bankId, EnrollmentId, bankCode);
                            if (efinres.success)
                            {
                                XlinkResponse result = new XlinkResponse();

                                // getting the latest App by bank
                                var latestbankApp = _apiObj.getLatestAppbyBank(_objAuth, efinid, bankCode);

                                if (bankCode == RBBank)
                                {
                                    // updating republic Bank
                                    result = UpdateRBBankApp(CustId, _objAuth, efinid, latestbankApp.BankAppID, latestbankApp, LoginUserId, bankId, EnrollmentId);
                                    if (result.success)
                                    {
                                        Data.InsertData("update BankEnrollmentForRB set IsUpdated=0 where Id='" + Id + "'");
                                    }
                                }
                            }
                        }
                        else
                        {
                            LogEvent("Authentication Failed for UserID:: " + UserId + " :: Pwd :: " + Password + " :: " + string.Join(",", isValid.message.ToArray()), EventLogEntryType.FailureAudit, CustId, 0, bankId, EnrollmentId);
                        }
                    }
                    else
                    {
                        LogEvent("Accesskey not found for UserId :: " + UserId + " :: Pwd ::" + Password, EventLogEntryType.FailureAudit, CustId, 0, bankId, EnrollmentId);
                    }
                }
                catch (Exception ex)
                {
                    Data.InsertData("insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" + ex.ToString() + "','" + Guid.Empty + "','WindowsService/UpdateEnrollmentData',GETDATE())");
                }
            }
            #endregion

            #region TPG Bank
            DataTable dttpg = Data.GetData(@"select c.EFIN,b.CustomerId,l.CrossLinkUserId,l.CrossLinkPassword,l.MasterIdentifier,c.ParentId,b.Id
                                        from BankEnrollmentForTPG b 
                                        join emp_CustomerInformation c on b.CustomerId = c.Id 
                                        join emp_CustomerLoginInformation l on c.Id = l.CustomerOfficeId
                                        where b.IsUpdated = 1 and b.IsActive=1");
            DataTable dttpgbank = Data.GetData("select Id from BankMaster where BankCode = '" + TPGBank + "'");
            foreach (DataRow appln in dttpg.Rows)
            {
                try
                {
                    DataTable dtenroll = Data.GetData("select Id from BankEnrollment where CustomerId = '" + appln["CustomerId"].ToString() + "' and IsActive=1");
                    string UserId = appln["MasterIdentifier"].ToString();//ConfigurationManager.AppSettings["UserID"];//
                    string Password = PasswordManager.DecryptText(appln["CrossLinkPassword"].ToString());// ConfigurationManager.AppSettings["Password"];
                    string bankCode = TPGBank;
                    Guid bankId = new Guid(dttpgbank.Rows[0][0].ToString());
                    int efin = Convert.ToInt32(appln["EFIN"]);
                    Guid CustId = new Guid(appln["CustomerId"].ToString());
                    int LoginUserId = Convert.ToInt32(appln["CrossLinkUserId"]);
                    Guid EnrollmentId = new Guid(dtenroll.Rows[0][0].ToString());
                    string ParentId = appln["ParentId"].ToString();
                    string Id = appln["Id"].ToString();

                    string _accesskey = _apiObj.getAccessKey(UserId, Password);
                    if (_accesskey != "")
                    {
                        AuthObject _objAuth = new AuthObject();
                        _objAuth.AccessKey = _accesskey;
                        _objAuth.UserID = UserId;

                        // checking the Authentication
                        XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                        if (isValid.success)
                        {
                            LogEvent("Updating the application for Customer " + CustId + ", bank " + bankCode, EventLogEntryType.Information, CustId, 1, bankId, EnrollmentId);

                            int efinid = _apiObj.getEFINID(efin);

                            var efinres = UpdateEfinObject(_objAuth, efin, efinid, UserId, CustId, ParentId, LoginUserId, bankId, EnrollmentId, bankCode);
                            if (efinres.success)
                            {
                                XlinkResponse result = new XlinkResponse();

                                // getting the latest App by bank
                                var latestbankApp = _apiObj.getLatestAppbyBank(_objAuth, efinid, bankCode);
                                if (bankCode == TPGBank)
                                {
                                    // updating the SBTPG bank
                                    result = UpdateTPGBankApp(CustId, _objAuth, efinid, latestbankApp.BankAppID, latestbankApp, LoginUserId, bankId, EnrollmentId);
                                    if (result.success)
                                    {
                                        Data.InsertData("update BankEnrollmentForTPG set IsUpdated=0 where Id='" + Id + "'");
                                    }
                                }
                            }
                        }
                        else
                        {
                            LogEvent("Authentication Failed for UserID:: " + UserId + " :: Pwd :: " + Password + " :: " + string.Join(",", isValid.message.ToArray()), EventLogEntryType.FailureAudit, CustId, 0, bankId, EnrollmentId);
                        }
                    }
                    else
                    {
                        LogEvent("Accesskey not found for UserId :: " + UserId + " :: Pwd ::" + Password, EventLogEntryType.FailureAudit, CustId, 0, bankId, EnrollmentId);
                    }
                }
                catch (Exception ex)
                {
                    Data.InsertData("insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" + ex.ToString() + "','" + Guid.Empty + "','WindowsService/UpdateEnrollmentData',GETDATE())");
                }
            }
            #endregion
        }

        public void CreateNewUsers()
        {
            try
            {
                DataTable dt = Data.GetData(@"select c.ParentId,c.Id as CustomerId,l.Id as LoginId,l.MasterIdentifier,c.CompanyName,c.PrimaryEMail,c.BusinessOwnerFirstName,
                                            c.BusinesOwnerLastName,c.OfficePhone,c.ShippingAddress1,c.ShippingCity,c.ShippingState,c.ShippingZipCode,c.EntityId,c.IsActivationCompleted,
                                            l.EMPUserId
                                            from emp_CustomerLoginInformation l 
                                            join emp_CustomerInformation c on l.CustomerOfficeId = c.Id
                                            where l.EMPPassword is null");
                foreach (DataRow appln in dt.Rows)
                {
                    int entityid = Convert.ToInt32(appln["EntityId"]);
                    if (appln["IsActivationCompleted"].ToString() != "1" && (entityid == (int)Entity.MO_AE || entityid == (int)Entity.MO_SO || entityid == (int)Entity.SVB_AE || entityid == (int)Entity.SVB_MO || entityid == (int)Entity.SVB_MO_AE || entityid == (int)Entity.SVB_MO_SO || entityid == (int)Entity.SVB_SO))
                        continue;
                    if (!string.IsNullOrEmpty(appln["EMPUserId"].ToString()))
                        continue;

                    using (var client = new HttpClient())
                    {
                        string APIUrl = ConfigurationManager.AppSettings["EmpwebApi"].ToString();

                        client.BaseAddress = new Uri(APIUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                        CreateNewUserModel ocustomer = new CreateNewUserModel();
                        ocustomer.AddBusinessProduct = false;
                        ocustomer.CompanyName = appln["CompanyName"].ToString();
                        ocustomer.CustomerId = new Guid(appln["CustomerId"].ToString());
                        ocustomer.Email = appln["PrimaryEmail"].ToString();
                        ocustomer.Fname = appln["BusinessOwnerFirstName"].ToString();
                        ocustomer.Lname = appln["BusinesOwnerLastName"].ToString();
                        ocustomer.MasterIdentifier = appln["MasterIdentifier"].ToString();
                        if (!string.IsNullOrEmpty(appln["ParentId"].ToString()))
                            ocustomer.ParentCustomerId = new Guid(appln["ParentId"].ToString());
                        ocustomer.Phone = appln["OfficePhone"].ToString();
                        ocustomer.ShippingAddress = appln["ShippingAddress1"].ToString();
                        ocustomer.ShippingCity = appln["ShippingCity"].ToString();
                        ocustomer.ShippingState = appln["ShippingState"].ToString();
                        ocustomer.ShippingZip = appln["ShippingZipCode"].ToString();


                        var response = client.PostAsJsonAsync("api/Crosslink/CreateNewUser", ocustomer).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var json = new JavaScriptSerializer();
                            string message = response.Content.ReadAsStringAsync().Result;
                            bool response1 = json.Deserialize<bool>(message);
                            if (response1)
                            {
                                var emailresponse = client.PostAsJsonAsync("api/Crosslink/SendEmailForNewUser", new NewUserEmailRequest() { CustomerId = new Guid(appln["CustomerId"].ToString()) }).Result;
                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Data.InsertData("insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" + ex.ToString() + "','" + Guid.Empty + "','WindowsService/CreateNewUsers',GETDATE())");
            }
        }

        public void SetDefaultBank(Guid CustomerId, Guid BankId)
        {
            try
            {
                DataTable dt = Data.GetData("select * from EnrollmentBankSelection where BankSubmissionStatus=1 and CustomerId='" + CustomerId.ToString() + "' and StatusCode='ACT'");
                if (dt.Rows.Count <= 0)
                {
                    Data.CallDefaultStoreProcedure("SetDefaultBankSP", CustomerId.ToString(), CustomerId.ToString(), BankId.ToString());
                    //Data.InsertData("update EnrollmentBankSelection set BankSubmissionStatus=1 where CustomerId='" + CustomerId.ToString() + "' and StatusCode='ACT'");
                }
                DataTable dtPrefer = Data.GetData("select * from EnrollmentBankSelection where IsPreferredBank=1 and CustomerId='" + CustomerId.ToString() + "' and StatusCode='ACT' and BankId='" + BankId.ToString() + "'");
                if (dtPrefer.Rows.Count > 0)
                {
                    Data.CallDefaultStoreProcedure("SetDefaultBankSP", CustomerId.ToString(), CustomerId.ToString(), BankId.ToString());
                    Data.InsertData("update EnrollmentBankSelection set IsPreferredBank=0 where Id='" + dtPrefer.Rows[0]["Id"] + "'");
                }
            }
            catch (Exception ex)
            {
                Data.InsertData("insert into ExceptionLog (ExceptionMessage,UserId,MethodName,CreatedDateTime) values('" + ex.ToString() + "','" + Guid.Empty + "','WindowsService/SetDefaultBank',GETDATE())");
            }
        }

        public void UpdateOnboardStatus()
        {
            DataTable dt = Data.GetData(@"select * from emp_CustomerInformation where OnBoardPrimaryKey is not null");
            List<int> _completedStus = new List<int>();
            _completedStus.Add(2);
            _completedStus.Add(4);
            _completedStus.Add(5);

            foreach (DataRow dr in dt.Rows)
            {
                DataTable dtofc = Data.GetData("select OnboardingStatus from OfficeManagement where CustomerId = '" + dr["CustomerId"] + "'");
                if (dtofc.Rows.Count > 0)
                {
                    if (dtofc.Rows[0][0].ToString() == "2")
                    {
                        continue;
                    }
                }

                Guid caseId = new Guid(dr["OnBoardPrimaryKey"].ToString());
                DataTable dtonbd = Data.GetData("select top 1 * from Onboarding where CaseId='" + caseId + "' order by LastModifiedDate desc", "csr");
                if (dtonbd.Rows.Count > 0)
                {
                    DataRow cdr = dtonbd.Rows[0];

                    int CurrentYearStaus = Convert.ToInt32(cdr["InstallCurrentYearStatus"]);
                    int BankEnrollmentStaus = Convert.ToInt32(cdr["BankEnrollmentStatus"]);
                    int ConversionStaus = Convert.ToInt32(cdr["ConversionStatus"]);
                    int DbConfigurationStaus = Convert.ToInt32(cdr["DbConfigurationStatus"]);
                    int NewYearStaus = Convert.ToInt32(cdr["InstallNewYearStatus"]);
                    int MobileAppStaus = Convert.ToInt32(cdr["MobileAppStatus"]);
                    int AdminStaus = Convert.ToInt32(cdr["AdminStatus"]);

                    if (CurrentYearStaus == 1 || CurrentYearStaus == 3 || BankEnrollmentStaus == 1 || BankEnrollmentStaus == 3 || ConversionStaus == 1 || ConversionStaus == 3 ||
                        DbConfigurationStaus == 1 || DbConfigurationStaus == 3 || NewYearStaus == 1 || NewYearStaus == 3 || MobileAppStaus == 1 || MobileAppStaus == 3
                        || AdminStaus == 1 || AdminStaus == 3)
                    {
                        Data.InsertData(@"update OfficeManagement set OnboardingStatus = '3' where CustomerId = '" + dr["CustomerId"] + "'");

                        string statusTooltip = "";
                        statusTooltip +="Admin Status - "+ ((OnboardStatus)AdminStaus).ToString().Replace("_", " ") + "\n";
                        statusTooltip += "Install Current Year Status - " + ((OnboardStatus)CurrentYearStaus).ToString().Replace("_", " ") + "\n";
                        statusTooltip += "Bank Enrollment Status - " + ((OnboardStatus)BankEnrollmentStaus).ToString().Replace("_", " ") + "\n";
                        statusTooltip +="Conversion Status - "+ ((OnboardStatus)ConversionStaus).ToString().Replace("_", " ") + "\n";
                        statusTooltip +="Db Configuration Status - "+ ((OnboardStatus)DbConfigurationStaus).ToString().Replace("_", " ") + "\n";
                        statusTooltip +="Install New Year Status - "+ ((OnboardStatus)NewYearStaus).ToString().Replace("_", " ") + "\n";
                        statusTooltip +="Mobile App Status - "+ ((OnboardStatus)MobileAppStaus).ToString().Replace("_"," ");

                        Data.InsertData("update OfficeManagement set OnboardStatusTooltip = '" + statusTooltip + "' where CustomerId = '" + dr["CustomerId"] + "'");
                    }
                    else if (_completedStus.Contains(CurrentYearStaus) && _completedStus.Contains(BankEnrollmentStaus) && _completedStus.Contains(ConversionStaus) &&
                        _completedStus.Contains(DbConfigurationStaus) && _completedStus.Contains(NewYearStaus) && _completedStus.Contains(MobileAppStaus) && _completedStus.Contains(AdminStaus))
                    {
                        Data.InsertData(@"update OfficeManagement set OnboardingStatus = '2',OnboardStatusTooltip = '' where CustomerId = '" + dr["CustomerId"] + "'");
                    }
                }
            }
        }
    }
}