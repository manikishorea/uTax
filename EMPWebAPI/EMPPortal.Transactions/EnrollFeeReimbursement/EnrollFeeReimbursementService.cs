using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPPortal.Transactions.EnrollFeeReimbursement.DTO;
using EMP.Core.Utilities;
using System.Data.Entity;
namespace EMPPortal.Transactions.EnrollFeeReimbursement
{
    public class EnrollFeeReimbursementService
    {
        DatabaseEntities db = new DatabaseEntities();
        /// <summary>
        /// This method is used to get the fee reimbursement by id
        /// </summary>
        /// <param name="strguid"></param>
        /// <returns></returns>
        public async Task<EnrollFeeReimbursementDTO> GetEnrollFeeReimbursementById(Guid strguid, Guid BankId)
        {
            try
            {
                db = new DatabaseEntities();
                var data = await db.EnrollmentFeeReimbursementConfigs.Where(o => o.StatusCode == EMPConstants.Active && o.emp_CustomerInformation_ID == strguid && o.BankId == BankId).Select(o => new EnrollFeeReimbursementDTO
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

                if (data == null)
                {
                    var olddatat = await db.EnrollmentFeeReimbursementConfigs.Where(o => o.StatusCode == EMPConstants.Active && o.emp_CustomerInformation_ID == strguid).OrderByDescending(x => x.LastUpdatedDate).Select(o => new EnrollFeeReimbursementDTO
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
                    return olddatat;
                }

                return data;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollFeeReimbursementService/GetEnrollFeeReimbursementById", strguid);
                return null;
            }
        }

        /// <summary>
        /// This method is used save the fee reimbursement details
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool Save(EnrollFeeReimbursementDTO dto)
        {
            int entityState = 0;
            EnrollmentFeeReimbursementConfig enrollfeereimbursementconfig = new EnrollmentFeeReimbursementConfig();
            if (dto != null)
            {
                Guid newguid, newguid2;
                if (Guid.TryParse(dto.ID, out newguid))
                {
                    enrollfeereimbursementconfig = db.EnrollmentFeeReimbursementConfigs.Where(a => a.ID == newguid).FirstOrDefault();
                    if (enrollfeereimbursementconfig != null)
                    {
                        entityState = (int)System.Data.Entity.EntityState.Modified;
                    }
                    else { return false; }
                }
                else
                {
                    enrollfeereimbursementconfig.ID = Guid.NewGuid();
                    entityState = (int)System.Data.Entity.EntityState.Added;
                }

                bool IsRefId = Guid.TryParse(dto.refId, out newguid2);
                enrollfeereimbursementconfig.BankId = dto.BankId;
                enrollfeereimbursementconfig.emp_CustomerInformation_ID = newguid2; // newguid;
                enrollfeereimbursementconfig.AccountName = dto.AccountName;
                enrollfeereimbursementconfig.BankName = dto.BankName;
                enrollfeereimbursementconfig.AccountType = dto.AccountType;
                enrollfeereimbursementconfig.RTN = dto.RTN;
                enrollfeereimbursementconfig.BankAccountNo = dto.BankAccountNo;
                enrollfeereimbursementconfig.IsAuthorize = dto.IsAuthorize;
                enrollfeereimbursementconfig.StatusCode = EMPConstants.Active;
                enrollfeereimbursementconfig.LastUpdatedBy = dto.UserId ?? Guid.Empty;
                enrollfeereimbursementconfig.LastUpdatedDate = System.DateTime.Now;

                if (entityState == (int)System.Data.Entity.EntityState.Added)
                {
                    enrollfeereimbursementconfig.CreatedBy = dto.UserId ?? Guid.Empty;
                    enrollfeereimbursementconfig.CreatedDate = System.DateTime.Now;
                    db.EnrollmentFeeReimbursementConfigs.Add(enrollfeereimbursementconfig);

                    //Guid SiteMapId;
                    //Guid.TryParse("a55334d1-3960-44c4-8cf1-e3ba9901f2be", out SiteMapId);
                    //CustomerConfigurationStatu ConfigStatusModel = new CustomerConfigurationStatu();
                    //ConfigStatusModel.Id = Guid.NewGuid();
                    //ConfigStatusModel.CustomerId = newguid2;
                    //ConfigStatusModel.SitemapId = SiteMapId;
                    //ConfigStatusModel.StatusCode = "done";
                    //ConfigStatusModel.UpdatedBy = dto.UserId ?? Guid.Empty;
                    //ConfigStatusModel.UpdatedDate = DateTime.Now;
                    //db.CustomerConfigurationStatus.Add(ConfigStatusModel);
                }
                else
                {
                    db.Entry(enrollfeereimbursementconfig).State = System.Data.Entity.EntityState.Modified;
                }
            }
            try
            {
                db.SaveChanges();
                db.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollFeeReimbursementService/Save", Guid.Empty);
                return false;
                throw;
            }
        }
    }
}
