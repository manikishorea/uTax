using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EMP.Core.DTO;
using EMPAdmin.Transactions.Entity.DTO;

namespace EMPAdmin.Transactions.Fees.DTO
{
    public class FeeEntityMapDTO
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> FeeId { get; set; }
        public Nullable<int> EntityId { get; set; }
    }
}
