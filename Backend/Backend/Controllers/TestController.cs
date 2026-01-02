using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Backend.Models;
using Backend.Controllers;
using System.Web;

namespace Backend.Controllers
{
    [RoutePrefix("api/Test")]
    public class TestController : ApiController
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);
        SqlCommand cmd = null;
        SqlDataAdapter da = null;

        [HttpPost]
        [Route("Registeration")]
        public string Registeration(Employee employee)
        {
            string msg = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(employee.Name?.Trim()))
                {
                    msg = "Name cannot be empty";
                    return msg;
                }

                if (!IsValidPhoneNumber(employee.PhoneNo))
                {
                    msg = "Phone number format is invalid";
                    return msg;
                }

                if (string.IsNullOrWhiteSpace(employee.Address?.Trim()))
                {
                    msg = "Address cannot be empty";
                    return msg;
                }

                if (IsDuplicateEmail(employee.Email.Trim()))
                {
                    return "Email already exists. Please use a different name.";
                }

                if (IsDuplicatePassword(employee.Password.Trim()))
                {
                    return "Password already registered. Each phone number can only be used once.";
                }

                cmd = new SqlCommand("USP_REGISTERTAION", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", employee.Name);
                cmd.Parameters.AddWithValue("@PhoneNo", employee.PhoneNo);
                cmd.Parameters.AddWithValue("@Address", employee.Address);
                cmd.Parameters.AddWithValue("@IsActive", employee.IsActive);
                cmd.Parameters.AddWithValue("@Email", employee.Email);
                cmd.Parameters.AddWithValue("@Password", employee.Password);

                conn.Open();
                int i = cmd.ExecuteNonQuery();  
                conn.Close();
                if (i > 0)
                {
                    msg = "User Register Successfully.";
                }
                else
                {
                    msg = "Something is Wrong!";
                }
            }
            catch (Exception ex)
            {
                CommonAPIController.Log("Registeration Exception:-"+ ex.Message);
                msg = ex.Message;
            }
       
            return msg;
        }


        [HttpPost]
        [Route("Login")]
        public string Login(Employee employee)
        {
            string msg = string.Empty;
            try
            {  
                da = new SqlDataAdapter("USP_LOGIN",conn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Email", employee.Email);
                da.SelectCommand.Parameters.AddWithValue("@Password", employee.Password);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    var ID = dt.Rows[0]["ID"].ToString();
                    var Name = dt.Rows[0]["Name"].ToString(); 
                    var PhoneNo = dt.Rows[0]["PhoneNo"].ToString(); 
                    msg = "User Login Successfully.";
                }
                else
                {
                    msg = "Something is Wrong!";
                }
            }
            catch (Exception ex)
            {
                CommonAPIController.Log("Login Exception:-" + ex.Message);
                msg = ex.Message;
            }

            return msg;
        }

        private bool IsValidPhoneNumber(string phoneNo)
        {
            if (string.IsNullOrEmpty(phoneNo))
                return false;
            string cleanPhone = System.Text.RegularExpressions.Regex.Replace(phoneNo, @"[^\d]", "");


            return cleanPhone.Length >= 1 && cleanPhone.Length <= 10;
        }

        private bool IsDuplicateEmail(string Email)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM EmpManagement WHERE Email = @Email AND IsActive = 1";
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", Email);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                conn.Close();

                return count > 0;
            }
            catch
            {
                conn.Close();
                return false; 
            }
        }

        private bool IsDuplicatePassword(string password)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM EmpManagement WHERE Password = @Password AND IsActive = 1";
                cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                conn.Close();

                return count > 0;
            }
            catch
            {
                conn.Close();
                return false; 
            }
        }
    }
}
