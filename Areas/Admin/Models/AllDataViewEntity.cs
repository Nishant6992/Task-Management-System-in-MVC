using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task_Management_System2.Areas.Admin.Models
{
    public class AllDataViewEntity
    {
        public int Employee_Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Date_of_Birth { get; set; }

        public string Email { get; set; }
        public string Address { get; set; }

        public string Phone { get; set; }
        public string JoinedCompany { get; set; }

        public string UpdatedEntry { get; set; }
        public string LeftCompany { get; set; }
        public string IsDeleted { get; set; }

        public string UserType { get; set; }

        public string FullName()
        {
            return this.FirstName + " " + this.LastName;
        }

    }
}