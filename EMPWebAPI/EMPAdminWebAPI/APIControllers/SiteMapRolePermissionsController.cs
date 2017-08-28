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
using EMPAdminWebAPI.Filters;
using EMPAdmin.Transactions.Role;
using EMPAdmin.Transactions.Role.DTO;
using EMPAdmin.Transactions.SitePermissions.DTO;

namespace EMPAdminWebAPI.APIControllers
{
    [TokenAuthorization]
    public class SiteMapRolePermissionsController : ApiController
    {
        //// GET: api/SiteMapRolePermissions
        //public IQueryable<SiteMapRolePermission> GetSiteMapRolePermissions()
        //{
        //    return db.SiteMapRolePermissions;
        //}

        //// GET: api/SiteMapRolePermissions/5
        //[ResponseType(typeof(SiteMapRolePermission))]
        //public async Task<IHttpActionResult> GetSiteMapRolePermission(Guid id)
        //{
        //    SiteMapRolePermission siteMapRolePermission = await db.SiteMapRolePermissions.FindAsync(id);
        //    if (siteMapRolePermission == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(siteMapRolePermission);
        //}

        RoleService _RoleService = new RoleService();

        [ResponseType(typeof(IQueryable<RoleDetailDTO>))]
        public IHttpActionResult GetSiteMapRolePermissions()
        {
            var result = _RoleService.ScreenPermission();
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        [ResponseType(typeof(IQueryable<RoleDetailDTO>))]
        public IHttpActionResult GetSiteMapRolePermissions(Guid id)
        {
            //  var result = _RoleService.ScreenPermission(id);
            var result = _RoleService.ScreenPermission1(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST: api/GroupRoleMaps
        [ResponseType(typeof(SiteRolePermissionsDTO))]
        public async Task<IHttpActionResult> PostGroupRoleMap(SiteRolePermissionsDTO groupRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool result = await _RoleService.SaveSitePermission(groupRoleDto);
            if (!result)
            {
                return NotFound();
            }

            return Ok(result);
        }
    
    }
}