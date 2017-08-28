using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using EMPPortal.Transactions.Account.Model;
using EMPEntityFramework.Edmx;
using EMP.Core.Utilities;
using EMPPortal.Core.Utilities;

namespace EMPPortal.Transactions.Account
{
    public class ChangePasswordService : IChangePasswordService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<ChangePasswordModel> GetAll()
        {
            try
            {
                db = new DatabaseEntities();
                var data = db.emp_CustomerLoginInformation.Select(o => new ChangePasswordModel
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
                ExceptionLogger.LogException(ex.ToString(), "ChangePassword/GetAllChangePassword", Guid.Empty);
                return null;
            }
        }

        public async Task<ChangePasswordModel> GetById(Guid Id)
        {
            try
            {
                var data = await db.emp_CustomerLoginInformation.Where(o => o.Id == Id).Select(o => new ChangePasswordModel
                {
                    Id = o.Id,
                    EMPPassword = o.EMPPassword,
                    CustomerOfficeId = o.CustomerOfficeId,
                    StatusCode = o.StatusCode
                }).FirstOrDefaultAsync();

                if (data == null)
                {
                    data = await db.UserMasters.Where(o => o.Id == Id).Select(o => new ChangePasswordModel
                    {
                        Id = o.Id,
                        EMPPassword = o.Password,
                        CustomerOfficeId = o.Id,
                        StatusCode = o.StatusCode
                    }).FirstOrDefaultAsync();
                }

                return data;
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ChangePassword/GetChangePassword", Id);
                return null;
            }
        }

        public bool Save(string password, Guid Id)
        {
            try
            {

                var IsCorrect = false;
                emp_CustomerLoginInformation _CustomerLoginInformation = new emp_CustomerLoginInformation();

                _CustomerLoginInformation = db.emp_CustomerLoginInformation.Where(o => o.CustomerOfficeId == Id).FirstOrDefault();

                if (_CustomerLoginInformation != null)
                {
                    string uTaxPassword = PasswordManager.CryptText(password);
                    _CustomerLoginInformation.EMPPassword = uTaxPassword;// password;
                    _CustomerLoginInformation.StatusCode = EMPConstants.Active;
                    _CustomerLoginInformation.LastUpdatedBy = Id;
                    _CustomerLoginInformation.LastUpdatedDate = DateTime.Now;

                    db.Entry(_CustomerLoginInformation).State = System.Data.Entity.EntityState.Modified;

                    IsCorrect = true;
                }
                else
                {
                    var UserMaster = db.UserMasters.Where(o => o.Id == Id).FirstOrDefault();
                    if (UserMaster != null)
                    {
                        string uTaxPassword = PasswordManager.CryptText(password);
                        UserMaster.Password = uTaxPassword;// password;
                        UserMaster.StatusCode = EMPConstants.Active;
                        UserMaster.LastUpdatedBy = UserMaster.Id;
                        UserMaster.LastUpdatedDate = DateTime.Now;
                        db.Entry(UserMaster).State = System.Data.Entity.EntityState.Modified;
                        IsCorrect = true;
                    }
                }

                if (!IsCorrect)
                {
                    return false;
                }

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ExceptionLogger.LogException(ex.ToString(), "ChangePassword/initial", Id);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ChangePassword/initial", Id);
                return false;
            }
        }

