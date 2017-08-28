using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPPortal.Transactions.FeeReimbursement.DTO;
using EMP.Core.Utilities;
using System.Data.Entity;
using EMPPortal.Transactions.Reports.DTO;
using EMPPortal.Transactions.EnrollmentBankSelectionInfo.DTO;
using EMPPortal.Transactions.DropDowns;
using EMPPortal.Transactions.DropDowns.DTO;

namespace EMPPortal.Transactions.ReportsService
{
    public class ReportsService
    {
        DatabaseEntities db = new DatabaseEntities();

        public List<FeeSetupReportDTO> GetFeeSetUpReport(string strguid)
        {
            try
            {
                List<string> lstCustGuid = strguid.Split(',').ToList();
                db = new DatabaseEntities();
                List<ReportCustomerModel> data = new List<ReportCustomerModel>();
                //var data = (from cu in db.emp_CustomerInformation
                //            join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                //            where cu.ParentId != null && cl.CrossLinkUserId != null && cu.StatusCode != EMPConstants.Pending
                //            select new { cu, cl }).Take(1);

                EnrollmentBankSelectionInfo.EnrollmentBankSelectionService oService = new EnrollmentBankSelectionInfo.EnrollmentBankSelectionService();
                if (db.emp_CustomerInformation.Any(a => lstCustGuid.Contains(a.Id.ToString())))
                {
                    List<string> lstGuids = GetChildData(lstCustGuid);

                    data = (from cu in db.emp_CustomerInformation
                            join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                            where lstGuids.Contains(cu.Id.ToString()) && cl.CrossLinkUserId != null && cu.StatusCode != EMPConstants.Pending
                            select new ReportCustomerModel { cu = cu, cl = cl }).ToList();
                }
                else
                {
                    data = (from cu in db.emp_CustomerInformation
                            join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                            where cl.CrossLinkUserId != null && (cu.StatusCode != EMPConstants.InProgress && cu.StatusCode != EMPConstants.NewCustomer)
                            orderby cl.EMPUserId
                            select new ReportCustomerModel { cu = cu, cl = cl }).ToList();
                }

                var custids = data.Select(x => x.cl.CustomerOfficeId).Distinct().ToList();

                var enrollstatus = (from eb in db.EnrollmentBankSelections
                                    join be in db.BankEnrollments on new { x1 = eb.BankId, x2 = eb.CustomerId } equals new { x1 = be.BankId.Value, x2 = be.CustomerId.Value }
                                    join b in db.BankMasters on eb.BankId equals b.Id
                                    where custids.Contains(be.CustomerId) && custids.Contains(eb.CustomerId)
                                    && ((be.StatusCode == EMPConstants.EnrPending || be.StatusCode == EMPConstants.Submitted || be.StatusCode == EMPConstants.Approved || be.StatusCode == EMPConstants.Rejected)
                                    && be.ArchiveStatusCode != EMPConstants.Archive) && be.IsActive == true
                                    && eb.StatusCode == EMPConstants.Active
                                    select new { eb.CustomerId, b.Id, b.BankName, be.StatusCode, eb.ServiceBureauBankAmount, eb.TransmissionBankAmount, eb.IsServiceBureauFee, eb.IsTransmissionFee }).ToList();

                //var bankenrolls = db.BankEnrollments.Where(x => custids.Contains(x.CustomerId) &&
                //                    (x.StatusCode == EMPConstants.EnrPending || x.StatusCode == EMPConstants.Submitted || x.StatusCode == EMPConstants.Approved || x.StatusCode == EMPConstants.Rejected)
                //                     && x.ArchiveStatusCode != EMPConstants.Archive).ToList();

                List<FeeSetupReportDTO> lstmodel = new List<FeeSetupReportDTO>();

                if (data.Count() > 0)
                {
                    foreach (var itm in data)
                    {
                        //var enrollstatus = (from eb in db.EnrollmentBankSelections
                        //                    join b in db.BankMasters on eb.BankId equals b.Id
                        //                    where eb.CustomerId == itm.cu.Id
                        //                    select new { b.Id, b.BankName, eb.StatusCode, eb.ServiceBureauBankAmount, eb.TransmissionBankAmount, eb.IsServiceBureauFee, eb.IsTransmissionFee }).ToList();
                        var banks = enrollstatus.Where(o => o.CustomerId == itm.cl.CustomerOfficeId).ToList();
                        foreach (var bank in banks)
                        //if (bank != null)
                        {
                            //var isexist = bankenrolls.Where(x => x.CustomerId == itm.cl.CustomerOfficeId && x.BankId == bank.Id).FirstOrDefault();
                            //if (isexist == null)
                            //    continue;

                            FeeSetupReportDTO omodel = new FeeSetupReportDTO();
                            omodel.UserID = itm.cl.EMPUserId;
                            if (itm.cu.ParentId != null)
                                omodel.ParentUserID = db.emp_CustomerLoginInformation.Where(a => a.CustomerOfficeId == itm.cu.ParentId).Select(a => a.CrossLinkUserId).FirstOrDefault();
                            else
                                omodel.ParentUserID = "";
                            omodel.CompanyName = itm.cu.CompanyName;
                            omodel.Efin = itm.cu.EFIN.ToString().PadLeft(6, '0');
                            omodel.MasterID = itm.cl.MasterIdentifier;
                            omodel.AccountOwner = itm.cu.BusinessOwnerFirstName;

                            if (enrollstatus != null)
                            {
                                omodel.Bank = bank.BankName;
                                if (bank.IsServiceBureauFee == true)
                                    omodel.SBAddOn = bank.ServiceBureauBankAmount.ToString();
                                else
                                    omodel.SBAddOn = string.Empty;
                                if (bank.IsTransmissionFee == true)
                                    omodel.AddonFeeERO = bank.TransmissionBankAmount.ToString();
                                else
                                    omodel.AddonFeeERO = string.Empty;
                                //var enroll = bankenrolls.Where(x => x.CustomerId == itm.cu.Id && x.IsActive == true && x.ArchiveStatusCode != EMPConstants.Archive).Select(x => x).FirstOrDefault();
                                //  if (isexist != null)
                                {
                                    omodel.EnrollmentStatus = bank.StatusCode == EMPConstants.EnrPending ? "Pending" : ((bank.StatusCode == EMPConstants.Submitted) ? "Submitted" :
                                    (bank.StatusCode == EMPConstants.Approved ? "Approved" : (bank.StatusCode == EMPConstants.Rejected ? "Rejected" :
                                    (bank.StatusCode == EMPConstants.Denied ? "Denied" : ""))));
                                }
                                //  else
                                //      omodel.EnrollmentStatus = "Not Started";
                                var BankFee = db.SubSiteBankFeesConfigs.Where(a => a.emp_CustomerInformation_ID == itm.cu.Id && a.BankMaster_ID == bank.Id).ToList();
                                if (BankFee.Count > 0)
                                {
                                    if (itm.cu.IsMSOUser == true)
                                        omodel.SBFee = BankFee.Where(a => a.ServiceOrTransmitter == 1).Select(a => a.BankMaxFees_MSO).FirstOrDefault().ToString(); // db.SubSiteBankFeesConfigs.Where(a => a.emp_CustomerInformation_ID == itm.cu.Id && a.BankMaster_ID == bank.Id && a.ServiceOrTransmitter == 1).Select(a => a.BankMaxFees_MSO).FirstOrDefault().ToString();
                                    else
                                        omodel.SBFee = BankFee.Where(a => a.ServiceOrTransmitter == 1).Select(a => a.BankMaxFees).FirstOrDefault().ToString();// db.SubSiteBankFeesConfigs.Where(a => a.emp_CustomerInformation_ID == itm.cu.Id && a.BankMaster_ID == bank.Id && a.ServiceOrTransmitter == 1).Select(a => a.BankMaxFees).FirstOrDefault().ToString();

                                    if (itm.cu.IsMSOUser == true)
                                        omodel.AddonFeeSB = BankFee.Where(a => a.ServiceOrTransmitter == 2).Select(a => a.BankMaxFees_MSO).FirstOrDefault().ToString();// db.SubSiteBankFeesConfigs.Where(a => a.emp_CustomerInformation_ID == itm.cu.Id && a.BankMaster_ID == bank.Id && a.ServiceOrTransmitter == 2).Select(a => a.BankMaxFees_MSO).FirstOrDefault().ToString();
                                    else
                                        omodel.AddonFeeSB = BankFee.Where(a => a.ServiceOrTransmitter == 2).Select(a => a.BankMaxFees).FirstOrDefault().ToString();// db.SubSiteBankFeesConfigs.Where(a => a.emp_CustomerInformation_ID == itm.cu.Id && a.BankMaster_ID == bank.Id && a.ServiceOrTransmitter == 2).Select(a => a.BankMaxFees).FirstOrDefault().ToString();
                                }

                            }
                            else
                            {
                                omodel.Bank = "";
                                omodel.SBAddOn = "";
                                omodel.AddonFeeSB = "";
                                omodel.AddonFeeERO = "";
                                omodel.EnrollmentStatus = "";
                                omodel.SBFee = "";
                            }
                            List<EnrollmentBankSelectionDTO> olist = oService.GetBankandFeesInfo(itm.cu.EntityId ?? 0, itm.cu.Id, Guid.Empty.ToString()).ToList(); //, bank.Id

                            omodel.uTaxFee = olist.Where(a => a.Name == "Service Bureau Fees").Select(a => a.Amount).FirstOrDefault().ToString();
                            //omodel.SBFee = olist.Where(a => a.Name == "Transmission Fees").Select(a => a.Amount).FirstOrDefault().ToString();

                            lstmodel.Add(omodel);
                        }
                    }
                }
                return lstmodel.OrderBy(a => a.CompanyName).ToList();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "ReportsService/GetFeeSetUpReport", Guid.Empty);
                return null;
            }
        }

