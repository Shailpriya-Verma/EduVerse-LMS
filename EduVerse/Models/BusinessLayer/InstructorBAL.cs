using EduVerse.Models.Allclasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using static EduVerse.Models.Allclasses.InstructorModel;

namespace EduVerse.Models.BusinessLayer
{
    public class InstructorBAL
    {
        DBAccess db = new DBAccess();
        //public DataTable cls_InsertCourse(InstructorModel.CourseModel obj)
        //{
        //    SqlParameter[] parameter = new SqlParameter[] {
        //       new SqlParameter("@Title", obj.Title), 
        //        new SqlParameter("@Description", obj.Description), 
        //        new SqlParameter("@TeacherId", obj.TeacherId), 
        //        new SqlParameter("@IsPaid", obj.IsPaid),
        //        new SqlParameter("@Price", obj.Price ?? (object)DBNull.Value),
        //        new SqlParameter("@MaterialTitle", obj.MaterialTitle),
        //        new SqlParameter("@MaterialPath", obj.MaterialPath),
        //        new SqlParameter("@MaterialType", obj.MaterialType),
        //        new SqlParameter("@Thumbnail", obj.ThumbnailPath),
        //    };
        //    DataTable dt = db.SelectData("proc_InsertCourse", parameter);
        //    return dt;
        //}



        public int SaveCourseBasic(CourseModel obj)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@Title", obj.Title),
                new SqlParameter("@Description", obj.Description),
                new SqlParameter("@TeacherId", obj.TeacherId),
                new SqlParameter("@IsPaid", obj.IsPaid),
                new SqlParameter("@Price", obj.Price ?? (object)DBNull.Value),
                new SqlParameter("@Thumbnail", obj.ThumbnailPath),
                new SqlParameter
                {
                    ParameterName = "@CourseId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                }
            };

            db.InsertUpdateDelete("proc_SaveCourseBasic", parameters);
            return Convert.ToInt32(parameters[6].Value);
        }

        public int SaveCourseMaterial(CourseMaterialModel material)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@CourseId", material.CourseId),
                new SqlParameter("@Title", material.Title),
                new SqlParameter("@FilePath", material.FilePath),
                new SqlParameter("@MaterialType", material.MaterialType)
            };
            return db.InsertUpdateDelete("proc_InsertCourseMaterial", parameters);
        }


        public DataTable getcoursedetailsByInstructorId(int id)
        {
            SqlParameter[] parameter = new SqlParameter[]
            {
                new SqlParameter("@id",id)
            };
            DataTable dt=db.SelectData("proc_getCourseDetailsByInstructorId", parameter);
            return dt;
        }


        public int SaveAssignment(AssignmentModel assign)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@CourseId", assign.CourseId),
                new SqlParameter("@Title", assign.Title),
                new SqlParameter("@FilePath", assign.AssignmentFilePath),
                new SqlParameter("@DueDate", assign.DueDate),
                new SqlParameter("@Description", assign.Description)
            };
            return db.InsertUpdateDelete("proc_insert_assignment", parameters);
        }

        public DataTable GetCoursesByInstructorId(int Ins_id)
        {
            SqlParameter[] parameter = new SqlParameter[]
            {
                new SqlParameter("@Ins_id",Ins_id)
            };
            DataTable table = db.SelectData("proc_GetCoursesByInstructorId", parameter);
            return table;
        }
        public DataTable cls_GetSubmissionsByInstructor(int InstructorId)
        {
            SqlParameter[] parameter = new SqlParameter[]
            {
                new SqlParameter("@InstructorId",InstructorId)
            };
            DataTable table = db.SelectData("proc_GetSubmissionsByInstructorId", parameter);
            return table;
        }

        public int cls_UpdateSubmissionGradeFeedback(int submissionId,int grade,string feedback)
        {
            SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@SubmissionId", submissionId),
                    new SqlParameter("@Grade", grade),
                    new SqlParameter("@Feedback", feedback)
                };

            int result = db.InsertUpdateDelete("proc_UpdateSubmissionGradeFeedback", parameters);
            return result;
        }

    }
}