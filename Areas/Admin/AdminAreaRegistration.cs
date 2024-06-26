﻿using System.Web.Mvc;

namespace Task_Management_System2.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Adminn", controller= "Admin", id = UrlParameter.Optional }
            );
        }
    }
}