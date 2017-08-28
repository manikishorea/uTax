using System;
using System.Collections.Generic;
using System.Linq;
using EMPEntityFramework.Edmx;
using System.Data.Entity;
using EMP.Core.Utilities;
using MoreLinq;
using EMPPortal.Core.Utilities;
using EMPPortal.Transactions.DropDowns;
using EMPPortal.Transactions.DropDowns.DTO;

namespace EMPPortal.Transactions.OfficeManagementTransactions
{
    public class OfficeManagementService
    {
        private DatabaseEntities db = new DatabaseEntities();

        public bool UpdateOfficeManagement(Guid Id, string SalesYearId)
        {
            DropDownService ddService = new DropDownService();
            List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
            EntityHierarchyDTOs = ddService.GetEntityHierarchies(Id);

            Guid RootParentId = Guid.Empty;
            int Level = EntityHierarchyDTOs.Count;
            if (EntityHierarchyDTOs.Count > 0)
            {
                var LevelOne = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                if (LevelOne != null)
                {
                    RootParentId = LevelOne.CustomerId ?? Guid.Empty;
                }
            }


            var mainResult = db.OfficeManagementGridSP(Id.ToString(), SalesYearId, RootParentId == Id ? null : RootParentId.ToString());
            return true;
        }

        public List<OfficeManagementDTO2> GetCustomerList(List<OfficeManagementGridSearch_Result> data)
        {
            try
            {
                var data1 = data.Where(o => o.ParentId == null).ToList();
                var entityids = data.Select(x => x.EntityId).ToList();
                var customerids = data.Select(x => x.CustomerId).Distinct().ToList();
                var enttities = db.EntityMasters.Where(x => entityids.Contains(x.Id)).ToList();
                var masteractions = (from s in db.EMP_ActionMaser
                                     where entityids.Contains(s.EntityId) && s.Status == EMPConstants.Active
                                     orderby s.DisplayOrder
                                     select new MasterActions
                                     {
                                         IsParent = s.ParentId,
                                         Name = s.Name,
                                         Display = s.ForActive,
                                         EntityId = s.EntityId
                                     }).ToList();
                var subconfigs = db.SubSiteOfficeConfigs.Where(o => customerids.Contains(o.RefId)).ToList();

                List<OfficeManagementDTO2> customerlst = new List<OfficeManagementDTO2>();
                foreach (var itm in data1)
                {
                    OfficeManagementDTO2 ocustomer = new OfficeManagementDTO2();
                    ocustomer.Id = itm.CustomerId;
                    ocustomer.CustomerId = itm.CustomerId;
                    ocustomer.ParentId = itm.ParentId ?? Guid.Empty;
                    ocustomer.str_ParentId = itm.ParentId.ToString() ?? "";
                    ocustomer.CompanyName = itm.CompanyName;
                    ocustomer.BusinessOwnerFirstName = itm.BusinessOwnerFirstName;
                    ocustomer.OfficePhone = itm.OfficePhone;
                    ocustomer.BusinessOwnerFirstName = itm.BusinessOwnerFirstName;
                    ocustomer.BusinessOwnerLastName = itm.BusinessOwnerLastName;
                    ocustomer.EntityId = itm.EntityId ?? 0;
                    ocustomer.BaseEntityId = itm.BaseEntityId ?? 0;
                    ocustomer.EROType = enttities.Where(o => o.Id == ocustomer.EntityId).Select(o => o.Name).FirstOrDefault() ?? "";
                    ocustomer.EMPUserId = itm.EMPUserId;
                    ocustomer.EMPPassword = !string.IsNullOrEmpty(itm.EMPPassword) ? PasswordManager.DecryptText(itm.EMPPassword) : "";

                    ocustomer.EFIN = itm.EFIN;
                    ocustomer.EFINStatus = itm.EFINStatus;

                    int obstatus = string.IsNullOrEmpty(itm.OnboardingStatus) ? 0 : Convert.ToInt32(itm.OnboardingStatus);
                    if (obstatus != 0)
                    {
                        ocustomer.OnboardingStatus = ((EMPConstants.OnboardStatus)obstatus).ToString().Replace("_", " ");
                        if (obstatus == (int)EMPConstants.OnboardStatus.Started_But_Incomplete)
                        {
                            ocustomer.OnboardStatusTooltip = itm.OnboardStatusTooltip;
                        }
                    }
                    else
                        ocustomer.OnboardingStatus = "";

                    int EFIN = ocustomer.EFIN ?? 0;
                    string EFINText = EFIN.ToString().PadLeft(6, '0');

                    if (ocustomer.EFINStatus == 16 || ocustomer.EFINStatus == 19)
                    {
                        ocustomer.EFINStatusText = EFINText;
                    }
                    else if (ocustomer.EFINStatus == 21)
                    {
                        ocustomer.EFINStatusText = (EFIN > 0) ? EFINText + "<u><b>S</b></u>" : "Sharing";
                    }
                    else if (ocustomer.EFINStatus == 17 || ocustomer.EFINStatus == 20)
                    {
                        ocustomer.EFINStatusText = "Applied";
                    }
                    else if (ocustomer.EFINStatus == 18)
                    {
                        ocustomer.EFINStatusText = "Not Required";
                    }
                    else
                    {
                        ocustomer.EFINStatusText = EFINText;
                    }


                    ocustomer.MasterIdentifier = itm.MasterIdentifier;
                    ocustomer.IsActivationCompleted = itm.IsActivationCompleted ?? 0;

                    if (itm.IsActivationCompleted == null || itm.IsActivationCompleted == 0)
                        ocustomer.AccountStatus = itm.AccountStatus == null ? "Not Active" : itm.AccountStatus;
                    else
                    {
                        ocustomer.AccountStatus = (itm.IsHold ?? false) ? "Hold" :(itm.AccountStatus=="Review"?"Review":"Active");
                    }

                    if (itm.IsHold ?? false && (itm.EntityId == (int)EMPConstants.Entity.MO || itm.EntityId == (int)EMPConstants.Entity.SVB || itm.EntityId == (int)EMPConstants.Entity.SO))
                    {
                        if (itm.IsVerified ?? false)
                        {
                            var custInfo = db.emp_CustomerInformation.Where(x => x.Id == itm.CustomerId).FirstOrDefault();
                            if (custInfo != null)
                            {
                                if ((custInfo.Federal_EF_Fee_New__c ?? 0) != 0 || (custInfo.State_EF_Fee_New__c ?? 0) != 0)
                                {
                                    string bnkid = Guid.Empty.ToString();
                                    if ((itm.ActiveBankId ?? Guid.Empty) == Guid.Empty)
                                    {
                                        var DefaultBank2 = (from enrollbank in db.EnrollmentBankSelections
                                                            where enrollbank.CustomerId == itm.CustomerId && enrollbank.StatusCode == EMPConstants.Active
                                                            orderby enrollbank.BankSubmissionStatus descending, enrollbank.LastUpdatedDate descending
                                                            select enrollbank.BankId).FirstOrDefault();

                                        if (DefaultBank2 != null)
                                        {
                                            bnkid = DefaultBank2.ToString();
                                        }
                                    }
                                    else
                                        bnkid = (itm.ActiveBankId ?? Guid.Empty).ToString();

                                    ocustomer.Paymentoptions = "PaymentOptions/efile?Id=" + itm.CustomerId.ToString() + "&entitydisplayid=1&entityid=" + itm.EntityId + "&ParentId=00000000-0000-0000-0000-000000000000&ptype=enrollment&bankid=" + bnkid;
                                }
                                if (string.IsNullOrEmpty(ocustomer.Paymentoptions))
                                {
                                    bool _canshow = false;
                                    if (!string.IsNullOrEmpty(custInfo.Cash_Saver__c) || !string.IsNullOrEmpty(custInfo.LOC_Program_Participant__c) || (custInfo.A_R_Amount_Due_Credit__c ?? 0) != 0)
                                    {
                                        if (!string.IsNullOrEmpty(custInfo.Cash_Saver__c))
                                        {
                                            if (custInfo.Cash_Saver__c.ToLower() == "true" && (custInfo.pymt__Balance__c ?? 0) != 0)
                                                _canshow = true;
                                        }
                                        if (!string.IsNullOrEmpty(custInfo.LOC_Program_Participant__c))
                                        {
                                            if (custInfo.LOC_Program_Participant__c.ToLower() == "true" && (custInfo.Total_Amount_Loaned__c ?? 0) != 0)
                                                _canshow = true;
                                        }
                                        if ((custInfo.A_R_Amount_Due_Credit__c ?? 0) != 0)
                                            _canshow = true;
                                    }
                                    if (_canshow)
                                    {
                                        string bnkid = Guid.Empty.ToString();
                                        if ((itm.ActiveBankId ?? Guid.Empty) == Guid.Empty)
                                        {
                                            var DefaultBank2 = (from enrollbank in db.EnrollmentBankSelections
                                                                where enrollbank.CustomerId == itm.CustomerId && enrollbank.StatusCode == EMPConstants.Active
                                                                orderby enrollbank.BankSubmissionStatus descending, enrollbank.LastUpdatedDate descending
                                                                select enrollbank.BankId).FirstOrDefault();

                                            if (DefaultBank2 != null)
                                            {
                                                bnkid = DefaultBank2.ToString();
                                            }
                                        }
                                        else
                                            bnkid = (itm.ActiveBankId ?? Guid.Empty).ToString();
                                        ocustomer.Paymentoptions = "/PaymentOptions/OutstandingBalance?Id=" + itm.CustomerId.ToString() + "&entitydisplayid=1&entityid=" + itm.EntityId + "&ParentId=00000000-0000-0000-0000-000000000000&ptype=enrollment&bankid=" + bnkid;
                                    }
                                }
                            }
                        }
                    }

                    ocustomer.SalesYearId = itm.SalesYearId ?? Guid.Empty;

                    if (ocustomer.EntityId == (int)EMPConstants.Entity.SO || ocustomer.EntityId == (int)EMPConstants.Entity.SOME || ocustomer.EntityId == (int)EMPConstants.Entity.SOME_SS)
                    {
                        ocustomer.IsTaxReturn = true;// itm.IsTaxReturn;
                    }
                    else
                    {
                        ocustomer.IsTaxReturn = itm.IsTaxReturn;
                    }

                    ocustomer.StatusCode = itm.StatusCode;
                    ocustomer.IsEnrollmentCompleted = itm.IsEnrollmentCompleted ?? false;
                    //ocustomer.CreatedDate = itm.cli.CreatedDate ?? Convert.ToDateTime("01/01/2000");
                    //ocustomer.LastUpdatedDate = itm.LastUpdatedDate ?? Convert.ToDateTime("01/01/2000");
                    //  ocustomer.IsEnrollmentCompleted = itm.IsEnrollmentCompleted;
                    if (ocustomer.IsEnrollmentCompleted == true)
                    {
                        decimal uTaxSVBFee = itm.uTaxSVBFee ?? 0;
                        decimal SVBAddonFee = itm.SVBAddonFee ?? 0;
                        decimal SVBEnrollAddonFee = itm.SVBEnrollAddonFee ?? 0;

                        ocustomer.TRANCanEnroll = itm.TRANCanEnroll ?? false;
                        ocustomer.TRANCanAddon = itm.TRANCanAddon ?? false;
                        ocustomer.SVBCanAddon = itm.SVBCanAddon ?? false;
                        ocustomer.SVBCanEnroll = itm.SVBCanEnroll ?? false;

                        if (ocustomer.TRANCanEnroll.Value || ocustomer.TRANCanAddon.Value || ocustomer.SVBCanAddon.Value || ocustomer.SVBCanEnroll.Value)
                        {
                            ocustomer.TotalServiceFee = Convert.ToString(uTaxSVBFee + SVBEnrollAddonFee);
                            ocustomer.ServiceTooltip = ocustomer.ServiceTooltip + "<span>uTax Service Beaure Fee : " + uTaxSVBFee.ToString() + "</span><br/>";
                            //ocustomer.ServiceTooltip = ocustomer.ServiceTooltip + "<span>" + itm.CompanyName + " : " + (ocustomer.SVBCanAddon == true ? SVBAddonFee.ToString() : "No Add on") + "</span><br/>";
                            ocustomer.ServiceTooltip = ocustomer.ServiceTooltip + "<span>Enrollment Add-on : " + (ocustomer.SVBCanEnroll == true ? SVBEnrollAddonFee.ToString() : "Not Allowed") + "</span><br/>";


                            decimal CrosslinkTransFee = itm.CrosslinkTransFee ?? 0;
                            decimal TransAddonFee = itm.TransAddonFee ?? 0;
                            decimal TransEnrollAddonFee = itm.TransEnrollAddonFee ?? 0;

                            ocustomer.TotalTransFee = Convert.ToString(CrosslinkTransFee + TransEnrollAddonFee);
                            ocustomer.TransTooltip = ocustomer.TransTooltip + "<span>Cross Link Transmitter Fee : " + CrosslinkTransFee.ToString() + "</span><br/>";
                            //ocustomer.TransTooltip = ocustomer.TransTooltip + "<span>" + itm.CompanyName + " : " + (ocustomer.TRANCanAddon == true ? itm.TransAddonFee.ToString() : "No Add on") + "</span><br/>";
                            ocustomer.TransTooltip = ocustomer.TransTooltip + "<span>Enrollment Add-on : " + (ocustomer.TRANCanEnroll == true ? TransEnrollAddonFee.ToString() : "Not Allowed") + "</span><br/>";
                        }
                        else
                        {
                            ocustomer.TotalServiceFee = "";
                            ocustomer.ServiceTooltip = "";
                            ocustomer.TotalTransFee = "";
                            ocustomer.TransTooltip = "";
                        }
                    }
                    else
                    {
                        ocustomer.TotalServiceFee = "";
                        ocustomer.ServiceTooltip = "";
                        ocustomer.TotalTransFee = "";
                        ocustomer.TransTooltip = "";
                    }


                    ocustomer.ActiveBankId = itm.ActiveBankId;
                    ocustomer.ActiveBankName = itm.ActiveBankName;

                    ocustomer.CanEnrollmentAllowed = itm.CanEnrollmentAllowed ?? false;
                    ocustomer.CanEnrollmentAllowedForMain = itm.CanEnrollmentAllowedForMain ?? false;
                    ocustomer.SubmissionDate = itm.EnrollmentSubmittionDate == null ? "" : itm.EnrollmentSubmittionDate.Value.ToString("MM'/'dd'/'yyyy HH:mm");
                    ocustomer.EnrollmentStatus = itm.EnrollmentStatus;
                    if (ocustomer.EnrollmentStatus == "INP")
                        ocustomer.EnrollmentStatus = "Incomplete";
                    else
                        ocustomer.EnrollmentStatus = ocustomer.EnrollmentStatus == EMPConstants.Ready ? "Unsuccessful" : ((ocustomer.EnrollmentStatus == EMPConstants.Submitted) ? "Submitted" :
                                  (ocustomer.EnrollmentStatus == EMPConstants.Approved ? "Approved" : (ocustomer.EnrollmentStatus == EMPConstants.Rejected ? "Rejected" :
                                  (ocustomer.EnrollmentStatus == EMPConstants.Denied ? "Denied" : (ocustomer.EnrollmentStatus == EMPConstants.Cancelled ? "Cancelled" : (ocustomer.EnrollmentStatus == EMPConstants.EnrPending ? "Pending" : "Not Started"))))));

                    //ocustomer.TRANCanEnroll = itm.TRANCanEnroll ?? false;
                    //ocustomer.TRANCanAddon = itm.TRANCanAddon ?? false;
                    //ocustomer.SVBCanAddon = itm.SVBCanAddon ?? false;
                    //ocustomer.SVBCanEnroll = itm.SVBCanEnroll ?? false;

                    //ocustomer.TotalServiceFee = Convert.ToString(itm.uTaxSVBFee +  itm.SVBEnrollAddonFee);
                    //ocustomer.ServiceTooltip = ocustomer.ServiceTooltip + "<span>uTax Service Beaure Fee : " + itm.uTaxSVBFee.ToString() + "</span><br/>";
                    ////ocustomer.ServiceTooltip = ocustomer.ServiceTooltip + "<span>" + itm.CompanyName + " : " + (ocustomer.SVBCanAddon == true ? itm.SVBAddonFee.ToString() : "No Add on") + "</span><br/>";
                    //ocustomer.ServiceTooltip = ocustomer.ServiceTooltip + "<span>Enrollment Add-on : " + (ocustomer.SVBCanEnroll == true ? itm.SVBEnrollAddonFee.ToString() : "Not Allowed") + "</span><br/>";

                    //ocustomer.TotalTransFee = Convert.ToString(itm.CrosslinkTransFee + itm.TransEnrollAddonFee);
                    //ocustomer.TransTooltip = ocustomer.TransTooltip + "<span>Cross Link Transmission Fee : " + itm.CrosslinkTransFee + "</span><br/>";
                    ////ocustomer.TransTooltip = ocustomer.TransTooltip + "<span>" + itm.CompanyName + " : " + (ocustomer.TRANCanAddon == true ? itm.TransAddonFee.ToString() : "No Add on") + "</span><br/>";
                    //ocustomer.TransTooltip = ocustomer.TransTooltip + "<span>Enrollment Add-on : " + (ocustomer.TRANCanEnroll == true ? itm.TransEnrollAddonFee.ToString() : "Not Allowed") + "</span><br/>";


                    ocustomer.ApprovedBank = itm.ApprovedBank;// + (ocustomer.EnrollmentStatus != "Approved" && !string.IsNullOrEmpty(itm.ApprovedBank) ? " (Previous)" : "");
                    ocustomer.RejectedBanks = itm.RejectedBanks;
                    ocustomer.UnlockedBanks = itm.UnlockedBanks;
                    ocustomer.SubmittedBanks = itm.SubmittedBanks;
                    int IsArchived = itm.IsArchived ?? 0;
                    #region "Actions"

                    var actions = (from s in masteractions
                                   where s.EntityId == itm.EntityId
                                   orderby s.Display
                                   select new Actions
                                   {
                                       Display = s.Display,
                                       IsParent = s.IsParent,
                                       Name = s.Name
                                   }).ToList();

                    if (IsArchived == 0)
                    {
                        actions = actions.Where(o => !o.Name.Contains("View Archive")).ToList();
                    }

                    ocustomer.Actions = actions;
                    #endregion
                    var ChilddataList1 = data.Where(o => o.ParentId == ocustomer.CustomerId).ToList();
                    var ChildDataLst = GetChildInfoSearch2(ChilddataList1, data, itm.EMPUserId, enttities, masteractions, subconfigs);
                    ocustomer.ChildCustomerInfo = ChildDataLst;
                    if (ocustomer.ChildCustomerInfo.Count > 0)
                        ocustomer.ChildInfo = "<a id='" + itm.Id + "' onClick=\"fnChildInfo(this)\"><i class='fa fa-plus'></i></a>";
                    else
                        ocustomer.ChildInfo = "";
                    customerlst.Add(ocustomer);
                }

                return customerlst;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetSearchCustomerInformationNewGrid", Guid.Empty);
                return new List<OfficeManagementDTO2>();
            }
        }

