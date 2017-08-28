using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.SubSiteOfficeConfiguration.DTO
{
    public class SubSiteBankFeeConfigDTO : CoreModel
    {
        public string Id { get; set; }
        public string RefId { get; set; }
        public string BankID { get; set; }
        public int ServiceorTransmitter { get; set; }
        public decimal AmountDSK { get; set; }
        public decimal AmountMSO { get; set; }
        public string QuestionID { get; set; }
    }
}
