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
            //int studentID = -1;
            //Int32.TryParse(Session["StudentID"].ToString(), out studentID)
            if (StudentController.IsLoggedIn)
            {
                int? studentID = StudentController.LoggedInStudentID;
                var constraints = listDB.Students.Find(studentID).StudentConstraints.ToList();
                ViewBag.StudentNumber = listDB.Students.Find(studentID).StudentNumber.ToString();
                return View(constraints);
            }
            else
            {
                return RedirectToAction("LogIn", "Student");
            }

        }



        // POST: Constraint/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitConstraint([Bind(Include = "Day,StartHour,EndHour,StartMinute,EndMinute")] StudentConstraint constraint)
        {
            int? id = StudentController.LoggedInStudentID;
            Student s = listDB.Students.Find(id);

            int start = Convert.ToInt32(constraint.StartHour + constraint.StartMinute);
            int end = Convert.ToInt32(constraint.EndHour + constraint.EndMinute);
            if (end <= start)
            {
                ModelState.AddModelError("EndHour", "End hour must be later than start");
            }

            if (ModelState.IsValid)
            {
                s.StudentConstraints.Add(constraint);
                //listDB.StudentConstraints.Add(constraint);
                listDB.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // POST: Constraint/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConstraint(int StudentConstraintId)
        {
            StudentConstraint c = listDB.StudentConstraints.Find(StudentConstraintId);
            listDB.StudentConstraints.Remove(c);
            listDB.SaveChanges();

            return RedirectToAction("Index");
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


    }
}