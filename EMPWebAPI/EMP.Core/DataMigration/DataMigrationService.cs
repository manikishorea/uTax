using System;
using System.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Data;

using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using EMP.Core.Utilities;

using EMPEntityFramework.Edmx;
using EMP.Core.DataMigration.DTO;

namespace EMP.Core.DataMigration
{
    public class DataMigrationService : IDataMigrationService, IDisposable
    {

        public DatabaseEntities db = new DatabaseEntities();
        public DatabaseArcEntities dbArc = new DatabaseArcEntities();

        //public UserService(uTaxDBEntities _db, UserDetailDTO _user)
        //{
        //    db = _db;
        //    user = _user;
        //}

        public IQueryable<SalesYearDTO> GetArchiveSalesYears(Guid Id)
        {
            db = new DatabaseEntities();
            dbArc = new DatabaseArcEntities();

            var Arc_Customers = dbArc.emp_CustomerInformation.Where(o => o.SalesYearGroupId == Id).ToList();
            var SalesYears = db.SalesYearMasters.ToList();

            if (Arc_Customers.ToList().Count > 0 && SalesYears.ToList().Count > 0)
            {
                var SalesYears1 = (from cu in Arc_Customers
                                       // join cl in dbArc.emp_CustomerInformation on cu.SalesYearGroupId equals cl.SalesYearGroupId
                                   join sy in SalesYears on cu.SalesYearID equals sy.Id
                                   //where sy.ApplicableToDate != null && cu.SalesYearGroupId == Id && cu.SalesYearID != null //sy.ApplicableFromDate > DateTime.Now.Date &&
                                   select new SalesYearDTO { Id = sy.Id, SalesYear = sy.SalesYear, Description = Id.ToString() }).DefaultIfEmpty().Distinct();
                return SalesYears1.AsQueryable();
            }
            else
            {
                return null;
            }

        }

