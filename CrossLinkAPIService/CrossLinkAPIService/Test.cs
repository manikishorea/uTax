using CrossLinkAPIService.Crosslinkapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace CrossLinkAPIService
{
    public class Test
    {
        CrosslinkWS17SoapClient _apiObj = new CrosslinkWS17SoapClient();

        public void GetInformation()
        {
            //DataTable dt = Data.GetData("select * from OfficeManagement where EnrollmentStatus in ('APR')");
            //foreach (DataRow item in dt.Rows)
            //{
            //    string CustId = item["CustomerId"].ToString();
            //    string Bank = item["ActiveBankId"].ToString();
            //    //string ParentId = item["ParentId"].ToString();
            //    //string rootparentid = ParentId;
            //    //bool isRoot = true;

            //    //if (!string.IsNullOrEmpty(ParentId))
            //    //{
            //    //    while (isRoot)
            //    //    {
            //    //        DataTable dt1 = Data.GetData("select * from emp_CustomerInformation where Id = '" + rootparentid + "'");
            //    //        if (dt1.Rows.Count > 0)
            //    //        {
            //    //            if (string.IsNullOrEmpty(dt1.Rows[0]["ParentId"].ToString()))
            //    //                isRoot = false;
            //    //            else
            //    //                rootparentid = dt1.Rows[0]["ParentId"].ToString();
            //    //        }
            //    //        else
            //    //            isRoot = false;
            //    //    }
            //    //}
            //    //if (rootparentid == CustId.ToString() || string.IsNullOrEmpty(rootparentid))
            //    //    rootparentid = null;


            //    //Data.CallStoreProcedure("OfficeManagementGridSP", CustId.ToString(), CustId.ToString(), rootparentid);

            //    Data.CallDefaultStoreProcedure("SetDefaultBankSP", CustId, CustId, Bank);
            //}
            //return;






            string UserId = "RIVEDG";// ConfigurationManager.AppSettings["UserID"];
            string Password = ConfigurationManager.AppSettings["Password"];
            string _accesskey = _apiObj.getAccessKey(UserId, Password);
            if (_accesskey != "")
            {
                AuthObject _objAuth = new AuthObject();
                _objAuth.AccessKey = _accesskey;
                _objAuth.UserID = UserId;

                XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                if (isValid.success)
                {

                    //var r = _apiObj.getEFINbyEFIN(_objAuth, 102560, "RIVEDG06", 0);
                    //var apps = _apiObj.getBankApps(_objAuth, r.EfinID);
                    
                    //var r = _apiObj.getEFINbyEFIN(_objAuth, 731464, "RIVEDG", 0);
                    //var xml = getEfinObjecyXml(r);
                    //var e = _apiObj.getEFIN(_objAuth,r.EfinID, "RIVEDG", 0);
                    var ssdsdfwe = _apiObj.getUserIDList(_objAuth, UserId, 0);

                    var efinsLst = _apiObj.getEFINSummaryList(_objAuth, UserId, null, 0);
                    var dfdf = string.Join("\n", efinsLst.Select(x => x.UserID + "_" + x.Efin + "_" + x.EfinID + "_" + x.AppBankName).ToArray());
                    var sss = efinsLst.Where(x => x.UserID == 19785).ToList();

                    int i = 0;
                    foreach (var efin in efinsLst)
                    {
                        var resss = _apiObj.hasMEfinBank(efin.Efin, "S");
                        var resss1 = _apiObj.hasMEfinBank(efin.Efin, "R");
                        var resss2 = _apiObj.hasMEfinBank(efin.Efin, "V");


                        //if (efin.Efin == 880263)
                        //{
                        //    var _banks = _apiObj.getBankApps(_objAuth, efin.EfinID);
                        //    if (_banks.Length > 0)
                        //    {
                        //        int BankAppId = _banks[0].BankAppID;
                        //        var _bankApp = _apiObj.getSBTPGApp(_objAuth, BankAppId);
                        //        var _repApp = _apiObj.getRepublicApp(_objAuth, BankAppId);
                        //        if (_bankApp != null)
                        //        {
                        //            RepublicAppObject republic = new RepublicAppObject();
                        //            republic.AdvertisingInd = "";
                        //            republic.AgreeBank = _bankApp.AgreeBank;
                        //            republic.AgreeDate = _bankApp.AgreeDate;
                        //            republic.AntiVirusInd = "";
                        //            republic.BankProductFacilitator = "";
                        //            republic.CardProgram = "";
                        //            republic.CellPhoneNumber = _bankApp.OwnerPhone;
                        //            republic.CheckCardStorageInd = "";
                        //            republic.CheckStockID = "";
                        //            republic.ComplianceWithLawInd = "";
                        //            republic.DebitProgramInd = "";
                        //            republic.Deleted = false;
                        //            republic.Delivered = _bankApp.Delivered;
                        //            republic.DeliveredDate = _bankApp.DeliveredDate;
                        //            republic.DocumentAccessInd = "";
                        //            republic.DocumentStorageInd = "";
                        //            republic.EfinID = _bankApp.EfinID;
                        //            republic.EFINOwnerDOB = _bankApp.OwnerDOB;
                        //            republic.EfinOwnerFirstName = _bankApp.OwnerFName;
                        //            republic.EfinOwnerLastName = _bankApp.OwnerLName;
                        //            republic.EfinOwnerSSN = _bankApp.OwnerSSN;
                        //            republic.EIN = _bankApp.OwnerEIN;
                        //            republic.EmailAddress = _bankApp.OwnerEmail;
                        //            republic.EROTranFee = _bankApp.EROTranFee;
                        //            republic.FaxNumber = _bankApp.OfficeFax;
                        //            republic.FirewallInd = "";
                        //            republic.FulfillmentShippingCity = _bankApp.ShipCity;
                        //            republic.FulfillmentShippingState = _bankApp.ShipState;
                        //            republic.FulfillmentShippingStreet = _bankApp.ShipAddress;
                        //            republic.FulfillmentShippingZip = _bankApp.ShipZip;
                        //            republic.IRSTransmittingOfficeInd = "";
                        //            republic.LastYearBankProducts = 0;
                        //            republic.LegalEntityStatusInd = "";
                        //            republic.LLCMembershipRegistration = "";
                        //            republic.LoginPassInd = "";
                        //            republic.MailingAddress = _bankApp.OfficeAddr;
                        //            republic.MailingCity = _bankApp.OfficeCity;
                        //            republic.MailingState = _bankApp.OfficeState;
                        //            republic.MailingZip = _bankApp.OfficeZip;
                        //            republic.MasterID = 0;
                        //            republic.MultiOffice = "";
                        //            republic.NumOfPersonnel = 0; // OPtional
                        //            republic.OfficeContactFirstName = "";
                        //            republic.OfficeContactLastName = "";
                        //            republic.OfficeContactSSN = "";
                        //            republic.OfficeDoorInd = "";
                        //            //republic.OfficeManagerDOB = DateTime.Now; //Optional
                        //            republic.OfficeManagerFirstName = _bankApp.ManagerFName;
                        //            republic.OfficeManagerLastName = _bankApp.ManagerLName;
                        //            republic.OfficeManagerSSN = "";
                        //            republic.OfficeName = "";
                        //            republic.OfficePhoneNumber = _bankApp.OfficePhone;
                        //            republic.OfficePhysicalCity = _bankApp.OfficeCity;
                        //            republic.OfficePhysicalState = _bankApp.OfficeState;
                        //            republic.OfficePhysicalStreet = _bankApp.OfficeAddr;
                        //            republic.OfficePhysicalZip = _bankApp.OfficeZip;
                        //            republic.OwnerAddress = _bankApp.OwnerAddr;
                        //            republic.OwnerCity = _bankApp.OwnerCity;
                        //            republic.OwnerDOB = _bankApp.OwnerDOB;
                        //            republic.OwnerFirstName = _bankApp.OwnerFName;
                        //            republic.OwnerHomePhone = _bankApp.OwnerPhone;
                        //            republic.OwnerLastName = _bankApp.OwnerLName;
                        //            republic.OwnerSSN = _bankApp.OwnerSSN;
                        //            republic.OwnerState = _bankApp.OwnerState;
                        //            republic.OwnerZip = _bankApp.OwnerZip;
                        //            republic.PEIRALTransmitterFee = 0; // OPtional
                        //            republic.PEITechFee = 0;//Optional
                        //            republic.PreviousViolationFineInd = "";
                        //            republic.ProductTrainingInd = "";
                        //            republic.PTINInd = "";
                        //            //republic.RepublicBankAppID = _bankApp.SBTPGBankAppID;
                        //            republic.Response = _bankApp.Response;
                        //            republic.Roll = "";
                        //            republic.SBPrepFee = 0;//Optional
                        //            republic.SensitiveDocumentDestInd = "";
                        //            republic.Sent = _bankApp.Sent;
                        //            republic.SentDate = _bankApp.SentDate;
                        //            republic.SystemHold = _bankApp.SystemHold;
                        //            republic.TaxPrepLicensing = "";
                        //            //republic.TermsDateTime = DateTime.Now; //Optional
                        //            republic.TransmitterFeeDefault = 0; //Ooptianl
                        //            republic.UpdatedBy = _bankApp.UpdatedBy;
                        //            republic.UpdatedDate = DateTime.Now;
                        //            republic.UserID = _bankApp.UserID;
                        //            republic.WebsiteAddress = "";
                        //            republic.WirelessInd = "";
                        //            republic.YearsInBusiness = 0; //Optional

                        //            var sdf = _apiObj.updateRepublicApp(_objAuth, republic, false);


                        //            //_bankApp.OfficeAddr = _bankApp.OfficeAddr + " Updated";
                        //            //var sadf = _apiObj.updateSBTPGApp(_objAuth, _bankApp, false);                                    
                        //        }
                        //    }
                        //}
                    }
                }
            }
            else
            {
            }

            //var banks = _apiObj.getAvailableBanks(efin.EfinID);

            //var dsf = _apiObj.getEFINbyEFIN(_objAuth, efin.EfinID, UserId, 0);
        }
        
        public void CheckEfin()
        {
            var res = _apiObj.verifyMasterEFIN(963710, "S");
            var res1 = _apiObj.verifyMasterEFIN(963710, "R");
            var res2 = _apiObj.verifyMasterEFIN(963710, "V");
        }

        public void getUserIdList()
        {
            string UserId = "RIVEDG";// appln["MasterIdentifier"].ToString();
            string Password = "xlink";// appln["CrossLinkPassword"].ToString();
            string _accesskey = _apiObj.getAccessKey(UserId, Password);
            if (_accesskey != "")
            {
                AuthObject _objAuth = new AuthObject();
                _objAuth.AccessKey = _accesskey;
                _objAuth.UserID = UserId;

                XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                if (isValid.success)
                {
                    var res = _apiObj.getUserIDList(_objAuth, UserId, 0);
                    res = res.OrderByDescending(x => x.UserID).Take(50).ToArray();
                    string xml = GetXML(res);
                    var ids = string.Join(",", res.Select(x => x.UserID).ToArray());
                }
            }
        }

        private string GetXML(GetCrosslinkUsers_Result[] appObj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(GetCrosslinkUsers_Result[]));
            StringWriter sww = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, appObj);
                var xml = sww.ToString();
                return xml;
            }
        }

        public void getEfinObject()
        {
            string UserId = "RIVEDG";// appln["MasterIdentifier"].ToString();
            string Password = "xlink";// appln["CrossLinkPassword"].ToString();
            string _accesskey = _apiObj.getAccessKey(UserId, Password);
            if (_accesskey != "")
            {
                AuthObject _objAuth = new AuthObject();
                _objAuth.AccessKey = _accesskey;
                _objAuth.UserID = UserId;

                XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                if (isValid.success)
                {
                    var res = _apiObj.getEFIN(_objAuth, 6409, "RIVEDG", 0);

                }
            }
        }

        public void CheckEfinStatus()
        {
            string UserId = "RIVEDG";// appln["MasterIdentifier"].ToString();
            string Password = "xlink";// appln["CrossLinkPassword"].ToString();
            string _accesskey = _apiObj.getAccessKey(UserId, Password);
            if (_accesskey != "")
            {
                AuthObject _objAuth = new AuthObject();
                _objAuth.AccessKey = _accesskey;
                _objAuth.UserID = UserId;

                XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                if (isValid.success)
                {
                    int efinid = _apiObj.getEFINbyEFIN(_objAuth, 963710, UserId, 0).EfinID;
                    var res = _apiObj.getEFIN(_objAuth, efinid, "RIVEDG", 0);
                  var ss =  _apiObj.getEFINCompleteStatus(res);
                  var sdf=  _apiObj.getBankApps(_objAuth, efinid);

                }
            }            
        }
    }
}
