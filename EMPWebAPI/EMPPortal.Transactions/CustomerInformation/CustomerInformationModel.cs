using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.CustomerInformation
{
    public class CustomerInformationModel : CoreModel
    {
        public System.Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string AccountStatus { get; set; }
        public Nullable<bool> Feeder { get; set; }
        public string BusinessOwnerFirstName { get; set; }
        public string BusinessOwnerLastName { get; set; }
        public string OfficePhone { get; set; }
        public string AlternatePhone { get; set; }
        public string Primaryemail { get; set; }
        public string SupportNotificationemail { get; set; }
        public string EROType { get; set; }
        public string AlternativeContact { get; set; }
        public Nullable<int> EFIN { get; set; }
        public string EFINStatusText { get; set; }
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
        public Nullable<System.Guid> ParentId { get; set; }
        public string OfficePortalUrl { get; set; }
        public Nullable<System.Guid> SiteMapId { get; set; }

        public string SalesforceParentID { get; set; }
        public string MasterIdentifier { get; set; }

        public bool IsVerified { get; set; }
        public Nullable<bool> IsMSOUser { get; set; }
        public Nullable<bool> IsEnrollmentSubmit { get; set; }
        public Nullable<int> IsActivationCompleted { get; set; }
        public Nullable<bool> IsAdditionalEFINSubSite { get; set; }

        public bool IsEnrollSubmitted { get; set; }

        public bool? IsNotCollectingFee { get; set; }
        public string SalesforceOpportunityID { get; set; }

        public bool IsHold { get; set; }
        public Guid? AlternativePhoneType { get; set; }
    }

    public class NewCustomersModel
    {
        public string EROType { get; set; }
        public string CompanyName { get; set; }
        public string BusinessOwnerFirstName { get; set; }
        public string OfficePhone { get; set; }
        public string Primaryemail { get; set; }
        public string StatusCode { get; set; }
        public Guid Id { get; set; }
        public int EntityId { get; set; }
    }

    public class ImportPartner
    {
        public string CompanyName { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int EFINStatus { get; set; }
        public int EFIN { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string MasterID { get; set; }
        public string OfficePortalUrl { get; set; }
    }
}