        public bool SetArchiveData_Old(Guid TokenId, Guid SalesYearId)
        {
            db = new DatabaseEntities();
            var SalesYear = db.SalesYearMasters.OrderByDescending(o => o.ApplicableFromDate).ToList();

            if (SalesYear.Count > 1)
            {
                var FirstSalesYear = SalesYear.Take(1).SingleOrDefault();
                var SecondSalesYear = SalesYear.Skip(1).Take(1).SingleOrDefault();

                List<emp_CustomerInformation> CustInfoList = db.emp_CustomerInformation.Where(o => o.SalesYearID == SecondSalesYear.Id && o.StatusCode != EMPConstants.NewCustomer).ToList(); //&& (o.StatusCode == EMPConstants.Active || o.StatusCode == EMPConstants.Created)
                foreach (var item in CustInfoList.Where(o => o.ParentId == null || o.ParentId == Guid.Empty).ToList())
                {
                    emp_CustomerInformation CustInfoSave = new emp_CustomerInformation();

                    if (item != null)
                    {
                        Guid OldCustId = item.Id;
                        // if (item.IsActivationCompleted == 1)
                        {
                            var mainResult = db.ArchiveData_InsertSP(OldCustId, 1, SalesYearId, TokenId);
                        }

                        List<emp_CustomerInformation> CustInfoParentList = CustInfoList.Where(o => o.ParentId == OldCustId).ToList();
                        foreach (var child in CustInfoParentList)
                        {
                            if (child != null)
                            {
                                //if (child.IsActivationCompleted == 1)
                                {
                                    var SubResult = db.ArchiveData_InsertSP(child.Id, 2, SalesYearId, TokenId);
                                }

                                List<emp_CustomerInformation> CustInfoAddEFINParentList = CustInfoList.Where(o => o.ParentId == child.Id).ToList();
                                foreach (var NewChild in CustInfoAddEFINParentList)
                                {
                                    if (NewChild != null)
                                    {
                                        //  if (NewChild.IsActivationCompleted == 1)
                                        {
                                            var AddEFINResult = db.ArchiveData_InsertSP(NewChild.Id, 2, SalesYearId, TokenId);
                                        }

                                        List<emp_CustomerInformation> CustInfoAddEFINParentList1 = CustInfoList.Where(o => o.ParentId == NewChild.Id).ToList();
                                        foreach (var NewChild1 in CustInfoAddEFINParentList1)
                                        {
                                            if (NewChild1 != null)
                                            {
                                                //  if (NewChild.IsActivationCompleted == 1)
                                                {
                                                    var AddEFINResult1 = db.ArchiveData_InsertSP(NewChild1.Id, 2, SalesYearId, TokenId);
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }

                }
            }
            return true;
        }

        public bool SetArchiveData(Guid TokenId, Guid SalesYearId)
        {
            db = new DatabaseEntities();
            var SalesYear = db.SalesYearMasters.OrderByDescending(o => o.ApplicableFromDate).ToList();

            if (SalesYear.Count > 1)
            {
                var FirstSalesYear = SalesYear.Take(1).SingleOrDefault();
                var SecondSalesYear = SalesYear.Skip(1).Take(1).SingleOrDefault();

                List<emp_CustomerInformation> CustInfoList = db.emp_CustomerInformation.Where(o => o.SalesYearID == SecondSalesYear.Id && o.StatusCode != EMPConstants.NewCustomer).ToList(); //&& (o.StatusCode == EMPConstants.Active || o.StatusCode == EMPConstants.Created)
                foreach (var item in CustInfoList)
                {
                    emp_CustomerInformation CustInfoSave = new emp_CustomerInformation();

                    if (item != null)
                    {
                        Guid OldCustId = item.Id;

                        if (item.EntityId == (int)EMPConstants.Entity.MO || item.EntityId == (int)EMPConstants.Entity.SVB)
                        {
                            var mainResult = db.ArchiveData_InsertSP(OldCustId, 1, SalesYearId, TokenId);
                        }
                        else
                        {
                            var SubResult = db.ArchiveData_InsertSP(OldCustId, 2, SalesYearId, TokenId);
                        }
                    }
                }

                var NewSalesYear = db.NewSalesYear_Update(FirstSalesYear.Id);
            }
            return true;
        }

        public int SetArchiveDataCount(Guid TokenId)
        {
            db = new DatabaseEntities();
            var SalesYear = db.SalesYearMasters.OrderByDescending(o => o.ApplicableFromDate).ToList();
            if (SalesYear.Count > 1)
            {
                var FirstSalesYear = SalesYear.Take(1).SingleOrDefault();
                var SecondSalesYear = SalesYear.Skip(1).Take(1).SingleOrDefault();
                List<emp_CustomerInformation> CustInfoList = db.emp_CustomerInformation.Where(o => o.SalesYearID == SecondSalesYear.Id && o.StatusCode != EMPConstants.NewCustomer).ToList(); // && (o.IsActivationCompleted == 0)
                if (CustInfoList.Count > 0)
                {
                    var tempdata = db.TempArchiveCustomerInfoes.ToList();

                    //var deletedata = db.TempArchiveCustomerInfoes.Where(o => o.TokenId == TokenId).ToList();
                    if (tempdata.ToList().Count > 0)
                    {
                        db.TempArchiveCustomerInfoes.RemoveRange(tempdata);
                        db.SaveChanges();
                    }

                    foreach (var cu in CustInfoList)
                    {
                        TempArchiveCustomerInfo otempInfo = new TempArchiveCustomerInfo();
                        otempInfo.CustomerId = cu.Id;
                        otempInfo.Status = "ACT";
                        otempInfo.TokenId = TokenId;
                        db.TempArchiveCustomerInfoes.Add(otempInfo);
                    }

                    db.SaveChanges();
                    db.Dispose();
                }
                return CustInfoList.Count;
            }
            else
            {
                return 0;
            }
        }

        public int SetArchiveDataStatus(Guid TokenId)
        {
            db = new DatabaseEntities();
            int tempDone = db.TempArchiveCustomerInfoes.Where(a => a.Status == "DONE" && a.TokenId == TokenId).Count();
            return tempDone;
        }

        public ArchiveMainSiteDataModel GetArchiveDataMainSite(Guid UserId)
        {
            ArchiveMainSiteDataModel ArchiveData = new ArchiveMainSiteDataModel();

            ArchiveData.MainOfficeConfigDTO = dbArc.MainOfficeConfigurations.Where(o => o.emp_CustomerInformation_ID == UserId).Select(o => new MainOfficeConfigurationDTO
            {
                Id = o.Id,
                refId = o.emp_CustomerInformation_ID,
                IsSiteTransmitTaxReturns = o.IsSiteTransmitTaxReturns,
                IsSiteOfferBankProducts = o.IsSiteOfferBankProducts,
                TaxProfessionals = o.TaxProfessionals,
                IsSoftwarebeInstalledNetwork = o.IsSoftwarebeInstalledNetwork,
                ComputerswillruninSoftware = o.ComputerswillruninSoftware,
                PreferredSupportLanguage = o.PreferredSupportLanguage,
            }).FirstOrDefault();

            //  ArchiveData.SubSiteConfigDTO

            ArchiveData.SubSiteConfigDTO = dbArc.SubSiteConfigurations.Where(o => o.emp_CustomerInformation_ID == UserId).Select(o => new SubSiteConfigurationDTO
            {
                Id = o.ID,
                refId = o.emp_CustomerInformation_ID,
                IsuTaxManageingEnrolling = o.IsuTaxManageingEnrolling,
                IsuTaxPortalEnrollment = o.IsuTaxPortalEnrollment,
                IsuTaxManageOnboarding = o.IsuTaxManageOnboarding,
                IsuTaxCustomerSupport = o.IsuTaxCustomerSupport,
                NoofSupportStaff = o.NoofSupportStaff,
                NoofDays = o.NoofDays,
                OpenHours = o.OpenHours.Value.ToString(),// "hh:mm tt"),// "hh:mm tt"),
                CloseHours = o.CloseHours.Value.ToString(),//"hh:mm tt"),//"hh:mm tt"),
                TimeZone = o.TimeZone.ToString(),
                SubSiteTaxReturn = o.SubSiteTaxReturn,
                IsAutoEnrollAffiliateProgram = o.IsAutoEnrollAffiliateProgram,
                IsSubSiteEFINAllow = o.IsSubSiteEFINAllow,
                CanSubSiteLoginToEmp = o.CanSubSiteLoginToEmp,
                SubSiteAffiliateProgramDTOs = o.SubSiteAffiliateProgramConfigs.Select(s => new SubSiteAffiliateProgramConfigDTO() { AffiliateProgramId = s.AffiliateProgramMaster_ID }).ToList(),
                SubSitBankQuestionDTOs = o.SubSiteBankConfigs.Select(s => new SubSiteBankConfigDTO() { BankId = s.BankMaster_ID, QuestionId = s.SubQuestion_ID ?? Guid.Empty }).ToList()
            }).FirstOrDefault();

            if (ArchiveData.SubSiteConfigDTO != null)
            {
                DateTime dt = new DateTime();
                bool res = DateTime.TryParse(ArchiveData.SubSiteConfigDTO.OpenHours.ToString(), out dt);
                string ddt = new DateTime().Add(dt.TimeOfDay).ToString("hh:mm tt");
                ArchiveData.SubSiteConfigDTO.OpenHours = ddt;

                DateTime dt1 = new DateTime();
                bool res1 = DateTime.TryParse(ArchiveData.SubSiteConfigDTO.CloseHours.ToString(), out dt1);
                string ddt1 = new DateTime().Add(dt1.TimeOfDay).ToString("hh:mm tt");
                ArchiveData.SubSiteConfigDTO.CloseHours = ddt1;
            }


            ArchiveData.SubSiteFeeDTOs = dbArc.SubSiteFeeConfigs.Where(o => o.emp_CustomerInformation_ID == UserId).Select(o => new SubSiteFeeConfigDTO()
            {
                Id = o.ID,
                refId = o.emp_CustomerInformation_ID,
                IsAddOnFeeCharge = o.IsAddOnFeeCharge,
                IsSameforAll = o.IsSameforAll,
                IsSubSiteAddonFee = o.IsSubSiteAddonFee,
                ServiceorTransmission = o.ServiceorTransmission,

            }).ToList();

            foreach (SubSiteFeeConfigDTO r in ArchiveData.SubSiteFeeDTOs)
            {
                r.SubSiteBankFeesDTOs = dbArc.SubSiteBankFeesConfigs.Where(a => a.SubSiteFeeConfig_ID == r.Id).Select(a => new SubSiteBankFeesConfigDTO
                {
                    BankID = a.BankMaster_ID,
                    AmountDSK = a.BankMaxFees,
                    AmountMSO = a.BankMaxFees_MSO
                }).ToList();
            }

            ArchiveData.FeeReimburseConfigDTO = dbArc.FeeReimbursementConfigs.Where(o => o.emp_CustomerInformation_ID == UserId).Select(o => new FeeReimbursementConfigDTO
            {
                ID = o.ID,
                refId = o.emp_CustomerInformation_ID,
                AccountName = o.AccountName,
                BankName = o.BankName,
                AccountType = o.AccountType,
                RTN = o.RTN,
                BankAccountNo = o.BankAccountNo,
                IsAuthorize = o.IsAuthorize,
            }).FirstOrDefault();

            return ArchiveData;
        }

        public ArchiveEnrollmentDataModel GetArchiveDataEnrollment(Guid UserId, Guid ParentId)
        {
            ArchiveEnrollmentDataModel ArchiveData = new ArchiveEnrollmentDataModel();

            ArchiveData.CustomerInformationDTO = GetCustomerInformation(UserId);
            ArchiveData.CustomerLoginInformationDTO = GetCustomerLoginInformation(UserId);
            ArchiveData.ParentInformationDTO = GetCustomerInformation(UserId, 1);

            ArchiveData.EnrollmentOfficeConfigDTO = dbArc.EnrollmentOfficeConfigurations.Where(o => o.CustomerId == UserId).Select(o => new EnrollmentOfficeConfigurationDTO
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                IsMainSiteTransmitTaxReturn = o.IsMainSiteTransmitTaxReturn,
                NoofComputers = o.NoofComputers,
                NoofTaxProfessionals = o.NoofTaxProfessionals,
                PreferredLanguage = o.PreferredLanguage,
                IsSoftwareOnNetwork = o.IsSoftwareOnNetwork,
                StatusCode = o.StatusCode,
            }).FirstOrDefault();


            ArchiveData.EnrollmentAffiliateConfigDTOs = dbArc.EnrollmentAffiliateConfigurations.Where(o => o.CustomerId == UserId).Select(o => new EnrollmentAffiliateConfigurationDTO
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                AffiliateProgramId = o.AffiliateProgramId,
                AffiliateProgramCharge = o.AffiliateProgramCharge,
            }).ToList();

            ArchiveData.EnrollmentFeeReimbursementDTO = dbArc.EnrollmentFeeReimbursementConfigs.Where(o => o.emp_CustomerInformation_ID == UserId).Select(o => new EnrollmentFeeReimbursementConfigDTO
            {
                ID = o.ID,
                refId = o.emp_CustomerInformation_ID,
                AccountName = o.AccountName,
                BankName = o.BankName,
                AccountType = o.AccountType,
                RTN = o.RTN,
                BankAccountNo = o.BankAccountNo,
                IsAuthorize = o.IsAuthorize,
            }).FirstOrDefault();

            ArchiveData.EnrollmentBankSelectDTOs = GetEnrollmentBankSelection(UserId, ParentId);

            if (ArchiveData.EnrollmentBankSelectDTOs.ToList().Count > 0)
            {
                if (ArchiveData.EnrollmentBankSelectDTOs[0].BankCode == "S")
                {
                    ArchiveData.TPGBankEnrollmentDTO = GetTPGBankEnrollment(UserId);
                }
                if (ArchiveData.EnrollmentBankSelectDTOs[0].BankCode == "R")
                {
                    ArchiveData.RBBankEnrollmentDTO = GetRBBankEnrollment(UserId);
                }
                if (ArchiveData.EnrollmentBankSelectDTOs[0].BankCode == "V")
                {
                    ArchiveData.RABankEnrollmentDTO = GetRABankEnrollment(UserId);
                }
            }

            if (ArchiveData.EnrollmentBankSelectDTOs.Count > 0)
            {
                ArchiveData.eFilePaymentInfoDTO = GetCustomerPaymentInfo(UserId, 1, ArchiveData.EnrollmentBankSelectDTOs[0].BankId);

                ArchiveData.OutstandingPaymentInfoDTO = GetCustomerPaymentInfo(UserId, 2, ArchiveData.EnrollmentBankSelectDTOs[0].BankId);
            }

            ArchiveData.CustomerBanksResponseDTO = getCutomerBanks(UserId);

            return ArchiveData;
        }

        public List<EnrollmentBankSelectionDTO> GetEnrollmentBankSelection(Guid userid, Guid Parentid)
        {
            try
            {
                db = new DatabaseEntities();
                dbArc = new DatabaseArcEntities();
                List<EnrollmentBankSelectionDTO> enrollmentbankSelection = new List<EnrollmentBankSelectionDTO>();

                var itm = (from rdb in dbArc.EnrollmentBankSelections where rdb.CustomerId == userid && rdb.StatusCode == EMPConstants.Active orderby rdb.BankSubmissionStatus descending, rdb.LastUpdatedDate descending select rdb).FirstOrDefault();
                // foreach (var itm in data)
                if (itm != null)
                {
                    List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                    EntityHierarchyDTOs = GetEntityHierarchies(userid);
                    var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                    Guid TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;


                    EnrollmentBankSelectionDTO enrollmentModel = new EnrollmentBankSelectionDTO();
                    enrollmentModel.Id = itm.Id;
                    enrollmentModel.CustomerId = itm.CustomerId;
                    enrollmentModel.BankId = itm.BankId;
                    enrollmentModel.BankCode = db.BankMasters.Where(o => o.Id == itm.BankId).Select(o => o.BankCode).FirstOrDefault() ?? "";
                    enrollmentModel.QuestionId = itm.QuestionId;
                    enrollmentModel.IsServiceBureauFee = itm.IsServiceBureauFee ?? false;
                    enrollmentModel.ServiceBureauBankAmount = itm.ServiceBureauBankAmount ?? 0;
                    enrollmentModel.IsTransmissionFee = itm.IsTransmissionFee ?? false;
                    enrollmentModel.TransmissionBankAmount = itm.TransmissionBankAmount ?? 0;
                    enrollmentModel.StatusCode = itm.StatusCode;

                    Guid bankmstid = new Guid("A29B3547-8954-4036-9BD3-312F1D6A3DAA");

                    var Options = (from ssb in dbArc.SubSiteBankConfigs
                                   join bs in dbArc.BankSubQuestions on ssb.SubQuestion_ID equals bs.Id
                                   where ssb.emp_CustomerInformation_ID == TopParentId && ssb.BankMaster_ID == bankmstid
                                   select bs.Options).FirstOrDefault();
                    if (Options != null)
                        enrollmentModel.TPGOptions = Options ?? 0;
                    else
                    {
                        enrollmentModel.TPGOptions = 0;
                    }



                    var SsFConfig = (from ssf in dbArc.SubSiteFeeConfigs where ssf.emp_CustomerInformation_ID == TopParentId select new { ssf.IsSubSiteAddonFee, ssf.ServiceorTransmission });
                    if (SsFConfig != null && Parentid != Guid.Empty)
                    {
                        foreach (var itms in SsFConfig)
                        {
                            if (itms.ServiceorTransmission == 1)
                                enrollmentModel.IsDVServiceBureauFee = itms.IsSubSiteAddonFee;
                            if (itms.ServiceorTransmission == 2)
                                enrollmentModel.IsDVTransmissionFee = itms.IsSubSiteAddonFee;
                        }
                    }
                    else
                    {
                        enrollmentModel.IsDVServiceBureauFee = true;
                        enrollmentModel.IsDVTransmissionFee = true;
                    }

                    enrollmentbankSelection.Add(enrollmentModel);
                }

                return enrollmentbankSelection;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/GetEnrollmentBankSelection", userid);
                return null;
            }
        }

        public RBBankEnrollmentDTO GetRBBankEnrollment(Guid CustomerId)
        {
            var IsMainSite = true;
            RBBankEnrollmentDTO rb = new DTO.RBBankEnrollmentDTO();
            db = new DatabaseEntities();
            try
            {
                var isExist = dbArc.BankEnrollmentForRBs.Where(x => x.CustomerId == CustomerId).FirstOrDefault();

                if (isExist != null)
                {
                    rb.ActualNoofBankProductsLastYear = isExist.ActualNoofBankProductsLastYear.Value;
                    rb.AdvertisingApproval = isExist.AdvertisingApproval;
                    rb.AltOfficeContact1Email = isExist.AltOfficeContact1Email;
                    rb.AltOfficeContact1FirstName = isExist.AltOfficeContact1FirstName;
                    rb.AltOfficeContact1LastName = isExist.AltOfficeContact1LastName;
                    rb.AltOfficeContact1SSn = isExist.AltOfficeContact1SSn;
                    rb.AltOfficeContact2Email = isExist.AltOfficeContact2Email;
                    rb.AltOfficeContact2FirstName = isExist.AltOfficeContact2FirstName;
                    rb.AltOfficeContact2LastName = isExist.AltOfficeContact2LastName;
                    rb.AltOfficeContact2SSn = isExist.AltOfficeContact2SSn;
                    rb.AltOfficePhysicalAddress = isExist.AltOfficePhysicalAddress;
                    rb.AltOfficePhysicalAddress2 = isExist.AltOfficePhysicalAddress2;
                    rb.AltOfficePhysicalCity = isExist.AltOfficePhysicalCity;
                    rb.AltOfficePhysicalState = isExist.AltOfficePhysicalState;
                    rb.AltOfficePhysicalZipcode = isExist.AltOfficePhysicalZipcode;
                    rb.AntivirusRequired = isExist.AntivirusRequired;
                    rb.BankAccountNumber = isExist.BankAccountNumber;
                    rb.BankAccountType = isExist.BankAccountType;
                    rb.BankRoutingNumber = isExist.BankRoutingNumber;
                    rb.BusinessEIN = isExist.BusinessEIN;
                    rb.BusinessIncorporation = isExist.BusinessIncorporation.HasValue ? isExist.BusinessIncorporation.Value.ToString("MM/dd/yyyy").Replace("-", "/") : "";
                    rb.BusinessName = isExist.BusinessName;
                    rb.CellPhoneNumber = isExist.CellPhoneNumber.Replace("-", "");
                    rb.CheckingAccountName = isExist.CheckingAccountName;
                    rb.ConsumerLending = isExist.ConsumerLending;
                    rb.EFINOwnerDOB = isExist.EFINOwnerDOB.HasValue ? isExist.EFINOwnerDOB.Value.ToString("MM/dd/yyyy").Replace("-", "/") : "";
                    rb.EFINOwnerFirstName = isExist.EFINOwnerFirstName;
                    rb.EFINOwnerLastName = isExist.EFINOwnerLastName;
                    rb.EFINOwnerSSN = isExist.EFINOwnerSSN;
                    rb.EmailAddress = isExist.EmailAddress;
                    rb.EROApplicattionDate = isExist.EROApplicattionDate.HasValue ? isExist.EROApplicattionDate.Value.ToString("MM/dd/yyyy hh:mm tt").Replace("-", "/") : "";
                    rb.EROParticipation = isExist.EROParticipation;
                    rb.EROReadTAndC = isExist.EROReadTAndC;
                    rb.FAXNumber = isExist.FAXNumber;
                    rb.FulfillmentShippingAddress = isExist.FulfillmentShippingAddress;
                    rb.FulfillmentShippingCity = isExist.FulfillmentShippingCity;
                    rb.FulfillmentShippingState = isExist.FulfillmentShippingState;
                    rb.FulfillmentShippingZip = isExist.FulfillmentShippingZip;
                    rb.HasFirewall = isExist.HasFirewall;
                    rb.IsAsPerComplainceLaw = isExist.IsAsPerComplainceLaw;
                    rb.IsAsPerProcessLaw = isExist.IsAsPerProcessLaw;
                    rb.IsLimitAccess = isExist.IsLimitAccess;
                    rb.IsLockedStore_Checks = isExist.IsLockedStore_Checks;
                    rb.IsLockedStore_Documents = isExist.IsLockedStore_Documents;
                    rb.IsLocked_Office = isExist.IsLocked_Office;
                    rb.IsMultiOffice = isExist.IsMultiOffice;
                    rb.IsOfficeTransmit = isExist.IsOfficeTransmit;
                    rb.IsPTIN = isExist.IsPTIN;
                    rb.LegarEntityStatus = isExist.LegarEntityStatus;
                    rb.LLCMembershipRegistration = isExist.LLCMembershipRegistration;
                    rb.LoginAccesstoEmployees = isExist.LoginAccesstoEmployees;
                    rb.MailingAddress = isExist.MailingAddress;
                    rb.MailingCity = isExist.MailingCity;
                    rb.MailingState = isExist.MailingState;
                    rb.MailingZip = isExist.MailingZip;
                    rb.NoofBankProductsLastYear = isExist.NoofBankProductsLastYear.Value;
                    rb.NoofPersoneel = isExist.NoofPersoneel.Value;
                    rb.OfficeContactFirstName = isExist.OfficeContactFirstName;
                    rb.OfficeContactLastName = isExist.OfficeContactLastName;
                    rb.OfficeContactSSN = isExist.OfficeContactSSN;
                    rb.OfficeManageLastName = isExist.OfficeManageLastName;
                    if (isExist.OfficeContactDOB != null)
                        rb.OfficeContactDOB = isExist.OfficeContactDOB.Value.ToString("MM/dd/yyyy").Replace("-", "/");
                    rb.OfficeManagerPhone = isExist.OfficeManagerPhone;
                    rb.OfficeManagerCellNo = isExist.OfficeManagerCellNo;
                    rb.OfficeManagerEmail = isExist.OfficeManagerEmail;

                    rb.OfficeManagerDOB = isExist.OfficeManagerDOB.Value.ToString("MM/dd/yyyy").Replace("-", "/");
                    rb.OfficeManagerFirstName = isExist.OfficeManagerFirstName;
                    rb.OfficeManagerSSN = isExist.OfficeManagerSSN;
                    rb.OfficeName = isExist.OfficeName;
                    rb.OfficePhoneNumber = isExist.OfficePhoneNumber.Replace("-", "");
                    rb.OfficePhysicalAddress = isExist.OfficePhysicalAddress;
                    rb.OfficePhysicalCity = isExist.OfficePhysicalCity;
                    rb.OfficePhysicalState = isExist.OfficePhysicalState;
                    rb.OfficePhysicalZip = isExist.OfficePhysicalZip;
                    rb.OnlineTraining = isExist.OnlineTraining;
                    rb.OnwerDOB = isExist.OnwerDOB.Value.ToString("MM/dd/yyyy").Replace("-", "/");
                    rb.OwnerAddress = isExist.OwnerAddress;
                    rb.OwnerCity = isExist.OwnerCity;
                    rb.OwnerFirstName = isExist.OwnerFirstName;
                    rb.OwnerHomePhone = isExist.OwnerHomePhone.Replace("-", "");
                    rb.OwnerLastName = isExist.OwnerLastName;
                    rb.OwnerSSN = isExist.OwnerSSN;
                    rb.OwnerState = isExist.OwnerState;
                    rb.OwnerZip = isExist.OwnerZip;
                    rb.PasswordRequired = isExist.PasswordRequired;
                    rb.PlantoDispose = isExist.PlantoDispose;
                    rb.PreviousBankProductFacilitator = isExist.PreviousBankProductFacilitator;
                    rb.ProductsOffering = isExist.ProductsOffering.Value;
                    rb.RetailPricingMethod = isExist.RetailPricingMethod;
                    rb.SPAAmount = isExist.SPAAmount.Value;
                    rb.SPADate = isExist.SPADate.HasValue ? isExist.SPADate.Value.ToString("MM/dd/yyyy hh:mm tt").Replace("-", "/") : "";
                    rb.WebsiteAddress = isExist.WebsiteAddress;
                    rb.YearsinBusiness = isExist.YearsinBusiness.Value;
                    rb.EFINOwnerTitle = isExist.EFINOwnerTitle;
                    rb.EFINOwnerAddress = isExist.EFINOwnerAddress;
                    rb.EFINOwnerCity = isExist.EFINOwnerCity;
                    rb.EFINOwnerState = isExist.EFINOwnerState;
                    rb.EFINOwnerZip = isExist.EFINOwnerZip;
                    rb.EFINOwnerPhone = isExist.EFINOwnerPhone;
                    rb.EFINOwnerMobile = isExist.EFINOwnerMobile;
                    rb.EFINOwnerEmail = isExist.EFINOwnerEmail;
                    rb.EFINOwnerIDNumber = isExist.EFINOwnerIDNumber;
                    rb.EFINOwnerIDState = isExist.EFINOwnerIDState;
                    rb.EFINOwnerEIN = isExist.EFINOwnerEIN;
                    rb.SupportOS = isExist.SupportOS;
                    rb.BankName = isExist.BankName;
                    rb.SBFeeonAll = isExist.SBFeeonAll;
                    rb.SBFee = isExist.SBFee;
                    rb.TransimissionAddon = isExist.TransimissionAddon;
                    rb.PrepaidCardProgram = isExist.PrepaidCardProgram;
                    rb.TandC = isExist.TandC.HasValue ? isExist.TandC.Value : false;


                    var info = dbArc.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                    if ((info.ParentId ?? Guid.Empty) != Guid.Empty)
                    {
                        IsMainSite = false;
                        var SubSiteOffice = dbArc.SubSiteOfficeConfigs.Where(o => o.RefId == info.Id).FirstOrDefault();
                        if (SubSiteOffice != null)
                        {
                            info.IsMSOUser = SubSiteOffice.SiteanMSOLocation;
                        }
                    }

                    decimal addon = 0, sb = 0;
                    var bankid = dbArc.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                    string addontitle = string.Empty, svbfeetitle = string.Empty;

                    if (bankid != null)
                    {
                        if (bankid.BankId != Guid.Empty)
                        {
                            var addonMasterfees = (from f in db.FeeMasters
                                                   where f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                                   select f.Amount).FirstOrDefault();
                            addon += addonMasterfees.Value;
                            addontitle = addontitle + "Cross Link Transmitter Fee :" + addonMasterfees.Value + ", ";

                            var xmissionAddownfees = dbArc.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 2 && x.BankMaster_ID == bankid.BankId).ToList();

                            if (!IsMainSite)
                            {
                                if ((info.IsMSOUser ?? false) == false)
                                {
                                    decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees);
                                    addon += BankFee;
                                    addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                }
                                else
                                {
                                    decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                    addon += BankFee;
                                    addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                }
                            }

                            addon += bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0;
                            addontitle = addontitle + "Add On Fee :" + (bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0);

                            var SvbAddownfees = dbArc.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 1 && x.BankMaster_ID == bankid.BankId).ToList();

                            var mainfees = (from f in db.FeeMasters
                                            where f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                                            select f.Amount).FirstOrDefault();
                            sb += mainfees.Value;
                            svbfeetitle = svbfeetitle + "uTax Service Fee :" + mainfees + ", ";
                            if (!IsMainSite)
                            {
                                if ((info.IsMSOUser ?? false) == false)
                                {
                                    decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees);
                                    sb += BankMaxFees;
                                    svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                }
                                else
                                {
                                    decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                    sb += BankMaxFees;
                                    svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                }

                            }
                            sb += bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0;
                            svbfeetitle = svbfeetitle + "Add On Fee :" + (bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0);
                        }
                    }

                    rb.AddonfeeTitle = addontitle;
                    rb.ServiceBureaufeeTitle = svbfeetitle;

                    //rb.TransimissionAddon = addon.ToString("0.00");
                    //rb.SBFee = sb.ToString("0.00");

                }

                return rb;


            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/GetRBBankEnrollment", CustomerId);
                return null;
            }
        }

        public RABankEnrollmentDTO GetRABankEnrollment(Guid CustomerId)
        {
            db = new DatabaseEntities();
            var IsMainSite = true;
            RABankEnrollmentDTO ra = new DTO.RABankEnrollmentDTO();
            try
            {
                var isExist = dbArc.BankEnrollmentForRAs.Where(x => x.CustomerId == CustomerId).FirstOrDefault();
                if (isExist != null)
                {

                    ra.BankAccountNumber = isExist.BankAccountNumber;
                    ra.BankAccountType = isExist.BankAccountType;
                    ra.BankRoutingNumber = isExist.BankRoutingNumber;
                    ra.BusinessEIN = isExist.BusinessEIN;
                    ra.BusinessFederalIDNumber = isExist.BusinessFederalIDNumber;
                    ra.CollectionofBusinessOwners = isExist.CollectionofBusinessOwners;
                    ra.CollectionOfOtherOwners = isExist.CollectionOfOtherOwners;
                    ra.CorporationType = isExist.CorporationType;
                    ra.EFINOwnersSite = isExist.EFINOwnersSite;
                    ra.EROMailingCity = isExist.EROMailingCity;
                    ra.EROMailingState = isExist.EROMailingState;
                    ra.EROMailingZipcode = isExist.EROMailingZipcode;
                    ra.EROMaillingAddress = isExist.EROMaillingAddress;
                    ra.EROOfficeAddress = isExist.EROOfficeAddress;
                    ra.EROOfficeCity = isExist.EROOfficeCity;
                    ra.EROOfficeName = isExist.EROOfficeName;
                    ra.EROOfficePhone = isExist.EROOfficePhone;
                    ra.EROOfficeState = isExist.EROOfficeState;
                    ra.EROOfficeZipCoce = isExist.EROOfficeZipCoce;
                    ra.EROShippingAddress = isExist.EROShippingAddress;
                    ra.EROShippingCity = isExist.EROShippingCity;
                    ra.EROShippingState = isExist.EROShippingState;
                    ra.EROShippingState = isExist.EROShippingState;
                    ra.EROShippingZip = isExist.EROShippingZip;
                    ra.ExpectedCurrentYearVolume = isExist.ExpectedCurrentYearVolume.HasValue ? isExist.ExpectedCurrentYearVolume.Value : 0;
                    ra.HasAssociatedWithVictims = isExist.HasAssociatedWithVictims;
                    ra.IRSAddress = isExist.IRSAddress;
                    ra.IRSCity = isExist.IRSCity;
                    ra.IRSState = isExist.IRSState;
                    ra.IRSZipcode = isExist.IRSZipcode;
                    ra.IsLastYearClient = isExist.IsLastYearClient;
                    ra.NoofYearsExperience = isExist.NoofYearsExperience.HasValue ? isExist.NoofYearsExperience.Value : 0;
                    ra.OwnerAddress = isExist.OwnerAddress;
                    ra.OwnerCellPhone = isExist.OwnerCellPhone;
                    ra.OwnerCity = isExist.OwnerCity;

                    if (isExist.OwnerDOB != DateTime.MinValue)
                    {
                        ra.OwnerDOB = (isExist.OwnerDOB ?? DateTime.MinValue).ToString("MM/dd/yyyy").Replace("-", "/");
                    }

                    ra.OwnerEmail = isExist.OwnerEmail;
                    ra.OwnerFirstName = isExist.OwnerFirstName;
                    ra.OwnerHomePhone = isExist.OwnerHomePhone;
                    ra.OwnerIssuingState = isExist.OwnerIssuingState;
                    ra.OwnerLastName = isExist.OwnerLastName;
                    ra.OwnerSSN = isExist.OwnerSSN;
                    ra.OwnerState = isExist.OwnerState;
                    ra.OwnerStateIssuedIdNumber = isExist.OwnerStateIssuedIdNumber;
                    ra.OwnerZipCode = isExist.OwnerZipCode;
                    ra.PreviousBankName = isExist.PreviousBankName;
                    ra.PreviousYearVolume = isExist.PreviousYearVolume.Value;

                    ra.AccountName = isExist.AccountName;
                    ra.AgreeTandC = isExist.AgreeTandC.Value;
                    ra.BankName = isExist.BankName;
                    ra.OwnerTitle = isExist.OwnerTitle;
                    ra.SbFee = isExist.SbFee;
                    ra.SbFeeall = isExist.SbFeeall;
                    ra.TransmissionAddon = isExist.TransmissionAddon;
                    ra.ElectronicFee = isExist.ElectronicFee;
                    ra.MainContactFirstName = isExist.MainContactFirstName;
                    ra.MainContactLastName = isExist.MainContactLastName;
                    ra.MainContactPhone = isExist.MainContactPhone;
                    ra.TextMessages = isExist.TextMessages.HasValue ? isExist.TextMessages.Value : false;
                    ra.LegalIssues = isExist.LegalIssues.HasValue ? isExist.LegalIssues.Value : false;
                    ra.StateOfIncorporation = isExist.StateOfIncorporation;

                    ra.RAEFINOwnerInfo = dbArc.BankEnrollmentEFINOwnersForRAs.Where(o => o.BankEnrollmentRAId == isExist.Id).Select(o => new EnrollmentBankEFINOwnerRADTO()
                    {
                        Id = o.Id,
                        EmailId = o.EmailId,
                        FirstName = o.FirstName,
                        LastName = o.LastName,
                        Address = o.Address,
                        BankEnrollmentRAId = o.BankEnrollmentRAId,
                        City = o.City,
                        StateId = o.StateId,
                        MobilePhone = o.MobilePhone,
                        HomePhone = o.HomePhone,
                        DateofBirth = o.DateofBirth,
                        IDNumber = o.IDNumber,
                        IDState = o.IDState,
                        PercentageOwned = o.PercentageOwned,
                        SSN = o.SSN,
                        ZipCode = o.ZipCode,
                        UpdatedBy = o.UpdatedBy,
                        UpdatedDate = o.UpdatedDate
                    }).ToList();




                    var info = dbArc.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                    if ((info.ParentId ?? Guid.Empty) != Guid.Empty)
                    {
                        IsMainSite = false;
                        var SubSiteOffice = dbArc.SubSiteOfficeConfigs.Where(o => o.RefId == info.Id).FirstOrDefault();
                        if (SubSiteOffice != null)
                        {
                            info.IsMSOUser = SubSiteOffice.SiteanMSOLocation;
                        }
                    }

                    decimal addon = 0, sb = 0;
                    var bankid = dbArc.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                    string addontitle = string.Empty, svbfeetitle = string.Empty;

                    if (bankid != null)
                    {
                        if (bankid.BankId != Guid.Empty)
                        {
                            var addonMasterfees = (from f in db.FeeMasters
                                                   where f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                                   select f.Amount).FirstOrDefault();
                            addon += addonMasterfees.Value;
                            addontitle = addontitle + "Cross Link Transmitter Fee :" + addonMasterfees.Value + ", ";

                            var xmissionAddownfees = dbArc.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 2 && x.BankMaster_ID == bankid.BankId).ToList();

                            if (!IsMainSite)
                            {
                                if ((info.IsMSOUser ?? false) == false)
                                {
                                    decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees);
                                    addon += BankFee;
                                    addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                }
                                else
                                {
                                    decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                    addon += BankFee;
                                    addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                }
                            }

                            addon += bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0;
                            addontitle = addontitle + "Add On Fee :" + (bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0);

                            var SvbAddownfees = dbArc.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 1 && x.BankMaster_ID == bankid.BankId).ToList();

                            var mainfees = (from f in db.FeeMasters
                                            where f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                                            select f.Amount).FirstOrDefault();
                            sb += mainfees.Value;
                            svbfeetitle = svbfeetitle + "uTax Service Fee :" + mainfees + ", ";
                            if (!IsMainSite)
                            {
                                if ((info.IsMSOUser ?? false) == false)
                                {
                                    decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees);
                                    sb += BankMaxFees;
                                    svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                }
                                else
                                {
                                    decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                    sb += BankMaxFees;
                                    svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                }

                            }
                            sb += bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0;
                            svbfeetitle = svbfeetitle + "Add On Fee :" + (bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0);
                        }
                    }

