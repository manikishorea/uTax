using EMPPortal.Transactions.SubSiteOfficeAffiProgConfig.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.SubSiteOfficeAffiProgConfig
{
    interface ISubSiteOfficeAffiProgConfigService
    {
        IQueryable<SubSiteOfficeAffiliateProgramConfigDTO> SubSiteOfficeAffiProgConfig(Guid UserId);
        Task<int> SaveSubSiteOfficeAffiProgConfig(SubSiteOfficeAffiliateProgramConfigDTO dto);
    }
}
