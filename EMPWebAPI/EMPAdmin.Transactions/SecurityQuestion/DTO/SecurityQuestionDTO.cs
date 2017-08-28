using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;

namespace EMPAdmin.Transactions.SecurityQuestion.DTO
{
    public class SecurityQuestionDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public string Question { get; set; }
        public string Description { get; set; }
    }
}
