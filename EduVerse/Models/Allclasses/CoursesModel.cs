using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EduVerse.Models.Allclasses
{
    public class CoursesModel
    {
        public class GetAllCourseModel
        {
            public int CourseId { get; set; }
            public string Title { get; set; }
            public string ThumbnailPath { get; set; }
            public string InstructorName { get; set; }
            public bool IsPaid { get; set; }
            public decimal? Price { get; set; }

        }

        public class GetAllCourseByStudentIdModel
        {
            public int CourseId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string ThumbnailPath { get; set; }
            public string InstructorName { get; set; }
            public DateTime EnrolledOn { get; set; }
            public string PaymentStatus { get; set; }
            public decimal? Price { get; set; }

            // hold multiple materials
            public List<CourseMaterialModel> Materials { get; set; }
        }

        public class CourseMaterialModel
        {
            public int MaterialId { get; set; }
            public string Title { get; set; }
            public string FilePath { get; set; }
            public string MaterialType { get; set; }
        }


        public class CourseCategoryModel
        {
            public int CategoryID { get; set; }

            public string CategoryName { get; set; }

            public string Thumbnail { get; set; }
            public int CourseCount { get; set; }
        }
        
    }
}