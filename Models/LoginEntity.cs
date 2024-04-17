using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task_Management_System2.Models
{
    public class LoginEntity
    {
        public string Employee_Id { get; set; }
        public string Password { get; set; }
        public AccessType UserType { get; set; }
        public enum AccessType
        {
            User,
            Admin
        }
    }
}