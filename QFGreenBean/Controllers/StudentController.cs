using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QFGreenBean.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace QFGreenBean.Controllers
{
    public class StudentController : Controller
    {
        private PlannerDbEntities db = new PlannerDbEntities();
        public static Boolean IsLoggedIn { get; set; } = false;
        public static int? LoggedInStudentID { get; private set; }

        // GET: Student
        public ActionResult Index()
        {

            //Student student;
            //if (StudentController.IsLoggedIn)
            //{
            //    student = db.Students.Find(StudentController.LoggedInStudentID);
            //} else
            //{
            //    return RedirectToAction("LogIn", "Student");
            //}
            //return View(student);

            return View(db.Students.ToList());
        }

        // GET: Student/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StudentId,StudentNumber,UserName,Password,FirstName,LastName,Telephone,Email,UserType")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: Student/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StudentId,StudentNumber,UserName,Password,FirstName,LastName,Telephone,Email,UserType")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Student/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: /Student/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Student/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Student student)
        {
            if (ModelState.IsValid)
            {
                var user = db.Students.Where(model => (model.StudentNumber == student.StudentNumber) && (model.Password == student.Password)).FirstOrDefault();
                Student s = (Student)user;
                if (user != null)
                {
                    Session["StudentID"] = s.StudentId.ToString();
                    LoggedInStudentID = Convert.ToInt32(Session["StudentID"].ToString());
                    Session["StudentNumber"] = s.StudentNumber.ToString();
                    IsLoggedIn = true;
                }
                else
                {
                    return RedirectToAction("FailLogin");
                }
            }

            return RedirectToAction("Index", "");
        }

        //
        // POST: /Student/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session["StudentID"] = "";
            LoggedInStudentID = null;
            Session["StudentNumber"] = "";
            IsLoggedIn = false;
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult FailLogin()
        {
            return View();
        }

    }
}
