using System.Collections.Generic;
using VTS.Core.Data.Models;

namespace VTS.Core.Data.Domain
{
    public class VacationResponce
    {
        public VacationResponce()
        { }
        public VacationResponce(List<VTSModel> vacationList, bool loginSuccess, string errorMessage)
       {
           if (vacationList != null)
           {
               VacationList = vacationList;
           }
           else
           {
               VacationList = new List<VTSModel>();
           }
           
           LoginSuccess = loginSuccess;
           ErrorMessage = errorMessage; 
       }
       
        public List<VTSModel> VacationList { get; set; }
        public string ErrorMessage { get; set; }      
        public bool LoginSuccess { get; set; }
    }
}