        public CustomerInfoNewGrid GetOfficeManagement(CustomerInfoNewGrid omodel)
        {
            try
            {
                int CounttotalRecords = 0;

                if (omodel.GridType == -1)
                {
                    var data = db.OfficeManagementGridRecord(omodel.UserType, omodel.UserId.ToString(), omodel.start, omodel.length).ToList();


                    if (omodel.UserType == 0)
                    {
                        CounttotalRecords = db.OfficeManagements.Where(o => o.StatusCode != EMPConstants.NewCustomer && o.ParentId == null).Count();
                    }
                    else
                    {
                        CounttotalRecords = 1;
                    }

                    omodel.recordsTotal = CounttotalRecords;
                    omodel.Customerlst = GetCustomerList(data.ToList()).OrderBy(a => a.CompanyName).ToList();
                }
                else if (omodel.GridType == 0)
                {
                    if (!string.IsNullOrEmpty(omodel.SearchText))
                    {
                        omodel.SearchText = omodel.SearchText.Trim();
                    }
                    var data = db.OfficeManagementGridSearch(omodel.SearchType, omodel.SearchText, omodel.UserType, omodel.UserId.ToString(), omodel.start, omodel.length).ToList(); //, omodel.start, omodel.length
                    CounttotalRecords = data.Where(o => o.StatusCode != EMPConstants.NewCustomer && o.ParentId == null).Count();
                    omodel.recordsTotal = CounttotalRecords;
                    var lst = GetCustomerList(data.ToList()).OrderBy(a => a.CompanyName).ToList();
                    omodel.Customerlst = lst.Skip(omodel.start).Take(omodel.length).ToList();
                }
                else if (omodel.GridType == 1)
                {
                    omodel.SiteType = omodel.SiteType == null ? "" : omodel.SiteType;
                    omodel.EnrollmentStatus = omodel.EnrollmentStatus == null ? "" : omodel.EnrollmentStatus;
                    omodel.BankPartner = omodel.BankPartner == null ? "" : omodel.BankPartner;
                    omodel.OnBoardingStatus = omodel.OnBoardingStatus == null ? "" : omodel.OnBoardingStatus;
                    omodel.Status = omodel.Status == null ? "" : omodel.Status;


                    string statusfilter = omodel.Status.Replace("ACT", "ACT,").Replace("INA", "INA,").Replace("REV", "REV,").Replace("HLD", "HLD,");
                    var res = db.OfficeManagementGridFilter(statusfilter, omodel.SiteType, omodel.EnrollmentStatus, omodel.BankPartner, omodel.OnBoardingStatus, omodel.UserType, omodel.UserId.ToString()).ToList();

                    //var totalfilters = GetCustomersFilter(omodel, res);
                    //totalfilters = totalfilters.Distinct().ToList();
                    //var filtereddata = res.Where(x => totalfilters.Contains(x.CustomerId.Value)).ToList();

                    //var data = db.OfficeManagementGridFilter(StatusFilter, SiteTypeFilter, BankPartnerFilter, EnrollmentStatusFilter, OnBoardingStatusFilter, omodel.UserType, omodel.UserId.ToString()).ToList(); //, omodel.start, omodel.length

                    CounttotalRecords = res.Where(o => o.StatusCode != EMPConstants.NewCustomer && o.ParentId == null).Count();
                    omodel.recordsTotal = CounttotalRecords;
                    var lst = GetCustomerList(res.OrderBy(a => a.CompanyName).ToList());
                    omodel.Customerlst = lst.Skip(omodel.start).Take(omodel.length).ToList();
                }

                return omodel;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetSearchCustomerInformationNewGrid", omodel.UserId);
                return omodel;
            }
        }

