using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using VTS.Core.Business.ViewModel;
using VTS.Droid.Controls.Fragments;
using VTS.Core.Data.Models;
using System.Threading.Tasks;

namespace VTS.Droid.Screens
{
    [Activity(Label = "CreateScreenActivity", ParentActivity = typeof(MainScreenActivity))]
    public class CreateScreenActivity : BaseScreenActivity
    {
        private VacationTabsFragment _fragment;
        private VacationsViewModel _vacationsViewModel;
        public const int VACATION_REQUEST = 0;
        public const int SICK_REQUEST = 1;
        public const int OVERTIME_REQUEST = 2;
        public const int LIVEWOP_REQUEST = 3;
        public const int EXCEPTIONAL_REQUEST = 4;
        private int _typevacation;
        private VacationInfoModel _vacationInfo;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.ActionBar);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);
            SetContentView(Resource.Layout.CreateScreen);
            _vacationsViewModel = FactorySingleton.Factory.Get<VacationsViewModel>();
            if (_vacationsViewModel.IsOnlineMode)
            {
                ActionBar.SetTitle(Resource.String.newRequest);
            }
            else
            {
                ActionBar.SetTitle(Resource.String.newRequestOffline);
            }            
            _vacationInfo = await _vacationsViewModel.CreateDraftVacationInfo();
            if (_vacationInfo == null)
            {
                Intent myIntent = new Intent(this, typeof(MainScreenActivity));
                SetResult(Result.Canceled, myIntent);
                Exit();
                return;
            }
            if (Intent.Extras != null)
            {
                _typevacation = Intent.Extras.GetInt("typevacation");
                switch (_typevacation)
                {
                    case VACATION_REQUEST:
                        {
                            _vacationInfo.Type.Key = "VAC";
                            _vacationInfo.Type.Value = "Regular (VAC)";
                            break;
                        }
                    case SICK_REQUEST:
                        {
                            _vacationInfo.Type.Key = "ILL";
                            _vacationInfo.Type.Value = "Illness (ILL)";
                            break;
                        }
                    case OVERTIME_REQUEST:
                        {
                            _vacationInfo.Type.Key = "OVT";
                            _vacationInfo.Type.Value = "Overtime (OVT)";
                            break;
                        }
                    case LIVEWOP_REQUEST:
                        {
                            _vacationInfo.Type.Key = "POV";
                            _vacationInfo.Type.Value = "Without pay (POV)";
                            break;
                        }
                    case EXCEPTIONAL_REQUEST:
                        {
                            _vacationInfo.Type.Key = "EXV";
                            _vacationInfo.Type.Value = "EXCEPTIONAL (EXV)";
                            break;
                        }
                }
                await _vacationsViewModel.UpdateDraftVacationInfo(_vacationInfo);
            }
            else
            {
                Finish();
            }
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            _fragment = new VacationTabsFragment();
            transaction.Replace(Resource.Id.sample_content_fragment, _fragment);
            transaction.Commit();
        }       

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                _fragment.Adapter.ActivityResult(requestCode, resultCode, data);
            }
        }

        public override void OnBackPressed()
        {
            _fragment.Adapter.ViewDispose();
            base.OnBackPressed();
        }
        
        #region menu top ActionBar
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.edit_screen_menu, menu);
            IMenuItem sendmenu = menu.FindItem(Resource.Id.menu_update);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            var result = false;
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    result = true;
                    if (ReturnToMainScreen())
                    {
                        Exit();
                    }
                    break;

                case Resource.Id.menu_update:
                    Save();
                    break;

                default:
                    result = OnOptionsItemSelected(item);
                    break;
            }
            return result;
        }

        private void Exit()
        {
            Finish();
        }

        private bool ReturnToMainScreen()
        {
            //TODO:
            return true;
        }

        private async Task<bool> Save()
        {
            var result = false;
            try
            {  
                ShowProgress(_vacationsViewModel.Localaizer.Localize("sending"));
                await Task.Delay(500);
                result = await _vacationsViewModel.SendDraftVacationInfo();
                HideProgress();
             
            }
            catch (Exception ex)
            {
                AlertDialog(ex.Message, null);                
                HideProgress();
                return false;
            }
            Intent myIntent = new Intent(this, typeof(MainScreenActivity));
            SetResult(Result.Ok, myIntent);
            Exit();
            return true;
        }
        #endregion menu top ActionBar
    }
}