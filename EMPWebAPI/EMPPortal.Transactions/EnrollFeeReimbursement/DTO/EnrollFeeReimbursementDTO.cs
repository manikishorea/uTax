using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;
namespace EMPPortal.Transactions.EnrollFeeReimbursement.DTO
{
    public class EnrollFeeReimbursementDTO : CoreModel
    {
        public string ID { get; set; }
        public string refId { get; set; }
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public int AccountType { get; set; }
        public string RTN { get; set; }
        public string BankAccountNo { get; set; }
        public bool IsAuthorize { get; set; }
        public Guid BankId { get; set; }
    }
}
