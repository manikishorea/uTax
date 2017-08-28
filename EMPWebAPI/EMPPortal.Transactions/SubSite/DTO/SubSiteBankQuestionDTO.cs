using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;
using EMPPortal.Transactions.DropDowns.DTO;

namespace EMPPortal.Transactions.SubSite.DTO
{
    public class SubSiteBankQuestionDTO
    {
        public System.Guid Id { get; set; }
        public System.Guid refid { get; set; }
        public System.Guid BankId { get; set; }
        public System.Guid QuestionId { get; set; }
        public System.Guid SubSiteConfigurationId { get; set; }
    }
}