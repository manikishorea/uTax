using EMPAdmin.Transactions.User;
using EMPAdmin.Transactions.User.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace EMPAdminWebAPI.APIControllers
{

    public class CrosslinkDetailsController : ApiController
    {
        UserService _UserService = new UserService();

        // GET: api/CrosslinkDetails/5
        [ResponseType(typeof(List<xlinkModel>))]
        public IHttpActionResult Getxlinkdetails()
        {
            var details = _UserService.getxlinkdetails();
            return Ok(details);
        }

        // POST: api/CrosslinkDetails
        [ResponseType(typeof(string))]
        public IHttpActionResult Postxlinkdetails(xlinkModel model)
        {
            var details = _UserService.updatexlinkdetails(model);
            return Ok(details);
        }

        [ResponseType(typeof(bool))]
        public IHttpActionResult Putxlinkdetails(xlinkModel model)
        {
            var details = _UserService.InactiveAccount(model);
            return Ok(details);
        }
    }
}
