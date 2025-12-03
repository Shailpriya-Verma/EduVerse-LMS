using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EduVerse.Models.Allclasses
{
    public class QuizModel
    {
        public class QuizMaterialModel
        {
            public int CourseId { get; set; }
            public HttpPostedFileBase UploadedFile { get; set; }
            public string FilePath { get; set; }
            public string FileType { get; set; }
        }

    }
}