        public List<OfficeManagementDTO2> GetChildInfoSearch2(List<OfficeManagementGridSearch_Result> data, List<OfficeManagementGridSearch_Result> data2, string ParentUserId,
            List<EntityMaster> enttities, List<MasterActions> masteractions, List<SubSiteOfficeConfig> subconfigs)
        {
            try
            {

                //List<TestModel1> data = new List<TestModel1>();
                //data = (from e in db.OfficeManagements
                //        orderby e.CompanyName
                //        where e.ParentId == id && e.EMPUserId != null
                //         && e.StatusCode != EMPConstants.NewCustomer
                //        select new TestModel1 { e = e }).ToList();
                List<OfficeManagementDTO2> customerlst = new List<OfficeManagementDTO2>();
                if (data != null)
                {
                    foreach (var itm in data)
                    {
                        OfficeManagementDTO2 ocustomer = new OfficeManagementDTO2();
                        ocustomer.Id = itm.CustomerId;
                        ocustomer.CustomerId = itm.CustomerId;
                        ocustomer.ParentId = itm.ParentId ?? Guid.Empty;
                        ocustomer.str_ParentId = ParentUserId;
                        ocustomer.CompanyName = itm.CompanyName;
                        ocustomer.BusinessOwnerFirstName = itm.BusinessOwnerFirstName;
                        ocustomer.OfficePhone = itm.OfficePhone;
                        ocustomer.BusinessOwnerFirstName = itm.BusinessOwnerFirstName;
                        ocustomer.BusinessOwnerLastName = itm.BusinessOwnerLastName;
                        ocustomer.EntityId = itm.EntityId ?? 0;
                        ocustomer.BaseEntityId = itm.BaseEntityId ?? 0;

                        ocustomer.EMPUserId = itm.EMPUserId;
                        ocustomer.EROType = enttities.Where(o => o.Id == ocustomer.EntityId).Select(o => o.Name).FirstOrDefault() ?? "";
                        ocustomer.EMPPassword = !string.IsNullOrEmpty(itm.EMPPassword) ? PasswordManager.DecryptText(itm.EMPPassword) : "";

                        ocustomer.EFIN = itm.EFIN;
                        ocustomer.EFINStatus = itm.EFINStatus;

                        int obstatus = string.IsNullOrEmpty(itm.OnboardingStatus) ? 0 : Convert.ToInt32(itm.OnboardingStatus);
                        if (obstatus != 0)
                        {
                            ocustomer.OnboardingStatus = ((EMPConstants.OnboardStatus)obstatus).ToString().Replace("_", " ");
                            if (obstatus == (int)EMPConstants.OnboardStatus.Started_But_Incomplete)
                            {
                                ocustomer.OnboardStatusTooltip = itm.OnboardStatusTooltip;
                            }
                        }
                        else
                            ocustomer.OnboardingStatus = "";

                        int EFIN = ocustomer.EFIN ?? 0;
                        string EFINText = EFIN.ToString().PadLeft(6, '0');

                        if (ocustomer.EFINStatus == 16 || ocustomer.EFINStatus == 19)
                        {
                            ocustomer.EFINStatusText = EFINText;
                        }
                        else if (ocustomer.EFINStatus == 21)
                        {
                            ocustomer.EFINStatusText = (EFIN > 0) ? EFINText + "<u><b>S</b></u>" : "Sharing";
                        }
                        else if (ocustomer.EFINStatus == 17 || ocustomer.EFINStatus == 20)
                        {
                            ocustomer.EFINStatusText = "Applied";
                        }
                        else if (ocustomer.EFINStatus == 18)
                        {
                            ocustomer.EFINStatusText = "Not Required";
                        }
                        else
                        {
                            ocustomer.EFINStatusText = EFINText;
                        }

                        ocustomer.MasterIdentifier = itm.MasterIdentifier;
                        ocustomer.IsActivationCompleted = itm.IsActivationCompleted ?? 0;

                        if (itm.IsActivationCompleted == null || itm.IsActivationCompleted == 0)
                            ocustomer.AccountStatus = itm.AccountStatus == null ? "Not Active" : itm.AccountStatus;
                        else
                            ocustomer.AccountStatus = (itm.IsHold ?? false) ? "Hold" : (itm.AccountStatus == "Review" ? "Review" : "Active");

                        ocustomer.SalesYearId = itm.SalesYearId ?? Guid.Empty;

                        //var taxreturn = db.MainOfficeConfigurations.Where(x => x.emp_CustomerInformation_ID == itm.CustomerId).Select(x => x).FirstOrDefault();
                        //ocustomer.IsTaxReturn = taxreturn == null ? true : taxreturn.IsSiteTransmitTaxReturns;
                        if (ocustomer.EntityId == (int)EMPConstants.Entity.SO || ocustomer.EntityId == (int)EMPConstants.Entity.SOME || ocustomer.EntityId == (int)EMPConstants.Entity.SOME_SS)
                        {
                            ocustomer.IsTaxReturn = true;// itm.IsTaxReturn;
                        }
                        else
                        {
                            ocustomer.IsTaxReturn = itm.IsTaxReturn;
                        }

                        ocustomer.StatusCode = itm.StatusCode;

                        //ocustomer.CreatedDate = itm.cli.CreatedDate ?? Convert.ToDateTime("01/01/2000");
                        //ocustomer.LastUpdatedDate = itm.LastUpdatedDate ?? Convert.ToDateTime("01/01/2000");

                        ocustomer.IsEnrollmentCompleted = itm.IsEnrollmentCompleted ?? false;
                        ocustomer.ActiveBankId = itm.ActiveBankId;
                        ocustomer.ActiveBankName = itm.ActiveBankName;

                        ocustomer.IsActivationCompleted = itm.IsActivationCompleted ?? 0;

                        //var SubOfficeConfig = db.SubSiteOfficeConfigs.Where(o => o.RefId == ocustomer.CustomerId).FirstOrDefault();
                        //if (SubOfficeConfig != null)
                        //{
                        //    if (SubOfficeConfig.SOorSSorEFIN == 3)
                        //    {
                        //        ocustomer.IsAdditionalEFINAllowed = true;
                        //    }
                        //}

                        decimal uTaxSVBFee = itm.uTaxSVBFee ?? 0;
                        decimal SVBAddonFee = itm.SVBAddonFee ?? 0;
                        decimal SVBEnrollAddonFee = itm.SVBEnrollAddonFee ?? 0;

                        ocustomer.IsEnrollmentCompleted = itm.IsEnrollmentCompleted ?? false;
                        if (ocustomer.IsEnrollmentCompleted == true)
                        {
                            ocustomer.TRANCanEnroll = itm.TRANCanEnroll ?? false;
                            ocustomer.TRANCanAddon = itm.TRANCanAddon ?? false;
                            ocustomer.SVBCanAddon = itm.SVBCanAddon ?? false;
                            ocustomer.SVBCanEnroll = itm.SVBCanEnroll ?? false;
                            if (ocustomer.TRANCanEnroll.Value || ocustomer.TRANCanAddon.Value || ocustomer.SVBCanAddon.Value || ocustomer.SVBCanEnroll.Value)
                            {
                                ocustomer.TotalServiceFee = Convert.ToString(uTaxSVBFee + SVBAddonFee + SVBEnrollAddonFee);
                                ocustomer.ServiceTooltip = ocustomer.ServiceTooltip + "<span>uTax Service Beaure Fee : " + uTaxSVBFee.ToString() + "</span><br/>";
                                ocustomer.ServiceTooltip = ocustomer.ServiceTooltip + "<span>" + itm.CompanyName + " : " + (ocustomer.SVBCanAddon == true ? SVBAddonFee.ToString() : "No Add on") + "</span><br/>";
                                ocustomer.ServiceTooltip = ocustomer.ServiceTooltip + "<span>Enrollment Add-on : " + (ocustomer.SVBCanEnroll == true ? SVBEnrollAddonFee.ToString() : "Not Allowed") + "</span><br/>";


                                decimal CrosslinkTransFee = itm.CrosslinkTransFee ?? 0;
                                decimal TransAddonFee = itm.TransAddonFee ?? 0;
                                decimal TransEnrollAddonFee = itm.TransEnrollAddonFee ?? 0;

                                ocustomer.TotalTransFee = Convert.ToString(CrosslinkTransFee + TransAddonFee + TransEnrollAddonFee);
                                ocustomer.TransTooltip = ocustomer.TransTooltip + "<span>Cross Link Transmitter Fee : " + CrosslinkTransFee.ToString() + "</span><br/>";

                                ocustomer.TransTooltip = ocustomer.TransTooltip + "<span>" + itm.CompanyName + " : " + (ocustomer.TRANCanAddon == true ? TransAddonFee.ToString() : "No Add on") + "</span><br/>";
                                ocustomer.TransTooltip = ocustomer.TransTooltip + "<span>Enrollment Add-on : " + (ocustomer.TRANCanEnroll == true ? itm.TransEnrollAddonFee == null ? "No" : TransEnrollAddonFee.ToString() : "Not Allowed") + "</span><br/>";
                            }
                            else
                            {
                                ocustomer.TotalServiceFee = "";
                                ocustomer.ServiceTooltip = "";
                                ocustomer.TotalTransFee = "";
                                ocustomer.TransTooltip = "";
                            }
                        }
                        else
                        {
                            ocustomer.TotalServiceFee = "";
                            ocustomer.ServiceTooltip = "";
                            ocustomer.TotalTransFee = "";
                            ocustomer.TransTooltip = "";
                        }

                        int IsArchived = itm.IsArchived ?? 0;
                        #region "Actions"
                        var actions = (from s in masteractions
                                       where s.EntityId == itm.EntityId
                                       orderby s.Display
                                       select new Actions
                                       {
                                           IsParent = s.IsParent,
                                           Name = s.Name,
                                           Display = s.Display
                                       }).ToList();

                        if (IsArchived == 0)
                        {
                            actions = actions.Where(o => !o.Name.Contains("View Archive")).ToList();
                        }

                        ocustomer.Actions = actions;


                        ocustomer.Actions = actions;

                        #endregion

                        ocustomer.ChildCustomerInfo = new List<OfficeManagementDTO2>();
                        var data3 = data2.Where(o => o.ParentId == ocustomer.CustomerId).ToList();
                        ocustomer.ChildCustomerInfo = GetChildInfoSearch2(data3, data2, itm.EMPUserId, enttities, masteractions, subconfigs);
                        ocustomer.CanEnrollmentAllowed = itm.CanEnrollmentAllowed ?? true;
                        ocustomer.CanEnrollmentAllowedForMain = itm.CanEnrollmentAllowedForMain ?? false;

                        ocustomer.SubmissionDate = itm.EnrollmentSubmittionDate == null ? "" : itm.EnrollmentSubmittionDate.Value.ToString("MM'/'dd'/'yyyy HH:mm");
                        ocustomer.EnrollmentStatus = itm.EnrollmentStatus;
                        if (ocustomer.EnrollmentStatus == "INP")
                            ocustomer.EnrollmentStatus = "Incomplete";
                        else
                            ocustomer.EnrollmentStatus = ocustomer.EnrollmentStatus == EMPConstants.Ready ? "Unsuccessful" : ((ocustomer.EnrollmentStatus == EMPConstants.Submitted) ? "Submitted" :
                                                        (ocustomer.EnrollmentStatus == EMPConstants.Approved ? "Approved" : (ocustomer.EnrollmentStatus == EMPConstants.Rejected ? "Rejected" :
                                                        (ocustomer.EnrollmentStatus == EMPConstants.Denied ? "Denied" : (ocustomer.EnrollmentStatus == EMPConstants.Cancelled ? "Cancelled" :
                                                        (ocustomer.EnrollmentStatus == EMPConstants.EnrPending ? "Pending" : "Not Started"))))));

                        ocustomer.ApprovedBank = itm.ApprovedBank;// + (ocustomer.EnrollmentStatus == "Approved" && !string.IsNullOrEmpty(itm.ApprovedBank) ? " (Previous)" : "");
                        ocustomer.RejectedBanks = itm.RejectedBanks;
                        ocustomer.UnlockedBanks = itm.UnlockedBanks;
                        ocustomer.SubmittedBanks = itm.SubmittedBanks;

                        customerlst.Add(ocustomer);
                    }

                }
                return customerlst;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetChildCustomerInfo", Guid.Empty);
                return new List<OfficeManagementDTO2>();
            }
        }

