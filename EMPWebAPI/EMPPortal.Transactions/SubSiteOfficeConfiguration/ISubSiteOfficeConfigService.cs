using EMPPortal.Transactions.SubSiteOfficeConfiguration.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.SubSiteOfficeConfiguration
{
    interface ISubSiteOfficeConfigService
    {
        Task<SubSiteOfficeConfigDTO> GetSubSiteOfficeConfigById(Guid userid,Guid parentId);
        Guid SaveSubSiteOfficeConfigInfo(SubSiteOfficeConfigDTO dto);
        IQueryable<CustomerNotesDTO> GetCustomerNotesById(Guid userid);
        Guid SaveCustomerNoteInfo(CustomerNotesDTO dto);
        IQueryable<SubSiteBankFeeConfigDTO> GetSubSiteBankFeeById(Guid userid);
        int SaveSubSiteBankFeeConfigInfo(List<SubSiteBankFeeConfigDTO> dto);
        bool GetValidUserID(string OwnID, string ParentID,Guid CustomerId);
    }
}
