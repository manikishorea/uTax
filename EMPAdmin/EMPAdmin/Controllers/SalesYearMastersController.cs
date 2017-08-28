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
    public class SalesYearMastersController : BaseController
    {
        private uTaxDBEntities db = new uTaxDBEntities();

        // GET: SalesYearMasters
        public async Task<ActionResult> Index()
        {
            var salesYearMasters = db.SalesYearMasters;
            return View(await salesYearMasters.ToListAsync());
        }

        // GET: SalesYearMasters/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesYearMaster salesYearMaster = await db.SalesYearMasters.FindAsync(id);
            if (salesYearMaster == null)
            {
                return HttpNotFound();
            }
            return View(salesYearMaster);
        }

        // GET: SalesYearMasters/Create
        public ActionResult Create()
        {
            //Presently there are no values in Entity master table 
            //ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes");
            // ViewBag.EntityMaster = db.EntityMasters.ToList();
            return View();
        }

        // POST: SalesYearMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,SalesYear,ApplicableFromDate,ApplicableToDate,Description,CreatedBy,CreatedDate,LastUpdatedDate,LastUpdatedBy,StatusCodeId")] SalesYearMaster salesYearMaster)
        {
            if (ModelState.IsValid)
            {
                salesYearMaster.Id = Guid.NewGuid();
                db.SalesYearMasters.Add(salesYearMaster);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            //ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes", salesYearMaster.StatusCodeId);
            return View(salesYearMaster);
        }

        // GET: SalesYearMasters/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesYearMaster salesYearMaster = await db.SalesYearMasters.FindAsync(id);
            if (salesYearMaster == null)
            {
                return HttpNotFound();
            }
            //ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes", salesYearMaster.StatusCodeId);
            return View(salesYearMaster);
        }

        // POST: SalesYearMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,SalesYear,ApplicableFromDate,ApplicableToDate,Description,CreatedBy,CreatedDate,LastUpdatedDate,LastUpdatedBy,StatusCodeId")] SalesYearMaster salesYearMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(salesYearMaster).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            //ViewBag.StatusCodeId = new SelectList(db.StatusCodeMasters, "Id", "StatusCodes", salesYearMaster.StatusCodeId);
            return View(salesYearMaster);
        }

        // GET: SalesYearMasters/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesYearMaster salesYearMaster = await db.SalesYearMasters.FindAsync(id);
            if (salesYearMaster == null)
            {
                return HttpNotFound();
            }
            return View(salesYearMaster);
        }

        // POST: SalesYearMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            SalesYearMaster salesYearMaster = await db.SalesYearMasters.FindAsync(id);
            db.SalesYearMasters.Remove(salesYearMaster);
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
