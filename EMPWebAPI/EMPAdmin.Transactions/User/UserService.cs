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
using EMPAdmin.Transactions.Group.DTO;
using EMPAdmin.Transactions.Role.DTO;

using System.Globalization;
using System.Configuration;

namespace EMPAdmin.Transactions.User
{
    public class UserService : IUserService, IDisposable
    {
        public DatabaseEntities db = new DatabaseEntities();
        public EMPDBEntities db2 = new EMPDBEntities();
        public UserDetailDTO user = new UserDetailDTO();
        public UserRoleMapService _UserRoleService = new UserRoleMapService();
        public UserGroupMapService _UserGroupService = new UserGroupMapService();

        //public UserService(DatabaseEntities _db, UserDetailDTO _user)
        //{
        //    db = _db;
        //    user = _user;
        //}

        public IQueryable<UserDTO> GetAllUser()
        {
            db = new DatabaseEntities();
            var User = db2.UserMasters.Select(o => new UserDTO
            {
                Id = o.Id,
                FirstName = o.FirstName,
                LastName = o.LastName,
                EmailAddress = o.EmailAddress,
                UserName = o.UserName,
                GroupName = o.UserGroupMaps.Select(s => new GroupDTO() { Id = s.GroupId, Name = s.GroupMaster.Name }).FirstOrDefault().Name,
                AccessType = o.EntityMaster.AccessTypeMaster.Name,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();
            db.Dispose();
            return User.OrderBy(o => o.FirstName);
        }

        public async Task<UserDetailDTO> GetUser(Guid UserId)
        {
            var user = await db.UserMasters.Where(o => o.Id == UserId).Select(o => new UserDetailDTO
            {
                Id = o.Id,
                FirstName = o.FirstName,
                LastName = o.LastName,
                EntityName = o.EntityMaster.Name,
                //CustomerName = o.CustomerMaster.Name,
                StatusCode = o.StatusCode,

                EntityId = o.EntityId,
                CustomerId = o.CustomerId,
                UserName = o.UserName,
                Password = o.Password,
                EmailAddress = o.EmailAddress,
                IsEmailConfirmed = o.IsEmailConfirmed,
                EmailConfirmationCode = o.EmailConfirmationCode,
                PasswordResetCode = o.PasswordResetCode,

                LastLoginDate = o.LastLoginDate,
                IsActive = o.IsActive,
                IsActiveDate = o.IsActiveDate,
                UserId = o.CreatedBy,
                Groups = o.UserGroupMaps.Select(s => new GroupDTO() { Id = s.GroupId, Name = s.GroupMaster.Name }).FirstOrDefault(),
                Roles = o.UserRolesMaps.Select(s => new RoleDTO() { Id = s.RoleId, Name = s.RoleMaster.Name }).ToList()

            }).FirstOrDefaultAsync();

            //(p => p.Id == UserId);
            return user;
        }

        public async Task<int> SaveUser(UserDetailDTO _user, Guid Id, int EntityState)
        {
            UserMaster usermaster = new UserMaster();
            string Password = "";

            if (_user != null)
            {
                if (EntityState == (int)System.Data.Entity.EntityState.Modified)
                {
                    var Existuser = db.UserMasters.Where(o => o.Id != Id && o.UserName == _user.UserName).Any();
                    if (Existuser)
                        return -1;

                    usermaster = db.UserMasters.Where(o => o.Id == Id).FirstOrDefault();
                }
                else
                {
                    var Existuser = db.UserMasters.Where(o => o.UserName == _user.UserName).Any();
                    if (Existuser)
                        return -1;
                    Password = GetRandomPassword();
                    usermaster.Password = PasswordManager.CryptText(Password); //"admin1"

                }
                //usermaster.Id = "";
                usermaster.Id = Id;
                usermaster.EntityId = _user.EntityId;
                usermaster.CustomerId = _user.CustomerId;
                usermaster.FirstName = _user.FirstName;
                usermaster.LastName = _user.LastName;
                // usermaster.MiddleName = _user.MiddleName;
                usermaster.UserName = _user.UserName;

                usermaster.EmailAddress = _user.EmailAddress;
                usermaster.IsEmailConfirmed = _user.IsEmailConfirmed;
                usermaster.EmailConfirmationCode = _user.EmailConfirmationCode;
                usermaster.PasswordResetCode = _user.PasswordResetCode;

                usermaster.LastLoginDate = _user.LastLoginDate;
                usermaster.IsActive = true;
                usermaster.IsActiveDate = DateTime.Now;
                usermaster.StatusCode = EMPConstants.Active;

                if (EntityState == (int)System.Data.Entity.EntityState.Modified)
                {

                    usermaster.CreatedBy = _user.UserId;
                    usermaster.CreatedDate = DateTime.Now;
                    usermaster.LastUpdatedDate = DateTime.Now;
                    usermaster.LastUpdatedBy = _user.UserId;
                    db.Entry(usermaster).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    usermaster.LastUpdatedDate = DateTime.Now;
                    usermaster.LastUpdatedBy = _user.UserId;
                    db.UserMasters.Add(usermaster);

                    EmailNotification _email = new EmailNotification();
                    _email.CreatedBy = _user.UserId;
                    _email.CreatedDate = DateTime.Now;
                    _email.EmailCC = "";
                    _email.EmailContent = "";
                    _email.EmailSubject = EMPConstants.NewUserMailSubject;
                    _email.EmailTo = _user.EmailAddress;
                    _email.EmailType = (int)EMPConstants.EmailTypes.NewAdminUser;
                    _email.IsSent = false;
                    _email.Parameters = _user.UserName + "$|$" + Password;
                    db.EmailNotifications.Add(_email);
                }


                // User Group Mapping Saving
                UserGroupMap usergroup = new UserGroupMap();

                UserGroupMapDTO UserGroupMapDto = await _UserGroupService.GetByUserId(usermaster.Id);
                bool IsNewGroup = true;
                if (UserGroupMapDto != null)
                {
                    if (UserGroupMapDto.Id != Guid.Empty)
                    {
                        IsNewGroup = false;
                        usergroup.Id = UserGroupMapDto.Id;
                        usergroup.GroupId = _user.Groups.Id;
                        usergroup.UserId = usermaster.Id;
                        usergroup.StatusCode = EMPConstants.Active;
                        db.Entry(usergroup).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                if (IsNewGroup)
                {
                    usergroup.Id = Guid.NewGuid();
                    usergroup.GroupId = _user.Groups.Id;
                    usergroup.UserId = usermaster.Id;
                    usergroup.StatusCode = EMPConstants.Active;
                    db.UserGroupMaps.Add(usergroup);
                }

                if (_user.Roles.ToList().Count > 0)
                {
                    if (EntityState == (int)System.Data.Entity.EntityState.Modified)
                    {
                        var UserRole1 = db.UserRolesMaps.Where(o => o.UserId == _user.Id).ToList();
                        if (UserRole1.Count() > 0)
                            db.UserRolesMaps.RemoveRange(UserRole1);
                    }

                    List<UserRolesMap> _UserRolesMapList = new List<UserRolesMap>();
                    foreach (RoleDTO Role in _user.Roles)
                    {
                        UserRolesMap _UserRolesMap = new UserRolesMap();
                        _UserRolesMap.Id = Guid.NewGuid();
                        _UserRolesMap.UserId = usermaster.Id;
                        _UserRolesMap.RoleId = Role.Id;
                        _UserRolesMap.StatusCode = EMPConstants.Active;
                        _UserRolesMapList.Add(_UserRolesMap);
                    }

                    db.UserRolesMaps.AddRange(_UserRolesMapList);
                }
            }

            try
            {

                await db.SaveChangesAsync();
                db.Dispose();

                return 1;
            }

            catch (DbUpdateConcurrencyException)
            {
                return 0;
            }
        }

        private string GetRandomPassword()
        {
            try
            {
                Random random = new Random();
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, 8)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            catch (Exception ex)
            {
                return "abc@1234";
            }
        }

        public List<xlinkModel> getxlinkdetails()
        {
            try
            {
                db = new DatabaseEntities();
                var data = (from s in db.UtaxCrosslinkDetails
                            select s).ToList();

                var lst = (from s in data
                           select new xlinkModel
                           {
                               MasterId = s.Username,
                               Password = PasswordManager.DecryptText(s.Password),
                               CLAccountId = s.CLAccountId,
                               CLLogin = s.CLLogin,
                               CLAccountPassword = string.IsNullOrEmpty(s.CLAccountPassword) ? "" : PasswordManager.DecryptText(s.CLAccountPassword),
                               status = s.StatusCode == EMPConstants.Active ? "Active" : "Inactive",
                               Id = s.Id
                           }).ToList();
                return lst;

            }
            catch (Exception ex)
            {
                return new List<xlinkModel>();
            }
        }

        public string updatexlinkdetails(xlinkModel model)
        {
            try
            {
                db = new DatabaseEntities();
                var isexist = (from s in db.UtaxCrosslinkDetails
                               where s.CLAccountId == model.CLAccountId && s.StatusCode == EMPConstants.Active
                               select s).FirstOrDefault();
                if (model.Id == 0)
                {
                    if (isexist != null)
                        return "Exists";

                    UtaxCrosslinkDetail detail = new UtaxCrosslinkDetail();
                    detail.CreatedBy = model.UserId;
                    detail.CreatedDate = DateTime.Now;
                    detail.Password = model.Password == "" ? "" : PasswordManager.CryptText(model.Password);
                    detail.StatusCode = EMPConstants.Active;
                    detail.CLAccountId = model.CLAccountId;
                    detail.CLAccountPassword = PasswordManager.CryptText(model.CLAccountPassword);
                    detail.CLLogin = model.CLLogin;

                    detail.UpdatedBy = model.UserId;
                    detail.UpdatedDate = DateTime.Now;
                    detail.Username = model.MasterId;
                    db.UtaxCrosslinkDetails.Add(detail);
                    db.SaveChanges();
                }
                else if (model.Id != 0)
                {
                    if (isexist != null)
                    {
                        if (isexist.Id != model.Id)
                            return "Exists";
                    }
                    var data = db.UtaxCrosslinkDetails.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (data != null)
                    {
                        data.Username = model.MasterId;
                        data.Password = model.Password == "" ? "" : PasswordManager.CryptText(model.Password);
                        data.StatusCode = EMPConstants.Active;

                        data.CLAccountId = model.CLAccountId;
                        data.CLAccountPassword = PasswordManager.CryptText(model.CLAccountPassword);
                        data.CLLogin = model.CLLogin;

                        data.UpdatedBy = model.UserId;
                        data.UpdatedDate = DateTime.Now;
                        db.SaveChanges();
                    }
                }
                return "Success";
            }
            catch (Exception)
            {
                return "Fail";
            }
        }

        public string InactiveAccount(xlinkModel model)
        {
            try
            {
                db = new DatabaseEntities();

                if (model.Id != 0)
                {
                    if (model.status == "Inactive")
                    {
                        var isexist = (from s in db.UtaxCrosslinkDetails
                                       where s.CLAccountId == model.CLAccountId && s.StatusCode == EMPConstants.Active && s.Id != model.Id
                                       select s).FirstOrDefault();
                        if (isexist != null)
                            return "Exists";
                    }
                    var data = db.UtaxCrosslinkDetails.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (data != null)
                    {
                        data.StatusCode = model.status == "Active" ? EMPConstants.InActive : EMPConstants.Active;
                        data.UpdatedBy = model.UserId;
                        data.UpdatedDate = DateTime.Now;
                        db.SaveChanges();
                        return "success";
                    }
                    else
                        return "Fail";
                }
                else
                    return "Fail";
            }
            catch (Exception)
            {
                return "Fail";
            }
        }

        public async Task<Guid> SaveStatus(UserDTO _user, Guid Id, int EntityState)
        {
            UserMaster usermaster = new UserMaster();
            usermaster = await db.UserMasters.Where(o => o.Id == Id).FirstOrDefaultAsync();

            if (_user.StatusCode == "MY_PROFILE" && EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                usermaster = await db.UserMasters.Where(o => o.Id == Id).FirstOrDefaultAsync();
                usermaster.FirstName = _user.FirstName;
                usermaster.LastName = _user.LastName;
                usermaster.StatusCode = EMPConstants.Active;
                usermaster.EmailAddress = _user.EmailAddress;
            }
            else
            {
                usermaster.IsActive = false;
                usermaster.IsActiveDate = DateTime.Now;

                if (usermaster.StatusCode == EMPConstants.InActive)
                {
                    usermaster.StatusCode = EMPConstants.Active;
                }
                else if (usermaster.StatusCode == EMPConstants.Active)
                {
                    usermaster.StatusCode = EMPConstants.InActive;
                }
            }
            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                usermaster.LastUpdatedDate = DateTime.Now;
                usermaster.LastUpdatedBy = _user.UserId;
                db.Entry(usermaster).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                await db.SaveChangesAsync();
                db.Dispose();
                return usermaster.Id;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!UserMasterExists(usermaster.Id))
                {
                    return Guid.Empty;
                }
                else
                {
                    throw;
                }
            }
        }

        private bool UserMasterExists(Guid id)
        {
            return db.UserMasters.Count(e => e.Id == id) > 0;
        }

        /// <summary>
        /// To Reset Employee Login Password in DB
        /// </summary>
        /// <param name="PasswordHash">hashed password string</param>
        /// <param name="userName">username of the Employee requested for password reset</param>
        /// <returns>true or false based on db update</returns>
        public bool ResetPassword(string passwordHash, string userName)
        {
            try
            {
                using (DatabaseEntities DatabaseEntities = new DatabaseEntities())
                {
                    string uTaxPassword = PasswordManager.CryptText(passwordHash);
                    var usermaster = (from user in DatabaseEntities.UserMasters
                                      where user.UserName == userName
                                      select user).FirstOrDefault();
                    usermaster.LastLoginDate = DateTime.Now;
                    db.Entry(usermaster).State = System.Data.Entity.EntityState.Modified;
                    usermaster.Password = uTaxPassword;// passwordHash;
                    DatabaseEntities.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex.InnerException;
            }
            finally
            {
            }
        }

        public bool ResetPassword(string passwordHash, Guid UserId)
        {
            try
            {
                using (DatabaseEntities DatabaseEntities = new DatabaseEntities())
                {
                    string uTaxPassword = PasswordManager.CryptText(passwordHash);
                    var usermaster = (from user in DatabaseEntities.UserMasters
                                      where user.Id == UserId
                                      select user).FirstOrDefault();

                    usermaster.LastLoginDate = DateTime.Now;
                    db.Entry(usermaster).State = System.Data.Entity.EntityState.Modified;
                    usermaster.Password = uTaxPassword;// passwordHash;
                    DatabaseEntities.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex.InnerException;

            }
            finally
            {

            }
        }

        public void Dispose()
        {
            db.Dispose();
            throw new NotImplementedException();
        }
    }
}