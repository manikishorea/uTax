using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using EMP.Core.DTO;
using EMPAdmin.Transactions.Entity.DTO;

namespace EMPAdmin.Transactions.SalesYear
{
    public class SalesYearDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public int SalesYear { get; set; }
        public Nullable<System.DateTime> ApplicableFromDate { get; set; }
        public Nullable<System.DateTime> ApplicableToDate { get; set; }
        public string Description { get; set; }
        public Nullable<bool> DateType { get; set; }
        public List<EntityDTO> Entities { get; set; }        
        public string BankName { get; set; }
        public List<BankInfoDTO> BankInfoList { get; set; }
    }

    public class BankInfoDTO
    {
        public System.Guid BankID { get; set; }
        public DateTime CutOfDate { get; set; }
    }
}
