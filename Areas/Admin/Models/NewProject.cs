using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task_Management_System2.Areas.Admin.Models
{
    public class NewProject
    {
        public string CreatedBy { get; set; }
        public string ProjectName { get; set; }
        public string ProjectNamess { get; set; }
        public string ProjectHours { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ProjectDescription { get; set; }
        public int ProjectId { get; set; }
        public string ModifiedDate { get; set; }
        
    }
}

