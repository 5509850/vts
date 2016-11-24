using System;
using System.Threading.Tasks;
using VTS.Core.CrossCutting;
using VTS.Core.Data.Models;

namespace VTS.Core.Business.ViewModel
{
    public  class LoginViewModel : BaseViewModel
    {
        private readonly ILoginManager _userManager;      
        private IPlatformException _platformException;
        private INetworkReachability _networkReachability;
        private IConfiguration _config;
        private bool _offlinemode;
        public LoginViewModel(ILoginManager userManager, ILocalizeService localizer, 
            INetworkReachability networkReachability, 
            IPlatformException platformException,
            IConfiguration config
            )
        {
            _userManager = userManager;
            this.Localizer = localizer;
            _networkReachability = networkReachability;          
            _platformException = platformException;
            _config = config;
            _offlinemode = !IsDeviceOnline();
            ChangeLanguage(_config.GetDefaultLanguage.ToString());            
        }     

        #region Property
        public ILocalizeService Localizer { get; internal set; }
        public UserState State { get; internal set; }
        public LoginModel LoginModelLocalize { get; internal set; }              
        public string AuthorizationError { get; internal set; }
        #endregion
        public bool IsDeviceOnline()
        {            
            return _networkReachability.IsOnlineMode; ;
        }

        public bool OffLineMode { get { return _offlinemode; } }
        
        /// <summary>
        /// Only ping IP address!!! Without check Reachability webservice REST
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsReachableHost()
        {
            if (!IsDeviceOnline())
            {
                _offlinemode = true;
                return false;
            }
            var isReachableHost = await _networkReachability.IsReachableHost(_config.RestServerUrl.Replace(@"http://", string.Empty), _config.ServerTimeOut);
            _offlinemode = !isReachableHost;
            return isReachableHost; 
        }
        public async Task<bool> SignIn(string username, string password)
        {
            try {
                await _userManager.SignIn(this, username, password);
                if (State == UserState.Authorized)
                {
                    return true;
                }
            }
            catch (AggregateException e)
            {
                if (e.InnerExceptions[0].GetType() == _platformException.URISyntaxException())
                {
                    AuthorizationError = Localizer.Localize("uncorrectURI");
                }
                else {
                    if (e.InnerExceptions[0].Data.Count > 0)
                    {
                        AuthorizationError = Localizer.Localize(e.InnerExceptions[0].Data["message"].ToString());
                    }
                    else {
                        AuthorizationError = Localizer.Localize("undefinedException");
                    }
                }
                return false;
            }
            return false;
        }
        public void ChangeLanguage(string local)
        {
            Localizer.LoadLocalization(local);
            LoginModelLocalize = new LoginModel
                (
                Localizer.Localize("VTS"),
                Localizer.Localize("IncorrectLoginOrPassword"),
                Localizer.Localize("EmailPlaceHolder"),
                Localizer.Localize("PasswordPlaceHolder"),
                Localizer.Localize("Login"),
                Localizer.Localize("EpamConfidential")
                );
        }
    }
}

