using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Core.Token.DTO
{
    public class TokenDTO
    {
        public int Id { get; set; }
        public System.Guid UserId { get; set; }
        public string AuthToken { get; set; }
        public System.DateTime IssuedOn { get; set; }
        public System.DateTime ExpiredOn { get; set; }
        public string StatusCode { get; set; }
        public string IPAddress { get; set; }
    }
}
