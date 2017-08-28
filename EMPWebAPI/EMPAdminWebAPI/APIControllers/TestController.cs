using EMPAdmin.Transactions.Entity;
using EMPAdmin.Transactions.Entity.DTO;
using EMPEntityFramework.Edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace EMPAdminWebAPI.APIControllers
{
    public class TestController : ApiController
    {

        //[ResponseType(typeof(string))]
        //public IHttpActionResult GetTest()
        //{
        //    try
        //    {
        //        return Ok(DateTime.Now.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ex.Message + ", " + ex.StackTrace + ", " + ex.InnerException.Message);
        //    }
        //}

        [ResponseType(typeof(EntityDTO))]
        public IHttpActionResult GetTestById()
        {
            try
            {
                DatabaseEntities _db = new DatabaseEntities();
                var entity = _db.EntityMasters.Where(a => a.Name != "uTax").OrderBy(o => o.DisplayOrder).Select(o => new EntityDTO
                {
                    Id = o.Id,
                    Name = o.Name,
                    Description = o.Description,
                    ParentId = o.ParentId ?? 0,
                    ParentName = o.ParentId != null ? _db.EntityMasters.Where(s => s.ParentId == o.ParentId).FirstOrDefault().Name : ""
                }).DefaultIfEmpty();

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message + ", " + ex.StackTrace + ", " + ex.InnerException.Message);
            }
        }
    }

}
