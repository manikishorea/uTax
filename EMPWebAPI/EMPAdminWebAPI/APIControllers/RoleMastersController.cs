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
using EMPAdmin.Transactions.Role;
using EMPAdmin.Transactions.Role.DTO;
using EMPAdminWebAPI.Filters;

using System.Web.Http.Cors;
namespace EMPAdminWebAPI.APIControllers
{
   
    [TokenAuthorization]
    public class RoleMastersController : ApiController
    {
        RoleService _roleService = new RoleService();
        // GET: api/RoleMasters

        public IQueryable<RoleDTO> GetRoleMasters()
        {
            var roleMaster = _roleService.GetAllRoles();
            return roleMaster;
        }

        // GET: api/RoleMasters/5
        public async Task<RoleDTO> GetRoleMaster(Guid id)
        {
            var roleMaster = await _roleService.GetRole(id);
            return roleMaster;
        }
        
        // POST: api/RoleMasters
        [ResponseType(typeof(RoleDTO))]
        public async Task<IHttpActionResult> PostContactPersonTitleMaster(RoleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid id = dto.Id;
            int EntityStateId = 0;
            if (id == Guid.Empty)
            {
                id = Guid.NewGuid();
                //user.Id = id;
                EntityStateId = (int)EntityState.Added;
            }
            else
            {
                EntityStateId = (int)EntityState.Modified;
            }

            int result = await _roleService.Save(dto, id, EntityStateId);
            dto.Id = id;

            if (result == -1)
            {
                return StatusCode(HttpStatusCode.NotModified);
            }
            else if (result == 0)
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            return Ok(id);
        }

        // PUT: api/PutUserMaster/id
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUserMaster(Guid id, RoleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dto.Id)
            {
                return BadRequest();
            }

            int EntityStateId = (int)EntityState.Modified;

            Guid result = await _roleService.SaveStatus(dto, id, EntityStateId);

            if (result == Guid.Empty)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        ////// GET: api/RoleMasters/5
        ////[ResponseType(typeof(RoleDTO))]
        ////public async Task<IHttpActionResult> GetRoleMaster(Guid id)
        ////{
        ////    RoleService roleService = new RoleService();
        ////    var roleMaster = await roleService.GetUserRoles(id);
        ////    if (roleMaster == null)
        ////    {
        ////        return NotFound();
        ////    }

        ////    return Ok(roleMaster);
        ////}

        // PUT: api/RoleMasters/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutRoleMaster(Guid id, RoleDTO roleMaster)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != roleMaster.Id)
        //    {
        //        return BadRequest();
        //    }


        //    bool IsUserRolesMapsExist = await db.UserRolesMaps.Where(o => o.RoleId == id).AnyAsync();
        //    bool IsGroupRoleMapsExist = await db.GroupRoleMaps.Where(o => o.RoleId == id).AnyAsync();
        //    if (!IsUserRolesMapsExist && !IsGroupRoleMapsExist)
        //    {
        //        db.Entry(roleMaster).State = EntityState.Modified;

        //        try
        //        {
        //            await db.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            //if (!RoleMasterExists(id))
        //            //{
        //            //    return NotFound();
        //            //}
        //            //else
        //            //{
        //            //    throw;
        //            //}
        //            return StatusCode(HttpStatusCode.InternalServerError);
        //        }
        //    }
        //    else { return StatusCode(HttpStatusCode.NotModified); }

        //    return StatusCode(HttpStatusCode.OK);
        //}


    }
}