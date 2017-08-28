using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using EMP.Core.Utilities;
using EMPPortal.Transactions.CustomerInformation.DTO;
using EMPPortal.Transactions.EnrollmentBankSelectionInfo;
using MoreLinq;
using EMPPortal.Core.Utilities;
using EMPPortal.Transactions.CrosslinkService;
using EMPPortal.Transactions.DropDowns;
using EMPPortal.Transactions.DropDowns.DTO;
using EMPPortal.Transactions.OfficeManagementTransactions;

namespace EMPPortal.Transactions.CustomerInformation
{
    public class CustomerInformationService
    {
        private DatabaseEntities db = new DatabaseEntities();
        CrosslinkWS17SoapClient _apiObj = new CrosslinkWS17SoapClient();

        // GET: api/emp_CustomerInformation
        public async Task<List<CustomerInformationModel>> GetAllCustomerInformation()
        {
            try
            {

                //Guid entityid;
                //  bool IsExist = Guid.TryParse("8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73", out entityid);

                int EntityId = (int)EMPConstants.Entity.uTax;


                var emp = await db.emp_CustomerInformation.Where(o => o.StatusCode == EMPConstants.NewCustomer && o.EntityId != EntityId).Select(e => new CustomerInformationModel // || (o.StatusCode == EMPConstants.InProgress && o.IsActivationCompleted == 1)
                {
                    Id = e.Id,
                    CompanyName = e.CompanyName != null ? e.CompanyName.ToString() : "",
                    AccountStatus = e.AccountStatus != null ? e.AccountStatus.ToString() : "",
                    Feeder = e.Feeder,
                    BusinessOwnerFirstName = e.BusinessOwnerFirstName != null ? e.BusinessOwnerFirstName.ToString() : "",
                    BusinessOwnerLastName = e.BusinesOwnerLastName != null ? e.BusinesOwnerLastName.ToString() : "",
                    OfficePhone = e.OfficePhone != null ? e.OfficePhone.ToString() : "",
                    AlternatePhone = e.AlternatePhone != null ? e.AlternatePhone.ToString() : "",
                    Primaryemail = e.PrimaryEmail != null ? e.PrimaryEmail.ToString() : "",
                    SupportNotificationemail = e.SupportNotificationEmail != null ? e.SupportNotificationEmail.ToString() : "",
                    EROType = e.EROType != null ? e.EROType.ToString() : "",
                    AlternativeContact = e.AlternativeContact != null ? e.AlternativeContact.ToString() : "",
                    //11212016
                    EFIN = e.EFIN,
                    EFINStatus = e.EFINStatus,

                    PhysicalAddress1 = e.PhysicalAddress1 != null ? e.PhysicalAddress1.ToString() : "",
                    PhysicalAddress2 = e.PhysicalAddress2 != null ? e.PhysicalAddress2.ToString() : "",
                    PhysicalZipcode = e.PhysicalZipCode != null ? e.PhysicalZipCode.ToString() : "",
                    PhysicalCity = e.PhysicalCity != null ? e.PhysicalCity.ToString() : "",
                    PhysicalState = e.PhysicalState != null ? e.PhysicalState.ToString() : "",
                    ShippingAddress1 = e.ShippingAddress1 != null ? e.ShippingAddress1.ToString() : "",
                    ShippingAddress2 = e.ShippingAddress2 != null ? e.ShippingAddress2.ToString() : "",
                    ShippingZipcode = e.ShippingZipCode != null ? e.ShippingZipCode.ToString() : "",
                    ShippingCity = e.ShippingCity != null ? e.ShippingCity.ToString() : "",
                    ShippingState = e.ShippingState != null ? e.ShippingState.ToString() : "",
                    PhoneTypeId = e.PhoneTypeId,
                    ParentId = e.ParentId,
                    TitleId = e.TitleId,
                    AlternativeType = e.AlternativeType,
                    PhoneType = db.PhoneTypeMasters.Where(a => a.Id == e.PhoneTypeId).Select(a => a.PhoneType != null ? a.PhoneType.ToString() : "").FirstOrDefault(),
                    ContactTitle = db.ContactPersonTitleMasters.Where(a => a.Id == e.TitleId).Select(a => a.ContactPersonTitle != null ? a.ContactPersonTitle.ToString() : "").FirstOrDefault(),
                    EntityId = e.EntityId ?? 0,
                    SalesYearID = e.SalesYearID ?? Guid.Empty,
                    SalesforceParentID = e.SalesforceParentID != null ? e.SalesforceParentID.ToString() : "",
                    MasterIdentifier = e.MasterIdentifier != null ? e.MasterIdentifier.ToString() : "",
                    IsVerified = e.IsVerified ?? false,
                    IsMSOUser = e.IsMSOUser ?? false,
                    IsActivationCompleted = e.IsActivationCompleted ?? 0,
                    StatusCode = e.StatusCode.ToString(),

                }).Distinct().ToListAsync();

                return emp;

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetAllCustomerInformation", Guid.Empty);
                return null;
            }

        }


        public IQueryable<NewCustomersModel> GetNewCustomers()
        {
            try
            {

                //Guid entityid;
                //  bool IsExist = Guid.TryParse("8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73", out entityid);

                int EntityId = (int)EMPConstants.Entity.uTax;


                var emp = db.emp_CustomerInformation.Where(o => o.StatusCode == EMPConstants.NewCustomer && o.EntityId != EntityId).Select(e => new NewCustomersModel // || (o.StatusCode == EMPConstants.InProgress && o.IsActivationCompleted == 1)
                {
                    Id = e.Id,
                    CompanyName = e.CompanyName != null ? e.CompanyName.ToString() : "",
                    BusinessOwnerFirstName = e.BusinessOwnerFirstName != null ? e.BusinessOwnerFirstName.ToString() : "",
                    OfficePhone = e.OfficePhone != null ? e.OfficePhone.ToString() : "",
                    Primaryemail = e.PrimaryEmail != null ? e.PrimaryEmail.ToString() : "",
                    EROType = e.EROType != null ? e.EROType.ToString() : "",
                    StatusCode = e.StatusCode.ToString(),
                    EntityId = e.EntityId ?? 0
                }).Distinct();

                var reviewemp = (from s in db.emp_CustomerInformation
                                 join l in db.emp_CustomerLoginInformation on s.Id equals l.CustomerOfficeId
                                 where s.IsActivationCompleted == 1 && s.StatusCode == EMPConstants.Created && (l.EMPPassword == null || l.EMPPassword == "") && l.EMPUserId != ""
                                 select new NewCustomersModel
                                 {
                                     Id = s.Id,
                                     CompanyName = s.CompanyName != null ? s.CompanyName.ToString() : "",
                                     BusinessOwnerFirstName = s.BusinessOwnerFirstName != null ? s.BusinessOwnerFirstName.ToString() : "",
                                     OfficePhone = s.OfficePhone != null ? s.OfficePhone.ToString() : "",
                                     Primaryemail = s.PrimaryEmail != null ? s.PrimaryEmail.ToString() : "",
                                     EROType = s.EROType != null ? s.EROType.ToString() : "",
                                     StatusCode = s.StatusCode.ToString(),
                                     EntityId = s.EntityId ?? 0
                                 }).Distinct().ToList();

                var total = emp.ToList();
                total.AddRange(reviewemp);
                return total.AsQueryable();

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetNewCustomers", Guid.Empty);
                return null;
            }

        }

        //public IQueryable<CustomerModel> CustomerSearchInfoLazyLoad(Guid UserName, string Status, string SiteType, string BankPartner, string EnrollmentStatus, string OnBoardingStatus, string SearchText, int SearchType, int count)
        //{
        //    try
        //    {

        //        List<string> Statuslst = !string.IsNullOrEmpty(Status) ? Status.Split(',').ToList() : null;
        //        List<string> SiteTypelst = !string.IsNullOrEmpty(SiteType) ? SiteType.Split(',').ToList() : null;
        //        List<string> BankPartnerlst = !string.IsNullOrEmpty(BankPartner) ? BankPartner.Split(',').ToList() : null;
        //        List<string> EnrollmentStatuslst = !string.IsNullOrEmpty(EnrollmentStatus) ? EnrollmentStatus.Split(',').ToList() : null;
        //        List<string> OnBoardingStatuslst = !string.IsNullOrEmpty(OnBoardingStatus) ? OnBoardingStatus.Split(',').ToList() : null;

        //        var data1 = (from e in db.emp_CustomerInformation
        //                     join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                     where e.ParentId == null && cli.CrossLinkUserId != null //&& e.AlternativeContact != null && e.AlternatePhone != null
        //                     && e.StatusCode != EMPConstants.NewCustomer
        //                     select new { e, cli }).ToList();

        //        var data = data1.Skip(count).Take(15).ToList();


        //        if (data.Count > 0)
        //        {
        //            if (data.Any(a => a.cli.CustomerOfficeId == UserName))
        //            {
        //                data = data.Where(a => a.cli.CustomerOfficeId == UserName).ToList();
        //            }
        //        }
        //        List<CustomerModel> customerlst = new List<CustomerModel>();
        //        foreach (var itm in data)
        //        {
        //            CustomerModel ocustomer = new CustomerModel();
        //            ocustomer.Id = itm.e.Id;
        //            ocustomer.ParentId = itm.e.ParentId ?? Guid.Empty;
        //            ocustomer.CompanyName = itm.e.CompanyName;
        //            ocustomer.AccountStatus = itm.e.AccountStatus;
        //            ocustomer.SalesforceParentID = itm.e.SalesforceParentID;
        //            ocustomer.MasterIdentifier = itm.e.MasterIdentifier;
        //            ocustomer.Feeder = itm.e.Feeder;
        //            ocustomer.BusinessOwnerFirstName = itm.e.BusinessOwnerFirstName;
        //            ocustomer.OfficePhone = itm.e.OfficePhone;
        //            ocustomer.AlternatePhone = itm.e.AlternatePhone;
        //            ocustomer.Primaryemail = itm.e.PrimaryEmail;
        //            ocustomer.SupportNotificationemail = itm.e.SupportNotificationEmail;
        //            ocustomer.EROType = itm.e.EROType;
        //            ocustomer.AlternativeContact = itm.e.AlternativeContact ?? "";
        //            ocustomer.EFIN = itm.e.EFIN;
        //            ocustomer.EFINStatus = itm.e.EFINStatus;
        //            ocustomer.PhysicalAddress1 = itm.e.PhysicalAddress1;
        //            ocustomer.PhysicalAddress2 = itm.e.PhysicalAddress2;
        //            ocustomer.PhysicalZipcode = itm.e.PhysicalZipCode;
        //            ocustomer.PhysicalCity = itm.e.PhysicalCity;
        //            ocustomer.PhysicalState = itm.e.PhysicalState;
        //            ocustomer.ShippingAddress1 = itm.e.ShippingAddress1;
        //            ocustomer.ShippingAddress2 = itm.e.ShippingAddress2;
        //            ocustomer.ShippingZipcode = itm.e.ShippingZipCode;
        //            ocustomer.ShippingCity = itm.e.ShippingCity;
        //            ocustomer.ShippingState = itm.e.ShippingState;
        //            ocustomer.PhoneTypeId = itm.e.PhoneTypeId;
        //            ocustomer.TitleId = itm.e.TitleId;
        //            ocustomer.AlternativeType = itm.e.AlternativeType;
        //            ocustomer.PhoneType = db.PhoneTypeMasters.Where(a => a.Id == itm.e.PhoneTypeId).Select(a => a.PhoneType).FirstOrDefault();
        //            ocustomer.ContactTitle = db.ContactPersonTitleMasters.Where(a => a.Id == itm.e.TitleId).Select(a => a.ContactPersonTitle).FirstOrDefault() ?? "";
        //            ocustomer.EntityId = itm.e.EntityId ?? 0;
        //            ocustomer.SalesYearID = itm.e.SalesYearID ?? Guid.Empty;
        //            ocustomer.BaseEntityId = db.EntityMasters.Where(a => a.Id == itm.e.EntityId).Select(a => a.BaseEntityId).FirstOrDefault();
        //            ocustomer.LoginId = itm.cli.Id.ToString();
        //            //ocustomer.LoginEFIN = itm.e.EFIN;
        //            ocustomer.MasterIdentifier = itm.cli.MasterIdentifier;
        //            ocustomer.CrossLinkUserId = itm.cli.CrossLinkUserId ?? "";
        //            ocustomer.CrossLinkPassword = itm.cli.CrossLinkPassword;
        //            ocustomer.OfficePortalUrl = itm.cli.OfficePortalUrl;
        //            ocustomer.TaxOfficeUsername = itm.cli.TaxOfficeUsername;
        //            ocustomer.TaxOfficePassword = itm.cli.TaxOfficePassword;
        //            ocustomer.CustomerOfficeId = itm.cli.CustomerOfficeId;
        //            ocustomer.MasterIdentifier = itm.cli.MasterIdentifier;
        //            ocustomer.IsActivationCompleted = itm.e.IsActivationCompleted ?? 0;

        //            ocustomer.SalesforceParentID = itm.e.SalesforceParentID;
        //            if (itm.e.IsActivationCompleted == null || itm.e.IsActivationCompleted == 0)
        //                ocustomer.ActivationStatus = "Not Active";
        //            else
        //                ocustomer.ActivationStatus = "Active";

        //            ocustomer.CreatedDate = itm.cli.CreatedDate ?? Convert.ToDateTime("01/01/2000");
        //            ocustomer.LastUpdatedDate = itm.e.LastUpdatedDate ?? Convert.ToDateTime("01/01/2000");

        //            EnrollmentBankSelectionService serv = new EnrollmentBankSelectionInfo.EnrollmentBankSelectionService();
        //            var enrdata = serv.getEnrollmentStatusInfo(ocustomer.Id);
        //            ocustomer.ActiveBank = enrdata.BankName;
        //            ocustomer.SubmissionDate = enrdata.SubmitedDate;
        //            ocustomer.EnrollmentStatus = enrdata.SubmissionStaus;
        //            ocustomer.ApprovedBank = enrdata.ApprovedBank;
        //            ocustomer.RejectedBanks = enrdata.RejectedBanks;

        //            //var CustFees = CustomerFees(itm.e.Id);
        //            //foreach (var fees in CustFees)
        //            //{
        //            //    if (fees.FeeFor == (int)EMPConstants.FeesFor.SVBFees)
        //            //    {
        //            //        ocustomer.TotalServiceFee = fees.Amount;
        //            //        ocustomer.ServiceTooltip = fees.FeesName;
        //            //    }

        //            //    if (fees.FeeFor == (int)EMPConstants.FeesFor.TransmissionFees)
        //            //    {
        //            //        ocustomer.TotalTransFee = fees.Amount;
        //            //        ocustomer.TransTooltip = fees.FeesName;
        //            //    }
        //            //}

        //            //var BankFees = BankFee(itm.e.Id, itm.e.Id, ocustomer.CompanyName, ocustomer.CompanyName, 0);
        //            //foreach (var fees in BankFees)
        //            //{
        //            //    if (fees.FeeFor == (int)EMPConstants.FeesFor.SVBFees)
        //            //    {
        //            //        ocustomer.TotalServiceFee = ocustomer.TotalServiceFee + fees.Amount;
        //            //        ocustomer.ServiceTooltip = ocustomer.ServiceTooltip + fees.FeesName;
        //            //    }

        //            //    if (fees.FeeFor == (int)EMPConstants.FeesFor.TransmissionFees)
        //            //    {
        //            //        ocustomer.TotalTransFee = ocustomer.TotalTransFee + fees.Amount;
        //            //        ocustomer.TransTooltip = ocustomer.TransTooltip + fees.FeesName;
        //            //    }
        //            //}
        //            //if (SearchType > 0)
        //            //{
        //            var ChildDataLst = GetChildCustomerInfo(itm.e.Id, itm.e.CompanyName, Statuslst, SiteTypelst, SearchText, SearchType, UserName, null);
        //            ocustomer.ChaildCustomerInfo = ChildDataLst;
        //            //}
        //            //else
        //            //{
        //            //    var ChildDataLst = GetChildCustomerInfo(itm.e.Id, "", Statuslst., SiteTypelst.DefaultIfEmpty, "", 0, Guid.Empty);
        //            //    ocustomer.ChaildCustomerInfo = ChildDataLst;
        //            //}

        //            if (ocustomer.EROType == "Single Office")
        //                ocustomer.IsActivated = true;
        //            else
        //            {
        //                ocustomer.IsActivated = itm.e.IsActivationCompleted.HasValue ? (itm.e.IsActivationCompleted.Value == 1 ? true : false) : false;
        //            }

        //            customerlst.Add(ocustomer);
        //        }

        //        if (Statuslst != null)
        //        {
        //            customerlst = customerlst.Where(o => Statuslst.Contains(o.StatusCode) || o.ChaildCustomerInfo.Count > 0).ToList();
        //        }
        //        if (SiteTypelst != null)
        //        {
        //            customerlst = customerlst.Where(o => SiteTypelst.Contains(o.EntityId.ToString()) || o.ChaildCustomerInfo.Count > 0).ToList();
        //        }

        //        if (SearchType > 0)
        //        {
        //            if (SearchType == 1)
        //            {
        //                customerlst = customerlst.Where(o => o.CompanyName.ToString().ToLower().Contains(SearchText.ToLower()) || o.ChaildCustomerInfo.Count > 0).ToList();
        //            }
        //            else if (SearchType == 2)
        //            {
        //                customerlst = customerlst.Where(o => o.CrossLinkUserId.ToString().ToLower().Contains(SearchText.ToLower()) || o.ChaildCustomerInfo.Count > 0).ToList();
        //            }
        //            else if (SearchType == 3)
        //            {
        //                customerlst = customerlst.Where(o => o.EFIN.ToString().ToLower().Contains(SearchText.ToLower()) || o.ChaildCustomerInfo.Count > 0).ToList();
        //            }
        //            else if (SearchType == 4)
        //            {
        //                customerlst = customerlst.Where(o => o.AlternativeContact != null ? o.AlternativeContact.ToString().ToLower().Contains(SearchText.ToLower()) : o.AlternativeContact == o.AlternativeContact).ToList();
        //            }
        //            else if (SearchType == 5)
        //            {
        //                customerlst = customerlst.Where(o => o.AlternatePhone != null ? o.AlternatePhone.ToString().ToLower().Contains(SearchText.ToLower()) : o.AlternatePhone == o.AlternatePhone).ToList();
        //            }
        //        }
        //        return customerlst.OrderBy(a => a.CompanyName).AsQueryable();

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/CustomerSearchInfoLazyLoad", UserName);
        //        return new List<CustomerModel>().AsQueryable();
        //    }
        //}

        // GET: api/emp_CustomerInformation/5
        public async Task<CustomerInformationModel> GetCustomerInformation(Guid id)
        {
            try
            {
                DropDownService ddService = new DropDownService();
                List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                EntityHierarchyDTOs = ddService.GetEntityHierarchies(id);

                Guid ParentId = Guid.Empty;
                int Level = EntityHierarchyDTOs.Count;
                if (EntityHierarchyDTOs.Count > 0)
                {
                    var LevelOne = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                    if (LevelOne != null)
                    {
                        ParentId = LevelOne.CustomerId ?? Guid.Empty;
                    }
                }

                var customerInformation = await db.emp_CustomerInformation.Where(e => e.Id == id).Select(e => new CustomerInformationModel
                {
                    Id = e.Id,
                    CompanyName = e.CompanyName != null ? e.CompanyName.ToString() : "",
                    AccountStatus = e.AccountStatus != null ? e.AccountStatus.ToString() : "",
                    Feeder = e.Feeder,
                    BusinessOwnerFirstName = e.BusinessOwnerFirstName != null ? e.BusinessOwnerFirstName.ToString() : "",
                    BusinessOwnerLastName = e.BusinesOwnerLastName != null ? e.BusinesOwnerLastName.ToString() : "",
                    OfficePhone = e.OfficePhone != null ? e.OfficePhone.ToString() : "",
                    AlternatePhone = e.AlternatePhone != null ? e.AlternatePhone.ToString() : "",
                    Primaryemail = e.PrimaryEmail != null ? e.PrimaryEmail.ToString() : "",
                    SupportNotificationemail = e.SupportNotificationEmail != null ? e.SupportNotificationEmail.ToString() : "",
                    EROType = e.EROType != null ? e.EROType.ToString() : "",
                    AlternativeContact = e.AlternativeContact != null ? e.AlternativeContact.ToString() : "",
                    //11212016
                    EFIN = e.EFIN ?? 0,
                    EFINStatus = e.EFINStatus,
                    PhysicalAddress1 = e.PhysicalAddress1 != null ? e.PhysicalAddress1.ToString() : "",
                    PhysicalAddress2 = e.PhysicalAddress2 != null ? e.PhysicalAddress2.ToString() : "",
                    PhysicalZipcode = e.PhysicalZipCode != null ? e.PhysicalZipCode.ToString() : "",
                    PhysicalCity = e.PhysicalCity != null ? e.PhysicalCity.ToString() : "",
                    PhysicalState = e.PhysicalState != null ? e.PhysicalState.ToString() : "",
                    ShippingAddress1 = e.ShippingAddress1 != null ? e.ShippingAddress1.ToString() : "",
                    ShippingAddress2 = e.ShippingAddress2 != null ? e.ShippingAddress2.ToString() : "",
                    ShippingZipcode = e.ShippingZipCode != null ? e.ShippingZipCode.ToString() : "",
                    ShippingCity = e.ShippingCity != null ? e.ShippingCity.ToString() : "",
                    ShippingState = e.ShippingState != null ? e.ShippingState.ToString() : "",
                    PhoneTypeId = e.PhoneTypeId,
                    ParentId = e.ParentId,
                    TitleId = e.TitleId,
                    AlternativeType = e.AlternativeType,
                    PhoneType = db.PhoneTypeMasters.Where(a => a.Id == e.PhoneTypeId).Select(a => a.PhoneType != null ? a.PhoneType.ToString() : "").FirstOrDefault(),
                    ContactTitle = db.ContactPersonTitleMasters.Where(a => a.Id == e.TitleId).Select(a => a.ContactPersonTitle != null ? a.ContactPersonTitle.ToString() : "").FirstOrDefault(),
                    EntityId = e.EntityId ?? 0,
                    SalesYearID = e.SalesYearID ?? Guid.Empty,
                    SalesforceParentID = e.SalesforceParentID != null ? e.SalesforceParentID.ToString() : "",
                    MasterIdentifier = e.MasterIdentifier != null ? e.MasterIdentifier.ToString() : "",
                    IsVerified = e.IsVerified ?? false,
                    IsMSOUser = e.IsMSOUser ?? false,
                    IsActivationCompleted = e.IsActivationCompleted ?? 0,
                    StatusCode = e.StatusCode.ToString(),
                    IsNotCollectingFee = e.uTaxNotCollectingSBFee ?? false,
                    BaseEntityId = e.EntityMaster.BaseEntityId ?? 0,
                    SalesforceOpportunityID = e.SalesforceOpportunityID,
                    IsHold = e.IsHold ?? false,
                    AlternativePhoneType = e.AlternatePhoneTypeId

                }).Distinct().FirstOrDefaultAsync();

                if (customerInformation != null)
                {
                    int EFIN = customerInformation.EFIN ?? 0;
                    string EFINText = EFIN.ToString().PadLeft(6, '0');
                    //customerInformation.EFIN = Convert.ToInt32(EFINText);

                    customerInformation.IsEnrollmentSubmit = IsEnrollmentSubmit(id);

                    List<string> bankstatus = new List<string>();
                    bankstatus.Add(EMPConstants.Submitted);
                    bankstatus.Add(EMPConstants.EnrPending);
                    var enrollments = (from s in db.BankEnrollments
                                       join bs in db.EnrollmentBankSelections on s.BankId equals bs.BankId
                                       where s.CustomerId == id && s.IsActive == true && bankstatus.Contains(s.StatusCode) && s.ArchiveStatusCode != EMPConstants.Archive
                                       && bs.StatusCode == EMPConstants.Active && bs.CustomerId == id
                                       select s).Count();
                    if (enrollments > 0)
                        customerInformation.IsEnrollSubmitted = true;
                    else
                        customerInformation.IsEnrollSubmitted = false;

                    if (EntityHierarchyDTOs.Count > 1)
                    {
                        customerInformation.IsMSOUser = db.emp_CustomerInformation.Where(o => o.Id == ParentId).Select(o => o.IsMSOUser).FirstOrDefault() ?? false;
                    }

                    if (customerInformation.EFINStatus == 16 || customerInformation.EFINStatus == 19)
                    {
                        customerInformation.EFINStatusText = EFINText;
                    }
                    else if (customerInformation.EFINStatus == 21)
                    {

                        customerInformation.EFINStatusText = (EFIN > 0) ? EFINText + "<u><b>S</b></u>".ToString() : "Sharing";
                    }
                    else if (customerInformation.EFINStatus == 17 || customerInformation.EFINStatus == 20)
                    {
                        customerInformation.EFINStatusText = "Applied";
                    }
                    else if (customerInformation.EFINStatus == 18)
                    {
                        customerInformation.EFINStatusText = "Not Required";
                    }
                    else
                    {
                        customerInformation.EFINStatusText = EFINText;
                    }
                }

                return customerInformation;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetCustomerInformation", id);
                return null;
            }
        }

