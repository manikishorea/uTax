using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EMP.Core;
using EMPAdmin.Transactions.User.DTO;
using EMPAdmin.Transactions.User;
using EMPAdminWebAPI.Filters;
using System.Web.Http.Cors;

namespace EMPAdminWebAPI.APIControllers
{
    
    [TokenAuthorization]
    public class UserRoleMapController : ApiController
    {
        //private readonly IUserService _UserService;
        //private readonly uTaxDBEntities _db;
        //private readonly UserDetailDTO _user;

        //public UserMastersController(
        //   IUserService UserService, uTaxDBEntities db, UserDetailDTO user)
        //{
        //    _UserService = UserService;
        //    _db = db;
        //    _user = user;
        //}

        public UserRoleMapService _UserRoleMapService = new UserRoleMapService();
        
        // GET: api/UserMasters
        // [Route("GetAllUser")]
        [ResponseType(typeof(UserRoleMapDTO))]
        public IHttpActionResult GetUserRoleMap()
        {
            var data = _UserRoleMapService.GetAllByUser();
            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
            // return user;
        }

        // GET: api/UserMasters
        [ResponseType(typeof(UserRoleMapDTO))]
        public IHttpActionResult GetUserRoleMap(Guid id)
        {
            var data = _UserRoleMapService.GetByUserId(id);

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }


        // POST: api/UserMasters
        //[ResponseType(typeof(UserRoleMapDTO))]
        //public async Task<IHttpActionResult> PostUserMaster(UserRoleMapDTO[] dtoList)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }


        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }


        //    UserRoleMapSaveDTO UserRoleMapSave = new UserRoleMapSaveDTO();
        //    UserRoleMapSave.UserRolesMapList = new List<UserRolesMap>();
        //    foreach (var item in dtoList)
        //    {
        //        UserRolesMap _UserRolesMap = new UserRolesMap();

        //        _UserRolesMap.Id = item.Id;
        //        _UserRolesMap.UserId = item.UserId;
        //        _UserRolesMap.RoleId = item.RoleId;
        //        //_UserRolesMap.StatusCode = item.Id;

        //        if (item.Id == Guid.Empty)
        //        {
        //            item.Id = Guid.NewGuid();
        //            _UserRolesMap.Id = item.Id;
        //            db.UserRolesMaps.Add(_UserRolesMap);
        //        }
        //        else
        //        {
        //            db.Entry(_UserRolesMap).State = System.Data.Entity.EntityState.Modified;
        //        }

        //        try
        //        {
        //            await db.SaveChangesAsync();
        //        }

        //        catch (DbUpdateException)
        //        {
        //            if (UserRolesMapExists(_UserRolesMap.Id))
        //            {
        //                return Conflict();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //    }

        //    Guid id=Guid.Empty;
        //    if (dtoList.ToList().Count > 0)
        //    {
        //        id = dtoList[0].Id;
        //    }

        //    return CreatedAtRoute("DefaultApi", new { id = id }, dtoList);
        //}

        // DELETE: api/UserMasters/5
        //[ResponseType(typeof(UserMaster))]
        //public async Task<IHttpActionResult> DeleteUserMaster(Guid id)
        //{
        //    UserMaster userMaster = new UserMaster();// = await db.UserMasters.FindAsync(id);
        //    //if (userMaster == null)
        //    //{
        //    //    return NotFound();
        //    //}

        //    //db.UserMasters.Remove(userMaster);
        //    //await db.SaveChangesAsync();

        //    return Ok(userMaster);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
              //  db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserRolesMapExists(Guid id)
        {
            return false;// db.UserRolesMaps.Count(e => e.Id == id) > 0; ;
        }
    }
}