        public List<string> GetChildData(List<string> UserIDlst)
        {
            try
            {
                db = new DatabaseEntities();
                //List<Guid> lstGuids = new List<Guid>();
                //lstGuids.Add(UserID);
                var subdetails = db.emp_CustomerInformation.Where(a => UserIDlst.Contains(a.ParentId.ToString()) && a.StatusCode != EMPConstants.Pending).Select(a => a.Id);
                foreach (var subi in subdetails)
                {
                    UserIDlst.Add(subi.ToString());
                    var ssub = db.emp_CustomerInformation.Where(a => a.ParentId == subi).Select(a => a.Id);
                    foreach (var ssi in ssub)
                    {
                        UserIDlst.Add(ssi.ToString());
                    }
                }
                return UserIDlst;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "ReportsService/GetChildData", Guid.Empty);
                return new List<string>();
            }
        }

        public List<CustomerDTO> GetCustomerMain()
        {
            try
            {
                //List<Guid> entitylist = new List<Guid>();
                //entitylist.Add(new Guid("3F11AC17-C5AA-412B-9DCD-4FD8BA1FE404"));
                //entitylist.Add(new Guid("734C5E39-5C33-47E0-8D75-6852DA39F907"));
                //entitylist.Add(new Guid("0676DFD0-DA29-41E3-A262-81CB528B796C"));

                List<int> entitylist = new List<int>();
                entitylist.Add((int)EMPConstants.Entity.MO);
                entitylist.Add((int)EMPConstants.Entity.SVB);
                entitylist.Add((int)EMPConstants.Entity.SO);
                entitylist.Add((int)EMPConstants.Entity.SOME);

                db = new DatabaseEntities();

                var data = (from cu in db.emp_CustomerInformation
                            join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                            where entitylist.Contains(cu.EntityId ?? 0) && cl.CrossLinkUserId != null && cu.StatusCode != EMPConstants.Pending
                            select new { cu.Id, cu.CompanyName, cl.EMPUserId, cu.EFIN }).ToList();
                var returndata = (from s in data
                                  select new CustomerDTO
                                  {
                                      ID = s.Id,
                                      CustomerName = s.CompanyName + " (" + s.EMPUserId + " - " + s.EFIN.Value.ToString().PadLeft(6, '0') + " )"
                                  }).ToList();
                return returndata.OrderBy(a => a.CustomerName).ToList();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "ReportsService/GetCustomerMain", Guid.Empty);
                return new List<CustomerDTO>();
            }
        }

        public List<CustomerDTO> GetCustomerSub(string ParentId)
        {
            try
            {
                List<CustomerDTO> lstCustomre = new List<CustomerDTO>();
                if (ParentId != "")
                {
                    List<string> lstCustGuid = ParentId.Split(',').ToList();

                    db = new DatabaseEntities();
                    var data = (from cu in db.emp_CustomerInformation
                                join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                                where lstCustGuid.Contains(cu.ParentId.ToString()) && cl.CrossLinkUserId != null && cu.StatusCode != EMPConstants.Pending
                                select new CustomerDTO
                                {
                                    ID = cu.Id,
                                    CustomerName = cu.CompanyName + " (" + cl.EMPUserId + " - " +
                                db.emp_CustomerLoginInformation.Where(a => a.CustomerOfficeId == cu.ParentId).Select(a => a.EMPUserId).FirstOrDefault() + ")"
                                });

                    var custIds = data.Select(x => x.ID).Distinct().ToList();
                    var custidsstring = custIds.Select(x => x.ToString()).ToList();
                    var data1 = (from cu in db.emp_CustomerInformation
                                 join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                                 where custidsstring.Contains(cu.ParentId.ToString()) && cl.CrossLinkUserId != null
                                 select new CustomerDTO
                                 {
                                     ID = cu.Id,
                                     CustomerName = cu.CompanyName + " (" + cl.EMPUserId + " - " +
                                 db.emp_CustomerLoginInformation.Where(a => a.CustomerOfficeId == cu.ParentId).Select(a => a.EMPUserId).FirstOrDefault() + ")"
                                 });

                    var sub = data.OrderBy(a => a.CustomerName).ToList();
                    var additional = data1.OrderBy(a => a.CustomerName).ToList();
                    sub.AddRange(additional);
                    return sub;
                }
                return lstCustomre;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "ReportsService/GetCustomerSub", Guid.Empty);
                return new List<CustomerDTO>();
            }
        }

        public List<FeeSetupReportDTO> GetNoBankAppSubmissionReport(string strguid)
        {
            try
            {
                db = new DatabaseEntities();
                List<string> lstCustGuid = strguid.Split(',').ToList();
                List<Guid> BE_CustomerList = db.BankEnrollments.Where(x => x.IsActive == true && x.StatusCode != EMPConstants.Ready && x.ArchiveStatusCode != EMPConstants.Archive).Select(a => a.CustomerId ?? Guid.Empty).ToList();
                //List<Guid> ES_CustomerList = (from be in db.EnrollmentBankSelections
                //                                  // join ssbc in db.SubSiteBankConfigs on new { x1 = be.BankId, x2 = be.CustomerId } equals new { x1 = ssbc.BankMaster_ID, x2 = ssbc.emp_CustomerInformation_ID }
                //                              where !BE_CustomerList.Contains(be.CustomerId) && be.StatusCode == EMPConstants.Active
                //                              select be.CustomerId).ToList();

                //var data = (from cu in db.emp_CustomerInformation
                //            join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                //            where ES_CustomerList.Contains(cu.Id) && cl.CrossLinkUserId != null
                //            select new { cu, cl }).Take(1);
                List<ReportCustomerModel> data = new List<ReportCustomerModel>();
                EnrollmentBankSelectionInfo.EnrollmentBankSelectionService oService = new EnrollmentBankSelectionInfo.EnrollmentBankSelectionService();
                if (db.emp_CustomerInformation.Any(a => lstCustGuid.Contains(a.Id.ToString())))
                {
                    List<string> lstGuids = GetChildData(lstCustGuid);

                    data = (from cu in db.emp_CustomerInformation
                            join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                            where !BE_CustomerList.Contains(cu.Id) && lstGuids.Contains(cu.Id.ToString()) && cl.CrossLinkUserId != null //&& ES_CustomerList.Contains(cu.Id)
                            select new ReportCustomerModel { cu = cu, cl = cl }).ToList();
                }
                else
                {
                    data = (from cu in db.emp_CustomerInformation
                            join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                            where !BE_CustomerList.Contains(cu.Id) && cl.CrossLinkUserId != null // && ES_CustomerList.Contains(cu.Id)
                            select new ReportCustomerModel { cu = cu, cl = cl }).ToList();
                }

                List<FeeSetupReportDTO> lstmodel = new List<FeeSetupReportDTO>();
                DropDownService _ddService = new DropDownService();
                //if (data.Count() > 0)
                {
                    foreach (var itm in data)
                    {
                        List<Guid> ES_BankList = (from be in db.EnrollmentBankSelections
                                                      // join ssbc in db.SubSiteBankConfigs on new { x1 = be.BankId, x2 = be.CustomerId } equals new { x1 = ssbc.BankMaster_ID, x2 = ssbc.emp_CustomerInformation_ID }
                                                  where be.CustomerId == itm.cu.Id && !BE_CustomerList.Contains(be.CustomerId) && be.StatusCode == EMPConstants.Active
                                                  select be.BankId).ToList();

                        if (ES_BankList.Count > 0)
                        {
                            if (itm.cu.EntityId != (int)EMPConstants.Entity.SO && itm.cu.EntityId != (int)EMPConstants.Entity.SOME && itm.cu.EntityId != (int)EMPConstants.Entity.SOME_SS)
                            {
                                List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                                EntityHierarchyDTOs = _ddService.GetEntityHierarchies(itm.cu.Id);

                                Guid TopParentId = Guid.Empty;
                                if (EntityHierarchyDTOs.Count > 0)
                                {
                                    var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                                    TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;

                                    var SubSiteBankConfigs = db.SubSiteBankConfigs.Where(o => o.emp_CustomerInformation_ID == TopParentId && ES_BankList.Contains(o.BankMaster_ID)).Select(o => o.BankMaster_ID).ToList();
                                    if (SubSiteBankConfigs.Count() == 0)
                                    {
                                        continue;
                                    }
                                    //  SubSiteBankConfigs
                                }
                                else
                                {
                                    var SubSiteBankConfigs = db.SubSiteBankConfigs.Where(o => o.emp_CustomerInformation_ID == itm.cu.Id && ES_BankList.Contains(o.BankMaster_ID)).Select(o => o.BankMaster_ID).ToList();
                                    if (SubSiteBankConfigs.Count() == 0)
                                    {
                                        continue;
                                    }
                                }
                            }

                            FeeSetupReportDTO omodel = new FeeSetupReportDTO();
                            omodel.UserID = itm.cl.EMPUserId;

                            if (itm.cu.ParentId != null)
                                omodel.ParentUserID = db.emp_CustomerLoginInformation.Where(a => a.CustomerOfficeId == itm.cu.ParentId).Select(a => a.CrossLinkUserId).FirstOrDefault();
                            else
                                omodel.ParentUserID = "";
                            omodel.CompanyName = itm.cu.CompanyName;
                            omodel.Efin = itm.cu.EFIN.ToString().PadLeft(6, '0');
                            omodel.MasterID = itm.cl.MasterIdentifier;
                            omodel.AccountOwner = itm.cu.BusinessOwnerFirstName;
                            lstmodel.Add(omodel);
                        }
                    }
                }

                return lstmodel.OrderBy(a => a.CompanyName).ToList();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "ReportsService/GetNoBankAppSubmissionReport", Guid.Empty);
                return new List<FeeSetupReportDTO>();
            }
        }

        public List<CustomerDTO> GetNoBankSubmissionCustomerMain()
        {
            try
            {
                //List<Guid> entitylist = new List<Guid>();
                //entitylist.Add(new Guid("3F11AC17-C5AA-412B-9DCD-4FD8BA1FE404"));
                //entitylist.Add(new Guid("734C5E39-5C33-47E0-8D75-6852DA39F907"));
                //entitylist.Add(new Guid("0676DFD0-DA29-41E3-A262-81CB528B796C"));

                List<int> entitylist = new List<int>();
                entitylist.Add((int)EMPConstants.Entity.MO);
                entitylist.Add((int)EMPConstants.Entity.SVB);
                entitylist.Add((int)EMPConstants.Entity.SO);
                entitylist.Add((int)EMPConstants.Entity.SOME);


                db = new DatabaseEntities();

                List<Guid> BE_CustomerList = db.BankEnrollments.Where(x => x.IsActive == true && x.ArchiveStatusCode != EMPConstants.Archive).Select(a => a.CustomerId ?? Guid.Empty).ToList();
                List<Guid> ES_CustomerList = (from be in db.EnrollmentBankSelections where !BE_CustomerList.Contains(be.CustomerId) && be.StatusCode == EMPConstants.Active select be.CustomerId).ToList();

                var data = (from cu in db.emp_CustomerInformation
                            join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                            where ES_CustomerList.Contains(cu.Id) && entitylist.Contains(cu.EntityId ?? 0) && cl.CrossLinkUserId != null && cu.StatusCode != EMPConstants.Pending
                            select new { cu.Id, cu.CompanyName, cl.EMPUserId, cu.EFIN }).Distinct().ToList();

                var returndata = (from s in data
                                  select new CustomerDTO
                                  {
                                      ID = s.Id,
                                      CustomerName = s.CompanyName + " (" + s.EMPUserId + " - " + s.EFIN.Value.ToString().PadLeft(6, '0') + " )"
                                  }).ToList();

                return returndata.OrderBy(a => a.CustomerName).ToList();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "ReportsService/GetNoBankSubmissionCustomerMain", Guid.Empty);
                return new List<CustomerDTO>();
            }
        }

        public List<LastLoginDTO> GetLastLoginReport(Guid UserID)
        {
            try
            {

                List<LastLoginDTO> ologin = new List<LastLoginDTO>();
                db = new DatabaseEntities();

                //var data = (from cu in db.emp_CustomerInformation
                //            join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                //            where cl.CrossLinkUserId != null
                //            select new { cu, cl }).Take(1);
                List<ReportCustomerModel> data = new List<ReportCustomerModel>();

                if (db.emp_CustomerInformation.Any(a => a.Id == UserID))
                {
                    List<string> userslst = new List<string>();
                    userslst.Add(UserID.ToString());
                    List<string> lstGuids = GetChildData(userslst);

                    data = (from cu in db.emp_CustomerInformation
                            join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                            where lstGuids.Contains(cu.Id.ToString()) && cl.CrossLinkUserId != null
                            select new ReportCustomerModel { cu = cu, cl = cl }).ToList();
                }
                else
                {
                    data = (from cu in db.emp_CustomerInformation
                            join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                            where cl.CrossLinkUserId != null
                            select new ReportCustomerModel { cu = cu, cl = cl }).ToList();
                }

                if (data.Count() > 0)
                {
                    foreach (var itm in data)
                    {
                        LastLoginDTO omodel = new LastLoginDTO();
                        omodel.MasterID = itm.cl.MasterIdentifier;
                        omodel.Efin = itm.cu.EFIN.ToString().PadLeft(6, '0');
                        omodel.UserID = itm.cl.EMPUserId;
                        omodel.CompanyName = itm.cu.CompanyName;
                        if (itm.cu.ParentId != null)
                            omodel.ParentUserID = db.emp_CustomerLoginInformation.Where(a => a.CustomerOfficeId == itm.cu.ParentId).Select(a => a.CrossLinkUserId).FirstOrDefault();
                        else
                            omodel.ParentUserID = "";
                        var tokendata = (from tk in db.TokenMasters where tk.UserId == itm.cu.Id orderby tk.IssuedOn descending select tk).FirstOrDefault();
                        if (tokendata != null)
                        {
                            omodel.LastLogin = itm.cu.MasterIdentifier;
                            omodel.DateTimeStamp = tokendata.IssuedOn.ToString("MM/dd/yyyy HH:mm tt");
                            omodel.IpAddress = tokendata.IPAddress;
                            ologin.Add(omodel);
                        }
                    }
                }
                return ologin.OrderBy(a => a.CompanyName).ToList();

            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "ReportsService/GetLastLoginReport", UserID);
                return new List<LastLoginDTO>();
            }
        }

        public List<EnrollmentDTO> GetAllNewEnrollmentCases(Guid UserId)
        {
            try
            {
                List<EnrollmentDTO> oenrolllist = new List<EnrollmentDTO>();

                db = new DatabaseEntities();
                var Bankenrolls = (from be in db.BankEnrollments where be.StatusCode == EMPConstants.Submitted && be.IsActive == true && be.ArchiveStatusCode != EMPConstants.Archive select be);
                var custList = Bankenrolls.Select(a => a.CustomerId).Distinct().ToList();
                var Customers = (from cu in db.emp_CustomerInformation
                                 join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                                 where custList.Contains(cu.Id)
                                 select new { cu, cl });

                foreach (var itm in Bankenrolls)
                {
                    EnrollmentDTO oEnrolModel = new EnrollmentDTO();
                    var cust = Customers.Where(a => a.cu.Id == itm.CustomerId).FirstOrDefault();
                    oEnrolModel.MasterID = cust.cu.MasterIdentifier;
                    oEnrolModel.UserID = cust.cl.EMPUserId.ToString();
                    if (cust.cu.ParentId != null)
                        oEnrolModel.ParentUserID = Customers.Where(a => a.cl.CustomerOfficeId == cust.cu.ParentId).Select(a => a.cl.CrossLinkUserId).FirstOrDefault();
                    else
                        oEnrolModel.ParentUserID = "";
                    oEnrolModel.CompanyName = cust.cu.CompanyName.ToString();
                    oEnrolModel.Efin = cust.cu.EFIN.ToString().PadLeft(6, '0');
                    oEnrolModel.AccountStatus = cust.cu.AccountStatus.ToString();
                    oEnrolModel.EROType = cust.cu.EROType.ToString();
                    oEnrolModel.CaseOrgin = string.Empty;
                    oEnrolModel.Product = string.Empty;
                    oEnrolModel.FuntionalArea = string.Empty;
                    oEnrolModel.Module = string.Empty;
                    oEnrolModel.Issue = string.Empty;
                    oEnrolModel.Status = string.Empty;
                    oEnrolModel.Casenumber = string.Empty;
                    if (cust.cu.IsMSOUser == true)
                        oEnrolModel.MSOUser = "Yes";
                    else
                        oEnrolModel.MSOUser = "No";
                    oEnrolModel.DateTimeOpened = itm.CreatedDate == null ? string.Empty : Convert.ToDateTime(itm.CreatedDate).ToString("MM/dd/yyyy hh:mm tt");
                    oEnrolModel.DateTimeLastModified = itm.UpdatedDate == null ? string.Empty : Convert.ToDateTime(itm.UpdatedDate).ToString("MM/dd/yyyy hh:mm tt");
                    oenrolllist.Add(oEnrolModel);
                }
                return oenrolllist.OrderBy(a => a.CompanyName).ToList();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "ReportsService/GetAllNewEnrollmentCases", UserId);
                return new List<EnrollmentDTO>();
            }
        }

        public List<EnrollmentDTO> GetAllCloseEnrollmentCases(Guid UserId)
        {
            try
            {
                List<EnrollmentDTO> oenrolllist = new List<EnrollmentDTO>();

                db = new DatabaseEntities();
                var Bankenrolls = (from be in db.BankEnrollments where (be.StatusCode == EMPConstants.Approved || be.StatusCode == EMPConstants.Rejected) && be.IsActive == true && be.ArchiveStatusCode != EMPConstants.Archive select be);
                var custList = Bankenrolls.Select(a => a.CustomerId).Distinct().ToList();
                var Customers = (from cu in db.emp_CustomerInformation
                                 join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                                 where custList.Contains(cu.Id)
                                 select new { cu, cl });

                foreach (var itm in Bankenrolls)
                {
                    EnrollmentDTO oEnrolModel = new EnrollmentDTO();
                    var cust = Customers.Where(a => a.cu.Id == itm.CustomerId).FirstOrDefault();
                    oEnrolModel.MasterID = cust.cu.MasterIdentifier;
                    oEnrolModel.UserID = cust.cl.EMPUserId.ToString();
                    if (cust.cu.ParentId != null)
                        oEnrolModel.ParentUserID = Customers.Where(a => a.cl.CustomerOfficeId == cust.cu.ParentId).Select(a => a.cl.CrossLinkUserId).FirstOrDefault();
                    else
                        oEnrolModel.ParentUserID = "";
                    oEnrolModel.CompanyName = cust.cu.CompanyName.ToString();
                    oEnrolModel.Efin = cust.cu.EFIN.ToString().PadLeft(6, '0');
                    oEnrolModel.AccountStatus = cust.cu.AccountStatus.ToString();
                    oEnrolModel.EROType = cust.cu.EROType.ToString();
                    oEnrolModel.CaseOrgin = string.Empty;
                    oEnrolModel.Product = string.Empty;
                    oEnrolModel.FuntionalArea = string.Empty;
                    oEnrolModel.Module = string.Empty;
                    oEnrolModel.Issue = string.Empty;
                    oEnrolModel.Status = string.Empty;
                    oEnrolModel.Casenumber = string.Empty;
                    if (cust.cu.IsMSOUser == true)
                        oEnrolModel.MSOUser = "Yes";
                    else
                        oEnrolModel.MSOUser = "No";
                    oEnrolModel.DateTimeOpened = itm.CreatedDate == null ? string.Empty : Convert.ToDateTime(itm.CreatedDate).ToString("MM/dd/yyyy hh:mm tt");
                    oEnrolModel.DateTimeLastModified = itm.UpdatedDate == null ? string.Empty : Convert.ToDateTime(itm.UpdatedDate).ToString("MM/dd/yyyy hh:mm tt");
                    oenrolllist.Add(oEnrolModel);
                }
                return oenrolllist.OrderBy(a => a.CompanyName).ToList();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "ReportsService/GetAllCloseEnrollmentCases", UserId);
                return new List<EnrollmentDTO>();
            }
        }

        public List<EnrollmentDTO> GetStaleEnrollmentCases(Guid UserId)
        {
            try
            {
                List<EnrollmentDTO> oenrolllist = new List<EnrollmentDTO>();

                db = new DatabaseEntities();
                DateTime cudate = System.DateTime.Now.AddDays(-2);
                var Bankenrolls = (from be in db.BankEnrollments where be.UpdatedDate >= cudate && be.IsActive == true && be.ArchiveStatusCode != EMPConstants.Archive select be);
                var custList = Bankenrolls.Select(a => a.CustomerId).Distinct().ToList();
                var Customers = (from cu in db.emp_CustomerInformation
                                 join cl in db.emp_CustomerLoginInformation on cu.Id equals cl.CustomerOfficeId
                                 where custList.Contains(cu.Id)
                                 select new { cu, cl });

                foreach (var itm in Bankenrolls)
                {
                    EnrollmentDTO oEnrolModel = new EnrollmentDTO();
                    var cust = Customers.Where(a => a.cu.Id == itm.CustomerId).FirstOrDefault();
                    oEnrolModel.MasterID = cust.cu.MasterIdentifier;
                    oEnrolModel.UserID = cust.cl.EMPUserId.ToString();
                    if (cust.cu.ParentId != null)
                        oEnrolModel.ParentUserID = Customers.Where(a => a.cl.CustomerOfficeId == cust.cu.ParentId).Select(a => a.cl.CrossLinkUserId).FirstOrDefault();
                    else
                        oEnrolModel.ParentUserID = "";
                    oEnrolModel.CompanyName = cust.cu.CompanyName.ToString();
                    oEnrolModel.Efin = cust.cu.EFIN.ToString().PadLeft(6, '0');
                    oEnrolModel.AccountStatus = cust.cu.AccountStatus.ToString();
                    oEnrolModel.EROType = cust.cu.EROType.ToString();
                    oEnrolModel.CaseOrgin = string.Empty;
                    oEnrolModel.Product = string.Empty;
                    oEnrolModel.FuntionalArea = string.Empty;
                    oEnrolModel.Module = string.Empty;
                    oEnrolModel.Issue = string.Empty;
                    oEnrolModel.Status = string.Empty;
                    oEnrolModel.Casenumber = string.Empty;
                    if (cust.cu.IsMSOUser == true)
                        oEnrolModel.MSOUser = "Yes";
                    else
                        oEnrolModel.MSOUser = "No";
                    oEnrolModel.DateTimeOpened = itm.CreatedDate == null ? string.Empty : Convert.ToDateTime(itm.CreatedDate).ToString("MM/dd/yyyy hh:mm tt");
                    oEnrolModel.DateTimeLastModified = itm.UpdatedDate == null ? string.Empty : Convert.ToDateTime(itm.UpdatedDate).ToString("MM/dd/yyyy hh:mm tt");
                    oenrolllist.Add(oEnrolModel);
                }
                return oenrolllist.OrderBy(a => a.DateTimeLastModified).ToList();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "ReportsService/GetStaleEnrollmentCases", UserId);
                return new List<EnrollmentDTO>();
            }
        }

        public List<EnrollmentStatusReport> getEnrollmentsList(string strguid)
        {
            try
            {
                List<EnrollmentStatusReport> _enrList = new List<EnrollmentStatusReport>();
                List<string> lstCustGuid = strguid.Split(',').ToList();

                var enrollments = (from s in db.BankEnrollments
                                   join b in db.BankMasters on s.BankId equals b.Id
                                   join c in db.emp_CustomerInformation on s.CustomerId equals c.Id
                                   where s.StatusCode == EMPConstants.Approved && s.IsActive == true && s.ArchiveStatusCode != EMPConstants.Archive
                                   orderby s.UpdatedDate descending
                                   select new { c.BusinessOwnerFirstName, s.BankId, b.BankName, b.BankCode, s.StatusCode, c.Id, c.CompanyName, c.EFIN, EnrId = s.Id, c.ParentId }).Take(1).ToList();
                if (db.emp_CustomerInformation.Any(a => lstCustGuid.Contains(a.Id.ToString())))
                {
                    List<string> lstGuids = GetChildData(lstCustGuid);

                    enrollments = (from s in db.BankEnrollments
                                   join b in db.BankMasters on s.BankId equals b.Id
                                   join c in db.emp_CustomerInformation on s.CustomerId equals c.Id
                                   where lstGuids.Contains(c.Id.ToString()) && s.StatusCode == EMPConstants.Approved && s.IsActive == true && s.ArchiveStatusCode != EMPConstants.Archive
                                   orderby s.UpdatedDate descending
                                   select new { c.BusinessOwnerFirstName, s.BankId, b.BankName, b.BankCode, s.StatusCode, c.Id, c.CompanyName, c.EFIN, EnrId = s.Id, c.ParentId }).ToList();
                }
                else
                {
                    enrollments = (from s in db.BankEnrollments
                                   join b in db.BankMasters on s.BankId equals b.Id
                                   join c in db.emp_CustomerInformation on s.CustomerId equals c.Id
                                   where s.StatusCode == EMPConstants.Approved && s.IsActive == true && s.ArchiveStatusCode != EMPConstants.Archive
                                   orderby s.UpdatedDate descending
                                   select new { c.BusinessOwnerFirstName, s.BankId, b.BankName, b.BankCode, s.StatusCode, c.Id, c.CompanyName, c.EFIN, EnrId = s.Id, c.ParentId }).ToList();
                }

                foreach (var item in enrollments)
                {
                    EnrollmentStatusReport enr = new EnrollmentStatusReport();
                    enr.AccountOwner = item.BusinessOwnerFirstName;
                    if (item.BankCode == "S")
                    {
                        var tpg = db.BankEnrollmentForTPGs.Where(x => x.CustomerId == item.Id && x.StatusCode == EMPConstants.Active).Select(x => x).FirstOrDefault();
                        if (tpg != null)
                        {
                            enr.AddonFee = tpg.AddonFee;
                            enr.SBFee = tpg.ServiceBureauFee;
                            enr.LastModifiedDate = tpg.UpdatedDate.Value.ToString("MM/dd/yyyy");
                            enr.LastModifiedUser = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == tpg.UpdatedBy).Select(x => x.EMPUserId).FirstOrDefault();
                        }
                    }
                    if (item.BankCode == "V")
                    {
                        var ra = db.BankEnrollmentForRAs.Where(x => x.CustomerId == item.Id && x.StatusCode == EMPConstants.Active).Select(x => x).FirstOrDefault();
                        if (ra != null)
                        {
                            enr.AddonFee = ra.TransmissionAddon;
                            enr.SBFee = ra.SbFee;
                            enr.LastModifiedDate = ra.UpdatedDate.Value.ToString("MM/dd/yyyy");
                            enr.LastModifiedUser = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == ra.UpdatedBy).Select(x => x.EMPUserId).FirstOrDefault();
                        }
                    }
                    if (item.BankCode == "R")
                    {
                        var rb = db.BankEnrollmentForRBs.Where(x => x.CustomerId == item.Id && x.StatusCode == EMPConstants.Active).Select(x => x).FirstOrDefault();
                        if (rb != null)
                        {
                            enr.AddonFee = rb.TransimissionAddon;
                            enr.SBFee = rb.SBFee;
                            enr.LastModifiedDate = rb.UpdatedDate.Value.ToString("MM/dd/yyyy");
                            enr.LastModifiedUser = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == rb.UpdatedBy).Select(x => x.EMPUserId).FirstOrDefault();
                        }
                    }
                    enr.Bank = item.BankName;
                    enr.Company = item.CompanyName;
                    enr.Efin = item.EFIN.Value.ToString().PadLeft(6, '0');
                    enr.ErrorMessage = string.Join(",", db.BankEnrollmentHistories.Where(x => x.EnrollmentId == item.EnrId && x.Status == false).Select(x => x.Message).ToArray());
                    enr.ParentId = item.ParentId != null ? db.emp_CustomerInformation.Where(x => x.Id == item.ParentId).Select(x => x.CompanyName).FirstOrDefault() : "";
                    enr.Status = EMPConstants.ApprovedService;
                    var date = db.BankEnrollmentStatus.Where(x => x.EnrollmentId == item.EnrId && x.Status == EMPConstants.SubmittedService).OrderByDescending(x => x.CreatedDate).Select(x => x.CreatedDate).FirstOrDefault();
                    enr.SubmissionDate = date.HasValue ? date.Value.ToString("MM/dd/yyyy") : "";
                    var loginfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == item.Id).Select(x => x).FirstOrDefault();
                    enr.UserId = loginfo.EMPUserId;
                    enr.MasterId = loginfo.MasterIdentifier;
                    enr.Id = item.Id.ToString();
                    enr.Parent = item.ParentId.HasValue ? item.ParentId.Value.ToString() : enr.Id;
                    _enrList.Add(enr);
                }
                return _enrList.OrderBy(x => x.Company).ToList();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "ReportsService/GetStaleEnrollmentCases", Guid.Empty);
                return new List<EnrollmentStatusReport>();
            }
        }

    }

    public class ReportCustomerModel
    {
        public emp_CustomerInformation cu { get; set; }
        public emp_CustomerLoginInformation cl { get; set; }
    }
}
