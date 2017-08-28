using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.EnrollmentBankConfig.DTO
{
    public class EnrollmentBankEFINOwnerRADTO : CoreModel
    {
        public int Id { get; set; }
        public System.Guid BankEnrollmentRAId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string DateofBirth { get; set; }
        public Nullable<int> SSN { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateId { get; set; }
        public string ZipCode { get; set; }
        public string IDNumber { get; set; }
        public string IDState { get; set; }
        public Nullable<decimal> PercentageOwned { get; set; }
        public Nullable<System.Guid> UpdateById { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
