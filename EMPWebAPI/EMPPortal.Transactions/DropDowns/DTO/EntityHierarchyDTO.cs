using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.DropDowns.DTO
{
    public class EntityHierarchyDTO
    {
        public long Id { get; set; }
        public Nullable<System.Guid> RelationId { get; set; }
        public Nullable<int> Customer_Level { get; set; }
        public Nullable<System.Guid> CustomerId { get; set; }
        public Nullable<int> EntityId { get; set; }
        public string Status { get; set; }
        public Nullable<int> FeeSourceEntityId { get; set; }
    }
}
