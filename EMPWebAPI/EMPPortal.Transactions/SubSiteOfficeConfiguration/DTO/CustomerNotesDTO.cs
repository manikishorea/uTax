using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.SubSiteOfficeConfiguration.DTO
{
    public class CustomerNotesDTO : CoreModel
    {
        public string Id { get; set; }
        public string RefId { get; set; }
        public string Note { get; set; }
        public string RefName { get; set; }
        public string CreatedDate { get; set; }
    }
}
