﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QFGreenBean.Models;
using QFGreenBean.Utils;

namespace QFGreenBean.Controllers
{
    public class StudentScheduleController : Controller
    {
        private PlannerDbEntities db = new PlannerDbEntities();

        // GET: StudentSchedule
        public ActionResult Index()
        {
            // Reset event tables
            if (ModelState.IsValid)
            {
                foreach (var item in db.FallEvents)
                {
                    db.FallEvents.Remove(item);
                }

                foreach (var item in db.WinterEvents)
                {
                    db.WinterEvents.Remove(item);
                }

                foreach (var item in db.SummerEvents)
                {
                    db.SummerEvents.Remove(item);
                }
                db.SaveChanges();
   
                // Fill event table for calendar view
                foreach(var item in db.StudentSchedules)
                {
                    if (item.Section.Term == "Fall")
                    {
                        FallEvent event1 = new FallEvent();
                        FallEvent event2 = new FallEvent();

                        if ((item.Section.StartDayTime1 != null) && (item.Section.EndDayTime1 != null))
                        {
                            DateTime startDayTime1 = (System.DateTime)item.Section.StartDayTime1;
                            DateTime endDayTime1 = (System.DateTime)item.Section.EndDayTime1;

                            event1.StartTime = startDayTime1.EqualTodayWeekDayTime();
                            event1.EndTime = endDayTime1.EqualTodayWeekDayTime();
                            event1.Description = item.Section.Course.Code + " - " + item.Section.Course.Name + "<br/>" + item.Section.Room;
                            db.FallEvents.Add(event1);
                        }

                        if ((item.Section.StartDayTime2 != null) && (item.Section.EndDayTime2 != null))
                        {
                            DateTime startDayTime2 = (System.DateTime)item.Section.StartDayTime2;
                            DateTime endDayTime2 = (System.DateTime)item.Section.EndDayTime2;

                            event2.StartTime = startDayTime2.EqualTodayWeekDayTime();
                            event2.EndTime = endDayTime2.EqualTodayWeekDayTime();
                            event2.Description = item.Section.Course.Code + " - " + item.Section.Course.Name + "<br/>" + item.Section.Room;
                            db.FallEvents.Add(event2);
                        }
                    }
                    else if (item.Section.Term == "Winter")
                    {
                        WinterEvent event1 = new WinterEvent();
                        WinterEvent event2 = new WinterEvent();

                        if ((item.Section.StartDayTime1 != null) && (item.Section.EndDayTime1 != null))
                        {
                            DateTime startDayTime1 = (System.DateTime)item.Section.StartDayTime1;
                            DateTime endDayTime1 = (System.DateTime)item.Section.EndDayTime1;

                            event1.StartTime = startDayTime1.EqualTodayWeekDayTime();
                            event1.EndTime = endDayTime1.EqualTodayWeekDayTime();
                            event1.Description = item.Section.Course.Code + " - " + item.Section.Course.Name + "<br/>" + item.Section.Room;
                            db.WinterEvents.Add(event1);
                        }

                        if ((item.Section.StartDayTime2 != null) && (item.Section.EndDayTime2 != null))
                        {
                            DateTime startDayTime2 = (System.DateTime)item.Section.StartDayTime2;
                            DateTime endDayTime2 = (System.DateTime)item.Section.EndDayTime2;

                            event2.StartTime = startDayTime2.EqualTodayWeekDayTime();
                            event2.EndTime = endDayTime2.EqualTodayWeekDayTime();
                            event2.Description = item.Section.Course.Code + " - " + item.Section.Course.Name + "<br/>" + item.Section.Room;
                            db.WinterEvents.Add(event2);
                        }
                    }
                    else if (item.Section.Term == "Summer")
                    {
                        SummerEvent event1 = new SummerEvent();
                        SummerEvent event2 = new SummerEvent();

                        if ((item.Section.StartDayTime1 != null) && (item.Section.EndDayTime1 != null))
                        {
                            DateTime startDayTime1 = (System.DateTime)item.Section.StartDayTime1;
                            DateTime endDayTime1 = (System.DateTime)item.Section.EndDayTime1;

                            event1.StartTime = startDayTime1.EqualTodayWeekDayTime();
                            event1.EndTime = endDayTime1.EqualTodayWeekDayTime();
                            event1.Description = item.Section.Course.Code + " - " + item.Section.Course.Name + "<br/>" + item.Section.Room;
                            db.SummerEvents.Add(event1);
                        }

                        if ((item.Section.StartDayTime2 != null) && (item.Section.EndDayTime2 != null))
                        {
                            DateTime startDayTime2 = (System.DateTime)item.Section.StartDayTime2;
                            DateTime endDayTime2 = (System.DateTime)item.Section.EndDayTime2;

                            event2.StartTime = startDayTime2.EqualTodayWeekDayTime();
                            event2.EndTime = endDayTime2.EqualTodayWeekDayTime();
                            event2.Description = item.Section.Course.Code + " - " + item.Section.Course.Name + "<br/>" + item.Section.Room;
                            db.SummerEvents.Add(event2);
                        }

                    } // end of else if (item.Section.Term == "Summer")
                } // end of foreach(var item in db.StudentSchedules)

                db.SaveChanges();

            }

            var studentSchedules = db.StudentSchedules.Include(s => s.Section).Include(s => s.Student);
            return View(studentSchedules.ToList());
        }

        // GET: StudentSchedule/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentSchedule studentSchedule = db.StudentSchedules.Find(id);
            if (studentSchedule == null)
            {
                return HttpNotFound();
            }
            return View(studentSchedule);
        }

        // GET: StudentSchedule/Create
        public ActionResult Create()
        {
            ViewBag.SectionId = new SelectList(db.Sections, "SectionId", "Name");
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "StudentNumber");
 
            return View();
        }

        // POST: StudentSchedule/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StudentScheduleId,StudentId,SectionId")] StudentSchedule studentSchedule)
        {
            if (ModelState.IsValid)
            {
                db.StudentSchedules.Add(studentSchedule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SectionId = new SelectList(db.Sections, "SectionId", "Name", studentSchedule.SectionId);
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "StudentNumber", studentSchedule.StudentId);
            return View(studentSchedule);
        }

        // GET: StudentSchedule/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentSchedule studentSchedule = db.StudentSchedules.Find(id);
            if (studentSchedule == null)
            {
                return HttpNotFound();
            }
            ViewBag.SectionId = new SelectList(db.Sections, "SectionId", "Name", studentSchedule.SectionId);
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "StudentNumber", studentSchedule.StudentId);
            return View(studentSchedule);
        }

        // POST: StudentSchedule/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StudentScheduleId,StudentId,SectionId")] StudentSchedule studentSchedule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(studentSchedule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SectionId = new SelectList(db.Sections, "SectionId", "Name", studentSchedule.SectionId);
            ViewBag.StudentId = new SelectList(db.Students, "StudentId", "StudentNumber", studentSchedule.StudentId);
            return View(studentSchedule);
        }

        // GET: StudentSchedule/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StudentSchedule studentSchedule = db.StudentSchedules.Find(id);
            if (studentSchedule == null)
            {
                return HttpNotFound();
            }
            return View(studentSchedule);
        }

        // POST: StudentSchedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StudentSchedule studentSchedule = db.StudentSchedules.Find(id);
            db.StudentSchedules.Remove(studentSchedule);
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
