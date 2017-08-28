using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;

namespace EMPAdmin.Transactions.Tooltip.DTO
{
    public class TooltipDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public string Field { get; set; }
        public string ToolTipText { get; set; }
        public string Description { get; set; }
        public string SitemapTitle { get; set; }
        public System.Guid SitemapId { get; set; }
        public Nullable<bool> IsUIVisible { get; set; }
    }
}