                    ra.AddonfeeTitle = addontitle;
                    ra.ServiceBureaufeeTitle = svbfeetitle;


                }

                return ra;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/GetRABankEnrollment", Guid.Empty);
                return null;
            }
        }

        public TPGBankEnrollmentDTO GetTPGBankEnrollment(Guid CustomerId)
        {
            var IsMainSite = true;
            db = new DatabaseEntities();
            TPGBankEnrollmentDTO tpg = new DTO.TPGBankEnrollmentDTO();
            try
            {
                var isExist = dbArc.BankEnrollmentForTPGs.Where(x => x.CustomerId == CustomerId).FirstOrDefault();

                if (isExist != null)
                {
                    tpg.AccountType = isExist.OfficeAccountType;
                    tpg.BankProductFund = isExist.PriorYearFund;
                    tpg.CompanyName = isExist.CompanyName;
                    tpg.LastYearBank = isExist.BankUsedLastYear;
                    tpg.LastYearEFIN = isExist.PriorYearEFIN;
                    tpg.LastYearVolume = isExist.PriorYearVolume;
                    tpg.ManagerEmail = isExist.ManagerEmail;
                    tpg.ManagerFirstName = isExist.ManagerFirstName;
                    tpg.ManagerLastName = isExist.ManagerLastName;
                    tpg.OfficeAddress = isExist.OfficeAddress;
                    tpg.OfficeCity = isExist.OfficeCity;
                    tpg.OfficeDAN = isExist.OfficeDAN;
                    tpg.OfficeFax = isExist.OfficeFAX;
                    tpg.OfficeRTN = isExist.OfficeRTN;
                    tpg.OfficeState = isExist.OfficeState;
                    tpg.OfficeTelephone = isExist.OfficeTelephone;
                    tpg.OfficeZip = isExist.OfficeZip;
                    tpg.OwnerAddress = isExist.EFINOwnerAddress;
                    tpg.OwnerCity = isExist.EFINOwnerCity;
                    tpg.OwnerDOB = isExist.EFINOwnerDOB.Value.ToString("MM/dd/yyyy");
                    tpg.OwnerEIN = isExist.EFINOwnerEIN;
                    tpg.OwnerEmail = isExist.EFINOwnerEmail;
                    tpg.OwnerFirstName = isExist.EFINOwnerFirstName;
                    tpg.OwnerLastName = isExist.EFINOwnerLastName;
                    tpg.OwnerSSn = isExist.EFINOwnerSSN;
                    tpg.OwnerState = isExist.EFINOwnerState;
                    tpg.OwnerTelephone = isExist.EFINOwnerTelephone;
                    tpg.OwnerZip = isExist.EFINOwnerZip;
                    tpg.ShippingAddress = isExist.ShippingAddress;
                    tpg.ShippingCity = isExist.ShippingCity;
                    tpg.ShippingState = isExist.ShippingState;
                    tpg.ShippingZip = isExist.ShippingZip;
                    tpg.EfinOwnerTitle = isExist.OwnerTitle;
                    tpg.EfinOwnerMobile = isExist.OwnerMobile;
                    tpg.EfinIDNumber = isExist.OwnerIdNumber;
                    tpg.EfinIdState = isExist.OwnerIdState;

                    tpg.CheckPrint = isExist.CheckPrint;
                    tpg.AgreeBank = isExist.AgreeBank.HasValue ? isExist.AgreeBank.Value : false;
                    tpg.SbfeeAll = isExist.FeeOnAll;
                    tpg.DocPrepFee = isExist.DocPrepFee;
                    tpg.BankName = isExist.BankName;
                    tpg.AccountName = isExist.AccountName;


                    var info = dbArc.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                    if ((info.ParentId ?? Guid.Empty) != Guid.Empty)
                    {
                        IsMainSite = false;
                        var SubSiteOffice = dbArc.SubSiteOfficeConfigs.Where(o => o.RefId == info.Id).FirstOrDefault();
                        if (SubSiteOffice != null)
                        {
                            info.IsMSOUser = SubSiteOffice.SiteanMSOLocation;
                        }
                    }

                    decimal addon = 0, sb = 0;
                    var bankid = dbArc.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                    string addontitle = string.Empty, svbfeetitle = string.Empty;

                    if (bankid != null)
                    {
                        if (bankid.BankId != Guid.Empty)
                        {

                            var addonMasterfees = (from f in db.FeeMasters
                                                   where f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                                   select f.Amount).FirstOrDefault();
                            addon += addonMasterfees.Value;
                            addontitle = addontitle + "Cross Link Transmitter Fee :" + addonMasterfees.Value + ", ";

                            var xmissionAddownfees = dbArc.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 2 && x.BankMaster_ID == bankid.BankId).ToList();

                            if (!IsMainSite)
                            {
                                if ((info.IsMSOUser ?? false) == false)
                                {
                                    decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees);
                                    addon += BankFee;
                                    addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                }
                                else
                                {
                                    decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                    addon += BankFee;
                                    addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                }
                            }

                            addon += bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0;
                            addontitle = addontitle + "Add On Fee :" + (bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0);

                            var SvbAddownfees = dbArc.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 1 && x.BankMaster_ID == bankid.BankId).ToList();

                            var mainfees = (from f in db.FeeMasters
                                            where f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                                            select f.Amount).FirstOrDefault();

                            if (mainfees != null)
                            {
                                sb += mainfees.Value;
                                svbfeetitle = svbfeetitle + "uTax Service Fee :" + mainfees + ", ";
                            }

                            if (!IsMainSite)
                            {
                                if ((info.IsMSOUser ?? false) == false)
                                {
                                    decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees);
                                    sb += BankMaxFees;
                                    svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                }
                                else
                                {
                                    decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                    sb += BankMaxFees;
                                    svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                }

                            }
                            sb += bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0;
                            svbfeetitle = svbfeetitle + "Add On Fee :" + (bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0);
                        }
                    }

                    tpg.AddonfeeTitle = addontitle;
                    tpg.ServiceBureaufeeTitle = svbfeetitle;

                    if (string.IsNullOrEmpty(tpg.Addonfee))
                    {
                        tpg.Addonfee = addon.ToString();
                    }

                    if (string.IsNullOrEmpty(tpg.ServiceBureaufee))
                    {
                        tpg.ServiceBureaufee = sb.ToString();
                    }

                }
                return tpg;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/GetTPGBankEnrollment", CustomerId);
                //GetTPGBankEnrollment
                return null;
            }
        }

        public ArchiveSubSiteDataModel GetArchiveDataSubSite(Guid UserId, Guid ParentId)
        {
            ArchiveSubSiteDataModel ArchiveData = new DTO.ArchiveSubSiteDataModel();

            ArchiveData.SubSiteOfficeDTO = dbArc.SubSiteOfficeConfigs.Where(o => o.RefId == UserId).Select(o => new SubSiteOfficeConfigDTO
            {
                Id = o.Id,
                RefId = o.RefId,
                EFINListedOtherOffice = o.EFINListedOtherOffice,
                SiteOwnthisEFIN = o.SiteOwnthisEFIN,
                EFINOwnerSite = o.EFINOwnerSite,
                SOorSSorEFIN = o.SOorSSorEFIN,
                SubSiteSendTaxReturn = o.SubSiteSendTaxReturn,
                SiteanMSOLocation = o.SiteanMSOLocation,
                IsMainSiteTransmitTaxReturn = o.IsMainSiteTransmitTaxReturn,
                NoofTaxProfessionals = o.NoofTaxProfessionals,
                IsSoftwareOnNetwork = o.IsSoftwareOnNetwork,
                NoofComputers = o.NoofComputers,
                PreferredLanguage = o.PreferredLanguage,
                CanSubSiteLoginToEmp = o.CanSubSiteLoginToEmp ?? false
            }).FirstOrDefault();


            dbArc = new DatabaseArcEntities();
            ArchiveData.SubSiteBankFeesDTOs = dbArc.SubSiteBankFeesConfigs.Where(o => o.emp_CustomerInformation_ID == UserId).Select(o => new SubSiteBankFeesConfigDTO()
            {
                ID = o.ID,
                RefId = o.emp_CustomerInformation_ID,
                BankID = o.BankMaster_ID,
                ServiceorTransmitter = o.ServiceOrTransmitter ?? 0,
                AmountDSK = o.BankMaxFees,
                AmountMSO = o.BankMaxFees_MSO ?? 0,
                QuestionID = o.QuestionID

            }).ToList();



            ArchiveData.SubSiteFeeDTOs = dbArc.SubSiteFeeConfigs.Where(o => o.emp_CustomerInformation_ID == ParentId).Select(o => new SubSiteFeeConfigDTO()
            {
                Id = o.ID,
                refId = o.emp_CustomerInformation_ID,
                IsAddOnFeeCharge = o.IsAddOnFeeCharge,
                IsSameforAll = o.IsSameforAll,
                IsSubSiteAddonFee = o.IsSubSiteAddonFee,
                ServiceorTransmission = o.ServiceorTransmission,
            }).ToList();


            return ArchiveData;
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public ArchiveCustomerModel GetCustomerBySalesYearID(Guid SYGrpId, Guid SalesYearId, bool IsAddEFIN = false)
        {
            dbArc = new DatabaseArcEntities();
            var data = (from e in dbArc.emp_CustomerInformation
                        join cli in dbArc.emp_CustomerLoginInformation on e.Id equals cli.CustomerOfficeId
                        where e.SalesYearGroupId == SYGrpId && e.SalesYearID == SalesYearId
                        select new ArchiveCustomerModel
                        {
                            Id = e.Id,
                            ParentId = e.ParentId ?? Guid.Empty,
                            EntityId = e.EntityId ?? 0,
                            SalesYearGroupId = e.SalesYearGroupId ?? Guid.Empty,
                            // DisplayId = dbArc.EntityMasters.Where(o => o.Id == e.EntityId).Select(o => o.DisplayId).FirstOrDefault(),
                            CompanyName = e.CompanyName,
                            AccountStatus = e.AccountStatus,
                            Feeder = e.Feeder,
                            BusinessOwnerFirstName = e.BusinessOwnerFirstName,
                            OfficePhone = e.OfficePhone,
                            AlternatePhone = e.AlternatePhone,
                            Primaryemail = e.PrimaryEmail,
                            SupportNotificationemail = e.SupportNotificationEmail,
                            EROType = e.EROType,
                            AlternativeContact = e.AlternativeContact,
                            EFIN = e.EFIN,
                            EFINStatus = e.EFINStatus,
                            PhysicalAddress1 = e.PhysicalAddress1,
                            PhysicalAddress2 = e.PhysicalAddress2,
                            PhysicalZipcode = e.PhysicalZipCode,
                            PhysicalCity = e.PhysicalCity,
                            PhysicalState = e.PhysicalState,
                            ShippingAddress1 = e.ShippingAddress1,
                            ShippingAddress2 = e.ShippingAddress2,
                            ShippingZipcode = e.ShippingZipCode,
                            ShippingCity = e.ShippingCity,
                            ShippingState = e.ShippingState,
                            PhoneTypeId = e.PhoneTypeId,
                            TitleId = e.TitleId,
                            IsVerified = e.IsVerified,

                            // PhoneType = dbArc.PhoneTypeMasters.Where(a => a.Id == e.PhoneTypeId).Select(a => a.PhoneType).FirstOrDefault(),
                            // ContactTitle = dbArc.ContactPersonTitleMasters.Where(a => a.Id == e.TitleId).Select(a => a.ContactPersonTitle).FirstOrDefault(),

                            SalesYearID = e.SalesYearID ?? Guid.Empty,
                            SalesforceParentID = e.SalesforceParentID,
                            IsMSOUser = e.IsMSOUser ?? false,

                            LoginId = cli.Id.ToString(),
                            //LoginEFIN = cli.EFIN,
                            MasterIdentifier = cli.MasterIdentifier,
                            CrossLinkUserId = cli.CrossLinkUserId,
                            CrossLinkPassword = cli.CrossLinkPassword,
                            OfficePortalUrl = cli.OfficePortalUrl,
                            TaxOfficeUsername = cli.TaxOfficeUsername,
                            TaxOfficePassword = cli.TaxOfficePassword,
                            CustomerOfficeId = cli.CustomerOfficeId,

                        }).Distinct().FirstOrDefault();


            if (data != null)
            {
                if (IsAddEFIN)
                {
                    var ParentId = dbArc.emp_CustomerInformation.Where(o => o.Id == data.ParentId).Select(o => o.ParentId).FirstOrDefault();

                    if (ParentId != null)
                    {
                        data.ParentId = ParentId ?? Guid.Empty;
                    }
                }
            }


            return data;
        }

        public IQueryable<DropDownDTO> GetAffiliateProgram(int entityid, Guid CustomerId)
        {
            try
            {
                dbArc = new DatabaseArcEntities();
                db = new DatabaseEntities();
                var data = (from affprog in db.AffiliateProgramMasters
                            join affprogent in db.AffiliationProgramEntityMaps on affprog.Id equals affprogent.AffiliateProgramId
                            where (affprogent.EntityId == entityid && affprog.StatusCode != EMPConstants.InActive)
                            select new { affprog, affprogent }).ToList();

                //var P_CustoemrID = dbArc.emp_CustomerInformation.Where(a => a.Id == CustomerId).Select(a => a.ParentId).FirstOrDefault();
                //if (P_CustoemrID == null)
                //{
                //    P_CustoemrID = CustomerId;
                //}

                //if(SubSiteAffiliateProgramConfig)

                List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                EntityHierarchyDTOs = GetEntityHierarchies(CustomerId);
                var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                Guid TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;


                List<DropDownDTO> DropDownDTOlst = new List<DropDownDTO>();
                foreach (var itm in data)
                {
                    DropDownDTO omodel = new DropDownDTO();
                    omodel.Id = itm.affprog.Id;
                    omodel.Name = itm.affprog.Name;
                    omodel.StatusCode = itm.affprog.StatusCode;
                    omodel.Description = "";
                    if (dbArc.SubSiteAffiliateProgramConfigs.Any(a => a.emp_CustomerInformation_ID == TopParentId && a.AffiliateProgramMaster_ID == itm.affprog.Id) || (entityid == (int)EMPConstants.Entity.SO) || (entityid == (int)EMPConstants.Entity.SOME) || (entityid == (int)EMPConstants.Entity.SOME_SS))
                        DropDownDTOlst.Add(omodel);
                }
                return DropDownDTOlst.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetAffiliateProgram", CustomerId);
                return new List<DropDownDTO>().AsQueryable();
            }
        }

        public CustomerPaymentInfoDTO GetCustomerPaymentInfo(Guid UserId, int SiteType, Guid BankId)
        {
            CustomerPaymentInfoDTO info = new CustomerPaymentInfoDTO();
            dbArc = new DatabaseArcEntities();
            db = new DatabaseEntities();

            var custinfo = dbArc.emp_CustomerInformation.Where(x => x.Id == UserId).FirstOrDefault();
            int EntityId = 0;

            if (custinfo != null)
            {
                EntityId = custinfo.EntityId ?? 0;
            }

            try
            {
                bool _isnonebank = false;
                var _banksel = dbArc.EnrollmentBankSelections.Where(x => x.CustomerId == UserId && x.StatusCode == EMPConstants.Active && x.BankId == BankId).FirstOrDefault();
                if (_banksel != null)
                {
                    _isnonebank = _banksel.BankId == Guid.Empty;
                }

                info.IsEnrollment = _isnonebank;

                var IsExist = (from pay in dbArc.CustomerPaymentOptions
                               where pay.CustomerId == UserId && pay.SiteType == SiteType && pay.BankId == BankId
                               select new { pay }).FirstOrDefault();
                if (IsExist != null)
                {
                    info.Id = IsExist.pay.Id.ToString();
                    info.IsSameBankAccount = IsExist.pay.IsSameasBankAccount ?? 0;
                    info.PaymentType = IsExist.pay.PaymentType ?? 0;
                    info.SiteType = IsExist.pay.SiteType ?? 0;
                    if (info.PaymentType == 1)
                    {
                        PaymentCreditCardInfo CardInfo = new PaymentCreditCardInfo();
                        var carddetails = (from card in dbArc.CustomerPaymentViaCreditCards
                                           where card.PaymentOptionId == IsExist.pay.Id
                                           select card).FirstOrDefault();
                        if (carddetails != null)
                        {
                            CardInfo.Address = carddetails.BillingAddress;
                            CardInfo.CardHolderName = carddetails.CardHolderName;
                            CardInfo.CardNumber = carddetails.CardNumber;
                            CardInfo.CardType = carddetails.Cardtype ?? 0;
                            CardInfo.City = carddetails.City;
                            CardInfo.Expiration = carddetails.Expiration;
                            CardInfo.StateCode = dbArc.StateMasters.Where(x => x.StateID == carddetails.State).Select(x => x.StateCode).FirstOrDefault();
                            CardInfo.StateId = carddetails.State ?? 0;
                            CardInfo.status = true;
                            CardInfo.ZipCode = carddetails.ZipCode;
                        }
                        else
                            CardInfo.status = false;
                        info.CreditCard = CardInfo;
                    }
                    else if (info.PaymentType == 2)
                    {
                        PaymetnACH ACH = new PaymetnACH();
                        var ach = (from s in dbArc.CustomerPaymentViaACHes
                                   where s.PaymentOptionId == IsExist.pay.Id
                                   select s).FirstOrDefault();
                        if (ach != null)
                        {
                            ACH.status = true;
                            ACH.AccountName = ach.AccountName;
                            ACH.AccountNumber = ach.AccountNumber;
                            ACH.AccountType = ach.AccountType ?? 0;
                            ACH.BankName = ach.BankName;
                            ACH.RTN = ach.RTN;
                        }
                        else
                            ACH.status = false;
                        info.ACH = ACH;
                    }

                    info.status = true;

                }
                else
                {
                    info.status = false;
                }

                if (SiteType == 1)
                {
                    List<FeeSummary> fess = new List<FeeSummary>();

                    if (custinfo != null)
                    {
                        fess.Add(new FeeSummary { Amount = custinfo.Federal_EF_Fee_New__c ?? 0, Fee = "uTax Federal e-File Fee" });
                        fess.Add(new FeeSummary { Amount = custinfo.State_EF_Fee_New__c ?? 0, Fee = "uTax State e-File Fee" });
                    }
                    info.Fees = fess;
                }
                else if (SiteType == 2)
                {
                    List<FeeSummary> fess = new List<FeeSummary>();
                    // custinfo = dbArc.emp_CustomerInformation.Where(x => x.Id == UserId).FirstOrDefault();
                    if (custinfo != null)
                    {
                        fess.Add(new FeeSummary { Amount = custinfo.pymt__Balance__c ?? 0, Fee = "Cash Saver" });
                        fess.Add(new FeeSummary { Amount = custinfo.Total_Amount_Loaned__c ?? 0, Fee = "LOC Program Participant" });
                        fess.Add(new FeeSummary { Amount = custinfo.A_R_Amount_Due_Credit__c ?? 0, Fee = "A/R Amount Due Credit" });
                    }
                    info.Fees = fess;
                }


                CustomerReimBankDetails bankinfo = new CustomerReimBankDetails();
                var bankdetails = (from bank in dbArc.EnrollmentFeeReimbursementConfigs
                                   where bank.emp_CustomerInformation_ID == UserId && bank.StatusCode == EMPConstants.Active && bank.BankId == BankId
                                   select bank).FirstOrDefault();
                if (bankdetails != null)
                {
                    bankinfo.BankName = bankdetails.BankName;
                    bankinfo.Status = bankdetails.StatusCode;
                    bankinfo.Availble = true;
                }
                else
                {
                    var mobankdetails = (from bank in dbArc.FeeReimbursementConfigs
                                         where bank.emp_CustomerInformation_ID == UserId && bank.StatusCode == EMPConstants.Active
                                         select bank).FirstOrDefault();
                    if (mobankdetails != null)
                    {
                        bankinfo.BankName = mobankdetails.BankName;
                        bankinfo.Status = mobankdetails.StatusCode;
                        bankinfo.Availble = true;
                    }
                    else
                    {
                        bankinfo.Availble = false;
                        bankinfo.BankName = "";
                        bankinfo.Status = "";
                    }
                }

                info.BankDetails = bankinfo;
                Guid feereium = new Guid("60025459-7568-4a77-b152-f81904aaaa63"); // Main office fee-reimbursement screen
                Guid ssfeereium = new Guid("a55334d1-3960-44c4-8cf1-e3ba9901f2be"); // Enrollment fee-reimbursement screen
                var issaved = dbArc.CustomerConfigurationStatus.Where(x => x.CustomerId == UserId && x.StatusCode == EMPConstants.Done && x.SitemapId == feereium).FirstOrDefault();
                info.IsFeeReimbursement = (issaved == null && bankdetails == null) ? false : true;
                if ((EntityId == (int)EMPConstants.Entity.SO || EntityId == (int)EMPConstants.Entity.SOME) && info.IsFeeReimbursement)
                {
                    var banksel = dbArc.EnrollmentBankSelections.Where(x => x.CustomerId == UserId && x.StatusCode == EMPConstants.Active && x.BankId == BankId).FirstOrDefault();
                    if (banksel != null)
                    {
                        if (!(banksel.IsServiceBureauFee ?? false) && !(banksel.IsTransmissionFee ?? false))
                            info.IsFeeReimbursement = false;
                    }
                    else
                        info.IsFeeReimbursement = false;
                }

                if (!info.IsFeeReimbursement)
                {
                    var bankdetails1 = (from bank in dbArc.EnrollmentFeeReimbursementConfigs
                                        where bank.emp_CustomerInformation_ID == UserId && bank.StatusCode == EMPConstants.Active
                                        select bank).FirstOrDefault();
                    if (bankdetails1 != null)
                    {
                        info.IsFeeReimbursement = true;
                    }
                }

            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/GetCustomerPaymentInfo", UserId);
                info.status = false;
            }

            return info;
        }

        public CustomerBanksResponseDTO getCutomerBanks(Guid CustomerId)
        {
            db = new DatabaseEntities();
            dbArc = new DatabaseArcEntities();
            CustomerBanksResponseDTO res = new CustomerBanksResponseDTO();
            try
            {
                List<string> bankstatus = new List<string>();
                bankstatus.Add(EMPConstants.Submitted);
                bankstatus.Add(EMPConstants.EnrPending);
                bankstatus.Add(EMPConstants.Approved);
                bankstatus.Add(EMPConstants.Rejected);
                bankstatus.Add(EMPConstants.Denied);
                bankstatus.Add(EMPConstants.Cancelled);

                var dbBankMaster = db.BankMasters.AsQueryable();
                List<CustomerBanksDTO> banks = new List<CustomerBanksDTO>();

                var enrollments = (from s in dbArc.BankEnrollments
                                       //join b in dbBankMaster on s.BankId equals b.Id
                                   join bs in dbArc.EnrollmentBankSelections on s.BankId equals bs.BankId
                                   where s.CustomerId == CustomerId && s.IsActive == true && bankstatus.Contains(s.StatusCode) && s.ArchiveStatusCode != EMPConstants.Archive
                                   && bs.StatusCode == EMPConstants.Active && bs.CustomerId == CustomerId //&& b.StatusCode == EMPConstants.Active 
                                   select new CustomerBanksDTO
                                   {
                                       // BankName = "",// b.BankName,
                                       BankId = bs.BankId,
                                       EnrollId = s.Id,
                                       Default = (bs.BankSubmissionStatus ?? 0),
                                       BankStatus = s.StatusCode
                                   }).ToList();

                foreach (var bank in enrollments)
                {
                    string submissionstatus = "Submitted", approvedstatus = "";

                    bank.IsSubmitted = true;
                    var date = dbArc.BankEnrollmentStatus.Where(x => x.EnrollmentId == bank.EnrollId && x.Status == EMPConstants.SubmittedService).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    if (date != null)
                        submissionstatus = "Submitted - " + date.CreatedDate.Value.ToString("dd MMM yyyy");
                    else
                        submissionstatus = "Submitted";

                    bank.IsApproved = bank.BankStatus == EMPConstants.Approved ? true : false;
                    if (bank.IsApproved)
                    {
                        var date1 = dbArc.BankEnrollmentStatus.Where(x => x.EnrollmentId == bank.EnrollId && x.Status == EMPConstants.ApprovedService).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                        if (date1 != null)
                            approvedstatus = "Approved - " + date1.CreatedDate.Value.ToString("dd MMM yyyy");
                        else
                            approvedstatus = "Approved";
                    }
                    else if (bank.BankStatus == EMPConstants.Rejected || bank.BankStatus == EMPConstants.Denied || bank.BankStatus == EMPConstants.Cancelled)
                    {
                        var date1 = dbArc.BankEnrollmentStatus.Where(x => x.EnrollmentId == bank.EnrollId && (x.Status == EMPConstants.RejectedService || x.Status == EMPConstants.DeniedServcce || x.Status == EMPConstants.CancelledService)).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                        if (date1 != null)
                            approvedstatus = "Rejected - " + date1.CreatedDate.Value.ToString("dd MMM yyyy");
                        else
                            approvedstatus = "Rejected";
                    }

                    bank.Acceptance = approvedstatus;
                    bank.BankStatus = bank.Default == 1 ? "Default" : "";
                    bank.Submission = submissionstatus;

                    var bankmaster = db.BankMasters.Where(o => o.Id == bank.BankId && o.StatusCode == EMPConstants.Active).FirstOrDefault();
                    if (bankmaster != null)
                    {
                        bank.BankName = bankmaster.BankName;
                    }
                }

                res.Banks = enrollments;
                if (enrollments.Count > 1)
                {
                    res.Status = true;
                }
                else
                {
                    res.Status = false;
                }

            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "BankSelectionService/getCutomerBanks", CustomerId);
                res.Status = false;
            }
            return res;
        }



        //public PaymetnACH GetCustomerBankDetails(Guid UserId, int EntityId, Guid CustId, Guid BankId)
        //{
        //    PaymetnACH info = new PaymetnACH();
        //    try
        //    {
        //        var IsExist = (from bank in db.EnrollmentFeeReimbursementConfigs
        //                       where bank.emp_CustomerInformation_ID == CustId && bank.StatusCode == EMPConstants.Active && bank.BankId == BankId
        //                       select bank).FirstOrDefault();
        //        if (IsExist != null)
        //        {
        //            info.AccountName = IsExist.AccountName;
        //            info.AccountNumber = IsExist.BankAccountNo;
        //            info.AccountType = IsExist.AccountType;
        //            info.BankName = IsExist.BankName;
        //            info.RTN = IsExist.RTN;
        //            info.status = true;
        //        }
        //        else
        //        {
        //            var moIsExist = (from bank in db.FeeReimbursementConfigs
        //                             where bank.emp_CustomerInformation_ID == CustId && bank.StatusCode == EMPConstants.Active
        //                             select bank).FirstOrDefault();
        //            if (moIsExist != null)
        //            {
        //                info.AccountName = moIsExist.AccountName;
        //                info.AccountNumber = moIsExist.BankAccountNo;
        //                info.AccountType = moIsExist.AccountType;
        //                info.BankName = moIsExist.BankName;
        //                info.RTN = moIsExist.RTN;
        //                info.status = true;
        //            }
        //            else
        //                info.status = false;
        //        }
        //        if (!info.status)
        //        {
        //            var IsExist1 = (from bank in db.EnrollmentFeeReimbursementConfigs
        //                            where bank.emp_CustomerInformation_ID == CustId && bank.StatusCode == EMPConstants.Active
        //                            select bank).FirstOrDefault();
        //            if (IsExist1 != null)
        //            {
        //                info.AccountName = IsExist1.AccountName;
        //                info.AccountNumber = IsExist1.BankAccountNo;
        //                info.AccountType = IsExist1.AccountType;
        //                info.BankName = IsExist1.BankName;
        //                info.RTN = IsExist1.RTN;
        //                info.status = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/GetCustomerBankDetails", UserId);
        //        info.status = false;
        //    }
        //    return info;
        //}


        /// <summary>
        /// This Method is used to Get the Bank Fee Config For Main And Sub Site
        /// </summary>
        /// <returns></returns>
        public IQueryable<BankQuestionDTO> GetSubSiteBankAndQuestions(Guid strguid)
        {
            try
            {
                db = new DatabaseEntities();
                dbArc = new DatabaseArcEntities();

                List<SubSiteBankFeesDTO> lstsbfee = new List<SubSiteBankFeesDTO>();

                List<BankQuestionDTO> BankLst = new List<BankQuestionDTO>();

                List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                EntityHierarchyDTOs = GetEntityHierarchies(strguid);

                Guid TopParentId = Guid.Empty;
                Guid FeeSourceParentId = Guid.Empty;

                if (EntityHierarchyDTOs.Count > 0)
                {
                    var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                    TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;

                    var FeeSource = EntityHierarchyDTOs.Where(o => o.EntityId == o.FeeSourceEntityId).FirstOrDefault();
                    if (FeeSource != null)
                    {
                        FeeSourceParentId = FeeSource.CustomerId ?? Guid.Empty;
                    }
                    else
                    {
                        FeeSourceParentId = TopParentId;
                    }
                }

                if (EntityHierarchyDTOs.Count <= 1)
                {
                    if (EntityHierarchyDTOs.Where(o => o.EntityId == (int)EMPConstants.Entity.SO || o.EntityId == (int)EMPConstants.Entity.SOME).Any())
                    {
                        return GetSubSiteBankAndQuestions_Level0();
                    }
                }

                if (EntityHierarchyDTOs.Count > 1)
                {
                    if (EntityHierarchyDTOs.Where(o => o.EntityId == (int)EMPConstants.Entity.SOME_SS).Any())
                    {
                        return GetSubSiteBankAndQuestions_LevelAE(strguid);
                    }
                }

                var SubSiteBankConfigs = dbArc.SubSiteBankConfigs.Where(o => o.emp_CustomerInformation_ID == TopParentId).ToList();
                var BankMasters = db.BankMasters.Where(o => o.StatusCode != EMPConstants.InActive).ToList();


                var items = SubSiteBankConfigs
                       .Join(BankMasters, ssbc => ssbc.BankMaster_ID, bank => bank.Id,
                               (ssbc, bank) => new { ssbc, bank })
                .Where(o => o.ssbc.emp_CustomerInformation_ID == TopParentId && o.bank.StatusCode != EMPConstants.InActive)
                .GroupBy(x => new
                {
                    x.ssbc.BankMaster_ID,
                    x.bank.BankName,
                    x.bank.StatusCode,
                    x.bank.MaxFeeLimitDeskTop,
                    x.bank.MaxFeeLimitMSO,
                    x.bank.MaxTranFeeDeskTop,
                    x.bank.MaxTranFeeMSO,
                    x.bank.BankProductDocument
                })
                .Select(g => new
                {
                    g.Key.BankMaster_ID,
                    g.Key.BankName,
                    g.Key.StatusCode,
                    g.Key.MaxFeeLimitDeskTop,
                    g.Key.MaxFeeLimitMSO,
                    g.Key.MaxTranFeeDeskTop,
                    g.Key.MaxTranFeeMSO,
                    g.Key.BankProductDocument
                }).ToList();

                var mainbankfeeconfig = dbArc.SubSiteBankFeesConfigs.Where(o => o.emp_CustomerInformation_ID == FeeSourceParentId).ToList();


                foreach (var itm in items)
                {

                    BankQuestionDTO bankmodel = new BankQuestionDTO();
                    bankmodel.BankId = itm.BankMaster_ID;
                    bankmodel.BankName = itm.BankName;
                    bankmodel.Questions = db.BankSubQuestions.Where(o => o.BankId == itm.BankMaster_ID && o.StatusCode == EMPConstants.Active).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions }).ToList();
                    bankmodel.StatusCode = itm.StatusCode;

                    var SVBFeeData = mainbankfeeconfig.Where(o => o.BankMaster_ID == itm.BankMaster_ID && o.ServiceOrTransmitter == 1).FirstOrDefault();
                    if (SVBFeeData != null)
                    {
                        bankmodel.DesktopFee = SVBFeeData.BankMaxFees;// itm.BankMaxFees;
                        bankmodel.MSOFee = SVBFeeData.BankMaxFees_MSO ?? 0;
                    }
                    else
                    {
                        bankmodel.DesktopFee = 0;
                        bankmodel.MSOFee = 0;
                    }


                    bankmodel.BankSVBDesktopFee = itm.MaxFeeLimitDeskTop ?? 0;
                    bankmodel.BankSVBMSOFee = itm.MaxFeeLimitMSO ?? 0;

                    bankmodel.BankTranDesktopFee = itm.MaxTranFeeDeskTop ?? 0;
                    bankmodel.BankTranMSOFee = itm.MaxTranFeeMSO ?? 0;

                    bankmodel.ServiceorTransmission = 1;
                    bankmodel.DocumentPath = "";// GetDocumentPath(itm.BankProductDocument).FilePath;

                    BankLst.Add(bankmodel);


                    bankmodel = new BankQuestionDTO();
                    bankmodel.BankId = itm.BankMaster_ID;
                    bankmodel.BankName = itm.BankName;
                    bankmodel.Questions = db.BankSubQuestions.Where(o => o.BankId == itm.BankMaster_ID && o.StatusCode == EMPConstants.Active).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions }).ToList();
                    bankmodel.StatusCode = itm.StatusCode;

                    var TranFeeData = mainbankfeeconfig.Where(o => o.BankMaster_ID == itm.BankMaster_ID && o.ServiceOrTransmitter == 2).FirstOrDefault();
                    if (TranFeeData != null)
                    {
                        bankmodel.DesktopFee = TranFeeData.BankMaxFees;// itm.BankMaxFees;
                        bankmodel.MSOFee = TranFeeData.BankMaxFees_MSO ?? 0;
                    }
                    else
                    {
                        bankmodel.DesktopFee = 0;
                        bankmodel.MSOFee = 0;
                    }


                    bankmodel.BankSVBDesktopFee = itm.MaxFeeLimitDeskTop ?? 0;
                    bankmodel.BankSVBMSOFee = itm.MaxFeeLimitMSO ?? 0;

                    bankmodel.BankTranDesktopFee = itm.MaxTranFeeDeskTop ?? 0;
                    bankmodel.BankTranMSOFee = itm.MaxTranFeeMSO ?? 0;

                    bankmodel.ServiceorTransmission = 2;
                    bankmodel.DocumentPath = "";// GetDocumentPath(itm.BankProductDocument).FilePath;

                    BankLst.Add(bankmodel);
                }

                return BankLst.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetSubSiteBankAndQuestions", strguid);
                return new List<BankQuestionDTO>().AsQueryable();
            }
        }

        /// <summary>
        /// This Method is used to Get the Bank Fee Config For SO,SOME and Level=0
        /// </summary>
        /// <returns></returns>
        public IQueryable<BankQuestionDTO> GetSubSiteBankAndQuestions_Level0()
        {
            try
            {
                List<BankQuestionDTO> BankLst = new List<BankQuestionDTO>();

                db = new DatabaseEntities();
                var items = db.BankMasters
                .Where(o => o.StatusCode != EMPConstants.InActive)
                .GroupBy(x => new
                {
                    x.Id,
                    x.BankName,
                    x.StatusCode,
                    x.MaxFeeLimitDeskTop,
                    x.MaxFeeLimitMSO,
                    x.MaxTranFeeDeskTop,
                    x.MaxTranFeeMSO,
                    x.BankProductDocument
                })
                .Select(g => new
                {
                    g.Key.Id,
                    g.Key.BankName,
                    g.Key.StatusCode,
                    g.Key.MaxFeeLimitDeskTop,
                    g.Key.MaxFeeLimitMSO,
                    g.Key.MaxTranFeeDeskTop,
                    g.Key.MaxTranFeeMSO,
                    g.Key.BankProductDocument
                }).ToList();

                // var mainbankfeeconfig = db.SubSiteBankFeesConfigs.Where(o => o.emp_CustomerInformation_ID == FeeSourceParentId).ToList();
                List<int> FeeFor = new List<int>();
                FeeFor.Add((int)EMPConstants.FeesFor.SVBFees);

                var mainbankfeeconfig = db.FeeMasters.Where(o => FeeFor.Contains(o.FeesFor ?? 0));
                List<SubSiteBankFeesDTO> lstsbfee = new List<SubSiteBankFeesDTO>();


                foreach (var itm in items)
                {

                    BankQuestionDTO bankmodel = new BankQuestionDTO();
                    bankmodel.BankId = itm.Id;
                    bankmodel.BankName = itm.BankName;
                    bankmodel.Questions = db.BankSubQuestions.Where(o => o.BankId == itm.Id && o.StatusCode == EMPConstants.Active).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions }).ToList();
                    bankmodel.StatusCode = itm.StatusCode;

                    var SVBFeeData = mainbankfeeconfig.Where(o => o.FeesFor == (int)EMPConstants.FeesFor.SVBFees).FirstOrDefault();
                    if (SVBFeeData != null)
                    {
                        bankmodel.DesktopFee = SVBFeeData.Amount ?? 0;// itm.BankMaxFees;
                        bankmodel.MSOFee = 0;// SVBFeeData.BankMaxFees_MSO ?? 0;
                    }
                    else
                    {
                        bankmodel.DesktopFee = 0;
                        bankmodel.MSOFee = 0;
                    }


                    bankmodel.BankSVBDesktopFee = itm.MaxFeeLimitDeskTop ?? 0;
                    bankmodel.BankSVBMSOFee = itm.MaxFeeLimitMSO ?? 0;

                    bankmodel.BankTranDesktopFee = itm.MaxTranFeeDeskTop ?? 0;
                    bankmodel.BankTranMSOFee = itm.MaxTranFeeMSO ?? 0;

                    bankmodel.ServiceorTransmission = 1;
                    bankmodel.DocumentPath = "";// GetDocumentPath(itm.BankProductDocument).FilePath;

                    BankLst.Add(bankmodel);


                    bankmodel = new BankQuestionDTO();
                    bankmodel.BankId = itm.Id;
                    bankmodel.BankName = itm.BankName;
                    bankmodel.Questions = db.BankSubQuestions.Where(o => o.BankId == itm.Id && o.StatusCode == EMPConstants.Active).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions }).ToList();
                    bankmodel.StatusCode = itm.StatusCode;

                    var TranFeeData = mainbankfeeconfig.Where(o => o.FeesFor == (int)EMPConstants.FeesFor.TransmissionFees).FirstOrDefault();
                    if (TranFeeData != null)
                    {
                        bankmodel.DesktopFee = TranFeeData.Amount ?? 0;// itm.BankMaxFees;
                        bankmodel.MSOFee = 0;// TranFeeData.BankMaxFees_MSO ?? 0;
                    }
                    else
                    {
                        bankmodel.DesktopFee = 0;
                        bankmodel.MSOFee = 0;
                    }

                    bankmodel.BankSVBDesktopFee = itm.MaxFeeLimitDeskTop ?? 0;
                    bankmodel.BankSVBMSOFee = itm.MaxFeeLimitMSO ?? 0;

                    bankmodel.BankTranDesktopFee = itm.MaxTranFeeDeskTop ?? 0;
                    bankmodel.BankTranMSOFee = itm.MaxTranFeeMSO ?? 0;

                    bankmodel.ServiceorTransmission = 2;
                    bankmodel.DocumentPath = "";// GetDocumentPath(itm.BankProductDocument).FilePath;

                    BankLst.Add(bankmodel);
                }

                return BankLst.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetSubSiteBankAndQuestions", null);
                return new List<BankQuestionDTO>().AsQueryable();
            }
        }

        public IQueryable<BankQuestionDTO> GetSubSiteBankAndQuestions_LevelAE(Guid MyId)
        {
            try
            {
                List<BankQuestionDTO> BankLst = new List<BankQuestionDTO>();

                dbArc = new DatabaseArcEntities();
                db = new DatabaseEntities();

                var SubSiteBankFeesConfigs = db.SubSiteBankFeesConfigs.Where(o => o.emp_CustomerInformation_ID == MyId).ToList();
                var BankMasters = db.BankMasters.Where(o => o.StatusCode != EMPConstants.InActive).ToList();

                var items = SubSiteBankFeesConfigs
                       .Join(BankMasters, ssbc => ssbc.BankMaster_ID, bank => bank.Id,
                               (ssbc, bank) => new { ssbc, bank })
                .Where(o => o.ssbc.emp_CustomerInformation_ID == MyId && o.bank.StatusCode != EMPConstants.InActive).Select(x => new
                {
                    x.ssbc.BankMaster_ID,
                    x.bank.BankName,
                    x.bank.StatusCode,
                    x.bank.MaxFeeLimitDeskTop,
                    x.bank.MaxFeeLimitMSO,
                    x.bank.MaxTranFeeDeskTop,
                    x.bank.MaxTranFeeMSO,
                    x.bank.BankProductDocument,
                    x.ssbc.BankMaxFees,
                    x.ssbc.BankMaxFees_MSO,
                    x.ssbc.ServiceOrTransmitter
                }).ToList();

                List<int> FeeFor = new List<int>();
                FeeFor.Add((int)EMPConstants.FeesFor.SVBFees);

                var mainbankfeeconfig = db.FeeMasters.Where(o => FeeFor.Contains(o.FeesFor ?? 0));
                List<SubSiteBankFeesDTO> lstsbfee = new List<SubSiteBankFeesDTO>();


                foreach (var itm in items)
                {

                    BankQuestionDTO bankmodel = new BankQuestionDTO();
                    bankmodel.BankId = itm.BankMaster_ID;
                    bankmodel.BankName = itm.BankName;
                    bankmodel.Questions = db.BankSubQuestions.Where(o => o.BankId == itm.BankMaster_ID && o.StatusCode == EMPConstants.Active).Select(o => new DropDownDTO { Id = o.Id, Name = o.Questions }).ToList();
                    bankmodel.StatusCode = itm.StatusCode;
                    bankmodel.DesktopFee = itm.BankMaxFees;
                    bankmodel.MSOFee = itm.BankMaxFees_MSO ?? 0;

                    bankmodel.BankSVBDesktopFee = itm.MaxFeeLimitDeskTop ?? 0;
                    bankmodel.BankSVBMSOFee = itm.MaxFeeLimitMSO ?? 0;

                    var SVBFeeData = mainbankfeeconfig.Where(o => o.FeesFor == (int)EMPConstants.FeesFor.SVBFees).FirstOrDefault();
                    if (SVBFeeData != null)
                    {
                        bankmodel.BankSVBDesktopFee = bankmodel.BankSVBDesktopFee - SVBFeeData.Amount ?? 0;
                        bankmodel.BankSVBMSOFee = bankmodel.BankSVBMSOFee - SVBFeeData.Amount ?? 0;
                    }

                    bankmodel.BankTranDesktopFee = itm.MaxTranFeeDeskTop ?? 0;
                    bankmodel.BankTranMSOFee = itm.MaxTranFeeMSO ?? 0;

                    bankmodel.ServiceorTransmission = itm.ServiceOrTransmitter ?? 0;
                    bankmodel.DocumentPath = "";// GetDocumentPath(itm.BankProductDocument).FilePath;
                    BankLst.Add(bankmodel);
                }

                return BankLst.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetSubSiteBankAndQuestions", null);
                return new List<BankQuestionDTO>().AsQueryable();
            }
        }

        public List<EntityHierarchyDTO> GetEntityHierarchies(Guid UserId)
        {
            try
            {
                int FeeSourceEntityId = 0;
                dbArc = new DatabaseArcEntities();
                List<EntityHierarchyDTO> Hierarchies = new List<EntityHierarchyDTO>();
                var items = (from emp in dbArc.emp_CustomerInformation
                             join ent in dbArc.EntityMasters on emp.EntityId equals ent.Id
                             join enthie in dbArc.EntityHierarchies on emp.Id equals enthie.CustomerId
                             where enthie.RelationId == UserId
                             select new
                             {
                                 Id = enthie.Id,
                                 RelationId = enthie.RelationId,
                                 Customer_Level = enthie.Customer_Level,
                                 CustomerId = enthie.CustomerId,
                                 EntityId = enthie.EntityId,
                                 Status = enthie.Status,
                                 FeeSourceEntityId = ent.FeeSourceEntityId
                             }).ToList();

                foreach (var item in items)
                {
                    EntityHierarchyDTO Hierarchy = new EntityHierarchyDTO();
                    Hierarchy.Id = item.Id;
                    Hierarchy.RelationId = item.RelationId;
                    Hierarchy.Customer_Level = item.Customer_Level;
                    Hierarchy.CustomerId = item.CustomerId;
                    Hierarchy.EntityId = item.EntityId;
                    Hierarchy.Status = item.Status;
                    if (Hierarchy.Customer_Level == 0)
                    {
                        FeeSourceEntityId = item.FeeSourceEntityId ?? 0;
                    }
                    Hierarchy.FeeSourceEntityId = FeeSourceEntityId;

                    Hierarchies.Add(Hierarchy);
                }

                return Hierarchies;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "DropDownService/GetCustomerActiveStatus", UserId);
                return null;
            }
        }

        public List<BankFeeDTO> getBankFee(Guid CustomerId)
        {
            try
            {
                var customer = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                if (customer != null)
                {
                    List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                    EntityHierarchyDTOs = GetEntityHierarchies(CustomerId);
                    var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                    Guid TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;

                    bool isMso = false;
                    if (CustomerId != TopParentId)
                    {
                        var parentcustomer = db.emp_CustomerInformation.Where(x => x.Id == TopParentId).FirstOrDefault();
                        isMso = parentcustomer.IsMSOUser ?? false;
                    }
                    else
                    {
                        return new List<BankFeeDTO>();
                    }

                    if (isMso)
                    {
                        var bankfees = (from s in db.SubSiteBankFeesConfigs
                                        where s.emp_CustomerInformation_ID == CustomerId //&& s.BankMaster_ID == BankId
                                        group s by s.BankMaster_ID into g
                                        select new
                                        {
                                            BankId = g.Key.ToString(),
                                            SvbAmount = g.Where(x => x.ServiceOrTransmitter == 1).Select(x => x.BankMaxFees_MSO).FirstOrDefault(),
                                            TransAmount = g.Where(x => x.ServiceOrTransmitter == 2).Select(x => x.BankMaxFees_MSO).FirstOrDefault()
                                        }).ToList();

                        var list = (from s in bankfees
                                    select new BankFeeDTO
                                    {
                                        BankId = s.BankId,
                                        SvbAmount = s.SvbAmount ?? 0,
                                        TransAmount = s.TransAmount ?? 0
                                    }).ToList();
                        return list;
                    }
                    else
                    {
                        var bankfees = (from s in db.SubSiteBankFeesConfigs
                                        where s.emp_CustomerInformation_ID == CustomerId //&& s.BankMaster_ID == BankId
                                        group s by s.BankMaster_ID into g
                                        select new BankFeeDTO
                                        {
                                            BankId = g.Key.ToString(),
                                            SvbAmount = g.Where(x => x.ServiceOrTransmitter == 1).Select(x => x.BankMaxFees).FirstOrDefault(),
                                            TransAmount = g.Where(x => x.ServiceOrTransmitter == 2).Select(x => x.BankMaxFees).FirstOrDefault()
                                        }).ToList();
                        return bankfees;
                    }
                }
                else
                    return new List<BankFeeDTO>();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/getBankFee", CustomerId);
                return new List<BankFeeDTO>();
            }
        }

        public BankEnrollmentStatusInfoDTO getBankEnrollmentStatus(Guid CustomerId, Guid bankid)
        {
            BankEnrollmentStatusInfoDTO status = new DTO.BankEnrollmentStatusInfoDTO();
            try
            {
                var isexist = db.BankEnrollments.Where(x => x.CustomerId == CustomerId && x.BankId == bankid && x.IsActive == true).FirstOrDefault();
                if (isexist != null)
                {
                    status.SubmissionStaus = isexist.StatusCode;
                    status.BankId = isexist.BankId.Value.ToString();
                }
                else
                {
                    status.SubmissionStaus = "";
                }

                status.SubmissionCount = db.BankEnrollments.Where(x => x.CustomerId == CustomerId && (x.StatusCode == EMPConstants.Submitted || x.StatusCode == EMPConstants.EnrPending) && x.IsActive == true).Count();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/getBankEnrollmentStatus", CustomerId);
                status.SubmissionStaus = "";
            }
            return status;
        }

        public CustomerInformationDTO GetCustomerInformation(Guid id, int CustomerLevel = 0)
        {
            try
            {
                dbArc = new DatabaseArcEntities();
                db = new DatabaseEntities();

                if (CustomerLevel == 1)
                {
                    List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                    EntityHierarchyDTOs = GetEntityHierarchies(id);

                    Guid ParentId = Guid.Empty;
                    int Level = EntityHierarchyDTOs.Count;
                    if (EntityHierarchyDTOs.Count > 0)
                    {
                        var LevelOne = EntityHierarchyDTOs.Where(o => o.Customer_Level == 1).FirstOrDefault();
                        if (LevelOne != null)
                        {
                            ParentId = LevelOne.CustomerId ?? Guid.Empty;
                            id = ParentId;
                        }
                        else
                        {
                            return new CustomerInformationDTO();
                        }
                    }
                }

                //  var PhoneTypeMasters = db.PhoneTypeMasters.ToList();
                //  var ContactPersonTitleMasters = db.ContactPersonTitleMasters.ToList();

                var customerInformation = dbArc.emp_CustomerInformation.Where(e => e.Id == id).Select(e => new CustomerInformationDTO
                {
                    Id = e.Id,
                    CompanyName = e.CompanyName != null ? e.CompanyName.ToString() : "",
                    AccountStatus = e.AccountStatus != null ? e.AccountStatus.ToString() : "",
                    Feeder = e.Feeder,
                    BusinessOwnerFirstName = e.BusinessOwnerFirstName != null ? e.BusinessOwnerFirstName.ToString() : "",
                    BusinessOwnerLastName = e.BusinesOwnerLastName != null ? e.BusinesOwnerLastName.ToString() : "",
                    OfficePhone = e.OfficePhone != null ? e.OfficePhone.ToString() : "",
                    AlternatePhone = e.AlternatePhone != null ? e.AlternatePhone.ToString() : "",
                    Primaryemail = e.PrimaryEmail != null ? e.PrimaryEmail.ToString() : "",
                    SupportNotificationemail = e.SupportNotificationEmail != null ? e.SupportNotificationEmail.ToString() : "",
                    EROType = e.EROType != null ? e.EROType.ToString() : "",
                    AlternativeContact = e.AlternativeContact != null ? e.AlternativeContact.ToString() : "",
                    //11212016
                    EFIN = e.EFIN ?? 0,
                    EFINStatus = e.EFINStatus,
                    PhysicalAddress1 = e.PhysicalAddress1 != null ? e.PhysicalAddress1.ToString() : "",
                    PhysicalAddress2 = e.PhysicalAddress2 != null ? e.PhysicalAddress2.ToString() : "",
                    PhysicalZipcode = e.PhysicalZipCode != null ? e.PhysicalZipCode.ToString() : "",
                    PhysicalCity = e.PhysicalCity != null ? e.PhysicalCity.ToString() : "",
                    PhysicalState = e.PhysicalState != null ? e.PhysicalState.ToString() : "",
                    ShippingAddress1 = e.ShippingAddress1 != null ? e.ShippingAddress1.ToString() : "",
                    ShippingAddress2 = e.ShippingAddress2 != null ? e.ShippingAddress2.ToString() : "",
                    ShippingZipcode = e.ShippingZipCode != null ? e.ShippingZipCode.ToString() : "",
                    ShippingCity = e.ShippingCity != null ? e.ShippingCity.ToString() : "",
                    ShippingState = e.ShippingState != null ? e.ShippingState.ToString() : "",
                    PhoneTypeId = e.PhoneTypeId,
                    ParentId = e.ParentId,
                    TitleId = e.TitleId,
                    AlternativeType = e.AlternativeType,
                    // PhoneType = PhoneTypeMasters.Where(a => a.Id == e.PhoneTypeId).Select(a => a.PhoneType != null ? a.PhoneType.ToString() : "").FirstOrDefault(),
                    //  ContactTitle = ContactPersonTitleMasters.Where(a => a.Id == e.TitleId).Select(a => a.ContactPersonTitle != null ? a.ContactPersonTitle.ToString() : "").FirstOrDefault(),
                    EntityId = e.EntityId ?? 0,
                    SalesYearID = e.SalesYearID ?? Guid.Empty,
                    SalesforceParentID = e.SalesforceParentID != null ? e.SalesforceParentID.ToString() : "",
                    MasterIdentifier = e.MasterIdentifier != null ? e.MasterIdentifier.ToString() : "",
                    IsVerified = e.IsVerified ?? false,
                    IsMSOUser = e.IsMSOUser ?? false,
                    IsActivationCompleted = e.IsActivationCompleted ?? 0,
                    StatusCode = e.StatusCode.ToString(),
                    IsNotCollectingFee = e.uTaxNotCollectingSBFee ?? false,
                    BaseEntityId = e.EntityMaster.BaseEntityId ?? 0,
                    IsEnrollmentSubmit = e.IsEnrolled ?? false

                }).Distinct().FirstOrDefault();

                if (customerInformation != null)
                {
                    int EFIN = customerInformation.EFIN ?? 0;
                    string EFINText = EFIN.ToString().PadLeft(6, '0');
                    //customerInformation.EFIN = Convert.ToInt32(EFINText);

                    // customerInformation.IsEnrollmentSubmit = IsEnrollmentSubmit(id);

                    //if (EntityHierarchyDTOs.Count > 1)
                    //{
                    //    customerInformation.IsMSOUser = dbArc.emp_CustomerInformation.Where(o => o.Id == ParentId).Select(o => o.IsMSOUser).FirstOrDefault() ?? false;
                    //}

                    if (customerInformation.EFINStatus == 16 || customerInformation.EFINStatus == 19)
                    {
                        customerInformation.EFINStatusText = EFINText;
                    }
                    else if (customerInformation.EFINStatus == 21)
                    {
                        customerInformation.EFINStatusText = (EFIN > 0) ? EFINText + "<u><b>S</b></u>".ToString() : "Sharing";
                    }
                    else if (customerInformation.EFINStatus == 17 || customerInformation.EFINStatus == 20)
                    {
                        customerInformation.EFINStatusText = "Applied";
                    }
                    else if (customerInformation.EFINStatus == 18)
                    {
                        customerInformation.EFINStatusText = "Not Required";
                    }
                    else
                    {
                        customerInformation.EFINStatusText = EFINText;
                    }
                }

                return customerInformation;
            }
            catch (Exception ex)
            {
                //ExceptionLogger.LogException(ex.ToString(), "CustomerInformation/GetCustomerInformation", id);
                return null;
            }
        }

        public CustomerLoginInformationDTO GetCustomerLoginInformation(Guid id)
        {
            try
            {
                db = new DatabaseEntities();
                dbArc = new DatabaseArcEntities();

                CustomerLoginInformationDTO model = new CustomerLoginInformationDTO();
                var itm = (from e in dbArc.emp_CustomerLoginInformation
                           where e.CustomerOfficeId == id
                           select new { e }).FirstOrDefault();

                if (itm != null)
                {
                    model.CLAccountId = itm.e.CLAccountId;
                    model.CLLogin = itm.e.CLLogin;
                    model.CLAccountPassword = itm.e.CLAccountPassword;
                    model.Id = itm.e.Id.ToString();
                    model.MasterIdentifier = itm.e.MasterIdentifier != null ? itm.e.MasterIdentifier.ToString() : "";
                    model.CrossLinkUserId = itm.e.CrossLinkUserId != null ? itm.e.CrossLinkUserId.ToString() : "";
                    model.CrossLinkPassword = !string.IsNullOrEmpty(itm.e.CrossLinkPassword) ? PasswordManager.DecryptText(itm.e.CrossLinkPassword.ToString()) : "";
                    model.OfficePortalUrl = itm.e.OfficePortalUrl != null ? itm.e.OfficePortalUrl.ToString() : "";
                    model.TaxOfficeUsername = itm.e.TaxOfficeUsername != null ? itm.e.TaxOfficeUsername.ToString() : "";
                    model.TaxOfficePassword = !string.IsNullOrEmpty(itm.e.TaxOfficePassword) ? PasswordManager.DecryptText(itm.e.TaxOfficePassword.ToString()) : "";
                    model.CustomerOfficeId = itm.e.CustomerOfficeId;
                    model.EMPPassword = !string.IsNullOrEmpty(itm.e.EMPPassword) ? PasswordManager.DecryptText(itm.e.EMPPassword.ToString()) : "";
                    model.EMPUserId = itm.e.EMPUserId != null ? itm.e.EMPUserId.ToString() : "";
                }

                return model;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerLoginInformationService/GetCustomerLoginInformation", id);
                throw;
            }
        }

    }
}