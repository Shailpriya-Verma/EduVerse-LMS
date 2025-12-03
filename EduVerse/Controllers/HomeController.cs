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
