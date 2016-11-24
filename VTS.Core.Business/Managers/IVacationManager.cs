using System.Collections.Generic;
using System.Threading.Tasks;
using VTS.Core.Business.ViewModel;
using VTS.Core.Data.Models;

namespace VTS.Core.Business
{
    public interface IVacationManager
    {
       Task <List<VTSModel>> GetVacationListFromRest(VacationsViewModel viewModel);
       Task<VacationInfoModel> GetVacationFromRestByID(VacationsViewModel viewModel, int ID);
       Task<VacationInfoModel> GetVacationFromSqlByID(VacationsViewModel viewModel, int ID);
       Task <List<VTSModel>> GetVacationListFromSQL();
       Task SaveVacationListToSql(List<VTSModel> listVTSModel);
       Task<bool> UpdateOrCreateVacationInRest(VacationsViewModel viewModel, VacationInfoModel vacation);
       Task UpdateOrCreateVacationInSql(VacationInfoModel vacation);
       Task<bool> DeleteVacation(VacationsViewModel viewModel, VTSModel vacation);
       Task<bool> DeleteVacationsInfoInSqlById(int id);
    }
}
