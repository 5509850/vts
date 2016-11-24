using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VTS.Core.Business.ViewModel;
using VTS.Core.CrossCutting.Helpers;
using VTS.Core.Data.Domain;
using VTS.Core.Data.Models;
using VTS.Core.Data.WebServices.Abstract;

namespace VTS.Core.Business
{
    public class VacationManager : IVacationManager
    {

        private readonly IVacationsDataService _vacationsDataService;

        public VacationManager(IVacationsDataService vacationsWebService)
        {
            _vacationsDataService = vacationsWebService;
        }

        public async Task<List<VTSModel>> GetVacationListFromRest(VacationsViewModel viewModel)
        {
            List<VTSModel> listVacations = new List<VTSModel>();                      

            try { 
            var responce = _vacationsDataService.GetResponceModel();
            responce = await _vacationsDataService.GetVacationsListFromRest();
            if (responce.LoginSuccess)
            {
                viewModel.State = UserState.Authorized;               
            }
                else {
                    viewModel.State = UserState.Unauthorized;
                    viewModel.ErrorMessage = responce.ErrorMessage;
                    return null;
                }

             listVacations = responce.VacationList;
           
            }
            catch (AggregateException e)
            {
                viewModel.ErrorMessage = e.InnerExceptions[0].Data["message"].ToString();
                listVacations = null;
            }
            return await Helper.Complete(listVacations);
        }

        public async Task<VacationInfoModel> GetVacationFromRestByID(VacationsViewModel viewModel, int ID)
        {
            VacationInfoModel vacation;
            try
            {              
                vacation = await _vacationsDataService.GetVacationByIdFromRest(ID);                                                        
            }
            catch (AggregateException e)
            {
                viewModel.ErrorMessage = e.InnerExceptions[0].Data["message"].ToString();
                vacation = null;
            }
            return await Helper.Complete(vacation);
        }

        public async Task<VacationInfoModel> GetVacationFromSqlByID(VacationsViewModel viewModel, int ID)
        {
            VacationInfoModel vacation;
            try
            {
                vacation = await _vacationsDataService.GetVacationByIdFromSql(ID);
            }
            catch (AggregateException e)
            {
                viewModel.ErrorMessage = e.InnerExceptions[0].Data["message"].ToString();
                vacation = null;
            }
            return await Helper.Complete(vacation);
        }
        public async Task<List<VTSModel>> GetVacationListFromSQL()
        {
            return await _vacationsDataService.GetVacationListFromSQL();
        }
        
        public async Task SaveVacationListToSql(List<VTSModel> listVTSModel)
        {
            await _vacationsDataService.SaveVacationsToSql(listVTSModel);            
        }

        public async Task<bool> UpdateOrCreateVacationInRest(VacationsViewModel viewModel, VacationInfoModel vacation)
        {
            VacationUpdateResponce responce = null;
            try
            {
                responce = await _vacationsDataService.UpdateOrCreateVacationsRest(vacation);               
            }
            catch (AggregateException e)
            {
                viewModel.ErrorMessage = e.InnerExceptions[0].Data["message"].ToString();              
            }
            catch(Exception ex)
            {
                viewModel.ErrorMessage = ex.Message;
            }

            if (responce.LoginSuccess)
            {
                viewModel.State = UserState.Authorized;
                viewModel.ErrorMessage = String.Empty;
            }
            else
            {
                viewModel.State = UserState.Unauthorized;
                viewModel.ErrorMessage = responce.ErrorMessage;
            }

            return await Helper.Complete(responce.LoginSuccess);
        }

        public async Task UpdateOrCreateVacationInSql(VacationInfoModel vacation)
        {
            await _vacationsDataService.UpdateOrCreateVacationsSql(vacation);
        }

        public async Task<bool> DeleteVacation(VacationsViewModel viewModel, VTSModel vacation)
        {
            VacationUpdateResponce responce = new VacationUpdateResponce(0, true, string.Empty);
            if (viewModel.IsOnlineMode)
            {
                try
                {
                    responce = await _vacationsDataService.DeleteVacationInRest(vacation);                    
                }
                catch (AggregateException e)
                {
                    viewModel.ErrorMessage = e.InnerExceptions[0].Data["message"].ToString();
                    return false;
                }
                catch (Exception ex)
                {
                    viewModel.ErrorMessage = ex.Message;
                    return false;
                }
            }
            if (responce.LoginSuccess)
            {
                viewModel.State = UserState.Authorized;
                viewModel.ErrorMessage = String.Empty;
            }
            else
            {
                viewModel.State = UserState.Unauthorized;
                viewModel.ErrorMessage = responce.ErrorMessage;
            }
            await _vacationsDataService.DeleteVacationsInSql(vacation);  
            return await Helper.Complete(responce.LoginSuccess);
        }

        public async Task<bool> DeleteVacationsInfoInSqlById(int id)
        {
            await _vacationsDataService.DeleteVacationsInfoInSqlById(id);
            return true;
        }
    }
}
