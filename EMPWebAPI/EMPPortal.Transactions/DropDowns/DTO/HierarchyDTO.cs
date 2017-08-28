using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.DropDowns.DTO
{
    public class HierarchyDTO
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public int EFIN { get; set; }
        public Nullable<Guid> ParentId { get; set; }
        public int ActiveStatus { get; set; }
    }
}
