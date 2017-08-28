using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.EnrollmentOfficeConfig.DTO
{
    public class EnrollmentOfficeConfigDTO : CoreModel
    {
        public Nullable<System.Guid> Id { get; set; }
        public Nullable<System.Guid> CustomerId { get; set; }
        public Nullable<bool> IsMainSiteTransmitTaxReturn { get; set; }
        public Nullable<int> NoofTaxProfessionals { get; set; }
        public Nullable<bool> IsSoftwareOnNetwork { get; set; }
        public Nullable<int> NoofComputers { get; set; }
        public Nullable<int> PreferredLanguage { get; set; }
    }
}
