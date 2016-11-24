using Ninject;
using VTS.Core.CrossCutting;
using VTS.Core.Data.WebServices.Abstract;
using VTS.Core.Data.WebServices.Concrete;

namespace VTS.Core.Data
{
    public class DataRegistry : IUIRegistry
    {
        public void Register(IKernel kernel)
        {
            kernel.Bind<ILoginDataService>().To<LoginDataService>().InSingletonScope();
            kernel.Bind<IVacationsDataService>().To<VacationsDataService>().InSingletonScope();
        }
    }
}
