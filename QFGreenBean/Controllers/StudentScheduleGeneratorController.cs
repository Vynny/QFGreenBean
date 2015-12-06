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
    public class StudentScheduleGeneratorController : Controller
    {
        private PlannerDbEntities db = new PlannerDbEntities();

        // GET: StudentScheduleGenerator
        public ActionResult Index()
        {
            return View(db.StudentScheduleGenerators.ToList());
        }

        // GET: StudentScheduleGenerator/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentScheduleGenerator studentScheduleGenerator = db.StudentScheduleGenerators.Find(id);
            if (studentScheduleGenerator == null)
            {
                return HttpNotFound();
            }
            return View(studentScheduleGenerator);
        }

        // GET: StudentScheduleGenerator/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StudentScheduleGenerator/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StudentScheduleGeneratorId,StudentNumber,StartTerm,DepartmentName,ProgramOptionName,IncludeSummer,DateGenerated")] StudentScheduleGenerator studentScheduleGenerator)
        {
            if (ModelState.IsValid)
            {
                studentScheduleGenerator.DateGenerated = DateTime.Now;
                db.StudentScheduleGenerators.Add(studentScheduleGenerator);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(studentScheduleGenerator);
        }

        // GET: StudentScheduleGenerator/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentScheduleGenerator studentScheduleGenerator = db.StudentScheduleGenerators.Find(id);
            if (studentScheduleGenerator == null)
            {
                return HttpNotFound();
            }
            return View(studentScheduleGenerator);
        }

        // POST: StudentScheduleGenerator/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StudentScheduleGeneratorId,StudentNumber,StartTerm,DepartmentName,ProgramOptionName,IncludeSummer,DateGenerated")] StudentScheduleGenerator studentScheduleGenerator)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentScheduleGenerator).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(studentScheduleGenerator);
        }

        // GET: StudentScheduleGenerator/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentScheduleGenerator studentScheduleGenerator = db.StudentScheduleGenerators.Find(id);
            if (studentScheduleGenerator == null)
            {
                return HttpNotFound();
            }
            return View(studentScheduleGenerator);
        }

        // POST: StudentScheduleGenerator/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StudentScheduleGenerator studentScheduleGenerator = db.StudentScheduleGenerators.Find(id);
            db.StudentScheduleGenerators.Remove(studentScheduleGenerator);
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
