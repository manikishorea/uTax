using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.EnrollmentBankConfig.DTO
{
    public class EnrollmentBankTPG : CoreModel
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> CustomerId { get; set; }
        public string CompanyName { get; set; }
        public string ManagerFirstName { get; set; }
        public string ManagerLastName { get; set; }
        public string OfficeAddress { get; set; }
        public string OfficeCity { get; set; }
        public string OfficeState { get; set; }
        public string OfficeZip { get; set; }
        public string OfficeTelephone { get; set; }
        public string OfficeFAX { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string ManagerEmail { get; set; }
        public string EFINOwnerEIN { get; set; }
        public string EFINOwnerSSN { get; set; }
        public string EFINOwnerFirstName { get; set; }
        public string EFINOwnerLastName { get; set; }
        public string EFINOwnerAddress { get; set; }
        public string EFINOwnerCity { get; set; }
        public string EFINOwnerState { get; set; }
        public string EFINOwnerZip { get; set; }
        public string EFINOwnerTelephone { get; set; }
        public Nullable<System.DateTime> EFINOwnerDOB { get; set; }
        public string EFINOwnerEmail { get; set; }
        public string BankUsedLastYear { get; set; }
        public string PriorYearEFIN { get; set; }
        public string PriorYearVolume { get; set; }
        public string PriorYearFund { get; set; }
        public string OfficeRTN { get; set; }
        public string OfficeDAN { get; set; }
        public string OfficeAccountType { get; set; }
        //public Nullable<System.Guid> CreatedBy { get; set; }
        //public Nullable<System.DateTime> CreatedDate { get; set; }
        //public Nullable<System.Guid> UpdatedBy { get; set; }
        //public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
