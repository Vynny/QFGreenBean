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
    public class CourseTakenController : Controller
    {
        private PlannerDbEntities db = new PlannerDbEntities();

        // GET: CourseTaken
        public ActionResult Index()
        {
            var courseTakens = db.CourseTakens.Include(c => c.Section).Include(c => c.Student);
            return View(courseTakens.ToList());
        }

        // GET: CourseTaken/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseTaken courseTaken = db.CourseTakens.Find(id);
            if (courseTaken == null)
            {
                return HttpNotFound();
            }
            return View(courseTaken);
        }

        // GET: CourseTaken/Create
        public ActionResult Create()
        {
            ViewBag.SectionId = new SelectList(db.Sections, "SectionId", "Name");
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "StudentNumber");
            return View();
        }

        // POST: CourseTaken/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseTakenId,StudentId,SectionId")] CourseTaken courseTaken)
        {
            if (ModelState.IsValid)
            {
                db.CourseTakens.Add(courseTaken);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SectionId = new SelectList(db.Sections, "SectionId", "Name", courseTaken.SectionId);
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "StudentNumber", courseTaken.StudentId);
            return View(courseTaken);
        }

        // GET: CourseTaken/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseTaken courseTaken = db.CourseTakens.Find(id);
            if (courseTaken == null)
            {
                return HttpNotFound();
            }
            ViewBag.SectionId = new SelectList(db.Sections, "SectionId", "Name", courseTaken.SectionId);
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "StudentNumber", courseTaken.StudentId);
            return View(courseTaken);
        }

        // POST: CourseTaken/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CourseTakenId,StudentId,SectionId")] CourseTaken courseTaken)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseTaken).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SectionId = new SelectList(db.Sections, "SectionId", "Name", courseTaken.SectionId);
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "StudentNumber", courseTaken.StudentId);
            return View(courseTaken);
        }

        // GET: CourseTaken/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseTaken courseTaken = db.CourseTakens.Find(id);
            if (courseTaken == null)
            {
                return HttpNotFound();
            }
            return View(courseTaken);
        }

        // POST: CourseTaken/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseTaken courseTaken = db.CourseTakens.Find(id);
            db.CourseTakens.Remove(courseTaken);
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
