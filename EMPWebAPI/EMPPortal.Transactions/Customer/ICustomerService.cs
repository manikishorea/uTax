using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.Customer.DTO;
using EMMPortal.Transactions.Customer.DTO;

namespace EMPPortal.Transactions.Customer
{
    public interface ICustomerService
    {
        IQueryable<CustomerDTO> GetAllCustomer();
        Task<CustomerDTO> GetCustomer(Guid Id);
        Task<CustomerDetailDTO> GetCustomerDetail(Guid Id);
    }
}
