namespace VTS.Core.Data.Domain
{
    public class LoginResponce
    {
        public LoginResponce()
        { }
       public LoginResponce(bool loginSuccess, string errorMessage)
       {
           LoginSuccess = loginSuccess;
           ErrorMessage = errorMessage; 
       }
       public bool LoginSuccess { get; set; }
       public string ErrorMessage { get; set; }      
    }
}
