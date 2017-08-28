using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.CustomerLoginInformation;

namespace EMPPortal.Transactions.CustomerInformation
{
    public class CustomerModel
    {
        public System.Guid Id { get; set; }
        public System.Guid ParentId { get; set; }
        public string str_ParentId { get; set; }
        public string CompanyName { get; set; }
        public string AccountStatus { get; set; }
        public Nullable<bool> Feeder { get; set; }
        public string BusinessOwnerFirstName { get; set; }
        public string OfficePhone { get; set; }
        public string AlternatePhone { get; set; }
        public string Primaryemail { get; set; }
        public string SupportNotificationemail { get; set; }
        public string EROType { get; set; }
        public string AlternativeContact { get; set; }
        public Nullable<int> EFIN { get; set; }
        public Nullable<int> EFINStatus { get; set; }
        public string PhysicalAddress1 { get; set; }
        public string PhysicalAddress2 { get; set; }
        public string PhysicalZipcode { get; set; }
        public string PhysicalCity { get; set; }
        public string PhysicalState { get; set; }
        public bool ShippingAddressSameAsPhysicalAddress { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingZipcode { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public Nullable<System.Guid> PhoneTypeId { get; set; }
        public Nullable<System.Guid> TitleId { get; set; }
        public Nullable<System.Guid> AlternativeType { get; set; }
        public string PhoneType { get; set; }
        public string ContactTitle { get; set; }
        public Nullable<System.Guid> SalesYearID { get; set; }
        public Nullable<int> EntityId { get; set; }
        public Nullable<int> BaseEntityId { get; set; }
        public string StatusCode { get; set; }
        public string EMPUserId { get; set; }
        public string LoginId { get; set; }
        public Nullable<int> LoginEFIN { get; set; }
        public string MasterIdentifier { get; set; }
        public string CrossLinkUserId { get; set; }
        public string CrossLinkPassword { get; set; }
        public string OfficePortalUrl { get; set; }
        public string TaxOfficeUsername { get; set; }
        public string TaxOfficePassword { get; set; }
        public string EMPPassword { get; set; }
        public Nullable<System.Guid> CustomerOfficeId { get; set; }

        public List<CustomerModel> ChaildCustomerInfo { get; set; }
        public string ChildInfo { get; set; }
        public int ChaildCustomerInfoCount { get; set; }
        public bool IsAdditionalEFINAllowed { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public decimal TotalServiceFee { get; set; }
        public decimal TotalTransFee { get; set; }
        public string ServiceTooltip { get; set; }
        public string TransTooltip { get; set; }

        public string SalesforceParentID { get; set; }
        public string ActivationStatus { get; set; }
        public bool IsActivated { get; set; }
        public bool IsMSOUser { get; set; }
        public bool IsTaxReturn { get; set; }

        public List<CustomerRecenltyModel> RecentlyCreatedList { get; set; }
        public List<CustomerRecenltyModel> RecentlyUpdatedList { get; set; }
        public int IsActivationCompleted { get; set; }

        public string ActiveBank { get; set; }
        public string SubmissionDate { get; set; }
        public string EnrollmentStatus { get; set; }
        public string ApprovedBank { get; set; }
        public string RejectedBanks { get; set; }
        public string UnlockedBanks { get; set; }

        public bool IsEnrolled { get; set; }
        public Guid? EnrolledBankId { get; set; }

        public Guid? SalesYearGroupId { get; set; }
        public Nullable<bool> IsVerified { get; set; }

        public List<Actions> Actions { get; set; }
    }
    
    public class Actions
    {
        public string Name { get; set; }
        public Int64 IsParent { get; set; }
        public int Display { get; set; }
    }

    public class CustomerRecenltyModel
    {
        public string Name { get; set; }
        public DateTime updateDatetime { get; set; }
    }

    public class CustomerInfoNewGrid
    {
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public string sortColumn { get; set; }
        public string sortColumnDir { get; set; }
        public List<CustomerModel> Customerlst { get; set; }
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }

        public Guid UserId { get; set; }
        public string Status { get; set; }
        public string SiteType { get; set; }
        public string BankPartner { get; set; }
        public string EnrollmentStatus { get; set; }
        public string OnBoardingStatus { get; set; }
        public string SearchText { get; set; }
        public int SearchType { get; set; }
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

    public class CreateNewUserResponse
    {
        public List<string> Message { get; set; }
        public bool Status { get; set; }
        public string MasterIdentifier { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }

    public class NewUserEmailRequest
    {
        public Guid CustomerId { get; set; }
        public Guid? UserId { get; set; }
    }
}
