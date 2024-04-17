using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Task_Management_System2.Areas.Admin.Models
{
    public class AdminDashBoardEntity
    {
        [Required]
        public int Employee_Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]

        public string FirstName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string LastName { get; set; }
        public string Date_of_Birth { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Phone { get; set; }

    }
}