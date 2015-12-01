using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QFGreenBean.Models;

namespace QFGreenBean.Controllers
{
    public class SummerEventController : Controller
    {
        private PlannerDbEntities db = new PlannerDbEntities();

        // GET: SummerEvent
        public ActionResult Index()
        {
            return View(db.SummerEvents.ToList());
        }

        // GET: SummerEvent/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SummerEvent summerEvent = db.SummerEvents.Find(id);
            if (summerEvent == null)
            {
                return HttpNotFound();
            }
            return View(summerEvent);
        }

        // GET: SummerEvent/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SummerEvent/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SummerEventId,StartTime,EndTime,Description")] SummerEvent summerEvent)
        {
            if (ModelState.IsValid)
            {
                db.SummerEvents.Add(summerEvent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(summerEvent);
        }

        // GET: SummerEvent/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SummerEvent summerEvent = db.SummerEvents.Find(id);
            if (summerEvent == null)
            {
                return HttpNotFound();
            }
            return View(summerEvent);
        }

        // POST: SummerEvent/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SummerEventId,StartTime,EndTime,Description")] SummerEvent summerEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(summerEvent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(summerEvent);
        }

        // GET: SummerEvent/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SummerEvent summerEvent = db.SummerEvents.Find(id);
            if (summerEvent == null)
            {
                return HttpNotFound();
            }
            return View(summerEvent);
        }

        // POST: SummerEvent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SummerEvent summerEvent = db.SummerEvents.Find(id);
            db.SummerEvents.Remove(summerEvent);
            db.SaveChanges();
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
