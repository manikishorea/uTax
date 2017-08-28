using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.CustomerLoginInformation;

namespace EMPPortal.Transactions.CustomerInformation
{
    public class CustomerInformationDisplayDTO : CoreModel
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string CompanyName { get; set; }
        public string EntityId { get; set; }
        public int DisplayId { get; set; }
        public bool IsAdditionalEFINAllowed { get; set; }
        public decimal TotalServiceFee { get; set; }
        public bool IsActivated { get; set; }
        public bool IsMSOUser { get; set; }
        public int IsActivationCompleted { get; set; }
    }
}
