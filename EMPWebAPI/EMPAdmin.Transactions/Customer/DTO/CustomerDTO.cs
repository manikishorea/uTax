using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPAdmin.Transactions.Customer.DTO
{
   public class CustomerDTO
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public int? EntityId { get; set; }
        public string EntityName{ get; set; }
        public string StatusCode { get; set; }
    }
}
