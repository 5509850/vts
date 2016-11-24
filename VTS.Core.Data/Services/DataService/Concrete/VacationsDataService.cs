using SQLite.Net.Interop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTS.Core.CrossCutting;
using VTS.Core.CrossCutting.Helpers;
using VTS.Core.Data.Domain;
using VTS.Core.Data.Models;
using VTS.Core.Data.SqlService;
using VTS.Core.Data.WebServices.Abstract;

namespace VTS.Core.Data.WebServices.Concrete
{
    public class VacationsDataService :  IVacationsDataService
    {      
        private SQLiteService sqliteService = null;
        ModelConverter _converter;

        private IConfiguration _configuration;
        private IFileSystemService _fileSystemService;
        private ISQLitePlatform _sqlitePlatform;
        private ILocalizeService _localizer;

        public VacationsDataService(ModelConverter converter, IConfiguration configuration, IFileSystemService fileSystemService, ISQLitePlatform sqlitePlatform, ILocalizeService localizer)
        {         
            _converter = converter;
            _configuration = configuration;
            _fileSystemService = fileSystemService;
            _sqlitePlatform = sqlitePlatform;
            _localizer = localizer;
        }
        public async Task<VacationResponce> GetVacationsListFromRest()
        {
            string result = String.Empty;
            bool loginSuccess = true;
            List<VacationInfoModel> listVacations = new List<VacationInfoModel>();
            List<VTSModel> VTSModelList = new List<VTSModel>();
            try
            {
                RestService restService = new RestService(_configuration.RestServerUrl, "Vacations");
                restService.Timeout = _configuration.ServerTimeOut;                                              
                listVacations = await restService.Get<VacationInfoModel>();
                if (listVacations != null)
                {
                    foreach (VacationInfoModel info in listVacations)
                    {
                        VTSModelList.Add(_converter.ConvertToVTSModel(info));
                    }
                }                                
            }
            catch (AggregateException e)
            {
                loginSuccess = false;
                result = e.InnerExceptions[0].Data["message"].ToString();
                listVacations = null;
            }
            catch (Exception e)
            {
                loginSuccess = false;
                result = e.Message;
                listVacations = null;
            }
            return await Helper.Complete(new VacationResponce(VTSModelList, loginSuccess, result));           
        }

        public async Task<VacationInfoModel> GetVacationByIdFromRest(int id)
        {
            string result = String.Empty;
            bool loginSuccess = false;
            VacationInfoModel vacation = null;            
            try
            {                
                RestService restService = new RestService(_configuration.RestServerUrl, "Vacations");
                restService.Timeout = _configuration.ServerTimeOut;
                vacation = await restService.Get<VacationInfoModel>(id);                    
            }
            catch (AggregateException e)
            {
                result = e.InnerExceptions[0].Data["message"].ToString();
                vacation = null;
            }
            catch (Exception e)
            {
                result = e.Message;
                vacation = null;
            }           
            return await Helper.Complete(vacation);
        }

        public async Task<VacationInfoModel> GetVacationByIdFromSql(int id)
        {           
            string result = String.Empty;            
            VacationInfoModel vacation = null;
            try
            {
                if (sqliteService == null)
                {
                    sqliteService = new SQLiteService(_sqlitePlatform, await _fileSystemService.GetPath(_configuration.SqlDatabaseName));
                }
                if (sqliteService != null)
                {                    
                    var vacationInfoModelDTO = await sqliteService.Get<VacationInfoModelDTO>(id.ToString());
                    if (vacationInfoModelDTO != null)
                    {
                        vacation = _converter.ConvertToVacationInfoModel(vacationInfoModelDTO);
                    }
                }
            }
            catch (AggregateException e)
            {
                result = e.InnerExceptions[0].Data["message"].ToString();
                vacation = null;
            }
            catch (Exception e)
            {
                result = e.Message;
                vacation = null;
            }
            return await Helper.Complete(vacation);
        }

        public VacationResponce GetResponceModel()
        {
            return new VacationResponce(new List <VTSModel>(), false, string.Empty);
        }

