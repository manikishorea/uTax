using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPPortal.Transactions.FeeReimbursement.DTO;
using EMP.Core.Utilities;
using System.Data.Entity;
namespace EMPPortal.Transactions.FeeReimbursement
{
    public class FeeReimbursementService : IFeeReimbursementService
    {
        DatabaseEntities db = new DatabaseEntities();

        /// <summary>
        /// This method is used to get the fee reimbursement by id
        /// </summary>
        /// <param name="strguid"></param>
        /// <returns></returns>
        public async Task<FeeReimbursementDTO> GetFeeReimbursementById(Guid strguid)
        {
            try
            {
                db = new DatabaseEntities();
                var data = await db.FeeReimbursementConfigs.Where(o => (o.StatusCode == EMPConstants.Active || o.StatusCode == EMPConstants.Pending) && o.emp_CustomerInformation_ID == strguid).Select(o => new FeeReimbursementDTO
                {
                    ID = o.ID.ToString(),
                    refId = o.emp_CustomerInformation_ID.ToString(),
                    AccountName = o.AccountName,
                    BankName = o.BankName,
                    AccountType = o.AccountType,
                    RTN = o.RTN,
                    BankAccountNo = o.BankAccountNo,
                    IsAuthorize = o.IsAuthorize,
                }).FirstOrDefaultAsync();
                return data;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/GetFeeReimbursementById", strguid);
                return null;
            }           
        }

        /// <summary>
        /// This method is used save the fee reimbursement details
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool Save(FeeReimbursementDTO dto)
        {            
            int entityState = 0;
            FeeReimbursementConfig feereimbursementconfig = new FeeReimbursementConfig();
            if (dto != null)
            {
                Guid newguid, newguid2;
                if (Guid.TryParse(dto.ID, out newguid))
                {
                    feereimbursementconfig = db.FeeReimbursementConfigs.Where(a => a.ID == newguid).FirstOrDefault();
                    if (feereimbursementconfig != null)
                    {
                        entityState = (int)System.Data.Entity.EntityState.Modified;
                    }
                    else { return false; }
                }
                else
                {
                    feereimbursementconfig.ID = Guid.NewGuid();
                    entityState = (int)System.Data.Entity.EntityState.Added;
                }

                bool IsRefId = Guid.TryParse(dto.refId, out newguid2);
                feereimbursementconfig.emp_CustomerInformation_ID = newguid2; // newguid;
                feereimbursementconfig.AccountName = dto.AccountName;
                feereimbursementconfig.BankName = dto.BankName;
                feereimbursementconfig.AccountType = dto.AccountType;
                feereimbursementconfig.RTN = dto.RTN;
                feereimbursementconfig.BankAccountNo = dto.BankAccountNo;
                feereimbursementconfig.IsAuthorize = dto.IsAuthorize;
                feereimbursementconfig.StatusCode = EMPConstants.Active;
                feereimbursementconfig.LastUpdatedBy = dto.UserId ?? Guid.Empty;
                feereimbursementconfig.LastUpdatedDate = System.DateTime.Now;

                if (entityState == (int)System.Data.Entity.EntityState.Added)
                {
                    feereimbursementconfig.CreatedBy = dto.UserId ?? Guid.Empty;
                    feereimbursementconfig.CreatedDate = System.DateTime.Now;
                    db.FeeReimbursementConfigs.Add(feereimbursementconfig);
                }
                else
                {
                    db.Entry(feereimbursementconfig).State = System.Data.Entity.EntityState.Modified;
                }

                //Guid SiteMapId;
                //Guid.TryParse("60025459-7568-4a77-b152-f81904aaaa63", out SiteMapId);
                //if (!db.CustomerConfigurationStatus.Any(a => a.SitemapId == SiteMapId && a.CustomerId == newguid2))
                //{
                //    CustomerConfigurationStatu ConfigStatusModel = new CustomerConfigurationStatu();
                //    ConfigStatusModel.Id = Guid.NewGuid();
                //    ConfigStatusModel.CustomerId = newguid2;
                //    ConfigStatusModel.SitemapId = SiteMapId;
                //    ConfigStatusModel.StatusCode = "done";
                //    ConfigStatusModel.UpdatedBy = dto.UserId ?? Guid.Empty;
                //    ConfigStatusModel.UpdatedDate = DateTime.Now;
                //    db.CustomerConfigurationStatus.Add(ConfigStatusModel);
                //}
            }
            try
            {
                db.SaveChanges();
                db.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/Save", Guid.Empty);
                return false;
                throw;
            }
        }        
    }
}
