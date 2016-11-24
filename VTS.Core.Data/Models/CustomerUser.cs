using SQLite.Net.Attributes;
using System;

namespace VTS.Core.Data.Models
{
    public class CustomerUser
    {
        public CustomerUser(string name, string password)
        {
            Name = name;
            Password = password;
        }
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public String Name { get; set; }
        public String Password { get; set; }
    }
}
