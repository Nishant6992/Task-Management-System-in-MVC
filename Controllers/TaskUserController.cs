using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Management_System2.Models;

namespace Task_Management_System2.Controllers
{
    public class TaskUserController : Controller
    {
        string ConnectionStringSettings = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        // GET: TaskUser
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UserTask()
        {
            int ID = (int)Session["Employee_IdUser"];

            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<TaskUserData> objlst = new List<TaskUserData>();
            SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemTask_SelectDelete", connection);
            command.Parameters.AddWithValue("@Action", "SELECT ALL TASK USER");
            command.Parameters.AddWithValue("@Employee_Id", ID);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();

            TaskUserData objTask = null;

            while (dr.Read())
            {
                objTask = new TaskUserData();

                objTask.TaskId = int.Parse(dr["TaskId"].ToString());
                objTask.TaskName = dr["TaskName"].ToString();
                objTask.ProjectName = dr["ProjectName"].ToString();
                objTask.ProjectId = dr["ProjectId"].ToString();
                objTask.TaskDescription = dr["TaskDescription"].ToString();
                objTask.StartDate = dr["StartDate"].ToString();
                objTask.EstimateDate = dr["EstimateDate"].ToString();
                objTask.Attachment = dr["Attachment"].ToString();
                objTask.AssignedTo = dr["AssignedTo"].ToString();
                objTask.UserComment = dr["UserComment"].ToString();
                objTask.TaskPriority = dr["TaskPriority"].ToString();
                objTask.TaskProjectHours = dr["TaskProjectHours"].ToString();

                
                objlst.Add(objTask);
            }
            return View(objlst);
        }

        

        public ActionResult CommentUser(string id)
        {
            SqlConnection conn = new SqlConnection(ConnectionStringSettings);
            conn.Open();

            SqlCommand comma = new SqlCommand("Select UserComment FROM Tarun_AddTask where TaskId=@TaskId", conn);
            comma.Parameters.AddWithValue("@TaskId", Convert.ToInt32(id));
            SqlDataReader drrr = comma.ExecuteReader();
            Comment ob = new Comment();

            while (drrr.Read())
            {
                ob.UserComment = drrr["UserComment"].ToString();
            }
            ob.TaskId = Convert.ToInt32(id);
            conn.Close();
            return View(ob);

        }
        [HttpPost]
        public ActionResult CommentUser(Comment m)
        {
            
                SqlConnection connection = new SqlConnection(ConnectionStringSettings);
                connection.Open();
                SqlCommand command = new SqlCommand("UPDATE Tarun_AddTask SET UserComment=@UserComment WHERE TaskId=@TaskId", connection);

                command.Parameters.AddWithValue("@TaskId", m.TaskId);
                command.Parameters.AddWithValue("@UserComment", m.UserComment);
                command.ExecuteNonQuery();
                connection.Close();
            
            return RedirectToAction("UserTask");
        }
        public ActionResult GeneratePdfTask()
        {
            int ID = (int)Session["Employee_IdUser"];
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<TaskUserData> objlst = new List<TaskUserData>();
            SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemTask_SelectDelete", connection);
            command.Parameters.AddWithValue("@Action", "SELECT ALL TASK USER");
            command.Parameters.AddWithValue("@Employee_Id", ID);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();

            TaskUserData objTask = null;

            while (dr.Read())
            {
                objTask = new TaskUserData();

                objTask.TaskId = int.Parse(dr["TaskId"].ToString());
                objTask.TaskName = dr["TaskName"].ToString();
                objTask.ProjectName = dr["ProjectName"].ToString();
                objTask.ProjectId = dr["ProjectId"].ToString();
                objTask.TaskDescription = dr["TaskDescription"].ToString();
                objTask.StartDate = dr["StartDate"].ToString();
                objTask.EstimateDate = dr["EstimateDate"].ToString();
                objTask.Attachment = dr["Attachment"].ToString();
                objTask.AssignedTo = dr["AssignedTo"].ToString();
                objTask.UserComment = dr["UserComment"].ToString();
                objTask.TaskPriority = dr["TaskPriority"].ToString();

                objlst.Add(objTask);
            }
            return new PartialViewAsPdf("GeneratePdfTask", objlst)
            {
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageSize = Rotativa.Options.Size.A4,
                CustomSwitches = "--footer-center \" [page] Page of [toPage] Pages\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                FileName = "YourTaskList.pdf"
            };
        }
        
        
        public ActionResult ForwardUser(string id)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<ForwardTaskk> UpdateEmployeeList = new List<ForwardTaskk>();

            SqlCommand a = new SqlCommand("Select Employee_Id, FirstName,LastName from Tarun_TaskManagementSystemReg where IsDeleted=0", connection);

            SqlDataReader drrr = a.ExecuteReader();

            ForwardTaskk ob = null;
            while (drrr.Read())
            {
                ob = new ForwardTaskk();

                ob.Forwarded_Employee_Id = int.Parse(drrr["Employee_Id"].ToString());
                ob.FirstName = drrr["FirstName"].ToString();
                ob.LastName = drrr["LastName"].ToString();

                UpdateEmployeeList.Add(ob);

            }
            drrr.Close();
            a.Dispose();

            ForwardTaskk obb = obb = new ForwardTaskk();
            obb.TaskId = int.Parse(id);
            ViewBag.UpdateEmployeeList = UpdateEmployeeList;
            return View(obb);
        }
        [HttpPost]
        public ActionResult ForwardUser(ForwardTaskk m)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();
            SqlCommand command = new SqlCommand("Tarun_Forward", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@TaskId", m.TaskId);
            command.Parameters.AddWithValue("@Employee_Id", m.Forwarded_Employee_Id);
            command.ExecuteNonQuery();
            connection.Close();
            return RedirectToAction("UserTask");
        }


        //Action For Add Hours in Task
        public ActionResult AddUser(string Task_Id,string Project_Id)
        {
            
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();
            SqlCommand command = new SqlCommand("select ProjectHours  from TarunTMG_Project where ProjectId = @ProjectId ", connection);
            command.Parameters.AddWithValue("@ProjectId", Project_Id);
            SqlDataReader drrr = command.ExecuteReader();
            AddHours ob = null;
            while (drrr.Read())
            {
                ob = new AddHours();

                
                ob.TotalHours = drrr["ProjectHours"].ToString();
                ob.TaskId = Task_Id;
                ob.ProjectId = Project_Id;
            }
            drrr.Close();
            command.Dispose();
            connection.Close();

            return View(ob);
        }
        [HttpPost]
        public ActionResult AddUser(AddHours m)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();
            SqlCommand command = new SqlCommand("TarunAddHoursTask", connection);

            // This SP Has A Problem TarunAddHoursTask

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "ADDHOURS");
            command.Parameters.AddWithValue("@TaskId", m.TaskId);
            command.Parameters.AddWithValue("@TaskProjectHours", m.AddHour);
            command.Parameters.AddWithValue("@ProjectId", m.ProjectId);
            command.ExecuteNonQuery();
            connection.Close();
            return RedirectToAction("UserTask");
        }
    }
}