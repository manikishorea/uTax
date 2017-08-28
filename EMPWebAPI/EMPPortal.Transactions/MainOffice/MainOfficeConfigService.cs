using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPPortal.Transactions.MainOffice.DTO;
using EMP.Core.Utilities;
using System.Data.Entity;
using EMPPortal.Core.Utilities;
using EMPPortal.Transactions.CustomerInformation;

namespace EMPPortal.Transactions.MainOffice
{
    public class MainOfficeConfigService : IMainOfficeConfigService
    {
        DatabaseEntities db = new DatabaseEntities();

        /// <summary>
        /// This Method is used to Get the Main Office Configuration Details
        /// </summary>
        /// <returns></returns>
        public async Task<MainOfficeDTO> GetMainOfficeById(Guid strguid)
        {
            try
            {
                db = new DatabaseEntities();
                var data = await db.MainOfficeConfigurations.Where(o => (o.StatusCode == EMPConstants.Active || o.StatusCode == EMPConstants.Pending) && o.emp_CustomerInformation_ID == strguid).Select(o => new MainOfficeDTO
                {
                    Id = o.Id.ToString(),
                    refId = o.emp_CustomerInformation_ID.ToString(),
                    IsSiteTransmitTaxReturns = o.IsSiteTransmitTaxReturns,
                    IsSiteOfferBankProducts = o.IsSiteOfferBankProducts,
                    TaxProfessionals = o.TaxProfessionals,
                    IsSoftwarebeInstalledNetwork = o.IsSoftwarebeInstalledNetwork,
                    ComputerswillruninSoftware = o.ComputerswillruninSoftware,
                    PreferredSupportLanguage = o.PreferredSupportLanguage,
                    IsBusinessSoftware = o.HasBusinessSoftware,
                    IsSharingEFIN = o.IsSharingEfin
                }).FirstOrDefaultAsync();

                if (data == null)
                {
                    MainOfficeDTO objMO = new MainOfficeDTO();
                    objMO.IsBusinessSoftware = db.emp_CustomerInformation.Where(x => x.Id == strguid).Select(x => x.QuoteSoftwarePackage).FirstOrDefault() == EMPConstants.EnterprisePackage;
                    objMO.Id = Guid.Empty.ToString();
                    return objMO;
                }

                return data;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/GetMainOfficeById", strguid);
                return null;
            }
        }

