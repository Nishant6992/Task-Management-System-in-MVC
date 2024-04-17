using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Task_Management_System2.Areas.Admin.Models;
using static Task_Management_System2.Areas.Admin.Models.Main;

namespace Task_Management_System2.Areas.Admin.Controllers
{
    public class TaskController : Controller
    {
        string ConnectionStringSettings = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        // GET: Admin/Task
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult NewTask()
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<AddTask> ProjectList = new List<AddTask>();
            // Add Project List
            SqlCommand command = new SqlCommand("Select ProjectId, ProjectName from TarunTMG_Project where IsDeleted=0", connection);
            SqlDataReader dr = command.ExecuteReader();

            AddTask objj = null;
            ////I was here on 19th solving bug
            while (dr.Read())
            {
                objj = new AddTask();

                objj.ProjectId = dr["ProjectId"].ToString();
                objj.ProjectName = dr["ProjectName"].ToString();

                ProjectList.Add(objj);
            }
            dr.Close();

            List<AddTask> EmployeeList = new List<AddTask>();
            SqlCommand com = new SqlCommand("Select Employee_Id, FirstName,LastName from Tarun_TaskManagementSystemReg where IsDeleted=0", connection);
            SqlDataReader drr = com.ExecuteReader();

            AddTask obj = null;
            while (drr.Read())
            {
                obj = new AddTask();

                obj.Employee_Id = drr["Employee_Id"].ToString();
                obj.FirstName = drr["FirstName"].ToString();
                obj.LastName = drr["LastName"].ToString();

                EmployeeList.Add(obj);
            }
            drr.Close();

            ViewBag.ProjectList = ProjectList;
            ViewBag.EmployeeList = EmployeeList;
            return View();
        }

        [HttpPost]
        public ActionResult NewTask(AddTask m)
        {

            string CreatedByID = Session["Employee_IdAdmin"].ToString();
            //if (CreatedByID != "")
            {
                SqlConnection connection = new SqlConnection(ConnectionStringSettings);
                connection.Open();
                SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemTask_SelectDelete", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Action", "ADMIN TASK INSERT");

                command.Parameters.AddWithValue("@TaskName", m.TaskName);
                command.Parameters.AddWithValue("@ProjectName", m.ProjectName);
                command.Parameters.AddWithValue("@ProjectId", m.ProjectId);
                command.Parameters.AddWithValue("@TaskDescription", m.TaskDescription);
                command.Parameters.AddWithValue("@StartDate", m.StartDate);
                command.Parameters.AddWithValue("@EstimateDate", m.EstimateDate);
                command.Parameters.AddWithValue("@Attachment", m.Attachment);
                command.Parameters.AddWithValue("@Employee_Id", m.AssignedTo);
                command.Parameters.AddWithValue("@UserComment", m.UserComment);
                command.Parameters.AddWithValue("@TaskPriority", m.TaskPriority);

                int result = command.ExecuteNonQuery();
                connection.Close();




                connection.Open();
                using (SqlCommand com = new SqlCommand("SELECT Email FROM Tarun_TaskManagementSystemReg WHERE IsDeleted = 0 AND Employee_Id = @Employee_Id", connection))
                {
                    com.Parameters.AddWithValue("@Employee_Id", CreatedByID);
                    using (SqlDataReader drr = com.ExecuteReader())
                    {
                        if (drr.Read())
                        {
                            Main obj = new Main();
                            obj.Email = drr["Email"].ToString();
                            drr.Close();

                            string color = "";
                            switch (m.TaskPriority)
                            {
                                case "Low":
                                    color = "yellow";
                                    break;
                                case "Medium":
                                    color = "orange";
                                    break;
                                case "High":
                                    color = "red";
                                    break;
                                default:
                                    color = "black";
                                    break;
                            }

                            string emailBody = @"
                                                <html>
                                                <head>
                                                    <style>
                                                        body {
                                                            font-family: Arial, sans-serif;
                                                            margin: 0;
                                                            padding: 0;
                                                            background-color: #f2f2f2;
                                                        }


                                                        .container {
                                                            max-width: 600px;
                                                            margin: 20px auto;
                                                            background-color: #fff;
                                                            border-radius: 10px;
                                                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                                        }

                                                        .header {
                                                            background-color: #007bff;
                                                            color: #fff;
                                                            padding: 20px;
                                                            border-top-left-radius: 10px;
                                                            border-top-right-radius: 10px;
                                                            text-align: center;
                                                        }

                                                        .content {
                                                            padding: 20px;
                                                        }

                                                        .task-detail {
                                                            margin-bottom: 10px;
                                                        }

                                                        .task-heading {
                                                            color: " + color + @";
                                                            font-size: 24px;
                                                            font-weight: bold;
                                                            margin-bottom: 20px;
                                                        }
                                                    </style>
                                                </head>
                                                <body>
                                                    <div class='container'>
                                                        <div class='header'>
                                                            New Task
                                                        </div>
                                                        <div class='content'>
                                                            <div class='task-heading'>Task Details</div>
                                                            <div class='task-detail'><b>Task Name:</b> " + m.TaskName + @"</div>
                                                            <div class='task-detail'><b>Project Name:</b> " + m.ProjectName + @"</div>
                                                            <div class='task-detail'><b>Project ID:</b> " + m.ProjectId + @"</div>
                                                            <div class='task-detail'><b>Task Description:</b> " + m.TaskDescription + @"</div>
                                                            <div class='task-detail'><b>Start Date:</b> " + m.StartDate + @"</div>
                                                            <div class='task-detail'><b>Estimated Date:</b> " + m.EstimateDate + @"</div>
                                                            <div class='task-detail'><b>Task Priority:</b> " + m.TaskPriority + @"</div>
                                                        </div>
                                                    </div>
                                                </body>
                                                </html>";



                            MailMessage msg = new MailMessage();
                            msg.From = new MailAddress("tarun@concept.co.in");
                            msg.To.Add(obj.Email);
                            msg.Subject = "New Task Assigned to You";
                            msg.Body = emailBody;
                            msg.IsBodyHtml = true;
                            SmtpClient smt = new SmtpClient();
                            smt.Host = "mail.concept.co.in";
                            smt.Port = 25;
                            smt.Send(msg);
                        }
                    }
                }

                ModelState.Clear();
            }

