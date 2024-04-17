using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task_Management_System2.Models
{
    public class AddHours
    {
        public string TaskId { get; set; }
        public string ProjectId { get; set; }
        public string TotalHours { get; set; }

        public string AddHour { get; set; }

    }
}