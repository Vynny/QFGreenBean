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
        Fall,
        Winter,
        Summer
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
                .AddCourse("COMP 232", Semester.Fall)
                .AddCourse("COMP 248", Semester.Fall)
                .AddCourse("ENGR 201", Semester.Fall)
                .AddCourse("ENGR 213", Semester.Fall)
                .AddCourse("GENERAL", Semester.Fall)

                //Winter
                .AddCourse("COMP 249", Semester.Winter)
                .AddCourse("ENGR 233", Semester.Winter)
                .AddCourse("SOEN 228", Semester.Winter)
                .AddCourse("SOEN 287", Semester.Winter)
                .AddCourse("SCIENCE1", Semester.Winter);

            SOEN_General.Year(2)
                //Fall
                .AddCourse("COMP 348", Semester.Fall)
                .AddCourse("COMP 352", Semester.Fall)
                .AddCourse("ELEC 375", Semester.Fall)
                .AddCourse("ENCS 282", Semester.Fall)
                .AddCourse("SCIENCE2", Semester.Fall)
                //Winter
                .AddCourse("COMP 346", Semester.Winter)
                .AddCourse("ENGR 275", Semester.Winter)
                .AddCourse("ENGR 371", Semester.Winter)
                .AddCourse("SOEN 331", Semester.Winter)
                .AddCourse("SOEN 341", Semester.Winter);

            SOEN_General.Year(3)
                //Fall
                .AddCourse("COMP 335", Semester.Fall)
                .AddCourse("COMP 342", Semester.Fall)
                .AddCourse("ENGR 343", Semester.Fall)
                .AddCourse("ENGR 384", Semester.Fall)
                .AddCourse("ENGR 391", Semester.Fall)
                //Winter
                .AddCourse("SOEN 344", Semester.Winter)
                .AddCourse("SOEN 345", Semester.Winter)
                .AddCourse("SOEN 357", Semester.Winter)
                .AddCourse("SOEN 390", Semester.Winter)
                .AddCourse("ELECTIVE1", Semester.Winter);

            SOEN_General.Year(4)
                //Fall
                .AddCourse("SOEN 490_1", Semester.Fall)
                .AddCourse("ENGR 301", Semester.Fall)
                .AddCourse("SOEN 321", Semester.Fall)
                .AddCourse("ELECTIVE2", Semester.Fall)
                .AddCourse("ELECTIVE3", Semester.Fall)
                //Winter
                .AddCourse("SOEN 385", Semester.Winter)
                .AddCourse("ENGR 392", Semester.Winter)
                .AddCourse("SOEN 490_2", Semester.Winter)
                .AddCourse("ELECTIVE4", Semester.Winter)
                .AddCourse("ELECTIVE5", Semester.Winter);

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