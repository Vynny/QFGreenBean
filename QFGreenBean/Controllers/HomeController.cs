using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QFGreenBean.Models;
using QFGreenBean.Helpers;

namespace QFGreenBean.Controllers
{
    public class HomeController : Controller
    {
        private PlannerDbEntities db = new PlannerDbEntities();

        [HttpGet]
        public ActionResult Index()
        {
            //To LAY
            //!! THIS IS HERE FOR TESTING, ONCE LOGGED IN A SCHEDULE WILL BE GENERATED FOR THE LOGGED IN STUDENT
            //RUN WITH DEBUG AND LOOK AT OUTPUT CONSOLE FOR PRINT OUTS !!
            //THIS CODE HAS BEEN PASTED INTO THE INDEX POST BUT NEEDS MODIFICATION
            if (StudentController.IsLoggedIn)
            {
                /*
                //Object containing course sequences, initialize once
                Programs p = new Programs();
                //Scheduler object, first parameter is student (change to logged in student), second parameter is the program option (from Programs object).

                //LAY: Student should work, add programs to Programs object and insert into second parameter
                Scheduler s = new Scheduler(db.Students.Find(StudentController.LoggedInStudentID), Programs.SOEN_General);

                //Generate a schedule. Argument is semester. If fall, you get fall and winter schedule. If winter, only winter schedule
                s.GenerateSchedule(Semester.Fall); //LAY: if starting term is fall, put Semester.Fall. If winter, put Semester.Winter

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
                */

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

                foreach (var item in db.StudentSchedules)
                {
                    db.StudentSchedules.Remove(item);
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
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "StudentScheduleGeneratorId,StudentNumber,StartTerm,DepartmentName,ProgramOptionName,IncludeSummer,DateGenerated")] StudentScheduleGenerator generator)
        {

            int? studentId = StudentController.LoggedInStudentID;
            generator.StudentNumber = db.Students.Find(studentId).StudentNumber;
            generator.DateGenerated = DateTime.Now;
            db.StudentScheduleGenerators.Add(generator);
           // db.SaveChanges();

            //Sylvain Code Here

            /*Schedule Generation*/

            //Object containing course sequences, initialize once
            Programs p = new Programs();
            //Scheduler object, first parameter is student (change to logged in student), second parameter is the program option (from Programs object).

            //LAY: Student should work, add programs to Programs object and insert into second parameter
            Scheduler s = new Scheduler(db.Students.Find(StudentController.LoggedInStudentID), Programs.SOEN_General);

            //Generate a schedule. Argument is semester. If fall, you get fall and winter schedule. If winter, only winter schedule
            if (generator.StartTerm == "Fall 2015")
            {
                s.GenerateSchedule(Semester.Fall); //LAY: if starting term is fall, put Semester.Fall. If winter, put Semester.Winter
            }
            else if (generator.StartTerm == "Winter 2016")
            {
                s.GenerateSchedule(Semester.Winter);
            }
            else
            {
                s.GenerateSchedule(Semester.Summer);
            }

            //Once generated, retrieve the list of scheduled sections. These are the sections to put on the schedule.
            List<Section> sectionsFall = s.ScheduledSectionsFall;
            List<Section> sectionsWinter = s.ScheduledSectionsWinter;
            List<StudentSchedule> scheduleList = new List<StudentSchedule>();
            
            foreach (var item in sectionsFall)
            {
                StudentSchedule schedule = new StudentSchedule();

                schedule.StudentId = studentId;
                schedule.SectionId = item.SectionId;

                scheduleList.Add(schedule);
            }

            foreach (var item in sectionsWinter)
            {
                StudentSchedule schedule = new StudentSchedule();

                schedule.StudentId = studentId;
                schedule.SectionId = item.SectionId;

                scheduleList.Add(schedule);
            }

            db.StudentSchedules.AddRange(scheduleList);
            db.SaveChanges();

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
            }
            /*END Schedule Generation*/
            return View("Index");
        }


    }
}