            return View();

        }
        public ActionResult ForwardTask(string id)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<ForwardTask> UpdateEmployeeList = new List<ForwardTask>();

            SqlCommand a = new SqlCommand("Select Employee_Id, FirstName,LastName from Tarun_TaskManagementSystemReg where IsDeleted=0", connection);

            SqlDataReader drrr = a.ExecuteReader();

            ForwardTask ob = null;
            while (drrr.Read())
            {
                ob = new ForwardTask();

                ob.Forwarded_Employee_Id = int.Parse(drrr["Employee_Id"].ToString());
                ob.FirstName = drrr["FirstName"].ToString();
                ob.LastName = drrr["LastName"].ToString();

                UpdateEmployeeList.Add(ob);

            }
            drrr.Close();
            a.Dispose();

            ForwardTask obb = obb = new ForwardTask();
            obb.TaskId = int.Parse(id);
            ViewBag.UpdateEmployeeList = UpdateEmployeeList;
            return View(obb);
        }
        [HttpPost]
        public ActionResult ForwardTask(ForwardTask m)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();
            SqlCommand command = new SqlCommand("Tarun_Forward", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@TaskId", m.TaskId);
            command.Parameters.AddWithValue("@Employee_Id", m.Forwarded_Employee_Id);
            command.ExecuteNonQuery();
            connection.Close();
            return RedirectToAction("AllTaskViewTable");

        }


        public ActionResult ForwardTaskIndividual(string id)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<ForwardTask> UpdateEmployeeList = new List<ForwardTask>();

            SqlCommand a = new SqlCommand("Select Employee_Id, FirstName,LastName from Tarun_TaskManagementSystemReg where IsDeleted=0", connection);

            SqlDataReader drrr = a.ExecuteReader();

            ForwardTask ob = null;
            while (drrr.Read())
            {
                ob = new ForwardTask();

                ob.Forwarded_Employee_Id = int.Parse(drrr["Employee_Id"].ToString());
                ob.FirstName = drrr["FirstName"].ToString();
                ob.LastName = drrr["LastName"].ToString();

                UpdateEmployeeList.Add(ob);

            }
            drrr.Close();
            a.Dispose();

            ForwardTask obb = obb = new ForwardTask();
            obb.TaskId = int.Parse(id);
            ViewBag.UpdateEmployeeList = UpdateEmployeeList;
            return View(obb);
        }
        [HttpPost]
        public ActionResult ForwardTaskIndividual(ForwardTask m)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();
            SqlCommand command = new SqlCommand("Tarun_Forward", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@TaskId", m.TaskId);
            command.Parameters.AddWithValue("@Employee_Id", m.Forwarded_Employee_Id);
            command.ExecuteNonQuery();
            connection.Close();
            return RedirectToAction("IndividualTaskAdmin");
        }

        public ActionResult DeleteTask(string id)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();
            SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemTask_SelectDelete", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "DELETE TASK");
            command.Parameters.AddWithValue("@TaskId", Convert.ToInt32(id));
            int result = command.ExecuteNonQuery();
            connection.Close();
            return RedirectToAction("AllTaskViewTable");
        }

        [HttpGet]
        public ActionResult EditTask(string id)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<AddTask> ProjectList = new List<AddTask>();
            SqlCommand command = new SqlCommand("Select ProjectId, ProjectName from TarunTMG_Project where IsDeleted=0", connection);
            SqlDataReader dr = command.ExecuteReader();

            AddTask objj = null;

            while (dr.Read())
            {
                objj = new AddTask();

                objj.ProjectId = dr["ProjectId"].ToString();
                objj.ProjectName = dr["ProjectName"].ToString();

                ProjectList.Add(objj);
            }
            dr.Close();
            command.Dispose();

            List<AddTask> EmployeeList = new List<AddTask>();
            SqlCommand com = new SqlCommand("Select Employee_Id, FirstName,LastName from Tarun_TaskManagementSystemReg where IsDeleted=0", connection);
            SqlDataReader drr = com.ExecuteReader();

            AddTask obj = null;
            while (drr.Read())
            {
                obj = new AddTask();

                obj.Employee_Id = drr["Employee_Id"].ToString();
                obj.FirstName = drr["FirstName"].ToString();
                obj.LastName = drr["LastName"].ToString();

                EmployeeList.Add(obj);
            }
            drr.Close();
            com.Dispose();
            connection.Close();

            ViewBag.ProjectList = ProjectList;
            ViewBag.EmployeeList = EmployeeList;





            SqlConnection conn = new SqlConnection(ConnectionStringSettings);
            conn.Open();

            SqlCommand comma = new SqlCommand("Tarun_TaskManagementSystemTask_SelectDelete", conn);
            comma.CommandType = CommandType.StoredProcedure;
            comma.Parameters.AddWithValue("@Action", "SELECT EDIT TASK");
            comma.Parameters.AddWithValue("@TaskId", Convert.ToInt32(id));
            SqlDataReader drrr = comma.ExecuteReader();
            AddTask ob = new AddTask();

            while (drrr.Read())
            {

                ob.TaskName = drrr["TaskName"].ToString();
                ob.ProjectName = drrr["ProjectName"].ToString();
                ob.ProjectId = drrr["ProjectId"].ToString();
                ob.TaskDescription = drrr["TaskDescription"].ToString();
                ob.StartDate = drrr["StartDate"].ToString();
                ob.EstimateDate = drrr["EstimateDate"].ToString();
                ob.AssignedTo = drrr["AssignedTo"].ToString();
                ob.UserComment = drrr["UserComment"].ToString();
                ob.TaskPriority = drrr["TaskPriority"].ToString();
                ob.Employee_Id = drrr["Employee_Id"].ToString();

            }
            ob.TaskId = Convert.ToInt32(id);
            connection.Close();
            return View(ob);
        }

        [HttpPost]
        public ActionResult EditTask(AddTask m)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();
            SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemTask_SelectDelete", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "ADMIN TASK UPDATE");

            command.Parameters.AddWithValue("@TaskId", m.TaskId);
            command.Parameters.AddWithValue("@TaskName", m.TaskName);
            command.Parameters.AddWithValue("@ProjectId", m.ProjectId);
            command.Parameters.AddWithValue("@TaskDescription", m.TaskDescription);
            command.Parameters.AddWithValue("@StartDate", m.StartDate);
            command.Parameters.AddWithValue("@EstimateDate", m.EstimateDate);
            command.Parameters.AddWithValue("@Attachment", m.Attachment);
            command.Parameters.AddWithValue("@Employee_Id", m.Employee_Id);
            command.Parameters.AddWithValue("@UserComment", m.UserComment);
            command.Parameters.AddWithValue("@TaskPriority", m.TaskPriority);
            command.ExecuteNonQuery();
            connection.Close();



            connection.Open();
            using (SqlCommand com = new SqlCommand("SELECT Email FROM Tarun_TaskManagementSystemReg WHERE IsDeleted = 0 AND Employee_Id = @Employee_Id", connection))
            {
                com.Parameters.AddWithValue("@Employee_Id", m.Employee_Id);
                using (SqlDataReader drr = com.ExecuteReader())
                {
                    if (drr.Read())
                    {
                        Main obj = new Main();
                        obj.Email = drr["Email"].ToString();
                        drr.Close();

                        string color = "";
                        switch (m.TaskPriority)
                        {
                            case "Low":
                                color = "yellow";
                                break;
                            case "Medium":
                                color = "orange";
                                break;
                            case "High":
                                color = "red";
                                break;
                            default:
                                color = "black";
                                break;
                        }

                        string emailBody = @"
                                                <html>
                                                <head>
                                                    <style>
                                                        body {
                                                            font-family: Arial, sans-serif;
                                                            margin: 0;
                                                            padding: 0;
                                                            background-color: #f2f2f2;
                                                        }


                                                        .container {
                                                            max-width: 600px;
                                                            margin: 20px auto;
                                                            background-color: #fff;
                                                            border-radius: 10px;
                                                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                                        }

                                                        .header {
                                                            background-color: #007bff;
                                                            color: #fff;
                                                            padding: 20px;
                                                            border-top-left-radius: 10px;
                                                            border-top-right-radius: 10px;
                                                            text-align: center;
                                                        }

                                                        .content {
                                                            padding: 20px;
                                                        }

                                                        .task-detail {
                                                            margin-bottom: 10px;
                                                        }

                                                        .task-heading {
                                                            color: " + color + @";
                                                            font-size: 24px;
                                                            font-weight: bold;
                                                            margin-bottom: 20px;
                                                        }
                                                    </style>
                                                </head>
                                                <body>
                                                    <div class='container'>
                                                        <div class='header'>
                                                            <h2>Your Task Got Edited</h2>
                                                        </div>
                                                        <div class='content'>
                                                            <div class='task-heading'>Updated Task Details</div>
                                                            <div class='task-detail'><b>Task Name:</b> " + m.TaskName + @"</div>
                                                            <div class='task-detail'><b>Project Name:</b> " + m.ProjectName + @"</div>
                                                            <div class='task-detail'><b>Project ID:</b> " + m.ProjectId + @"</div>
                                                            <div class='task-detail'><b>Task Description:</b> " + m.TaskDescription + @"</div>
                                                            <div class='task-detail'><b>Start Date:</b> " + m.StartDate + @"</div>
                                                            <div class='task-detail'><b>Estimated Date:</b> " + m.EstimateDate + @"</div>
                                                            <div class='task-detail'><b>Task Priority:</b> " + m.TaskPriority + @"</div>
                                                        </div>
                                                    </div>
                                                </body>
                                                </html>";



                        MailMessage msg = new MailMessage();
                        msg.From = new MailAddress("tarun@concept.co.in");
                        msg.To.Add(obj.Email);
                        msg.Subject = "New Task Assigned to You";
                        msg.Body = emailBody;
                        msg.IsBodyHtml = true;
                        SmtpClient smt = new SmtpClient();
                        smt.Host = "mail.concept.co.in";
                        smt.Port = 25;
                        smt.Send(msg);
                    }
                }
            }

            ModelState.Clear();

            return RedirectToAction("AllTaskViewTable");

        }

        public ActionResult CommentUserAdmin(string id)
        {
            SqlConnection conn = new SqlConnection(ConnectionStringSettings);
            conn.Open();

            SqlCommand comma = new SqlCommand("Select UserComment FROM Tarun_AddTask where TaskId=@TaskId", conn);
            comma.Parameters.AddWithValue("@TaskId", Convert.ToInt32(id));
            SqlDataReader drrr = comma.ExecuteReader();
            Commentt ob = new Commentt();

            while (drrr.Read())
            {
                ob.UserComment = drrr["UserComment"].ToString();
            }
            ob.TaskId = Convert.ToInt32(id);
            conn.Close();
            return View(ob);

        }
        [HttpPost]
        public ActionResult CommentUserAdmin(Commentt m)
        {

            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();
            SqlCommand command = new SqlCommand("UPDATE Tarun_AddTask SET UserComment=@UserComment WHERE TaskId=@TaskId", connection);

            command.Parameters.AddWithValue("@TaskId", m.TaskId);
            command.Parameters.AddWithValue("@UserComment", m.UserComment);
            command.ExecuteNonQuery();
            connection.Close();

            return RedirectToAction("IndividualTaskAdmin");
        }

        //To see Employee data in grid
        public ActionResult IndividualTaskAdmin()
        {
            int ID = (int)Session["Employee_IdAdmin"];

            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<AllTaskViewTableEntity> objlst = new List<AllTaskViewTableEntity>();
            SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemTask_SelectDelete", connection);
            command.Parameters.AddWithValue("@Action", "SELECT ALL TASK USER");
            command.Parameters.AddWithValue("@Employee_Id", ID);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();

            AllTaskViewTableEntity objTask = null;

            while (dr.Read())
            {
                objTask = new AllTaskViewTableEntity();

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
        public ActionResult AddHoursOfUser(string Project_Id, string Task_Id)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();
            SqlCommand command = new SqlCommand("select ProjectHours  from TarunTMG_Project where ProjectId = @ProjectId ", connection);
            command.Parameters.AddWithValue("@ProjectId", Project_Id);
            SqlDataReader drrr = command.ExecuteReader();
            AddHoursAdmin ob = null;
            while (drrr.Read())
            {
                ob = new AddHoursAdmin();


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
        public ActionResult AddHoursOfUser(AddHoursAdmin m)
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
            return RedirectToAction("IndividualTaskAdmin");
        }

        public ActionResult AllDataView()
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<AllDataViewEntity> objlst = new List<AllDataViewEntity>();
            SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemReg_SelectDelete", connection);
            command.Parameters.AddWithValue("@Action", "SELECT ALL");
            command.Parameters.AddWithValue("@Employee_Id", 0);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();

            AllDataViewEntity obj = null;
            ////I was here on 19th solving bug
            while (dr.Read())
            {
                obj = new AllDataViewEntity();

                obj.Employee_Id = int.Parse(dr["Employee_Id"].ToString());
                obj.FirstName = dr["FirstName"].ToString();
                obj.LastName = dr["LastName"].ToString();
                obj.Date_of_Birth = dr["Date_of_Birth"].ToString();
                obj.Email = dr["Email"].ToString();
                obj.Address = dr["Address"].ToString();
                obj.Phone = dr["Phone"].ToString();
                obj.JoinedCompany = dr["JoinedCompany"].ToString();
                obj.UpdatedEntry = dr["UpdatedEntry"].ToString();
                obj.LeftCompany = dr["LeftCompany"].ToString();
                obj.IsDeleted = dr["IsDeleted"].ToString();
                obj.UserType = dr["UserType"].ToString();

                objlst.Add(obj);
            }
            return View(objlst);
        }

        public ActionResult Edit(string id)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemReg_EditUpdate", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "EDIT");
            command.Parameters.AddWithValue("@Employee_Id", Convert.ToInt32(id));
            //command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();
            Main obj = new Main();

            while (dr.Read())
            {

                obj.FirstName = dr["FirstName"].ToString();
                obj.LastName = dr["LastName"].ToString();
                obj.Date_of_Birth = dr["Date_of_Birth"].ToString();
                obj.Email = dr["Email"].ToString();
                obj.Address = dr["Address"].ToString();
                obj.Phone = dr["Phone"].ToString();
                obj.Password = dr["Password"].ToString();

            }
            obj.Employee_Id = Convert.ToInt32(id);
            connection.Close();
            return View(obj);
        }

        [HttpPost]
        public ActionResult Edit(Main m)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();
            SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemReg_EditUpdate", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "UPDATE");
            command.Parameters.AddWithValue("@Employee_Id", m.Employee_Id);
            command.Parameters.AddWithValue("@FirstName", m.FirstName);
            command.Parameters.AddWithValue("@LastName", m.LastName);
            command.Parameters.AddWithValue("@Date_of_Birth", m.Date_of_Birth);
            command.Parameters.AddWithValue("@Email", m.Email);
            command.Parameters.AddWithValue("@Address", m.Address);
            command.Parameters.AddWithValue("@Phone", m.Phone);
            command.Parameters.AddWithValue("@Password", m.Password);
            int result = command.ExecuteNonQuery();
            connection.Close();
            return RedirectToAction("AllDataView");
        }

        public ActionResult AllTaskViewTable()
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<AllTaskViewTableEntity> objlst = new List<AllTaskViewTableEntity>();
            SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemTask_SelectDelete", connection);
            command.Parameters.AddWithValue("@Action", "SELECT ALL TASK");
            command.Parameters.AddWithValue("@TaskId", 0);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();

            AllTaskViewTableEntity objTask = null;

            while (dr.Read())
            {
                objTask = new AllTaskViewTableEntity();

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
            return View(objlst);
        }


        public ActionResult GeneratePdfData()
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<AllDataViewEntity> objlst = new List<AllDataViewEntity>();
            SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemReg_SelectDelete", connection);
            command.Parameters.AddWithValue("@Action", "SELECT ALL");
            command.Parameters.AddWithValue("@Employee_Id", 0);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();

            AllDataViewEntity obj = null;
            ////I was here on 19th solving bug
            while (dr.Read())
            {
                obj = new AllDataViewEntity();

                obj.Employee_Id = int.Parse(dr["Employee_Id"].ToString());
                obj.FirstName = dr["FirstName"].ToString();
                obj.LastName = dr["LastName"].ToString();
                obj.Date_of_Birth = dr["Date_of_Birth"].ToString();
                obj.Email = dr["Email"].ToString();
                obj.Address = dr["Address"].ToString();
                obj.Phone = dr["Phone"].ToString();
                obj.JoinedCompany = dr["JoinedCompany"].ToString();
                obj.UpdatedEntry = dr["UpdatedEntry"].ToString();
                obj.LeftCompany = dr["LeftCompany"].ToString();
                obj.IsDeleted = dr["IsDeleted"].ToString();
                obj.UserType = dr["UserType"].ToString();

                objlst.Add(obj);
            }
            return new PartialViewAsPdf("GeneratePdfData", objlst)
            {
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageSize = Rotativa.Options.Size.A4,
                CustomSwitches = "--footer-center \" [page] Page of [toPage] Pages\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                FileName = "EmployeeList(Tarun).pdf"
            };
        }

        public ActionResult GeneratePdfTaskIndividual()
        {
            int ID = (int)Session["Employee_IdAdmin"];
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<AllTaskViewTableEntity> objlst = new List<AllTaskViewTableEntity>();
            SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemTask_SelectDelete", connection);
            command.Parameters.AddWithValue("@Action", "SELECT ALL TASK USER");
            command.Parameters.AddWithValue("@Employee_Id", ID);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();

            AllTaskViewTableEntity objTask = null;

            while (dr.Read())
            {
                objTask = new AllTaskViewTableEntity();

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
            return new PartialViewAsPdf("GeneratePdfTaskIndividual", objlst)
            {
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageSize = Rotativa.Options.Size.A4,
                CustomSwitches = "--footer-center \" [page] Page of [toPage] Pages\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                FileName = "YourTaskList.pdf"
            };
        }

        public ActionResult GeneratePdfTask()
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            List<AllTaskViewTableEntity> objlst = new List<AllTaskViewTableEntity>();
            SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemTask_SelectDelete", connection);
            command.Parameters.AddWithValue("@Action", "SELECT ALL TASK");
            command.Parameters.AddWithValue("@TaskId", 0);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();

            AllTaskViewTableEntity objTask = null;

            while (dr.Read())
            {
                objTask = new AllTaskViewTableEntity();

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
                PageSize = Rotativa.Options.Size.A3,
                CustomSwitches = "--footer-center \" [page] Page of [toPage] Pages\" --footer-line --footer-font-size \"9\" --footer-spacing 5 --footer-font-name \"calibri light\"",
                FileName = "TaskList(Tarun).pdf"
            };
            //return View(objlst);
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();
            SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemReg_EditUpdate", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "DELETE");
            command.Parameters.AddWithValue("@Employee_Id", Convert.ToInt32(id));
            int result = command.ExecuteNonQuery();
            connection.Close();
            return RedirectToAction("AllDataView");

        }
    }
}