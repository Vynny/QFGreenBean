using QFGreenBean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QFGreenBean.Helpers
{
    public class Scheduler
    {
        /*DEBUG PRINTING FLAG*/
        private bool DEBUG = false;
        static int MaxSemesterCourses = 5;

        /*Utils*/
        private PlannerDbEntities db = new PlannerDbEntities(); //Database to retreive data

        /*Input*/
        private Student Student; //The Student
        private List<Course> CoursesTaken; //The Courses the Student has already taken
        private ProgramSequence MasterSequence; //The master sequence, containing the recommended sequence for a particular program and choice

        /*Helpful Structures*/
        private ProgramSequence LocalSequence; //A DEEP copy of the master sequence, used to track user's schedule throughout generation. The final result is the list of courses that must be added to the schedule"
        private List<Course> PrereqsMissing; //Courses are added to this list when prereqs for it are missing (ex: Student did COMP249 but not COMP232, COMP352 would be added to this list when encountered
        public List<string> CoursesToTake { get; set; }

        /*Output*/
        public List<Section> ScheduledSectionsFall { get; set; }
        public List<Section> ScheduledSectionsWinter { get; set; }

        public Scheduler(Student Student, ProgramSequence MasterSequence)
        {
            this.Student = Student;
            this.MasterSequence = MasterSequence;
            LocalSequence = ProgramSequence.DeepClone(MasterSequence);
            CoursesTaken = new List<Course>();
            CoursesToTake = new List<string>();
            PrereqsMissing = new List<Course>();
            ScheduledSectionsFall = new List<Section>();
            ScheduledSectionsWinter = new List<Section>();

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
                    if (DEBUG)
                        System.Diagnostics.Debug.WriteLine("Course Count is: " + CoursesToTake.Count);
                    if (CoursesToTake.Count <= ClassCount)
                    {
                        //The Student already took the course, remove it 
                        if (CoursesTaken.Contains(CourseByCode(course)))
                        {
                            LocalSequence.RemoveCourse(course);
                            if (DEBUG)
                                System.Diagnostics.Debug.WriteLine("[ALREADY TOOK]: " + course.ToString());
                            continue;
                        }
                        CheckPrereqs(course, sem);
                    }
                    else { break; }

                }
            }

            GenerateSectionList(sem);
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

        private void CheckPrereqs(String course, Semester sem)
        {
            int PrereqDoneCount = 0;
            int PrereqWaitingCount = 0;
            List<Course> list = GetPrereqs(course);
            if (DEBUG)
                System.Diagnostics.Debug.WriteLine("Checking " + course + " for prereqs");

            if (HasPrereqs(course))
            {
                if (DEBUG)
                    System.Diagnostics.Debug.WriteLine("There are " + list.Count + " prereqs");
                foreach (Course c in list)
                {
                    if (DEBUG)
                        System.Diagnostics.Debug.WriteLine("\tPrereq is: " + c.Code);
                    //This course was already taken by the student
                    if (CoursesTaken.Exists(x => (x.Code == course)))
                    {
                        PrereqDoneCount++;
                        if (PrereqDoneCount == list.Count)
                        {
                            if (!CoursesToTake.Exists(x => (x == (course))))
                            {
                                CoursesToTake.Add(course);
                            }
                        }
                    }
                    else if (CoursesToTake.Exists(x => (x == c.Code)))
                    {
                        if (DEBUG)
                            System.Diagnostics.Debug.WriteLine("\t\t" + c.Code + " IN CTT");
                        PrereqWaitingCount++;
                        if (PrereqWaitingCount == list.Count)
                        {
                            if (!CoursesToTake.Exists(x => (x == (course + "_SKIP"))) && sem == Semester.Fall)
                            {
                                CoursesToTake.Add(course + "_SKIP");
                            }
                        }
                    }
                    else
                    {
                        if (HasPrereqs(c.Code))
                        {
                            CheckPrereqs(c.Code, sem);
                        }

                    }
                }


                if (DEBUG) System.Diagnostics.Debug.WriteLine("\t" + course + " has prereqs.");
                if (!PrereqsMissing.Exists(x => x == CourseByCode(course)))
                {
                    PrereqsMissing.Add(CourseByCode(course));
                }
            }
            else
            {
                if (DEBUG)
                    System.Diagnostics.Debug.WriteLine("\t" + course + " needs to be done.");
                if (!CoursesToTake.Exists(x => x == course))
                    CoursesToTake.Add(course);
            }


            if (DEBUG)
                System.Diagnostics.Debug.WriteLine("..done checking " + course);
        }

        private bool HasSemester(String course, Semester sem)
        {
            bool flag = false;
            try
            {
                flag = CourseByCode(course).Sections.ToList().Exists(x => x.Term == sem.ToString());
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
            if (DEBUG)
                System.Diagnostics.Debug.WriteLine("Fall: " + Fall.Count + " | Winter: " + Winter.Count + " | Both: " + Both.Count);

            //Find Sections
            foreach (List<Course> list in Container)
            {
                if (DEBUG)
                    System.Diagnostics.Debug.WriteLine("In find section loop list size is: " + list.Count);
                foreach (Course c in list)
                {
                    foreach (Section sec in c.Sections.ToList())
                    {
                        SectionList.Add(sec);
                        if (DEBUG)
                            System.Diagnostics.Debug.WriteLine("\tAdding Section " + c.Code + " " + sec.Name);
                    }
                    if (DEBUG)
                        System.Diagnostics.Debug.WriteLine("Section count is: " + SectionList.Count);
                }
            }

            //Remove sections that the student cannot attend
            FilterConstrainedSections(SectionList);

            //At this point, we have all the potential sections the student can take, its time to start scheduling them.
            //If scheduling for Fall, include fall and winter. If winter, only winter.
            List<Course> TakeFall = new List<Course>();
            List<Course> TakeWinter = new List<Course>();

            //TakeFall.AddRange(Fall);

            foreach (string s in CoursesToTake)
            {
                if (s.Contains("_"))
                {
                    TakeWinter.Add(CourseByCode(s.Split('_')[0]));
                }
                else if (Both.Exists(x => x.Code == s))
                {
                    if (TakeFall.Count < MaxSemesterCourses && sem == Semester.Fall)
                        TakeFall.Add(CourseByCode(s));
                    else
                        TakeWinter.Add(CourseByCode(s));
                }
            }

            foreach (Course c in TakeFall)
            {
                try
                {

                    if (DEBUG) System.Diagnostics.Debug.WriteLine("FALL: " + c.Code);
                }
                catch (NullReferenceException) { }

            }
            foreach (Course c in TakeWinter)
            {
                try
                {
                    if (DEBUG)
                        System.Diagnostics.Debug.WriteLine("WINTER: " + c.Code);
                }
                catch (NullReferenceException) { }
            }

            foreach (Course c in TakeFall)
            {
                List<Section> courseSections = SectionList.Where(x => (x.Course.Code == c.Code) && (x.Term == "Fall")).ToList();
                if (DEBUG)
                    System.Diagnostics.Debug.WriteLine("Sections Found: " + courseSections.Count);
                if (c.HasLab && c.HasTutortial)
                {
                    AddType("LEC", courseSections, ScheduledSectionsFall);
                    AddType("LAB", courseSections, ScheduledSectionsFall);
                    AddType("TUT", courseSections, ScheduledSectionsFall);
                }
                else if (c.HasTutortial)
                {
                    AddType("LEC", courseSections, ScheduledSectionsFall);
                    AddType("TUT", courseSections, ScheduledSectionsFall);
                }
                else
                {
                    AddType("LEC", courseSections, ScheduledSectionsFall);
                }

            }

            foreach (Course c in TakeWinter)
            {
                List<Section> courseSections = SectionList.Where(x => (x.Course.Code == c.Code) && (x.Term == Semester.Winter.ToString())).ToList();
                if (c.HasLab && c.HasTutortial)
                {
                    AddType("LEC", courseSections, ScheduledSectionsWinter);
                    AddType("LAB", courseSections, ScheduledSectionsWinter);
                    AddType("TUT", courseSections, ScheduledSectionsWinter);
                }
                else if (c.HasTutortial)
                {
                    AddType("LEC", courseSections, ScheduledSectionsWinter);
                    AddType("TUT", courseSections, ScheduledSectionsWinter);
                }
                else
                {
                    AddType("LEC", courseSections, ScheduledSectionsWinter);
                }
            }

            foreach (Section c in ScheduledSectionsFall)
            {
                if (DEBUG)
                    System.Diagnostics.Debug.WriteLine("[FALL SECTION SCHEDULED]: " + c.Course.Code + " : " + c.Name);
            }
            foreach (Section c in ScheduledSectionsWinter)
            {
                if (DEBUG)
                    System.Diagnostics.Debug.WriteLine("[WINTER SECTION SCHEDULED]: " + c.Course.Code + " : " + c.Name);
            }


        }

        private void AddType(string type, List<Section> sections, List<Section> output)
        {
            foreach (Section sec in sections)
            {
                if (sec.Type == type)
                {
                    if (output.Count > 0)
                    {
                        foreach (Section sched in output.ToList())
                        {
                            if (!isOverlap(sec, sched))
                            {
                                if (!output.Exists(x => x == sec))
                                    output.Add(sec);
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (!output.Exists(x => x == sec))
                            output.Add(sec);
                        return;
                    }


                }
            }
        }
        private bool DayOverlap(Section a, Section b)
        {
            string aday1 = null;
            string aday2 = null;
            string bday1 = null;
            string bday2 = null;

            if (a.Day.Length > 2)
            {
                aday1 = FormatDay(a.Day.Substring(0, 2));
                aday2 = FormatDay(a.Day.Substring(2, 2));
            }
            else
            {
                aday1 = FormatDay(a.Day);
            }

            if (b.Day.Length > 2)
            {
                bday1 = FormatDay(b.Day.Substring(0, 2));
                bday2 = FormatDay(b.Day.Substring(2, 2));
            }
            else
            {
                bday1 = FormatDay(b.Day);
            }

            if (aday1 == bday1 || aday1 == bday2)
                return true;

            if (aday2 == bday1 || aday2 == bday2)
                return true;

            return false;

        }
        private bool isOverlap(Section a, Section b)
        {
            string aStartHour = a.StartTime.ToString().Split(':')[0];
            string aStartMinute = a.StartTime.ToString().Split(':')[1];
            string aEndHour = a.EndTime.ToString().Split(':')[0];
            string aEndMinute = a.EndTime.ToString().Split(':')[1];

            string bStartHour = b.StartTime.ToString().Split(':')[0];
            string bStartMinute = b.StartTime.ToString().Split(':')[1];
            string bEndHour = b.EndTime.ToString().Split(':')[0];
            string bEndMinute = b.EndTime.ToString().Split(':')[1];

            int aStart = Convert.ToInt32(aStartHour + aStartMinute);
            int bStart = Convert.ToInt32(bStartHour + bStartMinute);
            int aEnd = Convert.ToInt32(aEndHour + aEndMinute);
            int bEnd = Convert.ToInt32(bEndHour + bEndMinute);
            
            return aStart < bEnd && bStart < aEnd && DayOverlap(a, b);
        }

        private void FilterConstrainedSections(List<Section> SectionList)
        {
            foreach (Section sec in SectionList.ToList())
            {
                if (isConstrained(Student, sec))
                {
                    if (DEBUG)
                        System.Diagnostics.Debug.WriteLine("Section" + sec.Name + " has time: " + sec.StartTime.ToString() + " and is CONSTRAINED");
                    SectionList.Remove(sec);
                }
            }
        }

        private string FormatDay(string day)
        {
            switch (day)
            {
                case "Mo":
                    return "Monday";
                case "Tu":
                    return "Tuesday";
                case "We":
                    return "Wednesday";
                case "Th":
                    return "Thursday";
                case "Fr":
                    return "Friday";
            }
            return null;
        }

        private bool HasDay(StudentConstraint c, Section s)
        {
            string day1 = null;
            string day2 = null;
            if (s.Day.Length > 2)
            {
                day1 = FormatDay(s.Day.Substring(0, 2));
                day2 = FormatDay(s.Day.Substring(2, 2));
            }
            else
            {
                day1 = FormatDay(s.Day);
            }

            if (c.Day == day1)
                return true;

            if (c.Day == day2)
                return true;

            return false;
        }

        private bool isConstrained(Student s, Section sec)
        {
            string startHour = sec.StartTime.ToString().Split(':')[0];
            string startMinute = sec.StartTime.ToString().Split(':')[1];
            string endHour = sec.EndTime.ToString().Split(':')[0];
            string endMinute = sec.EndTime.ToString().Split(':')[1];

            foreach (StudentConstraint constraint in s.StudentConstraints.ToList())
            {
                if (!HasDay(constraint,sec))
                    continue;

                if (constraint.EndHour == startHour && constraint.EndMinute == startMinute)
                    return false;

                if (constraint.StartHour == endHour && constraint.StartMinute == endMinute)
                    return false;

                DateTime startTime = DateTime.Parse(constraint.StartHour + ":" + constraint.StartMinute + ":" + "00");
                DateTime endTime = DateTime.Parse(constraint.EndHour + ":" + constraint.EndMinute + ":" + "00");
                while (startTime <= endTime)
                {
                    if (startTime.Hour.ToString() == startHour && startTime.Minute.ToString() == startMinute)
                        return true;

                    startTime = startTime.AddMinutes(15);
                }
                //int aStart = Convert.ToInt32(constraint.StartHour + constraint.StartMinute);
                //int aEnd = Convert.ToInt32(constraint.EndHour + constraint.EndMinute);
                //int bStart = Convert.ToInt32(startHour + startMinute);
                //int bEnd = Convert.ToInt32(endHour + endMinute);

                //return aStart < bEnd && bStart < aEnd;

            }
            return false;
        }

        private void FilterClasses(List<Course> Fall, List<Course> Winter, List<Course> Both)
        {
            foreach (string CourseCode in CoursesToTake)
            {
                string code = CourseCode.Split('_')[0];
                Course course = CourseByCode(code);
                if (HasSemester(code, Semester.Fall) && HasSemester(code, Semester.Winter))
                {
                    Both.Add(course);
                }
                else if (HasSemester(code, Semester.Fall))
                {
                    Fall.Add(course);
                }
                else if (HasSemester(code, Semester.Winter))
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

