using EduVerse.Models.AllClasses;
using EduVerse.Models.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using static EduVerse.Models.AllClasses.AccountModel;

namespace EduVerse.Controllers.APIController
{
    public class AccountApiController : ApiController
    {
        AccountModel cls = new AccountModel();
        AccountBAL acc = new AccountBAL();


        #region Register
        [HttpPost]
        [Route("api/AccountApi/Register")]
        public IHttpActionResult Register(Register obj)
        {
            try
            {
                if (obj == null)
                    return BadRequest("Invalid data.");

                if (string.IsNullOrWhiteSpace(obj.name) ||
                    string.IsNullOrWhiteSpace(obj.email) ||
                    string.IsNullOrWhiteSpace(obj.password) ||
                    obj.roleId <= 0)
                {
                    return BadRequest("All fields are required");
                }

                // Email format validation
                if (!System.Text.RegularExpressions.Regex.IsMatch(obj.email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    return BadRequest("Invalid email format.");
                }

                int result = acc.cls_Register(obj);

                if (result == 1)
                    return Ok(new { status = 1, message = "Registered Successfully!" });
                else if (result == -1)
                    return Content(HttpStatusCode.Conflict, new { status = -1, message = "User Already Registered For This Role" });
                else
                    return Content(HttpStatusCode.InternalServerError, new { status = 0, message = "Registration failed!" });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new { status = 0, message = "Unexpected error: " + ex.Message });
            }
        }

        #endregion Register


        //[HttpPost]
        //[Route("api/AccountApi/Login")]
        //public IHttpActionResult Login(Login obj)
        //{
        //    try
        //    {
        //        if (obj == null || string.IsNullOrWhiteSpace(obj.email) || string.IsNullOrWhiteSpace(obj.password) || obj.roleId <= 0)
        //            return BadRequest("Email, Password, and Role are required.");

        //        // Step 1: Get password hash for email + role
        //        DataTable dt = acc.GetPasswordHash(obj.email, obj.roleId);

        //        if (dt == null || dt.Rows.Count == 0)
        //            return Content(HttpStatusCode.Unauthorized, new { status = -1, message = "Invalid credentials or role." });

        //        string storedHash = dt.Rows[0]["PasswordHash"].ToString();
        //        string fullName = dt.Rows[0]["FullName"].ToString();
        //        string email = dt.Rows[0]["Email"].ToString();
        //        string role = dt.Rows[0]["RoleName"].ToString();
        //        int userId = Convert.ToInt32(dt.Rows[0]["UserId"]);

        //        // Step 2: Verify password
        //        bool isValid = Crypto.VerifyHashedPassword(storedHash, obj.password);
        //        if (!isValid)
        //            return Content(HttpStatusCode.Unauthorized, new { status = -1, message = "Incorrect password." });

        //        // Step 3: Success response
        //        return Ok(new
        //        {
        //            status = 1,
        //            message = "Login Successful!",
        //            userId = userId,
        //            fullName = fullName,
        //            email = email,
        //            roleId = obj.roleId,
        //            role = role,
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(HttpStatusCode.InternalServerError, new { status = 0, message = "Unexpected error: " + ex.Message });
        //    }
        //}



    }
}