        public List<Guid> GetCustomersFilter(CustomerInfoNewGrid omodel, List<OfficeManagementGridSearch_Result> data)
        {
            List<Guid> lstCust = new List<Guid>();
            try
            {
                var parents = data.Where(x => x.ParentId == null).ToList();
                foreach (var itm in parents)
                {
                    var childs = GetChildCustomersFilter(omodel, data, itm.CustomerId.Value);
                    if (childs.Count > 0)
                        lstCust.AddRange(childs);
                    lstCust.Add(itm.CustomerId.Value);
                }
            }
            catch (Exception)
            {
            }
            return lstCust;
        }

        public List<Guid> GetChildCustomersFilter(CustomerInfoNewGrid omodel, List<OfficeManagementGridSearch_Result> data, Guid ParentId)
        {
            List<Guid> lstchilds = new List<Guid>();
            try
            {
                var childs = data.Where(x => x.ParentId == ParentId).ToList();
                foreach (var item in childs)
                {
                    if (!getFilterValiation(omodel, item))
                    {
                        var subchilds = data.Where(x => x.ParentId == item.CustomerId).ToList();
                        if (subchilds.Count <= 0)
                            continue;
                        else
                        {
                            foreach (var sc in subchilds)
                            {
                                var childrens = GetChildCustomersFilter(omodel, data, sc.CustomerId.Value);
                                if (childrens.Count > 0)
                                {
                                    lstchilds.AddRange(childrens);
                                    lstchilds.Add(item.CustomerId.Value);
                                }
                            }
                        }
                    }
                    else
                    {
                        lstchilds.Add(item.CustomerId.Value);
                        lstchilds.Add(item.ParentId.Value);
                    }
                }
            }
            catch (Exception)
            {

            }
            return lstchilds.Distinct().ToList();
        }

