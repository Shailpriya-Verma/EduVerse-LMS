using EduVerse.Models.Allclasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using static EduVerse.Models.Allclasses.StudentModel;

namespace EduVerse.Models.BusinessLayer
{
    public class StudentBAL
    {
        DBAccess db = new DBAccess();
        public DataTable cls_GetAllCourses()
        {
            SqlParameter[] parameter = new SqlParameter[] {
               
            };
            DataTable dt = db.SelectData("proc_GetAllCourses", parameter);
            return dt;
        }

        public int cls_EnrollCourse(EnrollCourseModel obj)
        {
            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter("@CourseId", obj.courseId),
                new SqlParameter("@StudentId", obj.studentId),
                new SqlParameter("@PaymentStatus", obj.paymentStatus)
            };
            int res = db.InsertUpdateDeleteWithReturnValue("proc_EnrollCourse", parameter);
            return res;
        }


        public DataTable cls_GetCoursesByStudentId(int studentId)
        {
            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter("@StudentId",studentId)
            };
            DataTable dt = db.SelectData("proc_GetCoursesByStudentId", parameter);
            return dt;
        }

        public DataTable cls_GetCourseDetailByCourseId(int courseId)
        {
            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter("@CourseId",courseId)
            };
            DataTable dt = db.SelectData("proc_GetCourseDetailByCourseId", parameter);
            return dt;
        }

        public DataTable cls_GetAssignmentByStudentId(int st_id)
        {
            SqlParameter[] parameter = new SqlParameter[]
            {
                new SqlParameter("@st_id",st_id)
            };
            DataTable table = db.SelectData("proc_GetAssignmentByStudentId", parameter);
            return table;
        }

        public int GetSubmissionsCountById(int StudentId, int AssesmentId)
        {
            SqlParameter[] parameter = new SqlParameter[]
            {
                new SqlParameter("@StudentId",StudentId),
                new SqlParameter("@AssessmentId ",AssesmentId)
            };
            int res = db.InsertUpdateDelete("proc_GetSubmissionsCountById", parameter);
            return res;
        }
        public int cls_SubmitAssignment(int StudentId,int AssesmentId, string SubmissionPath)
        {
            SqlParameter[] parameter = new SqlParameter[]
            {
                new SqlParameter("@StudentId",StudentId),
                new SqlParameter("@AssessmentId ",AssesmentId),
                new SqlParameter("@SubmissionPath",SubmissionPath)
            };
            int res = db.InsertUpdateDelete("proc_SubmitAssignment", parameter);
            return res;
        }


        public DataTable cls_GetCount()
        {
            SqlParameter[] parameter = new SqlParameter[]
            {
            };
            DataTable table = db.SelectData("proc_GetCount", parameter);
            return table;
        }
        public DataTable cls_GetCategories()
        {
            SqlParameter[] parameter = new SqlParameter[]
            {
            };
            DataTable table = db.SelectData("proc_GetTopCategories", parameter);
            return table;
        }
    }
}