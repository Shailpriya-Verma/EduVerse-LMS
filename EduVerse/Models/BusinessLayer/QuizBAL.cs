using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EduVerse.Models.BusinessLayer
{
    public class QuizBAL
    {
        DBAccess db = new DBAccess();
        public DataTable cls_GetCoursesByInstructor(int InstructorId)
        {
            SqlParameter[] parameter = new SqlParameter[]
            {
                new SqlParameter("@InstructorId",InstructorId)
            };
            DataTable table = db.SelectData("proc_get_courses_by_instructor", parameter);
            return table;
        }

        public int cls_InsertQuizMaterial(int CourseId, string relativePath)
        {
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@CourseId", CourseId),
                new SqlParameter("@FilePath", relativePath)
            };

            int result = db.InsertUpdateDelete("proc_insert_quiz_material", param);
            return result;
        }


    }
}