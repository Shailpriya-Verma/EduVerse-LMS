using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static EduVerse.Models.Allclasses.InstructorModel;
using EduVerse.Models;
using Org.BouncyCastle.Asn1.X500;
using EduVerse.Models.BusinessLayer;

namespace EduVerse.Controllers
{
    public class InstructorController : Controller
    {
        DBAccess db = new DBAccess();
        InstructorBAL cls = new InstructorBAL();
        public ActionResult CreateCourse()
        {
            if (Session["UserId"] == null || Session["RoleId"].Equals("3"))
            {
                return RedirectToAction("Login", "Account"); 
            }
            return View();
        }


        #region InsertCourse
        [HttpPost]
        public ActionResult InsertCourse(CourseModel obj)
        {
            try
            {
                if (Session["UserId"] == null || Session["RoleId"].Equals("3"))
                    return Json(new { status = -1, message = "Session expired. Please login again." });

                obj.TeacherId = Convert.ToInt32(Session["UserId"]);

                // To upload thumbnail
                if (obj.Thumbnail != null)
                {
                    string picName = Guid.NewGuid() + Path.GetExtension(obj.Thumbnail.FileName);
                    string thumbPath = Path.Combine(Server.MapPath("~/Content/UploadedCourseMaterials/"), picName);
                    obj.Thumbnail.SaveAs(thumbPath);
                    obj.ThumbnailPath = "/Content/UploadedCourseMaterials/" + picName;
                }

                //To save Course and get newly created CourseId
                int courseId = cls.SaveCourseBasic(obj);

                // To save all uploaded materials
                if (obj.CourseMaterials != null && obj.CourseMaterials.Count > 0)
                {
                    for (int i = 0; i < obj.CourseMaterials.Count; i++)
                    {
                        var file = obj.CourseMaterials[i];
                        if (file != null)
                        {
                            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                            string savePath = Path.Combine(Server.MapPath("~/Content/UploadedCourseMaterials/"), fileName);
                            file.SaveAs(savePath);

                            CourseMaterialModel material = new CourseMaterialModel
                            {
                                CourseId = courseId,
                                Title = obj.MaterialTitles[i],
                                FilePath = "/Content/UploadedCourseMaterials/" + fileName,
                                MaterialType = obj.MaterialTypes[i]
                            };

                            cls.SaveCourseMaterial(material);
                        }
                    }
                }

                return Json(new { status = 1, message = "Course created successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = ex.Message });
            }
        }


        #endregion  InsertCourse



        #region Assignment
        public ActionResult Assignment()
        {
            if (Session["UserId"] == null || Session["RoleId"].Equals("3"))
            {
                return RedirectToAction("Login", "Account");
            }

            // To Get Course details by TeacherId
            int id = Convert.ToInt32(Session["UserId"]);
            DataTable table = cls.getcoursedetailsByInstructorId(id);

            return View(table);
        }
        #endregion Assignment

        #region SaveAssignment
        [HttpPost]
        public JsonResult SaveAssignment(AssignmentModel model)
        {
            string filePath = "";

            if (model.AssignmentFile != null && model.AssignmentFile.ContentLength > 0)
            {
                string ext = Path.GetExtension(model.AssignmentFile.FileName).ToLower();
                string[] allowedExts = { ".jpg", ".jpeg", ".png", ".pdf" };

                if (allowedExts.Contains(ext))
                {
                    Random rnd=new Random();
                    string fileName = rnd.Next(100, 999)+Path.GetFileName(model.AssignmentFile.FileName);
                    filePath = Path.Combine(Server.MapPath("~/Content/Uploads/Assignments/"), fileName);
                    model.AssignmentFile.SaveAs(filePath);

                    // Save relative path to DB
                    model.AssignmentFilePath = "/Content/Uploads/Assignments/" + fileName;
                }
                else
                {
                    return Json(new { success = false, message = "Invalid file type. Only JPG, PNG, or PDF allowed." });
                }
            }

            int result = cls.SaveAssignment(model); 

            if (result > 0)
            {
                return Json(new { success = true, message = "Assignment created successfully!" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to save assignment." });
            }

        }

        #endregion SaveAssignment



        #region ViewCourses
        public ActionResult ViewCourses()
        {
            if (Session["UserId"] == null || Session["RoleId"].Equals("3"))
            {
                return RedirectToAction("Login", "Account");
            }

            int Ins_id = Convert.ToInt32(Session["UserId"]);
            DataTable table=cls.GetCoursesByInstructorId(Ins_id);
            return View(table);
        }
        #endregion ViewCourses


        #region Submissions

        public ActionResult Submissions()
        {
            if (Session["UserId"] == null || Session["RoleId"].Equals("3"))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        #endregion Submissions

        #region GetSubmissionsByInstructor
        [HttpGet]
        public JsonResult GetSubmissionsByInstructor()
        {

            int instructorId = Convert.ToInt32(Session["UserId"]);

            DataTable dt = cls.cls_GetSubmissionsByInstructor(instructorId);

            List<object> submissions = new List<object>();
            if (dt != null && dt.Rows.Count>0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    submissions.Add(new
                    {
                        SubmissionId = row["SubmissionId"].ToString(),
                        StudentName = row["StudentName"].ToString(),
                        AssessmentTitle = row["AssessmentTitle"].ToString(),
                        CourseName = row["CourseName"].ToString(),
                        DueDate = Convert.ToDateTime(row["DueDate"]).ToString("dd-MM-yyyy"),
                        SubmittedOn = Convert.ToDateTime(row["SubmittedOn"]).ToString("dd-MM-yyyy"),
                        FilePath = row["SubmissionPath"].ToString(),
                        Grade = row["Grade"] == DBNull.Value ? "" : row["Grade"].ToString(),
                        Feedback = row["Feedback"] == DBNull.Value ? "" : row["Feedback"].ToString()
                    });
                }

            }

            return Json(submissions, JsonRequestBehavior.AllowGet);
        }

        #endregion GetSubmissionsByInstructor



        #region UpdateGradeFeedback
        [HttpPost]
        public JsonResult UpdateGradeFeedback(int submissionId, int grade, string feedback)
        
        {
            try
            {
                int result=cls.cls_UpdateSubmissionGradeFeedback(submissionId, grade, feedback);
                if (result > 0)
                {
                    return Json(new { success = true, message = "Grade & feedback updated successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to update. Please try again." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
        #endregion

    }
}