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
    
    public partial class CustomerPaymentViaACH
    {
        public System.Guid Id { get; set; }
        public System.Guid PaymentOptionId { get; set; }
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public string RTN { get; set; }
        public string AccountNumber { get; set; }
        public Nullable<int> AccountType { get; set; }
        public Nullable<System.Guid> CreatdedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.Guid> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
