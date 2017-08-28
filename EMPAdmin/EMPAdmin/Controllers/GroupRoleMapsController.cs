using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EMPAdmin.Filters;
namespace EMPAdmin.Controllers
{
    [SessionCheckAttribute]
    public class GroupRoleMapsController : BaseController
    {
        public async Task<ActionResult> Index()
        {
            //  var groupRoleMaps = db.GroupRoleMaps.Include(g => g.GroupMaster).Include(g => g.RoleMaster);
            //  return View(await groupRoleMaps.ToListAsync());
            return View();
        }

        public ActionResult Create(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                Id = Guid.Empty.ToString();

            ViewBag.Id = Id;
            return View();
        }
    }
}
