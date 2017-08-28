using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Core.AuditLogs.DTO
{
    public class AuditLogDTO2
    {
        public long AuditLogID { get; set; }
        public System.Guid UserID { get; set; }
        public string TableName { get; set; }
        public long PrimaryKey { get; set; }
        public string AuditStatus { get; set; }
        public System.DateTime DateStamp { get; set; }
        public string FieldName { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
    }
}
