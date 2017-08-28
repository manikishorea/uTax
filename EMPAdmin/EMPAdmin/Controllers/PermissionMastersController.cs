using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace uTax.Portal.Controllers
{
    public class PermissionMastersController : Controller
    {
        // GET: PermissionMasters
        public async Task<ActionResult> Index()
        {
            var permissionMasters = db.PermissionMasters;
            return View(await permissionMasters.ToListAsync());
        }

        // GET: PermissionMasters/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PermissionMaster permissionMaster = await db.PermissionMasters.FindAsync(id);
            if (permissionMaster == null)
            {
                return HttpNotFound();
            }
            return View(permissionMaster);
        }

        // GET: PermissionMasters/Create
        public ActionResult Create()
        {
            //ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes");
            return View();
        }

        // POST: PermissionMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,CreatedBy,CreatedDate,LastUpdatedDate,LastUpdatedBy,StatusCodeId")] PermissionMaster permissionMaster)
        {
            if (ModelState.IsValid)
            {
                permissionMaster.Id = Guid.NewGuid();
                db.PermissionMasters.Add(permissionMaster);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            //ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes", permissionMaster.StatusCodeId);
            return View(permissionMaster);
        }

        // GET: PermissionMasters/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PermissionMaster permissionMaster = await db.PermissionMasters.FindAsync(id);
            if (permissionMaster == null)
            {
                return HttpNotFound();
            }
           // ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes", permissionMaster.StatusCodeId);
            return View(permissionMaster);
        }

        // POST: PermissionMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,CreatedBy,CreatedDate,LastUpdatedDate,LastUpdatedBy,StatusCodeId")] PermissionMaster permissionMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(permissionMaster).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
           // ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes", permissionMaster.StatusCodeId);
            return View(permissionMaster);
        }

        // GET: PermissionMasters/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PermissionMaster permissionMaster = await db.PermissionMasters.FindAsync(id);
            if (permissionMaster == null)
            {
                return HttpNotFound();
            }
            return View(permissionMaster);
        }

        // POST: PermissionMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            PermissionMaster permissionMaster = await db.PermissionMasters.FindAsync(id);
            db.PermissionMasters.Remove(permissionMaster);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
