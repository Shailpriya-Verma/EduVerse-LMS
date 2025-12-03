using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EduVerse.Models.Allclasses
{
    public class StudentModel
    {
        public class EnrollCourseModel
        {
            public int courseId { get; set; }
            public int studentId { get; set; }
            public string paymentStatus { get; set; }
        }
    }
}