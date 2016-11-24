using System.Threading.Tasks;
using VTS.Core.Data.Domain;

namespace VTS.Core.Data.WebServices.Abstract
{
    public interface ILoginDataService
    {
        Task<LoginResponce> LogInOnline(string name, string password);
        Task<LoginResponce> LogInOffline(string name, string password);
        LoginResponce GetResponceModel();
    }
}
