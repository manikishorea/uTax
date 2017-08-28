using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.Account.Model
{
    public class SecurityQuestionAnswerModel : CoreModel
    {
        public System.Guid Id { get; set; }
        public string Answer { get; set; }
        public System.Guid QuestionId { get; set; }
        public string Question { get; set; }
        public int DisplayOrder { get; set; }
        //public  List<SecurityQuestionModel> QuestionsLst { get; set; }
        //public string StatusCode { get; internal set; }
    }

    public class userSecurityAnswerModel : CoreModel
    {
        public System.Guid Id { get; set; }
        public string UserName { get; set; }
        public List<SecurityQuestionAnswerModel> QuestionsLst { get; set; }
        public string Status { get; set; }
    }
}
