using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;

namespace EMPPortal.Transactions.CustomerInformation.DTO
{
    public class CustomerAssociatedFeesDTO : CoreModel
    {
        public string Id { get; set; }
        public string FeesName { get; set; }
        public int FeeFor { get; set; }
        public string FeeForText { get; set; }
        public decimal Amount { get; set; }
        public string FeeStatus { get; set; }
    }

}
