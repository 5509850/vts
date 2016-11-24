using Specific.iOS;
using UIKit;
using VTS.Core.Business;
using VTS.Core.CrossCutting;
using VTS.Core.Data;

namespace VTS.iOS.Ninject
{
    public class FactorySingleton
    {
        private static Factory _factory;
        private static UIWindow _window;  

        private FactorySingleton() { }

        public static Factory Factory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = new Factory();
                    _factory.Init(new CrossCuttingConcernsRegistry());
                    _factory.Init(new DataRegistry());
                    _factory.Init(new BusinessRegistry());
                    _factory.Init(new iOSNavigateRegistry());
                    _factory.Init(new iOSRegistry());
                }
                return _factory;
            }
        }

        public static UIWindow CurrentWindow
        {
            get
            {
                if (_window == null)
                {                    
                    _window = new UIWindow(UIScreen.MainScreen.Bounds);                 
                }
                return _window;
            }
        }
    }
}

