using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EMP.Core.DTO;
using EMPAdmin.Transactions.Entity.DTO;

namespace EMPAdmin.Transactions.Bank.DTO
{
    public class BankMasterDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public string BankName { get; set; }
        public Nullable<decimal> BankServiceFees { get; set; }
        public Nullable<decimal> MaxFeeLimitDeskTop { get; set; }
        public Nullable<decimal> MaxTranFeeDeskTop { get; set; }
        public Nullable<decimal> MaxFeeLimitMSO { get; set; }
        public Nullable<decimal> MaxTranFeeMSO { get; set; }
        public Nullable<System.DateTime> ActivatedDate { get; set; }
        public string BankProductDocument { get; set; }
        public Nullable<System.DateTime> DeActivatedDate { get; set; }
        public string Description { get; set; }
        public int BankSubQuestionsCount { get; set; }
        public string BankCode { get; set; }

        public List<BankSubQuestionDTO> BankSubQuestions { get; set; }
        public List<EntityDTO> Entities { get; set; }
        // public List<EntityDTO> EntityListCheckedItems { get; set; }
    }
}
