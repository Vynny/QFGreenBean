using QFGreenBean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QFGreenBean.Helpers
{
    public class Scheduler
    {

        private int MaxCoursesPerSemester = 5;

        /*Utils*/
        private PlannerDbEntities db = new PlannerDbEntities();//Database to retreive data

        /*Input*/
        private Student Student;//The Student
        private List<Course> CoursesTaken;//The Courses the Student has already taken
        private ProgramSequence MasterSequence;//The master sequence, containing the recommended sequence for a particular program and choice

        /*Helpful Structures*/
        private ProgramSequence LocalSequence;//A DEEP copy of the master sequence, used to track user's schedule throughout generation. The final result is the list of courses that must be added to the schedule"
        private List<Course> PrereqsMissing;//Courses are added to this list when prereqs for it are missing (ex: Student did COMP249 but not COMP232, COMP352 would be added to this list when encountered
        public List<string> CoursesToTake { get; set; }

        public Scheduler(Student Student, ProgramSequence MasterSequence)
        {
            this.Student = Student;
            this.MasterSequence = MasterSequence;
            LocalSequence = ProgramSequence.DeepClone(MasterSequence);
            CoursesTaken = new List<Course>();
            CoursesToTake = new List<string>();
            PrereqsMissing = new List<Course>();

            foreach (CourseTaken s in db.Students.Find(Student.StudentId).CourseTakens.ToList())
                CoursesTaken.Add(s.Section.Course);
        }

        //FALL = FALL N Winter
        //Winter = Winter ONLY
        //NO SUMMER
        public void GenerateSchedule(Semester sem)
        {
            // bool SwitchSemester = false;
            //10 if Fall, 5 if Winter
            int ClassCount = (sem == Semester.Fall) ? 10 : 5;

            foreach (List<string> year in MasterSequence.YearToListOfCourses)
            {

                foreach (string course in year)
                {
                    System.Diagnostics.Debug.WriteLine("Course Count is: " + CoursesToTake.Count);
                    if (CoursesToTake.Count < ClassCount)
                    {
                        //The Student already took the course, remove it 
                        if (CoursesTaken.Contains(CourseByCode(course)))
                        {
                            LocalSequence.RemoveCourse(course);
                            System.Diagnostics.Debug.WriteLine("[ALREADY TOOK]: " + course.ToString());
                            continue;
                        }
                        CheckPrereqs(course);
                    }
                    else { break; }

                }


                //Now that the year has been sifted, assume the student passes all his/her classes
                // AddTakenCourses(YearMap, Semester.Winter);
                //Update the local sequence
                //  UpdateLocal();
            }
        }

        private Course CourseByCode(string Code)
        {
            return db.Courses.Where(x => (x.Code == Code)).FirstOrDefault();
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

        private bool HasPrereqs(String course)
        {
            Course Prereq1 = null;
            Course Prereq2 = null;
            try
            {
                Prereq1 = CourseByCode(CourseByCode(course).Prerequisite1);
                Prereq2 = CourseByCode(CourseByCode(course).Prerequisite2);
            }
            catch (NullReferenceException) { }
            if (Prereq1 != null || Prereq2 != null)
                return true;
            return false;
        }

        private List<Course> GetPrereqs(String course)
        {
            Course Prereq1 = null;
            Course Prereq2 = null;
            List<Course> list = new List<Course>();
            try
            {
                Prereq1 = CourseByCode(CourseByCode(course).Prerequisite1);
                Prereq2 = CourseByCode(CourseByCode(course).Prerequisite2);
            }
            catch (NullReferenceException) { }
            if (Prereq1 != null && Prereq2 != null)
            {
                list.Add(Prereq1);
                list.Add(Prereq2);
            }
            else if (Prereq1 != null)
            {
                list.Add(Prereq1);
            }
            else if (Prereq2 != null)
            {
                list.Add(Prereq2);
            }

            return list;
        }

        private void CheckPrereqs(String course)
        {
            int PrereqDoneCount = 0;
            List<Course> list = GetPrereqs(course);
            System.Diagnostics.Debug.WriteLine("Checking " + course + " for prereqs");

            if (HasPrereqs(course))
            {
                foreach (Course c in list)
                {
                    //This course was already taken by the student
                    if (CoursesTaken.Exists(x => (x.Code == course)))
                    {
                        PrereqDoneCount++;
                        if (PrereqDoneCount == list.Count)
                            CoursesToTake.Add(course);
                    }
                    else
                    {
                        if (HasPrereqs(c.Code))
                        {
                            CheckPrereqs(c.Code);
                        }

                    }
                }

                System.Diagnostics.Debug.WriteLine("\t" + course + " has prereqs.");
                PrereqsMissing.Add(CourseByCode(course));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("\t" + course + " needs to be done.");
                CoursesToTake.Add(course);
            }


            System.Diagnostics.Debug.WriteLine("..done checking " + course);
        }

        private bool HasSemester(String course, Semester sem)
        {
            bool flag = false;
            try
            {
               flag= CourseByCode(course).Sections.ToList().Exists(x => x.Term == sem.ToString());
            }
            catch (NullReferenceException) { }

            return flag;
        }

        public void GenerateSectionList(Semester sem)
        {
            List<Section> SectionList = new List<Section>();
            List<Course> Fall = new List<Course>();
            List<Course> Winter = new List<Course>();
            List<Course> Both = new List<Course>();
            List<List<Course>> Container = new List<List<Course>>();

            Container.Add(Fall);
            Container.Add(Winter);
            Container.Add(Both);

            //Filter by availability
            FilterClasses(Fall, Winter, Both);
            System.Diagnostics.Debug.WriteLine("Fall: " + Fall.Count + " | Winter: " + Winter.Count + " | Both: " + Both.Count);

            //Find Sections
            foreach (List<Course> list in Container)
            {
                System.Diagnostics.Debug.WriteLine("In find section loop list size is: " + list.Count);
                foreach (Course c in list)
                {
                    foreach (Section sec in c.Sections.ToList())
                    {
                        SectionList.Add(sec);
                        System.Diagnostics.Debug.WriteLine("\tAdding Section " + c.Code + " "+ sec.Name);
                    }
                    System.Diagnostics.Debug.WriteLine("Section count is: " + SectionList.Count);
                }
            }

        }

        private void FilterClasses(List<Course> Fall, List<Course> Winter, List<Course> Both)
        {
            foreach (string CourseCode in CoursesToTake)
            {
                Course course = CourseByCode(CourseCode);
                if (HasSemester(CourseCode, Semester.Fall) && HasSemester(CourseCode, Semester.Winter))
                {
                    Both.Add(course);
                }
                else if (HasSemester(CourseCode, Semester.Fall))
                {
                    Fall.Add(course);
                }
                else if (HasSemester(CourseCode, Semester.Winter))
                {
                    Winter.Add(course);
                }
            }
        }

        /*DEBUG*/
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

        public void PrintList(List<string> list, String title)
        {
            foreach (String c in list)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("[" + title + "]: " + c);
                }
                catch (NullReferenceException) { }
            }
        }
    }
}

