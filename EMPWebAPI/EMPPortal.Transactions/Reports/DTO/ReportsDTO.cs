using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EMPPortal.Transactions.Reports.DTO
{
    public class FeeSetupReportDTO
    {
        public string MasterID { get; set; }
        public string UserID { get; set; }
        public string ParentUserID { get; set; }
        public string CompanyName { get; set; }
        public string Efin { get; set; }
        public string uTaxFee { get; set; }
        public string SBFee { get; set; }
        public string SBAddOn { get; set; }
        public string AddonFeeSB { get; set; }
        public string AddonFeeERO { get; set; }
        public string Bank { get; set; }
        public string EnrollmentStatus { get; set; }
        public string AccountOwner { get; set; }
    }

    public class CustomerDTO
    {
        public Guid ID { get; set; }
        public string CustomerName { get; set; }
    }

    public class LastLoginDTO
    {
        public string MasterID { get; set; }
        public string UserID { get; set; }
        public string ParentUserID { get; set; }
        public string CompanyName { get; set; }
        public string Efin { get; set; }
        public string LastLogin { get; set; }
        public string DateTimeStamp { get; set; }
        public string IpAddress { get; set; }
    }

    public class EnrollmentDTO
    {
        public string MasterID { get; set; }
        public string UserID { get; set; }
        public string ParentUserID { get; set; }
        public string CompanyName { get; set; }
        public string Efin { get; set; }
        public string AccountStatus { get; set; }
        public string EROType { get; set; }
        public string CaseOrgin { get; set; }
        public string Product { get; set; }
        public string FuntionalArea { get; set; }
        public string Module { get; set; }
        public string Issue { get; set; }
        public string Status { get; set; }
        public string Casenumber { get; set; }
        public string MSOUser { get; set; }
        public string DateTimeOpened { get; set; }
        public string DateTimeLastModified { get; set; }
    }

    public class EnrollmentStatusReport
    {
        public string Efin { get; set; }
        public string UserId { get; set; }
        public string ParentId { get; set; }
        public string Company { get; set; }
        public string Bank { get; set; }
        public string Status { get; set; }
        public string SBFee { get; set; }
        public string AddonFee { get; set; }
        public string SubmissionDate { get; set; }
        public string ErrorMessage { get; set; }
        public string LastModifiedDate { get; set; }
        public string LastModifiedUser { get; set; }
        public string AccountOwner { get; set; }
        public string Id { get; set; }
        public string Parent { get; set; }
        public string MasterId { get; internal set; }
    }
}
