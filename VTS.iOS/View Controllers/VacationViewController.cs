using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;
using VTS.Core.Business;
using VTS.Core.Business.ViewModel;
using VTS.Core.Data.Models;
using VTS.iOS.Helpers;
using VTS.iOS.Navigation;
using VTS.iOS.Ninject;
using VTS.iOS.View_Controllers.Table;

namespace VTS.iOS.View_Controllers
{
    public class VacationViewController : ViewControllerBase
    {        
        private UITableView table;
        private UIBarButtonItem _butnAdd, _butnMenu;
        private List<VTSModel> _listVTSModel;
        public VacationsViewModel _vacationsViewModel;
        private TableSource tableSource;
        private UINavigationController navController;
        private object localizer;

        protected override void CreateLayout()
        {
            table = new UITableView(View.Bounds, UITableViewStyle.Plain);                      
            table.SeparatorStyle = UITableViewCellSeparatorStyle.DoubleLineEtched;
            table.AutoresizingMask = UIViewAutoresizing.All;          
            Add(table);
            View.BackgroundColor = UIColor.FromRGB(230, 229, 228);
        }
        
        protected override void FetchDataToControl()
        {            
            try
            {
                _vacationsViewModel = FactorySingleton.Factory.Get<VacationsViewModel>();
                Task<List<VTSModel>> task = Task.Run(async () => await _vacationsViewModel.GetVTSList());
                task.Wait();
                _listVTSModel = task.Result;
                if (_listVTSModel == null && _vacationsViewModel.State == UserState.Unauthorized)
                {
                    GoToLoginScreen();
                    return;
                }
                tableSource = new TableSource(_listVTSModel, this);
                table.Source = tableSource;
                _butnAdd = new UIBarButtonItem(UIBarButtonSystemItem.Add, ButnAddClicked);
                NavigationItem.RightBarButtonItem = _butnAdd;
                _butnMenu = new  UIBarButtonItem();
                _butnMenu.Title = Localize("Menu");                
                NavigationItem.LeftBarButtonItem = _butnMenu;
                NavigationItem.Title = Localize("Requests");                
            }
            catch (Exception)
            {               
            }
        }

        void ButnAddClicked(object sender, EventArgs e)
        {
            var navigate = FactorySingleton.Factory.Get<NavigationService>();
            var createController = new CreateVacationViewController();
            createController.TypeVacation = Utils.VACATION_REQUEST;
            createController.ID = 0;
            navigate.Navigate(createController);
        }

        void ButnMenuClicked(object sender, EventArgs e)
        {           
            FactorySingleton.Factory.Get<NavigationService>()
              .NavigateWithSlide(new VacationViewController());
        }

        protected void GoToLoginScreen()
        {
            FactorySingleton.Factory.Get<NavigationService>()
                .Navigate(new LoginViewController());
        }
        
        protected override void ApplyEvents()
        {
            if (_butnMenu != null)
            {
                _butnMenu.Clicked += ButnMenuClicked;
            }
        }

        protected override void DisposeResource()
        {
            if (_butnMenu != null)
            {
                _butnMenu.Clicked -= ButnMenuClicked;
            }
            if (_butnAdd != null)
            {
                _butnAdd.Clicked -= ButnAddClicked;
            }
        }
        
        public async override void DeleteItem(object item)
        {
            if (item != null)
            {
                VTSModel vts = item as VTSModel;
                if (vts != null)
                {
                    await FactorySingleton.Factory.Get<VacationsViewModel>().DeleteVacationInfo(vts);
                }
            }            
        }
    }
}
