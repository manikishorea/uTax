using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EMP.Core.DTO;

namespace EMPAdmin.Transactions.Entity.DTO
{
    public class EntityDetailDTO : CoreModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ParentName { get; set; }

    }
}