        public bool getFilterValiation(CustomerInfoNewGrid omodel, OfficeManagementGridSearch_Result item)
        {
            bool _status = false, _sitetype = false, _bank = false, _bankstatus = false;
            if (!string.IsNullOrEmpty(omodel.Status))
            {
                List<string> statuss = omodel.Status.Split(',').ToList();
                var isstatus = statuss.Where(x => x != "ACT" && x != "INA").Contains(item.StatusCode);
                if (statuss.Contains("ACT") && !isstatus)
                {
                    isstatus = item.IsActivationCompleted == 1;
                }
                if (!isstatus && statuss.Contains("INA"))
                {
                    isstatus = item.IsActivationCompleted == 0 || item.IsActivationCompleted == null;
                }
                _status = isstatus;
            }
            else
                _status = true;

            if (!string.IsNullOrEmpty(omodel.SiteType))
            {
                List<int> statuss = omodel.SiteType.Split(',').Select(x => Convert.ToInt32(x)).ToList();
                _sitetype = statuss.Where(x => x == item.EntityId).Count() > 0;
            }
            else
                _sitetype = true;

            if (!string.IsNullOrEmpty(omodel.BankPartner))
            {
                List<string> statuss = omodel.BankPartner.Split(',').ToList();
                _bank = statuss.Contains(item.ActiveBankName);
            }
            else
                _bank = true;

            if (!string.IsNullOrEmpty(omodel.EnrollmentStatus))
            {
                List<string> statuss = omodel.EnrollmentStatus.Split(',').ToList();
                _bankstatus = statuss.Contains(item.EnrollmentStatus);
            }
            else
                _bankstatus = true;

            return _bankstatus && _bank && _sitetype && _status;
        }

