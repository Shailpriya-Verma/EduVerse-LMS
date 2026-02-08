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

        public int cls_InsertQuiz(int CourseId, string Title, string Description, int CreatedBy, int DurationInMinutes)
        {
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@CourseId", CourseId),
                new SqlParameter("@Title", Title),
                new SqlParameter("@Description", Description),
                new SqlParameter("@CreatedBy", CreatedBy),
                new SqlParameter("@DurationInMinutes", DurationInMinutes),
            };
            int QuizId=db.InsertUpdateDeleteWithOutput("proc_insert_quiz", param);
            return QuizId;
        }

        public int cls_InsertQuizQuestion(int QuizId, string QuestionText, string A, string B, string C, string D, string Correct)
        {
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@QuizId", QuizId),
                new SqlParameter("@QuestionText", QuestionText),
                new SqlParameter("@OptionA", A),
                new SqlParameter("@OptionB", B),
                new SqlParameter("@OptionC", C),
                new SqlParameter("@OptionD", D),
                new SqlParameter("@CorrectOption", Correct)
            };

            return db.InsertUpdateDelete("proc_insert_quiz_question", param);
        }
    }
}