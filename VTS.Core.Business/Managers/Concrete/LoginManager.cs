using System;
using System.Threading.Tasks;
using VTS.Core.Business.ViewModel;
using VTS.Core.Data.WebServices.Abstract;

namespace VTS.Core.Business
{
    public class LoginManager : ILoginManager
    {

        private readonly ILoginDataService _loginDataService;

        public LoginManager(ILoginDataService loginDataService)
        {
            _loginDataService = loginDataService;
        }
        public async Task SignIn(LoginViewModel loginViewModel, string name, string password)
        {           
            var responce = _loginDataService.GetResponceModel();
            if (loginViewModel.IsDeviceOnline())
            {
                if (loginViewModel.OffLineMode)
                {
                    responce = await _loginDataService.LogInOffline(name, password);
                }
                else
                {
                    responce = await _loginDataService.LogInOnline(name, password);
                }
            }
            else
            {
                responce = await _loginDataService.LogInOffline(name, password);
            }
            
            if (responce.LoginSuccess)
            {
                loginViewModel.State = UserState.Authorized;                       
                loginViewModel.AuthorizationError = String.Empty;                
            }
            else
            {
                loginViewModel.State = UserState.Unauthorized;
                loginViewModel.AuthorizationError = loginViewModel.Localizer.Localize(responce.ErrorMessage);
                if (loginViewModel.AuthorizationError.Equals("N/A"))
                {
                    loginViewModel.AuthorizationError = responce.ErrorMessage;
                }                           
            }
            return;
        }
    }
}
