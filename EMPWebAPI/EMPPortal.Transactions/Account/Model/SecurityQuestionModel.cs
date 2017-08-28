using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;

namespace EMPPortal.Transactions.Account.Model
{
    public class SecurityQuestionModel: CoreModel
    {
        public System.Guid Id { get; set; }
        public string Question { get; set; }
        public string Description { get; set; }
    }
}
