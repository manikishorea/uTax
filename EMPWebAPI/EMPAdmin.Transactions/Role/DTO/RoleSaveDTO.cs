using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EMP.Core.DTO;
using EMPEntityFramework.Edmx;

namespace EMPAdmin.Transactions.Role.DTO
{
    public class RoleSaveDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public bool IsVisible { get; set; }
    }
}
