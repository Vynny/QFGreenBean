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
        private Dictionary<string, Semester> b;

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

        public void RemoveCompletedCourses()
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
                    //

                    //The Student is missing a pre-req
                    if (!(CoursesTaken.Contains(CourseByCode(CourseByCode(course.Key).Prerequisite1))))
                    {
                        PrereqsMissing.Add(CourseByCode(CourseByCode(course.Key).Prerequisite1));
                    }
                    if (!(CoursesTaken.Contains(CourseByCode(CourseByCode(course.Key).Prerequisite2))))
                    {
                        PrereqsMissing.Add(CourseByCode(CourseByCode(course.Key).Prerequisite2));
                    }
                    //



                }
            }

        }

        public void aa(List<Dictionary<string,Semester>> year)
        {
            foreach (Dictionary<string, Semester> course in year)
            {

                //The Student is missing a pre-req
                if (!(CoursesTaken.Contains(CourseByCode(CourseByCode(course.Key).Prerequisite1))))
                {
                    PrereqsMissing.Add(CourseByCode(CourseByCode(course.Key).Prerequisite1));
                }
                if (!(CoursesTaken.Contains(CourseByCode(CourseByCode(course.Key).Prerequisite2))))
                {
                    PrereqsMissing.Add(CourseByCode(CourseByCode(course.Key).Prerequisite2));
                }
                //
            }
        }

        private Course CourseByCode(string Code)
        {
            return db.Courses.Where(x => (x.Code == Code)).FirstOrDefault();
        }

        public void DebugPrint()
        {
            int c = 1;
            foreach (var year in MasterSequence.YearToListOfCourses)
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
