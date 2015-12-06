using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QFGreenBean.Models;

namespace QFGreenBean.Controllers
{
    public class HomeController : Controller
    {
        private PlannerDbEntities db = new PlannerDbEntities();

        [HttpGet]
        public ActionResult Index()
        {
            if (StudentController.IsLoggedIn)
            {
                int? studentId = StudentController.LoggedInStudentID;
                ViewBag.StudentNumber = db.Students.Find(studentId).StudentNumber;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Student");
            }
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "StudentScheduleGeneratorId,StudentNumber,StartTerm,DepartmentName,ProgramOptionName,IncludeSummer,DateGenerated")] StudentScheduleGenerator generator)
        {
            if (ModelState.IsValid)
            {
                if (StudentController.IsLoggedIn)
                {
                    int? studentId = StudentController.LoggedInStudentID;
                    generator.StudentNumber = db.Students.Find(studentId).StudentNumber;
                    generator.DateGenerated = DateTime.Now;
                    db.StudentScheduleGenerators.Add(generator);
                    db.SaveChanges();
                }
            }

            return View("DisplayCurrentYearSchedules", generator);
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