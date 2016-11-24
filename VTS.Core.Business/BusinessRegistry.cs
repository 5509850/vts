using Ninject;
using VTS.Core.Business.Services;
using VTS.Core.Business.ViewModel;
using VTS.Core.CrossCutting;

namespace VTS.Core.Business
{
    public class BusinessRegistry : IUIRegistry
    {
        public void Register(IKernel kernel)
        {
            kernel.Bind<ILoginManager>().To<LoginManager>().InSingletonScope();
            kernel.Bind<IVacationManager>().To<VacationManager>().InSingletonScope();
            kernel.Bind<INetworkReachability>().To<NetworkReachability>().InSingletonScope();
            kernel.Bind<LoginViewModel>().ToSelf();
            kernel.Bind<VacationsViewModel>().ToSelf();
        }
    }
}