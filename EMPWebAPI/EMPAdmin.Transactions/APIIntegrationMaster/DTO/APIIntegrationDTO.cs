using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EMP.Core.DTO;

namespace EMPAdmin.Transactions.APIIntegrationMaster.DTO
{
    public class APIIntegrationDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