        /// <summary>
        /// This method is used to Save the main office details 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool Save(MainOfficeDTO dto)
        {
            try
            {
                int entityState = 0;
                bool prevBS = false;
                Guid newguid, newguid2;
                MainOfficeConfiguration mainofficeconfigurationDto = new MainOfficeConfiguration();
                if (dto != null)
                {
                    if (Guid.TryParse(dto.Id, out newguid))
                    {
                        mainofficeconfigurationDto = db.MainOfficeConfigurations.Where(a => a.Id == newguid).FirstOrDefault();
                        if (mainofficeconfigurationDto != null)
                        {
                            prevBS = mainofficeconfigurationDto.HasBusinessSoftware;
                            entityState = (int)System.Data.Entity.EntityState.Modified;
                        }
                        else { return false; }
                    }
                    else
                    {
                        mainofficeconfigurationDto.Id = Guid.NewGuid();
                        entityState = (int)System.Data.Entity.EntityState.Added;
                    }

                    bool IsRefId = Guid.TryParse(dto.refId, out newguid2);
                    mainofficeconfigurationDto.emp_CustomerInformation_ID = newguid2; // newguid;
                    mainofficeconfigurationDto.IsSiteTransmitTaxReturns = dto.IsSiteTransmitTaxReturns;
                    mainofficeconfigurationDto.IsSiteOfferBankProducts = dto.IsSiteOfferBankProducts;
                    mainofficeconfigurationDto.TaxProfessionals = dto.TaxProfessionals;
                    mainofficeconfigurationDto.IsSoftwarebeInstalledNetwork = dto.IsSoftwarebeInstalledNetwork;
                    mainofficeconfigurationDto.ComputerswillruninSoftware = dto.ComputerswillruninSoftware;
                    mainofficeconfigurationDto.PreferredSupportLanguage = dto.PreferredSupportLanguage;
                    mainofficeconfigurationDto.StatusCode = EMPConstants.Active;
                    mainofficeconfigurationDto.LastUpdatedBy = dto.UserId ?? Guid.Empty;
                    mainofficeconfigurationDto.LastUpdatedDate = System.DateTime.Now;
                    mainofficeconfigurationDto.HasBusinessSoftware = dto.IsBusinessSoftware;
                    mainofficeconfigurationDto.IsSharingEfin = dto.IsSharingEFIN;

                    if (entityState == (int)System.Data.Entity.EntityState.Added)
                    {
                        mainofficeconfigurationDto.CreatedBy = dto.UserId ?? Guid.Empty;
                        mainofficeconfigurationDto.CreatedDate = System.DateTime.Now;
                        db.MainOfficeConfigurations.Add(mainofficeconfigurationDto);
                    }
                    else
                    {
                        db.Entry(mainofficeconfigurationDto).State = System.Data.Entity.EntityState.Modified;
                    }

                    var enrollconfig = db.EnrollmentOfficeConfigurations.Where(x => x.CustomerId == newguid2 && x.StatusCode==EMPConstants.Active).FirstOrDefault();
                    if (enrollconfig != null)
                    {
                        enrollconfig.IsSoftwareOnNetwork = dto.IsSoftwarebeInstalledNetwork;
                        enrollconfig.LastUpdatedBy = dto.UserId??Guid.Empty;
                        enrollconfig.LastUpdatedDate = DateTime.Now;
                        enrollconfig.NoofComputers = dto.ComputerswillruninSoftware;
                        enrollconfig.NoofTaxProfessionals = dto.TaxProfessionals;
                        enrollconfig.PreferredLanguage = dto.PreferredSupportLanguage;                        
                    }

                }
                else
                    newguid2 = Guid.Empty;
                db.SaveChanges();

                var custinfo = db.emp_CustomerInformation.Where(x => x.Id == newguid2).FirstOrDefault();
                if (custinfo != null)
                {
                    if (custinfo.IsActivationCompleted == 1)
                    {
                        if (prevBS != dto.IsBusinessSoftware && dto.IsBusinessSoftware)
                        {
                            var loginfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == newguid2).FirstOrDefault();

                            var sy = db.SalesYearMasters.Where(x => x.Id == custinfo.SalesYearID).Select(x => x.SalesYear).FirstOrDefault();
                            CustomerInformationService cis = new CustomerInformation.CustomerInformationService();
                            cis.SaveEmpCsrData(newguid2, "Business Software", custinfo.SalesforceAccountID, sy.Value.ToString());

                            EmailNotification _email = new EmailNotification();
                            _email.CreatedBy = dto.UserId ?? Guid.Empty;
                            _email.CreatedDate = DateTime.Now;
                            _email.EmailCC = "";
                            _email.EmailContent = "";
                            _email.EmailSubject = "Business Software";
                            _email.EmailTo = EMPConstants.SupportutaxEmail;
                            _email.EmailType = (int)EMPConstants.EmailTypes.BusinessSoftware;
                            _email.IsSent = false;
                            _email.Parameters = loginfo.EMPUserId + "$|$" + loginfo.MasterIdentifier + "$|$" + dto.IsBusinessSoftware;
                            db.EmailNotifications.Add(_email);
                            db.SaveChanges();
                        }
                    }
                    //if (prevBS != dto.IsBusinessSoftware && dto.IsBusinessSoftware && (custinfo.EntityId!=(int)EMPConstants.Entity.MO && custinfo.EntityId!=(int)EMPConstants.Entity.SVB && custinfo.EntityId != (int)EMPConstants.Entity.SO))
                    //{
                    //    var parentinfo = db.emp_CustomerInformation.Where(x => x.Id == custinfo.ParentId).FirstOrDefault();
                    //    var parentsb = db.MainOfficeConfigurations.Where(x=>x)
                    //    if(parentinfo!=null)
                    //}
                }


                db.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/Save", dto.UserId);
                return false;
            }
        }
    }
}
