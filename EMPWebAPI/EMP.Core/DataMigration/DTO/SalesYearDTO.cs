using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Core.DataMigration.DTO
{
    public class SalesYearDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public Nullable<int> SalesYear { get; set; }
        public Nullable<System.DateTime> ApplicableFromDate { get; set; }
        public Nullable<System.DateTime> ApplicableToDate { get; set; }
        public string Description { get; set; }
        public Nullable<bool> DateType { get; set; }
    }
}
