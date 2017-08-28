using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.Account.Model;
using EMP.Core.Utilities;
using EMPEntityFramework.Edmx;
using EMPPortal.Core.Utilities;

namespace EMPPortal.Transactions.Account
{
    public class ResetPasswordService : IResetPasswordService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<ResetPasswordModel> GetAll()
        {
            try
            {
                db = new DatabaseEntities();
                var data = db.emp_CustomerLoginInformation.Select(o => new ResetPasswordModel
                {
                    Id = o.Id,
                    EMPPassword = o.EMPPassword,
                    CustomerOfficeId = o.CustomerOfficeId,
                    StatusCode = o.StatusCode
                }).DefaultIfEmpty();
                return data;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ResetPassword/GetAllResetPassword", Guid.Empty);
                return null;
            }
        }

        public async Task<ResetPasswordModel> GetById(Guid Id)
        {
            try
            {
                var data = await db.emp_CustomerLoginInformation.Where(o => o.Id == Id).Select(o => new ResetPasswordModel
                {
                    Id = o.Id,
                    EMPPassword = o.EMPPassword,
                    CustomerOfficeId = o.CustomerOfficeId,
                    StatusCode = o.StatusCode
                }).FirstOrDefaultAsync();

                return data;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ResetPassword/GetResetPassword", Guid.Empty);
                return null;
            }
        }

        public Guid Save(ResetPasswordModel _Dto, Guid Id, int EntityState)
        {
            try
            {
                emp_CustomerLoginInformation _CustomerLoginInformation = new emp_CustomerLoginInformation();

                if (_Dto != null)
                {
                    string EMPPassword = PasswordManager.CryptText(_Dto.EMPPassword);

                    _CustomerLoginInformation.Id = Id;
                    _CustomerLoginInformation.EMPPassword = EMPPassword;
                    _CustomerLoginInformation.CustomerOfficeId = _Dto.CustomerOfficeId;
                    _CustomerLoginInformation.CreatedDate = DateTime.Now;
                    _CustomerLoginInformation.StatusCode = EMPConstants.Active;
                }

                if (EntityState == (int)System.Data.Entity.EntityState.Modified)
                {
                    _CustomerLoginInformation.LastUpdatedBy = _Dto.UserId;
                    _CustomerLoginInformation.LastUpdatedDate = DateTime.Now;

                    db.Entry(_CustomerLoginInformation).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    _CustomerLoginInformation.CreatedBy = _Dto.UserId;
                    _CustomerLoginInformation.CreatedDate = DateTime.Now;
                    _CustomerLoginInformation.LastUpdatedBy = _Dto.UserId;
                    _CustomerLoginInformation.LastUpdatedDate = DateTime.Now;
                    db.emp_CustomerLoginInformation.Add(_CustomerLoginInformation);
                }

                try
                {
                    db.SaveChanges();
                    return _CustomerLoginInformation.Id;
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!IsExists(_CustomerLoginInformation.Id))
                    {
                        return _CustomerLoginInformation.Id;
                    }
                    else
                    {
                        return Guid.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ResetPassword/PostResetPassword", Id);
                return Guid.Empty;
            }
        }

        public async Task<Guid> SaveStatus(ResetPasswordModel _Dto, Guid Id, int EntityState)
        {
            try
            {

                emp_CustomerLoginInformation CustomerLoginInformation = new emp_CustomerLoginInformation();
                CustomerLoginInformation = await db.emp_CustomerLoginInformation.Where(o => o.Id == Id).FirstOrDefaultAsync();
                //securityQuestionMaster.ActivatedDate = DateTime.Now;

                if (CustomerLoginInformation.StatusCode == EMPConstants.InActive)
                {
                    CustomerLoginInformation.StatusCode = EMPConstants.Active;
                }
                else if (CustomerLoginInformation.StatusCode == EMPConstants.Active)
                {
                    CustomerLoginInformation.StatusCode = EMPConstants.InActive;
                }

                if (EntityState == (int)System.Data.Entity.EntityState.Modified)
                {
                    CustomerLoginInformation.LastUpdatedDate = DateTime.Now;
                    CustomerLoginInformation.LastUpdatedBy = _Dto.UserId;
                    db.Entry(CustomerLoginInformation).State = System.Data.Entity.EntityState.Modified;
                }

                try
                {
                    await db.SaveChangesAsync();
                    db.Dispose();
                    return CustomerLoginInformation.Id;
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!IsExists(CustomerLoginInformation.Id))
                    {
                        return Guid.Empty;
                    }
                    else
                    {
                        return Guid.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ResetPassword/PutResetPassword", Id);
                return Guid.Empty;
            }
        }

        private bool IsExists(Guid id)
        {
            return db.SecurityQuestionMasters.Count(e => e.Id == id) > 0;
        }


    }
}
