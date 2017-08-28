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

namespace uTax.Portal.Controllers
{
    [SessionCheckAttribute]
    public class RoleMastersController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                Id = Guid.Empty.ToString();

            ViewBag.Id = Id;
            return View();
        }

        //// GET: RoleMasters
        //public async Task<ActionResult> Index()
        //{
        //    Guid userId = Guid.Empty;
        //    if (Session["UserId"] != null)
        //    {
        //        userId = (Guid)Session["UserId"];
        //    }
        //    RoleService role = new RoleService();
        //    var roles = role.GetAllRoles();
        //    RoleDTO roleDTO = new RoleDTO();
        //    roleDTO.ManageRoleList = roles;
        //    return View(roleDTO);
        //}

        //// GET: RoleMasters/Details/5
        //public async Task<ActionResult> Details(Guid? id)
        //{
        //    RoleService rs = new RoleService();
        //    var rm = await rs.Details(id);
        //    return View(rm);
        //}

        public ActionResult ScreenPermission(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                Id = Guid.Empty.ToString();

            ViewBag.RoleId = Id;
            return View();
        }

        //[HttpPost]
        //public ActionResult ScreenPermission(FormCollection fcollection)
        //{
        //    RoleService roleService = new RoleService();
        //    //var role = roleService.GetRole(roleId ?? Guid.NewGuid());
        //    //return View(role);
        //    var UserId = Guid.Empty;
        //    if (Session["UserId"] != null) {
        //        UserId = Guid.Parse(Session["UserId"].ToString());
        //    }

        //    string sResult = roleService.ScreenPermission(fcollection, UserId);
        //    //oRoleModel = oRoleTrans.GetRoleListByID(Convert.ToInt32(fcollection["hdnRoleID"]));
        //    //oRoleModel.RoleName = oRoleModel.RoleName;
        //    TempData["ReturnMessage"] = sResult;
        //    return RedirectToAction("ScreenPermission", "RoleMasters", new { roleId = Guid.Parse(fcollection["hdnRoleID"].ToString()) });

        //}

        //// GET: RoleMasters/Create
        //public ActionResult Create()
        //{
        //    //ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes");
        //    return View();
        //}

        //// POST: RoleMasters/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create( RoleDTO roleMaster)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        RoleService roleService = new RoleService();
        //      await roleService.Create(roleMaster);
        //        return RedirectToAction("Index");
        //    }

        //    //ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes", roleMaster.StatusCodeId);
        //    return View(roleMaster);
        //}

        //// GET: RoleMasters/Edit/5
        //public async Task<ActionResult> Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    RoleService roleService = new RoleService();
        //    RoleDTO roleMaster =await roleService.Details(id);
        //    if (roleMaster == null)
        //    {
        //        return HttpNotFound();
        //    }
        //  //  ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes", roleMaster.StatusCodeId);
        //    return View(roleMaster);
        //}

        //// POST: RoleMasters/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit(RoleDTO roleMaster)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        RoleService roleService = new RoleService();
        //        await roleService.Edit(roleMaster);
        //        return RedirectToAction("Index");
        //    }
        //    //ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes", roleMaster.StatusCodeId);
        //    return View(roleMaster);
        //}
    }
}
