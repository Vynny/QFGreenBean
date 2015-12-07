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
<<<<<<< HEAD
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
=======
        public ActionResult Index() {
            if (StudentController.IsLoggedIn)
            {
                /*Schedule Generation*/

                //Object containing course sequences, initialize once
                Programs p = new Programs();
                //Scheduler object, first parameter is student (change to logged in student), second parameter is the program option (from Programs object).
                Scheduler s = new Scheduler(db.Students.Find(StudentController.LoggedInStudentID), Programs.SOEN_General);
                //Generate a schedule. Argument is semester. If fall, you get fall and winter schedule. If winter, only winter schedule.
                s.GenerateSchedule(Semester.Winter);
                //Once generated, retrieve the list of scheduled sections. These are the sections to put on the schedule.
                List<Section> sectionsFall = s.ScheduledSectionsFall;
                List<Section> sectionsWinter = s.ScheduledSectionsWinter;

                //Print out the sections for DEBUG 
                foreach (Section sec in sectionsFall)
                {
                    System.Diagnostics.Debug.WriteLine("[Fall] : " + sec.Course.Code + " : " + sec.Name);

                }
                foreach (Section sec in sectionsWinter)
                {
                    System.Diagnostics.Debug.WriteLine("[Winter] : " + sec.Course.Code + " : " + sec.Name);

                }
                /*END Schedule Generation*/
            }
>>>>>>> scheduler in working state
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

        //Place holder for schedule generation
        public ActionResult GenerateSchedule()
        {
            return View("Index");
        }
    }
}