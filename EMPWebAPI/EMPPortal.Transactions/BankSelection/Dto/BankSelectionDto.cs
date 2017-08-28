using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.BankSelection.Dto
{
    public class BankSelectionDto
    {
    }

    public class CustomerBanksResponse
    {
        public bool Status { get; set; }
        public List<CustomerBanks> Banks { get; set; }
    }

    public class CustomerBanks
    {
        public Guid BankId { get; set; }
        public string BankName { get; set; }
        public string Submission { get; set; }
        public string Acceptance { get; set; }
        public string BankStatus { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDefault { get; set; }

        public Guid EnrollId { get; set; }
        public int Default { get; set; }
    }
}
