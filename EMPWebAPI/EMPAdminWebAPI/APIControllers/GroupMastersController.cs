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
    public class GroupMastersController : ApiController
    {
        private GroupService _GroupService = new GroupService();

        // GET: api/GroupMasters
        [ResponseType(typeof(IQueryable<GroupDTO>))]
        public IHttpActionResult GetGroupMasters()
        {
            var groupMasters = _GroupService.GetAllGroup();
            if (groupMasters == null)
            {
                return NotFound();
            }

            return Ok(groupMasters);
        }

        // GET: api/GroupMasters/5
        [ResponseType(typeof(GroupDTO))]
        public async Task<IHttpActionResult> GetGroupMaster(Guid id)
        {
            var groupMaster = await _GroupService.GetGroupById(id);
            if (groupMaster == null)
            {
                return NotFound();
            }

            return Ok(groupMaster);
        }
    }
}