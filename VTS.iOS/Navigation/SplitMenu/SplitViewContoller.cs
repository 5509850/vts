using UIKit;
using VTS.iOS.Helpers;
using VTS.iOS.Ninject;
using VTS.iOS.View_Controllers;

namespace VTS.iOS.Navigation.SplitMenu
{
    public class SplitViewContoller : UISplitViewController
    {
        MenuController menuView;

        public SplitViewContoller(ViewControllerBase control) : base()
        {
            menuView = new MenuController();
            ViewControllers = new UIViewController[] { menuView, control };
            menuView.RowClicked += (object sender, MenuController.RowClickedEventArgs e) =>
            {
                var navigate = FactorySingleton.Factory.Get<NavigationService>();
                switch (e.Item)
                {
                    case Utils.EXIT_REQUEST:
                        {
                            navigate.Navigate(new LoginViewController());
                            break;
                        }
                    case Utils.LIST_VACATION:
                        {
                            navigate.Navigate(new VacationViewController());                            
                            break;
                        }
                    default:
                        {
                            var createVacationController = new CreateVacationViewController();
                            createVacationController.ID = 0;
                            createVacationController.TypeVacation = e.Item;
                            navigate.Navigate(createVacationController);
                            break;
                        }
                }
            };
            
            WillHideViewController += (object sender, UISplitViewHideEventArgs e) => 
            {   
            };

            WillShowViewController += (object sender, UISplitViewShowEventArgs e) => 
            {
            };
            
            ShouldHideViewController = (svc, viewController, inOrientation) => 
            {
                return inOrientation == UIInterfaceOrientation.Portrait ||
                    inOrientation == UIInterfaceOrientation.PortraitUpsideDown;
            };
        }

        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.All;
        }
    }
}
