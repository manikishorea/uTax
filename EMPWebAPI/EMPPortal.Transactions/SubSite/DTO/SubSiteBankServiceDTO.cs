using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;
using EMPPortal.Transactions.DropDowns.DTO;

namespace EMPPortal.Transactions.SubSite.DTO
{
    public class SubSiteBankServiceDTO : CoreModel
    {
        public string Id { get; set; }
        public string refId { get; set; }
        public Nullable<bool> IsuTaxManageingEnrolling { get; set; }
        public Nullable<bool> IsuTaxPortalEnrollment { get; set; }
        public string formtype { get; set; }
        public List<SubSiteBankQuestionDTO> SubSiteBankQuestions { get; set; }

        public bool CanSubSiteLoginToEmp { get; set; }
        public Nullable<bool> IsSubSiteEFINAllow { get; set; }
        public bool EnrollmentEmails { get; set; }
    }
}
