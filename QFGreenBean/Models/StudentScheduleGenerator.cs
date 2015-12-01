using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QFGreenBean.Models
{
    public class StudentScheduleGenerator
    {
        public string StudentNumber { get; set; }

        public string StartingSemester { get; set; }

        public string Department { get; set; }

        public string ProgramOption { get; set; }

        public bool IncludeSummer { get; set; }
    }
}
