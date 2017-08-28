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
using EMPAdmin.Transactions.Entity.DTO;
using EMPAdmin.Transactions.Entity;

using EMPAdminWebAPI.Filters;
using System.Web.Http.Cors;

namespace EMPAdminWebAPI.APIControllers
{
    // [AllowCors("UserMasters")]
    // 
    // [TokenAuthorization]
    //[Route("User")]
    public class EntityMastersController : ApiController
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

        public EntityService _EntityService = new EntityService();

        [ResponseType(typeof(EntityDTO))]
        public IHttpActionResult GetEntityMasters()
        {
            var entity = _EntityService.GetAllEntity();
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        [ResponseType(typeof(EntityDTO))]
        public async Task<IHttpActionResult> GetEntityMaster(int id)
        {
            var entity = await _EntityService.GetEntity(id);

            if (entity.Id <= 0)
            {
                return NotFound();
            }

            return Ok(entity);
        }

    }
}