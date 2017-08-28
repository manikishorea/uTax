using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using EMPPortal.Core.DTO;
using EMP.Core.DTO;

namespace EMPPortal.Transactions.Account.Model
{
    public class ForgotPasswordModel: CoreModel
    {
        public System.Guid Id { get; set; }
        public Nullable<int> EFIN { get; set; }
        public string MasterIdentifier { get; set; }
        public string CrossLinkUserId { get; set; }
        public string CrossLinkPassword { get; set; }
        public string OfficePortalUrl { get; set; }
        public string TaxOfficeUsername { get; set; }
        public string TaxOfficePassword { get; set; }
        public string EMPPassword { get; set; }
        public Nullable<System.Guid> CustomerOfficeId { get; set; }
        //public string StatusCode { get; internal set; }

        public List<SecurityQuestionModel> QuestionsLst { get; set; }
    }

    public class ForgotAccountModel
    {
        public System.Guid Id { get; set; }
        public int Status { get; internal set; }
        public string StatusCode { get; internal set; }
        public List<SecurityQuestionModel> SecurityQuestions { get; set; }
    }

    
}
