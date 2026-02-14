using EduVerse.Models.BusinessLayer;
using EduVerse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using EduVerse.Models.Allclasses;
using static EduVerse.Models.Allclasses.CoursesModel;

namespace EduVerse.Controllers
{
    public class HomeController : Controller
    {
        StudentBAL cls = new StudentBAL();
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            DataTable dt = cls.cls_GetCount();
            ViewBag.count_courses = dt.Rows[0][0];
            ViewBag.count_student = dt.Rows[0][1];
            ViewBag.count_instructor = dt.Rows[0][2];

            DataTable dt2=cls.cls_GetCategories();
            List<CourseCategoryModel> categories = new List<CourseCategoryModel>();
            foreach(DataRow row in dt2.Rows)
            {
                CourseCategoryModel m = new CourseCategoryModel();
                m.CategoryID = Convert.ToInt32(row["CategoryId"]);
                m.CategoryName = row["CategoryName"].ToString();
                m.Thumbnail = row["ThumbNail"].ToString();
                m.CourseCount = Convert.ToInt32(row["Course_Count"]);
                categories.Add(m);
            }
            ViewBag.clist= categories;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Title = "About Page";

            return View();
        }
        public ActionResult Courses()
        {
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
    }
}
