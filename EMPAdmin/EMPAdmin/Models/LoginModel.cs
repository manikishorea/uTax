using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMPAdmin.Models
{
    public class LoginModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public TokenModel Token { get; set; }
    }
}