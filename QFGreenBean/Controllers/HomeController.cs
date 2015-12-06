using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QFGreenBean.Models;
<<<<<<< HEAD
using QFGreenBean.Utils;
=======
using QFGreenBean.Helpers;
>>>>>>> before list change

namespace QFGreenBean.Controllers
{
    public class HomeController : Controller
    {
        private PlannerDbEntities db = new PlannerDbEntities();

        [HttpGet]
        public ActionResult Index()
        {
<<<<<<< HEAD
            if (StudentController.IsLoggedIn)
            {
                int? studentId = StudentController.LoggedInStudentID;
                ViewBag.StudentNumber = db.Students.Find(studentId).StudentNumber;

                foreach (var item in db.FallEvents)
                {
                    db.FallEvents.Remove(item);
                }
                foreach (var item in db.WinterEvents)
                {
                    db.WinterEvents.Remove(item);
                }

                foreach (var item in db.StudentScheduleGenerators)
                {
                    db.StudentScheduleGenerators.Remove(item);
                }

                db.SaveChanges();
 
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Student");
            }
=======
            Programs p = new Programs();
            Scheduler s = new Scheduler(db.Students.Find(7), Programs.SOEN_General);
            s.GenerateSchedule(Semester.Fall);
            s.DebugPrint();
            s.PrintList(s.CoursesToTake, "CoursesToTake");
            s.GenerateSectionList(Semester.Fall);
            return View();
>>>>>>> before list change
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "StudentScheduleGeneratorId,StudentNumber,StartTerm,DepartmentName,ProgramOptionName,IncludeSummer,DateGenerated")] StudentScheduleGenerator generator)
        {

            int? studentId = StudentController.LoggedInStudentID;
            generator.StudentNumber = db.Students.Find(studentId).StudentNumber;
            generator.DateGenerated = DateTime.Now;
            db.StudentScheduleGenerators.Add(generator);
            db.SaveChanges();

            // return View("DisplayCurrentYearSchedules", generator);
            return RedirectToAction("Index", "StudentSchedule");
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}