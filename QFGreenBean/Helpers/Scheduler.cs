using QFGreenBean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QFGreenBean.Helpers
{
    public class Scheduler
    {

        /*Utils*/
        private PlannerDbEntities db = new PlannerDbEntities();//Database to retreive data

        /*Input*/
        private Student Student;//The Student
        private List<Course> CoursesTaken;//The Courses the Student has already taken
        private ProgramSequence MasterSequence;//The master sequence, containing the recommended sequence for a particular program and choice

        /*Helpful Structures*/
        private ProgramSequence LocalSequence;//A DEEP copy of the master sequence, used to track user's schedule throughout generation. The final result is the list of courses that must be added to the schedule"
        private List<Course> PrereqsMissing;//Courses are added to this list when prereqs for it are missing (ex: Student did COMP249 but not COMP232, COMP352 would be added to this list when encountered


        public Scheduler(Student Student, ProgramSequence MasterSequence)
        {
            this.Student = Student;
            this.MasterSequence = MasterSequence;
            LocalSequence = ProgramSequence.DeepClone(MasterSequence);
            CoursesTaken = new List<Course>();
            PrereqsMissing = new List<Course>();

            foreach (CourseTaken s in db.Students.Find(Student.StudentId).CourseTakens.ToList())
                CoursesTaken.Add(s.Section.Course);
        }

        public void GenerateSchedule(int year)
        {
            List<string> YearMap = MasterSequence.YearToListOfCourses[year];
            bool SwitchSemester = false;

            foreach (string course in YearMap)
            {
                if (MasterSequence.getSemester(course) == Semester.WINTER && !SwitchSemester)
                {
                    SwitchSemester = true;
                    AddTakenCourses(YearMap, Semester.FALL);
                }

                //The Student already took the course, remove it 
                if (CoursesTaken.Contains(CourseByCode(course)))
                {
                    LocalSequence.RemoveCourse(course);
                    System.Diagnostics.Debug.WriteLine("[ALREADY TOOK]: " + course.ToString());
                    continue;
                }

                //Check if course has prerequisites
                Course Prereq1 = null;
                Course Prereq2 = null;
                try
                {
                    Prereq1 = CourseByCode(CourseByCode(course).Prerequisite1);
                    Prereq2 = CourseByCode(CourseByCode(course).Prerequisite2);
                }
                catch (NullReferenceException) { }

                //Case 1, course has 2 non null prereqs
                if (Prereq1 != null && Prereq2 != null)
                {
                    if (CoursesTaken.Exists(x => x.Code == Prereq1.Code))
                    {
                        PrereqsMissing.Add(CourseByCode(course));
                    }
                    if (!CoursesTaken.Exists(x => x.Code == Prereq2.Code))
                    {
                        PrereqsMissing.Add(CourseByCode(course));
                    }
                }
                //Case 2: Only prereq1 present
                else if (Prereq1 != null)
                {
                    if (!CoursesTaken.Exists(x => x.Code == Prereq1.Code))
                    {
                        PrereqsMissing.Add(CourseByCode(course));
                    }
                }
                //Case 3: Only prereq2 present
                else if (Prereq2 != null)
                {
                    if (!CoursesTaken.Exists(x => x.Code == Prereq2.Code))
                    {
                        PrereqsMissing.Add(CourseByCode(course));
                    }
                }
            }

            //Now that the year has been sifted, assume the student passes all his/her classes
            AddTakenCourses(YearMap, Semester.WINTER);
            //Update the local sequence
            UpdateLocal();
        }

        private void AddTakenCourses(List<string> year, Semester s)
        {
            System.Diagnostics.Debug.WriteLine("Adding courses for " + s.ToString());
            foreach (string course in year)
            {
                if (MasterSequence.getSemester(course) == s)
                {
                    System.Diagnostics.Debug.WriteLine("Course " + course + " was taken.");
                    CoursesTaken.Add(CourseByCode(course));
                }
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
                foreach (string course in year)
                {
                    System.Diagnostics.Debug.WriteLine("\tKey is " + course);
                }
                c++;
            }
        }

        public void PrintCoursesTaken()
        {
            foreach (Course c in CoursesTaken)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("[TAKEN-LIST]: " + c.Code);
                }
                catch (NullReferenceException) { }
            }
        }

        private void UpdateLocal()
        {
            foreach (var year in LocalSequence.YearToListOfCourses.ToList())
            {
                foreach (string course in year.ToList())
                {
                    if (CoursesTaken.Contains(CourseByCode(course)))
                        LocalSequence.RemoveCourse(course);
                }
            }
        }
    }
}
