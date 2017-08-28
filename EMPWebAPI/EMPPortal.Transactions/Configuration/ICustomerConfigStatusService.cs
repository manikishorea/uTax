using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.Configuration.DTO;

namespace EMPPortal.Transactions.Configuration
{
    interface ICustomerConfigStatusService
    {
        IQueryable<CustomerConfigStatusDTO> GetById(Guid userid);
        bool CustomerConfigStatusSave(CustomerConfigStatusDTO dto);
        bool SaveConfigurationSatus(Guid CustomerId, Guid UserId, Guid SiteMapID,string resettype,string ActiveLinkSiteMapID,Guid BankId, string status);
    }
}
