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
namespace EMPAdminWebAPI.APIControllers
{
    [TokenAuthorization]
    public class RolePermissionMapsController : ApiController
    {
        private uTaxDBEntities db = new uTaxDBEntities();

        // GET: api/RolePermissionMaps
        public IQueryable<RolePermissionMap> GetRolePermissionMaps()
        {
            return db.RolePermissionMaps;
        }

        // GET: api/RolePermissionMaps/5
        [ResponseType(typeof(RolePermissionMap))]
        public async Task<IHttpActionResult> GetRolePermissionMap(Guid id)
        {
            RolePermissionMap rolePermissionMap = await db.RolePermissionMaps.FindAsync(id);
            if (rolePermissionMap == null)
            {
                return NotFound();
            }

            return Ok(rolePermissionMap);
        }

        // PUT: api/RolePermissionMaps/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRolePermissionMap(Guid id, RolePermissionMap rolePermissionMap)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rolePermissionMap.Id)
            {
                return BadRequest();
            }

            db.Entry(rolePermissionMap).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolePermissionMapExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/RolePermissionMaps
        [ResponseType(typeof(RolePermissionMap))]
        public async Task<IHttpActionResult> PostRolePermissionMap(RolePermissionMap rolePermissionMap)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RolePermissionMaps.Add(rolePermissionMap);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RolePermissionMapExists(rolePermissionMap.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = rolePermissionMap.Id }, rolePermissionMap);
        }

        // DELETE: api/RolePermissionMaps/5
        [ResponseType(typeof(RolePermissionMap))]
        public async Task<IHttpActionResult> DeleteRolePermissionMap(Guid id)
        {
            RolePermissionMap rolePermissionMap = await db.RolePermissionMaps.FindAsync(id);
            if (rolePermissionMap == null)
            {
                return NotFound();
            }

            db.RolePermissionMaps.Remove(rolePermissionMap);
            await db.SaveChangesAsync();

            return Ok(rolePermissionMap);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RolePermissionMapExists(Guid id)
        {
            return db.RolePermissionMaps.Count(e => e.Id == id) > 0;
        }
    }
}