using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMP.Core.DataMigration.DTO
{
    class DataMigrationDTO
    {

    }

    public class ArchiveMainSiteDataModel
    {
   
        public MainOfficeConfigurationDTO MainOfficeConfigDTO { get; set; }
        public SubSiteConfigurationDTO SubSiteConfigDTO { get; set; }
        public List<SubSiteFeeConfigDTO> SubSiteFeeDTOs { get; set; }
        public FeeReimbursementConfigDTO FeeReimburseConfigDTO { get; set; }
    }

    public class ArchiveEnrollmentDataModel
    {
        public CustomerInformationDTO CustomerInformationDTO { get; set; }
        public CustomerLoginInformationDTO CustomerLoginInformationDTO { get; set; }
        public CustomerInformationDTO ParentInformationDTO { get; set; }

        public EnrollmentOfficeConfigurationDTO EnrollmentOfficeConfigDTO { get; set; }
        public List<EnrollmentAffiliateConfigurationDTO> EnrollmentAffiliateConfigDTOs { get; set; }

        public List<EnrollmentBankSelectionDTO> EnrollmentBankSelectDTOs { get; set; }
        public EnrollmentFeeReimbursementConfigDTO EnrollmentFeeReimbursementDTO { get; set; }

        public TPGBankEnrollmentDTO TPGBankEnrollmentDTO { get; set; }
        public RABankEnrollmentDTO RABankEnrollmentDTO { get; set; }
        public RBBankEnrollmentDTO RBBankEnrollmentDTO { get; set; }

        public CustomerPaymentInfoDTO eFilePaymentInfoDTO { get; set; }
        public CustomerPaymentInfoDTO OutstandingPaymentInfoDTO { get; set; }

        public CustomerBanksResponseDTO CustomerBanksResponseDTO { get; set; }
    }

    public class ArchiveSubSiteDataModel
    {
        public SubSiteOfficeConfigDTO SubSiteOfficeDTO { get; set; }
        public List<SubSiteBankFeesConfigDTO> SubSiteBankFeesDTOs { get; set; }
        public List<SubSiteFeeConfigDTO> SubSiteFeeDTOs { get; set; }

    }

    public class ArchiveCustomerModel
    {
        public System.Guid Id { get; set; }
        public System.Guid ParentId { get; set; }
        public string str_ParentId { get; set; }
        public string CompanyName { get; set; }
        public string AccountStatus { get; set; }
        public Nullable<bool> Feeder { get; set; }
        public string BusinessOwnerFirstName { get; set; }
        public string OfficePhone { get; set; }
        public string AlternatePhone { get; set; }
        public string Primaryemail { get; set; }
        public string SupportNotificationemail { get; set; }
        public string EROType { get; set; }
        public string AlternativeContact { get; set; }
        public Nullable<int> EFIN { get; set; }
        public Nullable<int> EFINStatus{ get; set; }
        public string PhysicalAddress1 { get; set; }
        public string PhysicalAddress2 { get; set; }
        public string PhysicalZipcode { get; set; }
        public string PhysicalCity { get; set; }
        public string PhysicalState { get; set; }
        public bool ShippingAddressSameAsPhysicalAddress { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingZipcode { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public Nullable<System.Guid> PhoneTypeId { get; set; }
        public Nullable<System.Guid> TitleId { get; set; }
        public Nullable<System.Guid> AlternativeType { get; set; }
        public string PhoneType { get; set; }
        public string ContactTitle { get; set; }
        public Nullable<System.Guid> SalesYearID { get; set; }
        public Nullable<int> EntityId { get; set; }
        public Nullable<int> DisplayId { get; set; }
        public string StatusCode { get; set; }
        public string EMPUserId { get; set; }
        public string LoginId { get; set; }
        public Nullable<int> LoginEFIN { get; set; }
        public string MasterIdentifier { get; set; }
        public string CrossLinkUserId { get; set; }
        public string CrossLinkPassword { get; set; }
        public string OfficePortalUrl { get; set; }
        public string TaxOfficeUsername { get; set; }
        public string TaxOfficePassword { get; set; }
        public string EMPPassword { get; set; }
        public Nullable<System.Guid> CustomerOfficeId { get; set; }

        public string ChildInfo { get; set; }
        public int ChaildCustomerInfoCount { get; set; }
        public bool IsAdditionalEFINAllowed { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public decimal TotalServiceFee { get; set; }
        public decimal TotalTransFee { get; set; }
        public string ServiceTooltip { get; set; }
        public string TransTooltip { get; set; }

        public string SalesforceParentID { get; set; }
        public string ActivationStatus { get; set; }
        public bool IsActivated { get; set; }
        public bool IsMSOUser { get; set; }
        public bool IsTaxReturn { get; set; }

        public int IsActivationCompleted { get; set; }

        public string ActiveBank { get; set; }
        public string SubmissionDate { get; set; }
        public string EnrollmentStatus { get; set; }
        public string ApprovedBank { get; set; }
        public string RejectedBanks { get; set; }
        public string UnlockedBanks { get; set; }

        public bool IsEnrolled { get; set; }
        public Guid? EnrolledBankId { get; set; }

        public Guid? SalesYearGroupId { get; set; }
        public Nullable<bool> IsVerified { get; set; }
    }

    public class MainOfficeConfigurationDTO
    {
        public System.Guid Id { get; set; }
        public System.Guid refId { get; set; }
        public bool IsSiteTransmitTaxReturns { get; set; }
        public bool IsSiteOfferBankProducts { get; set; }
        public int TaxProfessionals { get; set; }
        public bool IsSoftwarebeInstalledNetwork { get; set; }
        public int ComputerswillruninSoftware { get; set; }
        public int PreferredSupportLanguage { get; set; }
        public System.Guid CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public System.Guid LastUpdatedBy { get; set; }
        public string StatusCode { get; set; }
    }

    public class SubSiteConfigurationDTO
    {
        public System.Guid Id { get; set; }
        public System.Guid refId { get; set; }
        public Nullable<bool> IsuTaxManageingEnrolling { get; set; }
        public Nullable<bool> IsuTaxPortalEnrollment { get; set; }
        public Nullable<bool> IsuTaxManageOnboarding { get; set; }
        public Nullable<bool> IsuTaxCustomerSupport { get; set; }
        public bool CanSubSiteLoginToEmp { get; set; }
        public Nullable<int> NoofSupportStaff { get; set; }
        public string NoofDays { get; set; }
        public string OpenHours { get; set; }
        public string CloseHours { get; set; }
        public string TimeZone { get; set; }
        public Nullable<bool> IsAutoEnrollAffiliateProgram { get; set; }
        public Nullable<int> SubSiteTaxReturn { get; set; }
        public Nullable<bool> IsSubSiteEFINAllow { get; set; }
        public System.Guid CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public System.Guid LastUpdatedBy { get; set; }
        public string StatusCode { get; set; }

        public List<SubSiteBankConfigDTO> SubSitBankQuestionDTOs { get; set; }
        public List<SubSiteAffiliateProgramConfigDTO> SubSiteAffiliateProgramDTOs { get; set; }

    }

    public class SubSiteBankConfigDTO
    {
        public System.Guid ID { get; set; }
        public System.Guid emp_CustomerInformation_ID { get; set; }
        public System.Guid BankId { get; set; }
        public System.Guid SubSiteConfiguration_ID { get; set; }
        public Nullable<System.Guid> QuestionId { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<System.Guid> LastUpdatedBy { get; set; }
        public string StatusCode { get; set; }

        // public virtual SubSiteConfigurationDTO SubSiteConfiguration { get; set; }
    }

    public class SubSiteAffiliateProgramConfigDTO
    {
        public System.Guid ID { get; set; }
        public System.Guid emp_CustomerInformation_ID { get; set; }
        public System.Guid AffiliateProgramId { get; set; }
        public System.Guid SubSiteConfiguration_ID { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<System.Guid> LastUpdatedBy { get; set; }
        public string StatusCode { get; set; }
    }

    public class SubSiteFeeConfigDTO
    {
        public System.Guid Id { get; set; }
        public System.Guid refId { get; set; }
        public bool IsAddOnFeeCharge { get; set; }
        public bool IsSameforAll { get; set; }
        public bool IsSubSiteAddonFee { get; set; }
        public int ServiceorTransmission { get; set; }
        public System.Guid CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public System.Guid LastUpdatedBy { get; set; }
        public string StatusCode { get; set; }
        public List<SubSiteBankFeesConfigDTO> SubSiteBankFeesDTOs { get; set; }
    }

    public class SubSiteBankFeesConfigDTO
    {
        public System.Guid ID { get; set; }
        public System.Guid RefId { get; set; }
        public System.Guid BankID { get; set; }
        public Nullable<System.Guid> SubSiteFeeConfig_ID { get; set; }
        public decimal AmountDSK { get; set; }
        public Nullable<decimal> AmountMSO { get; set; }
        public Nullable<int> ServiceorTransmitter { get; set; }
        public Nullable<System.Guid> QuestionID { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<System.Guid> LastUpdatedBy { get; set; }
        public string StatusCode { get; set; }
    }

    public class FeeReimbursementConfigDTO
    {
        public System.Guid ID { get; set; }
        public System.Guid refId { get; set; }
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public int AccountType { get; set; }
        public string RTN { get; set; }
        public string BankAccountNo { get; set; }
        public bool IsAuthorize { get; set; }
        public System.Guid CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public System.Guid LastUpdatedBy { get; set; }
        public string StatusCode { get; set; }
    }

    // Enrollment Models
    public class EnrollmentOfficeConfigurationDTO
    {
        public System.Guid Id { get; set; }
        public System.Guid CustomerId { get; set; }
        public Nullable<bool> IsMainSiteTransmitTaxReturn { get; set; }
        public Nullable<int> NoofTaxProfessionals { get; set; }
        public Nullable<bool> IsSoftwareOnNetwork { get; set; }
        public Nullable<int> NoofComputers { get; set; }
        public Nullable<int> PreferredLanguage { get; set; }
        public string StatusCode { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public Nullable<System.Guid> LastUpdatedBy { get; set; }
    }

    public class EnrollmentAffiliateConfigurationDTO
    {
        public System.Guid Id { get; set; }
        public System.Guid CustomerId { get; set; }
        public System.Guid AffiliateProgramId { get; set; }
        public Nullable<decimal> AffiliateProgramCharge { get; set; }
        public string StatusCode { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.Guid> LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
    }

    public class EnrollmentBankSelectionDTO
    {
        public System.Guid Id { get; set; }
        public System.Guid CustomerId { get; set; }
        public System.Guid BankId { get; set; }
        public System.Guid QuestionId { get; set; }
        public Nullable<bool> IsServiceBureauFee { get; set; }
        public Nullable<decimal> ServiceBureauBankAmount { get; set; }
        public Nullable<bool> IsTransmissionFee { get; set; }
        public Nullable<decimal> TransmissionBankAmount { get; set; }
        public string StatusCode { get; set; }
        public System.Guid CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public System.Guid LastUpdatedBy { get; set; }

        //Other Fiels
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int FeeCategoryID { get; set; }
        public int TPGOptions { get; set; }
        public bool IsDVServiceBureauFee { get; set; }
        public bool IsDVTransmissionFee { get; set; }
        public string AmountStatus { get; set; }
        public int FeeFor { get; set; }
        public List<Guid> UnLockedBanks { get; set; }

        // For Bank Enrollment
        public string BankCode { get; set; }
    }

    public class EnrollmentFeeReimbursementConfigDTO
    {
        public System.Guid ID { get; set; }
        public System.Guid refId { get; set; }
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public int AccountType { get; set; }
        public string RTN { get; set; }
        public string BankAccountNo { get; set; }
        public bool IsAuthorize { get; set; }
        public System.Guid CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public System.Guid LastUpdatedBy { get; set; }
        public string StatusCode { get; set; }
    }

    public class TPGBankEnrollmentDTO
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

    }

    public class RABankEnrollmentDTO
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

    }

    public class RBBankEnrollmentDTO
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

    //SubSite Models
    public class SubSiteOfficeConfigDTO
    {
        public System.Guid Id { get; set; }
        public System.Guid RefId { get; set; }
        public bool EFINListedOtherOffice { get; set; }
        public bool SiteOwnthisEFIN { get; set; }
        public string EFINOwnerSite { get; set; }
        public int SOorSSorEFIN { get; set; }
        public bool SubSiteSendTaxReturn { get; set; }
        public bool SiteanMSOLocation { get; set; }
        public Nullable<bool> IsMainSiteTransmitTaxReturn { get; set; }
        public Nullable<int> NoofTaxProfessionals { get; set; }
        public Nullable<bool> IsSoftwareOnNetwork { get; set; }
        public Nullable<int> NoofComputers { get; set; }
        public Nullable<int> PreferredLanguage { get; set; }
        public Nullable<bool> CanSubSiteLoginToEmp { get; set; }
        public string StatusCode { get; set; }
        public System.Guid CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
        public System.Guid LastUpdatedBy { get; set; }
    }


    //Common DropDown Model
    public class DropDownDTO
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StatusCode { get; set; }
    }


    /// <summary>
    /// Payment Options Models
    /// </summary>
    public class CustomerPaymentInfoSummary
    {
        public List<CustomerPaymentInfoDTO> PaymentOptions { get; set; }
        public bool status { get; set; }
    }

    public class CustomerPaymentInfoDTO
    {
        public int PaymentType { get; set; }
        public int IsSameBankAccount { get; set; }
        public List<FeeSummary> Fees { get; set; }
        public PaymentCreditCardInfo CreditCard { get; set; }
        public PaymetnACH ACH { get; set; }
        public bool status { get; set; }
        public string UserId { get; set; }
        public string Id { get; set; }
        public CustomerReimBankDetails BankDetails { get; set; }
        public int SiteType { get; set; }
        public bool IsFeeReimbursement { get; set; }
        public bool IsEnrollment { get; set; }
        public Guid BankId { get; set; }
    }

    public class FeeSummary
    {
        public string Fee { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentCreditCardInfo
    {
        public string CardHolderName { get; set; }
        public int CardType { get; set; }
        public string Address { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public int StateId { get; set; }
        public string ZipCode { get; set; }
        public bool status { get; set; }
        public Guid UserId { get; set; }
        public Guid PaymentOptionId { get; set; }
    }

    public class PaymetnACH
    {
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string RTN { get; set; }
        public int AccountType { get; set; }
        public bool status { get; set; }
        public Guid UserId { get; set; }
        public Guid PaymentOptionId { get; set; }
    }

    public class PaymentOptionResponse
    {
        public bool status { get; set; }
        public string Id { get; set; }
    }

    public class CustomerReimBankDetails
    {
        public string BankName { get; set; }
        public string Status { get; set; }
        public bool Availble { get; set; }
    }


    ////BankStatus

    public class CustomerBanksResponseDTO
    {
        public bool Status { get; set; }
        public List<CustomerBanksDTO> Banks { get; set; }
    }

    public class CustomerBanksDTO
    {
        public Guid BankId { get; set; }
        public string BankName { get; set; }
        public string Submission { get; set; }
        public string Acceptance { get; set; }
        public string BankStatus { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDefault { get; set; }
        public Guid EnrollId { get; set; }
        public int Default { get; set; }
    }


    ////BankQuestion Model


    public class BankQuestionDTO
    {
        public System.Guid BankId { get; set; }
        public string BankName { get; set; }
        public string DocumentPath { get; set; }
        public decimal DesktopFee { get; set; }
        public decimal MSOFee { get; set; }
        public bool IsSameforAll { get; set; }
        public int ServiceorTransmission { get; set; }
        public List<DropDownDTO> Questions { get; set; }

        public decimal BankSVBDesktopFee { get; set; }
        public decimal BankSVBMSOFee { get; set; }

        public decimal BankTranDesktopFee { get; set; }
        public decimal BankTranMSOFee { get; set; }

        public decimal SubDesktopFee { get; set; }
        public decimal SubMSOFee { get; set; }

        public Guid QuestionId { get; set; }
        public string StatusCode { get; set; }
    }


    public class SubSiteBankFeesDTO 
    {
        public string Id { get; set; }
        public string refId { get; set; }
        public string BankMaster_ID { get; set; }
        public string SubSiteFeeConfig_ID { get; set; }
        public decimal BankMaxFees { get; set; }
        public int ServiceorTransmission { get; set; }
        public Nullable<decimal> BankMaxFees_MSO { get; set; }
        public Nullable<Guid> QuestionID { get; set; }
    }


    public class EntityHierarchyDTO
    {
        public long Id { get; set; }
        public Nullable<System.Guid> RelationId { get; set; }
        public Nullable<int> Customer_Level { get; set; }
        public Nullable<System.Guid> CustomerId { get; set; }
        public Nullable<int> EntityId { get; set; }
        public string Status { get; set; }
        public Nullable<int> FeeSourceEntityId { get; set; }
    }

    public class BankFeeDTO
    {
        public string BankName { get; set; }
        public string BankId { get; set; }
        public string BankCode { get; set; }
        public decimal SvbAmount { get; set; }
        public decimal TransAmount { get; set; }
    }

    public class BankEnrollmentStatusInfoDTO
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



    ////Customer Information

    public class CustomerInformationDTO
    {
        public System.Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string AccountStatus { get; set; }
        public Nullable<bool> Feeder { get; set; }
        public string BusinessOwnerFirstName { get; set; }
        public string BusinessOwnerLastName { get; set; }
        public string OfficePhone { get; set; }
        public string AlternatePhone { get; set; }
        public string Primaryemail { get; set; }
        public string SupportNotificationemail { get; set; }
        public string EROType { get; set; }
        public string AlternativeContact { get; set; }
        public Nullable<int> EFIN { get; set; }
        public string EFINStatusText { get; set; }
        public Nullable<int> EFINStatus { get; set; }
        public string PhysicalAddress1 { get; set; }
        public string PhysicalAddress2 { get; set; }
        public string PhysicalZipcode { get; set; }
        public string PhysicalCity { get; set; }
        public string PhysicalState { get; set; }
        public bool ShippingAddressSameAsPhysicalAddress { get; set; }
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingZipcode { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public Nullable<System.Guid> PhoneTypeId { get; set; }
        public Nullable<System.Guid> TitleId { get; set; }
        public Nullable<System.Guid> AlternativeType { get; set; }
        public string PhoneType { get; set; }
        public string ContactTitle { get; set; }
        public Nullable<System.Guid> SalesYearID { get; set; }
        public Nullable<int> EntityId { get; set; }
        public Nullable<int> BaseEntityId { get; set; }
        public Nullable<System.Guid> ParentId { get; set; }
        public string OfficePortalUrl { get; set; }
        public Nullable<System.Guid> SiteMapId { get; set; }

        public string SalesforceParentID { get; set; }
        public string MasterIdentifier { get; set; }

        public bool IsVerified { get; set; }
        public Nullable<bool> IsMSOUser { get; set; }
        public Nullable<bool> IsEnrollmentSubmit { get; set; }
        public Nullable<int> IsActivationCompleted { get; set; }
        public Nullable<bool> IsAdditionalEFINSubSite { get; set; }

        public bool? IsNotCollectingFee { get; set; }
        public string StatusCode { get; set; }
    }


    public class CustomerLoginInformationDTO
    {
        public string Id { get; set; }
        public Nullable<int> EFIN { get; set; }
        public Nullable<int> EFINStatus { get; set; }
        public string EFINStatusText { get; set; }
        public Nullable<int> EntityId { get; set; }
        public string MasterIdentifier { get; set; }
        public string CrossLinkUserId { get; set; }
        public string CrossLinkPassword { get; set; }
        public string OfficePortalUrl { get; set; }
        public string TaxOfficeUsername { get; set; }
        public string TaxOfficePassword { get; set; }
        public string EMPPassword { get; set; }
        public string EMPUserId { get; set; }
        public string CompanyName { get; set; }
        public string BusinessOwnerFirstName { get; set; }
        public string BusinessOwnerLastName { get; set; }
        public string PhysicalAddress1 { get; set; }
        public bool IsMSOUser { get; set; }
        public string CityStateZip { get; set; }
        public Nullable<System.Guid> CustomerOfficeId { get; set; }

        public string SalesforceParentID { get; set; }
        public string TransmitType { get; set; }
        public string MSO { get; set; }
        public string Bank { get; set; }
        public bool IsAdditionalEFINAllowed { get; set; }
        public string ParentId { get; set; }

        public string MasterIdentifierPassword { get; set; }

        public string CLAccountId { get; set; }
        public string CLLogin { get; set; }
        public string CLAccountPassword { get; set; }
    }

}


