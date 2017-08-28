using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using System.Threading.Tasks;
using EMPEntityFramework.Edmx;

using System.Data.Entity;
using EMPAdmin.Transactions.User.DTO;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using EMP.Core.Utilities;
using EMPAdmin.Transactions.Account.DTO;
using System.Globalization;
using EMP.Core.Token;

namespace EMPAdmin.Transactions.Account
{
    public class AccountService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public AccountDTO _AccountDto = new AccountDTO();
        public AccountDetailDTO _AccountDetailDTO = new AccountDetailDTO();
        TokenService _TokenService = new TokenService();

        //public UserService(DatabaseEntities _db, UserDetailDTO _user)
        //{
        //    db = _db;
        //    user = _user;
        //}

        public Task<AccountDTO> IsUsernameExist(string userName)
        {
            using (DatabaseEntities DatabaseEntities = new DatabaseEntities())
            {
                var Account = DatabaseEntities.UserMasters.Where(o => o.UserName.Equals(userName)).Select(o => new AccountDTO()
                {
                    Id = o.Id,
                    Email = o.EmailAddress,
                    Name = o.FirstName + " " + o.LastName,
                    UserName = o.UserName
                }).FirstOrDefaultAsync();

                return Account;
            }
        }

        public AccountDTO IsUserExist(string userName, string password)
        {
            //string ClientIP = Request.GetClientIpAddress();
            using (DatabaseEntities DatabaseEntities = new DatabaseEntities())
            {
                string uTaxPassword = PasswordManager.CryptText(password);

                var usermaster = (from user in DatabaseEntities.UserMasters
                                  where user.UserName == userName && user.Password == uTaxPassword//password
                                  select user).FirstOrDefault();

                if (usermaster != null)
                {
                    _AccountDto.Id = usermaster.Id;
                    _AccountDto.Email = usermaster.EmailAddress;
                    _AccountDto.Name = usermaster.FirstName + " " + usermaster.LastName;
                    _AccountDto.UserName = usermaster.UserName;
                    _AccountDto.Token = _TokenService.GenerateToken(_AccountDto.Id);

                    usermaster.LastLoginDate = DateTime.Now;
                    db.Entry(usermaster).State = System.Data.Entity.EntityState.Modified;
                    DatabaseEntities.SaveChanges();
                }

                return _AccountDto;
            }
        }

        public bool LastLoginUpdate(string userName)
        {
            try
            {
                using (DatabaseEntities DatabaseEntities = new DatabaseEntities())
                {
                    var usermaster = (from user in DatabaseEntities.UserMasters
                                      where user.UserName == userName
                                      select user).FirstOrDefault();

                    usermaster.LastLoginDate = DateTime.Now;
                    db.Entry(usermaster).State = System.Data.Entity.EntityState.Modified;
                    DatabaseEntities.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex.InnerException;
            }
        }



        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //          db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}