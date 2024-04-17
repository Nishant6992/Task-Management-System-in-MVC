using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task_Management_System2.Areas.Admin.Models
{
    public class ForwardTask
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
            set
            {
                string[] parts = value.Split(' ');
                if (parts.Length > 1)
                {
                    FirstName = parts[0];
                    LastName = string.Join(" ", parts.Skip(1));
                }
                else
                {
                    FirstName = value;
                    LastName = "";
                }
            }
        }

        public int Forwarded_Employee_Id { get; set; }
        public int TaskId { get; set; }

        public string AssignedTo { get; set; }

    }
}