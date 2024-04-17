using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task_Management_System2.Areas.Admin.Models
{
    public class AllTaskViewTableEntity
    {
        
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string ProjectName { get; set; }
        public string TaskProjectHours { get; set; }
        public string ProjectId { get; set; }
        public string TaskDescription { get; set; }
        public string StartDate { get; set; }
        public string EstimateDate { get; set; }
        public string Attachment { get; set; }
        public string AssignedTo { get; set; }
        public string UserComment { get; set; }
        public string TaskPriority { get; set; }

    }
    
}