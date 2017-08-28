using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMPAdmin.Models
{
    public class TokenModel
    {
        public int Id { get; set; }
        public System.Guid UserId { get; set; }
        public string AuthToken { get; set; }
        public System.DateTime IssuedOn { get; set; }
        public System.DateTime ExpiredOn { get; set; }
        public string StatusCode { get; set; }
    }
}