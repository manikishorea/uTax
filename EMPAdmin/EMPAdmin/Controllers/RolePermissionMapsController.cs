using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using uTax.EntityFramework.Edmx;

namespace uTax.Portal.Controllers
{
    public class RolePermissionMapsController : BaseController
    {
        private uTaxDBEntities db = new uTaxDBEntities();

        // GET: RolePermissionMaps
        public async Task<ActionResult> Index()
        {
            var rolePermissionMaps = db.RolePermissionMaps.Include(r => r.PermissionMaster).Include(r => r.RoleMaster);
            return View(await rolePermissionMaps.ToListAsync());
        }

        // GET: RolePermissionMaps/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolePermissionMap rolePermissionMap = await db.RolePermissionMaps.FindAsync(id);
            if (rolePermissionMap == null)
            {
                return HttpNotFound();
            }
            return View(rolePermissionMap);
        }

        // GET: RolePermissionMaps/Create
        public ActionResult Create()
        {
            ViewBag.PermissionId = new SelectList(db.PermissionMasters, "Id", "Name");
            ViewBag.RoleId = new SelectList(db.RoleMasters, "Id", "Name");
            return View();
        }

        // POST: RolePermissionMaps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,RoleId,PermissionId,StatusCodeId")] RolePermissionMap rolePermissionMap)
        {
            if (ModelState.IsValid)
            {
                rolePermissionMap.Id = Guid.NewGuid();
                db.RolePermissionMaps.Add(rolePermissionMap);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PermissionId = new SelectList(db.PermissionMasters, "Id", "Name", rolePermissionMap.PermissionId);
            ViewBag.RoleId = new SelectList(db.RoleMasters, "Id", "Name", rolePermissionMap.RoleId);
            return View(rolePermissionMap);
        }

        // GET: RolePermissionMaps/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolePermissionMap rolePermissionMap = await db.RolePermissionMaps.FindAsync(id);
            if (rolePermissionMap == null)
            {
                return HttpNotFound();
            }
            ViewBag.PermissionId = new SelectList(db.PermissionMasters, "Id", "Name", rolePermissionMap.PermissionId);
            ViewBag.RoleId = new SelectList(db.RoleMasters, "Id", "Name", rolePermissionMap.RoleId);
            return View(rolePermissionMap);
        }

        // POST: RolePermissionMaps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,RoleId,PermissionId,StatusCodeId")] RolePermissionMap rolePermissionMap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rolePermissionMap).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PermissionId = new SelectList(db.PermissionMasters, "Id", "Name", rolePermissionMap.PermissionId);
            ViewBag.RoleId = new SelectList(db.RoleMasters, "Id", "Name", rolePermissionMap.RoleId);
            return View(rolePermissionMap);
        }

        // GET: RolePermissionMaps/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolePermissionMap rolePermissionMap = await db.RolePermissionMaps.FindAsync(id);
            if (rolePermissionMap == null)
            {
                return HttpNotFound();
            }
            return View(rolePermissionMap);
        }

        // POST: RolePermissionMaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            RolePermissionMap rolePermissionMap = await db.RolePermissionMaps.FindAsync(id);
            db.RolePermissionMaps.Remove(rolePermissionMap);
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
