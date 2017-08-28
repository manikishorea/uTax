using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Core.Utilities
{
    public static class EMPConstants
    {
        public static string Active = StatusCode.ACT.ToString();
        public static string NewCustomer = StatusCode.NEW.ToString();
        public static string InActive = StatusCode.INA.ToString();
        public static string Deleted = StatusCode.DEL.ToString();
        public static string Created = StatusCode.CRT.ToString();
        public static string Archive = StatusCode.ARC.ToString();
        public static string Pending = StatusCode.PEA.ToString();
        public static string Done = StatusCode.done.ToString();
        public static string None = StatusCode.none.ToString();
        public static string Ready = StatusCode.RDY.ToString();
        public static string InProgress = StatusCode.INP.ToString();
        public static string Approved = "APR";
        public static string Rejected = "REJ";
        public static string Submitted = "SUB";
        public static string Denied = "DEN";
        public static string EnrPending = "PEN";
        public static string Cancelled = "CAN";
        public static string SubmittedService = "Submitted the Application";
        public static string ApprovedService = "Approved";
        public static string RejectedService = "Rejected";
        public static string PendingServcce = "Pending";
        public static string DeniedServcce = "Denied";
        public static string CancelledService = "Cancelled";
        public static string UnLocked = "UNL";
        public static string BankApproved = "Bank App Approved";
        public static string BankRejected = "Bank App Rejected";
        public static string BankPending = "Bank App Pending";

        public static string xlinkMaster = "RIVEDG";
        public static string xlinkPassword = "xlink";

        public static string Fixedamount = FeeType.Fixed.ToString();
        public static string Useramount = FeeType.User.ToString();
        public static string SalesForce = FeeType.SalesForce.ToString();

        public static string Mandatory = FeeNature.Mandatory.ToString();
        public static string Optional = FeeNature.Optional.ToString();

        public static string Others = FeesFor.Others.ToString();
        public static string SVBFees = FeesFor.SVBFees.ToString();
        public static string TransmissionFees = FeesFor.TransmissionFees.ToString();

        public static string SiteAdmin = SitemapType.Admin.ToString();
        public static string SiteEMPConfiguration = SitemapType.EMPConfiguration.ToString();
        public static string SubSiteConfiguration = SitemapType.SubSiteConfiguration.ToString();

        public static int eFileFeeCategory = 2;

        public static string NewUserMailSubject = "New User Created";

        public static string TPGBank = "S";
        public static string RABank = "V";
        public static string RBBank = "R";

        public static string PasswordHashKey = "11EMP@PA$SWO&D03";
        public static string EnterprisePackage = "uTax Enterprise";

        public static string SupportutaxEmail = "support@utaxsoftware.com";
        public static string accountutaxEmail = "accounting@utaxsoftware.com";


        public static int DefaultBank = 1;


        public enum StatusCode
        {
            ACT = 1,// Active status
            INA = 2, // In Active status
            DEL = 3,
            CRT = 4,
            ARC = 5,
            PEA = 6,
            done = 7,
            NEW = 8,
            RDY = 9,
            INP = 10,
            none = 11,
            PEN = 12,

        }

        public enum FeeType
        {
            Fixed = 1,// Active status
            User = 2, // In Active status
            SalesForce = 3, // Salesforce
        }

        public enum FeeNature
        {
            Mandatory = 1,// Active status
            Optional = 2, // In Active status
        }

        public enum SitemapType
        {
            Admin = 1,// Active status
            EMPConfiguration = 2, // In Active status
            SubSiteConfiguration = 3,
            EnrollmentConfiguration = 4,
            uTaxConfiguration = 5,
            PaymentConfiguration = 6,
            ReportsConfiguration = 7,
            MainSiteHome = 8,
            SubSiteHome = 9,
            ProfileConfiguration = 10,
            ArchiveConfiguration = 11,
            BankEnrollmentConfiguration = 12,
            ArchiveSubConfiguration = 13,
            /*
                1 for Admin
                2 for EMP Main
                3 for EMP Enrollment
             * */
        }


        public static string statuscode = StatusType.statuscode.ToString();
        public static string enrollmentstatus = StatusType.enrollmentstatus.ToString();
        public static string onboarding = StatusType.onboarding.ToString();

        public static string SingleOffice = "Single Office";
        public static string SVBSub = "SVB Sub-Office";
        public static string MOSub = "Multi-Office Sub-Office";

        public enum StatusType
        {
            statuscode = 1,// Active status
            enrollmentstatus = 2, // In Active status
            onboarding = 3,
            efinstatusmain = 4,
            efinstatussub = 5,
        }

        public enum FeesFor
        {
            Others = 1,// Active status
            SVBFees = 2, // In Active status
            TransmissionFees = 3,
        }

        public enum Entity
        {
            uTax = 1,
            SO = 2,
            SOME = 3,
            SOME_SS = 4,
            MO = 5,
            MO_SO = 6,
            MO_AE = 7,
            MO_AE_SS = 8,
            SVB = 9,
            SVB_SO = 10,
            SVB_MO = 11,
            SVB_MO_SO = 12,
            SVB_MO_AE = 13,
            SVB_MO_AE_SS = 14,
            SVB_AE = 15,
            SVB_AE_SS = 16
        }

        public enum BaseEntities
        {
            uTax = 0,
            SVB = 1,
            MO = 1,
            SO = 1,
            SO_ME = 1,
            AE = 2,
            AE_SS = 3,
        }

        public enum EmailTypes
        {
            NewUser = 1,
            SubSiteCreation = 2,
            HoldUnhold = 3,
            BusinessSoftware = 4,
            EnrollmentCancel =5,
            HoldUser =6,
            NewAdminUser = 7
        }

        public enum EFINStatus_ForMain
        {
            Valid_EFIN = 16,// Active status
            Applied = 17, // In Active status
            Not_Required = 18,
        }

        public enum EFINStatus_ForSub
        {
            Valid_EFIN = 19,// Active status
            Applied = 20, // In Active status
            Sharing = 21
        }

        public enum OnboardStatus
        {
            Not_Started = 1,
            Completed = 2,
            Started_But_Incomplete = 3,
            Partner_Declined = 4,
            Not_Applicable = 5
        }
    }
}
