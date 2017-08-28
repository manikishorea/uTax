using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Core.DTO
{
    public class CoreModel
    {
        //   public Nullable<System.Guid> CreatedBy { get; set; }
        //   public Nullable<System.DateTime> CreatedDate { get; set; }
        //   public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        //   public Nullable<System.Guid> LastUpdatedBy { get; set; }

        public Nullable<System.Guid> UserId { get; set; }
        public string StatusCode { get; set; }

        //     public string CreatedByName { get; set; }
        //     public string CreatedOn { get; set; }
        //     public string LastUpdateByName { get; set; }
        //      public string LastUpdatedOn { get; set; }
    }
}
