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
    
    public partial class TokenMaster
    {
        public int Id { get; set; }
        public System.Guid UserId { get; set; }
        public string AuthToken { get; set; }
        public System.DateTime IssuedOn { get; set; }
        public System.DateTime ExpiredOn { get; set; }
        public string StatusCode { get; set; }
        public string IPAddress { get; set; }
    }
}
