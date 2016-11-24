using System.Threading.Tasks;
using VTS.Core.Business.ViewModel;

namespace VTS.Core.Business
{
    public interface ILoginManager
    {
        Task SignIn(LoginViewModel loginviewmodel, string name, string password);        
    }
}
