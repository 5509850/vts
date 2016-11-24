using System;

namespace VTS.Core.Data.Models
{
    public class LoginModel
    {
        public LoginModel(string name, string error, string username, string password, string login,  string confidential)
        {
            Name = name;
            Error = error;
            UserName = username;
            Password = password;
            Login = login;
            Confidential = confidential;
        }

        public String Name { get; set; }
        public String Error { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String Login { get; set; }

        public String Confidential { get; set; }

    }
}
