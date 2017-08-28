using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EMP.Core.DTO;

namespace EMPAdmin.Transactions.PhoneType.DTO
{
    public class PhoneTypeDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public string PhoneType { get; set; }
        public DateTime ActivatedDate { get; set; }
        public string Description { get; set; }
    }
}
