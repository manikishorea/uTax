using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using System.Data.Entity;

using EMPPortal.Transactions.Customer.DTO;
using EMMPortal.Transactions.Customer.DTO;
using EMPPortal.Transactions.Customer;
using EMPPortal.Transactions.CustomerInformation;
using EMP.Core.Utilities;
using EMPPortal.Transactions.EnrollmentBankSelectionInfo;
using EMPPortal.Core.Utilities;

namespace EMPPortal.Transactions.Customer
{
    public class CustomerService : ICustomerService
    {
        // private readonly uTaxDBEntities _db = new uTaxDBEntities();
        // private readonly CustomerDTO _entity = new CustomerDTO();

        //public CustomerService(uTaxDBEntities db, CustomerDTO entity)
        //{
        //    _db = db;
        //    _entity = entity;
        //}

        public DatabaseEntities _db = new DatabaseEntities();
        public CustomerDTO _customer = new CustomerDTO();

        public IQueryable<CustomerDTO> GetAllCustomer()
        {
            var customer = _db.CustomerMasters.Select(o => new CustomerDTO
            {
                Id = o.Id,
                Name = o.Name,
                EntityId = o.EntityId,
                // EntityName = o.EntityMaster.Name,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();

            return customer;
        }

        public async Task<CustomerDTO> GetCustomer(Guid Id)
        {
            var customer = _db.CustomerMasters.Select(o => new CustomerDTO
            {
                Id = o.Id,
                Name = o.Name,
                EntityId = o.EntityId,
                //  EntityName = o.EntityMaster.Name,
                StatusCode = o.StatusCode
            }).SingleOrDefaultAsync(o => o.Id == Id);

            return await customer;
        }

        public async Task<CustomerDetailDTO> GetCustomerDetail(Guid Id)
        {
            var customer = _db.CustomerMasters.Select(o => new CustomerDetailDTO
            {
                Id = o.Id,
                Name = o.Name,
                EntityId = o.EntityId,
                // EntityName = o.EntityMaster.Name,
                StatusCode = o.StatusCode
            }).SingleOrDefaultAsync(o => o.Id == Id);

            return await customer;
        }

        public CustomerGrid GetCustomerSearch(int pageno, int maxcount)
        {

            CustomerGrid CustomerGr = new CustomerGrid();
            try
            {
                //string Status, string SiteType, string BankPartner, string EnrollmentStatus, string OnBoardingStatus
                DatabaseEntities db = new DatabaseEntities();
                //List<string> Statuslst = !string.IsNullOrEmpty(Status) ? Status.Split(',').ToList() : null;
                //List<string> SiteTypelst = !string.IsNullOrEmpty(SiteType) ? SiteType.Split(',').ToList() : null;
                //List<string> BankPartnerlst = !string.IsNullOrEmpty(BankPartner) ? BankPartner.Split(',').ToList() : null;
                //List<string> EnrollmentStatuslst = !string.IsNullOrEmpty(EnrollmentStatus) ? EnrollmentStatus.Split(',').ToList() : null;
                //List<string> OnBoardingStatuslst = !string.IsNullOrEmpty(OnBoardingStatus) ? OnBoardingStatus.Split(',').ToList() : null;

                var data = (from e in db.emp_CustomerInformation
                            join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
                            where e.ParentId == null && cli.CrossLinkUserId != null
                            && e.StatusCode != EMPConstants.NewCustomer
                            orderby e.CompanyName ascending
                            select new { e, cli }).Skip((pageno - 1) * maxcount).Take(maxcount).ToList();//.ToList().OrderBy(a => a.e.CompanyName).ToList();

                //if (data.Count > 0)
                //{
                //    if (data.Any(a => a.cli.CustomerOfficeId == UserName))
                //    {
                //        data = data.Where(a => a.cli.CustomerOfficeId == UserName).ToList();
                //    }
                //}

                CustomerGr.totalrecords = (from e in db.emp_CustomerInformation
                                           join cli in db.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
                                           where e.ParentId == null && cli.CrossLinkUserId != null
                                           && e.StatusCode != EMPConstants.NewCustomer
                                           orderby e.CompanyName ascending
                                           select new { e, cli }).ToList().Count;


                List<CustomerModel> customerlst = new List<CustomerModel>();
                foreach (var itm in data)
                {
                    CustomerModel ocustomer = new CustomerModel();
                    ocustomer.Id = itm.e.Id;
                    ocustomer.ParentId = itm.e.ParentId ?? Guid.Empty;
                    ocustomer.CompanyName = itm.e.CompanyName;
                    ocustomer.AccountStatus = itm.e.AccountStatus;
                    ocustomer.SalesforceParentID = itm.e.SalesforceParentID;
                    ocustomer.MasterIdentifier = itm.e.MasterIdentifier;
                    ocustomer.Feeder = itm.e.Feeder;
                    ocustomer.BusinessOwnerFirstName = itm.e.BusinessOwnerFirstName;
                    ocustomer.OfficePhone = itm.e.OfficePhone;
                    ocustomer.AlternatePhone = itm.e.AlternatePhone;
                    ocustomer.Primaryemail = itm.e.PrimaryEmail;
                    ocustomer.SupportNotificationemail = itm.e.SupportNotificationEmail;
                    ocustomer.EROType = itm.e.EROType;
                    ocustomer.AlternativeContact = itm.e.AlternativeContact ?? "";
                    ocustomer.EFIN = itm.e.EFIN;
                    ocustomer.EFINStatus = itm.e.EFINStatus;
                    ocustomer.PhysicalAddress1 = itm.e.PhysicalAddress1;
                    ocustomer.PhysicalAddress2 = itm.e.PhysicalAddress2;
                    ocustomer.PhysicalZipcode = itm.e.PhysicalZipCode;
                    ocustomer.PhysicalCity = itm.e.PhysicalCity;
                    ocustomer.PhysicalState = itm.e.PhysicalState;
                    ocustomer.ShippingAddress1 = itm.e.ShippingAddress1;
                    ocustomer.ShippingAddress2 = itm.e.ShippingAddress2;
                    ocustomer.ShippingZipcode = itm.e.ShippingZipCode;
                    ocustomer.ShippingCity = itm.e.ShippingCity;
                    ocustomer.ShippingState = itm.e.ShippingState;
                    ocustomer.PhoneTypeId = itm.e.PhoneTypeId;
                    ocustomer.TitleId = itm.e.TitleId;
                    ocustomer.AlternativeType = itm.e.AlternativeType;
                    ocustomer.PhoneType = db.PhoneTypeMasters.Where(a => a.Id == itm.e.PhoneTypeId).Select(a => a.PhoneType).FirstOrDefault();
                    ocustomer.ContactTitle = db.ContactPersonTitleMasters.Where(a => a.Id == itm.e.TitleId).Select(a => a.ContactPersonTitle).FirstOrDefault() ?? "";
                    ocustomer.EntityId = itm.e.EntityId ?? 0;
                    ocustomer.SalesYearID = itm.e.SalesYearID ?? Guid.Empty;
                    ocustomer.BaseEntityId = db.EntityMasters.Where(a => a.Id == itm.e.EntityId).Select(a => a.BaseEntityId).FirstOrDefault();
                    ocustomer.LoginId = itm.cli.Id.ToString();
                    //ocustomer.LoginEFIN = itm.cli.EFIN;
                    ocustomer.MasterIdentifier = itm.cli.MasterIdentifier;
                    ocustomer.CrossLinkUserId = itm.cli.CrossLinkUserId ?? "";
                    ocustomer.CrossLinkPassword = itm.cli.CrossLinkPassword;
                    ocustomer.OfficePortalUrl = itm.cli.OfficePortalUrl;
                    ocustomer.TaxOfficeUsername = itm.cli.TaxOfficeUsername;
                    ocustomer.TaxOfficePassword = itm.cli.TaxOfficePassword;
                    ocustomer.CustomerOfficeId = itm.cli.CustomerOfficeId;
                    ocustomer.MasterIdentifier = itm.cli.MasterIdentifier;
                    ocustomer.IsActivationCompleted = itm.e.IsActivationCompleted ?? 0;

                    ocustomer.SalesforceParentID = itm.e.SalesforceParentID;
                    if (itm.e.IsActivationCompleted == null || itm.e.IsActivationCompleted == 0)
                        ocustomer.ActivationStatus = "Not Active";
                    else
                        ocustomer.ActivationStatus = "Active";

                    ocustomer.CreatedDate = itm.cli.CreatedDate ?? Convert.ToDateTime("01/01/2000");
                    ocustomer.LastUpdatedDate = itm.e.LastUpdatedDate ?? Convert.ToDateTime("01/01/2000");

                    ocustomer.IsEnrolled = itm.e.IsEnrolled ?? false;
                    ocustomer.EnrolledBankId = itm.e.EnrolledBankId;

                    //EnrollmentBankSelectionService serv = new EnrollmentBankSelectionInfo.EnrollmentBankSelectionService();
                    //var enrdata = serv.getEnrollmentStatusInfo(ocustomer.Id);
                    //ocustomer.ActiveBank = enrdata.BankName;
                    //ocustomer.SubmissionDate = enrdata.SubmitedDate;
                    //ocustomer.EnrollmentStatus = enrdata.SubmissionStaus;
                    //ocustomer.ApprovedBank = enrdata.ApprovedBank;
                    //ocustomer.RejectedBanks = enrdata.RejectedBanks;

                    var ChildDataLst = db.emp_CustomerInformation.Where(o => o.ParentId == ocustomer.Id).ToList();
                    ocustomer.ChaildCustomerInfoCount = ChildDataLst.ToList().Count;

                    if (ocustomer.EROType == "Single Office")
                        ocustomer.IsActivated = true;
                    else
                    {
                        ocustomer.IsActivated = itm.e.IsActivationCompleted.HasValue ? (itm.e.IsActivationCompleted.Value == 1 ? true : false) : false;
                    }

                    customerlst.Add(ocustomer);
                }
                CustomerGr.CustomerModel = new List<CustomerModel>();
                CustomerGr.CustomerModel = customerlst;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "Configuration/GetCustomerSearch", Guid.Empty);
            }
            return CustomerGr;
        }

    }

    public class CustomerGrid
    {
        public int totalrecords { get; set; }
        public List<CustomerModel> CustomerModel { get; set; }
    }
}