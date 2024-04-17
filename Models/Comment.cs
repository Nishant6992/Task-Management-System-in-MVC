using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task_Management_System2.Models
{
    public class Comment
    {
        public int TaskId { get; set; }
        public string UserComment { get; set; }
    }
}