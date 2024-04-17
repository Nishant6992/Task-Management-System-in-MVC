using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task_Management_System2.Models
{
    public class ForgetPassword
    {
        public string Email { get; set; }
        public string UserId { get; set; }
        public string FromEmail { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}