using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.SubSiteOfficeConfiguration.DTO
{
    public class SubSiteOfficeConfigDTO : CoreModel
    {
        public string Id { get; set; }
        public string RefId { get; set; }
        public Nullable<bool> EFINListedOtherOffice { get; set; }
        public Nullable<bool> SiteOwnthisEFIN { get; set; }
        public string EFINOwnerSite { get; set; }
        public string SOorSSorEFIN { get; set; }
        public Nullable<bool> SubSiteSendTaxReturn { get; set; }
        public Nullable<bool> SiteanMSOLocation { get; set; }
        public Nullable<bool> IsMainSiteTransmitTaxReturn { get; set; }
        public Nullable<int> NoofTaxProfessionals { get; set; }
        public Nullable<bool> IsSoftwareOnNetwork { get; set; }
        public Nullable<int> NoofComputers { get; set; }
        public Nullable<int> PreferredLanguage { get; set; }
        public Nullable<int> iIsSubSiteSendTaxReturn { get; set; }
        public Nullable<bool> CanSubSiteLoginToEmp { get; set; }
        public bool IsBusinessSoftware { get; set; }
        public bool IsSharingEFIN { get; set; }
    }
}
