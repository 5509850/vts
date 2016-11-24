using UIKit;
using VTS.iOS.View_Controllers;

namespace VTS.iOS.Navigation
{
    public interface INavigationService
    {
        void Navigate(ViewControllerBase viewcontroller);
        void NavigateWithSlide(ViewControllerBase viewcontroller);
        UIWindow Window { get; }
    }
}
