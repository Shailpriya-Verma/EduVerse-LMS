using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EduVerse.Models.Allclasses
{
    public class InstructorModel
    {

        public class CourseModel
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public bool IsPaid { get; set; }
            public decimal? Price { get; set; }
            public int TeacherId { get; set; }

            public HttpPostedFileBase Thumbnail { get; set; }
            public string ThumbnailPath { get; set; }

            public List<HttpPostedFileBase> CourseMaterials { get; set; }
            public List<string> MaterialTitles { get; set; }
            public List<string> MaterialTypes { get; set; }
        }


        public class CourseMaterialModel
        {
            public int CourseId { get; set; }
            public int MaterialId { get; set; }
            public string Title { get; set; }
            public string FilePath { get; set; }
            public string MaterialType { get; set; } 
            public DateTime UploadedAt { get; set; }
        }


        public class AssignmentModel
        {
            public int CourseId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime DueDate { get; set; }
            public HttpPostedFileBase AssignmentFile { get; set; }
            public string AssignmentFilePath { get; set; }
        }

    }
}