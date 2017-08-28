using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.Customer.DTO;

namespace EMPAdmin.Transactions.Customer
{
   public interface ICustomerService
    {
       IQueryable<CustomerDTO> GetAllCustomer();
       Task<CustomerDTO> GetCustomer(Guid Id);
       Task<CustomerDetailDTO> GetCustomerDetail(Guid Id);
    }
}