        public async Task SaveVacationsToSql(List<VTSModel> listVTSModel)
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
                    var vacationInfoList = await sqliteService.Get<VacationInfoDTO>();
                    if (vacationInfoList != null)
                    {
                        foreach (VacationInfoDTO old in vacationInfoList)
                        {
                            await sqliteService.Delete<VacationInfoDTO>(old.Id.ToString());
                        }
                    }
                    foreach (VTSModel info in listVTSModel)
                    {
                        await sqliteService.Insert(_converter.ConvertToVacationInfoDTO((info)));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<List<VTSModel>> GetVacationListFromSQL()
        {
            var vacationsViewModelList = new List<VTSModel>();
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
            try
            {
                if (sqliteService != null)
                {                   
                    List<VacationInfoDTO> vacationInfoList = await sqliteService.Get<VacationInfoDTO>();
                    if (vacationInfoList != null)
                    {
                        foreach (VacationInfoDTO info in vacationInfoList)
                        {
                            vacationsViewModelList.Add(_converter.ConvertToVTSModel(info));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
            return await Helper.Complete(vacationsViewModelList);
        }

        public async Task<VacationUpdateResponce> UpdateOrCreateVacationsRest(VacationInfoModel vacation)
        {   
            string result = String.Empty;
            bool loginSuccess = false;
            try
            {
                RestService restService = new RestService(_configuration.RestServerUrl, "Vacations");
                restService.Timeout = _configuration.ServerTimeOut;
                var request = await restService.Post(vacation);
                loginSuccess = request.IsSuccessStatusCode;
                if (request.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    result = "notFound";
                }
                if (request.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    result = "IncorrectLoginOrPassword";
                }
                if (request.StatusCode == System.Net.HttpStatusCode.NoContent || request.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = String.Empty;
                }               
            }
            catch (AggregateException e)
            {
                result = e.InnerExceptions[0].Data["message"].ToString();
                loginSuccess = false;
            }
            catch (Exception e)
            {
                result = e.Message;
                loginSuccess = false;
            }

            return await Helper.Complete(new VacationUpdateResponce(vacation.Id, loginSuccess, result));
        }

        public async Task UpdateOrCreateVacationsSql(VacationInfoModel vacation)
        {
            if (vacation == null)
            {
                return;
            }
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
                    var oldvacation = await sqliteService.Get<VacationInfoModelDTO>(vacation.Id.ToString());
                    if (oldvacation != null)
                    {
                        await sqliteService.Update(_converter.ConvertToVacationInfoModelDTO(vacation));                       
                    }
                    else
                    { 
                        await sqliteService.Insert(_converter.ConvertToVacationInfoModelDTO(vacation));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<VacationUpdateResponce> DeleteVacationInRest(VTSModel vacation)
        {
            string result = String.Empty;
            bool loginSuccess = false;
            try
            {
                RestService restService = new RestService(_configuration.RestServerUrl, "Vacations");
                restService.Timeout = _configuration.ServerTimeOut;
                var request = await restService.Delete(vacation.Id);
                loginSuccess = request.IsSuccessStatusCode;
                if (request.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    result = "notFound";
                }
                if (request.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    result = "IncorrectLoginOrPassword";
                }
                if (request.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed)
                {
                    result = "MethodNotAllowed";
                }
                if (request.StatusCode == System.Net.HttpStatusCode.NoContent || request.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = String.Empty;
                }
            }
            catch (AggregateException e)
            {
                result = e.InnerExceptions[0].Data["message"].ToString();
                loginSuccess = false;
            }
            catch (Exception e)
            {
                result = e.Message;
                loginSuccess = false;
            }

            return await Helper.Complete(new VacationUpdateResponce(vacation.Id, loginSuccess, result));
        }

        public async Task DeleteVacationsInSql(VTSModel vtsmodel)
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
                    await sqliteService.Delete<VacationInfoDTO>(vtsmodel.Id.ToString());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task DeleteVacationsInfoInSqlById(int id)
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
                    await sqliteService.Delete<VacationInfoModelDTO>(id.ToString());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
