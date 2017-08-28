using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;

namespace EMPPortal.Transactions.SubSiteFees.DTO
{
    public class SubSiteFeeDTO : CoreModel
    {
        public string Id { get; set; }
        public string refId { get; set; }
        public bool IsAddOnFeeCharge { get; set; }
        public bool IsSameforAll { get; set; }
        public bool IsSubSiteAddonFee { get; set; }
        public int ServiceorTransmission { get; set; }
        public List<SubSiteBankFeesDTO> SubSiteBankFees { get; set; }
    }

    public class SubSiteFeesDTO : CoreModel
    {
        public string refId { get; set; }
        public List<SubSiteFeeDTO> SubsiteFees { get; set; }
    }

    public class SubSiteBankFeesDTO: CoreModel
    {
        public string Id { get; set; }
        public string refId { get; set; }
        public string BankMaster_ID { get; set; }
        public string SubSiteFeeConfig_ID { get; set; }
        public decimal BankMaxFees { get; set; }
        public int ServiceorTransmission { get; set; }
        public Nullable<decimal> BankMaxFees_MSO { get; set; }
        public Nullable<Guid> QuestionID { get; set; }
    }

    public class TransmitterFeeDTO : CoreModel
    {
        public string Id { get; set; }
        public string refId { get; set; }
        public string FeeMaster_ID { get; set; }
        public decimal Amount { get; set; }      
    }

    public class BankDateCrossDTO 
    {
        public Guid BankId { get; set; }
        public Guid SalesYearID { get; set; }
        public bool Active { get; set; }        
    }
}
