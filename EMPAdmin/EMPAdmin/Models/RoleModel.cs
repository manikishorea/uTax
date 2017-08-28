using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMPAdmin.Models
{
    public class RoleModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; }
    }
}