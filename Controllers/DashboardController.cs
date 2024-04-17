using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Management_System2.Models;

namespace Task_Management_System2.Controllers
{
    public class DashboardController : Controller
    {
        string ConnectionStringSettings = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserDashboard()
        {
            UserDashBoardEntity reg = new UserDashBoardEntity();
            return View();
        }

        [HttpPost]
        public ActionResult UserDashboard(string a)
        {
            
            return View();
        }

        public ActionResult UserProject(string a)
        {
            int idInt = (int)Session["Employee_IdUser"]; // Assuming it's stored as int
            string id = idInt.ToString();
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<NewProjectss> objlst = new List<NewProjectss>();
            SqlCommand command = new SqlCommand("Tarun_ProjectViewForUser", connection);
            command.Parameters.AddWithValue("@Employee_Id", id);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();

            NewProjectss objTask = null;

            while (dr.Read())
            {
                objTask = new NewProjectss();

                objTask.ProjectId = int.Parse(dr["ProjectId"].ToString());
                objTask.ProjectName = dr["ProjectName"].ToString();
                objTask.ProjectDescription = dr["ProjectDescription"].ToString();
                objTask.ProjectHours = dr["ProjectHours"].ToString();
                objTask.StartDate = dr["StartDate"].ToString();
                objTask.ModifiedDate = dr["ModifiedDate"].ToString();
                objTask.EndDate = dr["EndDate"].ToString();

                objlst.Add(objTask);
            }
            return View(objlst);
        }
        //public ActionResult GeneratePdfProjects()
        //{
        //    int idInt = (int)Session["Employee_IdUser"]; // Assuming it's stored as int
        //    string id = idInt.ToString();
        //    SqlConnection connection = new SqlConnection(ConnectionStringSettings);
        //    connection.Open();

        //    List<NewProjectss> objlst = new List<NewProjectss>();
        //    SqlCommand command = new SqlCommand("Tarun_ProjectViewForUser", connection);
        //    command.Parameters.AddWithValue("@Employee_Id", id);
        //    command.CommandType = System.Data.CommandType.StoredProcedure;
        //    SqlDataReader dr = command.ExecuteReader();

        //    NewProjectss objTask = null;

        //    while (dr.Read())
        //    {
        //        objTask = new NewProjectss();

        //        objTask.ProjectId = int.Parse(dr["ProjectId"].ToString());
        //        objTask.ProjectName = dr["ProjectName"].ToString();
        //        objTask.ProjectDescription = dr["ProjectDescription"].ToString();
        //        objTask.StartDate = dr["StartDate"].ToString();
        //        objTask.ModifiedDate = dr["ModifiedDate"].ToString();
        //        objTask.EndDate = dr["EndDate"].ToString();

        //        objlst.Add(objTask);
        //    }
        //    return new PartialViewAsPdf("GeneratePdfProject", objlst)
        //    {
        //        PageOrientation = Rotativa.Options.Orientation.Landscape,
        //        PageSize = Rotativa.Options.Size.A4,
        //        CustomSwitches = "--footer-center \" [page] Page of [toPage] Pages\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
        //        FileName = "EmployeeList(Tarun).pdf"
        //    };
        //}
    }
}