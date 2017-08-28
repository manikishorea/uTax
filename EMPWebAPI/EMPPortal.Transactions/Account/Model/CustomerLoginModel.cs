using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.Token.DTO;

namespace EMPPortal.Transactions.Account.Model
{
    public class CustomerLoginModel : CoreModel
    {
        public System.Guid Id { get; set; }
        public Nullable<int> EFIN { get; set; }
        public Nullable<int> EFINStatus { get; set; }
        public string MasterIdentifier { get; set; }
        public string CrossLinkUserId { get; set; }
        public string CrossLinkPassword { get; set; }
        public string OfficePortalUrl { get; set; }
        public string TaxOfficeUsername { get; set; }
        public string TaxOfficePassword { get; set; }
        public string EMPPassword { get; set; }
        public string EMPUserId { get; set; }
        public bool IsChangedPassword { get; set; }
        public bool IsSetSecurityQuestion { get; set; }
        public TokenDTO Token { get; set; }
        public Nullable<System.Guid> CustomerOfficeId { get; set; }
        public int IsActivationCompleted { get; set; }
        public bool IsEnrollmentSubmit { get; set; }
        public int? EntityID { get; set; }
        public int? BaseEntityId { get; set; }
        public string ParentID { get; set; }
        public string SalesYearID { get; set; }
        //public Nullable<int> EntityDisplayID { get; set; }
        public bool CanSubSiteLoginToEmp { get; set; }
        public string SupParentID { get; set; }
        public bool IsMSOUser { get; set; }
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
