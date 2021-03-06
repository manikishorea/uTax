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
    
    public partial class CustomerPaymentOption
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CustomerPaymentOption()
        {
            this.CustomerPaymentViaCreditCards = new HashSet<CustomerPaymentViaCreditCard>();
        }
    
        public System.Guid Id { get; set; }
        public System.Guid CustomerId { get; set; }
        public Nullable<System.Guid> BankId { get; set; }
        public Nullable<int> PaymentType { get; set; }
        public Nullable<int> IsSameasBankAccount { get; set; }
        public Nullable<int> SiteType { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.Guid> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    
        public virtual emp_CustomerInformation emp_CustomerInformation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerPaymentViaCreditCard> CustomerPaymentViaCreditCards { get; set; }
    }
}
