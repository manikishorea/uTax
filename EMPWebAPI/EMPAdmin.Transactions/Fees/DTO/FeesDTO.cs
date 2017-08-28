using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EMP.Core.DTO;
using EMPAdmin.Transactions.Entity.DTO;

namespace EMPAdmin.Transactions.Fees.DTO
{
    public class FeesDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string FeeType { get; set; }
        public Nullable<short> FeeTypeId { get; set; }
        public string FeeNature { get; set; }
        public Nullable<short> FeeNatureId { get; set; }
        public Nullable<System.DateTime> ActivatedDate { get; set; }
        public Nullable<System.DateTime> DeActivatedDate { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string NoteForUser { get; set; }
        public string Note { get; set; }
        public int FeeCategoryID { get; set; }
        public bool IsIncludedMaxAmtCalculation { get; set; }
        public string SalesforceFeesFieldID { get; set; }

        public string FeesFor { get; set; }
        public string FeesForName { get; set; }

        public List<EntityDTO> Entities { get; set; }
    }
}
