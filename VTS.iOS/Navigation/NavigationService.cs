using UIKit;
using VTS.iOS.Navigation.SplitMenu;
using VTS.iOS.Ninject;
using VTS.iOS.View_Controllers;

namespace VTS.iOS.Navigation
{
    public class NavigationService : INavigationService
    {
        public UIWindow Window { private set; get; }
        private UINavigationController nav;
        private SplitViewContoller splitView;

        public NavigationService()
        {
            if (Window == null)
            {
                Window = FactorySingleton.CurrentWindow;               
            }
        }

        public void Navigate(ViewControllerBase viewcontroller)
        {           
            if (Window != null)
            {
                nav = new UINavigationController(viewcontroller);
                Window.RootViewController = nav;
            }                 
        }

        public void NavigateWithSlide(ViewControllerBase viewcontroller)
        {
            if (Window != null)
            {
                splitView = new SplitViewContoller(viewcontroller);
                Window.RootViewController = splitView;
                Window.MakeKeyAndVisible();
            }
        }
    }
}
