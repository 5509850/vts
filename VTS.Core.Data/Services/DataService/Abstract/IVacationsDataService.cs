using System.Collections.Generic;
using System.Threading.Tasks;
using VTS.Core.Data.Domain;
using VTS.Core.Data.Models;

namespace VTS.Core.Data.WebServices.Abstract
{
    public interface IVacationsDataService
    {
        Task<VacationResponce> GetVacationsListFromRest();
        Task<VacationInfoModel> GetVacationByIdFromRest(int id);
        Task<VacationInfoModel> GetVacationByIdFromSql(int id);
        VacationResponce GetResponceModel();    

        Task<List<VTSModel>> GetVacationListFromSQL();
        Task SaveVacationsToSql(List<VTSModel> listVTSModel);
        Task<VacationUpdateResponce> UpdateOrCreateVacationsRest(VacationInfoModel vacation);

        Task UpdateOrCreateVacationsSql(VacationInfoModel vacation);
        Task<VacationUpdateResponce> DeleteVacationInRest(VTSModel vacation);
        Task DeleteVacationsInSql(VTSModel vtsmodel);
        Task DeleteVacationsInfoInSqlById(int id);
    }
}
