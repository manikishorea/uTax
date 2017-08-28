using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace EMPAdmin.Transactions.SalesYear.DTO
{
    public class SalesYearEntityMapDTO
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> SalesYearId { get; set; }
        public Nullable<int> EntityId { get; set; } 
    }
}
