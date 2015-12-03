using QFGreenBean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QFGreenBean.Helpers
{
    public class Scheduler
    {

        //Utils
        private PlannerDbEntities db = new PlannerDbEntities();

        //Input
        private Student Student;
        private List<Course> CoursesTaken;
        private ProgramSequence MasterSequence;

        //Helpful Structures
        private ProgramSequence LocalSequence; /* Output */
        private List<Course> PrereqsMissing;

        //Code
        public Scheduler(Student Student, ProgramSequence MasterSequence)
        {
            this.Student = Student;
            this.MasterSequence = MasterSequence;
            this.LocalSequence = ProgramSequence.DeepClone(MasterSequence);
            this.CoursesTaken = new List<Course>();
            this.PrereqsMissing = new List<Course>();


            foreach (CourseTaken s in db.Students.Find(Student.StudentId).CourseTakens.ToList())
                CoursesTaken.Add(s.Section.Course);
        }

        public void GenerateSchedule()
        {
            foreach (var year in MasterSequence.YearToListOfCourses)
            {
                foreach (KeyValuePair<string, Semester> course in year)
                {
                    //The Student already took the course, remove it 
                    if (CoursesTaken.Contains(CourseByCode(course.Key)))
                    {
                        LocalSequence.RemoveCourse(course.Key);
                        System.Diagnostics.Debug.WriteLine("!!!ALREADY TOOK!!! ~ " + course.Key.ToString());
                        continue;
                    }

                    Course Prereq1 = null;
                    Course Prereq2 = null;
                    try
                    {
                        Prereq1 = CourseByCode(CourseByCode(course.Key).Prerequisite1);
                        Prereq2 = CourseByCode(CourseByCode(course.Key).Prerequisite2);
                    }
                    catch (NullReferenceException) { }


                    if (Prereq1 != null && Prereq2 != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Prereqs Missing For ({0}), Prereqs are {1}, {2} ", course.Key, Prereq1.Name.ToString(), Prereq2.Name.ToString());
                        if (!CoursesTaken.Contains(Prereq1))
                        {
                            PrereqsMissing.Add(CourseByCode(course.Key));
                        }
                        if (!CoursesTaken.Contains(Prereq2))
                        {
                            PrereqsMissing.Add(CourseByCode(course.Key));
                        }
                    }
                    else if (Prereq1 != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Prereq Missing For ({0}), Prereq is {1}", course.Key, Prereq1.Name.ToString());
                        if (!CoursesTaken.Contains(Prereq1))
                        {
                            PrereqsMissing.Add(CourseByCode(course.Key));
                        }
                    }
                    else if (Prereq2 != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Prereq Missing For ({0}), Prereq is {1} ", course.Key, Prereq2.Name.ToString());
                        if (!CoursesTaken.Contains(Prereq2))
                        {
                            PrereqsMissing.Add(CourseByCode(course.Key));
                        }
                    }

                }

                //Now that the year has been sifted, assume the student passes all his/her classes
                AddTakenCourses(year);
            }

        }

        private void AddTakenCourses(Dictionary<string, Semester> year)
        {
            foreach (KeyValuePair<string, Semester> course in year)
            {
                System.Diagnostics.Debug.WriteLine("Course " + course.Key + " was taken.");
                CoursesTaken.Add(CourseByCode(course.Key));
            }

        }

        private Course CourseByCode(string Code)
        {
            return db.Courses.Where(x => (x.Code == Code)).FirstOrDefault();
        }

        public void DebugPrint()
        {
            int c = 1;
            foreach (var year in LocalSequence.YearToListOfCourses)
            {

                System.Diagnostics.Debug.WriteLine("Year: " + c.ToString());
                foreach (KeyValuePair<string, Semester> course in year)
                {
                    System.Diagnostics.Debug.WriteLine("\tKey is " + course.Key.ToString());
                }
                c++;
            }
        }
    }
}
