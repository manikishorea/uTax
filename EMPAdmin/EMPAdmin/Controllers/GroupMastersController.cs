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
    public class GroupMastersController : BaseController
    {

        // GET: GroupMasters
        public async Task<ActionResult> Index()
        {
            var groupMasters = db.GroupMasters;
            return View(await groupMasters.ToListAsync());
        }

        // GET: GroupMasters/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupMaster groupMaster = await db.GroupMasters.FindAsync(id);
            if (groupMaster == null)
            {
                return HttpNotFound();
            }
            return View(groupMaster);
        }

        // GET: GroupMasters/Create
        public ActionResult Create()
        {
           // ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes");
            return View();
        }

        // POST: GroupMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,CreatedBy,CreatedDate,LastUpdatedDate,LastUpdatedBy,StatusCodeId")] GroupMaster groupMaster)
        {
            if (ModelState.IsValid)
            {
                groupMaster.Id = Guid.NewGuid();
                db.GroupMasters.Add(groupMaster);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

           // ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes", groupMaster.StatusCodeId);
            return View(groupMaster);
        }

        // GET: GroupMasters/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupMaster groupMaster = await db.GroupMasters.FindAsync(id);
            if (groupMaster == null)
            {
                return HttpNotFound();
            }
            //ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes", groupMaster.StatusCodeId);
            return View(groupMaster);
        }

        // POST: GroupMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,CreatedBy,CreatedDate,LastUpdatedDate,LastUpdatedBy,StatusCodeId")] GroupMaster groupMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(groupMaster).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
           // ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes", groupMaster.StatusCodeId);
            return View(groupMaster);
        }

        // GET: GroupMasters/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupMaster groupMaster = await db.GroupMasters.FindAsync(id);
            if (groupMaster == null)
            {
                return HttpNotFound();
            }
            return View(groupMaster);
        }

        // POST: GroupMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            GroupMaster groupMaster = await db.GroupMasters.FindAsync(id);
            db.GroupMasters.Remove(groupMaster);
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
