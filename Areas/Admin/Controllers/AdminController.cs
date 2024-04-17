using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Task_Management_System2.Areas.Admin.Models;

namespace Task_Management_System2.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        string ConnectionStringSettings = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();

        [HttpGet]
        // GET: Admin/Admin
        public ActionResult Adminn()
        {
            LoginEntityy admin = new LoginEntityy();
            return View(admin);
        }

        [HttpPost]
        public ActionResult Adminn(LoginEntityy m)
        {
            SqlConnection conn = new SqlConnection(ConnectionStringSettings);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand selectCommand = new SqlCommand("SELECT Employee_Id ,FirstName, Password, UserType FROM Tarun_TaskManagementSystemReg WHERE Employee_Id = @Employee_Id and IsDeleted = 0 ", conn);
            selectCommand.Parameters.AddWithValue("@Employee_Id", m.Employee_Id);

            SqlDataReader reader = selectCommand.ExecuteReader();
            {
                if (reader.Read())
                {
                    int Employee_IdStored = Convert.ToInt32(reader["Employee_Id"]);
                    string storedPassword = Convert.ToString(reader["Password"]);
                    string FirstNameStored = Convert.ToString(reader["FirstName"]);
                    string UsertypeStored = Convert.ToString(reader["UserType"]);
                    string UsertypeLogin = "Admin";

                    if (m.Password == storedPassword && UsertypeStored == UsertypeLogin)
                    {
                        Session["Employee_IdAdmin"] = Employee_IdStored;
                        Session["FirstNameAdmin"] = FirstNameStored;
                        Session["UserAdmin"] = UsertypeStored;
                        TempData["Message"] = "LOGIN SUCCESSFUL!";
                        return RedirectToAction("AdminDashUser");

                    }
                    else
                    {
                        TempData["Message"] = "Please Check Your User Or Password.";
                        return RedirectToAction("Adminn");
                    }
                }
            }

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            return View();

        }
        [HttpGet]
        // GET: Admin/Admin
        public ActionResult RegistationAdmin()
        {
            Main reg = new Main();
            return View();
        }

        [HttpPost]
        public ActionResult RegistationAdmin(Main m)
        {
            string UserAdminn = (string)Session["UserAdmin"];

            if (UserAdminn == "Admin")
            {
                if (Convert.ToString(m.UserType).Equals("Admin"))
                {
                    SqlConnection connection = new SqlConnection(ConnectionStringSettings);
                    connection.Open();
                    SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemReg_EditUpdate", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "ADD_ADMIN");
                    command.Parameters.AddWithValue("@Employee_Id", m.Employee_Id);
                    command.Parameters.AddWithValue("@FirstName", m.FirstName);
                    command.Parameters.AddWithValue("@LastName", m.LastName);
                    command.Parameters.AddWithValue("@Date_of_Birth", m.Date_of_Birth);
                    command.Parameters.AddWithValue("@Email", m.Email);
                    command.Parameters.AddWithValue("@Address", m.Address);
                    command.Parameters.AddWithValue("@Phone", m.Phone);
                    command.Parameters.AddWithValue("@Password", m.Password);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                else
                {
                    SqlConnection connection = new SqlConnection(ConnectionStringSettings);
                    connection.Open();
                    SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemReg_EditUpdate", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "ADD_USER");
                    command.Parameters.AddWithValue("@Employee_Id", m.Employee_Id);
                    command.Parameters.AddWithValue("@FirstName", m.FirstName);
                    command.Parameters.AddWithValue("@LastName", m.LastName);
                    command.Parameters.AddWithValue("@Date_of_Birth", m.Date_of_Birth);
                    command.Parameters.AddWithValue("@Email", m.Email);
                    command.Parameters.AddWithValue("@Address", m.Address);
                    command.Parameters.AddWithValue("@Phone", m.Phone);
                    command.Parameters.AddWithValue("@Password", m.Password);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                ModelState.Clear();
            }

            return View();
        }

        [HttpGet]
        public ActionResult AdminDashUser()
        {
            if (Session["Employee_IdAdmin"] != null)
            {
                int EmplyeeId = (int)Session["Employee_IdAdmin"];

                SqlConnection connection = new SqlConnection(ConnectionStringSettings);
                connection.Open();

                SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemReg_SelectDelete", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Action", "SELECT");
                command.Parameters.AddWithValue("@Employee_Id", EmplyeeId);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                SqlDataReader dr = command.ExecuteReader();
                Models.AdminDashBoardEntity objReg = new Models.AdminDashBoardEntity();

                while (dr.Read())
                {
                    objReg.Employee_Id = EmplyeeId;
                    objReg.FirstName = dr["FirstName"].ToString();
                    objReg.LastName = dr["LastName"].ToString();
                    objReg.Date_of_Birth = (dr["Date_of_Birth"].ToString());
                    //objReg.Date_of_Birth = DateTime.Parse(dr["Date_of_Birth"].ToString()).Date;
                    objReg.Address = dr["Address"].ToString();
                    objReg.Phone = dr["Phone"].ToString();
                    objReg.Email = dr["Email"].ToString();

                }

                connection.Close();
                return View(objReg);
            }
            else
            {
                return RedirectToAction("Adminn");
            }
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Adminn");
        }

        public ActionResult AdminForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdminForgotPassword(ForgetPasswordAdmin m)
        {

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("tarun@concept.co.in");
            msg.To.Add(m.Email);
            string url = "http://localhost:60880/Admin/Admin/AdminChangePassword?EncryptedEmail=" + HttpUtility.UrlEncode(Encrypt(m.Email));
            msg.Subject = "Forget Password Mail";
            msg.Body = "Click Here To Change Your Password : " + url;
            Session["url"] = url;
            string a = (string)Session["url"] ;
            msg.IsBodyHtml = true;

            SmtpClient smt = new SmtpClient();
            smt.Host = "mail.concept.co.in";
            smt.Port = 25;
            smt.Send(msg);
            ModelState.Clear();
            return RedirectToAction("Adminn");
        }
        public ActionResult AdminChangePassword(string EncryptedEmail)
        {
            string a = (string)Session["url"];
            if (a != null) { 
            ForgetPasswordAdmin ob = new ForgetPasswordAdmin();
            ob.Email = Decrypt(HttpUtility.UrlDecode(EncryptedEmail));
            return View(ob);
            }
            return View("Adminn");
        }
        [HttpPost]
        public ActionResult AdminChangePassword(ForgetPasswordAdmin m)
        {
            string a = (string)Session["url"];
            if (a != null)
            {

                SqlConnection conn = new SqlConnection(ConnectionStringSettings);
                conn.Open();

                SqlCommand selectCommand = new SqlCommand("Update Tarun_TaskManagementSystemReg set Password = @Password WHERE Email = @Email", conn);

                selectCommand.Parameters.AddWithValue("@Email", m.Email);
                selectCommand.Parameters.AddWithValue("@Password", m.NewPassword);
                selectCommand.ExecuteNonQuery();
                selectCommand.Dispose();
                conn.Close();
                Session.Clear();
                Session.Abandon();
                return View("Adminn");
            }
            return View("Adminn");
        }

        private string Encrypt(string stringToEncrypt)
        {
            byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
            byte[] rgbIV = { 0x21, 0x43, 0x56, 0x87, 0x10, 0xfd, 0xea, 0x1c };
            byte[] key = { };
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes("A0D1nX0Q");
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, rgbIV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        private string Decrypt(string EncryptedText)
        {
            byte[] inputByteArray = new byte[EncryptedText.Length + 1];
            byte[] rgbIV = { 0x21, 0x43, 0x56, 0x87, 0x10, 0xfd, 0xea, 0x1c };
            byte[] key = { };

            try
            {
                key = System.Text.Encoding.UTF8.GetBytes("A0D1nX0Q");
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(EncryptedText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, rgbIV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}