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
    
    public partial class AffiliateProgramMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AffiliateProgramMaster()
        {
            this.AffiliationProgramEntityMaps = new HashSet<AffiliationProgramEntityMap>();
        }
    
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<System.DateTime> ActivationDate { get; set; }
        public Nullable<System.DateTime> DeactivationDate { get; set; }
        public string DocumentPath { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<System.Guid> LastUpdatedBy { get; set; }
        public string StatusCode { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AffiliationProgramEntityMap> AffiliationProgramEntityMaps { get; set; }
    }
}