using QFGreenBean.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QFGreenBean.Controllers
{
    public class ConstraintController : Controller
    {
        private PlannerDbEntities listDB = new PlannerDbEntities();
        // GET: Constraint
        public ActionResult Index()
        {
            return View();
        }


        // GET: Constraint
        public ActionResult Add()
        {
            return View();
        }

        // POST: Constraint/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitConstraint([Bind(Include = "Day,StartHour,EndHour,StartMinute,EndMinute")] StudentConstraint constraint)
        {

            int start = Convert.ToInt32(constraint.StartHour + constraint.StartMinute);
            int end = Convert.ToInt32(constraint.EndHour + constraint.EndMinute);
            if (end <= start)
            {
                ModelState.AddModelError("EndHour", "End hour must be later than start");
            }

            if (ModelState.IsValid)
            {
                listDB.StudentConstraints.Add(constraint);
                listDB.SaveChanges();
            }

            return RedirectToAction("ViewConstraints");
        }

        // POST: Constraint/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConstraint(int StudentConstraintId)
        {
            StudentConstraint c = listDB.StudentConstraints.Find(StudentConstraintId);
            listDB.StudentConstraints.Remove(c);
            listDB.SaveChanges();

            return RedirectToAction("ViewConstraints");
        }

        public string TestConstraint(StudentConstraint c)
        {
            string s = "Value of WeekDay is: " + c.Day.ToLower() + "\n";
            s += "startHour: " + c.StartHour + "\n";
            s += "startMinute: " + c.StartMinute + "\n";
            s += "endHour: " + c.EndHour + "\n";
            s += "endMinute: " + c.EndMinute + "\n";
            return s;
        }

        // GET: Constraint
        public ActionResult ViewConstraints()
        {
            return View(listDB.StudentConstraints.ToList());
        }

    }
}