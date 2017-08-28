using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMPPortal.Models
{
    public class LoginModel
    {
        public string Id { get; set; }
        public string CustomerOfficeId { get; set; }
        public string EFIN { get; set; }
        public string MasterIdentifier { get; set; }
        public string CrossLinkUserId { get; set; }
        public string CrossLinkPassword { get; set; }
        public string OfficePortalUrl { get; set; }
        public string TaxOfficeUsername { get; set; }
        public string TaxOfficePassword { get; set; }
        public string EMPPassword { get; set; }
        public string EMPUserId { get; set; }
        public TokenModel Token { get; set; }
        public bool RememberMe { get; set; }
        public bool IsChangedPassword { get; set; }
        public bool IsSetSecurityQuestion { get; set; }
        public int EntityId { get; set; }
        public int EFINStatus { get; set; }
        public int IsActivationCompleted { get; set; }
        public string ParentID { get; set; }
        public string CurrentPassword { get; set; }

        public string SalesYearID { get; set; }
        public bool CanSubSiteLoginToEmp { get; set; }
        public Nullable<int> BaseEntityId { get; set; }
        // public string SupParentID { get; set; }
        public bool IsMSOUser { get; set; }
        public bool IsEnrollmentSubmit { get; set; }
        public bool IsVerified { get; set; }
        public bool EFINOwnerUserId { get; set; }
        public string userip { get; set; }
        public bool uTaxNotCollectingSBFee { get; set; }
        public bool IsTaxReturn { get; set; }

        public bool IsOfficeMgmt { get; set; }
        public bool IsnewCustomers { get; set; }
        public bool FeeReport { get; set; }
        public bool NoBankApp { get; set; }
        public bool Enrollstatus { get; set; }
        public bool LoginReport { get; set; }

        public Guid BankId { get; set; }

        public string Message { get; set; }
        public bool IsHold { get; set; }
    }

}