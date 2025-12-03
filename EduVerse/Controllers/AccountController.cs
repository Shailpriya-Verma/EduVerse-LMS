using EduVerse.Models.AllClasses;
using EduVerse.Models.BusinessLayer;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using static EduVerse.Models.AllClasses.AccountModel;
using static EduVerse.Models.BusinessLayer.AccountBAL;

namespace EduVerse.Controllers
{
    public class AccountController : Controller
    {
        AccountModel cls = new AccountModel();
        AccountBAL acc = new AccountBAL();
        public ActionResult Login()
        
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult RegisterInstructor()
        {
            return View();
        }

        #region Login

        [HttpPost]
        public ActionResult UserLogin(UserLogin obj)
        {
            try
            {
                DataTable dt = acc.cls_Login(obj);

                if (dt == null || dt.Rows.Count == 0)
                    return Json(new { status = -1, message = "Invalid credentials" });
                int Status = Convert.ToInt32(dt.Rows[0]["Status"]);
                string Message = dt.Rows[0]["Message"].ToString();

                if (Status==-1)
                {
                    return Json(new { status = Status, message = Message });
                }
                if (Status == -2)
                {
                    return Json(new { status = Status, message = Message });
                }

                string fullName = dt.Rows[0]["FullName"]?.ToString() ?? "";
                int userId = Convert.ToInt32(dt.Rows[0]["UserId"]);
                int roleId = obj.roleId;
                // for session
                Session["UserId"] = userId;
                Session["FullName"] = fullName;
                Session["RoleId"] = obj.roleId;

                return Json(new
                {
                    status = 1,
                    message = "Login Successful!",
                    fullName = fullName,
                    userId,
                    roleId
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = "Unexpected error: " + ex.Message });
            }
        }

        #endregion Login


        #region Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }
        #endregion Logout

        #region ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }
        #endregion ForgotPassword


        #region SendOtp
        [HttpPost]
        public JsonResult SendOtp(string email)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            Session["OTP"] = otp;
            Session["OTPExpiry"] = DateTime.Now.AddMinutes(5);
            Session["OTPEmail"] = email;

            SendOtpEmail(email, otp);
            return Json(new { success = true });
        }
        #endregion SendOtp


        #region SendOtpEmail
        private void SendOtpEmail(string toEmail, string otp)
        {
            var mail = new MailMessage();
            mail.To.Add(toEmail);
            mail.Subject = "🔐 One-Time Password (OTP) for Your Account Access";
            mail.From = new MailAddress("shailpriyav222@gmail.com", "EduVerse Support");

            // Use HTML content for styling
            mail.IsBodyHtml = true;
            mail.Body = $@"
        <div style='font-family: Arial, sans-serif; padding: 20px; background-color: #f9f9f9;'>
            <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                <h2 style='color: #4CAF50;'>OTP Verification</h2>
                <p>Dear user,</p>
                <p>Your One-Time Password (OTP) for accessing your EduVerse account is:</p>
                <div style='font-size: 24px; font-weight: bold; color: #333; background-color: #f0f0f0; padding: 10px; text-align: center; border-radius: 4px;'>
                    {otp}
                </div>
                <p style='margin-top: 20px;'>This OTP is valid for <strong>5 minutes</strong>. Please do not share it with anyone.</p>
                <hr />
                <p style='font-size: 12px; color: #777;'>If you didn’t request this OTP, please ignore this message.</p>
                <p style='font-size: 12px; color: #777;'>Regards,<br/>EduVerse Support Team</p>
            </div>
        </div>";

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("shailpriyav222@gmail.com", "ekzi vorv mrnn fami"), // App Password (not your real password)
                EnableSsl = true
            };

            smtp.Send(mail);
        }

        #endregion SendOtpEmail

        #region VerifyOtp
        [HttpPost]
        public JsonResult VerifyOtp(string otp)
        {
            if (Session["OTP"] == null || Session["OTPExpiry"] == null)
                return Json(new { success = false, message = "Session expired or no OTP sent!" });

            if (Session["OTP"].ToString() != otp)
                return Json(new { success = false, message = "Invalid OTP!" });
            if (DateTime.Now > (DateTime)Session["OTPExpiry"])
                return Json(new { success = false, message = "Expired OTP!" });

            return Json(new { success = true });
        }
        #endregion VerifyOtp

        #region ResetPassword
        [HttpPost]
        public JsonResult ResetPassword(string email, string newPassword, string confirmPassword,int roleId)
        {
            int res = acc.cls_ResetPassword(email, newPassword, roleId);

            if (res>=1)
            {
                // Clear OTP session after reset
                Session.Remove("OTP");
                Session.Remove("OTPExpiry");
                Session.Remove("OTPEmail");

                return Json(new { success = true });
            }

            return Json(new { success = false, message = "User not found!" });
        }
        #endregion ResetPassword
    }
}