using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Collections.ObjectModel;

namespace QFGreenBean.Helpers
{

    public enum Semester
    {
        FALL,
        WINTER,
        SUMMER
    }

    public class Programs
    {
        public static ProgramSequence SOEN_General;

        public Programs()
        {
            SOEN_General = new ProgramSequence("SOEN", "General");

            /*
            *Program: Software Engineering 
            *Option: General 
            */
            SOEN_General.Year(1)
                //Fall
                .AddCourse("COMP 232", Semester.FALL)
                .AddCourse("COMP 248", Semester.FALL)
                .AddCourse("ENGR 201", Semester.FALL)
                .AddCourse("ENGR 213", Semester.FALL)
                .AddCourse("GENERAL", Semester.FALL)

                //Winter
                .AddCourse("COMP 249", Semester.WINTER)
                .AddCourse("ENGR 233", Semester.WINTER)
                .AddCourse("SOEN 228", Semester.WINTER)
                .AddCourse("SOEN 287", Semester.WINTER)
                .AddCourse("SCIENCE1", Semester.WINTER);

            SOEN_General.Year(2)
                //Fall
                .AddCourse("COMP 348", Semester.FALL)
                .AddCourse("COMP 352", Semester.FALL)
                .AddCourse("ELEC 375", Semester.FALL)
                .AddCourse("ENCS 282", Semester.FALL)
                .AddCourse("SCIENCE2", Semester.FALL)
                //Winter
                .AddCourse("COMP 346", Semester.WINTER)
                .AddCourse("ENGR 275", Semester.WINTER)
                .AddCourse("ENGR 371", Semester.WINTER)
                .AddCourse("SOEN 331", Semester.WINTER)
                .AddCourse("SOEN 341", Semester.WINTER);

            SOEN_General.Year(3)
                //Fall
                .AddCourse("COMP 335", Semester.FALL)
                .AddCourse("COMP 342", Semester.FALL)
                .AddCourse("ENGR 343", Semester.FALL)
                .AddCourse("ENGR 384", Semester.FALL)
                .AddCourse("ENGR 391", Semester.FALL)
                //Winter
                .AddCourse("SOEN 344", Semester.WINTER)
                .AddCourse("SOEN 345", Semester.WINTER)
                .AddCourse("SOEN 357", Semester.WINTER)
                .AddCourse("SOEN 390", Semester.WINTER)
                .AddCourse("ELECTIVE1", Semester.WINTER);

            SOEN_General.Year(4)
                //Fall
                .AddCourse("SOEN 490_1", Semester.FALL)
                .AddCourse("ENGR 301", Semester.FALL)
                .AddCourse("SOEN 321", Semester.FALL)
                .AddCourse("ELECTIVE2", Semester.FALL)
                .AddCourse("ELECTIVE3", Semester.FALL)
                //Winter
                .AddCourse("SOEN 385", Semester.WINTER)
                .AddCourse("ENGR 392", Semester.WINTER)
                .AddCourse("SOEN 490_2", Semester.WINTER)
                .AddCourse("ELECTIVE4", Semester.WINTER)
                .AddCourse("ELECTIVE5", Semester.WINTER);

        }

    }

    [Serializable]
    public class ProgramSequence
    {
        private int curYear  = -1;
        public string Program { get; set; }
        public string Option { get; set; }   
        public List<List<string>> YearToListOfCourses { get; set; }

        //private List<string> CourseToSemester;
        private Dictionary<string, Semester> SemesterMapping;

        public ProgramSequence(string Program, string Option)
        {
            this.YearToListOfCourses = new List<List<string>>();
            this.SemesterMapping = new Dictionary<string, Semester>();
            this.Program = Program;
            this.Option = Option;
        }

        public ProgramSequence Year(int year)
        {
            YearToListOfCourses.Add(new List<string>());
            curYear++;
            return this;
        }

        public ProgramSequence AddCourse(string courseID, Semester s)
        {
            SemesterMapping.Add(courseID, s);
            YearToListOfCourses[curYear].Add(courseID);
            return this;
        }

        public void RemoveCourse(string courseID)
        {
            foreach (List<string> course in YearToListOfCourses.ToList())
            {
               foreach (string c in course.ToList())
                {
                    if (courseID == c)
                        course.Remove(c);
                }
            }
        }

        public static T DeepClone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }


        public Semester getSemester(string courseID)
        {
            return SemesterMapping[courseID];
        }

    }



}