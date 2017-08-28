using EMPPortal.Transactions.CrosslinkService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;
namespace EMPPortal.Transactions.EnrollmentBankSelectionInfo.DTO
{
    public class EnrollmentBankSelectionDTO : CoreModel
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int FeeCategoryID { get; set; }

        public string Id { get; set; }
        public System.Guid CustomerId { get; set; }
        public System.Guid BankId { get; set; }
        public System.Guid QuestionId { get; set; }
        public bool IsServiceBureauFee { get; set; }
        public decimal ServiceBureauBankAmount { get; set; }
        public bool IsTransmissionFee { get; set; }
        public decimal TransmissionBankAmount { get; set; }
        //  public string StatusCode { get; set; }
        public int TPGOptions { get; set; }
        public bool IsDVServiceBureauFee { get; set; }
        public bool IsDVTransmissionFee { get; set; }

        public string AmountStatus { get; set; }
        public int FeeFor { get; set; }

        public List<Guid> UnLockedBanks { get; set; }
        public List<BankFee> BankFees { get; set; }

        public bool IsUnlocked { get; set; }

        public int IsAvailable { get; set; }
    }

    public class BankFee
    {
        public string BankName { get; set; }
        public string BankId { get; set; }
        public string BankCode { get; set; }
        public decimal SvbAmount { get; set; }
        public decimal TransAmount { get; set; }
    }

    public class TPGBankEnrollment : CoreModel
    {
        public string CompanyName { get; set; }
        public string ManagerFirstName { get; set; }
        public string ManagerLastName { get; set; }
        public string OfficeAddress { get; set; }
        public string OfficeCity { get; set; }
        public string OfficeZip { get; set; }
        public string OfficeTelephone { get; set; }
        public string OfficeFax { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingZip { get; set; }
        public string ManagerEmail { get; set; }
        public string OwnerEIN { get; set; }
        public string OwnerSSn { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerAddress { get; set; }
        public string OwnerCity { get; set; }
        public string OwnerZip { get; set; }
        public string OwnerTelephone { get; set; }
        public string OwnerDOB { get; set; }
        public string OwnerEmail { get; set; }
        public string LastYearVolume { get; set; }
        public string LastYearEFIN { get; set; }
        public string BankProductFund { get; set; }
        public string OfficeRTN { get; set; }
        public string OfficeDAN { get; set; }
        public string AccountType { get; set; }
        public string LastYearBank { get; set; }
        public string OwnerState { get; set; }
        public string ShippingState { get; set; }
        public string OfficeState { get; set; }
      //  public Guid UserId { get; set; }
        public Guid CustomerId { get; set; }
        public string EfinOwnerTitle { get; set; }
        public string EfinOwnerMobile { get; set; }
        public string EfinIDNumber { get; set; }
        public string EfinIdState { get; set; }
        public string Addonfee { get; set; }
        public string ServiceBureaufee { get; set; }
        public string CheckPrint { get; internal set; }
        public bool AgreeBank { get; set; }
        public string SbfeeAll { get; set; }
        public string DocPrepFee { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public int EntryLevel { get; set; }

        public string AddonfeeTitle { get; set; }
        public string ServiceBureaufeeTitle { get; set; }
        public string BankStatus { get; set; }
        public AppObject LatestAppRawXml { get; set; }
        public EfinObject EfinRawXml { get; set; }
        public SBTPGAppObject TPGRawXml { get; set; }

        public Guid BankId { get; set; }
    }

    public class RABankEnrollment : CoreModel
    {
        public string OwnerEmail { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerSSN { get; set; }
        public string OwnerDOB { get; set; }
        public string OwnerCellPhone { get; set; }
        public string OwnerHomePhone { get; set; }
        public string OwnerAddress { get; set; }
        public string OwnerCity { get; set; }
        public string OwnerState { get; set; }
        public string OwnerZipCode { get; set; }
        public string OwnerStateIssuedIdNumber { get; set; }
        public string OwnerIssuingState { get; set; }
        public string EROOfficeName { get; set; }
        public string EROOfficeAddress { get; set; }
        public string EROOfficeCity { get; set; }
        public string EROOfficeState { get; set; }
        public string EROOfficeZipCoce { get; set; }
        public string EROOfficePhone { get; set; }
        public string EROMaillingAddress { get; set; }
        public string EROMailingCity { get; set; }
        public string EROMailingState { get; set; }
        public string EROMailingZipcode { get; set; }
        public string EROShippingAddress { get; set; }
        public string EROShippingCity { get; set; }
        public string EROShippingState { get; set; }
        public string EROShippingZip { get; set; }
        public string IRSAddress { get; set; }
        public string IRSCity { get; set; }
        public string IRSState { get; set; }
        public string IRSZipcode { get; set; }
        public int PreviousYearVolume { get; set; }
        public int ExpectedCurrentYearVolume { get; set; }
        public string PreviousBankName { get; set; }
        public string CorporationType { get; set; }
        public string CollectionofBusinessOwners { get; set; }
        public string CollectionOfOtherOwners { get; set; }
        public int NoofYearsExperience { get; set; }
        public string HasAssociatedWithVictims { get; set; }
        public string BusinessFederalIDNumber { get; set; }
        public string BusinessEIN { get; set; }
        public string EFINOwnersSite { get; set; }
        public string IsLastYearClient { get; set; }
        public string BankRoutingNumber { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountType { get; set; }
        //public Guid UserId { get; set; }
        public Guid CustomerId { get; set; }

        public string OwnerTitle { get; set; }
        public string SbFeeall { get; set; }
        public string TransmissionAddon { get; set; }
        public string SbFee { get; set; }
        public string ElectronicFee { get; set; }
        public bool AgreeTandC { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string MainContactFirstName { get; set; }
        public string MainContactLastName { get; set; }
        public string MainContactPhone { get; set; }
        public bool TextMessages { get; set; }
        public bool LegalIssues { get; set; }
        public string StateOfIncorporation { get; set; }
        public int EntryLevel { get; set; }
        public List<EnrollmentBankEFINOwnerRADTO> RAEFINOwnerInfo { get; set; }

        public string AddonfeeTitle { get; set; }
        public string ServiceBureaufeeTitle { get; set; }
        public string BankStatus { get; set; }
        public AppObject LatestAppRawXml { get; set; }
        public EfinObject EfinRawXml { get; set; }
        public RefundAdvantageAppObject RARawXml { get; set; }

        public Guid BankId { get; set; }

    }

    public class RBBankEnrollment : CoreModel
    {
        public string OfficeName { get; set; }
        public string OfficePhysicalAddress { get; set; }
        public string OfficePhysicalCity { get; set; }
        public string OfficePhysicalState { get; set; }
        public string OfficePhysicalZip { get; set; }
        public string OfficeContactFirstName { get; set; }
        public string OfficeContactLastName { get; set; }
        public string OfficeContactSSN { get; set; }
        public string OfficePhoneNumber { get; set; }
        public string CellPhoneNumber { get; set; }
        public string FAXNumber { get; set; }
        public string EmailAddress { get; set; }
        public string OfficeManagerFirstName { get; set; }
        public string OfficeManageLastName { get; set; }
        public string OfficeManagerSSN { get; set; }
        public string OfficeManagerPhone { get; set; }
        public string OfficeManagerCellNo { get; set; }
        public string OfficeManagerEmail { get; set; }

        public string OfficeManagerDOB { get; set; }
        public string OfficeContactDOB { get; set; }
        public string AltOfficeContact1FirstName { get; set; }
        public string AltOfficeContact1LastName { get; set; }
        public string AltOfficeContact1Email { get; set; }
        public string AltOfficeContact1SSn { get; set; }
        public string AltOfficeContact2FirstName { get; set; }
        public string AltOfficeContact2LastName { get; set; }
        public string AltOfficeContact2Email { get; set; }
        public string AltOfficeContact2SSn { get; set; }
        public string AltOfficePhysicalAddress { get; set; }
        public string AltOfficePhysicalAddress2 { get; set; }
        public string AltOfficePhysicalCity { get; set; }
        public string AltOfficePhysicalState { get; set; }
        public string AltOfficePhysicalZipcode { get; set; }
        public string MailingAddress { get; set; }
        public string MailingCity { get; set; }
        public string MailingState { get; set; }
        public string MailingZip { get; set; }
        public string FulfillmentShippingAddress { get; set; }
        public string FulfillmentShippingCity { get; set; }
        public string FulfillmentShippingState { get; set; }
        public string FulfillmentShippingZip { get; set; }
        public string WebsiteAddress { get; set; }
        public int YearsinBusiness { get; set; }
        public int NoofBankProductsLastYear { get; set; }
        public string PreviousBankProductFacilitator { get; set; }
        public int ActualNoofBankProductsLastYear { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerSSN { get; set; }
        public string OnwerDOB { get; set; }
        public string OwnerHomePhone { get; set; }
        public string OwnerAddress { get; set; }
        public string OwnerCity { get; set; }
        public string OwnerState { get; set; }
        public string OwnerZip { get; set; }
        public string LegarEntityStatus { get; set; }
        public string LLCMembershipRegistration { get; set; }
        public string BusinessName { get; set; }
        public string BusinessEIN { get; set; }
        public string BusinessIncorporation { get; set; }
        public string EFINOwnerFirstName { get; set; }
        public string EFINOwnerLastName { get; set; }
        public string EFINOwnerSSN { get; set; }
        public string EFINOwnerDOB { get; set; }
        public string IsMultiOffice { get; set; }
        public int ProductsOffering { get; set; }
        public string IsOfficeTransmit { get; set; }
        public string IsPTIN { get; set; }
        public string IsAsPerProcessLaw { get; set; }
        public string IsAsPerComplainceLaw { get; set; }
        public string ConsumerLending { get; set; }
        public int NoofPersoneel { get; set; }
        public string AdvertisingApproval { get; set; }
        public string EROParticipation { get; set; }
        public decimal SPAAmount { get; set; }
        public string SPADate { get; set; }
        public string RetailPricingMethod { get; set; }
        public string IsLockedStore_Documents { get; set; }
        public string IsLockedStore_Checks { get; set; }
        public string IsLocked_Office { get; set; }
        public string IsLimitAccess { get; set; }
        public string PlantoDispose { get; set; }
        public string LoginAccesstoEmployees { get; set; }
        public string AntivirusRequired { get; set; }
        public string HasFirewall { get; set; }
        public string OnlineTraining { get; set; }
        public string PasswordRequired { get; set; }
        public string EROApplicattionDate { get; set; }
        public string EROReadTAndC { get; set; }
        public string CheckingAccountName { get; set; }
        public string BankRoutingNumber { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountType { get; set; }
        public Guid CustomerId { get; set; }
        //public Guid UserId { get; set; }
        public string EFINOwnerTitle { get; set; }
        public string EFINOwnerAddress { get; set; }
        public string EFINOwnerCity { get; set; }
        public string EFINOwnerState { get; set; }
        public string EFINOwnerZip { get; set; }
        public string EFINOwnerPhone { get; set; }
        public string EFINOwnerMobile { get; set; }
        public string EFINOwnerEmail { get; set; }
        public string EFINOwnerIDNumber { get; set; }
        public string EFINOwnerIDState { get; set; }
        public string EFINOwnerEIN { get; set; }
        public string SupportOS { get; set; }
        public string BankName { get; set; }
        public string SBFeeonAll { get; set; }
        public string SBFee { get; set; }
        public string TransimissionAddon { get; set; }
        public string PrepaidCardProgram { get; set; }
        public bool TandC { get; set; }
        public int EntryLevel { get; set; }

        public string AddonfeeTitle { get; set; }
        public string ServiceBureaufeeTitle { get; set; }
        public string BankStatus { get; set; }
        public AppObject LatestAppRawXml { get; set; }
        public EfinObject EfinRawXml { get; set; }
        public RepublicAppObject RBRawXml { get; set; }

        public Guid BankId { get; set; }
    }

    public class BankFeeLimit
    {
        public decimal SvbFeeLimit { get; set; }
        public decimal TransFeeLimit { get; set; }
        public bool ShowSvb { get; set; }
        public bool ShoeTransmission { get; set; }
        public bool Status { get; set; }
    }


    public class EnrollmentBankEFINOwnerRADTO
    {
        public System.Guid Id { get; set; }
        public System.Guid BankEnrollmentRAId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string DateofBirth { get; set; }
        public string SSN { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateId { get; set; }
        public string ZipCode { get; set; }
        public string IDNumber { get; set; }
        public string IDState { get; set; }
        public Nullable<decimal> PercentageOwned { get; set; }
        public Nullable<System.Guid> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }

    public class BankEnrollmentStatusInfo
    {
        public string BankName { get; set; }
        public string SubmitedDate { get; set; }
        public string SubmissionStaus { get; set; }
        public bool status { get; set; }
        public string BankId { get; set; }
        public string ApprovedBank { get; set; }
        public string RejectedBanks { get; set; }
        public string UnlockedBanks { get; set; }
        public bool ShowUnlock { get; set; }
        public bool ShowBankselection { get; set; }
        public int SubmissionCount { get; set; }
    }

    public class UnlockEnrollment
    {
        public Guid CustomerId { get; set; }
        public Guid UserId { get; set; }
        public string Reason { get; set; }
    }

    public class XlinkResponseModel
    {
        public List<string> Messages { get; set; }
        public bool Status { get; set; }
        public bool IsXlink { get; set; }
    }

    public class SubmitAppModel
    {
        public AuthObject Auth { get; set; }
        public int BankAppId { get; set; }
        public int EFINId { get; set; }
        public string AccountId { get; set; }
        public string BankCode { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class UpdateEnrollmentAddon
    {
        public Guid CustomerId { get; set; }
        public Guid UserId { get; set; }
        public bool IsSvbFee { get; set; }
        public decimal SvbAmount { get; set; }
        public bool IsTransmissionFee { get; set; }
        public decimal TransmissionAmount { get; set; }
        public Guid BankId { get; set; }
    }

    public class UpdateEnrollmentAddonRes
    {
        public bool Status { get; set; }
        public Guid StagingId { get; set; }
    }

    public class ApprovedBanks
    {
        public string BankName { get; set; }
        public Guid BankId { get; set; }
    }

    public class CustomerBankStaus
    {
        public bool DisableEfin { get; set; }
        public bool IsUnlocked { get; set; }
    }
}