        public IQueryable<CustomerRecenltyModel> GetRecentlyCreatedDetails(Guid UserID, int Mainentity)
        {
            try
            {
                List<CustomerRecenltyModel> _newrecords = new List<CustomerRecenltyModel>();

                if (Mainentity == (int)EMPConstants.Entity.uTax)
                {
                    var data = (from s in db.OfficeManagements
                                where s.StatusCode != EMPConstants.NewCustomer
                                orderby s.CreatedDate descending
                                select new CustomerRecenltyModel
                                {
                                    Name = s.CompanyName,
                                    updateDatetime = s.CreatedDate ?? System.DateTime.Now,
                                    EntityDisplayId = s.BaseEntityId ?? 0,
                                    EntityId = s.EntityId ?? 0,
                                    Id = s.CustomerId.Value,
                                    Ptype = (s.EntityId.Value == (int)EMPConstants.Entity.MO || s.EntityId.Value == (int)EMPConstants.Entity.SVB) ? "config" : "subconfig"
                                }).Take(5);
                    foreach (var item in data)
                    {
                        var rec = item;
                        rec.ParentId = getTopParentId(item.Id);
                        rec.Date = item.updateDatetime.ToString("MM'/'dd'/'yyyy hh:mm tt");
                        _newrecords.Add(rec);
                    }
                    return _newrecords.AsQueryable();
                }
                else
                {
                    List<Guid> childrens = new List<Guid>();
                    new DropDownService().getChildrenIds(UserID, ref childrens);

                    var data = (from s in db.OfficeManagements
                                where s.StatusCode != EMPConstants.NewCustomer && childrens.Contains(s.CustomerId.Value)
                                orderby s.CreatedDate descending
                                select new CustomerRecenltyModel
                                {
                                    Name = s.CompanyName,
                                    updateDatetime = s.CreatedDate ?? System.DateTime.Now,
                                    EntityDisplayId = s.BaseEntityId ?? 0,
                                    EntityId = s.EntityId ?? 0,
                                    Id = s.CustomerId.Value,
                                    Ptype = (s.EntityId.Value == (int)EMPConstants.Entity.MO || s.EntityId.Value == (int)EMPConstants.Entity.SVB) ? "config" : "subconfig"
                                }).Take(5);
                    foreach (var item in data)
                    {
                        var rec = item;
                        rec.ParentId = getTopParentId(item.Id);
                        rec.Date = item.updateDatetime.ToString("MM'/'dd'/'yyyy hh:mm tt");
                        _newrecords.Add(rec);
                    }
                    return _newrecords.AsQueryable();
                }

                //var data = (from e in db.OfficeManagements
                //                // join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
                //            where //cli.CrossLinkUserId != null && 
                //            e.StatusCode != EMPConstants.NewCustomer
                //            select new { e }).ToList();
                //var data2 = data;
                //if (data.Count > 0)
                //{
                //    if (data.Any(a => a.e.CustomerId == UserID))
                //    {
                //        data = data.Where(a => a.e.CustomerId == UserID).ToList();
                //    }
                //}

                //if (data.ToList().Count == 1)
                //{
                //    var data_recCreate = data2.Where(a => a.e.ParentId == data[0].e.CustomerId).OrderByDescending(a => a.e.CreatedDate).Select(o => new CustomerRecenltyModel
                //    {
                //        Name = o.e.CompanyName,
                //        updateDatetime = o.e.CreatedDate ?? System.DateTime.Now
                //    }).Take(3).DefaultIfEmpty();
                //    return data_recCreate.AsQueryable();
                //}
                //else
                //{
                //    var data_recCreate = data.OrderByDescending(a => a.e.CreatedDate).Select(o => new CustomerRecenltyModel
                //    {
                //        Name = o.e.CompanyName,
                //        updateDatetime = o.e.CreatedDate ?? System.DateTime.Now
                //    }).Take(3).DefaultIfEmpty();
                //    return data_recCreate.AsQueryable();

                //}
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetRecentlyCreatedDetails", UserID);
                return new List<CustomerRecenltyModel>().AsQueryable();
            }
        }