        public bool IsEnrollmentSubmit(Guid Id)
        {
            try
            {

                var BankEnroll = db.BankEnrollments.Any(a => a.CustomerId == Id && a.IsActive == true);
                if (BankEnroll)
                {
                    return true;
                }
                else
                {
                    var custominfo = db.emp_CustomerInformation.Where(a => a.ParentId == Id);
                    if (custominfo != null)
                    {
                        foreach (var cu in custominfo)
                        {
                            var BankEnrolls = db.BankEnrollments.Any(a => a.CustomerId == cu.Id && a.IsActive == true);
                            if (BankEnrolls)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/IsEnrollmentSubmit", Id);
                return false;
            }
        }
        // GET: api/emp_CustomerInformation/5
        public CustomerInformationDisplayDTO Save(CustomerInformationModel model, Guid id)
        {
            try
            {
                int ParentEntityId = 0;
                CustomerInformationDisplayDTO CustomerInfoDisplayDTO = new CustomerInformationDisplayDTO();
                int BaseEntityId = 0;
                int EntityTypeC = 0;
                bool IsAdditionalEFINSubSite = false;
                Guid Customer_GUID;
                bool IsFeeder = false;
                int customerEntity = 0;

                if (model != null)
                {
                    //var custExist = db.emp_CustomerLoginInformation.Any(a => a.EFIN == model.EFIN && a.CustomerOfficeId != id);
                    //if (custExist)
                    //{
                    //    CustomerInfoDisplayDTO.StatusCode = "-1";
                    //    return CustomerInfoDisplayDTO;
                    //}

                    int EFIN = model.EFIN ?? 0;
                    if (EFIN > 0 && (model.EFINStatus == 16 || model.EFINStatus == 19))
                    {
                        var custExist = db.emp_CustomerInformation.Any(a => a.EFIN == model.EFIN && a.Id != id && a.EFINStatus != 21);
                        if (custExist)
                        {
                            CustomerInfoDisplayDTO.StatusCode = "-1";
                            return CustomerInfoDisplayDTO;
                        }
                    }
                    emp_CustomerInformation customerInformation = db.emp_CustomerInformation.Where(e => e.Id == id).FirstOrDefault();
                    if (customerInformation != null)
                    {
                        customerEntity = customerInformation.EntityId ?? 0;
                        IsFeeder = customerInformation.Feeder ?? false;
                        EntityTypeC = (int)System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        customerInformation = new emp_CustomerInformation();
                        customerInformation.Id = Guid.NewGuid();
                        Customer_GUID = customerInformation.Id;
                    }
                    if (customerInformation.IsActivationCompleted != 1)
                    {
                        //customerInformation.IsActivationCompleted = 0;                        
                        customerInformation.AccountStatus = "Not Active";
                    }

                    if (model.ParentId != null && model.ParentId != Guid.Empty)
                    {

                        customerInformation.ParentId = model.ParentId;

                        var dbParent = (from cu in db.emp_CustomerInformation
                                        join ci in db.emp_CustomerLoginInformation on cu.Id equals ci.CustomerOfficeId
                                        where cu.Id == model.ParentId
                                        select new { cu, ci }).FirstOrDefault();

                        customerInformation.SalesforceParentID = dbParent.ci.CrossLinkUserId;
                        customerInformation.MasterIdentifier = dbParent.ci.MasterIdentifier;

                        // var EntityMaster = db.EntityMasters.Where(a => a.Id == dbParent.cu.EntityId).Select(o => new { o.Name, o.Id,o.BaseEntityId }).FirstOrDefault();

                        if (model.IsAdditionalEFINSubSite == true)
                        {
                            //Entity = (from e in db.EntityMasters
                            //          where e.DisplayId == (int)EMPConstants.Entities.SOME_SubSite
                            //          select new { e.DisplayId, e.Id, e.Name }).FirstOrDefault();

                            IsAdditionalEFINSubSite = true;

                            if (dbParent.cu.EntityId == (int)EMPConstants.Entity.SVB_AE)
                            {
                                customerInformation.EROType = "Service Bureau - Additional EFIN - Sub Site";
                                customerInformation.EntityId = (int)EMPConstants.Entity.SVB_AE_SS;// new Guid("d894953e-88d3-4edd-9185-a2c07a76da56");
                                BaseEntityId = (int)EMPConstants.BaseEntities.AE_SS;
                            }
                            else if (dbParent.cu.EntityId == (int)EMPConstants.Entity.SVB_MO_AE)
                            {
                                customerInformation.EROType = "Service Bureau - Multi-Office - Additional EFIN - Sub Site";
                                customerInformation.EntityId = (int)EMPConstants.Entity.SVB_MO_AE_SS;// new Guid("d894953e-88d3-4edd-9185-a2c07a76da56");
                                BaseEntityId = (int)EMPConstants.BaseEntities.AE_SS;
                            }
                            else if (dbParent.cu.EntityId == (int)EMPConstants.Entity.MO_AE)
                            {
                                customerInformation.EROType = "Multi Office - Additional EFIN - Sub Site";
                                customerInformation.EntityId = (int)EMPConstants.Entity.MO_AE_SS;// new Guid("d894953e-88d3-4edd-9185-a2c07a76da56");
                                BaseEntityId = (int)EMPConstants.BaseEntities.AE_SS;
                            }
                            else if (dbParent.cu.EntityId == (int)EMPConstants.Entity.SOME)
                            {
                                customerInformation.EROType = "SOME - Additional EFIN - Sub Site";
                                customerInformation.EntityId = (int)EMPConstants.Entity.SOME_SS;// new Guid("d894953e-88d3-4edd-9185-a2c07a76da56");
                                BaseEntityId = (int)EMPConstants.BaseEntities.AE_SS;
                                ParentEntityId = (int)EMPConstants.Entity.SOME;
                            }

                            // EntityDisplayId = (int)EMPConstants.Entities.SOME_SubSite;

                            //  customerInformation.IsActivationCompleted = 1;
                            customerInformation.IsVerified = true;
                            // customerInformation.AccountStatus = "Active";
                        }
                        else
                        {
                            if (model.IsAdditionalEFINSubSite != true)
                            {
                                if (dbParent.cu.EntityId == (int)EMPConstants.Entity.MO)
                                {
                                    customerInformation.EROType = "Multi Office - Single Office";
                                    customerInformation.EntityId = (int)EMPConstants.Entity.MO_SO;// new Guid("d4240d22-c1e5-41c0-b34c-a711522f6046");
                                    BaseEntityId = (int)EMPConstants.BaseEntities.AE;
                                }
                                else if (dbParent.cu.EntityId == (int)EMPConstants.Entity.SVB)
                                {
                                    customerInformation.EROType = "Service Bureau -  Single Office";
                                    customerInformation.EntityId = (int)EMPConstants.Entity.SVB_SO;// new Guid("d894953e-88d3-4edd-9185-a2c07a76da56");
                                    BaseEntityId = (int)EMPConstants.BaseEntities.AE;
                                }
                                else if (dbParent.cu.EntityId == (int)EMPConstants.Entity.SVB_MO)
                                {
                                    customerInformation.EROType = "Service Bureau - Multi Office - Single Office";
                                    customerInformation.EntityId = (int)EMPConstants.Entity.SVB_MO_SO;// new Guid("d894953e-88d3-4edd-9185-a2c07a76da56");
                                    BaseEntityId = (int)EMPConstants.BaseEntities.AE;
                                }
                                else
                                {
                                    return CustomerInfoDisplayDTO;
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(model.CompanyName))
                        customerInformation.CompanyName = model.CompanyName;


                    if (model.Feeder != null)
                        customerInformation.Feeder = model.Feeder;

                    if (!string.IsNullOrEmpty(model.BusinessOwnerFirstName))
                        customerInformation.BusinessOwnerFirstName = model.BusinessOwnerFirstName;

                    if (!string.IsNullOrEmpty(model.BusinessOwnerLastName))
                        customerInformation.BusinesOwnerLastName = model.BusinessOwnerLastName;

                    if (!string.IsNullOrEmpty(model.OfficePhone))
                        customerInformation.OfficePhone = model.OfficePhone;

                    //   if (!string.IsNullOrEmpty(model.AlternatePhone))
                    customerInformation.AlternatePhone = model.AlternatePhone;

                    if (!string.IsNullOrEmpty(model.Primaryemail))
                        customerInformation.PrimaryEmail = model.Primaryemail;

                    //     if (!string.IsNullOrEmpty(model.SupportNotificationemail))
                    customerInformation.SupportNotificationEmail = model.SupportNotificationemail;

                    if (!string.IsNullOrEmpty(model.EROType))
                        customerInformation.EROType = model.EROType;

                    //   if (!string.IsNullOrEmpty(model.AlternativeContact))
                    customerInformation.AlternativeContact = model.AlternativeContact;

                    if (model.EFIN == null)
                        customerInformation.EFIN = 0;
                    else
                        customerInformation.EFIN = model.EFIN;

                    //11212016
                    if (model.EFINStatus != null)
                        customerInformation.EFINStatus = model.EFINStatus;

                    //  if (!string.IsNullOrEmpty(model.PhysicalAddress1))
                    customerInformation.PhysicalAddress1 = model.PhysicalAddress1;

                    // if (!string.IsNullOrEmpty(model.PhysicalAddress2))
                    customerInformation.PhysicalAddress2 = model.PhysicalAddress2;

                    //  if (!string.IsNullOrEmpty(model.PhysicalZipcode))
                    customerInformation.PhysicalZipCode = model.PhysicalZipcode;

                    //  if (!string.IsNullOrEmpty(model.PhysicalCity))
                    customerInformation.PhysicalCity = model.PhysicalCity;

                    //  if (!string.IsNullOrEmpty(model.PhysicalState))
                    customerInformation.PhysicalState = model.PhysicalState;

                    // if (!string.IsNullOrEmpty(model.ShippingAddress1))
                    customerInformation.ShippingAddress1 = model.ShippingAddress1;

                    //  if (!string.IsNullOrEmpty(model.ShippingAddress2))
                    customerInformation.ShippingAddress2 = model.ShippingAddress2;

                    //  if (!string.IsNullOrEmpty(model.ShippingZipcode))
                    customerInformation.ShippingZipCode = model.ShippingZipcode;

                    //  if (!string.IsNullOrEmpty(model.ShippingCity))
                    customerInformation.ShippingCity = model.ShippingCity;

                    //  if (!string.IsNullOrEmpty(model.ShippingState))
                    customerInformation.ShippingState = model.ShippingState;

                    if (model.PhoneTypeId != Guid.Empty)
                        customerInformation.PhoneTypeId = model.PhoneTypeId;

                    //   if (model.TitleId != Guid.Empty)
                    customerInformation.TitleId = model.TitleId;

                    //  if (model.AlternativeType != Guid.Empty)
                    customerInformation.AlternativeType = model.AlternativeType;

                    customerInformation.AlternatePhoneTypeId = model.AlternativePhoneType;


                    //if (!string.IsNullOrEmpty(model.SalesforceParentID))
                    //    customerInformation.SalesforceParentID = model.SalesforceParentID;

                    //if (!string.IsNullOrEmpty(model.MasterIdentifier))
                    //    customerInformation.MasterIdentifier = model.MasterIdentifier;

                    customerInformation.LastUpdatedDate = DateTime.Now;
                    customerInformation.LastUpdatedBy = model.UserId;

                    // if (model.UserId == customerInformation.Id || IsAdditionalEFINSubSite)
                    customerInformation.IsVerified = true;

                    if (EntityTypeC == (int)System.Data.Entity.EntityState.Modified)
                    {
                        if (customerInformation.StatusCode != EMPConstants.InProgress && customerInformation.StatusCode != EMPConstants.Active)
                        {
                            customerInformation.StatusCode = EMPConstants.Created;
                        }

                        db.Entry(customerInformation).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        var SalesYear = db.SalesYearMasters.Where(o => o.ApplicableToDate == null).FirstOrDefault();
                        if (SalesYear != null)
                        {
                            customerInformation.SalesYearID = SalesYear.Id;
                            customerInformation.SalesYearGroupId = customerInformation.Id;
                        }

                        customerInformation.StatusCode = EMPConstants.InProgress;
                        if (ParentEntityId == (int)EMPConstants.Entity.SOME)
                        {
                            customerInformation.AccountStatus = "Active";
                            customerInformation.StatusCode = EMPConstants.Active;
                            customerInformation.IsActivationCompleted = 1;
                        }

                        customerInformation.CreatedDate = DateTime.Now;
                        customerInformation.CreatedBy = model.UserId;
                        db.emp_CustomerInformation.Add(customerInformation);
                    }

                    try
                    {
                        db.SaveChanges();

                        if (customerInformation != null)
                        {
                            if (customerInformation.ParentId != Guid.Empty)
                            {
                                DropDownService ddService = new DropDownService();
                                var items = ddService.GetBottomToTopHierarchy(customerInformation.Id);
                            }
                        }


                        if (EntityTypeC != (int)System.Data.Entity.EntityState.Modified)
                        {
                            //CreateNewUserResponse newuser = new CreateNewUserResponse();
                            //if (customerInformation.ParentId.HasValue)
                            //{
                            //    CreateNewUserModel userModel = new CreateNewUserModel();
                            //    userModel.MasterIdentifier = "";
                            //    userModel.AddBusinessProduct = false;
                            //    userModel.CompanyName = model.CompanyName;
                            //    userModel.Email = model.Primaryemail;
                            //    userModel.Fname = model.BusinessOwnerFirstName;
                            //    userModel.Lname = model.BusinessOwnerFirstName;
                            //    userModel.Phone = model.OfficePhone;
                            //    userModel.ShippingAddress = model.ShippingAddress1;
                            //    userModel.ShippingCity = model.ShippingCity; ;
                            //    userModel.ShippingState = model.ShippingState;
                            //    userModel.ShippingZip = model.ShippingZipcode;
                            //    userModel.ParentCustomerId = customerInformation.ParentId.Value;
                            //    userModel.CustomerId = customerInformation.Id;
                            //    newuser = CreateNewUser(userModel);
                            //}

                            db = new DatabaseEntities();
                            emp_CustomerLoginInformation oLogin = new emp_CustomerLoginInformation();
                            oLogin.Id = Guid.NewGuid();
                            // oLogin.EFIN = customerInformation.EFIN;
                            oLogin.MasterIdentifier = customerInformation.MasterIdentifier;
                            oLogin.OfficePortalUrl = "https://www.mytaxofficeportal.com";
                            oLogin.CreatedBy = model.UserId;
                            oLogin.CreatedDate = System.DateTime.Now;
                            oLogin.CustomerOfficeId = customerInformation.Id;



                            oLogin.StatusCode = EMPConstants.InProgress;
                            oLogin.LastUpdatedBy = model.UserId;
                            oLogin.LastUpdatedDate = System.DateTime.Now;
                            //if (newuser.Status)
                            //{
                            //    oLogin.CrossLinkUserId = newuser.UserId;
                            //    string Password = PasswordManager.CryptText(newuser.Password);

                            //    oLogin.CrossLinkPassword = Password;
                            //    oLogin.EMPPassword = Password;
                            //    oLogin.EMPUserId = newuser.UserId;
                            //    oLogin.StatusCode = EMPConstants.Active;

                            //    var officeinfo = db.emp_CustomerInformation.Where(x => x.Id == customerInformation.Id).FirstOrDefault();
                            //    officeinfo.StatusCode = EMPConstants.Created;

                            //}
                            //else
                            //{
                            if (ParentEntityId == (int)EMPConstants.Entity.SOME)
                            {
                                EMPCustomerLogin EMPCustomer = GetEMPCustomerLogin(model.ParentId ?? Guid.Empty);
                                oLogin.CrossLinkPassword = EMPCustomer.EMPPassword;
                                oLogin.CrossLinkUserId = EMPCustomer.EMPUserId;
                                oLogin.EMPPassword = EMPCustomer.EMPPassword;
                                oLogin.EMPUserId = EMPCustomer.EMPUserId;
                                oLogin.StatusCode = EMPConstants.Active;
                            }
                            // }


                            db.emp_CustomerLoginInformation.Add(oLogin);
                            db.SaveChanges();

                            if (IsAdditionalEFINSubSite)
                            {
                                if ((customerInformation.ParentId ?? Guid.Empty) != Guid.Empty)
                                {
                                    SaveExistingForAdditionalEFINSubSite(customerInformation.Id, customerInformation.ParentId ?? Guid.Empty, model.UserId ?? Guid.Empty, model.SalesYearID ?? Guid.Empty);
                                }
                            }
                            //else
                            //{
                            //    if ((customerInformation.ParentId ?? Guid.Empty) != Guid.Empty)
                            //    {
                            //        SaveExistingForSubSite(customerInformation.Id, customerInformation.ParentId ?? Guid.Empty);
                            //    }
                            //}


                        }
                        else
                        {
                            db = new DatabaseEntities();
                            var CustLoginInfo = db.emp_CustomerLoginInformation.Where(a => a.CustomerOfficeId == id).FirstOrDefault();
                            if (CustLoginInfo != null)
                            {
                                // CustLoginInfo.EFIN = customerInformation.EFIN;
                                CustLoginInfo.LastUpdatedBy = model.UserId;
                                CustLoginInfo.LastUpdatedDate = System.DateTime.Now;
                                db.Entry(CustLoginInfo).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                        db.Dispose();

                        CustomerInfoDisplayDTO.Id = customerInformation.Id.ToString();

                        if (EntityTypeC == (int)System.Data.Entity.EntityState.Modified)
                        {
                            CustomerInfoDisplayDTO.StatusCode = "2";
                            CustomerInfoDisplayDTO.EntityId = customerInformation.EntityId.ToString();
                            return CustomerInfoDisplayDTO;
                        }
                        else
                        {
                            CustomerInfoDisplayDTO.Id = customerInformation.Id.ToString();
                            CustomerInfoDisplayDTO.EntityId = customerInformation.EntityId.ToString();
                            CustomerInfoDisplayDTO.DisplayId = BaseEntityId;
                            return CustomerInfoDisplayDTO;
                        }
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        CustomerInfoDisplayDTO.StatusCode = "0";
                        return CustomerInfoDisplayDTO;
                    }
                }

                CustomerInfoDisplayDTO.StatusCode = "0";
                return CustomerInfoDisplayDTO;

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/Save", id);
                return new CustomerInformationDisplayDTO();
            }
        }

        public int UpdateSubSiteCustomerInfo(Guid Id, Guid ParentId)
        {
            try
            {
                int iretval = 0;
                try
                {
                    string StatusCode = "";

                    emp_CustomerLoginInformation customer_login = db.emp_CustomerLoginInformation.Where(e => e.CustomerOfficeId == Id).FirstOrDefault();
                    customer_login.StatusCode = EMPConstants.Active;

                    StatusCode = EMPConstants.NewCustomer;

                    bool RootIsMsoUser = false;

                    DropDownService _ddService = new DropDownService();
                    List<BankQuestionDTO> DropDownDTOlst = new List<BankQuestionDTO>();
                    db = new DatabaseEntities();
                    //var entitytype = db.EntityMasters.Where(x => x.Id == entityid).Select(x => x.DisplayId).FirstOrDefault();

                    List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                    EntityHierarchyDTOs = _ddService.GetEntityHierarchies(Id);

                    Guid TopParentId = Guid.Empty;

                    if (EntityHierarchyDTOs.Count > 0)
                    {
                        var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                        TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;
                    }

                    if (TopParentId != Guid.Empty)
                    {
                        RootIsMsoUser = db.emp_CustomerInformation.Where(e => e.Id == TopParentId).Select(o => o.IsMSOUser ?? false).FirstOrDefault();
                    }


                    var parentdata = db.emp_CustomerInformation
                                      .Join(db.emp_CustomerLoginInformation, custinfo => custinfo.Id, custlog => custlog.CustomerOfficeId,
                                                          (custinfo, custlog) => new { custinfo, custlog })
                                      .Where(o => o.custinfo.Id == ParentId)
                                      .Select(g => new
                                      {
                                          Id = g.custinfo.Id,
                                          CrossLinkUserId = g.custlog.CrossLinkUserId,
                                          //IsMsoUser = g.custinfo.IsMSOUser ?? false,
                                          ParentId = g.custinfo.ParentId ?? Guid.Empty
                                      }).FirstOrDefault();


                    if (parentdata != null)
                    {
                        Guid NewParentId = Guid.Empty;

                        bool IsMSOorAdditionalEFINSubSite = RootIsMsoUser;

                        if (!RootIsMsoUser && parentdata.ParentId != Guid.Empty)
                        {
                            var SubSiteOfficeCofig = db.SubSiteOfficeConfigs.Where(o => o.RefId == ParentId).FirstOrDefault();
                            if (SubSiteOfficeCofig != null)
                            {
                                if (SubSiteOfficeCofig.SOorSSorEFIN == 3)
                                {
                                    NewParentId = ParentId;// parentdata.ParentId;
                                    IsMSOorAdditionalEFINSubSite = true;
                                }
                            }
                        }
                        else
                        {
                            NewParentId = ParentId;
                        }


                        if (IsMSOorAdditionalEFINSubSite)
                        {
                            var parentdata2 = db.emp_CustomerInformation
                                             .Join(db.emp_CustomerLoginInformation, custinfo => custinfo.Id, custlog => custlog.CustomerOfficeId,
                                                                 (custinfo, custlog) => new { custinfo, custlog })
                                             .Where(o => o.custinfo.ParentId == NewParentId && o.custlog.CrossLinkUserId != null)
                                             .Select(g => new
                                             {
                                                 CrossLinkUserId = g.custlog.CrossLinkUserId,
                                             }).ToList();

                            string UserIdChar = "A";
                            if (parentdata2.ToList().Count > 0)
                            {
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "A")).ToList().Count > 0)
                                {
                                    UserIdChar = "B";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "B")).ToList().Count > 0)
                                {
                                    UserIdChar = "C";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "C")).ToList().Count > 0)
                                {
                                    UserIdChar = "D";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "D")).ToList().Count > 0)
                                {
                                    UserIdChar = "E";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "E")).ToList().Count > 0)
                                {
                                    UserIdChar = "F";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "F")).ToList().Count > 0)
                                {
                                    UserIdChar = "G";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "G")).ToList().Count > 0)
                                {
                                    UserIdChar = "H";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "H")).ToList().Count > 0)
                                {
                                    UserIdChar = "I";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "I")).ToList().Count > 0)
                                {
                                    UserIdChar = "J";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "J")).ToList().Count > 0)
                                {
                                    UserIdChar = "K";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "K")).ToList().Count > 0)
                                {
                                    UserIdChar = "L";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "L")).ToList().Count > 0)
                                {
                                    UserIdChar = "M";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "M")).ToList().Count > 0)
                                {
                                    UserIdChar = "N";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "N")).ToList().Count > 0)
                                {
                                    UserIdChar = "O";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "O")).ToList().Count > 0)
                                {
                                    UserIdChar = "P";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "P")).ToList().Count > 0)
                                {
                                    UserIdChar = "Q";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "Q")).ToList().Count > 0)
                                {
                                    UserIdChar = "R";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "R")).ToList().Count > 0)
                                {
                                    UserIdChar = "S";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "S")).ToList().Count > 0)
                                {
                                    UserIdChar = "T";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "T")).ToList().Count > 0)
                                {
                                    UserIdChar = "U";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "U")).ToList().Count > 0)
                                {
                                    UserIdChar = "V";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "V")).ToList().Count > 0)
                                {
                                    UserIdChar = "W";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "W")).ToList().Count > 0)
                                {
                                    UserIdChar = "X";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "X")).ToList().Count > 0)
                                {
                                    UserIdChar = "Y";
                                }
                                if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "Y")).ToList().Count > 0)
                                {
                                    UserIdChar = "Z";
                                }
                            }

                            customer_login.CrossLinkUserId = parentdata.CrossLinkUserId + UserIdChar;
                            customer_login.EMPUserId = parentdata.CrossLinkUserId + UserIdChar;

                            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                            var stringChars = new char[8];
                            var random = new Random();

                            for (int i = 0; i < stringChars.Length; i++)
                            {
                                stringChars[i] = chars[random.Next(chars.Length)];
                            }

                            var finalString = new String(stringChars);
                            string Password = PasswordManager.CryptText(finalString);
                            customer_login.CrossLinkPassword = Password;
                            customer_login.EMPPassword = Password;
                            customer_login.StatusCode = EMPConstants.Active;
                            StatusCode = EMPConstants.Active;

                            iretval = 1;
                            db.Entry(customer_login).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(customer_login.EMPPassword))
                            {
                                CreateNewUserResponse newuser = new CreateNewUserResponse();
                                var empCustInfo = db.emp_CustomerInformation.Where(e => e.Id == Id).FirstOrDefault();
                                CreateNewUserModel userModel = new CreateNewUserModel();
                                userModel.MasterIdentifier = "";
                                userModel.AddBusinessProduct = false;
                                userModel.CompanyName = empCustInfo.CompanyName;
                                userModel.Email = empCustInfo.PrimaryEmail;
                                userModel.Fname = empCustInfo.BusinessOwnerFirstName;
                                userModel.Lname = empCustInfo.BusinessOwnerFirstName;
                                userModel.Phone = empCustInfo.OfficePhone;
                                userModel.ShippingAddress = empCustInfo.ShippingAddress1;
                                userModel.ShippingCity = empCustInfo.ShippingCity;
                                userModel.ShippingState = empCustInfo.ShippingState;
                                userModel.ShippingZip = empCustInfo.ShippingZipCode;
                                userModel.ParentCustomerId = empCustInfo.ParentId.Value;
                                userModel.CustomerId = empCustInfo.Id;
                                newuser = CreateNewUser(userModel);
                                if (newuser.Status)
                                {
                                    if (string.IsNullOrEmpty(newuser.Password))
                                    {
                                        StatusCode = EMPConstants.Created;
                                        iretval = 4;
                                    }
                                    else
                                    {
                                        iretval = 2;
                                        StatusCode = EMPConstants.Active; // previously Created status.
                                    }
                                    //customer_login.CrossLinkUserId = newuser.UserId;
                                    //string Password = PasswordManager.CryptText(newuser.Password);
                                    //customer_login.CrossLinkPassword = Password;
                                    //customer_login.EMPPassword = Password;
                                    //customer_login.EMPUserId = newuser.UserId;
                                    //customer_login.StatusCode = EMPConstants.Active;
                                    SendEmailForNewUser(new NewUserEmailRequest() { CustomerId = Id, UserId = Id });
                                }
                                else
                                {
                                    iretval = 3;
                                }

                                db.SaveChanges();
                            }
                            else
                                iretval = 1;
                        }
                    }
                    else
                        iretval = 1;



                    emp_CustomerInformation customerInformation = db.emp_CustomerInformation.Where(e => e.Id == Id).FirstOrDefault();
                    customerInformation.StatusCode = StatusCode;


                    //if (StatusCode == EMPConstants.Created)
                    //    customerInformation.AccountStatus = "Created";
                    //else if (StatusCode == EMPConstants.NewCustomer)
                    //    customerInformation.AccountStatus = "Not Active";
                    //else if (StatusCode == EMPConstants.Active)
                    //    customerInformation.AccountStatus = "Active";

                    if (iretval == 3)
                    {
                        customerInformation.AccountStatus = "Not Active";
                        customerInformation.IsActivationCompleted = 1;
                    }
                    else
                    {
                        customerInformation.AccountStatus = "Active";
                        customerInformation.IsActivationCompleted = 1;
                    }
                    customerInformation.LastUpdatedDate = System.DateTime.Now;
                    db.Entry(customerInformation).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    db.Dispose();

                    if (customerInformation != null)
                    {
                        DropDownService ddService = new DropDownService();
                        var items = ddService.GetBottomToTopHierarchy(customerInformation.Id);
                    }

                }
                catch (Exception)
                {
                    iretval = 0;
                }
                return iretval;

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/UpdateSubSiteCustomerInfo", Id);
                return 0;
            }
        }

        private bool emp_CustomerInformationExists(Guid id)
        {
            return db.emp_CustomerInformation.Count(e => e.Id == id) > 0;
        }

        public async Task<CustomerModel> GetCustomerInformationWithSalyesYearID(Guid SYGrpId, Guid SalesYearId)
        {
            try
            {
                var data = await (from e in db.emp_CustomerInformation
                                  join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
                                  where e.SalesYearGroupId == SYGrpId && e.SalesYearID == SalesYearId && e.StatusCode != EMPConstants.InActive
                                  select new CustomerModel
                                  {
                                      Id = e.Id,
                                      ParentId = e.ParentId ?? Guid.Empty,
                                      EntityId = e.EntityId ?? 0,
                                      SalesYearGroupId = e.SalesYearGroupId ?? Guid.Empty,
                                      BaseEntityId = db.EntityMasters.Where(o => o.Id == e.EntityId).Select(o => o.BaseEntityId).FirstOrDefault(),
                                      CompanyName = e.CompanyName,
                                      AccountStatus = e.AccountStatus,
                                      Feeder = e.Feeder,
                                      BusinessOwnerFirstName = e.BusinessOwnerFirstName,
                                      OfficePhone = e.OfficePhone,
                                      AlternatePhone = e.AlternatePhone,
                                      Primaryemail = e.PrimaryEmail,
                                      SupportNotificationemail = e.SupportNotificationEmail,
                                      EROType = e.EROType,
                                      AlternativeContact = e.AlternativeContact,
                                      EFIN = e.EFIN,
                                      EFINStatus = e.EFINStatus,
                                      PhysicalAddress1 = e.PhysicalAddress1,
                                      PhysicalAddress2 = e.PhysicalAddress2,
                                      PhysicalZipcode = e.PhysicalZipCode,
                                      PhysicalCity = e.PhysicalCity,
                                      PhysicalState = e.PhysicalState,
                                      ShippingAddress1 = e.ShippingAddress1,
                                      ShippingAddress2 = e.ShippingAddress2,
                                      ShippingZipcode = e.ShippingZipCode,
                                      ShippingCity = e.ShippingCity,
                                      ShippingState = e.ShippingState,
                                      PhoneTypeId = e.PhoneTypeId,
                                      TitleId = e.TitleId,
                                      IsVerified = e.IsVerified,
                                      // PhoneType = db.PhoneTypeMasters.Where(a => a.Id == e.PhoneTypeId).Select(a => a.PhoneType).FirstOrDefault(),
                                      // ContactTitle = db.ContactPersonTitleMasters.Where(a => a.Id == e.TitleId).Select(a => a.ContactPersonTitle).FirstOrDefault(),

                                      SalesYearID = e.SalesYearID ?? Guid.Empty,
                                      SalesforceParentID = e.SalesforceParentID,
                                      IsMSOUser = e.IsMSOUser ?? false,

                                      LoginId = cli.Id.ToString(),
                                      //LoginEFIN = cli.EFIN,
                                      MasterIdentifier = cli.MasterIdentifier,
                                      CrossLinkUserId = cli.CrossLinkUserId,
                                      CrossLinkPassword = cli.CrossLinkPassword,
                                      OfficePortalUrl = cli.OfficePortalUrl,
                                      TaxOfficeUsername = cli.TaxOfficeUsername,
                                      TaxOfficePassword = cli.TaxOfficePassword,
                                      CustomerOfficeId = cli.CustomerOfficeId,


                                  }).Distinct().FirstOrDefaultAsync();
                return data;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetCustomerInformationWithSalyesYearID", Guid.Empty);
                return null;
            }
        }

        /// <summary>
        /// This method is used to get search customer informatation
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>

        //public IQueryable<CustomerModel> GetSearchCustomerInformation(Guid UserName)
        //{
        //    try
        //    {

        //        var data = (from e in db.emp_CustomerInformation
        //                    join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                    where e.ParentId == null && cli.CrossLinkUserId != null
        //                    && e.StatusCode != EMPConstants.NewCustomer
        //                    select new { e, cli }).ToList();
        //        if (data.Count > 0)
        //        {
        //            if (data.Any(a => a.cli.CustomerOfficeId == UserName))
        //            {
        //                data = data.Where(a => a.cli.CustomerOfficeId == UserName).ToList();
        //            }
        //        }
        //        List<CustomerModel> customerlst = new List<CustomerModel>();
        //        foreach (var itm in data)
        //        {
        //            CustomerModel ocustomer = new CustomerModel();
        //            ocustomer.Id = itm.e.Id;
        //            ocustomer.ParentId = itm.e.ParentId ?? Guid.Empty;
        //            ocustomer.CompanyName = itm.e.CompanyName;
        //            ocustomer.AccountStatus = itm.e.AccountStatus;
        //            ocustomer.SalesforceParentID = itm.e.SalesforceParentID;
        //            ocustomer.MasterIdentifier = itm.e.MasterIdentifier;
        //            ocustomer.Feeder = itm.e.Feeder;
        //            ocustomer.BusinessOwnerFirstName = itm.e.BusinessOwnerFirstName;
        //            ocustomer.OfficePhone = itm.e.OfficePhone;
        //            ocustomer.AlternatePhone = itm.e.AlternatePhone;
        //            ocustomer.Primaryemail = itm.e.PrimaryEmail;
        //            ocustomer.SupportNotificationemail = itm.e.SupportNotificationEmail;
        //            ocustomer.EROType = itm.e.EROType;
        //            ocustomer.AlternativeContact = itm.e.AlternativeContact ?? "";
        //            ocustomer.EFIN = itm.e.EFIN;
        //            ocustomer.EFINStatus = itm.e.EFINStatus;
        //            ocustomer.PhysicalAddress1 = itm.e.PhysicalAddress1;
        //            ocustomer.PhysicalAddress2 = itm.e.PhysicalAddress2;
        //            ocustomer.PhysicalZipcode = itm.e.PhysicalZipCode;
        //            ocustomer.PhysicalCity = itm.e.PhysicalCity;
        //            ocustomer.PhysicalState = itm.e.PhysicalState;
        //            ocustomer.ShippingAddress1 = itm.e.ShippingAddress1;
        //            ocustomer.ShippingAddress2 = itm.e.ShippingAddress2;
        //            ocustomer.ShippingZipcode = itm.e.ShippingZipCode;
        //            ocustomer.ShippingCity = itm.e.ShippingCity;
        //            ocustomer.ShippingState = itm.e.ShippingState;
        //            ocustomer.PhoneTypeId = itm.e.PhoneTypeId;
        //            ocustomer.TitleId = itm.e.TitleId;
        //            ocustomer.AlternativeType = itm.e.AlternativeType;
        //            ocustomer.PhoneType = db.PhoneTypeMasters.Where(a => a.Id == itm.e.PhoneTypeId).Select(a => a.PhoneType).FirstOrDefault();
        //            ocustomer.ContactTitle = db.ContactPersonTitleMasters.Where(a => a.Id == itm.e.TitleId).Select(a => a.ContactPersonTitle).FirstOrDefault() ?? "";
        //            ocustomer.EntityId = itm.e.EntityId ?? 0;
        //            ocustomer.SalesYearID = itm.e.SalesYearID ?? Guid.Empty;
        //            ocustomer.BaseEntityId = db.EntityMasters.Where(a => a.Id == itm.e.EntityId).Select(a => a.BaseEntityId).FirstOrDefault();
        //            ocustomer.LoginId = itm.cli.Id.ToString();

        //            //ocustomer.LoginEFIN = itm.cli.EFIN;
        //            ocustomer.MasterIdentifier = itm.cli.MasterIdentifier;
        //            ocustomer.CrossLinkUserId = itm.cli.CrossLinkUserId ?? "";
        //            ocustomer.CrossLinkPassword = itm.cli.CrossLinkPassword;
        //            ocustomer.OfficePortalUrl = itm.cli.OfficePortalUrl;
        //            ocustomer.TaxOfficeUsername = itm.cli.TaxOfficeUsername;
        //            ocustomer.TaxOfficePassword = itm.cli.TaxOfficePassword;
        //            ocustomer.CustomerOfficeId = itm.cli.CustomerOfficeId;
        //            ocustomer.MasterIdentifier = itm.cli.MasterIdentifier;
        //            ocustomer.IsActivationCompleted = itm.e.IsActivationCompleted ?? 0;
        //            ocustomer.ChaildCustomerInfoCount = db.emp_CustomerInformation.Where(a => a.ParentId == itm.e.Id).Count();
        //            ocustomer.SalesforceParentID = itm.e.SalesforceParentID;
        //            if (itm.e.IsActivationCompleted == null || itm.e.IsActivationCompleted == 0)
        //                ocustomer.ActivationStatus = "Not Active";
        //            else
        //                ocustomer.ActivationStatus = "Active";

        //            ocustomer.CreatedDate = itm.cli.CreatedDate ?? Convert.ToDateTime("01/01/2000");
        //            ocustomer.LastUpdatedDate = itm.e.LastUpdatedDate ?? Convert.ToDateTime("01/01/2000");

        //            //var CustFees = CustomerFees(itm.e.Id);
        //            //foreach (var fees in CustFees)
        //            //{
        //            //    if (fees.FeeFor == (int)EMPConstants.FeesFor.SVBFees)
        //            //    {
        //            //        ocustomer.TotalServiceFee = fees.Amount;
        //            //        ocustomer.ServiceTooltip = fees.FeesName;
        //            //    }

        //            //    if (fees.FeeFor == (int)EMPConstants.FeesFor.TransmissionFees)
        //            //    {
        //            //        ocustomer.TotalTransFee = fees.Amount;
        //            //        ocustomer.TransTooltip = fees.FeesName;
        //            //    }
        //            //}
        //            //var BankFees = BankFee(itm.e.Id, itm.e.Id, ocustomer.CompanyName, ocustomer.CompanyName, 0);
        //            //foreach (var fees in BankFees)
        //            //{
        //            //    if (fees.FeeFor == (int)EMPConstants.FeesFor.SVBFees)
        //            //    {
        //            //        ocustomer.TotalServiceFee = ocustomer.TotalServiceFee + fees.Amount;
        //            //        ocustomer.ServiceTooltip = ocustomer.ServiceTooltip + fees.FeesName;
        //            //    }

        //            //    if (fees.FeeFor == (int)EMPConstants.FeesFor.TransmissionFees)
        //            //    {
        //            //        ocustomer.TotalTransFee = ocustomer.TotalTransFee + fees.Amount;
        //            //        ocustomer.TransTooltip = ocustomer.TransTooltip + fees.FeesName;
        //            //    }
        //            //}


        //            if (ocustomer.EROType == "Single Office")
        //                ocustomer.IsActivated = true;
        //            else
        //            {
        //                ocustomer.IsActivated = itm.e.IsActivationCompleted.HasValue ? (itm.e.IsActivationCompleted.Value == 1 ? true : false) : false;
        //            }
        //            customerlst.Add(ocustomer);
        //        }
        //        return customerlst.OrderBy(a => a.CompanyName).AsQueryable();

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetSearchCustomerInformation", UserName);
        //        return new List<CustomerModel>().AsQueryable();
        //    }
        //}

        //public IQueryable<CustomerModel> GetSearchCustomerInformation(Guid UserName, string Status, string SiteType, string BankPartner, string EnrollmentStatus, string OnBoardingStatus, string SearchText, int SearchType)
        //{
        //    try
        //    {

        //        //string Status, string SiteType, string BankPartner, string EnrollmentStatus, string OnBoardingStatus

        //        List<string> Statuslst = !string.IsNullOrEmpty(Status) ? Status.Split(',').ToList() : null;
        //        List<string> SiteTypelst = !string.IsNullOrEmpty(SiteType) ? SiteType.Split(',').ToList() : null;
        //        List<string> BankPartnerlst = !string.IsNullOrEmpty(BankPartner) ? BankPartner.Split(',').ToList() : null;
        //        List<string> EnrollmentStatuslst = !string.IsNullOrEmpty(EnrollmentStatus) ? EnrollmentStatus.Split(',').ToList() : null;
        //        List<string> OnBoardingStatuslst = !string.IsNullOrEmpty(OnBoardingStatus) ? OnBoardingStatus.Split(',').ToList() : null;

        //        var data = (from e in db.emp_CustomerInformation
        //                    join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                    where e.ParentId == null && cli.CrossLinkUserId != null //&& e.AlternativeContact != null && e.AlternatePhone != null
        //                    && e.StatusCode != EMPConstants.NewCustomer
        //                    select new { e, cli }).ToList();
        //        if (data.Count > 0)
        //        {
        //            if (data.Any(a => a.cli.CustomerOfficeId == UserName))
        //            {
        //                data = data.Where(a => a.cli.CustomerOfficeId == UserName).ToList();
        //            }
        //        }
        //        List<CustomerModel> customerlst = new List<CustomerModel>();
        //        foreach (var itm in data)
        //        {
        //            CustomerModel ocustomer = new CustomerModel();
        //            ocustomer.Id = itm.e.Id;
        //            ocustomer.ParentId = itm.e.ParentId ?? Guid.Empty;
        //            ocustomer.CompanyName = itm.e.CompanyName;
        //            ocustomer.AccountStatus = itm.e.AccountStatus;
        //            ocustomer.SalesforceParentID = itm.e.SalesforceParentID;
        //            ocustomer.MasterIdentifier = itm.e.MasterIdentifier;
        //            ocustomer.Feeder = itm.e.Feeder;
        //            ocustomer.BusinessOwnerFirstName = itm.e.BusinessOwnerFirstName;
        //            ocustomer.OfficePhone = itm.e.OfficePhone;
        //            ocustomer.AlternatePhone = itm.e.AlternatePhone;
        //            ocustomer.Primaryemail = itm.e.PrimaryEmail;
        //            ocustomer.SupportNotificationemail = itm.e.SupportNotificationEmail;
        //            ocustomer.EROType = itm.e.EROType;
        //            ocustomer.AlternativeContact = itm.e.AlternativeContact ?? "";
        //            ocustomer.EFIN = itm.e.EFIN;
        //            ocustomer.PhysicalAddress1 = itm.e.PhysicalAddress1;
        //            ocustomer.PhysicalAddress2 = itm.e.PhysicalAddress2;
        //            ocustomer.PhysicalZipcode = itm.e.PhysicalZipCode;
        //            ocustomer.PhysicalCity = itm.e.PhysicalCity;
        //            ocustomer.PhysicalState = itm.e.PhysicalState;
        //            ocustomer.ShippingAddress1 = itm.e.ShippingAddress1;
        //            ocustomer.ShippingAddress2 = itm.e.ShippingAddress2;
        //            ocustomer.ShippingZipcode = itm.e.ShippingZipCode;
        //            ocustomer.ShippingCity = itm.e.ShippingCity;
        //            ocustomer.ShippingState = itm.e.ShippingState;
        //            ocustomer.PhoneTypeId = itm.e.PhoneTypeId;
        //            ocustomer.TitleId = itm.e.TitleId;
        //            ocustomer.AlternativeType = itm.e.AlternativeType;
        //            ocustomer.PhoneType = db.PhoneTypeMasters.Where(a => a.Id == itm.e.PhoneTypeId).Select(a => a.PhoneType).FirstOrDefault();
        //            ocustomer.ContactTitle = db.ContactPersonTitleMasters.Where(a => a.Id == itm.e.TitleId).Select(a => a.ContactPersonTitle).FirstOrDefault() ?? "";
        //            ocustomer.EntityId = itm.e.EntityId ?? 0;
        //            ocustomer.SalesYearID = itm.e.SalesYearID ?? Guid.Empty;
        //            ocustomer.BaseEntityId = db.EntityMasters.Where(a => a.Id == itm.e.EntityId).Select(a => a.BaseEntityId).FirstOrDefault();
        //            ocustomer.LoginId = itm.cli.Id.ToString();
        //            ocustomer.LoginEFIN = itm.cli.EFIN;
        //            ocustomer.MasterIdentifier = itm.cli.MasterIdentifier;
        //            ocustomer.CrossLinkUserId = itm.cli.CrossLinkUserId ?? "";
        //            ocustomer.CrossLinkPassword = itm.cli.CrossLinkPassword;
        //            ocustomer.OfficePortalUrl = itm.cli.OfficePortalUrl;
        //            ocustomer.TaxOfficeUsername = itm.cli.TaxOfficeUsername;
        //            ocustomer.TaxOfficePassword = itm.cli.TaxOfficePassword;
        //            ocustomer.CustomerOfficeId = itm.cli.CustomerOfficeId;
        //            ocustomer.MasterIdentifier = itm.cli.MasterIdentifier;
        //            ocustomer.IsActivationCompleted = itm.e.IsActivationCompleted ?? 0;

        //            ocustomer.SalesforceParentID = itm.e.SalesforceParentID;
        //            if (itm.e.IsActivationCompleted == null || itm.e.IsActivationCompleted == 0)
        //                ocustomer.ActivationStatus = "Not Active";
        //            else
        //                ocustomer.ActivationStatus = "Active";

        //            ocustomer.CreatedDate = itm.cli.CreatedDate ?? Convert.ToDateTime("01/01/2000");
        //            ocustomer.LastUpdatedDate = itm.e.LastUpdatedDate ?? Convert.ToDateTime("01/01/2000");

        //            ocustomer.IsEnrolled = itm.e.IsEnrolled ?? false;
        //            ocustomer.EnrolledBankId = itm.e.EnrolledBankId;

        //            EnrollmentBankSelectionService serv = new EnrollmentBankSelectionInfo.EnrollmentBankSelectionService();
        //            var enrdata = serv.getEnrollmentStatusInfo(ocustomer.Id);
        //            ocustomer.ActiveBank = enrdata.BankName;
        //            ocustomer.SubmissionDate = enrdata.SubmitedDate;
        //            ocustomer.EnrollmentStatus = enrdata.SubmissionStaus;
        //            ocustomer.ApprovedBank = enrdata.ApprovedBank;
        //            ocustomer.RejectedBanks = enrdata.RejectedBanks;
        //            ocustomer.UnlockedBanks = enrdata.UnlockedBanks;

        //            var ChildDataLst = GetChildCustomerInfo(itm.e.Id, itm.e.CompanyName, Statuslst, SiteTypelst, SearchText, SearchType, UserName, null);
        //            ocustomer.ChaildCustomerInfo = ChildDataLst;

        //            if (ocustomer.EROType == "Single Office")
        //                ocustomer.IsActivated = true;
        //            else
        //            {
        //                ocustomer.IsActivated = itm.e.IsActivationCompleted.HasValue ? (itm.e.IsActivationCompleted.Value == 1 ? true : false) : false;
        //            }

        //            customerlst.Add(ocustomer);
        //        }

        //        if (Statuslst != null)
        //        {
        //            customerlst = customerlst.Where(o => Statuslst.Contains(o.StatusCode) || o.ChaildCustomerInfo.Count > 0).ToList();
        //        }
        //        if (SiteTypelst != null)
        //        {
        //            customerlst = customerlst.Where(o => SiteTypelst.Contains(o.EntityId.ToString()) || o.ChaildCustomerInfo.Count > 0).ToList();
        //        }

        //        if (SearchType > 0)
        //        {
        //            if (SearchType == 1)
        //            {
        //                customerlst = customerlst.Where(o => o.CompanyName.ToString().ToLower().Contains(SearchText.ToLower()) || o.ChaildCustomerInfo.Count > 0).ToList();
        //            }
        //            else if (SearchType == 2)
        //            {
        //                customerlst = customerlst.Where(o => o.CrossLinkUserId.ToString().ToLower().Contains(SearchText.ToLower()) || o.ChaildCustomerInfo.Count > 0).ToList();
        //            }
        //            else if (SearchType == 3)
        //            {
        //                customerlst = customerlst.Where(o => o.EFIN.ToString().ToLower().Contains(SearchText.ToLower()) || o.ChaildCustomerInfo.Count > 0).ToList();
        //            }
        //            else if (SearchType == 4)
        //            {
        //                customerlst = customerlst.Where(o => o.AlternativeContact != null ? o.AlternativeContact.ToString().ToLower().Contains(SearchText.ToLower()) : o.AlternativeContact == o.AlternativeContact).ToList();
        //            }
        //            else if (SearchType == 5)
        //            {
        //                customerlst = customerlst.Where(o => o.AlternatePhone != null ? o.AlternatePhone.ToString().ToLower().Contains(SearchText.ToLower()) : o.AlternatePhone == o.AlternatePhone).ToList();
        //            }
        //        }
        //        return customerlst.OrderBy(a => a.CompanyName).AsQueryable();

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetSearchCustomerInformation", UserName);
        //        return new List<CustomerModel>().AsQueryable();
        //    }
        //}

        //public List<CustomerModel> GetChildCustomerInfo(Guid id, string companyname, List<string> Statuslst, List<string> SiteTypelst, string SearchText, int SearchType, Guid LoginUserId, CustomerInfoNewGrid omodel)
        //{
        //    try
        //    {
        //        #region "Searching Functionality"

        //        List<string> Statuslst2 = !string.IsNullOrEmpty(omodel.Status) ? omodel.Status.Split(',').ToList() : null;
        //        List<string> SiteTypelst2 = !string.IsNullOrEmpty(omodel.SiteType) ? omodel.SiteType.Split(',').ToList() : null;
        //        List<string> BankPartnerlst = !string.IsNullOrEmpty(omodel.BankPartner) ? omodel.BankPartner.Split(',').ToList() : null;
        //        List<string> EnrollmentStatuslst = !string.IsNullOrEmpty(omodel.EnrollmentStatus) ? omodel.EnrollmentStatus.Split(',').ToList() : null;
        //        List<string> OnBoardingStatuslst = !string.IsNullOrEmpty(omodel.OnBoardingStatus) ? omodel.OnBoardingStatus.Split(',').ToList() : null;
        //        List<TestModel1> data = new List<TestModel1>();
        //        bool iretval = false;

        //        #region "Search Drop down lists"
        //        if (Statuslst2 != null)
        //        {
        //            iretval = true;
        //            #region "Status Dropdown list"

        //            List<int?> actLst = new List<int?>();
        //            List<string> stsLst = new List<string>();
        //            if (Statuslst2.Contains("INA"))
        //            {
        //                //Statuslst2.Add("CRT");
        //                //Statuslst2.Add("PEA");
        //                stsLst.Add("ACT");
        //                actLst.Add(0);
        //            }
        //            if (Statuslst2.Contains("ACT"))
        //            {
        //                stsLst.Add("ACT");
        //                actLst.Add(1);
        //            }

        //            if (Statuslst2.Contains("CRT"))
        //            {
        //                stsLst.Add("CRT");
        //                stsLst.Add("INP");
        //                if (actLst.Count == 0)
        //                {
        //                    actLst.Add(1);
        //                    actLst.Add(0);
        //                }
        //            }
        //            else
        //            {
        //                stsLst.Add("CRT");
        //                stsLst.Add("INP");
        //                if (!stsLst.Contains("ACT"))
        //                    stsLst.Add("ACT");
        //            }
        //            if (Statuslst2.Count == 3)
        //            {
        //                stsLst.Add("ACT");
        //                actLst.Add(null);
        //            }

        //            if (ExistOrNotCustomer(omodel.UserId))
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        orderby e.CompanyName
        //                        where e.ParentId == id && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                         && (stsLst.Contains(e.StatusCode) && actLst.Contains(e.IsActivationCompleted))
        //                         && cli.CustomerOfficeId == omodel.UserId
        //                        select new TestModel1 { e = e, cli = cli }).ToList();


        //                //if (Statuslst2.Contains("ACT"))
        //                //{
        //                //    data = (from e in db.emp_CustomerInformation
        //                //            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                //            orderby e.CompanyName
        //                //            where e.ParentId == id && cli.CrossLinkUserId != null
        //                //             && e.StatusCode != EMPConstants.NewCustomer
        //                //             && (Statuslst2.Contains(e.StatusCode) || (e.IsActivationCompleted == 1 && e.StatusCode != EMPConstants.NewCustomer))
        //                //             && cli.CustomerOfficeId == omodel.UserId
        //                //            select new TestModel1 { e = e, cli = cli }).ToList();
        //                //}
        //                //else
        //                //{
        //                //    data = (from e in db.emp_CustomerInformation
        //                //            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                //            orderby e.CompanyName
        //                //            where e.ParentId == id && cli.CrossLinkUserId != null
        //                //             && (e.StatusCode != EMPConstants.NewCustomer)
        //                //             && Statuslst2.Contains(e.StatusCode) && cli.CustomerOfficeId == omodel.UserId
        //                //            select new TestModel1 { e = e, cli = cli }).ToList();
        //                //}
        //            }
        //            else
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        orderby e.CompanyName
        //                        where e.ParentId == id && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                         && (stsLst.Contains(e.StatusCode) && actLst.Contains(e.IsActivationCompleted))
        //                        select new TestModel1 { e = e, cli = cli }).ToList();
        //                //if (Statuslst2.Contains("ACT"))
        //                //{
        //                //    data = (from e in db.emp_CustomerInformation
        //                //            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                //            orderby e.CompanyName
        //                //            where e.ParentId == id && cli.CrossLinkUserId != null
        //                //             && e.StatusCode != EMPConstants.NewCustomer
        //                //             && (Statuslst2.Contains(e.StatusCode) || (e.IsActivationCompleted == 1 && e.StatusCode != EMPConstants.NewCustomer))
        //                //            select new TestModel1 { e = e, cli = cli }).ToList();
        //                //}
        //                //else
        //                //{
        //                //    data = (from e in db.emp_CustomerInformation
        //                //            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                //            orderby e.CompanyName
        //                //            where e.ParentId == id && cli.CrossLinkUserId != null
        //                //             && e.StatusCode != EMPConstants.NewCustomer
        //                //             && Statuslst2.Contains(e.StatusCode)
        //                //            select new TestModel1 { e = e, cli = cli }).ToList();
        //                //}

        //            }
        //            #endregion
        //        }
        //        if (SiteTypelst2 != null)
        //        {
        //            iretval = true;
        //            #region "Site type Drop down list"
        //            if (ExistOrNotCustomer(omodel.UserId))
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        orderby e.CompanyName
        //                        where e.ParentId == id && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                         && SiteTypelst2.Contains(e.EntityId.ToString()) && cli.CustomerOfficeId == omodel.UserId
        //                        select new TestModel1 { e = e, cli = cli }).ToList();
        //            }
        //            else
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        orderby e.CompanyName
        //                        where e.ParentId == id && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                         && SiteTypelst2.Contains(e.EntityId.ToString())
        //                        select new TestModel1 { e = e, cli = cli }).ToList();
        //            }
        //            #endregion
        //        }
        //        if (EnrollmentStatuslst != null)
        //        {
        //            iretval = true;
        //            #region "Enrollment Status Drop down list"
        //            if (ExistOrNotCustomer(omodel.UserId))
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        join b in db.BankEnrollments on e.Id equals b.CustomerId
        //                        orderby e.CompanyName
        //                        where e.ParentId == id && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                         && EnrollmentStatuslst.Contains(b.StatusCode) && cli.CustomerOfficeId == omodel.UserId
        //                         && b.IsActive == true
        //                        select new TestModel1 { e = e, cli = cli }).ToList();
        //            }
        //            else
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        join b in db.BankEnrollments on e.Id equals b.CustomerId
        //                        orderby e.CompanyName
        //                        where e.ParentId == id && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                          && EnrollmentStatuslst.Contains(b.StatusCode)
        //                         && b.IsActive == true
        //                        select new TestModel1 { e = e, cli = cli }).ToList();
        //            }
        //            #endregion
        //        }
        //        if (OnBoardingStatuslst != null)
        //        {
        //            var dbcsr = new EMPDB_CSREntities();
        //            iretval = true;
        //            #region "OnBoarding Status Drop down list"
        //            if (ExistOrNotCustomer(omodel.UserId))
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        join c in dbcsr.tblCases on e.OnBoardPrimaryKey equals c.Id
        //                        orderby e.CompanyName
        //                        where e.ParentId == id && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                         && OnBoardingStatuslst.Contains(c.Status) && cli.CustomerOfficeId == omodel.UserId
        //                        select new TestModel1 { e = e, cli = cli }).ToList();
        //            }
        //            else
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        join c in dbcsr.tblCases on e.OnBoardPrimaryKey equals c.Id
        //                        orderby e.CompanyName
        //                        where e.ParentId == id && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                          && OnBoardingStatuslst.Contains(c.Status)
        //                        select new TestModel1 { e = e, cli = cli }).ToList();
        //            }
        //            #endregion
        //        }
        //        #endregion

        //        if (omodel.SearchType > 0)
        //        {
        //            List<Guid> lstGuids = GetCustomerInfo_ChildList(Statuslst, SiteTypelst, omodel.SearchText, omodel.SearchType, Guid.Empty);
        //            iretval = true;
        //            #region "Search Data"
        //            if (omodel.SearchType == 1)
        //            {
        //                #region "Company Name Search"
        //                if (ExistOrNotCustomer(omodel.UserId))
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == id && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && e.CompanyName.ToString().ToLower().Contains(omodel.SearchText.ToLower()) && cli.CustomerOfficeId == omodel.UserId
        //                            select new TestModel1 { e = e, cli = cli }).ToList();
        //                }
        //                else
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == id && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && e.CompanyName.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                            select new TestModel1 { e = e, cli = cli }).ToList();
        //                }
        //                #endregion
        //            }
        //            else if (omodel.SearchType == 2)
        //            {
        //                #region "User ID Search"
        //                if (ExistOrNotCustomer(omodel.UserId))
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == id && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && cli.CrossLinkUserId.ToString().ToLower().Contains(omodel.SearchText.ToLower()) && cli.CustomerOfficeId == omodel.UserId
        //                            select new TestModel1 { e = e, cli = cli }).ToList();
        //                }
        //                else
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == id && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && cli.CrossLinkUserId.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                            select new TestModel1 { e = e, cli = cli }).ToList();
        //                }
        //                #endregion
        //            }
        //            else if (omodel.SearchType == 3)
        //            {
        //                #region "EFIN Search"
        //                if (ExistOrNotCustomer(omodel.UserId))
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == id && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && cli.EFIN.ToString().ToLower().Contains(omodel.SearchText.ToLower()) && cli.CustomerOfficeId == omodel.UserId
        //                            select new TestModel1 { e = e, cli = cli }).ToList();
        //                }
        //                else
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == id && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && cli.EFIN.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                            select new TestModel1 { e = e, cli = cli }).ToList();
        //                }
        //                #endregion
        //            }
        //            else if (omodel.SearchType == 4)
        //            {
        //                #region "Alternative Contact Search"
        //                if (ExistOrNotCustomer(omodel.UserId))
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == id && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && e.AlternativeContact.ToString().ToLower().Contains(omodel.SearchText.ToLower()) && cli.CustomerOfficeId == omodel.UserId
        //                            select new TestModel1 { e = e, cli = cli }).ToList();
        //                }
        //                else
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == id && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && e.AlternativeContact.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                            select new TestModel1 { e = e, cli = cli }).ToList();
        //                }
        //                #endregion
        //            }
        //            else if (omodel.SearchType == 5)
        //            {
        //                #region "Alternative Phone Search"
        //                if (ExistOrNotCustomer(omodel.UserId))
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == id && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && e.AlternatePhone.ToString().ToLower().Contains(omodel.SearchText.ToLower()) && cli.CustomerOfficeId == omodel.UserId
        //                            select new TestModel1 { e = e, cli = cli }).ToList();
        //                }
        //                else
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == id && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && e.AlternatePhone.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                            select new TestModel1 { e = e, cli = cli }).ToList();
        //                }
        //                #endregion
        //            }
        //            #endregion

        //            if (lstGuids.Count() > 0)
        //            {
        //                iretval = true;
        //                var data1 = (from e in db.emp_CustomerInformation
        //                             join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                             orderby e.CompanyName
        //                             where e.ParentId == id && lstGuids.Contains(e.Id) && cli.CrossLinkUserId != null
        //                              && e.StatusCode != EMPConstants.NewCustomer
        //                             select new TestModel1 { e = e, cli = cli }).ToList();

        //                data.AddRange(data1);

        //                data = data.DistinctBy(x => x.e.Id).ToList();
        //            }
        //        }
        //        if (iretval == false)
        //        {
        //            var _Statuslst1 = new List<string>();
        //            _Statuslst1.Add("ACT");
        //            _Statuslst1.Add("CRT");
        //            _Statuslst1.Add("PEA");
        //            _Statuslst1.Add("INP");

        //            data = (from e in db.emp_CustomerInformation
        //                    join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                    where e.ParentId == id &&
        //                    //cli.CrossLinkUserId != null && e.AlternativeContact != null && e.AlternatePhone != null && 
        //                    e.StatusCode != EMPConstants.NewCustomer
        //                    && _Statuslst1.Contains(e.StatusCode)
        //                    //&& e.IsActivationCompleted == 1
        //                    select new TestModel1 { e = e, cli = cli }).ToList();
        //        }

        //        #endregion



        //        //List<string> Statuslst1 = new List<string>();
        //        //if (omodel != null)
        //        //{
        //        //    Statuslst1 = !string.IsNullOrEmpty(omodel.Status) ? omodel.Status.Split(',').ToList() : null;

        //        //}

        //        //if (Statuslst1 != null)
        //        //{
        //        //    if (Statuslst1.Contains("INA"))
        //        //    {
        //        //        Statuslst1.Add("PEA");
        //        //        Statuslst1.Add("CRT");
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    Statuslst1 = new List<string>();
        //        //    Statuslst1.Add("ACT");
        //        //    Statuslst1.Add("CRT");
        //        //    Statuslst1.Add("PEA");
        //        //    Statuslst1.Add("INP");
        //        //}


        //        //var data = (from e in db.emp_CustomerInformation
        //        //            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //        //            where e.ParentId == id &&
        //        //            //cli.CrossLinkUserId != null && e.AlternativeContact != null && e.AlternatePhone != null && 
        //        //            e.StatusCode != EMPConstants.NewCustomer
        //        //            && Statuslst1.Contains(e.StatusCode)
        //        //            //&& e.IsActivationCompleted == 1
        //        //            select new { e, cli });
        //        List<CustomerModel> customerlst = new List<CustomerModel>();
        //        if (data != null)
        //        {
        //            foreach (var itm in data)
        //            {
        //                CustomerModel ocustomer = new CustomerModel();
        //                ocustomer.Id = itm.e.Id;
        //                ocustomer.ParentId = itm.e.ParentId ?? Guid.Empty;
        //                ocustomer.CompanyName = itm.e.CompanyName;
        //                ocustomer.AccountStatus = itm.e.AccountStatus;
        //                ocustomer.SalesforceParentID = itm.e.SalesforceParentID;
        //                ocustomer.MasterIdentifier = itm.e.MasterIdentifier;
        //                ocustomer.Feeder = itm.e.Feeder;
        //                ocustomer.BusinessOwnerFirstName = itm.e.BusinessOwnerFirstName;
        //                ocustomer.OfficePhone = itm.e.OfficePhone;
        //                ocustomer.AlternatePhone = itm.e.AlternatePhone;
        //                ocustomer.Primaryemail = itm.e.PrimaryEmail;
        //                ocustomer.SupportNotificationemail = itm.e.SupportNotificationEmail;
        //                ocustomer.EROType = itm.e.EROType;
        //                ocustomer.AlternativeContact = itm.e.AlternativeContact ?? "";
        //                ocustomer.EFIN = itm.e.EFIN;
        //                ocustomer.PhysicalAddress1 = itm.e.PhysicalAddress1;
        //                ocustomer.PhysicalAddress2 = itm.e.PhysicalAddress2;
        //                ocustomer.PhysicalZipcode = itm.e.PhysicalZipCode;
        //                ocustomer.PhysicalCity = itm.e.PhysicalCity;
        //                ocustomer.PhysicalState = itm.e.PhysicalState;
        //                ocustomer.ShippingAddress1 = itm.e.ShippingAddress1;
        //                ocustomer.ShippingAddress2 = itm.e.ShippingAddress2;
        //                ocustomer.ShippingZipcode = itm.e.ShippingZipCode;
        //                ocustomer.ShippingCity = itm.e.ShippingCity;
        //                ocustomer.ShippingState = itm.e.ShippingState;
        //                ocustomer.PhoneTypeId = itm.e.PhoneTypeId;
        //                ocustomer.TitleId = itm.e.TitleId;
        //                ocustomer.PhoneType = db.PhoneTypeMasters.Where(a => a.Id == itm.e.PhoneTypeId).Select(a => a.PhoneType).FirstOrDefault();
        //                ocustomer.ContactTitle = db.ContactPersonTitleMasters.Where(a => a.Id == itm.e.TitleId).Select(a => a.ContactPersonTitle).FirstOrDefault();
        //                ocustomer.EntityId = itm.e.EntityId ?? 0;
        //                ocustomer.BaseEntityId = db.EntityMasters.Where(a => a.Id == itm.e.EntityId).Select(a => a.BaseEntityId).FirstOrDefault();

        //                ocustomer.SalesYearID = itm.e.SalesYearID ?? Guid.Empty;
        //                ocustomer.StatusCode = itm.e.StatusCode;
        //                ocustomer.LoginId = itm.cli.Id.ToString();
        //                ocustomer.LoginEFIN = itm.cli.EFIN;
        //                ocustomer.MasterIdentifier = itm.cli.MasterIdentifier;
        //                ocustomer.CrossLinkUserId = itm.cli.CrossLinkUserId ?? "";
        //                ocustomer.CrossLinkPassword = itm.cli.CrossLinkPassword;
        //                ocustomer.EMPUserId = itm.cli.EMPUserId;
        //                ocustomer.EMPPassword = !string.IsNullOrEmpty(itm.cli.EMPPassword) ? PasswordManager.DecryptText(itm.cli.EMPPassword) : "";
        //                ocustomer.OfficePortalUrl = itm.cli.OfficePortalUrl;
        //                ocustomer.TaxOfficeUsername = itm.cli.TaxOfficeUsername;
        //                ocustomer.TaxOfficePassword = itm.cli.TaxOfficePassword;
        //                ocustomer.CustomerOfficeId = itm.cli.CustomerOfficeId;

        //                ocustomer.IsActivationCompleted = itm.e.IsActivationCompleted ?? 0;

        //                var SubOfficeConfig = db.SubSiteOfficeConfigs.Where(o => o.RefId == ocustomer.Id).FirstOrDefault();
        //                if (SubOfficeConfig != null)
        //                {
        //                    if (SubOfficeConfig.SOorSSorEFIN == 3)
        //                    {
        //                        ocustomer.IsAdditionalEFINAllowed = true;
        //                    }
        //                }

        //                ocustomer.CreatedDate = itm.cli.CreatedDate ?? Convert.ToDateTime("01/01/2000");
        //                ocustomer.LastUpdatedDate = itm.e.LastUpdatedDate ?? Convert.ToDateTime("01/01/2000");

        //                if (itm.e.IsActivationCompleted == null || itm.e.IsActivationCompleted == 0)
        //                    ocustomer.ActivationStatus = "Not Active";
        //                else
        //                    ocustomer.ActivationStatus = "Active";

        //                //int IsActivationCompleted = itm.e.IsActivationCompleted ?? 0;

        //                //if (IsActivationCompleted == 1)
        //                //{
        //                //    ocustomer.AccountStatus = "Active";
        //                //}
        //                //else if (ocustomer.StatusCode == EMPConstants.Created)
        //                //{
        //                //    ocustomer.AccountStatus = "Created";
        //                //}
        //                //else if (ocustomer.StatusCode == EMPConstants.NewCustomer)
        //                //{
        //                //    ocustomer.AccountStatus = "New";
        //                //}
        //                //else if (ocustomer.StatusCode == EMPConstants.InProgress)
        //                //{
        //                //    ocustomer.AccountStatus = "In Progress";
        //                //}
        //                //else
        //                //{
        //                //    ocustomer.AccountStatus = "Not Active";
        //                //}

        //                ocustomer.IsEnrolled = itm.e.IsEnrolled ?? false;
        //                ocustomer.EnrolledBankId = itm.e.EnrolledBankId;

        //                if (ocustomer.IsEnrolled)
        //                {
        //                    var BankFees = BankFee(itm.e.Id, id, itm.e.CompanyName, companyname, 1, itm.e.IsMSOUser ?? false);
        //                    foreach (var fees in BankFees)
        //                    {
        //                        if (fees.FeeFor == (int)EMPConstants.FeesFor.SVBFees)
        //                        {
        //                            ocustomer.TotalServiceFee = ocustomer.TotalServiceFee + fees.Amount;
        //                            ocustomer.ServiceTooltip = ocustomer.ServiceTooltip + fees.FeesName;
        //                        }

        //                        if (fees.FeeFor == (int)EMPConstants.FeesFor.TransmissionFees)
        //                        {
        //                            ocustomer.TotalTransFee = ocustomer.TotalTransFee + fees.Amount;
        //                            ocustomer.TransTooltip = ocustomer.TransTooltip + fees.FeesName;
        //                        }
        //                    }
        //                }

        //                #region "Actions"
        //                var actions = (from s in db.EMP_ActionMaser
        //                               where s.EntityId == itm.e.EntityId && s.Status == EMPConstants.Active
        //                               orderby s.DisplayOrder
        //                               select new Actions
        //                               {
        //                                   IsParent = s.ParentId,
        //                                   Name = s.Name,
        //                                   Display = s.ForActive
        //                               }).ToList();

        //                ocustomer.Actions = actions;
        //                #endregion

        //                //if (ocustomer.IsAdditionalEFINAllowed)
        //                //{
        //                ocustomer.ChaildCustomerInfo = new List<CustomerInformation.CustomerModel>();
        //                //ocustomer.ChaildCustomerInfo = GetChildCustomerInfo_AdditionalEFIN(itm.e.Id, id, itm.e.CompanyName, companyname, Statuslst, SiteTypelst, SearchText, SearchType, LoginUserId);
        //                ocustomer.ChaildCustomerInfo = GetChildCustomerInfo(itm.e.Id, "", null, null, "", 0, LoginUserId, omodel);
        //                //}

        //                if (itm.e.IsActivationCompleted == 1)
        //                {
        //                    var rParent = Guid.Empty;
        //                    var EntityHierarchyDTOs = new DropDownService().GetEntityHierarchies(ocustomer.Id);
        //                    if (EntityHierarchyDTOs.Count <= 0)
        //                        rParent = id;
        //                    else
        //                    {
        //                        var rootParent = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
        //                        rParent = rootParent.CustomerId.Value;
        //                    }
        //                    if (rParent == LoginUserId)
        //                    {
        //                        var yesenroll = db.SubSiteConfigurations.Where(x => x.emp_CustomerInformation_ID == rParent).Select(x => x).FirstOrDefault();
        //                        if (yesenroll != null)
        //                        {
        //                            bool utaxmanage = yesenroll.IsuTaxManageingEnrolling.HasValue ? yesenroll.IsuTaxManageingEnrolling.Value : false;
        //                            if (!utaxmanage)
        //                            {
        //                                var isUtaxenroll = db.SubSiteConfigurations.Where(x => x.emp_CustomerInformation_ID == LoginUserId).Select(x => x.IsuTaxPortalEnrollment).FirstOrDefault();
        //                                ocustomer.IsActivated = true;
        //                            }
        //                            else
        //                                ocustomer.IsActivated = true;
        //                        }
        //                        else
        //                            ocustomer.IsActivated = false;

        //                        //var isUtaxenroll = db.SubSiteConfigurations.Where(x => x.emp_CustomerInformation_ID == LoginUserId).Select(x => x.IsuTaxPortalEnrollment).FirstOrDefault();
        //                        //ocustomer.IsActivated = isUtaxenroll.HasValue ? isUtaxenroll.Value : false;
        //                        //ocustomer.IsActivated = true;
        //                    }
        //                    else
        //                    {
        //                        var isUtaxenroll = db.SubSiteConfigurations.Where(x => x.emp_CustomerInformation_ID == rParent).Select(x => x.IsuTaxManageingEnrolling).FirstOrDefault();
        //                        ocustomer.IsActivated = isUtaxenroll.HasValue ? isUtaxenroll.Value : true;
        //                    }
        //                }

        //                EnrollmentBankSelectionService serv = new EnrollmentBankSelectionInfo.EnrollmentBankSelectionService();
        //                var enrdata = serv.getEnrollmentStatusInfo(ocustomer.Id);
        //                ocustomer.ActiveBank = enrdata.BankName;
        //                ocustomer.SubmissionDate = enrdata.SubmitedDate;
        //                ocustomer.EnrollmentStatus = enrdata.SubmissionStaus;
        //                ocustomer.ApprovedBank = enrdata.ApprovedBank;
        //                ocustomer.RejectedBanks = enrdata.RejectedBanks;
        //                ocustomer.UnlockedBanks = enrdata.UnlockedBanks;

        //                customerlst.Add(ocustomer);
        //            }

        //            if (Statuslst != null)
        //            {
        //                customerlst = customerlst.Where(o => Statuslst.Contains(o.StatusCode)).ToList();
        //            }
        //            if (SiteTypelst != null)
        //            {
        //                customerlst = customerlst.Where(o => SiteTypelst.Contains(o.EntityId.ToString())).ToList();
        //            }
        //            if (SearchType > 0)
        //            {
        //                if (SearchType == 1)
        //                {
        //                    customerlst = customerlst.Where(o => o.CompanyName.ToString().ToLower().Contains(SearchText.ToLower())).ToList();
        //                }
        //                else if (SearchType == 2)
        //                {
        //                    customerlst = customerlst.Where(o => o.CrossLinkUserId != null ? o.CrossLinkUserId.ToString().ToLower().Contains(SearchText.ToLower()) : o.CrossLinkUserId == o.CrossLinkUserId).ToList();
        //                }
        //                else if (SearchType == 3)
        //                {
        //                    customerlst = customerlst.Where(o => o.EFIN.ToString().ToLower().Contains(SearchText.ToLower())).ToList();
        //                }
        //                else if (SearchType == 4)
        //                {
        //                    customerlst = customerlst.Where(o => o.AlternativeContact != null ? o.AlternativeContact.ToString().ToLower().Contains(SearchText.ToLower()) : o.AlternativeContact == o.AlternativeContact).ToList();
        //                }
        //                else if (SearchType == 5)
        //                {
        //                    customerlst = customerlst.Where(o => o.AlternatePhone != null ? o.AlternatePhone.ToString().ToLower().Contains(SearchText.ToLower()) : o.AlternatePhone == o.AlternatePhone).ToList();
        //                }
        //            }
        //        }
        //        return customerlst;
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetChildCustomerInfo", id);
        //        return new List<CustomerModel>();
        //    }
        //}

        //public List<CustomerModel> GetChildCustomerInfo_AdditionalEFIN(Guid id, Guid ParentId, string companyname, string ParentCompanyname, List<string> Statuslst, List<string> SiteTypelst, string SearchText, int SearchType, Guid LoginUserId)
        //{
        //    try
        //    {

        //        var data = (from e in db.emp_CustomerInformation
        //                    join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                    where e.ParentId == id //&& cli.CrossLinkUserId != null && e.AlternativeContact != null && e.AlternatePhone != null 
        //                    && e.StatusCode != EMPConstants.NewCustomer
        //                    && e.IsActivationCompleted == 1
        //                    select new { e, cli });
        //        List<CustomerModel> customerlst = new List<CustomerModel>();
        //        if (data != null)
        //        {
        //            foreach (var itm in data)
        //            {
        //                CustomerModel ocustomer = new CustomerModel();
        //                ocustomer.Id = itm.e.Id;
        //                ocustomer.ParentId = itm.e.ParentId ?? Guid.Empty;
        //                ocustomer.CompanyName = itm.e.CompanyName;
        //                ocustomer.AccountStatus = itm.e.AccountStatus;
        //                ocustomer.Feeder = itm.e.Feeder;
        //                ocustomer.BusinessOwnerFirstName = itm.e.BusinessOwnerFirstName;
        //                ocustomer.OfficePhone = itm.e.OfficePhone;
        //                ocustomer.AlternatePhone = itm.e.AlternatePhone;
        //                ocustomer.Primaryemail = itm.e.PrimaryEmail;
        //                ocustomer.SupportNotificationemail = itm.e.SupportNotificationEmail;
        //                ocustomer.EROType = itm.e.EROType;
        //                ocustomer.AlternativeContact = itm.e.AlternativeContact ?? "";
        //                ocustomer.EFIN = itm.e.EFIN;
        //                ocustomer.PhysicalAddress1 = itm.e.PhysicalAddress1;
        //                ocustomer.PhysicalAddress2 = itm.e.PhysicalAddress2;
        //                ocustomer.PhysicalZipcode = itm.e.PhysicalZipCode;
        //                ocustomer.PhysicalCity = itm.e.PhysicalCity;
        //                ocustomer.PhysicalState = itm.e.PhysicalState;
        //                ocustomer.ShippingAddress1 = itm.e.ShippingAddress1;
        //                ocustomer.ShippingAddress2 = itm.e.ShippingAddress2;
        //                ocustomer.ShippingZipcode = itm.e.ShippingZipCode;
        //                ocustomer.ShippingCity = itm.e.ShippingCity;
        //                ocustomer.ShippingState = itm.e.ShippingState;
        //                ocustomer.PhoneTypeId = itm.e.PhoneTypeId;
        //                ocustomer.TitleId = itm.e.TitleId;
        //                ocustomer.PhoneType = db.PhoneTypeMasters.Where(a => a.Id == itm.e.PhoneTypeId).Select(a => a.PhoneType).FirstOrDefault();
        //                ocustomer.ContactTitle = db.ContactPersonTitleMasters.Where(a => a.Id == itm.e.TitleId).Select(a => a.ContactPersonTitle).FirstOrDefault();
        //                ocustomer.EntityId = itm.e.EntityId ?? 0;
        //                ocustomer.BaseEntityId = db.EntityMasters.Where(a => a.Id == itm.e.EntityId).Select(a => a.BaseEntityId).FirstOrDefault();

        //                ocustomer.SalesYearID = itm.e.SalesYearID ?? Guid.Empty;
        //                ocustomer.StatusCode = itm.e.StatusCode;
        //                ocustomer.LoginId = itm.cli.Id.ToString();
        //                ocustomer.LoginEFIN = itm.cli.EFIN;
        //                ocustomer.MasterIdentifier = itm.cli.MasterIdentifier;
        //                ocustomer.CrossLinkUserId = itm.cli.CrossLinkUserId;
        //                ocustomer.CrossLinkPassword = itm.cli.CrossLinkPassword != null ? PasswordManager.DecryptText(itm.cli.CrossLinkPassword.ToString()) : "";  //itm.cli.CrossLinkPassword;
        //                ocustomer.EMPUserId = itm.cli.EMPUserId;
        //                ocustomer.EMPPassword = itm.cli.EMPPassword != null ? PasswordManager.DecryptText(itm.cli.EMPPassword.ToString()) : ""; //itm.cli.EMPPassword;
        //                ocustomer.OfficePortalUrl = itm.cli.OfficePortalUrl;
        //                ocustomer.TaxOfficeUsername = itm.cli.TaxOfficeUsername;
        //                ocustomer.TaxOfficePassword = itm.cli.TaxOfficePassword != null ? PasswordManager.DecryptText(itm.cli.TaxOfficePassword.ToString()) : ""; //itm.cli.TaxOfficePassword;
        //                ocustomer.CustomerOfficeId = itm.cli.CustomerOfficeId;
        //                ocustomer.IsAdditionalEFINAllowed = itm.e.IsAdditionalEFINAllowed ?? false;
        //                ocustomer.IsActivationCompleted = itm.e.IsActivationCompleted ?? 0;
        //                ocustomer.SalesforceParentID = itm.e.SalesforceParentID;

        //                if (itm.e.IsActivationCompleted == null || itm.e.IsActivationCompleted == 0)
        //                    ocustomer.ActivationStatus = "Not Active";
        //                else
        //                    ocustomer.ActivationStatus = "Active";

        //                EnrollmentBankSelectionService serv = new EnrollmentBankSelectionInfo.EnrollmentBankSelectionService();
        //                var enrdata = serv.getEnrollmentStatusInfo(ocustomer.Id);
        //                ocustomer.ActiveBank = enrdata.BankName;
        //                ocustomer.SubmissionDate = enrdata.SubmitedDate;
        //                ocustomer.EnrollmentStatus = enrdata.SubmissionStaus;
        //                ocustomer.ApprovedBank = enrdata.ApprovedBank;
        //                ocustomer.RejectedBanks = enrdata.RejectedBanks;
        //                ocustomer.UnlockedBanks = enrdata.UnlockedBanks;

        //                //var CustFees = CustomerFees(id);
        //                //foreach (var fees in CustFees)
        //                //{
        //                //    if (fees.FeeFor == (int)EMPConstants.FeesFor.SVBFees)
        //                //    {
        //                //        ocustomer.TotalServiceFee = fees.Amount;
        //                //        ocustomer.ServiceTooltip = fees.FeesName;
        //                //    }

        //                //    if (fees.FeeFor == (int)EMPConstants.FeesFor.TransmissionFees)
        //                //    {
        //                //        ocustomer.TotalTransFee = fees.Amount;
        //                //        ocustomer.TransTooltip = fees.FeesName;
        //                //    }
        //                //}
        //                ocustomer.IsEnrolled = itm.e.IsEnrolled ?? false;
        //                ocustomer.EnrolledBankId = itm.e.EnrolledBankId;

        //                if (ocustomer.IsEnrolled)
        //                {
        //                    var BankFees = BankFee(itm.e.Id, ParentId, itm.e.CompanyName, ParentCompanyname, 1, itm.e.IsMSOUser ?? false);
        //                    foreach (var fees in BankFees)
        //                    {
        //                        if (fees.FeeFor == (int)EMPConstants.FeesFor.SVBFees)
        //                        {
        //                            ocustomer.TotalServiceFee = ocustomer.TotalServiceFee + fees.Amount;
        //                            ocustomer.ServiceTooltip = ocustomer.ServiceTooltip + fees.FeesName;
        //                        }

        //                        if (fees.FeeFor == (int)EMPConstants.FeesFor.TransmissionFees)
        //                        {
        //                            ocustomer.TotalTransFee = ocustomer.TotalTransFee + fees.Amount;
        //                            ocustomer.TransTooltip = ocustomer.TransTooltip + fees.FeesName;
        //                        }
        //                    }
        //                }

        //                if (itm.e.IsActivationCompleted == 1)
        //                {
        //                    if (ParentId == LoginUserId)
        //                    {
        //                        var isUtaxenroll = db.SubSiteConfigurations.Where(x => x.emp_CustomerInformation_ID == ParentId).Select(x => x.IsuTaxPortalEnrollment).FirstOrDefault();
        //                        //ocustomer.IsActivated = isUtaxenroll.HasValue ? isUtaxenroll.Value : false;
        //                        ocustomer.IsActivated = true;
        //                    }
        //                    else
        //                    {
        //                        var isUtaxenroll = db.SubSiteConfigurations.Where(x => x.emp_CustomerInformation_ID == ParentId).Select(x => x.IsuTaxManageingEnrolling).FirstOrDefault();
        //                        ocustomer.IsActivated = isUtaxenroll.HasValue ? isUtaxenroll.Value : false;
        //                    }
        //                }

        //                customerlst.Add(ocustomer);
        //            }

        //            if (Statuslst != null)
        //            {
        //                customerlst = customerlst.Where(o => Statuslst.Contains(o.StatusCode)).ToList();
        //            }
        //            if (SiteTypelst != null)
        //            {
        //                customerlst = customerlst.Where(o => SiteTypelst.Contains(o.EntityId.ToString())).ToList();
        //            }
        //            if (SearchType > 0)
        //            {
        //                if (SearchType == 1)
        //                {
        //                    customerlst = customerlst.Where(o => o.CompanyName.ToString().ToLower().Contains(SearchText.ToLower())).ToList();
        //                }
        //                else if (SearchType == 2)
        //                {
        //                    customerlst = customerlst.Where(o => o.CrossLinkUserId != null ? o.CrossLinkUserId.ToString().ToLower().Contains(SearchText.ToLower()) : o.CrossLinkUserId == o.CrossLinkUserId).ToList();
        //                }
        //                else if (SearchType == 3)
        //                {
        //                    customerlst = customerlst.Where(o => o.EFIN.ToString().ToLower().Contains(SearchText.ToLower())).ToList();
        //                }
        //                else if (SearchType == 4)
        //                {
        //                    customerlst = customerlst.Where(o => o.AlternativeContact != null ? o.AlternativeContact.ToString().ToLower().Contains(SearchText.ToLower()) : o.AlternativeContact == o.AlternativeContact).ToList();
        //                }
        //                else if (SearchType == 5)
        //                {
        //                    customerlst = customerlst.Where(o => o.AlternatePhone != null ? o.AlternatePhone.ToString().ToLower().Contains(SearchText.ToLower()) : o.AlternatePhone == o.AlternatePhone).ToList();
        //                }
        //            }
        //        }

        //        return customerlst;

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetChildCustomerInfo_AdditionalEFIN", id);
        //        return new List<CustomerModel>();
        //    }
        //}

        ///Not In Used
        public int SaveCustomerSubSiteInfo(CustomerInformationModel model, Guid ParentId)
        {
            try
            {
                if (model != null)
                {
                    var isExist = db.emp_CustomerInformation.Any(a => a.EFIN == model.EFIN);

                    if (!isExist)
                    {

                        emp_CustomerInformation emp_Customerinfo = new emp_CustomerInformation();
                        emp_Customerinfo.Id = Guid.NewGuid();
                        emp_Customerinfo.CompanyName = model.CompanyName;
                        emp_Customerinfo.ParentId = model.ParentId;
                        emp_Customerinfo.AccountStatus = "Active";

                        var Entity = (from c in db.emp_CustomerInformation
                                      join e in db.EntityMasters on c.EntityId equals e.ParentId
                                      where c.Id == model.ParentId
                                      select new { e.BaseEntityId, e.Id, e.Name }).FirstOrDefault();

                        emp_Customerinfo.StatusCode = EMPConstants.NewCustomer;

                        if (Entity == null)
                        {
                            Entity = (from e in db.EntityMasters
                                      where e.BaseEntityId == (int)EMPConstants.BaseEntities.AE
                                      select new { e.BaseEntityId, e.Id, e.Name }).FirstOrDefault();

                            emp_Customerinfo.StatusCode = EMPConstants.Created;
                        }

                        emp_Customerinfo.MasterIdentifier = model.MasterIdentifier;
                        emp_Customerinfo.SalesforceParentID = db.emp_CustomerLoginInformation.Where(a => a.CustomerOfficeId == model.ParentId).Select(a => a.CrossLinkUserId).FirstOrDefault();

                        emp_Customerinfo.EntityId = Entity.Id;
                        emp_Customerinfo.Feeder = true;
                        emp_Customerinfo.BusinessOwnerFirstName = model.BusinessOwnerFirstName;
                        emp_Customerinfo.BusinesOwnerLastName = model.BusinessOwnerLastName;
                        emp_Customerinfo.OfficePhone = model.OfficePhone;
                        emp_Customerinfo.AlternatePhone = model.AlternatePhone;
                        emp_Customerinfo.PrimaryEmail = model.Primaryemail;

                        // if (EntName == 6)
                        emp_Customerinfo.EROType = Entity.Name;
                        // else if (EntName == 4)
                        //     emp_Customerinfo.EROType = "Mo Sub Office";
                        emp_Customerinfo.AlternativeContact = model.AlternativeContact;
                        emp_Customerinfo.EFIN = model.EFIN;
                        emp_Customerinfo.EFINStatus = model.EFINStatus;
                        emp_Customerinfo.PhysicalAddress1 = model.PhysicalAddress1;
                        emp_Customerinfo.PhysicalZipCode = model.PhysicalZipcode;
                        emp_Customerinfo.PhysicalCity = model.PhysicalCity;
                        emp_Customerinfo.PhysicalState = model.PhysicalState;
                        emp_Customerinfo.PhoneTypeId = Guid.Empty;

                        var SalesYear = db.SalesYearMasters.Where(o => o.ApplicableToDate == null).FirstOrDefault();
                        if (SalesYear != null)
                        {
                            emp_Customerinfo.SalesYearID = SalesYear.Id;
                            emp_Customerinfo.SalesYearGroupId = emp_Customerinfo.Id;
                        }

                        emp_Customerinfo.CreatedBy = model.UserId ?? Guid.Empty;
                        emp_Customerinfo.CreatedDate = DateTime.Now;
                        emp_Customerinfo.LastUpdatedDate = DateTime.Now;
                        emp_Customerinfo.LastUpdatedBy = model.UserId ?? Guid.Empty;

                        try
                        {
                            db.emp_CustomerInformation.Add(emp_Customerinfo);

                            emp_CustomerLoginInformation emp_CustomerLoginInfo = new emp_CustomerLoginInformation();
                            emp_CustomerLoginInfo.Id = Guid.NewGuid();
                            emp_CustomerLoginInfo.CustomerOfficeId = emp_Customerinfo.Id;
                            // emp_CustomerLoginInfo.EFIN = model.EFIN;

                            if (Entity.BaseEntityId == (int)EMPConstants.BaseEntities.AE)
                            {
                                var parentdata = db.emp_CustomerInformation
           .Join(db.emp_CustomerLoginInformation, custinfo => custinfo.Id, custlog => custlog.CustomerOfficeId,
                               (custinfo, custlog) => new { custinfo, custlog })
           .Where(o => o.custinfo.Id == model.ParentId)
           //.GroupBy(x => new {
           //    x.fee.FeesFor,
           //    x.fee.Name
           //})
           .Select(g => new
           {
               Id = g.custinfo.Id,
               CrossLinkUserId = g.custlog.CrossLinkUserId,
           }
         ).FirstOrDefault();


                                if (parentdata != null)
                                {
                                    var NewUser = db.emp_CustomerLoginInformation.Where(o => o.CrossLinkUserId == parentdata.CrossLinkUserId).OrderByDescending(o => o.LastUpdatedDate).Take(1).SingleOrDefault();


                                    var parentdata2 = db.emp_CustomerInformation
         .Join(db.emp_CustomerLoginInformation, custinfo => custinfo.Id, custlog => custlog.CustomerOfficeId,
                             (custinfo, custlog) => new { custinfo, custlog })
         .Where(o => o.custinfo.ParentId == model.ParentId)
         .Select(g => new
         {
             CrossLinkUserId = g.custlog.CrossLinkUserId,
         }
       ).ToList();
                                    string UserIdChar = "A";
                                    if (parentdata2.ToList().Count > 0)
                                    {
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "A")).ToList().Count > 0)
                                        {
                                            UserIdChar = "B";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "B")).ToList().Count > 0)
                                        {
                                            UserIdChar = "C";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "C")).ToList().Count > 0)
                                        {
                                            UserIdChar = "D";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "D")).ToList().Count > 0)
                                        {
                                            UserIdChar = "E";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "E")).ToList().Count > 0)
                                        {
                                            UserIdChar = "F";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "F")).ToList().Count > 0)
                                        {
                                            UserIdChar = "G";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "G")).ToList().Count > 0)
                                        {
                                            UserIdChar = "H";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "H")).ToList().Count > 0)
                                        {
                                            UserIdChar = "I";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "I")).ToList().Count > 0)
                                        {
                                            UserIdChar = "J";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "J")).ToList().Count > 0)
                                        {
                                            UserIdChar = "K";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "K")).ToList().Count > 0)
                                        {
                                            UserIdChar = "L";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "L")).ToList().Count > 0)
                                        {
                                            UserIdChar = "M";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "M")).ToList().Count > 0)
                                        {
                                            UserIdChar = "N";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "N")).ToList().Count > 0)
                                        {
                                            UserIdChar = "O";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "O")).ToList().Count > 0)
                                        {
                                            UserIdChar = "P";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "P")).ToList().Count > 0)
                                        {
                                            UserIdChar = "Q";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "Q")).ToList().Count > 0)
                                        {
                                            UserIdChar = "R";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "R")).ToList().Count > 0)
                                        {
                                            UserIdChar = "S";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "S")).ToList().Count > 0)
                                        {
                                            UserIdChar = "T";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "T")).ToList().Count > 0)
                                        {
                                            UserIdChar = "U";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "U")).ToList().Count > 0)
                                        {
                                            UserIdChar = "V";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "V")).ToList().Count > 0)
                                        {
                                            UserIdChar = "W";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "W")).ToList().Count > 0)
                                        {
                                            UserIdChar = "X";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "X")).ToList().Count > 0)
                                        {
                                            UserIdChar = "Y";
                                        }
                                        if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "Y")).ToList().Count > 0)
                                        {
                                            UserIdChar = "Z";
                                        }
                                    }

                                    emp_CustomerLoginInfo.CrossLinkUserId = parentdata.CrossLinkUserId + UserIdChar;
                                    emp_CustomerLoginInfo.EMPUserId = parentdata.CrossLinkUserId + UserIdChar;

                                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                                    var stringChars = new char[8];
                                    var random = new Random();

                                    for (int i = 0; i < stringChars.Length; i++)
                                    {
                                        stringChars[i] = chars[random.Next(chars.Length)];
                                    }

                                    var finalString = new String(stringChars);

                                    string Password = PasswordManager.CryptText(finalString);
                                    emp_CustomerLoginInfo.CrossLinkPassword = Password;
                                    emp_CustomerLoginInfo.EMPPassword = Password;
                                }
                                else
                                {
                                    return -2;
                                }
                            }

                            emp_CustomerLoginInfo.StatusCode = EMPConstants.Active;
                            emp_CustomerLoginInfo.CreatedBy = model.UserId ?? Guid.Empty;
                            emp_CustomerLoginInfo.CreatedDate = DateTime.Now;
                            emp_CustomerLoginInfo.LastUpdatedDate = DateTime.Now;
                            emp_CustomerLoginInfo.LastUpdatedBy = model.UserId ?? Guid.Empty;
                            emp_CustomerLoginInfo.OfficePortalUrl = model.OfficePortalUrl;
                            db.emp_CustomerLoginInformation.Add(emp_CustomerLoginInfo);
                            db.SaveChanges();
                            db.Dispose();
                            return 1;
                        }
                        catch (Exception)
                        {
                            return 0;
                        }

                    }
                    else
                    {
                        return -1;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/SaveCustomerSubSiteInfo", model.UserId);
                return 0;
            }
        }

        public int SaveCustomerconfigStatus(Guid CustomerId, Guid UserId, string SiteMapID)
        {
            int res = 0;
            try
            {
                db = new DatabaseEntities();
                Guid SiteMapId;
                if (!Guid.TryParse(SiteMapID, out SiteMapId))
                {
                    return -1;
                }

                var Exist = db.CustomerConfigurationStatus.Any(a => a.CustomerId == CustomerId && a.SitemapId == SiteMapId);
                if (Exist == false)
                {
                    CustomerConfigurationStatu ConfigStatusModel = new CustomerConfigurationStatu();
                    ConfigStatusModel.Id = Guid.NewGuid();
                    ConfigStatusModel.CustomerId = CustomerId;
                    ConfigStatusModel.SitemapId = SiteMapId;
                    ConfigStatusModel.StatusCode = "done";
                    ConfigStatusModel.UpdatedBy = UserId;
                    ConfigStatusModel.UpdatedDate = DateTime.Now;
                    db.CustomerConfigurationStatus.Add(ConfigStatusModel);
                    db.SaveChanges();
                    res = 1;

                    bool iscrosslink = false;
                    var empCustInfo = db.emp_CustomerInformation.Where(o => o.Id == CustomerId).FirstOrDefault();
                    var loginfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                    if (loginfo != null)
                    {
                        iscrosslink = string.IsNullOrEmpty(loginfo.EMPPassword);
                    }
                    else
                        iscrosslink = true;

                    if (empCustInfo != null && iscrosslink)
                    {
                        empCustInfo.IsActivationCompleted = 1;

                        CreateNewUserResponse newuser = new CreateNewUserResponse();
                        //if (empCustInfo.ParentId.HasValue)
                        //{
                        CreateNewUserModel userModel = new CreateNewUserModel();
                        userModel.MasterIdentifier = "";
                        userModel.AddBusinessProduct = false;
                        userModel.CompanyName = empCustInfo.CompanyName;
                        userModel.Email = empCustInfo.PrimaryEmail;
                        userModel.Fname = empCustInfo.BusinessOwnerFirstName;
                        userModel.Lname = empCustInfo.BusinessOwnerFirstName;
                        userModel.Phone = empCustInfo.OfficePhone;
                        userModel.ShippingAddress = empCustInfo.ShippingAddress1;
                        userModel.ShippingCity = empCustInfo.ShippingCity;
                        userModel.ShippingState = empCustInfo.ShippingState;
                        userModel.ShippingZip = empCustInfo.ShippingZipCode;
                        userModel.ParentCustomerId = empCustInfo.ParentId;
                        userModel.CustomerId = empCustInfo.Id;
                        newuser = CreateNewUser(userModel);
                        if (!newuser.Status)
                        {
                            res = 3;
                        }
                        //}

                        if (newuser.Status)
                        {
                            if (!string.IsNullOrEmpty(newuser.Password))
                                empCustInfo.StatusCode = EMPConstants.Active;
                            else
                                empCustInfo.StatusCode = EMPConstants.Created;

                            if (loginfo != null)
                            {
                                loginfo.CrossLinkUserId = newuser.UserId;
                                string Password = string.IsNullOrEmpty(newuser.Password) ? "" : PasswordManager.CryptText(newuser.Password);
                                loginfo.CrossLinkPassword = Password;
                                loginfo.EMPPassword = Password;
                                loginfo.EMPUserId = newuser.UserId;
                                loginfo.StatusCode = EMPConstants.Active;
                            }
                            else
                            {
                                emp_CustomerLoginInformation newlogin = new emp_CustomerLoginInformation();
                                newlogin.Id = Guid.NewGuid();
                                //newlogin.EFIN = empCustInfo.EFIN;
                                newlogin.MasterIdentifier = empCustInfo.MasterIdentifier;
                                newlogin.OfficePortalUrl = "https://www.mytaxofficeportal.com";
                                newlogin.CreatedBy = UserId;
                                newlogin.CreatedDate = System.DateTime.Now;
                                newlogin.CustomerOfficeId = empCustInfo.Id;
                                newlogin.LastUpdatedBy = UserId;
                                newlogin.LastUpdatedDate = System.DateTime.Now;
                                newlogin.CrossLinkUserId = newuser.UserId;
                                string Password = string.IsNullOrEmpty(newuser.Password) ? "" : PasswordManager.CryptText(newuser.Password);
                                newlogin.CrossLinkPassword = Password;
                                newlogin.EMPPassword = Password;
                                newlogin.EMPUserId = newuser.UserId;
                                newlogin.StatusCode = EMPConstants.Active;
                                db.emp_CustomerLoginInformation.Add(newlogin);
                            }
                            SendEmailForNewUser(new NewUserEmailRequest() { CustomerId = CustomerId, UserId = CustomerId });

                        }
                        db.SaveChanges();
                        if (newuser.Status)
                        {
                            if (!string.IsNullOrEmpty(newuser.Password))
                                res = 2;
                            else
                                res = 4;
                        }
                    }
                    else if (empCustInfo != null)
                    {
                        if (res == 2 || !empCustInfo.ParentId.HasValue)
                        {
                            empCustInfo.IsActivationCompleted = 1;
                            empCustInfo.StatusCode = EMPConstants.Active;
                            empCustInfo.AccountStatus = "Active";
                        }
                        else if (res == 4 || !empCustInfo.ParentId.HasValue)
                        {
                            empCustInfo.IsActivationCompleted = 1;
                            empCustInfo.StatusCode = EMPConstants.Created;
                            empCustInfo.AccountStatus = "Active";
                        }
                        else if (empCustInfo.StatusCode == EMPConstants.InProgress)
                        {
                            empCustInfo.IsActivationCompleted = 0;
                            empCustInfo.StatusCode = EMPConstants.NewCustomer;
                            empCustInfo.AccountStatus = "Not Active";
                        }
                        else if (empCustInfo.StatusCode == EMPConstants.Created || empCustInfo.StatusCode == EMPConstants.Pending)
                        {
                            empCustInfo.IsActivationCompleted = 1;
                            empCustInfo.StatusCode = EMPConstants.Active;
                            empCustInfo.AccountStatus = "Active";
                        }
                        db.Entry(empCustInfo).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    var loginfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                    if (loginfo != null)
                    {
                        var empCustInfo = db.emp_CustomerInformation.Where(o => o.Id == CustomerId).FirstOrDefault();

                        if (string.IsNullOrEmpty(loginfo.EMPPassword))
                        {
                            if (empCustInfo != null)
                            {
                                empCustInfo.IsActivationCompleted = 1;

                                CreateNewUserResponse newuser = new CreateNewUserResponse();
                                //if (empCustInfo.ParentId.HasValue)
                                //{
                                CreateNewUserModel userModel = new CreateNewUserModel();
                                userModel.MasterIdentifier = "";
                                userModel.AddBusinessProduct = false;
                                userModel.CompanyName = empCustInfo.CompanyName;
                                userModel.Email = empCustInfo.PrimaryEmail;
                                userModel.Fname = empCustInfo.BusinessOwnerFirstName;
                                userModel.Lname = empCustInfo.BusinessOwnerFirstName;
                                userModel.Phone = empCustInfo.OfficePhone;
                                userModel.ShippingAddress = empCustInfo.ShippingAddress1;
                                userModel.ShippingCity = empCustInfo.ShippingCity;
                                userModel.ShippingState = empCustInfo.ShippingState;
                                userModel.ShippingZip = empCustInfo.ShippingZipCode;
                                userModel.ParentCustomerId = empCustInfo.ParentId.Value;
                                userModel.CustomerId = empCustInfo.Id;
                                newuser = CreateNewUser(userModel);
                                if (!newuser.Status)
                                {
                                    res = 3;
                                }
                                //}

                                if (newuser.Status)
                                {
                                    if (!string.IsNullOrEmpty(newuser.Password))
                                        empCustInfo.StatusCode = EMPConstants.Active;
                                    else
                                        empCustInfo.StatusCode = EMPConstants.Created;

                                    loginfo.CrossLinkUserId = newuser.UserId;
                                    string Password = string.IsNullOrEmpty(newuser.Password) ? "" : PasswordManager.CryptText(newuser.Password);
                                    loginfo.CrossLinkPassword = Password;
                                    loginfo.EMPPassword = Password;
                                    loginfo.EMPUserId = newuser.UserId;
                                    loginfo.StatusCode = EMPConstants.Active;

                                    SendEmailForNewUser(new NewUserEmailRequest() { CustomerId = CustomerId, UserId = CustomerId });

                                }
                                db.SaveChanges();
                                if (newuser.Status)
                                {
                                    if (!string.IsNullOrEmpty(newuser.Password))
                                        res = 2;
                                    else
                                        res = 4;
                                }
                            }
                        }
                        else
                        {
                            if (empCustInfo.StatusCode == EMPConstants.Created || empCustInfo.StatusCode == EMPConstants.Pending)
                            {
                                empCustInfo.IsActivationCompleted = 1;
                                empCustInfo.StatusCode = EMPConstants.Active;
                                empCustInfo.AccountStatus = "Active";

                                db.Entry(empCustInfo).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                                res = 1;
                            }
                        }
                    }
                    else
                    {
                        var empCustInfo = db.emp_CustomerInformation.Where(o => o.Id == CustomerId).FirstOrDefault();
                        if (empCustInfo != null)
                        {
                            if (res == 2 || !empCustInfo.ParentId.HasValue)
                            {
                                empCustInfo.IsActivationCompleted = 1;
                                empCustInfo.StatusCode = EMPConstants.Active;
                                empCustInfo.AccountStatus = "Active";
                            }
                            else if (res == 4 || !empCustInfo.ParentId.HasValue)
                            {
                                empCustInfo.IsActivationCompleted = 1;
                                empCustInfo.StatusCode = EMPConstants.Created;
                                empCustInfo.AccountStatus = "Active";
                            }
                            else if (empCustInfo.StatusCode == EMPConstants.InProgress)
                            {
                                empCustInfo.IsActivationCompleted = 0;
                                empCustInfo.StatusCode = EMPConstants.NewCustomer;
                                empCustInfo.AccountStatus = "Not Active";
                            }
                            else if (empCustInfo.StatusCode == EMPConstants.Created || empCustInfo.StatusCode == EMPConstants.Pending)
                            {
                                empCustInfo.IsActivationCompleted = 1;
                                empCustInfo.StatusCode = EMPConstants.Active;
                                empCustInfo.AccountStatus = "Active";
                            }

                            db.Entry(empCustInfo).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/SaveCustomerconfigStatus", UserId);
                return -1;
            }
        }

        //public List<CustomerAssociatedFeesDTO> CustomerFees(Guid Id)
        //{
        //    try
        //    {

        //        List<CustomerAssociatedFeesDTO> CustomerAssociatedFeesDTOList = new List<CustomerAssociatedFeesDTO>();
        //        var items = db.CustomerAssociatedFees
        //          .Join(db.FeeMasters, cfe => cfe.FeeMaster_ID, fee => fee.Id,
        //                              (cfe, fee) => new { cfe, fee })
        //          .Where(o => o.cfe.emp_CustomerInformation_ID == Id)
        //          .GroupBy(x => new
        //          {
        //              x.fee.FeesFor,
        //              x.fee.Name
        //          }
        //           )
        //          .Select(g => new
        //          {
        //              FeeFor = g.Key.FeesFor,
        //              Name = g.Key.Name,
        //              Amount = g.Sum(z => z.fee.Amount),
        //              Count = g.Count()
        //          }
        //        ).ToList();

        //        foreach (var value in items)
        //        {
        //            CustomerAssociatedFeesDTO CustFees = new CustomerAssociatedFeesDTO();
        //            CustFees.FeesName = "<span>" + value.Name + " : " + value.Amount + "</span><br/>";
        //            CustFees.Amount = value.Amount ?? 0;
        //            CustFees.FeeFor = value.FeeFor ?? 0;
        //            CustFees.FeeForText = (value.FeeFor == (int)EMPConstants.FeesFor.SVBFees) ? EMPConstants.FeesFor.SVBFees.ToString() : EMPConstants.FeesFor.TransmissionFees.ToString();

        //            if (CustomerAssociatedFeesDTOList.Where(o => o.FeeFor == value.FeeFor).ToList().Count > 0)
        //            {
        //                CustFees.Amount = CustomerAssociatedFeesDTOList.Where(o => o.FeeFor == value.FeeFor).FirstOrDefault().Amount + value.Amount ?? 0;
        //                CustFees.FeesName = CustomerAssociatedFeesDTOList.Where(o => o.FeeFor == value.FeeFor).FirstOrDefault().FeesName + CustFees.FeesName;
        //                CustomerAssociatedFeesDTOList.RemoveAll(o => o.FeeFor == value.FeeFor);
        //            }

        //            CustomerAssociatedFeesDTOList.Add(CustFees);
        //        }

        //        return CustomerAssociatedFeesDTOList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/CustomerFees", Id);
        //        return new List<CustomerAssociatedFeesDTO>();
        //    }
        //}

        //public List<CustomerAssociatedFeesDTO> BankFee(Guid Id, Guid ParentId, string CompanyName, string ParentCompanyName, int EntityType, bool IsMSO)
        //{
        //    List<CustomerAssociatedFeesDTO> CustomerAssociatedFeesDTOList = new List<CustomerAssociatedFeesDTO>();

        //    try
        //    {
        //        DropDownService ddService = new DropDownService();
        //        List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
        //        EntityHierarchyDTOs = ddService.GetEntityHierarchies(Id);

        //        Guid TopParentId = Guid.Empty;
        //        Guid FeeSourceParentId = Guid.Empty;

        //        if (EntityHierarchyDTOs.Count > 0)
        //        {
        //            var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
        //            TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;

        //            var FeeSource = EntityHierarchyDTOs.Where(o => o.EntityId == o.FeeSourceEntityId).FirstOrDefault();
        //            if (FeeSource != null)
        //            {
        //                FeeSourceParentId = FeeSource.CustomerId ?? Guid.Empty;
        //            }
        //            else
        //            {
        //                FeeSourceParentId = TopParentId;
        //            }
        //        }

        //        CustomerAssociatedFeesDTOList.AddRange(CustomerFees(TopParentId));

        //        //var EnrollBank = db.BankEnrollments.Where(o => o.CustomerId == ParentId && o.StatusCode == EMPConstants.Ready).ToList();

        //        //if (EnrollBank != null)
        //        //{

        //        List<SubSiteFeeConfig> SubSiteFeeConfigd = new List<SubSiteFeeConfig>();

        //        SubSiteFeeConfigd = db.SubSiteFeeConfigs.Where(o => o.emp_CustomerInformation_ID == TopParentId).ToList();



        //        List<int> ServerOrTransmissions = new List<int>();

        //        CustomerAssociatedFeesDTO CustomerAssociatedFees;





        //        //List<SubSiteFeeConfig> SubSiteFeeConfigd = new List<SubSiteFeeConfig>();

        //        //if (ServerOrTransmissions.ToList().Count > 0)
        //        //{
        //        //    SubSiteFeeConfigd = db.SubSiteFeeConfigs.Where(o => o.emp_CustomerInformation_ID == TopParentId && ServerOrTransmissions.Contains(o.ServiceorTransmission)).ToList();
        //        //}
        //        //else
        //        //{
        //        //    SubSiteFeeConfigd = db.SubSiteFeeConfigs.Where(o => o.emp_CustomerInformation_ID == TopParentId).ToList();
        //        //}
        //        List<EnrollmentBankSelection> EnrollBankSelection = new List<EnrollmentBankSelection>();
        //        EnrollBankSelection = db.EnrollmentBankSelections.Where(o => o.CustomerId == Id && o.StatusCode != EMPConstants.InActive).ToList();
        //        foreach (var item in SubSiteFeeConfigd)
        //        {
        //            CustomerAssociatedFees = new CustomerAssociatedFeesDTO();

        //            if (EntityType > 0)
        //            {
        //                bool IsSubSiteFee = true;
        //                bool IsBankSelectionFee = true;
        //                if (item.IsAddOnFeeCharge == false)
        //                {
        //                    IsSubSiteFee = false;
        //                    CustomerAssociatedFees = new CustomerAssociatedFeesDTO();
        //                    CustomerAssociatedFees.FeeStatus = "No Add-On";
        //                    CustomerAssociatedFees.FeeFor = item.ServiceorTransmission == 1 ? (int)EMPConstants.FeesFor.SVBFees : (int)EMPConstants.FeesFor.TransmissionFees;
        //                    CustomerAssociatedFees.FeesName = "<span>" + CompanyName + " : " + CustomerAssociatedFees.FeeStatus + "</span><br/>";
        //                    CustomerAssociatedFeesDTOList.Add(CustomerAssociatedFees);
        //                }

        //                if (item.IsSubSiteAddonFee == false)
        //                {
        //                    IsBankSelectionFee = false;
        //                }


        //                foreach (var itemn in EnrollBankSelection)
        //                {
        //                    if (IsSubSiteFee)
        //                    {
        //                        CustomerAssociatedFees = new CustomerAssociatedFeesDTO();
        //                        List<SubSiteBankFeesConfig> SubSiteBankFeesConfigList2 = new List<SubSiteBankFeesConfig>();
        //                        SubSiteBankFeesConfigList2 = db.SubSiteBankFeesConfigs.Where(o => o.emp_CustomerInformation_ID == Id && o.ServiceOrTransmitter == item.ServiceorTransmission && o.BankMaster_ID == itemn.BankId).ToList();

        //                        decimal ChildAmount = 0;
        //                        foreach (var BankFees in SubSiteBankFeesConfigList2)
        //                        {
        //                            if (IsMSO)
        //                            {
        //                                ChildAmount = BankFees.BankMaxFees_MSO ?? 0;
        //                                CustomerAssociatedFees.Amount = BankFees.BankMaxFees_MSO ?? 0;
        //                            }
        //                            else
        //                            {
        //                                ChildAmount = BankFees.BankMaxFees;
        //                                CustomerAssociatedFees.Amount = BankFees.BankMaxFees;
        //                            }
        //                        }

        //                        CustomerAssociatedFees.FeeStatus = ChildAmount.ToString();
        //                        CustomerAssociatedFees.FeeFor = item.ServiceorTransmission == 1 ? (int)EMPConstants.FeesFor.SVBFees : (int)EMPConstants.FeesFor.TransmissionFees;
        //                        CustomerAssociatedFees.FeesName = "<span>" + CompanyName + " : " + CustomerAssociatedFees.FeeStatus + "</span><br/>";
        //                        CustomerAssociatedFeesDTOList.Add(CustomerAssociatedFees);
        //                    }


        //                    if (IsBankSelectionFee)
        //                    {
        //                        EnrollmentBankSelection EnrollBankSel = new EnrollmentBankSelection();
        //                        CustomerAssociatedFees = new CustomerAssociatedFeesDTO();
        //                        if (itemn.IsTransmissionFee == true)
        //                        {
        //                            ServerOrTransmissions.Add(2);
        //                            EnrollBankSel = EnrollBankSelection.Where(o => o.IsTransmissionFee == true).FirstOrDefault();
        //                        }

        //                        if (itemn.IsServiceBureauFee == true)
        //                        {
        //                            EnrollBankSel = EnrollBankSelection.Where(o => o.IsServiceBureauFee == true).FirstOrDefault();
        //                            ServerOrTransmissions.Add(1);
        //                        }

        //                        List<SubSiteBankFeesConfig> SubSiteBankFeeConfigList = new List<SubSiteBankFeesConfig>();
        //                        //SubSiteBankFeeConfigList = db.SubSiteBankFeesConfigs.Where(o => o.emp_CustomerInformation_ID == FeeSourceParentId && o.SubSiteFeeConfig_ID == item.ID && o.BankMaster_ID == itemn.BankId).ToList();
        //                        //Enrollment
        //                        if (ServerOrTransmissions.ToList().Count > 0)
        //                        {
        //                            if (item.ServiceorTransmission == 1)
        //                            {
        //                                CustomerAssociatedFees.Amount = EnrollBankSel.ServiceBureauBankAmount ?? 0;
        //                            }
        //                            else
        //                            {
        //                                CustomerAssociatedFees.Amount = EnrollBankSel.TransmissionBankAmount ?? 0;
        //                            }

        //                            CustomerAssociatedFees.FeeStatus = CustomerAssociatedFees.Amount.ToString();
        //                            CustomerAssociatedFees.FeeFor = item.ServiceorTransmission == 1 ? (int)EMPConstants.FeesFor.SVBFees : (int)EMPConstants.FeesFor.TransmissionFees;
        //                            CustomerAssociatedFees.FeesName = "<span>Enrollment Add-On : " + CustomerAssociatedFees.FeeStatus + "</span><br/>";
        //                            CustomerAssociatedFeesDTOList.Add(CustomerAssociatedFees);
        //                        }
        //                        else
        //                        {
        //                            CustomerAssociatedFees.FeeStatus = "No";
        //                            CustomerAssociatedFees.FeeFor = item.ServiceorTransmission == 1 ? (int)EMPConstants.FeesFor.SVBFees : (int)EMPConstants.FeesFor.TransmissionFees;
        //                            CustomerAssociatedFees.FeesName = "<span>Enrollment Add-On : " + CustomerAssociatedFees.FeeStatus + "</span><br/>";
        //                            CustomerAssociatedFeesDTOList.Add(CustomerAssociatedFees);
        //                        }
        //                    }
        //                }

        //                if (!IsBankSelectionFee)
        //                {
        //                    CustomerAssociatedFees = new CustomerAssociatedFeesDTO();
        //                    CustomerAssociatedFees.FeeStatus = "Not Allowed";
        //                    CustomerAssociatedFees.FeeFor = item.ServiceorTransmission == 1 ? (int)EMPConstants.FeesFor.SVBFees : (int)EMPConstants.FeesFor.TransmissionFees;
        //                    CustomerAssociatedFees.FeesName = "<span>Enrollment Add-on : " + CustomerAssociatedFees.FeeStatus + "</span><br/>";
        //                    CustomerAssociatedFeesDTOList.Add(CustomerAssociatedFees);
        //                }
        //            }
        //        }


        //        return CustomerAssociatedFeesDTOList;
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/BankFee_Old2", Id);
        //        return CustomerAssociatedFeesDTOList;
        //    }
        //}

        public bool EmpCsrLogger(Guid UserId)
        {
            try
            {
                using (var db = new DatabaseEntities())
                {
                    string strDescription = "Automated 2016 EMP Enrollment Case";

                    var strOwnerID = (from s in db.emp_CustomerInformation
                                      join y in db.SalesYearMasters on s.SalesYearID equals y.Id
                                      where s.Id == UserId
                                      select new { s.SalesforceAccountID, y.SalesYear }).FirstOrDefault();

                    var Enroll = db.SubSiteConfigurations.Where(a => a.emp_CustomerInformation_ID == UserId && a.IsuTaxManageingEnrolling == true).FirstOrDefault();
                    if (Enroll != null)
                    {
                        strDescription = "Automated 2016 EMP Enrollment Case";
                        Guid iretval = SaveEmpCsrData(UserId, strDescription, strOwnerID.SalesforceAccountID, strOwnerID.SalesYear.Value.ToString());

                        emp_CustomerInformation empCustInfo = new emp_CustomerInformation();
                        empCustInfo = db.emp_CustomerInformation.Where(o => o.Id == UserId).FirstOrDefault();
                        if (empCustInfo != null)
                        {
                            empCustInfo.IsCSRUpdated = true;
                            empCustInfo.CSRUpdatedDt = System.DateTime.Now;
                            empCustInfo.EnrollmentPrimaryKey = iretval;
                            db.Entry(empCustInfo).State = System.Data.Entity.EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                    var OnBoard = db.SubSiteConfigurations.Where(a => a.emp_CustomerInformation_ID == UserId && a.IsuTaxManageOnboarding == true).FirstOrDefault();
                    if (OnBoard != null)
                    {
                        strDescription = "Automated 2016 Onboarding Case";
                        Guid iretval = SaveEmpCsrData(UserId, strDescription, strOwnerID.SalesforceAccountID, strOwnerID.SalesYear.Value.ToString());

                        emp_CustomerInformation empCustInfo = new emp_CustomerInformation();
                        empCustInfo = db.emp_CustomerInformation.Where(o => o.Id == UserId).FirstOrDefault();
                        if (empCustInfo != null)
                        {
                            empCustInfo.IsCSRUpdated = true;
                            empCustInfo.CSRUpdatedDt = System.DateTime.Now;
                            empCustInfo.OnBoardPrimaryKey = iretval;
                            db.Entry(empCustInfo).State = System.Data.Entity.EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/EmpCsrLogger", UserId);
                return false;
            }
        }

        public Guid SaveEmpCsrData(Guid UserId, string strDescription, string strOwnerID, string taxSeason, string Status = "New")
        {
            Guid ReturnID;
            try
            {
                using (var DBCSREntity = new EMPDB_CSREntities())
                {
                    tblCase otblCase = new tblCase();
                    //otblCase.AccountSfdcId=
                    otblCase.Id = Guid.NewGuid();
                    ReturnID = otblCase.Id;
                    //otblCase.AssignedAgentId=
                    otblCase.BankEnrollment = false;
                    otblCase.CctSelections = "";
                    //otblCase.ClosedDate=
                    otblCase.CreatedByAgentId = "emp";
                    otblCase.CreatedDate = System.DateTime.Now;
                    otblCase.Description = strDescription;
                    //otblCase.FunctionalArea=
                    otblCase.IsClosed = false;
                    otblCase.IsDeleted = false;
                    //otblCase.Issue=
                    otblCase.LastModifiedByAgentId = "";
                    otblCase.LastModifiedDate = System.DateTime.Now;
                    otblCase.LastViewedDate = System.DateTime.Now;
                    otblCase.LockedByAgentId = "";
                    //otblCase.Module=
                    //otblCase.Origin=
                    otblCase.Priority = "Medium";
                    //otblCase.Product=
                    //otblCase.Reason=
                    //otblCase.SfdcCaseNumber=
                    otblCase.Status = Status;
                    otblCase.Subject = strDescription;
                    otblCase.TaxSeason = taxSeason;
                    //otblCase.Type=
                    DBCSREntity.tblCases.Add(otblCase);
                    DBCSREntity.SaveChanges();
                    DBCSREntity.Dispose();
                }
                return ReturnID;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/SaveEmpCsrData", UserId);
                return Guid.Empty;
            }
        }

        public Guid SaveCSRCase(string SFDCId, string TaxYear, bool IsOnboard, string Description, bool IsOnboardCall = true)
        {
            using (var db = new EMPDB_CSREntities())
            {
                tblCase _case = new tblCase();
                _case.AccountSfdcId = SFDCId;
                _case.AssignedAgentId = null;
                _case.BankEnrollment = true;
                _case.CctSelections = "-1";
                _case.ClosedDate = null;
                _case.CreatedByAgentId = "EMP";
                _case.CreatedDate = DateTime.UtcNow;
                _case.Description = Description;
                _case.FunctionalArea = null;
                _case.Id = Guid.NewGuid();
                _case.IsClosed = false;
                _case.IsDeleted = false;
                _case.Issue = null;
                _case.LastModifiedByAgentId = "EMP";
                _case.LastModifiedDate = DateTime.UtcNow;
                _case.LastViewedDate = DateTime.UtcNow;
                _case.LockedByAgentId = null;
                _case.Module = null;
                _case.Origin = "EMP Generated";
                _case.Priority = "High";
                _case.Product = null;
                _case.Reason = null;
                _case.SfdcCaseNumber = (db.tblCases.Count() + 900001).ToString();
                _case.Status = "New";
                _case.Subject = _case.Description;
                _case.TaxSeason = TaxYear;
                _case.Type = null;
                db.tblCases.Add(_case);
                db.SaveChanges();

                if (IsOnboardCall)
                {
                    Onboarding _onboard = new Onboarding();
                    _onboard.RowId = Guid.NewGuid();
                    _onboard.CaseId = _case.Id.ToString();
                    _onboard.AdminStatus = 1;
                    _onboard.InstallCurrentYearStatus = IsOnboard ? 1 : 4;
                    _onboard.BankEnrollmentStatus = IsOnboard ? 1 : 4;
                    _onboard.ConversionStatus = IsOnboard ? 1 : 4; ;
                    _onboard.DbConfigurationStatus = IsOnboard ? 1 : 4; ;
                    _onboard.InstallNewYearStatus = IsOnboard ? 1 : 4; ;
                    _onboard.MobileAppStatus = IsOnboard ? 1 : 4;
                    _onboard.LastModifiedDate = DateTime.UtcNow;
                    _onboard.LastModifiedById = "emp";
                    db.Onboardings.Add(_onboard);
                    db.SaveChanges();
                }
                return _case.Id;
            }
        }

        public IQueryable<CustomerRecenltyModel> GetRecentlyCreatedDetails(Guid UserID)
        {
            try
            {

                var data = (from e in db.emp_CustomerInformation
                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
                            where //cli.CrossLinkUserId != null && 
                            e.StatusCode != EMPConstants.NewCustomer
                            select new { e, cli }).ToList();
                var data2 = data;
                if (data.Count > 0)
                {
                    if (data.Any(a => a.cli.CustomerOfficeId == UserID))
                    {
                        data = data.Where(a => a.cli.CustomerOfficeId == UserID).ToList();
                    }
                }
                if (data.ToList().Count == 1)
                {
                    var data_recCreate = data2.Where(a => a.e.ParentId == data[0].e.Id).OrderByDescending(a => a.cli.CreatedDate).Select(o => new CustomerRecenltyModel
                    {
                        Name = o.e.CompanyName,
                        updateDatetime = o.cli.CreatedDate ?? System.DateTime.Now
                    }).Take(3).DefaultIfEmpty();
                    return data_recCreate.AsQueryable();
                }
                else
                {
                    var data_recCreate = data.OrderByDescending(a => a.cli.CreatedDate).Select(o => new CustomerRecenltyModel
                    {
                        Name = o.e.CompanyName,
                        updateDatetime = o.cli.CreatedDate ?? System.DateTime.Now
                    }).Take(3).DefaultIfEmpty();
                    return data_recCreate.AsQueryable();

                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetRecentlyCreatedDetails", UserID);
                return new List<CustomerRecenltyModel>().AsQueryable();
            }
        }

        public IQueryable<CustomerRecenltyModel> GetRecentlyUpdateDetails(Guid UserID)
        {
            try
            {

                var data = (from e in db.emp_CustomerInformation
                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
                            where //cli.CrossLinkUserId != null && 
                            e.StatusCode != EMPConstants.NewCustomer
                            select new { e, cli }).ToList();
                var data2 = data;
                if (data.Count > 0)
                {
                    if (data.Any(a => a.cli.CustomerOfficeId == UserID))
                    {
                        data = data.Where(a => a.cli.CustomerOfficeId == UserID).ToList();
                    }
                }
                if (data.ToList().Count == 1)
                {
                    var data_recCreate = data2.Where(a => a.e.ParentId == data[0].e.Id).OrderByDescending(a => a.e.LastUpdatedDate).Select(o => new CustomerRecenltyModel
                    {
                        Name = o.e.CompanyName,
                        updateDatetime = o.e.LastUpdatedDate ?? System.DateTime.Now
                    }).Take(3).DefaultIfEmpty();
                    return data_recCreate.AsQueryable();
                }
                else
                {
                    var data_recCreate = data.OrderByDescending(a => a.e.LastUpdatedDate).Select(o => new CustomerRecenltyModel
                    {
                        Name = o.e.CompanyName,
                        updateDatetime = o.e.LastUpdatedDate ?? System.DateTime.Now
                    }).Take(3).DefaultIfEmpty();
                    return data_recCreate.AsQueryable();
                }

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetRecentlyUpdateDetails", UserID);
                return new List<CustomerRecenltyModel>().AsQueryable();
            }
        }

        public bool SaveExistingForAdditionalEFINSubSite(Guid MyId, Guid ParentId, Guid UserId, Guid SalesYearId)
        {
            try
            {

                db = new DatabaseEntities();

                var SubSiteOfficeConfigs = db.SubSiteOfficeConfigs.Where(o => o.RefId == ParentId).ToList();
                //List<SubSiteOfficeConfig> SubSiteOfficeConfigList = new List<SubSiteOfficeConfig>();
                foreach (var item in SubSiteOfficeConfigs)
                {
                    SubSiteOfficeConfig SubSiteOfficeCon = new SubSiteOfficeConfig();
                    SubSiteOfficeCon.Id = Guid.NewGuid();
                    SubSiteOfficeCon.RefId = MyId;
                    SubSiteOfficeCon.SubSiteSendTaxReturn = item.SubSiteSendTaxReturn;
                    SubSiteOfficeCon.CanSubSiteLoginToEmp = item.CanSubSiteLoginToEmp;
                    SubSiteOfficeCon.EFINListedOtherOffice = item.EFINListedOtherOffice;
                    SubSiteOfficeCon.EFINOwnerSite = item.EFINOwnerSite;
                    SubSiteOfficeCon.IsMainSiteTransmitTaxReturn = item.IsMainSiteTransmitTaxReturn;
                    SubSiteOfficeCon.IsSoftwareOnNetwork = item.IsSoftwareOnNetwork;
                    SubSiteOfficeCon.NoofComputers = item.NoofComputers;
                    SubSiteOfficeCon.NoofTaxProfessionals = item.NoofTaxProfessionals;
                    SubSiteOfficeCon.PreferredLanguage = item.PreferredLanguage;
                    SubSiteOfficeCon.SiteanMSOLocation = item.SiteanMSOLocation;
                    SubSiteOfficeCon.SiteOwnthisEFIN = item.SiteOwnthisEFIN;
                    SubSiteOfficeCon.SOorSSorEFIN = 1;
                    SubSiteOfficeCon.CreatedBy = UserId;
                    SubSiteOfficeCon.CreatedDate = DateTime.Now;
                    SubSiteOfficeCon.LastUpdatedBy = UserId;
                    SubSiteOfficeCon.LastUpdatedDate = DateTime.Now;
                    SubSiteOfficeCon.IsSharingEFIN = item.IsSharingEFIN;
                    SubSiteOfficeCon.HasBusinessSoftware = item.HasBusinessSoftware;

                    //SubSiteOfficeConfigList.Add(SubSiteOfficeCon);
                    db.SubSiteOfficeConfigs.Add(SubSiteOfficeCon);
                }


                var SubSiteBankFeesConfig = db.SubSiteBankFeesConfigs.Where(o => o.emp_CustomerInformation_ID == ParentId).ToList();
                foreach (var item in SubSiteBankFeesConfig)
                {
                    SubSiteBankFeesConfig SubSiteBankFeesConfi = new SubSiteBankFeesConfig();
                    SubSiteBankFeesConfi.ID = Guid.NewGuid();
                    SubSiteBankFeesConfi.BankMaster_ID = item.BankMaster_ID;
                    SubSiteBankFeesConfi.BankMaxFees = item.BankMaxFees;
                    SubSiteBankFeesConfi.BankMaxFees_MSO = item.BankMaxFees_MSO;
                    SubSiteBankFeesConfi.emp_CustomerInformation_ID = MyId;
                    SubSiteBankFeesConfi.QuestionID = item.QuestionID;
                    SubSiteBankFeesConfi.ServiceOrTransmitter = item.ServiceOrTransmitter;
                    SubSiteBankFeesConfi.SubSiteFeeConfig_ID = item.SubSiteFeeConfig_ID;

                    SubSiteBankFeesConfi.CreatedBy = UserId;
                    SubSiteBankFeesConfi.CreatedDate = DateTime.Now;
                    SubSiteBankFeesConfi.LastUpdatedBy = UserId;
                    SubSiteBankFeesConfi.LastUpdatedDate = DateTime.Now;
                    db.SubSiteBankFeesConfigs.Add(SubSiteBankFeesConfi);
                }


                List<Guid> SiteMapIdList = new List<Guid>();
                SiteMapIdList.Add(Guid.Parse("7a2c166c-c2ef-47c0-aa5f-e4950f9ff369"));
                SiteMapIdList.Add(Guid.Parse("067c03a3-34f1-4143-beae-35327a8fca44"));
                SiteMapIdList.Add(Guid.Parse("0feeb0fe-d0e7-4370-8733-dd5f7d2041fc"));
                SiteMapIdList.Add(Guid.Parse("98a706d7-031f-4c5d-8cc4-d32cc7658b69"));
                SiteMapIdList.Add(Guid.Parse("a55334d1-3960-44c4-8cf1-e3ba9901f2be"));

                var CustomerConfigurationStatu = db.CustomerConfigurationStatus.Where(o => o.CustomerId == ParentId && !SiteMapIdList.Contains(o.SitemapId ?? Guid.Empty)).ToList();
                foreach (var item in CustomerConfigurationStatu)
                {
                    CustomerConfigurationStatu CustomerConfiguration = new CustomerConfigurationStatu();
                    CustomerConfiguration.Id = Guid.NewGuid();
                    CustomerConfiguration.CustomerId = MyId;
                    CustomerConfiguration.SitemapId = item.SitemapId;
                    CustomerConfiguration.StatusCode = item.StatusCode;
                    CustomerConfiguration.UpdatedBy = UserId;
                    CustomerConfiguration.UpdatedDate = DateTime.Now;
                    db.CustomerConfigurationStatus.Add(CustomerConfiguration);
                }

                //[dbo].[EnrollmentOfficeConfiguration]
                //[dbo].[EnrollmentAffiliateConfiguration]
                var EnrollmentOfficeConfigList = db.EnrollmentOfficeConfigurations.Where(o => o.CustomerId == ParentId).ToList();
                foreach (var item in EnrollmentOfficeConfigList)
                {
                    EnrollmentOfficeConfiguration EnrollmentOfficeConfig = new EnrollmentOfficeConfiguration();
                    EnrollmentOfficeConfig.Id = Guid.NewGuid();
                    EnrollmentOfficeConfig.CustomerId = MyId;
                    EnrollmentOfficeConfig.IsMainSiteTransmitTaxReturn = item.IsMainSiteTransmitTaxReturn;
                    EnrollmentOfficeConfig.IsSoftwareOnNetwork = item.IsSoftwareOnNetwork;
                    EnrollmentOfficeConfig.NoofComputers = item.NoofComputers;
                    EnrollmentOfficeConfig.NoofTaxProfessionals = item.NoofTaxProfessionals;
                    EnrollmentOfficeConfig.PreferredLanguage = item.PreferredLanguage;
                    EnrollmentOfficeConfig.StatusCode = item.StatusCode;
                    EnrollmentOfficeConfig.CreatedBy = ParentId;
                    EnrollmentOfficeConfig.CreatedDate = DateTime.Now;
                    EnrollmentOfficeConfig.LastUpdatedBy = ParentId;
                    EnrollmentOfficeConfig.LastUpdatedDate = DateTime.Now;
                    db.EnrollmentOfficeConfigurations.Add(EnrollmentOfficeConfig);
                }

                var EnrollmentAffiliateConfigList = db.EnrollmentAffiliateConfigurations.Where(o => o.CustomerId == ParentId).ToList();
                foreach (var item in EnrollmentAffiliateConfigList)
                {
                    EnrollmentAffiliateConfiguration EnrollmentAffiliateConfig = new EnrollmentAffiliateConfiguration();
                    EnrollmentAffiliateConfig.Id = Guid.NewGuid();
                    EnrollmentAffiliateConfig.CustomerId = MyId;
                    EnrollmentAffiliateConfig.AffiliateProgramCharge = item.AffiliateProgramCharge;
                    EnrollmentAffiliateConfig.AffiliateProgramId = item.AffiliateProgramId;
                    EnrollmentAffiliateConfig.StatusCode = item.StatusCode;
                    EnrollmentAffiliateConfig.CreatedBy = ParentId;
                    EnrollmentAffiliateConfig.CreatedDate = DateTime.Now;
                    EnrollmentAffiliateConfig.LastUpdatedBy = ParentId;
                    EnrollmentAffiliateConfig.LastUpdatedDate = DateTime.Now;
                    db.EnrollmentAffiliateConfigurations.Add(EnrollmentAffiliateConfig);
                }

                OfficeManagementService _OfficeManagementService = new OfficeManagementService();
                var result = _OfficeManagementService.UpdateOfficeManagement(MyId, SalesYearId.ToString());

                try
                {
                    db.SaveChanges();
                    db.Dispose();
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/SaveExistingForAdditionalEFINSubSite", MyId);
                return false;
            }
        }

        public bool SaveExistingForSubSite(Guid MyId, Guid ParentId)
        {
            try
            {

                db = new DatabaseEntities();

                var SubSiteBankFeesConfig = db.SubSiteBankFeesConfigs.Where(o => o.emp_CustomerInformation_ID == ParentId).ToList();
                foreach (var item in SubSiteBankFeesConfig)
                {
                    SubSiteBankFeesConfig SubSiteBankFeesConfi = new SubSiteBankFeesConfig();
                    SubSiteBankFeesConfi.ID = Guid.NewGuid();
                    SubSiteBankFeesConfi.BankMaster_ID = item.BankMaster_ID;
                    SubSiteBankFeesConfi.BankMaxFees = item.BankMaxFees;
                    SubSiteBankFeesConfi.BankMaxFees_MSO = item.BankMaxFees_MSO;
                    SubSiteBankFeesConfi.emp_CustomerInformation_ID = MyId;
                    // SubSiteBankFeesConfi.QuestionID = item.QuestionID;
                    SubSiteBankFeesConfi.ServiceOrTransmitter = item.ServiceOrTransmitter;
                    //   SubSiteBankFeesConfi.SubSiteFeeConfig_ID = item.SubSiteFeeConfig_ID;

                    SubSiteBankFeesConfi.CreatedBy = ParentId;
                    SubSiteBankFeesConfi.CreatedDate = DateTime.Now;
                    SubSiteBankFeesConfi.LastUpdatedBy = ParentId;
                    SubSiteBankFeesConfi.LastUpdatedDate = DateTime.Now;

                    db.SubSiteBankFeesConfigs.Add(SubSiteBankFeesConfi);
                }

                try
                {
                    db.SaveChanges();
                    db.Dispose();
                }
                catch (Exception)
                {
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/SaveExistingForSubSite", MyId);
                return false;
            }
        }

        public bool SaveUtaxSVBFee(bool uTaxNotCollectingSVBFee, Guid Id)
        {
            try
            {
                db = new DatabaseEntities();
                emp_CustomerInformation customerInformation = db.emp_CustomerInformation.Where(e => e.Id == Id).FirstOrDefault();

                if (customerInformation != null)
                {
                    customerInformation.uTaxNotCollectingSBFee = uTaxNotCollectingSVBFee;
                    db.Entry(customerInformation).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/SaveUtaxSVBFee", Id);
                return false;
            }
        }

        //public CustomerInfoNewGrid GetSearchCustomerInformationNewGrid(CustomerInfoNewGrid omodel)
        //{
        //    try
        //    {

        //        int CounttotalRecords = 0;

        //        List<string> Statuslst = !string.IsNullOrEmpty(omodel.Status) ? omodel.Status.Split(',').ToList() : null;
        //        List<string> SiteTypelst = !string.IsNullOrEmpty(omodel.SiteType) ? omodel.SiteType.Split(',').ToList() : null;
        //        List<string> BankPartnerlst = !string.IsNullOrEmpty(omodel.BankPartner) ? omodel.BankPartner.Split(',').ToList() : null;
        //        List<string> EnrollmentStatuslst = !string.IsNullOrEmpty(omodel.EnrollmentStatus) ? omodel.EnrollmentStatus.Split(',').ToList() : null;
        //        List<string> OnBoardingStatuslst = !string.IsNullOrEmpty(omodel.OnBoardingStatus) ? omodel.OnBoardingStatus.Split(',').ToList() : null;
        //        List<TestModel1> data = new List<TestModel1>();
        //        bool iretval = false;

        //        #region "Search Drop down lists"
        //        if (Statuslst != null)
        //        {
        //            iretval = true;
        //            #region "Status Dropdown list"

        //            List<int?> actLst = new List<int?>();
        //            List<string> stsLst = new List<string>();
        //            if (Statuslst.Contains("INA"))
        //            {
        //                //Statuslst.Add("CRT");
        //                //Statuslst.Add("PEA");
        //                stsLst.Add("ACT");
        //                stsLst.Add("INA");
        //                stsLst.Add("INP");
        //                stsLst.Add("CRT");
        //                actLst.Add(0);
        //            }

        //            if (Statuslst.Contains("ACT"))
        //            {
        //                actLst.Add(1);
        //                stsLst.Add("ACT");
        //                stsLst.Add("INA");
        //                stsLst.Add("INP");
        //                stsLst.Add("CRT");
        //            }

        //            if (Statuslst.Contains("CRT"))
        //            {
        //                stsLst.Add("CRT");
        //                stsLst.Add("INP");
        //                if (actLst.Count == 0)
        //                {
        //                    actLst.Add(1);
        //                    actLst.Add(0);
        //                }
        //            }
        //            else
        //            {
        //                stsLst.Add("CRT");
        //                stsLst.Add("INP");
        //                if (!stsLst.Contains("ACT"))
        //                    stsLst.Add("ACT");
        //            }

        //            if (Statuslst.Count == 3)
        //            {
        //                stsLst.Add("ACT");
        //                actLst.Add(null);
        //            }

        //            if (ExistOrNotCustomer(omodel.UserId))
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        orderby e.CompanyName
        //                        where e.ParentId == null && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                         && (stsLst.Contains(e.StatusCode) && actLst.Contains(e.IsActivationCompleted))
        //                         && cli.CustomerOfficeId == omodel.UserId
        //                        select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                     join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                     orderby e.CompanyName
        //                                     where e.ParentId == null && cli.CrossLinkUserId != null
        //                                      && e.StatusCode != EMPConstants.NewCustomer
        //                                       && (stsLst.Contains(e.StatusCode) && actLst.Contains(e.IsActivationCompleted))
        //                                       && cli.CustomerOfficeId == omodel.UserId
        //                                     select new { e, cli }).Count();



        //                //if (Statuslst.Contains("ACT"))
        //                //{
        //                //    data = (from e in db.emp_CustomerInformation
        //                //            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                //            orderby e.CompanyName
        //                //            where e.ParentId == null && cli.CrossLinkUserId != null
        //                //             && e.StatusCode != EMPConstants.NewCustomer
        //                //             && (Statuslst.Contains(e.StatusCode) || (e.IsActivationCompleted == 1 && e.StatusCode != EMPConstants.NewCustomer))
        //                //             && cli.CustomerOfficeId == omodel.UserId
        //                //            select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                //    CounttotalRecords = (from e in db.emp_CustomerInformation
        //                //                         join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                //                         orderby e.CompanyName
        //                //                         where e.ParentId == null && cli.CrossLinkUserId != null
        //                //                          && e.StatusCode != EMPConstants.NewCustomer
        //                //                           && (Statuslst.Contains(e.StatusCode) || (e.IsActivationCompleted == 1 && e.StatusCode != EMPConstants.NewCustomer))
        //                //                           && cli.CustomerOfficeId == omodel.UserId
        //                //                         select new { e, cli }).Count();
        //                //}
        //                //else
        //                //{
        //                //    data = (from e in db.emp_CustomerInformation
        //                //            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                //            orderby e.CompanyName
        //                //            where e.ParentId == null && cli.CrossLinkUserId != null
        //                //             && (e.StatusCode != EMPConstants.NewCustomer)
        //                //             && Statuslst.Contains(e.StatusCode) && cli.CustomerOfficeId == omodel.UserId
        //                //            select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                //    CounttotalRecords = (from e in db.emp_CustomerInformation
        //                //                         join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                //                         orderby e.CompanyName
        //                //                         where e.ParentId == null && cli.CrossLinkUserId != null
        //                //                          && e.StatusCode != EMPConstants.NewCustomer
        //                //                           && Statuslst.Contains(e.StatusCode)
        //                //                           && cli.CustomerOfficeId == omodel.UserId
        //                //                         select new { e, cli }).Count();
        //                //}
        //            }
        //            else
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        orderby e.CompanyName
        //                        where e.ParentId == null && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                         && (stsLst.Contains(e.StatusCode) && actLst.Contains(e.IsActivationCompleted))
        //                        select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                     join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                     orderby e.CompanyName
        //                                     where e.ParentId == null && cli.CrossLinkUserId != null
        //                                      && e.StatusCode != EMPConstants.NewCustomer
        //                                       && (stsLst.Contains(e.StatusCode) && actLst.Contains(e.IsActivationCompleted))
        //                                     select new { e, cli }).Count();


        //                //if (Statuslst.Contains("ACT"))
        //                //{
        //                //    data = (from e in db.emp_CustomerInformation
        //                //            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                //            orderby e.CompanyName
        //                //            where e.ParentId == null && cli.CrossLinkUserId != null
        //                //             && e.StatusCode != EMPConstants.NewCustomer
        //                //             && (Statuslst.Contains(e.StatusCode) || (e.IsActivationCompleted == 1 && e.StatusCode != EMPConstants.NewCustomer))
        //                //            select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                //    CounttotalRecords = (from e in db.emp_CustomerInformation
        //                //                         join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                //                         orderby e.CompanyName
        //                //                         where e.ParentId == null && cli.CrossLinkUserId != null
        //                //                          && e.StatusCode != EMPConstants.NewCustomer
        //                //                           && (Statuslst.Contains(e.StatusCode) || (e.IsActivationCompleted == 1 && e.StatusCode != EMPConstants.NewCustomer))
        //                //                         select new { e, cli }).Count();
        //                //}
        //                //else
        //                //{
        //                //    data = (from e in db.emp_CustomerInformation
        //                //            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                //            orderby e.CompanyName
        //                //            where e.ParentId == null && cli.CrossLinkUserId != null
        //                //             && e.StatusCode != EMPConstants.NewCustomer
        //                //             && Statuslst.Contains(e.StatusCode)
        //                //            select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                //    CounttotalRecords = (from e in db.emp_CustomerInformation
        //                //                         join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                //                         orderby e.CompanyName
        //                //                         where e.ParentId == null && cli.CrossLinkUserId != null
        //                //                          && e.StatusCode != EMPConstants.NewCustomer
        //                //                           && Statuslst.Contains(e.StatusCode)
        //                //                         select new { e, cli }).Count();
        //                //}

        //            }
        //            #endregion
        //        }
        //        if (SiteTypelst != null)
        //        {
        //            iretval = true;
        //            #region "Site type Drop down list"
        //            if (ExistOrNotCustomer(omodel.UserId))
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        orderby e.CompanyName
        //                        where e.ParentId == null && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                         && SiteTypelst.Contains(e.EntityId.ToString()) && cli.CustomerOfficeId == omodel.UserId
        //                        select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                     join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                     orderby e.CompanyName
        //                                     where e.ParentId == null && cli.CrossLinkUserId != null
        //                                      && e.StatusCode != EMPConstants.NewCustomer
        //                                      && SiteTypelst.Contains(e.EntityId.ToString()) && cli.CustomerOfficeId == omodel.UserId
        //                                     select new { e, cli }).Count();
        //            }
        //            else
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        orderby e.CompanyName
        //                        where e.ParentId == null && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                         && SiteTypelst.Contains(e.EntityId.ToString())
        //                        select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                     join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                     orderby e.CompanyName
        //                                     where e.ParentId == null && cli.CrossLinkUserId != null
        //                                      && e.StatusCode != EMPConstants.NewCustomer
        //                                      && SiteTypelst.Contains(e.EntityId.ToString())
        //                                     select new { e, cli }).Count();
        //            }
        //            #endregion
        //        }
        //        if (EnrollmentStatuslst != null)
        //        {
        //            iretval = true;
        //            #region "Enrollment Status Drop down list"
        //            if (ExistOrNotCustomer(omodel.UserId))
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        join b in db.BankEnrollments on e.Id equals b.CustomerId
        //                        orderby e.CompanyName
        //                        where e.ParentId == null && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                         && EnrollmentStatuslst.Contains(b.StatusCode) && cli.CustomerOfficeId == omodel.UserId
        //                         && b.IsActive == true
        //                        select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                     join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                     join b in db.BankEnrollments on e.Id equals b.CustomerId
        //                                     orderby e.CompanyName
        //                                     where e.ParentId == null && cli.CrossLinkUserId != null
        //                                      && e.StatusCode != EMPConstants.NewCustomer
        //                                      && EnrollmentStatuslst.Contains(b.StatusCode) && cli.CustomerOfficeId == omodel.UserId && b.IsActive == true
        //                                     select new { e, cli }).Count();
        //            }
        //            else
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        join b in db.BankEnrollments on e.Id equals b.CustomerId
        //                        orderby e.CompanyName
        //                        where e.ParentId == null && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                          && EnrollmentStatuslst.Contains(b.StatusCode)
        //                         && b.IsActive == true
        //                        select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                     join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                     join b in db.BankEnrollments on e.Id equals b.CustomerId
        //                                     orderby e.CompanyName
        //                                     where e.ParentId == null && cli.CrossLinkUserId != null
        //                                      && e.StatusCode != EMPConstants.NewCustomer
        //                                      && EnrollmentStatuslst.Contains(b.StatusCode)
        //                                        && b.IsActive == true
        //                                     select new { e, cli }).Count();
        //            }
        //            #endregion
        //        }
        //        if (OnBoardingStatuslst != null)
        //        {
        //            var dbcsr = new EMPDB_CSREntities();
        //            iretval = true;
        //            #region "OnBoarding Status Drop down list"
        //            if (ExistOrNotCustomer(omodel.UserId))
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        join c in dbcsr.tblCases on e.OnBoardPrimaryKey equals c.Id
        //                        orderby e.CompanyName
        //                        where e.ParentId == null && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                         && OnBoardingStatuslst.Contains(c.Status) && cli.CustomerOfficeId == omodel.UserId
        //                        select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                     join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                     join c in dbcsr.tblCases on e.OnBoardPrimaryKey equals c.Id
        //                                     orderby e.CompanyName
        //                                     where e.ParentId == null && cli.CrossLinkUserId != null
        //                                      && e.StatusCode != EMPConstants.NewCustomer
        //                                      && OnBoardingStatuslst.Contains(c.Status) && cli.CustomerOfficeId == omodel.UserId
        //                                     select new { e, cli }).Count();
        //            }
        //            else
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        join c in dbcsr.tblCases on e.OnBoardPrimaryKey equals c.Id
        //                        orderby e.CompanyName
        //                        where e.ParentId == null && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                          && OnBoardingStatuslst.Contains(c.Status)
        //                        select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                     join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                     join c in dbcsr.tblCases on e.OnBoardPrimaryKey equals c.Id
        //                                     orderby e.CompanyName
        //                                     where e.ParentId == null && cli.CrossLinkUserId != null
        //                                      && e.StatusCode != EMPConstants.NewCustomer
        //                                      && OnBoardingStatuslst.Contains(c.Status)
        //                                     select new { e, cli }).Count();
        //            }
        //            #endregion
        //        }
        //        #endregion

        //        if (omodel.SearchType > 0)
        //        {
        //            List<Guid> lstGuids = GetCustomerInfo_ChildList(Statuslst, SiteTypelst, omodel.SearchText, omodel.SearchType, Guid.Empty);
        //            iretval = true;
        //            #region "Search Data"
        //            if (omodel.SearchType == 1)
        //            {
        //                #region "Company Name Search"
        //                if (ExistOrNotCustomer(omodel.UserId))
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == null && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && e.CompanyName.ToString().ToLower().Contains(omodel.SearchText.ToLower()) && cli.CustomerOfficeId == omodel.UserId
        //                            select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                    CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                         join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                         orderby e.CompanyName
        //                                         where e.ParentId == null && cli.CrossLinkUserId != null
        //                                          && e.StatusCode != EMPConstants.NewCustomer && cli.CustomerOfficeId == omodel.UserId
        //                                          && e.CompanyName.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                                         select new { e, cli }).Count();
        //                }
        //                else
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == null && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && e.CompanyName.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                            select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                    CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                         join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                         orderby e.CompanyName
        //                                         where e.ParentId == null && cli.CrossLinkUserId != null
        //                                          && e.StatusCode != EMPConstants.NewCustomer
        //                                          && e.CompanyName.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                                         select new { e, cli }).Count();
        //                }
        //                #endregion
        //            }
        //            else if (omodel.SearchType == 2)
        //            {
        //                #region "User ID Search"
        //                if (ExistOrNotCustomer(omodel.UserId))
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == null && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && cli.CrossLinkUserId.ToString().ToLower().Contains(omodel.SearchText.ToLower()) && cli.CustomerOfficeId == omodel.UserId
        //                            select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                    CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                         join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                         orderby e.CompanyName
        //                                         where e.ParentId == null && cli.CrossLinkUserId != null
        //                                          && e.StatusCode != EMPConstants.NewCustomer && cli.CustomerOfficeId == omodel.UserId
        //                                          && cli.CrossLinkUserId.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                                         select new { e, cli }).Count();
        //                }
        //                else
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == null && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && cli.CrossLinkUserId.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                            select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                    CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                         join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                         orderby e.CompanyName
        //                                         where e.ParentId == null && cli.CrossLinkUserId != null
        //                                          && e.StatusCode != EMPConstants.NewCustomer
        //                                          && cli.CrossLinkUserId.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                                         select new { e, cli }).Count();
        //                }
        //                #endregion
        //            }
        //            else if (omodel.SearchType == 3)
        //            {
        //                #region "EFIN Search"
        //                if (ExistOrNotCustomer(omodel.UserId))
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == null && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && e.EFIN.ToString().ToLower().Contains(omodel.SearchText.ToLower()) && cli.CustomerOfficeId == omodel.UserId
        //                            select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                    CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                         join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                         orderby e.CompanyName
        //                                         where e.ParentId == null && cli.CrossLinkUserId != null
        //                                          && e.StatusCode != EMPConstants.NewCustomer
        //                                          && cli.EFIN.ToString().ToLower().Contains(omodel.SearchText.ToLower()) && cli.CustomerOfficeId == omodel.UserId
        //                                         select new { e, cli }).Count();
        //                }
        //                else
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == null && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && cli.EFIN.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                            select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                    CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                         join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                         orderby e.CompanyName
        //                                         where e.ParentId == null && cli.CrossLinkUserId != null
        //                                          && e.StatusCode != EMPConstants.NewCustomer
        //                                          && cli.EFIN.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                                         select new { e, cli }).Count();
        //                }
        //                #endregion
        //            }
        //            else if (omodel.SearchType == 4)
        //            {
        //                #region "Alternative Contact Search"
        //                if (ExistOrNotCustomer(omodel.UserId))
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == null && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && e.AlternativeContact.ToString().ToLower().Contains(omodel.SearchText.ToLower()) && cli.CustomerOfficeId == omodel.UserId
        //                            select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                    CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                         join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                         orderby e.CompanyName
        //                                         where e.ParentId == null && cli.CrossLinkUserId != null
        //                                          && e.StatusCode != EMPConstants.NewCustomer && cli.CustomerOfficeId == omodel.UserId
        //                                          && e.AlternativeContact.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                                         select new { e, cli }).Count();
        //                }
        //                else
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == null && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && e.AlternativeContact.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                            select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                    CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                         join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                         orderby e.CompanyName
        //                                         where e.ParentId == null && cli.CrossLinkUserId != null
        //                                          && e.StatusCode != EMPConstants.NewCustomer
        //                                          && e.AlternativeContact.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                                         select new { e, cli }).Count();
        //                }
        //                #endregion
        //            }
        //            else if (omodel.SearchType == 5)
        //            {
        //                #region "Alternative Phone Search"
        //                if (ExistOrNotCustomer(omodel.UserId))
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == null && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && e.AlternatePhone.ToString().ToLower().Contains(omodel.SearchText.ToLower()) && cli.CustomerOfficeId == omodel.UserId
        //                            select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                    CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                         join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                         orderby e.CompanyName
        //                                         where e.ParentId == null && cli.CrossLinkUserId != null
        //                                          && e.StatusCode != EMPConstants.NewCustomer && cli.CustomerOfficeId == omodel.UserId
        //                                          && e.AlternatePhone.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                                         select new { e, cli }).Count();
        //                }
        //                else
        //                {
        //                    data = (from e in db.emp_CustomerInformation
        //                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                            orderby e.CompanyName
        //                            where e.ParentId == null && cli.CrossLinkUserId != null
        //                             && e.StatusCode != EMPConstants.NewCustomer
        //                             && e.AlternatePhone.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                            select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                    CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                         join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                         orderby e.CompanyName
        //                                         where e.ParentId == null && cli.CrossLinkUserId != null
        //                                          && e.StatusCode != EMPConstants.NewCustomer
        //                                          && e.AlternatePhone.ToString().ToLower().Contains(omodel.SearchText.ToLower())
        //                                         select new { e, cli }).Count();
        //                }
        //                #endregion
        //            }
        //            #endregion

        //            if (lstGuids.Count() > 0)
        //            {
        //                iretval = true;
        //                var data1 = (from e in db.emp_CustomerInformation
        //                             join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                             orderby e.CompanyName
        //                             where e.ParentId == null && lstGuids.Contains(e.Id) && cli.CrossLinkUserId != null
        //                              && e.StatusCode != EMPConstants.NewCustomer
        //                             select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                data.AddRange(data1);

        //                data = data.DistinctBy(x => x.e.Id).ToList();
        //                CounttotalRecords = data.ToList().Count();
        //            }
        //        }
        //        if (iretval == false)
        //        {
        //            iretval = ExistOrNotCustomer(omodel.UserId);
        //            if (iretval)
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        orderby e.CompanyName
        //                        where e.ParentId == null && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer && cli.CustomerOfficeId == omodel.UserId
        //                        select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                     join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                     where e.ParentId == null && cli.CrossLinkUserId != null && e.StatusCode != EMPConstants.NewCustomer
        //                                     && cli.CustomerOfficeId == omodel.UserId
        //                                     select e).Count();
        //            }
        //            else
        //            {
        //                data = (from e in db.emp_CustomerInformation
        //                        join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                        orderby e.CompanyName
        //                        where e.ParentId == null && cli.CrossLinkUserId != null
        //                         && e.StatusCode != EMPConstants.NewCustomer
        //                        select new TestModel1 { e = e, cli = cli }).ToList().Skip(omodel.start).Take(omodel.length).ToList();

        //                CounttotalRecords = (from e in db.emp_CustomerInformation
        //                                     join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
        //                                     where e.ParentId == null && cli.CrossLinkUserId != null && e.StatusCode != EMPConstants.NewCustomer
        //                                     select e).Count();
        //            }
        //        }
        //        List<CustomerModel> customerlst = new List<CustomerModel>();
        //        foreach (var itm in data)
        //        {
        //            CustomerModel ocustomer = new CustomerModel();
        //            ocustomer.Id = itm.e.Id;
        //            ocustomer.ParentId = itm.e.ParentId ?? Guid.Empty;
        //            ocustomer.str_ParentId = itm.e.ParentId.ToString() ?? "";
        //            ocustomer.CompanyName = itm.e.CompanyName;

        //            ocustomer.SalesforceParentID = itm.e.SalesforceParentID;
        //            ocustomer.MasterIdentifier = itm.e.MasterIdentifier;
        //            ocustomer.Feeder = itm.e.Feeder;
        //            ocustomer.BusinessOwnerFirstName = itm.e.BusinessOwnerFirstName;
        //            ocustomer.OfficePhone = itm.e.OfficePhone;
        //            ocustomer.AlternatePhone = itm.e.AlternatePhone;
        //            ocustomer.Primaryemail = itm.e.PrimaryEmail;
        //            ocustomer.SupportNotificationemail = itm.e.SupportNotificationEmail;
        //            ocustomer.EROType = itm.e.EROType;
        //            ocustomer.AlternativeContact = itm.e.AlternativeContact ?? "";
        //            ocustomer.EFIN = itm.e.EFIN;
        //            ocustomer.PhysicalAddress1 = itm.e.PhysicalAddress1;
        //            ocustomer.PhysicalAddress2 = itm.e.PhysicalAddress2;
        //            ocustomer.PhysicalZipcode = itm.e.PhysicalZipCode;
        //            ocustomer.PhysicalCity = itm.e.PhysicalCity;
        //            ocustomer.PhysicalState = itm.e.PhysicalState;
        //            ocustomer.ShippingAddress1 = itm.e.ShippingAddress1;
        //            ocustomer.ShippingAddress2 = itm.e.ShippingAddress2;
        //            ocustomer.ShippingZipcode = itm.e.ShippingZipCode;
        //            ocustomer.ShippingCity = itm.e.ShippingCity;
        //            ocustomer.ShippingState = itm.e.ShippingState;
        //            ocustomer.PhoneTypeId = itm.e.PhoneTypeId;
        //            ocustomer.TitleId = itm.e.TitleId;
        //            ocustomer.AlternativeType = itm.e.AlternativeType;
        //            ocustomer.PhoneType = db.PhoneTypeMasters.Where(a => a.Id == itm.e.PhoneTypeId).Select(a => a.PhoneType).FirstOrDefault();
        //            ocustomer.ContactTitle = db.ContactPersonTitleMasters.Where(a => a.Id == itm.e.TitleId).Select(a => a.ContactPersonTitle).FirstOrDefault() ?? "";
        //            ocustomer.EntityId = itm.e.EntityId ?? 0;
        //            ocustomer.SalesYearID = itm.e.SalesYearID ?? Guid.Empty;
        //            ocustomer.BaseEntityId = db.EntityMasters.Where(a => a.Id == itm.e.EntityId).Select(a => a.BaseEntityId).FirstOrDefault();
        //            ocustomer.LoginId = itm.cli.Id.ToString();
        //            ocustomer.LoginEFIN = itm.cli.EFIN;
        //            ocustomer.MasterIdentifier = itm.cli.MasterIdentifier;
        //            ocustomer.CrossLinkUserId = itm.cli.CrossLinkUserId ?? "";
        //            ocustomer.CrossLinkPassword = itm.cli.CrossLinkPassword;
        //            ocustomer.OfficePortalUrl = itm.cli.OfficePortalUrl;
        //            ocustomer.EMPUserId = itm.cli.EMPUserId;
        //            ocustomer.EMPPassword = itm.cli.EMPPassword;
        //            ocustomer.TaxOfficeUsername = itm.cli.TaxOfficeUsername;
        //            ocustomer.TaxOfficePassword = itm.cli.TaxOfficePassword;
        //            ocustomer.CustomerOfficeId = itm.cli.CustomerOfficeId;
        //            ocustomer.MasterIdentifier = itm.cli.MasterIdentifier;
        //            ocustomer.IsActivationCompleted = itm.e.IsActivationCompleted ?? 0;
        //            var taxreturn = db.MainOfficeConfigurations.Where(x => x.emp_CustomerInformation_ID == itm.e.Id).Select(x => x).FirstOrDefault();
        //            ocustomer.IsTaxReturn = taxreturn == null ? true : taxreturn.IsSiteTransmitTaxReturns;
        //            ocustomer.StatusCode = itm.e.StatusCode;

        //            ocustomer.SalesforceParentID = itm.e.SalesforceParentID;
        //            if (itm.e.IsActivationCompleted == null || itm.e.IsActivationCompleted == 0)
        //                ocustomer.ActivationStatus = "Not Active";
        //            else
        //                ocustomer.ActivationStatus = "Active";

        //            //int IsActivationCompleted = itm.e.IsActivationCompleted ?? 0;

        //            //if (IsActivationCompleted == 1)
        //            //{
        //            //    ocustomer.AccountStatus = "Active";
        //            //}
        //            //else if (ocustomer.StatusCode == EMPConstants.Created)
        //            //{
        //            //    ocustomer.AccountStatus = "Created";
        //            //}
        //            //else if (ocustomer.StatusCode == EMPConstants.NewCustomer)
        //            //{
        //            //    ocustomer.AccountStatus = "New";
        //            //}
        //            //else if (ocustomer.StatusCode == EMPConstants.InProgress)
        //            //{
        //            //    ocustomer.AccountStatus = "In Progress";
        //            //}
        //            //else
        //            //{
        //            //    ocustomer.AccountStatus = "Not Active";
        //            //}


        //            ocustomer.CreatedDate = itm.cli.CreatedDate ?? Convert.ToDateTime("01/01/2000");
        //            ocustomer.LastUpdatedDate = itm.e.LastUpdatedDate ?? Convert.ToDateTime("01/01/2000");

        //            ocustomer.IsEnrolled = itm.e.IsEnrolled ?? false;
        //            ocustomer.EnrolledBankId = itm.e.EnrolledBankId;

        //            EnrollmentBankSelectionService serv = new EnrollmentBankSelectionInfo.EnrollmentBankSelectionService();
        //            var enrdata = serv.getEnrollmentStatusInfo(ocustomer.Id);
        //            ocustomer.ActiveBank = enrdata.BankName;
        //            ocustomer.SubmissionDate = enrdata.SubmitedDate;
        //            ocustomer.EnrollmentStatus = enrdata.SubmissionStaus;
        //            ocustomer.ApprovedBank = enrdata.ApprovedBank;
        //            ocustomer.RejectedBanks = enrdata.RejectedBanks;
        //            ocustomer.UnlockedBanks = enrdata.UnlockedBanks;
        //            string bankid = enrdata.BankId;

        //            if (ocustomer.EROType == "Single Office" && (itm.e.IsActivationCompleted != 0 && itm.e.IsActivationCompleted != null))
        //                ocustomer.IsActivated = true;
        //            else
        //            {
        //                ocustomer.IsActivated = itm.e.IsActivationCompleted.HasValue ? (itm.e.IsActivationCompleted.Value == 1 ? true : false) : false;
        //            }



        //            #region "Actions"
        //            var actions = (from s in db.EMP_ActionMaser
        //                           where s.EntityId == itm.e.EntityId && s.Status == EMPConstants.Active
        //                           orderby s.DisplayOrder
        //                           select new Actions
        //                           {
        //                               IsParent = s.ParentId,
        //                               Name = s.Name,
        //                               Display = s.ForActive
        //                           }).ToList();

        //            ocustomer.Actions = actions;
        //            #endregion


        //            //var ChildDataLst = GetChildCustomerInfo(itm.e.Id, "", Statuslst, SiteTypelst, omodel.SearchText, omodel.SearchType, Guid.Empty);
        //            var ChildDataLst = GetChildCustomerInfo(itm.e.Id, "", null, null, "", 0, omodel.UserId, omodel);
        //            ocustomer.ChaildCustomerInfo = ChildDataLst;
        //            if (ocustomer.ChaildCustomerInfo.Count > 0)
        //                ocustomer.ChildInfo = "<a id='" + itm.e.Id + "' onClick=\"fnChildInfo(this)\"><i class='fa fa-plus'></i></a>";
        //            else
        //                ocustomer.ChildInfo = "";
        //            customerlst.Add(ocustomer);
        //        }
        //        omodel.recordsTotal = CounttotalRecords;
        //        omodel.Customerlst = customerlst.OrderBy(a => a.CompanyName).ToList();
        //        return omodel;

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetSearchCustomerInformationNewGrid", omodel.UserId);
        //        return omodel;
        //    }
        //}

        public bool ExistOrNotCustomer(Guid CustomerId)
        {
            try
            {

                db = new DatabaseEntities();
                return db.emp_CustomerLoginInformation.Any(a => a.CustomerOfficeId == CustomerId);

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/ExistOrNotCustomer", CustomerId);
                return false;
            }
        }

        public List<Guid> GetCustomerInfo_ChildList(List<string> Statuslst, List<string> SiteTypelst, string SearchText, int SearchType, Guid UserName)
        {
            try
            {

                var data = (from e in db.emp_CustomerInformation
                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
                            where e.ParentId != null && cli.CrossLinkUserId != null &&
                            e.StatusCode != EMPConstants.NewCustomer
                            select new { e, cli }).ToList();

                if (Statuslst != null)
                {
                    data = data.Where(o => Statuslst.Contains(o.e.StatusCode)).ToList();
                }
                if (SiteTypelst != null)
                {
                    data = data.Where(o => SiteTypelst.Contains(o.e.EntityId.ToString())).ToList();
                }

                if (SearchType > 0)
                {
                    if (SearchType == 1)
                    {
                        data = data.Where(o => o.e.CompanyName.ToString().ToLower().Contains(SearchText.ToLower())).ToList();
                    }
                    else if (SearchType == 2)
                    {
                        data = data.Where(o => o.cli.CrossLinkUserId.ToString().ToLower().Contains(SearchText.ToLower())).ToList();
                    }
                    else if (SearchType == 3)
                    {
                        data = data.Where(o => o.e.EFIN.ToString().ToLower().Contains(SearchText.ToLower())).ToList();
                    }
                    else if (SearchType == 4)
                    {
                        data = data.Where(o => o.e.AlternativeContact != null ? o.e.AlternativeContact.ToString().ToLower().Contains(SearchText.ToLower()) : o.e.AlternativeContact == o.e.AlternativeContact).ToList();
                    }
                    else if (SearchType == 5)
                    {
                        data = data.Where(o => o.e.AlternatePhone != null ? o.e.AlternatePhone.ToString().ToLower().Contains(SearchText.ToLower()) : o.e.AlternatePhone == o.e.AlternatePhone).ToList();
                    }
                    else if (SearchType == 6)
                    {
                        data = data.Where(o => o.cli.MasterIdentifier.ToString().ToLower().Contains(SearchText.ToLower())).ToList();
                    }
                }
                return data.Select(a => a.e.ParentId ?? Guid.Empty).ToList();

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetCustomerInfo_ChildList", UserName);
                return new List<Guid>();
            }
        }

        public CreateNewUserResponse CreateNewUser(CreateNewUserModel model)
        {
            CreateNewUserResponse res = new CreateNewUserResponse();
            try
            {
                if (model.ParentCustomerId.HasValue && model.ParentCustomerId.Value != Guid.Empty)
                {
                    var logininfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == model.ParentCustomerId).FirstOrDefault();
                    if (logininfo != null)
                    {
                        var custinfo = db.emp_CustomerInformation.Where(x => x.Id == model.CustomerId).FirstOrDefault();
                        model.MasterIdentifier = logininfo.MasterIdentifier;
                        res = CreateNewUserApi(model, PasswordManager.DecryptText(logininfo.CrossLinkPassword), res, logininfo.CrossLinkUserId, logininfo.CLAccountId, logininfo.CLAccountPassword, logininfo.CLLogin, model.MasterIdentifier, model.ParentCustomerId.Value, custinfo.EntityId.Value);
                        if (res.Status)
                        {
                            var customerlogininfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == model.CustomerId).FirstOrDefault();
                            if (customerlogininfo != null)
                            {
                                customerlogininfo.CrossLinkUserId = res.UserId;
                                customerlogininfo.CrossLinkPassword = string.IsNullOrEmpty(res.Password) ? "" : PasswordManager.CryptText(res.Password); //logininfo.CrossLinkPassword;
                                customerlogininfo.EMPPassword = string.IsNullOrEmpty(res.Password) ? "" : PasswordManager.CryptText(res.Password);
                                customerlogininfo.EMPUserId = res.UserId;
                                customerlogininfo.StatusCode = EMPConstants.Active;
                                //customerlogininfo.MasterIdentifier = res.MasterIdentifier;


                                var officeinfo = db.emp_CustomerInformation.Where(x => x.Id == model.CustomerId).FirstOrDefault();
                                if (officeinfo != null)
                                {
                                    officeinfo.StatusCode = EMPConstants.Created;
                                    //officeinfo.MasterIdentifier = res.MasterIdentifier;
                                }

                                db.SaveChanges();
                            }
                        }
                    }
                    else
                        res.Status = false;
                }
                else
                {
                    res = CreateNewUserApiService(model);
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerInformationService/CreateNewUser", model.CustomerId);
                res.Status = false;
            }
            return res;
        }

        public CreateNewUserResponse CreateNewUserApiService(CreateNewUserModel model)
        {
            CreateNewUserResponse res = new CreateNewUserResponse();
            try
            {
                var masterId = db.UtaxCrosslinkDetails.Where(x => x.CLAccountId == model.MasterIdentifier && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (masterId != null)
                {
                    var logininfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == model.CustomerId).FirstOrDefault();
                    if (logininfo != null)
                    {
                        var custinfo = db.emp_CustomerInformation.Where(x => x.Id == model.CustomerId).FirstOrDefault();
                        res = CreateNewUserApi(model, PasswordManager.DecryptText(masterId.Password), res, logininfo.CrossLinkUserId, masterId.CLAccountId, masterId.CLAccountPassword, masterId.CLLogin, model.MasterIdentifier, Guid.Empty, custinfo.EntityId.Value);
                        if (res.Status)
                        {
                            logininfo.CrossLinkUserId = res.UserId;
                            logininfo.CrossLinkPassword = string.IsNullOrEmpty(res.Password) ? "" : PasswordManager.CryptText(res.Password); //masterId.Password;
                            logininfo.EMPPassword = string.IsNullOrEmpty(res.Password) ? "" : PasswordManager.CryptText(res.Password);
                            logininfo.EMPUserId = res.UserId;
                            logininfo.StatusCode = EMPConstants.Active;
                            //logininfo.MasterIdentifier = res.MasterIdentifier;

                            var officeinfo = db.emp_CustomerInformation.Where(x => x.Id == model.CustomerId).FirstOrDefault();
                            if (officeinfo != null)
                            {
                                officeinfo.StatusCode = EMPConstants.Created;
                                if (officeinfo.IsActivationCompleted == 1 && !string.IsNullOrEmpty(logininfo.EMPPassword))
                                    officeinfo.StatusCode = EMPConstants.Active;
                            }

                            db.SaveChanges();
                        }
                    }
                }
                else
                    res.Status = false;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerInformationService/CreateNewUserApiService", model.CustomerId);
                res.Status = false;
            }
            return res;
        }

        public CreateNewUserResponse CreateNewUserApi(CreateNewUserModel model, string Password1, CreateNewUserResponse res, string xCrossLinkUserId, string CLAccountId, string CLAccountPassword, string CLLogin, string MasterIdentifier, Guid parentId, int EntityId)
        {
            try
            {
                string MasterId = "";
                string Password = "";
                int CrossLinkUserId = 0;
                new EnrollmentBankSelectionService().getxlinkCredentials(ref MasterId, ref Password, ref CrossLinkUserId, xCrossLinkUserId, CLAccountId, CLAccountPassword, CLLogin, MasterIdentifier, parentId, EntityId);


                string _accesskey = _apiObj.getAccessKey(MasterId, Password);
                if (_accesskey != "")
                {
                    AuthObject _objAuth = new AuthObject();
                    _objAuth.AccessKey = _accesskey;
                    _objAuth.UserID = MasterId;
                    XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                    if (isValid.success)
                    {
                        NewUserObject _objUser = new NewUserObject();
                        _objUser.Account = model.MasterIdentifier;
                        _objUser.AddBusinessProduct = model.AddBusinessProduct;
                        _objUser.CompanyName = model.CompanyName;
                        _objUser.Email = model.Email;
                        _objUser.Fname = model.Fname;
                        _objUser.Lname = model.Lname;
                        _objUser.Phone = model.Phone.Replace("-", "").Replace("(", "").Replace(")", "");
                        _objUser.ShippingAddress = model.ShippingAddress;
                        _objUser.ShippingCity = model.ShippingCity;
                        _objUser.ShippingState = model.ShippingState;
                        _objUser.ShippingState = model.ShippingState;
                        _objUser.ShippingZip = model.ShippingZip;
                        var result = _apiObj.CreateNewUser(_objAuth, _objUser);

                        if (result.success)
                        {
                            var userIds = _apiObj.getUserIDList(_objAuth, MasterId, 0);
                            int newid = Convert.ToInt32(result.message[1].Split(' ')[1].Trim());
                            var newuser = userIds.Where(x => x.UserID == newid).FirstOrDefault();
                            if (newuser != null)
                            {
                                res.MasterIdentifier = newuser.Account;
                                res.Message = result.message.ToList();
                                res.Password = newuser.TransmitPassword;//== null ? GetRandomPassword() : newuser.TransmitPassword;
                                res.Status = result.success;
                                res.UserId = newuser.UserID.ToString();
                            }
                            else
                                res.Status = false;
                        }
                        else
                            res.Status = false;
                    }
                    else
                        res.Status = false;
                }
                else
                    res.Status = false;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerInformationService/CreateNewUserApi", model.CustomerId);
                res.Status = false;
            }
            return res;
        }

        private string GetRandomPassword()
        {
            try
            {
                Random random = new Random();
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, 8)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            catch (Exception ex)
            {
                return "abc@1234";
            }
        }

        public bool SendEmailForNewUser(NewUserEmailRequest model)
        {
            try
            {
                var customer = db.emp_CustomerInformation.Where(x => x.Id == model.CustomerId).FirstOrDefault();
                if (customer != null)
                {
                    var logininfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == model.CustomerId).FirstOrDefault();
                    EmailNotification email = new EmailNotification();
                    email.CreatedBy = model.UserId;
                    email.CreatedDate = DateTime.Now;
                    email.EmailCC = customer.SupportNotificationEmail;
                    email.EmailContent = "";
                    email.EmailSubject = EMPConstants.NewUserMailSubject;
                    email.EmailTo = customer.PrimaryEmail;
                    email.EmailType = (int)EMPConstants.EmailTypes.NewUser;
                    email.IsSent = false;
                    email.Parameters = "$|$" + logininfo.EMPUserId + "$|$" + PasswordManager.DecryptText(logininfo.EMPPassword) + "$|$" + logininfo.CrossLinkUserId + "$|$" + PasswordManager.DecryptText(logininfo.CrossLinkPassword);
                    db.EmailNotifications.Add(email);
                    db.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerInformationService/SendEmailForNewUser", model.UserId);
                return false;
            }
        }

        public EMPCustomerLogin GetEMPCustomerLogin(Guid ParentId)
        {
            EMPCustomerLogin EMPCustomerLogInfo = new EMPCustomerLogin();
            var parentdata = db.emp_CustomerInformation
          .Join(db.emp_CustomerLoginInformation, custinfo => custinfo.Id, custlog => custlog.CustomerOfficeId,
                              (custinfo, custlog) => new
                              {
                                  custinfo,
                                  custlog
                              })
          .Where(o => o.custinfo.Id == ParentId)
          .Select(g => new
          {
              Id = g.custinfo.Id,
              CrossLinkUserId = g.custlog.CrossLinkUserId,
              IsMsoUser = g.custinfo.IsMSOUser ?? false,
              ParentId = g.custinfo.ParentId ?? Guid.Empty
          }).FirstOrDefault();

            if (parentdata != null)
            {
                Guid NewParentId = Guid.Empty;
                NewParentId = ParentId;

                var parentdata2 = db.emp_CustomerInformation
                     .Join(db.emp_CustomerLoginInformation, custinfo => custinfo.Id, custlog => custlog.CustomerOfficeId,
                                         (custinfo, custlog) => new { custinfo, custlog })
                     .Where(o => o.custinfo.ParentId == NewParentId && o.custlog.CrossLinkUserId != null)
                     .Select(g => new
                     {
                         CrossLinkUserId = g.custlog.CrossLinkUserId,
                     }).ToList();

                string UserIdChar = "A";
                if (parentdata2.ToList().Count > 0)
                {
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "A")).ToList().Count > 0)
                    {
                        UserIdChar = "B";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "B")).ToList().Count > 0)
                    {
                        UserIdChar = "C";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "C")).ToList().Count > 0)
                    {
                        UserIdChar = "D";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "D")).ToList().Count > 0)
                    {
                        UserIdChar = "E";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "E")).ToList().Count > 0)
                    {
                        UserIdChar = "F";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "F")).ToList().Count > 0)
                    {
                        UserIdChar = "G";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "G")).ToList().Count > 0)
                    {
                        UserIdChar = "H";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "H")).ToList().Count > 0)
                    {
                        UserIdChar = "I";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "I")).ToList().Count > 0)
                    {
                        UserIdChar = "J";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "J")).ToList().Count > 0)
                    {
                        UserIdChar = "K";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "K")).ToList().Count > 0)
                    {
                        UserIdChar = "L";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "L")).ToList().Count > 0)
                    {
                        UserIdChar = "M";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "M")).ToList().Count > 0)
                    {
                        UserIdChar = "N";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "N")).ToList().Count > 0)
                    {
                        UserIdChar = "O";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "O")).ToList().Count > 0)
                    {
                        UserIdChar = "P";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "P")).ToList().Count > 0)
                    {
                        UserIdChar = "Q";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "Q")).ToList().Count > 0)
                    {
                        UserIdChar = "R";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "R")).ToList().Count > 0)
                    {
                        UserIdChar = "S";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "S")).ToList().Count > 0)
                    {
                        UserIdChar = "T";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "T")).ToList().Count > 0)
                    {
                        UserIdChar = "U";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "U")).ToList().Count > 0)
                    {
                        UserIdChar = "V";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "V")).ToList().Count > 0)
                    {
                        UserIdChar = "W";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "W")).ToList().Count > 0)
                    {
                        UserIdChar = "X";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "X")).ToList().Count > 0)
                    {
                        UserIdChar = "Y";
                    }
                    if (parentdata2.Where(o => o.CrossLinkUserId.Contains(parentdata.CrossLinkUserId + "Y")).ToList().Count > 0)
                    {
                        UserIdChar = "Z";
                    }
                }

                EMPCustomerLogInfo.EMPUserId = parentdata.CrossLinkUserId + UserIdChar;
                EMPCustomerLogInfo.EMPPassword = parentdata.CrossLinkUserId + UserIdChar;

                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var stringChars = new char[8];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                var finalString = new String(stringChars);
                string Password = PasswordManager.CryptText(finalString);
                EMPCustomerLogInfo.EMPPassword = Password;
            }
            return EMPCustomerLogInfo;
        }

        public bool GetIsCustomerTaxReturn(Guid CustomerID)
        {
            try
            {
                var tax = db.MainOfficeConfigurations.Where(x => x.emp_CustomerInformation_ID == CustomerID).FirstOrDefault();
                if (tax != null)
                {
                    return tax.IsSiteTransmitTaxReturns;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int checkActivationandCreateUser(Guid CustomerId, Guid UserId)
        {
            try
            {
                string MasterId = "";
                bool _continue = false;
                var customer = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                if (customer != null)
                {
                    if (customer.IsActivationCompleted == 1)
                    {
                        var login = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == CustomerId).FirstOrDefault();
                        if (login != null)
                        {
                            if (string.IsNullOrEmpty(login.EMPUserId))
                            {
                                _continue = true;
                            }
                        }
                    }
                    else
                        return 2;
                }

                if (_continue)
                {
                    CreateNewUserModel model = new CreateNewUserModel();
                    model.CompanyName = customer.CompanyName;
                    model.CustomerId = CustomerId;
                    model.Email = customer.PrimaryEmail;
                    model.Fname = customer.BusinesOwnerLastName;
                    model.Lname = customer.BusinessOwnerFirstName;
                    model.MasterIdentifier = MasterId;
                    model.ParentCustomerId = null;
                    model.Phone = customer.OfficePhone;
                    model.ShippingAddress = customer.ShippingAddress1;
                    model.ShippingCity = customer.ShippingCity;
                    model.ShippingState = customer.ShippingState;
                    model.ShippingZip = customer.ShippingZipCode;
                    var res = CreateNewUserApiService(model);
                    if (res.Status)
                    {
                        SendEmailForNewUser(new NewUserEmailRequest() { CustomerId = CustomerId, UserId = UserId });
                        return 1;
                    }
                    else
                    {
                        customer.StatusCode = EMPConstants.NewCustomer;
                        customer.LastUpdatedBy = UserId;
                        customer.LastUpdatedDate = DateTime.Now;
                        db.SaveChanges();
                        return -1;
                    }
                }
                else
                {
                    var customer1 = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                    if (customer1 != null)
                    {
                        if (customer.IsActivationCompleted == 1)
                        {
                            var login = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == CustomerId).FirstOrDefault();
                            if (login != null)
                            {
                                if (string.IsNullOrEmpty(login.EMPUserId))
                                {
                                    customer1.StatusCode = EMPConstants.NewCustomer;
                                    customer1.LastUpdatedBy = UserId;
                                    customer1.LastUpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    return -1;
                                }
                                else if (!string.IsNullOrEmpty(login.EMPUserId))
                                {
                                    customer1.StatusCode = EMPConstants.Active;
                                    customer1.LastUpdatedBy = UserId;
                                    customer1.LastUpdatedDate = DateTime.Now;
                                    db.SaveChanges();
                                    return 3;
                                }
                                return 3;
                            }
                        }
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerInformationService/checkActivationandCreateUser", CustomerId);
                return 0;
            }
        }

        public bool HoldUnHoldCustomer(Guid CustomerId, Guid UserId, string Description)
        {
            try
            {
                var customer = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                if (customer != null)
                {
                    var loginfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == CustomerId).Select(x => new { x.EMPUserId, x.MasterIdentifier }).FirstOrDefault();
                    var parentuserid = "";
                    if (customer.ParentId.HasValue && customer.ParentId != Guid.Empty)
                    {
                        parentuserid = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == customer.ParentId).Select(x => x.EMPUserId).FirstOrDefault();
                    }
                    string strDescription = !(customer.IsHold ?? false) ? "Hold" : "Unhold";

                    customer.IsHold = !(customer.IsHold ?? false);
                    customer.LastUpdatedBy = UserId;
                    customer.LastUpdatedDate = DateTime.Now;
                    customer.HoldDescription = Description;
                    db.SaveChanges();

                    EmailNotification notification = new EmailNotification();
                    notification.CreatedBy = UserId;
                    notification.CreatedDate = DateTime.Now;
                    notification.EmailCC = "";
                    notification.EmailContent = "";
                    notification.EmailSubject = "User ID " + loginfo.EMPUserId + " site on " + strDescription;
                    notification.EmailTo = strDescription == "Hold" ? EMPConstants.SupportutaxEmail : EMPConstants.accountutaxEmail;
                    notification.EmailType = (int)EMPConstants.EmailTypes.HoldUnhold;
                    notification.IsSent = false;
                    notification.Parameters = loginfo.EMPUserId + "$|$" + loginfo.MasterIdentifier + "$|$" + parentuserid + "$|$" + strDescription;
                    db.EmailNotifications.Add(notification);
                    db.SaveChanges();

                    if (strDescription == "Hold")
                    {
                        EmailNotification notification1 = new EmailNotification();
                        notification1.CreatedBy = UserId;
                        notification1.CreatedDate = DateTime.Now;
                        notification1.EmailCC = "";
                        notification1.EmailContent = "";
                        notification1.EmailSubject = "Your site has been placed on Hold";
                        notification1.EmailTo = customer.PrimaryEmail;
                        notification1.EmailType = (int)EMPConstants.EmailTypes.HoldUser;
                        notification1.IsSent = false;
                        notification1.Parameters = customer.BusinessOwnerFirstName + " " + customer.BusinesOwnerLastName + "$|$" + Description;
                        db.EmailNotifications.Add(notification1);
                        db.SaveChanges();

                        string caseStatus = "Open";
                        //Description = "this site has been removed from Hold";

                        var strOwnerID = (from s in db.emp_CustomerInformation
                                          join y in db.SalesYearMasters on s.SalesYearID equals y.Id
                                          where s.Id == CustomerId
                                          select new { s.SalesforceAccountID, y.SalesYear }).FirstOrDefault();
                        SaveEmpCsrData(CustomerId, Description, strOwnerID.SalesforceAccountID, strOwnerID.SalesYear.Value.ToString(), caseStatus);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddSO(ImportPartner info)
        {
            var isexist = db.emp_CustomerInformation.Where(x => x.CompanyName == info.CompanyName && x.StatusCode != EMPConstants.InActive).FirstOrDefault();
            if (isexist != null)
                return false;

            var isexist1 = db.emp_CustomerInformation.Where(x => x.EFIN == info.EFIN && x.StatusCode != EMPConstants.InActive).FirstOrDefault();
            if (isexist1 != null)
                return false;

            emp_CustomerInformation cust = new emp_CustomerInformation();
            cust.AccountStatus = EMPConstants.NewCustomer;
            cust.AlternatePhone = "";
            cust.BusinesOwnerLastName = info.LName;
            cust.BusinessOwnerFirstName = info.FName;
            cust.CompanyName = info.CompanyName;
            cust.CreatedDate = DateTime.Now;
            cust.EFIN = info.EFIN;
            cust.EFINStatus = info.EFINStatus;
            cust.EntityId = (int)EMPConstants.Entity.SO;
            cust.EROType = "Single Office";
            cust.Id = Guid.NewGuid();
            cust.IsAdditionalEFINAllowed = true;
            cust.IsMSOUser = false;
            cust.IsVerified = false;
            cust.MasterIdentifier = info.MasterID;
            cust.MSOUser = false;
            cust.OfficePhone = info.Phone;
            cust.PhysicalAddress1 = info.Address1;
            cust.PhysicalAddress2 = info.Address2;
            cust.PhysicalCity = info.City;
            cust.PhysicalState = info.State;
            cust.PhysicalZipCode = info.Zip;
            cust.PrimaryEmail = info.Email;
            cust.SalesforceAccountID = "";
            cust.SalesforceOpportunityID = "";
            cust.SalesforceParentID = "";
            cust.SalesYearID = db.SalesYearMasters.Where(x => x.StatusCode == EMPConstants.Active).Select(x => x.Id).FirstOrDefault();
            cust.ShippingAddress1 = info.Address1;
            cust.ShippingAddress2 = info.Address2;
            cust.ShippingAddressSameAsPhysicalAddress = true;
            cust.ShippingCity = info.City;
            cust.ShippingState = info.State;
            cust.ShippingZipCode = info.Zip;
            cust.StatusCode = EMPConstants.NewCustomer;
            db.emp_CustomerInformation.Add(cust);
            db.SaveChanges();

            emp_CustomerLoginInformation login = new emp_CustomerLoginInformation();
            login.CustomerOfficeId = cust.Id;
            login.Id = Guid.NewGuid();
            login.MasterIdentifier = info.MasterID;
            login.OfficePortalUrl = info.OfficePortalUrl;
            login.StatusCode = EMPConstants.Active;
            db.emp_CustomerLoginInformation.Add(login);
            db.SaveChanges();

            CreateNewUserModel newuser = new CreateNewUserModel();
            newuser.AddBusinessProduct = true;
            newuser.CompanyName = info.CompanyName;
            newuser.CustomerId = cust.Id;
            newuser.Email = info.Email;
            newuser.Fname = info.FName;
            newuser.Lname = info.LName;
            newuser.MasterIdentifier = info.MasterID;
            newuser.ParentCustomerId = Guid.Empty;
            newuser.Phone = info.Phone;
            newuser.ShippingAddress = info.Address1;
            newuser.ShippingCity = info.City;
            newuser.ShippingState = info.State;
            newuser.ShippingZip = info.Zip;
            CreateNewUser(newuser);

            return true;
        }




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
            public emp_CustomerInformation e { get; set; }
            public emp_CustomerLoginInformation cli { get; set; }
        }
    }
}