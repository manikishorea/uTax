using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPPortal.Transactions.Configuration.DTO;
using EMP.Core.Utilities;
using System.Data.Entity;
using EMPPortal.Core.Utilities;

namespace EMPPortal.Transactions.Configuration
{
    public class CustomerConfigStatusService : ICustomerConfigStatusService, IDisposable
    {
        DatabaseEntities db;
        /// <summary>
        /// This Method is used to Get the Main Office Configuration Details
        /// </summary>
        /// <returns></returns>
        public IQueryable<CustomerConfigStatusDTO> GetById(Guid userid)
        {
            try
            {

                db = new DatabaseEntities();
                var data = db.CustomerConfigurationStatus.Where(o => o.CustomerId == userid).Select(o => new CustomerConfigStatusDTO
                {
                    Id = o.Id.ToString(),
                    CustomerId = o.CustomerId.ToString(),
                    SitemapId = o.SitemapId.ToString(),
                    Status = o.StatusCode,
                    bankid = o.bankid.ToString(),
                }).DefaultIfEmpty();

                return data;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "Configuration/Get", Guid.Empty);
                return new List<CustomerConfigStatusDTO>().AsQueryable();
            }
        }

        /// <summary>
        /// This method is used to Save the main office details 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public bool CustomerConfigStatusSave(CustomerConfigStatusDTO dto)
        {
            try
            {

                int entityState = 0;

                CustomerConfigurationStatu model = new CustomerConfigurationStatu();

                if (dto != null)
                {
                    Guid Id, SitemapId, CustomerId, xBankId;

                    bool IsBankId = Guid.TryParse(dto.bankid, out xBankId);
                    if (!IsBankId)
                    {
                        return false;
                    }

                    bool IsSitemapId = Guid.TryParse(dto.SitemapId, out SitemapId);
                    if (!IsSitemapId)
                    {
                        return false;
                    }

                    bool IsCustomerId = Guid.TryParse(dto.CustomerId, out CustomerId);
                    if (!IsCustomerId)
                    {
                        return false;
                    }

                    if (Guid.TryParse(dto.Id, out Id))
                    {
                        model = db.CustomerConfigurationStatus.Where(a => a.CustomerId == CustomerId && a.SitemapId == SitemapId).FirstOrDefault();
                        if (model != null)
                        {
                            entityState = (int)System.Data.Entity.EntityState.Modified;
                        }
                        else { return false; }
                    }
                    else
                    {
                        model.Id = Guid.NewGuid();
                        entityState = (int)System.Data.Entity.EntityState.Added;
                    }

                    model.CustomerId = CustomerId; // newguid;
                    model.StatusCode = dto.Status;
                    model.UpdatedBy = dto.UserId ?? Guid.Empty;
                    model.UpdatedDate = System.DateTime.Now;
                    model.bankid = xBankId; // newguid;

                    if (entityState == (int)System.Data.Entity.EntityState.Added)
                    {
                        db.CustomerConfigurationStatus.Add(model);
                    }
                    else
                    {
                        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
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
                    ExceptionLogger.LogException(ex.ToString(), "Configuration/ConfigurationStatus", Guid.Empty);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "Configuration/ConfigurationStatus", Guid.Empty);
                return false;
            }
        }

        public bool SaveConfigurationSatus(Guid CustomerId, Guid UserId, Guid SiteMapID, string resettype, string ActiveLinkSiteMapID, Guid BankId, string status = "done")
        {
            try
            {
                db = new DatabaseEntities();


                int entityState = 0;
                CustomerConfigurationStatu model = new CustomerConfigurationStatu();

                if (BankId == Guid.Empty && SiteMapID != new Guid("067C03A3-34F1-4143-BEAE-35327A8FCA44"))
                {
                    model = db.CustomerConfigurationStatus.Where(a => a.CustomerId == CustomerId && a.SitemapId == SiteMapID).FirstOrDefault();
                }
                else
                {
                    model = db.CustomerConfigurationStatus.Where(a => a.CustomerId == CustomerId && a.SitemapId == SiteMapID && a.bankid == BankId).FirstOrDefault();
                }

                if (model != null)
                {
                    entityState = (int)System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    model = new CustomerConfigurationStatu();
                    model.Id = Guid.NewGuid();
                    entityState = (int)System.Data.Entity.EntityState.Added;
                }
                model.SitemapId = SiteMapID;
                model.CustomerId = CustomerId;

                if (status.ToLower() == EMPConstants.Done)
                {
                    model.StatusCode = EMPConstants.Done;
                }
                else if (status.ToLower() == EMPConstants.None)
                {
                    model.StatusCode = EMPConstants.None;
                }

                model.UpdatedBy = UserId;
                model.UpdatedDate = System.DateTime.Now;
                model.bankid = BankId;

                if (entityState == (int)System.Data.Entity.EntityState.Added)
                {
                    db.CustomerConfigurationStatus.Add(model);
                }
                else
                {
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                }
                try
                {
                    db.SaveChanges();
                    

                    var xResetType = resettype;
                    if (!string.IsNullOrEmpty(xResetType))
                    {
                        if (xResetType == "_reset" || xResetType == "reset")
                        {
                            Guid GActiveLinkSitemapID = Guid.Empty;
                            if (!Guid.TryParse(ActiveLinkSiteMapID, out GActiveLinkSitemapID))
                            {
                                return false;
                            }

                            var result = ResetConfigurationSatus(CustomerId, UserId, GActiveLinkSitemapID);
                        }
                    }

                    var isverified = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).Select(x => x.IsVerified).FirstOrDefault();
                    if(SiteMapID.ToString().ToLower()== "98a706d7-031f-4c5d-8cc4-d32cc7658b63" || (SiteMapID.ToString().ToLower()== "1f2a6418-f9bc-4878-ab0b-2fa004c01c01" && (isverified??false)))
                    {
                        SaveConfigurationSatus(CustomerId, UserId, new Guid("7c8aa474-2535-4f69-a2ae-c3794887f92d"), resettype, ActiveLinkSiteMapID, BankId, status);
                    }
                    db.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogException(ex.ToString(), "Configuration/Save", UserId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "Configuration/Save", UserId);
                return false;
            }
        }

        public bool ResetConfigurationSatus(Guid CustomerId, Guid UserId, Guid ActiveLinkSiteMapID)
        {
            try
            {
                db = new DatabaseEntities();

                var User = db.emp_CustomerInformation.Where(o => o.Id == CustomerId).FirstOrDefault();
                if (User != null)
                {
                    User.IsActivationCompleted = 0;
                    db.Entry(User).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    var ConfigStatus = db.CustomerConfigurationStatus.Where(o => o.CustomerId == CustomerId && o.SitemapId == ActiveLinkSiteMapID).FirstOrDefault();
                    if (ConfigStatus != null)
                    {
                        db.CustomerConfigurationStatus.Remove(ConfigStatus);
                        db.SaveChanges();
                    }
                    return true;

                    //int entityState = 0;
                    //CustomerConfigurationStatu model = new CustomerConfigurationStatu();
                    //model = db.CustomerConfigurationStatus.Where(a => a.CustomerId == UserId && a.SitemapId == FormSiteMapID).FirstOrDefault();
                    //if (model != null)
                    //{
                    //    entityState = (int)System.Data.Entity.EntityState.Modified;
                    //}
                    //else
                    //{
                    //    model = new CustomerConfigurationStatu();
                    //    model.Id = Guid.NewGuid();
                    //    entityState = (int)System.Data.Entity.EntityState.Added;
                    //}

                    //model.SitemapId = FormSiteMapID;
                    //model.CustomerId = UserId;
                    //model.StatusCode = EMPConstants.Done;
                    //model.UpdatedBy = UserId;
                    //model.UpdatedDate = System.DateTime.Now;


                    //var User = db.emp_CustomerInformation.Where(o => o.Id == CustomerId).FirstOrDefault();
                    //if (User != null)
                    //{
                    //    User.IsActivationCompleted = 0;
                    //    db.Entry(User).State = System.Data.Entity.EntityState.Modified;
                    //    db.SaveChanges();

                    //    try
                    //    {
                    //        db.SaveChanges();
                    //        db.Dispose();
                    //        return true;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        ExceptionLogger.LogException(ex.ToString(), "Configuration/ResetActivation", UserId);
                    //        return false;
                    //    }
                    //}
                }
                return false;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "Configuration/ResetActivation", UserId);
                return false;
            }
        }

