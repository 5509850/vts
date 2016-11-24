using SQLite.Net.Attributes;
using System;

namespace VTS.Core.Data.Models
{
    [Table("LoginInfo")]
    public class LoginInfoDTO
    {
        [PrimaryKey]
        public int Id { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
    }
}
