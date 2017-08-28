using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;

namespace EMPPortal.Transactions.Configuration.DTO
{
   public class CustomerConfigStatusDTO : CoreModel
    {
        public string Id { get; set; }
        public string SitemapId { get; set; }
        public string CustomerId { get; set; }
        public string Status { get; set; }
        public string bankid { get; set; }
    }
}
