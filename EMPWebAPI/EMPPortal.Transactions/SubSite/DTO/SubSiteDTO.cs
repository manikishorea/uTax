using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;
using EMPPortal.Transactions.DropDowns.DTO;

namespace EMPPortal.Transactions.SubSite.DTO
{
    public class SubSiteDTO : CoreModel
    {
        public string Id { get; set; }
        public string refId { get; set; }
        public Nullable<bool> IsuTaxManageingEnrolling { get; set; }
        public Nullable<bool> IsuTaxPortalEnrollment { get; set; }
        public Nullable<bool> IsuTaxManageOnboarding { get; set; }
        public Nullable<bool> IsuTaxCustomerSupport { get; set; }
        public Nullable<int> NoofSupportStaff { get; set; }
        public string NoofDays { get; set; }
        public string OpenHours { get; set; }
        public string CloseHours { get; set; }
        public string TimeZone { get; set; }
        public Nullable<bool> IsAutoEnrollAffiliateProgram { get; set; }
        public Nullable<int> SubSiteTaxReturn { get; set; }
        public bool CanSubSiteLoginToEmp { get; set; }

        public List<SubSiteAffiliateProgramDTO> Affiliates { get; set; }
        public List<SubSiteBankQuestionDTO> SubSiteBankQuestions { get; set; }
        public Nullable<bool> IsSubSiteEFINAllow { get; set; }
        public bool EnrollmentEmails { get; set; }
    }
}
