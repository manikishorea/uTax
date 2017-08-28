using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EMP.Core.DTO;
using EMPAdmin.Transactions.Entity.DTO;

namespace EMPAdmin.Transactions.AffiliateProgram.DTO
{
    public class AffiliateEntityMapDTO
    {
        public System.Guid Id { get; set; }
        public System.Guid AffiliateProgramId { get; set; }
        public int? EntityId { get; set; }
    }
}
