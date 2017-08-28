using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;

namespace EMPPortal.Transactions.MainOffice.DTO
{
    public class MainOfficeDTO : CoreModel
    {
        public string Id { get; set; }
        public string refId { get; set; }
        public bool IsSiteTransmitTaxReturns { get; set; }
        public bool IsSiteOfferBankProducts { get; set; }
        public int TaxProfessionals { get; set; }
        public bool IsSoftwarebeInstalledNetwork { get; set; }
        public int ComputerswillruninSoftware { get; set; }
        public int PreferredSupportLanguage { get; set; }
        public bool IsBusinessSoftware { get; set; }
        public bool IsSharingEFIN { get; set; }
    }

}