        public bool SaveMyPassword(string currentpassword, string password, Guid Id)
        {
            try
            {

                var IsCorrect = false;
                emp_CustomerLoginInformation _CustomerLoginInformation = new emp_CustomerLoginInformation();

                string EMPPassword = PasswordManager.CryptText(currentpassword);

                _CustomerLoginInformation = db.emp_CustomerLoginInformation.Where(o => o.CustomerOfficeId == Id && o.EMPPassword == EMPPassword).FirstOrDefault();
                if (_CustomerLoginInformation != null)
                {
                    string uTaxPassword = PasswordManager.CryptText(password);
                    _CustomerLoginInformation.EMPPassword = uTaxPassword;// password;
                    _CustomerLoginInformation.StatusCode = EMPConstants.Active;
                    _CustomerLoginInformation.LastUpdatedBy = Id;
                    _CustomerLoginInformation.LastUpdatedDate = DateTime.Now;

                    db.Entry(_CustomerLoginInformation).State = System.Data.Entity.EntityState.Modified;
                    IsCorrect = true;
                }
                else
                {
                    var UserMaster = db.UserMasters.Where(o => o.Id == Id && o.Password == EMPPassword).FirstOrDefault();
                    if (UserMaster != null)
                    {
                        string uTaxPassword = PasswordManager.CryptText(password);
                        UserMaster.Password = uTaxPassword;// password;
                        UserMaster.StatusCode = EMPConstants.Active;
                        UserMaster.LastUpdatedBy = Id;
                        UserMaster.LastUpdatedDate = DateTime.Now;

                        db.Entry(UserMaster).State = System.Data.Entity.EntityState.Modified;
                        IsCorrect = true;
                    }
                }


                if (!IsCorrect)
                {
                    return false;
                }

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ExceptionLogger.LogException(ex.ToString(), "ChangePassword/update", Id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ChangePassword/update", Id);
                return false;
            }
        }

        private bool IsExists(Guid id)
        {
            return db.SecurityQuestionMasters.Count(e => e.Id == id) > 0;
        }

        public bool ResetPassword(string password, string EMPUserId)
        {
            try
            {
                var IsCorrect = false;

                emp_CustomerLoginInformation _CustomerLoginInformation = new emp_CustomerLoginInformation();

                _CustomerLoginInformation = db.emp_CustomerLoginInformation.Where(o => o.EMPUserId == EMPUserId).FirstOrDefault();

                if (_CustomerLoginInformation != null)
                {
                    string uTaxPassword = PasswordManager.CryptText(password);
                    _CustomerLoginInformation.EMPPassword = uTaxPassword;// password;
                    _CustomerLoginInformation.StatusCode = EMPConstants.Active;
                    _CustomerLoginInformation.LastUpdatedBy = _CustomerLoginInformation.CustomerOfficeId;
                    _CustomerLoginInformation.LastUpdatedDate = DateTime.Now;
                    db.Entry(_CustomerLoginInformation).State = System.Data.Entity.EntityState.Modified;

                    IsCorrect = true;
                }
                else
                {
                    var UserMaster = db.UserMasters.Where(o => o.UserName == EMPUserId).FirstOrDefault();
                    if (UserMaster != null)
                    {
                        string uTaxPassword = PasswordManager.CryptText(password);
                        UserMaster.Password = uTaxPassword;// password;
                        UserMaster.StatusCode = EMPConstants.Active;
                        UserMaster.LastUpdatedBy = UserMaster.Id;
                        UserMaster.LastUpdatedDate = DateTime.Now;
                        db.Entry(UserMaster).State = System.Data.Entity.EntityState.Modified;
                        IsCorrect = true;
                    }
                }


                if (!IsCorrect)
                {
                    return false;
                }

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ExceptionLogger.LogException(ex.ToString(), "ChangePassword/reset", Guid.Empty);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ChangePassword/reset", Guid.Empty);
                return false;
            }
        }

        public bool ResetPasswordFromAdmin(Guid UserId, Guid CustomerOfficeId)
        {
            try
            {

                var IsCorrect = false;

                emp_CustomerLoginInformation _CustomerLoginInformation = new emp_CustomerLoginInformation();

                _CustomerLoginInformation = db.emp_CustomerLoginInformation.Where(o => o.CustomerOfficeId == CustomerOfficeId).FirstOrDefault();

                if (_CustomerLoginInformation != null)
                {
                    //string uTaxPassword = PasswordManager.CryptText(_CustomerLoginInformation.CrossLinkPassword);
                    _CustomerLoginInformation.EMPPassword = _CustomerLoginInformation.CrossLinkPassword;
                    _CustomerLoginInformation.StatusCode = EMPConstants.Active;
                    _CustomerLoginInformation.LastUpdatedBy = UserId;
                    _CustomerLoginInformation.LastUpdatedDate = DateTime.Now;
                    db.Entry(_CustomerLoginInformation).State = System.Data.Entity.EntityState.Modified;

                    IsCorrect = true;
                }

                if (!IsCorrect)
                {
                    return false;
                }

                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    ExceptionLogger.LogException(ex.ToString(), "ChangePassword/ResetPassword", UserId);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex.ToString(), "ChangePassword/ResetPassword", UserId);
                return false;
            }
        }

        public Guid GetCustomerIdByUserId(string EmpUserId)
        {
            try
            {
                var user = (from l in db.emp_CustomerLoginInformation
                            where l.EMPUserId == EmpUserId
                            select l.CustomerOfficeId).FirstOrDefault();

                return (Guid)user;
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            }
        }
    }
}