        public bool ActivateCustomer(Guid CustomerId, Guid UserId)
        {
            try
            {
                db = new DatabaseEntities();

                emp_CustomerInformation model = new emp_CustomerInformation();
                model = db.emp_CustomerInformation.Where(a => a.Id == CustomerId).FirstOrDefault();

                if (model != null)
                {
                    model.IsActivationCompleted = 1;
                    model.AccountStatus = "Active";
                    model.LastUpdatedBy = UserId;
                    model.LastUpdatedDate = DateTime.Now;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                }

                try
                {
                    db.SaveChanges();
                    db.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogException(ex.ToString(), "Configuration/ActivateCustomer", UserId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "Configuration/ActivateCustomer", UserId);
                return false;
            }
        }


        public bool UpdateEFINAfterApproved(Guid CustomerId, Guid UserId,int EFIN)
        {
            db = new DatabaseEntities();
            var mainResult = db.UpdateEFINAfterApprove(CustomerId.ToString(), UserId.ToString(), EFIN);
            return true;
        }

        public bool GetBankEnrollmentStatusByUser(Guid CustomerId)
        {
            db = new DatabaseEntities();
            var mainResult = db.BankEnrollments.Where(o => o.CustomerId == CustomerId && o.StatusCode == "APR").Any();
            return mainResult;
        }

        public int SetSOSOMEActivation(Guid CustomerId, Guid UserId)
        {
            db = new DatabaseEntities();
            var mainResult = db.SF_SOSOME_Update(CustomerId.ToString(), UserId.ToString(), true, true, true);
            return mainResult;
        }

        public bool UpdateFeeAfterApproved(Guid CustomerId, Guid UserId)
        {
            db = new DatabaseEntities();
            var mainResult = db.UpdateFeeAfterApprove(CustomerId.ToString(), UserId.ToString());
            return true;
        }

        #region IDisposable Support
        public void Dispose()
        {
            db.Dispose();
            throw new NotImplementedException();
        }
        #endregion
    }
}
