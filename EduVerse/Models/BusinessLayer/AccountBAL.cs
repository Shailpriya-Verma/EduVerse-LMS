using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using static EduVerse.Models.AllClasses.AccountModel;

namespace EduVerse.Models.BusinessLayer
{
    public class AccountBAL
    {
        DBAccess db = new DBAccess();
        public int cls_Register(Register obj)
        {
            SqlParameter[] parameter = new SqlParameter[] {
              new SqlParameter("@action","Register"),
              new SqlParameter("@roleid",obj.roleId),
              new SqlParameter("@name",obj.name),
              new SqlParameter("@email",obj.email),
              new SqlParameter("@password",obj.password)
        };
            int res = db.InsertUpdateDeleteWithReturnValue("proc_Register", parameter);
            return res;
        }

        public DataTable cls_Login(UserLogin obj)
        {
            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter("@roleid",obj.roleId),
              new SqlParameter("@email",obj.email),
              new SqlParameter("@password",obj.password)
            };
            DataTable dt = db.SelectData("proc_Login", parameter);
            return dt;
        }

        public int cls_ResetPassword(string email, string newPassword,int RoleId)
        {
            SqlParameter[] parameter = new SqlParameter[] {
                new SqlParameter("@RoleId",RoleId),
              new SqlParameter("@Email",email),
              new SqlParameter("@NewPassword",newPassword)
            };
            int res = db.InsertUpdateDelete("proc_ResetUserPassword", parameter);
            return res;
        }
    }
}
