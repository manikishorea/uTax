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
using System.Web.Http.Cors;
using System.Web.Http.Description;
using EMPAdmin.Transactions.Group;
using EMPAdmin.Transactions.Group.DTO;
using EMPAdminWebAPI.Filters;
namespace EMPAdminWebAPI.APIControllers
{

    [TokenAuthorization]
    public class GroupRoleMapsController : ApiController
    {
        GroupRoleService groupRoleService = new GroupRoleService();

        // GET: api/GroupRoleMaps
        public IQueryable<GroupRoleDTO> GetGroupRoleMaps()
        {
            var groupRoleMap = groupRoleService.GetAllGroupRole();
            return groupRoleMap;
        }

        // GET: api/GroupRoleMaps/5
        public GroupRoleDTO GetGroupRoleMap(Guid id)
        {
            var groupRoleMap = groupRoleService.GetGroupRolesByGroupId(id);
            return groupRoleMap;
        }

        // POST: api/GroupRoleMaps
        [ResponseType(typeof(GroupRoleDTO))]
        public async Task<IHttpActionResult> PostGroupRoleMap(GroupRoleDTO groupRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            Guid id = groupRoleDto.Id;
            int EntityStateId = 0;
            if (id == Guid.Empty)
            {
                id = Guid.NewGuid();
                // user.Id = id;
                EntityStateId = (int)EntityState.Added;
            }
            else
            {
                EntityStateId = (int)EntityState.Modified;
            }

            int result = await groupRoleService.Save(groupRoleDto, id, EntityStateId);
            groupRoleDto.Id = id;
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

        // PUT: api/GroupRoleMaps/id
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUserMaster(Guid id, GroupDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            if (id == user.Id)
            {
                // return 
            }
            int EntityStateId = (int)EntityState.Modified;

            int result = await groupRoleService.SaveStatus(user, id, EntityStateId);

            if (result == (int)HttpStatusCode.NotModified)
            {
                return StatusCode(HttpStatusCode.NotModified);
            }
            else if (result == (int)HttpStatusCode.InternalServerError)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/GroupRoleMaps/5
        [ResponseType(typeof(bool))]
        public async Task<IHttpActionResult> DeleteGroupRoleMap(Guid id)
        {
            bool result = await groupRoleService.Delete(id);
            if (result == false)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}