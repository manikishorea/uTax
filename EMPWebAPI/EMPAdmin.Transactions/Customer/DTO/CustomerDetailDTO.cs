using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EMP.Core.DTO;
namespace EMPAdmin.Transactions.Customer.DTO
{
   public class CustomerDetailDTO : CoreModel
        {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public int? EntityId { get; set; }
        public string EntityName { get; set; }
    }
}
