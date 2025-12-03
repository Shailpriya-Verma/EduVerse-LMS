using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EduVerse.Models.AllClasses
{
    public class AccountModel
    {
        public class Register
        {
            public int roleId { get; set; }
            public string email { get; set; }
            public string password { get; set; }
            public string googleId { get; set; }
            public string profilePic { get; set; }
            public string name { get; set; }
        }

        public class UserLogin
        {
            public int roleId { get; set; }
            public string email { get; set; }
            public string password { get; set; }
        }
    }
}