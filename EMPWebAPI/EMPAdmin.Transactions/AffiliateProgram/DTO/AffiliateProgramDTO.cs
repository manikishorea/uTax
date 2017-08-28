using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EMP.Core.DTO;
using EMPAdmin.Transactions.Entity.DTO;
namespace EMPAdmin.Transactions.AffiliateProgram.DTO
{
    public class AffiliateProgramDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<System.DateTime> ActivationDate { get; set; }
        public Nullable<System.DateTime> DeactivationDate { get; set; }
        public string DocumentPath { get; set; }

        public List<EntityDTO> Entities { get; set; }
    }
}
