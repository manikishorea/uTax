using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.SubSite.DTO;
namespace EMPPortal.Transactions.SubSite
{
    interface ISubSiteConfigService
    {
        Task<SubSiteDTO> GetById(Guid userid);
        Guid BankServiceSave(SubSiteBankServiceDTO dto);
        Guid OnBoardingServiceSave(SubSiteOnBoardingDTO dto);
        Guid SupportServiceSave(SubSiteSupportDTO dto);
    }
}
