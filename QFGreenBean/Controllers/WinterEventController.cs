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
    public class WinterEventController : Controller
    {
        private PlannerDbEntities db = new PlannerDbEntities();

        // GET: WinterEvent
        public ActionResult Index()
        {
            return View(db.WinterEvents.ToList());
        }

        // GET: WinterEvent/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WinterEvent winterEvent = db.WinterEvents.Find(id);
            if (winterEvent == null)
            {
                return HttpNotFound();
            }
            return View(winterEvent);
        }

        // GET: WinterEvent/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WinterEvent/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WinterEventId,StartTime,EndTime,Description")] WinterEvent winterEvent)
        {
            if (ModelState.IsValid)
            {
                db.WinterEvents.Add(winterEvent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(winterEvent);
        }

        // GET: WinterEvent/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WinterEvent winterEvent = db.WinterEvents.Find(id);
            if (winterEvent == null)
            {
                return HttpNotFound();
            }
            return View(winterEvent);
        }

        // POST: WinterEvent/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WinterEventId,StartTime,EndTime,Description")] WinterEvent winterEvent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(winterEvent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(winterEvent);
        }

        // GET: WinterEvent/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WinterEvent winterEvent = db.WinterEvents.Find(id);
            if (winterEvent == null)
            {
                return HttpNotFound();
            }
            return View(winterEvent);
        }

        // POST: WinterEvent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WinterEvent winterEvent = db.WinterEvents.Find(id);
            db.WinterEvents.Remove(winterEvent);
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
