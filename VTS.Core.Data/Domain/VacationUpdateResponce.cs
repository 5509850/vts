using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTS.Core.Data.Domain
{   
    public class VacationUpdateResponce
    {
        public VacationUpdateResponce()
        { }
        public VacationUpdateResponce(int id, bool loginSuccess, string errorMessage)
        {
            ID = id;
            LoginSuccess = loginSuccess;
            ErrorMessage = errorMessage;
        }

        public int ID { get; set; }
        public string ErrorMessage { get; set; }
        public bool LoginSuccess { get; set; }
    }
}
