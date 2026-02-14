using EduVerse.Models.Allclasses;
using EduVerse.Models.AllClasses;
using EduVerse.Models.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using static EduVerse.Models.Allclasses.CoursesModel;
using static EduVerse.Models.Allclasses.StudentModel;

namespace EduVerse.Controllers
{
    public class StudentController : Controller
    {
        StudentBAL cls = new StudentBAL();
        PaymentBAL paycls = new PaymentBAL();

        #region StudentDashboard
        public ActionResult StudentDashboard()
        {
            if (Session["UserId"] == null || Session["RoleId"].Equals("2"))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
        #endregion StudentDashboard

        #region Courses
        public ActionResult Courses()
        {

            if (Session["UserId"] == null || Session["RoleId"].Equals("2"))
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Title = "Courses";

            DataTable dt = cls.cls_GetAllCourses();
            List<GetAllCourseModel> list = new List<GetAllCourseModel>();

            foreach (DataRow data in dt.Rows)
            {
                GetAllCourseModel obj2 = new GetAllCourseModel();
                obj2.CourseId = Convert.ToInt32(data["CourseId"]);
                obj2.Title = data["Title"].ToString();
                obj2.Price = data["Price"] != DBNull.Value ? Convert.ToDecimal(data["Price"]) : 0;
                obj2.IsPaid = Convert.ToBoolean(data["IsPaid"]);
                obj2.InstructorName = data["InstructorName"].ToString();
                obj2.ThumbnailPath = data["Thumbnail"].ToString();
                list.Add(obj2);
            }

            return View(list);
        }
        #endregion Courses


        #region InsertEnrollCourse
        [HttpPost]
        public ActionResult EnrollCourse(int courseId, bool isPaid, decimal amount)
        {
            try
            {
                if (Session["UserId"] == null || Session["RoleId"].Equals("2"))
                {
                    return RedirectToAction("Login", "Account");
                }

                int studentId = Convert.ToInt32(Session["UserId"]);

                // Insert into Enrollments table
                EnrollCourseModel obj = new EnrollCourseModel
                {
                    courseId = courseId,
                    studentId = studentId,
                    paymentStatus = isPaid ? "Pending" : "Free"
                };

                int enrollResult = cls.cls_EnrollCourse(obj);

                // If already enrolled
                if (enrollResult == -1)
                {
                    return Json(new { res = -1, success = true, message = "You are already enrolled in this course." });
                }

                string transactionId = null;

                // If course is paid, create a pending payment entry
                if (isPaid && enrollResult == 1)
                {
                    transactionId = Guid.NewGuid().ToString();

                    PaymentModel payment = new PaymentModel
                    {
                        UserId = studentId,
                        CourseId = courseId,
                        Amount = amount,
                        PaymentStatus = "Pending",
                        TransactionId = transactionId
                    };

                    int paymentResult = paycls.InsertPayment(payment);

                }

                return Json(new
                {
                    res = 1,
                    success = true,
                    transactionid = transactionId,
                    message = isPaid ? "Enrolled successfully. Proceed to payment." : "Enrolled successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion InsertEnrollCourse







        [HttpPost]
        public ActionResult CreatePayment(int courseId)
        {
            try
            {
                int studentId = Convert.ToInt32(Session["UserId"]);

                string transactionId = Guid.NewGuid().ToString();

                PaymentModel payment = new PaymentModel
                {
                    UserId = studentId,
                    CourseId = courseId,
                    TransactionId = transactionId,
                    PaymentStatus = "Pending"
                };

                paycls.InsertPayment(payment);

                return Json(new
                {
                    success = true,
                    TransactionId = transactionId
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        #region VerifyPayment

        [HttpPost]
        public ActionResult VerifyPayment(string transactionId, int courseId)
        {
            try
            {
                if (Session["UserId"] == null || Session["RoleId"].Equals("2"))
                {
                    return RedirectToAction("Login", "Account");
                }

                int studentId = Convert.ToInt32(Session["UserId"]);

                // Update Payments table
                PaymentModel paymentUpdate = new PaymentModel
                {
                    TransactionId = transactionId,
                    PaymentStatus = "Paid"
                };
                int res1 = paycls.UpdatePaymentStatus(paymentUpdate);

                // Update Enrollments table
                EnrollCourseModel enrollUpdate = new EnrollCourseModel
                {
                    courseId = courseId,
                    studentId = studentId,
                    paymentStatus = "Paid"
                };
                int res2 = paycls.UpdateEnrollmentStatus(enrollUpdate);

                return Json(new { success = true, message = "Payment verified successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        #endregion VerifyPayment


        #region EnrolledCourse
        public ActionResult EnrolledCourses()
        {
            if (Session["UserId"] == null || Session["RoleId"].Equals("2"))
            {
                return RedirectToAction("Login", "Account");
            }

            int studentId = Convert.ToInt32(Session["UserId"]);
            ViewBag.Title = "Courses";

            DataTable dt = cls.cls_GetCoursesByStudentId(studentId);
            List<GetAllCourseByStudentIdModel> list = new List<GetAllCourseByStudentIdModel>();

            foreach (DataRow data in dt.Rows)
            {
                GetAllCourseByStudentIdModel obj = new GetAllCourseByStudentIdModel();
                obj.CourseId = Convert.ToInt32(data["CourseId"]);
                obj.Title = data["Title"].ToString();
                obj.ThumbnailPath = data["Thumbnail"].ToString();
                obj.InstructorName = data["InstructorName"].ToString();
                obj.EnrolledOn = Convert.ToDateTime(data["EnrolledOn"]);
                obj.PaymentStatus = data["PaymentStatus"].ToString();
                obj.Price = data["Amount"] == DBNull.Value ? (decimal?)null
                        : Convert.ToDecimal(data["Amount"]);
                list.Add(obj);
            }

            return View(list);
        }

        #endregion EnrolledCourse



        #region ViewEnrollCourse

        public ActionResult ViewCourseDetails(int courseId)
        {
            if (Session["UserId"] == null || Session["RoleId"].Equals("2"))
            {
                return RedirectToAction("Login", "Account");
            }

            DataTable dt = cls.cls_GetCourseDetailByCourseId(courseId);
            if (dt.Rows.Count == 0)
            {
                return HttpNotFound();
            }

            var model = new GetAllCourseByStudentIdModel();
            model.Materials = new List<CourseMaterialModel>();

            // fill header info from first row
            var header = dt.Rows[0];
            model.CourseId = Convert.ToInt32(header["CourseId"]);
            model.Title = header["Title"].ToString();
            model.InstructorName = header["InstructorName"].ToString();
            model.EnrolledOn = Convert.ToDateTime(header["EnrolledOn"]);
            model.PaymentStatus = header["PaymentStatus"].ToString();
            model.ThumbnailPath = header["ThumbnailPath"].ToString();
            model.Description = header["Description"].ToString();
            


            // loop through all materials
            foreach (DataRow row in dt.Rows)
            {
                if (row["MaterialId"] != DBNull.Value)
                {
                    model.Materials.Add(new CourseMaterialModel
                    {
                        MaterialId = Convert.ToInt32(row["MaterialId"]),
                        Title = row["MaterialTitle"].ToString(),
                        FilePath = row["FilePath"].ToString(),
                        MaterialType = row["MaterialType"].ToString()
                    });
                }
            }

            return View("CourseDetails", model);
        }

        #endregion ViewEnrollCourse




        #region CourseDetails
        public ActionResult CourseDetails()
        {
            if (Session["UserId"] == null || Session["RoleId"].Equals("2"))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        #endregion CourseDetails


        #region Assignment
        public ActionResult Assignment()
        {
            if (Session["UserId"] == null || Session["RoleId"].Equals("2"))
            {
                return RedirectToAction("Login", "Account");
            }
            //int st_id = Convert.ToInt32(Session["UserId"]);
            
            //DataTable table = cls.cls_GetAssignmentByStudentId(st_id);
            return View();
        }
        #endregion Assignment


        #region GetAssignmentByStudentId
        [HttpGet]
        public JsonResult GetAssignmentByStudentId()
        {
            int st_id = Convert.ToInt32(Session["UserId"]);
            DataTable dt = cls.cls_GetAssignmentByStudentId(st_id);

            List<object> assignments = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                assignments.Add(new
                {
                    AssessmentId = row["AssessmentId"],
                    Title = row["Title"].ToString(),
                    Description = row["Description"].ToString(),
                    DueDate = Convert.ToDateTime(row["DueDate"]).ToString("dd-MM-yyyy"),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"]).ToString("dd-MM-yyyy"),
                    FilePath = row["FilePath"].ToString(),
                    IsSubmitted = row["IsSubmitted"] != DBNull.Value,
                    Grade = row["Grade"] == DBNull.Value ? "" : row["Grade"].ToString(),
                    Feedback = row["Feedback"] == DBNull.Value ? "" : row["Feedback"].ToString()
                });
            }

            return Json(new { success = true, data = assignments }, JsonRequestBehavior.AllowGet);
        }


        #endregion GetAssignmentByStudentId






        #region SubmitAssignment


        [HttpPost]
        public ActionResult SubmitAssignment(int AssessmentId,HttpPostedFileBase AssignmentFile)
        {
            if (Session["UserId"] == null || Session["RoleId"].Equals("2"))
            {
                return RedirectToAction("Login", "Account");
            }

            int StudentId=Convert.ToInt32(Session["UserId"]);

            Random rnd = new Random();
            string filename = rnd.Next(1000, 9999) + AssignmentFile.FileName;
            string SubmissionPath = Path.Combine("/Content/Uploads/SubmittedAssignments/", filename);
            string fullpath = Server.MapPath(SubmissionPath);
            AssignmentFile.SaveAs(fullpath);

            int res = cls.cls_SubmitAssignment(StudentId, AssessmentId, SubmissionPath);

            if (res > 0)
            {
                return Json(new { success = true, message = "Assignment submitted successfully!" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to submit assignment !!" });
            }

        }

        #endregion SubmitAssignment
    }
}