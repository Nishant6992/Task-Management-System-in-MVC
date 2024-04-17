using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Management_System2.Areas.Admin.Models;

namespace Task_Management_System2.Areas.Admin.Controllers
{
    public class ProjectController : Controller
    {
        string ConnectionStringSettings = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();

        // GET: Admin/Project
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewProject()
        {
            string CreatedByy = Session["FirstNameAdmin"].ToString();
            

            NewProject obj1 = new NewProject();
            obj1.CreatedBy = CreatedByy;
            
            return View(obj1);
        }


        [HttpPost]
        public ActionResult NewProject(NewProject m)
        {
           
            string CreatedByID = Session["Employee_IdAdmin"].ToString();
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();
            SqlCommand command = new SqlCommand("Tarun_TMC_ProjectEditUpdate", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "NEW_PROJECT");
            command.Parameters.AddWithValue("@CreatedBy", CreatedByID);
            command.Parameters.AddWithValue("@ProjectName", m.ProjectName);
            m.StartDate = DateTime.Now.ToString("MM-dd-yyyy");
            command.Parameters.AddWithValue("@StartDate", m.StartDate );
            command.Parameters.AddWithValue("@EndDate", m.EndDate);
            command.Parameters.AddWithValue("@ProjectDescription", m.ProjectDescription);
            command.ExecuteNonQuery();
            connection.Close();

            ModelState.Clear();
            return View();
        }
        

        public ActionResult GeneratePdfProject()
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<NewProject> objlst = new List<NewProject>();
            SqlCommand command = new SqlCommand("Tarun_TMC_ProjectEditUpdate", connection);
            command.Parameters.AddWithValue("@Action", "SELECT ALL");
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();

            NewProject objTask = null;

            while (dr.Read())
            {
                objTask = new NewProject();

                objTask.ProjectId = int.Parse(dr["ProjectId"].ToString());
                objTask.ProjectName = dr["ProjectName"].ToString();
                objTask.ProjectDescription = dr["ProjectDescription"].ToString();
                objTask.StartDate = dr["StartDate"].ToString();
                objTask.ModifiedDate = dr["ModifiedDate"].ToString();
                objTask.EndDate = dr["EndDate"].ToString();

                objlst.Add(objTask);
            }
            return new PartialViewAsPdf("GeneratePdfProject", objlst)
            {
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageSize = Rotativa.Options.Size.A4,
                CustomSwitches = "--footer-center \" [page] Page of [toPage] Pages\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                FileName = "EmployeeList(Tarun).pdf"
            };
        }
        public ActionResult AllProjects()
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<NewProject> objlst = new List<NewProject>();
            SqlCommand command = new SqlCommand("Tarun_TMC_ProjectEditUpdate", connection);
            command.Parameters.AddWithValue("@Action", "SELECT ALL");
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();

            NewProject objTask = null;

            while (dr.Read())
            {
                objTask = new NewProject();

                objTask.ProjectId = int.Parse(dr["ProjectId"].ToString());
                objTask.ProjectName = dr["ProjectName"].ToString();
                objTask.ProjectDescription = dr["ProjectDescription"].ToString();
                objTask.StartDate = dr["StartDate"].ToString();
                objTask.ModifiedDate = dr["ModifiedDate"].ToString();
                objTask.EndDate = dr["EndDate"].ToString();
                objTask.ProjectHours = dr["ProjectHours"].ToString();

                objlst.Add(objTask);
            }
            return View(objlst);
        }

        public ActionResult ProjectDetails(int ID)
        {

            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<AllTaskViewTableEntity> objlst = new List<AllTaskViewTableEntity>();
            SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemTask_SelectDelete", connection);
            command.Parameters.AddWithValue("@Action", "PROJECT DETAILS");
            command.Parameters.AddWithValue("@ProjectId", ID);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();

            AllTaskViewTableEntity objTask = null;

            while (dr.Read())
            {
                objTask = new AllTaskViewTableEntity();

                objTask.TaskId = int.Parse(dr["TaskId"].ToString());
                objTask.TaskName = dr["TaskName"].ToString();
                objTask.ProjectName = dr["ProjectName"].ToString();
                objTask.TaskDescription = dr["TaskDescription"].ToString();
                objTask.StartDate = dr["StartDate"].ToString();
                objTask.EstimateDate = dr["EstimateDate"].ToString();
                objTask.AssignedTo = dr["AssignedTo"].ToString();
                objTask.UserComment = dr["UserComment"].ToString();
                objTask.TaskPriority = dr["TaskPriority"].ToString();
                objTask.TaskProjectHours = dr["TaskProjectHours"].ToString();

                objlst.Add(objTask);
            }
            return View(objlst);
        }

        public ActionResult UserAssignedProject(string a)
        {
            int idInt = (int)Session["Employee_IdAdmin"]; 
            string id = idInt.ToString();
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<NewProject> objlst = new List<NewProject>();
            SqlCommand command = new SqlCommand("Tarun_ProjectViewForUser", connection);
            command.Parameters.AddWithValue("@Employee_Id", id);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();

            NewProject objTask = null;

            while (dr.Read())
            {
                objTask = new NewProject();

                objTask.ProjectId = int.Parse(dr["ProjectId"].ToString());
                objTask.ProjectName = dr["ProjectName"].ToString();
                objTask.ProjectDescription = dr["ProjectDescription"].ToString();
                objTask.StartDate = dr["StartDate"].ToString();
                objTask.ModifiedDate = dr["ModifiedDate"].ToString();
                objTask.EndDate = dr["EndDate"].ToString();

                objlst.Add(objTask);
            }
            return View(objlst);
        }


        [HttpGet]
        public ActionResult EditProject()
        {
            return View();
        }
        //[HttpPost]
        //public ActionResult Ed()
        //{
        //    return View();
        //}
    }
}