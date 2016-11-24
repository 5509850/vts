using SQLite.Net.Interop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTS.Core.CrossCutting;
using VTS.Core.CrossCutting.Extensions;
using VTS.Core.CrossCutting.Helpers;
using VTS.Core.Data.Domain;
using VTS.Core.Data.Models;
using VTS.Core.Data.SqlService;
using VTS.Core.Data.WebServices.Abstract;

namespace VTS.Core.Data.WebServices.Concrete
{
    public class LoginDataService : ILoginDataService
    {
        private SQLiteService sqliteService = null;
        private IConfiguration _configuration;
        private IFileSystemService _fileSystemService;
        private ISQLitePlatform _sqlitePlatform;
        private ILocalizeService _localizservice;

        public LoginDataService(IConfiguration configuration, IFileSystemService fileSystemService, ISQLitePlatform sqlitePlatform, ILocalizeService localizservice)
        {
            _configuration = configuration;
            _fileSystemService = fileSystemService;
            _sqlitePlatform = sqlitePlatform;
            _localizservice = localizservice;
        }

        public async Task<LoginResponce> LogInOnline(string name, string password)
        {
            string result = String.Empty;
            bool success = false;           
            try
            {                    
                RestService restService = new RestService(_configuration.RestServerUrl, "login");
                restService.Timeout = _configuration.ServerTimeOut;
                var request = await restService.Post(name, password);
                success = request.IsSuccessStatusCode;

                if (request.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    result = "notFound";
                }
                if (request.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    result = "IncorrectLoginOrPassword";
                }
                if (request.StatusCode == System.Net.HttpStatusCode.NoContent) //OK!!!
                {
                    result = String.Empty;
                }
            }
            catch (AggregateException e)
            {                    
                if (e.InnerExceptions[0].Data.Count > 0)
                {
                    result = e.InnerExceptions[0].Data["message"].ToString();                       
                }
                else {
                    result = "undefinedException";  
                }
            }
            catch (Exception e)
            {
                result = String.Format("Error = " + e.Message);
                success = false;
            }

            if (success)
            {
                await SaveLoginModelToSQLite(name, password);
            }

            return await Helper.Complete(new LoginResponce(success, result));          
        }

        public async Task<LoginResponce> LogInOffline(string name, string password)
        {
            string result = String.Empty;
            bool success = false;
            try
            {
                if (sqliteService == null)
                {
                    sqliteService = new SQLiteService(_sqlitePlatform, await _fileSystemService.GetPath(_configuration.SqlDatabaseName));
                }

            }
            catch (Exception exp)
            {
                sqliteService = null;
            }
            try {
                    if (sqliteService != null)
                    {
                        LoginInfoDTO currentLoginInfo = new LoginInfoDTO();
                        currentLoginInfo.UserName = name;
                        currentLoginInfo.Password = SecurytyHash.CalculateSha1Hash(password);
                        result = _localizservice.Localize("NotSavedDataOffline");
                        List<LoginInfoDTO> listlogins = await sqliteService.Get<LoginInfoDTO>();
                        if (listlogins != null && listlogins.Count != 0)
                        {
                            foreach (LoginInfoDTO login in listlogins)
                            {
                                if (login.UserName == currentLoginInfo.UserName)
                                {
                                    result = string.Empty;
                                    if (login.Password == currentLoginInfo.Password)
                                    {
                                        success = true;
                                        result = string.Empty;
                                    }
                                    else
                                    {
                                        result = _localizservice.Localize("IncorrectLoginOrPassword");
                                    }
                                }
                            }
                        }
                    }
               }
            catch (Exception e)
            {
                result = String.Format("Error = " + e.Message);
                success = false;
            }
            return await Helper.Complete(new LoginResponce(success, result));
        }       
                
        public LoginResponce GetResponceModel()
        {
            return new LoginResponce();
        }

        public async Task SaveLoginModelToSQLite(string name, string password)
        {
            try
            {
                if (sqliteService == null)
                {
                    sqliteService = new SQLiteService(_sqlitePlatform, await _fileSystemService.GetPath(_configuration.SqlDatabaseName));
                }

            }
            catch (Exception exp)
            {
                sqliteService = null;
            }
            if (sqliteService != null)
            {
                try
                {
                    LoginInfoDTO currentLoginInfo = new LoginInfoDTO();
                    currentLoginInfo.UserName = name;
                    currentLoginInfo.Password = SecurytyHash.CalculateSha1Hash(password);

                    List<LoginInfoDTO> listlogins = await sqliteService.Get<LoginInfoDTO>();
                    if (listlogins != null && listlogins.Count != 0)
                    {
                        foreach (LoginInfoDTO login in listlogins)
                        {
                            if (login.UserName == currentLoginInfo.UserName)
                            {
                                await sqliteService.Delete<LoginInfoDTO>(login.Id.ToString());
                            }
                        }
                    }

                    await sqliteService.Insert<LoginInfoDTO>(currentLoginInfo);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
