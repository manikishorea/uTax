using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EMPPortalWebAPI.Filters;
using EMPPortal.Transactions.Sitemap.DTO;
using EMPPortal.Transactions.Sitemap;

namespace EMPPortalWebAPI.APIControllers
{
    [TokenAuthorization]
    public class SitemapController : ApiController
    {
        public SitemapService _SitemapService = new SitemapService();
        [ActionName("Main")]
        [ResponseType(typeof(IQueryable<SitemapDTO>))]
        public IHttpActionResult GetSitemap(string entityid,string UserId,string ptype,string mainentity,Guid BankId)
        {
            int Entityid;
            if (!int.TryParse(entityid, out Entityid))
            {
                return NotFound();
            }

            int mainEntityid;
            if (!int.TryParse(mainentity, out mainEntityid))
            {
                return NotFound();
            }

            Guid GUserId;
            if (!Guid.TryParse(UserId, out GUserId))
            {
                return NotFound();
            }

            var data = _SitemapService.GetSitemap(Entityid, GUserId, ptype, mainEntityid, BankId);
            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [ActionName("SubSiteMap")]
        [ResponseType(typeof(IQueryable<SitemapDTO>))]
        public IHttpActionResult GetSubSitemap(string entityid,Guid CustomerId)
        {
            int Entityid;
            if (!int.TryParse(entityid, out Entityid))
            {
                return NotFound();
            }
            var data = _SitemapService.GetSubSitemap(Entityid, CustomerId);
            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [ActionName("GetUserRolePermissions")]
        [ResponseType(typeof(UserRolePermission))]
        public IHttpActionResult GetUserRolePermissions(Guid UserId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            UserRolePermission result = _SitemapService.GetUserRolePermissions(UserId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [ActionName("GetPartnerPermissions")]
        [ResponseType(typeof(UserRolePermission))]
        public IHttpActionResult GetPartnerPermissions()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            UserRolePermission result = _SitemapService.GetPartnerPermissions();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