        public IQueryable<CustomerRecenltyModel> GetRecentlyUpdateDetails(Guid UserID, int Mainentity)
        {
            try
            {
                List<CustomerRecenltyModel> _newrecords = new List<CustomerRecenltyModel>();

                if (Mainentity == (int)EMPConstants.Entity.uTax)
                {
                    var data = (from s in db.OfficeManagements
                                where s.StatusCode != EMPConstants.NewCustomer
                                orderby s.UpdatedDate descending
                                select new CustomerRecenltyModel
                                {
                                    Name = s.CompanyName,
                                    updateDatetime = s.UpdatedDate ?? System.DateTime.Now,
                                    EntityDisplayId = s.BaseEntityId ?? 0,
                                    EntityId = s.EntityId ?? 0,
                                    Id = s.CustomerId.Value,
                                    Ptype = (s.EntityId.Value == (int)EMPConstants.Entity.MO || s.EntityId.Value == (int)EMPConstants.Entity.SVB) ? "config" : "subconfig"
                                }).Take(5);
                    foreach (var item in data)
                    {
                        var rec = item;
                        rec.ParentId = getTopParentId(item.Id);
                        rec.Date = item.updateDatetime.ToString("MM'/'dd'/'yyyy hh:mm tt");
                        _newrecords.Add(rec);
                    }
                    return _newrecords.AsQueryable();
                }
                else
                {
                    List<Guid> childrens = new List<Guid>();
                    new DropDownService().getChildrenIds(UserID, ref childrens);

                    var data = (from s in db.OfficeManagements
                                where s.StatusCode != EMPConstants.NewCustomer && childrens.Contains(s.CustomerId.Value)
                                orderby s.UpdatedDate descending
                                select new CustomerRecenltyModel
                                {
                                    Name = s.CompanyName,
                                    updateDatetime = s.UpdatedDate ?? System.DateTime.Now,
                                    EntityDisplayId = s.BaseEntityId ?? 0,
                                    EntityId = s.EntityId ?? 0,
                                    Id = s.CustomerId.Value,
                                    Ptype = (s.EntityId.Value == (int)EMPConstants.Entity.MO || s.EntityId.Value == (int)EMPConstants.Entity.SVB) ? "config" : "subconfig"
                                }).Take(5);
                    foreach (var item in data)
                    {
                        var rec = item;
                        rec.ParentId = getTopParentId(item.Id);
                        rec.Date = item.updateDatetime.ToString("MM'/'dd'/'yyyy hh:mm tt");
                        _newrecords.Add(rec);
                    }
                    return _newrecords.AsQueryable();
                }

                //var data = (from e in db.OfficeManagements
                //                //join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
                //            where //cli.CrossLinkUserId != null && 
                //            e.StatusCode != EMPConstants.NewCustomer
                //            select new { e }).ToList();
                //var data2 = data;
                //if (data.Count > 0)
                //{
                //    if (data.Any(a => a.e.CustomerId == UserID))
                //    {
                //        data = data.Where(a => a.e.CustomerId == UserID).ToList();
                //    }
                //}
                //if (data.ToList().Count == 1)
                //{
                //    var data_recCreate = data2.Where(a => a.e.ParentId == data[0].e.CustomerId).OrderByDescending(a => a.e.UpdatedDate).Select(o => new CustomerRecenltyModel
                //    {
                //        Name = o.e.CompanyName,
                //        updateDatetime = o.e.UpdatedDate ?? System.DateTime.Now
                //    }).Take(3).DefaultIfEmpty();
                //    return data_recCreate.AsQueryable();
                //}
                //else
                //{
                //    var data_recCreate = data.OrderByDescending(a => a.e.UpdatedDate).Select(o => new CustomerRecenltyModel
                //    {
                //        Name = o.e.CompanyName,
                //        updateDatetime = o.e.UpdatedDate ?? System.DateTime.Now
                //    }).Take(3).DefaultIfEmpty();
                //    return data_recCreate.AsQueryable();
                //}
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetRecentlyUpdateDetails", UserID);
                return new List<CustomerRecenltyModel>().AsQueryable();
            }
        }

        public Guid getTopParentId(Guid Id)
        {
            DropDownService ddService = new DropDownService();
            List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
            EntityHierarchyDTOs = ddService.GetEntityHierarchies(Id);

            Guid RootParentId = Guid.Empty;
            int Level = EntityHierarchyDTOs.Count;
            if (EntityHierarchyDTOs.Count > 0)
            {
                var LevelOne = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                if (LevelOne != null)
                {
                    RootParentId = LevelOne.CustomerId ?? Guid.Empty;
                }
            }
            if (RootParentId == Id)
                return Guid.Empty;
            return RootParentId;
        }
    }

    #region Old Search
    public class EMPCustomerLogin
    {
        public string EMPUserId { get; set; }
        public string EMPPassword { get; set; }
    }

    public class CustModel
    {
        public List<TestModel1> data { get; set; }
    }

    public class TestModel1
    {
        public OfficeManagementGridSearch_Result e { get; set; }
        //public emp_CustomerLoginInformation cli { get; set; }
    }
    #endregion
}