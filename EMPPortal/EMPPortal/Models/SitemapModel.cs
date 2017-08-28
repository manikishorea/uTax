using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMPPortal.Models
{
    public class SitemapModel
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public string DisplayClass { get; set; }
        public Nullable<bool> DisplayPartial { get; set; }
        public int? BaseEntityId { get; set; }
        public int? EntityId { get; set; }
        public Nullable<int> DisplayOrderBeforeAct { get; set; }
        public Nullable<int> DisplayOrderAfterAct { get; set; }
        public Nullable<int> DisplayBeforeVerify { get; set; }
        public int SitemapTypeId { get; set; }
        public string StatusCode { get; set; }
    }
}