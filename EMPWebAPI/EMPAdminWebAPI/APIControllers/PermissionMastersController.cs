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
using EMPAdmin.Transactions.PermissionMaster.DTO;
using System.Web.Http.Cors;
using EMPAdminWebAPI.Filters;
using EMPAdmin.Transactions.PermissionMaster;
using EMPAdmin.Transactions.User.DTO;

namespace EMPAdminWebAPI.APIControllers
{
    
    [TokenAuthorization]
    public class PermissionMastersController : ApiController
    {
        PermissionMasterService _PermissionMasterService =new PermissionMasterService();

        // GET: api/PermissionMasters/5
        [ResponseType(typeof(PermissionMasterDTO))]
        public async Task<IHttpActionResult> GetPermissionMaster(Guid userId,string roleId)
        {
            var result = await _PermissionMasterService.GetPermissionByUserAndRole(userId, roleId);
            if (result.ToList().Count > 0) {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [ResponseType(typeof(IQueryable<PermissionMasterDTO>))]
        public IHttpActionResult GetPermissionofUserByRole(UserRolesDTO UserRoles)
        {
            var result = _PermissionMasterService.GetPermissionByUserAndRole(UserRoles);
            if (result.ToList().Count <= 0)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}