using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.CustomerLoginInformation
{
   public class CustomerLoginInformationModel : CoreModel
    {
        public string Id { get; set; }
        public Nullable<int> EFIN { get; set; }
        public Nullable<int> EFINStatus { get; set; }
        public string EFINStatusText { get; set; }
        public Nullable<int> EntityId { get; set; }
        public string MasterIdentifier { get; set; }
        public string CrossLinkUserId { get; set; }
        public string CrossLinkPassword { get; set; }
        public string OfficePortalUrl { get; set; }
        public string TaxOfficeUsername { get; set; }
        public string TaxOfficePassword { get; set; }
        public string EMPPassword { get; set; }
        public string EMPUserId { get; set; }
        public string CompanyName { get; set; }
        public string BusinessOwnerFirstName { get; set; }
        public string BusinessOwnerLastName { get; set; }
        public string PhysicalAddress1 { get; set; }
        public bool IsMSOUser { get; set; }
        public string CityStateZip { get; set; }
        public Nullable<System.Guid> CustomerOfficeId { get; set; }

        public string SalesforceParentID { get; set; }
        public string TransmitType { get; set; }
        public string MSO { get; set; }
        public string Bank { get; set; }
        public bool IsAdditionalEFINAllowed { get; set; }
        public string ParentId { get; set; }

        public string MasterIdentifierPassword { get; set; }

        public string CLAccountId { get; set; }
        public string CLLogin { get; set; }
        public string CLAccountPassword { get; set; }
    }
}
