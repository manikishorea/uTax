using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EMP.Core.DTO;

namespace EMPAdmin.Transactions.ContactPersonTitle.DTO
{
    public class ContactPersonTitleDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public string ContactPersonTitle { get; set; }
        public DateTime ActivatedDate { get; set; }
        public string Description { get; set; }
    }
}
