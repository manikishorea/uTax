//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EMPEntityFramework.Edmx
{
    using System;
    using System.Collections.Generic;
    
    public partial class AuditLog_EFIN
    {
        public long Id { get; set; }
        public Nullable<int> NewEFIN { get; set; }
        public Nullable<int> OldEFIN { get; set; }
        public string ActionType { get; set; }
        public Nullable<System.Guid> CustomerId { get; set; }
        public Nullable<System.Guid> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
