using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Task_Management_System2.Models;

namespace Task_Management_System2.Controllers
{
    public class LoginController : Controller
    {
        string ConnectionStringSettings = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        // GET: Loginc
        [HttpGet]
        public ActionResult Index()
        {
            LoginEntity user = new LoginEntity();
            return View(user);
        }

        [HttpPost]
        public ActionResult Index(LoginEntity m)
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
                    string FirstNameStored= Convert.ToString(reader["FirstName"]);
                    string UsertypeStored = Convert.ToString(reader["UserType"]);
                    string UsertypeLogin = "User";

                    if (m.Password == storedPassword && UsertypeStored== UsertypeLogin)
                    {
                        Session["Employee_IdUser"] = Employee_IdStored;
                        Session["FirstNameUser"] = FirstNameStored;

                        return RedirectToAction("DashUser");

                    }
                }
            }
           
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            return View();

        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(ForgetPassword m)
        {
            
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("tarun@concept.co.in");
            msg.To.Add(m.Email);
            string url = "http://localhost:60880/Login/ChangePassword?EncryptedEmail=" + HttpUtility.UrlEncode(Encrypt(m.Email));
            msg.Subject = "Forget Password Mail";
            msg.Body = "Click Here To Change Your Password : " + url;
            Session["url"] = url;
            msg.IsBodyHtml = true;

            SmtpClient smt = new SmtpClient();
            smt.Host = "mail.concept.co.in";
            smt.Port = 25;
            smt.Send(msg);
            ModelState.Clear();
            return RedirectToAction("Index");
        }
        public ActionResult ChangePassword(string EncryptedEmail)
        {
            ForgetPassword ob = new ForgetPassword();
            ob.Email = Decrypt(HttpUtility.UrlDecode(EncryptedEmail));
            return View(ob);
        }
        [HttpPost]
        public ActionResult ChangePassword(ForgetPassword m)
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
            return View("Index");
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


        [HttpGet]
        public ActionResult DashUser()
        {
            int EmplyeeId = (int)Session["Employee_IdUser"];


            SqlConnection connection = new SqlConnection(ConnectionStringSettings);
            connection.Open();

            SqlCommand command = new SqlCommand("Tarun_TaskManagementSystemReg_SelectDelete", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Action", "SELECT");
            command.Parameters.AddWithValue("@Employee_Id", EmplyeeId);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlDataReader dr = command.ExecuteReader();
            Models.UserDashBoardEntity objReg = new Models.UserDashBoardEntity();

            while (dr.Read())
            {
                objReg.Employee_Id = EmplyeeId;
                objReg.FirstName = dr["FirstName"].ToString();
                objReg.LastName = dr["LastName"].ToString();
                objReg.Date_of_Birth = (dr["Date_of_Birth"].ToString());
                objReg.Address = dr["Address"].ToString();
                objReg.Phone = dr["Phone"].ToString();
                objReg.Email = dr["Email"].ToString();

            }
            connection.Close();
            return View(objReg);
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index");
        }


    }
}