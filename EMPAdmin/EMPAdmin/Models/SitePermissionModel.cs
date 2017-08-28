using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMPAdmin.Models
{
    public class SitePermissionModel
    {
        public string Id { get; set; }
        public string PermissionId { get; set; }
        public string PermissionName { get; set; }
        public string SiteName { get; set; }
        public string SiteMapId { get; set; }
    }
}