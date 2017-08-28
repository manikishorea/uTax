namespace EMPPortal.Transactions.Account
{
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.Account.Model;


  public interface ICustomerLoginService
    {
        // IQueryable<ChangePasswordModel> GetAll();
        CustomerLoginModel IsUserExist(CustomerLoginModel _Dto);
    }   
}
