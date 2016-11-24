using Ninject;

namespace VTS.Core.CrossCutting
{
    public class CrossCuttingConcernsRegistry : IUIRegistry
    {
        public void Register(IKernel kernel)
        {
            kernel.Bind<ILocalizeService>().To<LocalizeService>().InSingletonScope();           
            kernel.Bind<IConfiguration>().To<Configuration>().InSingletonScope();
        }
    }
}
