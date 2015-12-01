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
    public class FallEventController : Controller
    {
        private PlannerDbEntities db = new PlannerDbEntities();

        // GET: FallEvent
        public ActionResult Index()
        {
            return View(db.FallEvents.ToList());
        }

        // GET: FallEvent/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FallEvent fallEvent = db.FallEvents.Find(id);
            if (fallEvent == null)
            {
                return HttpNotFound();
            }
            return View(fallEvent);
        }

        // GET: FallEvent/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FallEvent/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FallEventId,StartTime,EndTime,Description")] FallEvent fallEvent)
        {
            if (ModelState.IsValid)
            {
                db.FallEvents.Add(fallEvent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fallEvent);
        }

        // GET: FallEvent/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FallEvent fallEvent = db.FallEvents.Find(id);
            if (fallEvent == null)
            {
                return HttpNotFound();
            }
            return View(fallEvent);
        }

        // POST: FallEvent/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FallEventId,StartTime,EndTime,Description")] FallEvent fallEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fallEvent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fallEvent);
        }

        // GET: FallEvent/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FallEvent fallEvent = db.FallEvents.Find(id);
            if (fallEvent == null)
            {
                return HttpNotFound();
            }
            return View(fallEvent);
        }

        // POST: FallEvent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FallEvent fallEvent = db.FallEvents.Find(id);
            db.FallEvents.Remove(fallEvent);
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
