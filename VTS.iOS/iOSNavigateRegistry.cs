using Ninject;
using VTS.Core.CrossCutting;
using VTS.iOS.Navigation;


namespace VTS.iOS
{
    public class iOSNavigateRegistry : IUIRegistry
    {
        public void Register(IKernel kernel)
        {           
            kernel.Bind<INavigationService>().To<NavigationService>().InSingletonScope();            
        }
    }
}
