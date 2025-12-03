using EduVerse.Models.Allclasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static EduVerse.Models.Allclasses.StudentModel;
using static EduVerse.Models.AllClasses.AccountModel;

namespace EduVerse.Models.BusinessLayer
{
    public class PaymentBAL
    {
        DBAccess db = new DBAccess();
        public int InsertPayment(PaymentModel payment)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", payment.UserId),
                new SqlParameter("@CourseId", payment.CourseId),
                new SqlParameter("@Amount", payment.Amount),
                new SqlParameter("@PaymentStatus", payment.PaymentStatus),
                new SqlParameter("@TransactionId", payment.TransactionId)
            };
            return db.InsertUpdateDelete("proc_InsertPayment", parameters);
        }


        public int UpdatePaymentStatus(PaymentModel obj)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TransactionId", obj.TransactionId),
                new SqlParameter("@PaymentStatus", obj.PaymentStatus)
            };
            return db.InsertUpdateDelete("proc_UpdatePaymentStatus", parameters);
        }

        public int UpdateEnrollmentStatus(EnrollCourseModel obj)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CourseId", obj.courseId),
                new SqlParameter("@StudentId", obj.studentId),
                new SqlParameter("@PaymentStatus", obj.paymentStatus)
            };
            return db.InsertUpdateDelete("proc_UpdateEnrollmentStatus", parameters);
        }
    }
}