using System;
using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Threading.Tasks;

using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using VTS.Core.Business.ViewModel;
using VTS.Core.Data.Models;
using VTS.Droid.Adapters;
using Android.Content;
using Android.Views;
using VTS.Droid.Helpers;

namespace VTS.Droid.Screens
{
    [Activity(Label = "VTS.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", ClearTaskOnLaunch = true,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainScreenActivity : ActionBarActivity
    {
        private VacationsViewModel _vacationsViewModel;
        private List<VTSModel> _listVTSModel;
        private ListView _listView;
        private VTSListAdapter _listAdapter;
        private SupportToolbar _toolbar;
        private MainDrawerToggle _drawerToggle;
        private DrawerLayout _drawerLayout;
        private ListView _leftDrawer;
        private ListView _rightDrawer;
        private ArrayAdapter _leftAdapter;
        private ArrayAdapter _rightAdapter;
        private List<string> _leftDataSet;
        private List<string> _rightDataSet;

        private const int EDIT_VACATION = 1;
        private const int CREATE_VACATION = 2;

        private int[] leftmenu = new int[] { CreateScreenActivity.VACATION_REQUEST, CreateScreenActivity.SICK_REQUEST };
        private int[] rightmenu = new int[] { CreateScreenActivity.OVERTIME_REQUEST, CreateScreenActivity.LIVEWOP_REQUEST, CreateScreenActivity.EXCEPTIONAL_REQUEST};



        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);            
            SetContentView(Resource.Layout.MainScreen);
            _vacationsViewModel = FactorySingleton.Factory.Get<VacationsViewModel>();
            _listView = FindViewById<ListView>(Resource.Id.VTSListView);
            _listView.ItemClick += OnListViewItemClick;
            _listView.ItemLongClick += OnListViewLongClick;
            LoadListView();
            InitDrawer(bundle);            
        }

        private void InitDrawer(Bundle bundle)
        {
            _toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            _leftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
            _rightDrawer = FindViewById<ListView>(Resource.Id.right_drawer);
            _leftDrawer.Tag = 0;
            _rightDrawer.Tag = 1;
            _leftDrawer.ItemClick += OnLeftDrawerItemClick;
            _rightDrawer.ItemClick += OnRightDrawerItemClick;

             SetSupportActionBar(_toolbar);

            _leftDataSet = new List<string>();
            _leftDataSet.Add(_vacationsViewModel.Localaizer.Localize("addVacation"));
            _leftDataSet.Add(_vacationsViewModel.Localaizer.Localize("SickToday"));            
            _leftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, _leftDataSet);
            _leftDrawer.Adapter = _leftAdapter;

            _rightDataSet = new List<string>();
            _rightDataSet.Add(_vacationsViewModel.Localaizer.Localize("Overtime"));
            _rightDataSet.Add(_vacationsViewModel.Localaizer.Localize("LeaveWOPay"));
            _rightDataSet.Add(_vacationsViewModel.Localaizer.Localize("ExceptionalLeave"));
            _rightAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, _rightDataSet);
            _rightDrawer.Adapter = _rightAdapter;
            if (_vacationsViewModel.IsOnlineMode)
            {
                _drawerToggle = new MainDrawerToggle(
               this,
               _drawerLayout,
               Resource.String.openDrawer,
               Resource.String.closeDrawer
                                                   );
            }
            else
            {
                _drawerToggle = new MainDrawerToggle(
               this,
               _drawerLayout,
               Resource.String.openDrawer,
               Resource.String.closeDrawerOffline
                 );
            }           
            _drawerLayout.SetDrawerListener(_drawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            _drawerToggle.SyncState();

            if (bundle != null)
            {
                if (bundle.GetString("DrawerState") == "Opened")
                {
                    SupportActionBar.SetTitle(Resource.String.openDrawer);
                }
                else {
                    if (_vacationsViewModel.IsOnlineMode)
                    {
                        SupportActionBar.SetTitle(Resource.String.closeDrawer);
                    }
                    else
                    {
                        SupportActionBar.SetTitle(Resource.String.closeDrawerOffline);
                    }
                }
            }
            else {
                    if (_vacationsViewModel.IsOnlineMode)
                    {
                        SupportActionBar.SetTitle(Resource.String.closeDrawer);
                    }
                    else
                    {
                        SupportActionBar.SetTitle(Resource.String.closeDrawerOffline);
                    }           
                }
        }      

        private void LoadListView()
        {
            try
            {
                Task<List<VTSModel>> task = Task.Run(async () => await _vacationsViewModel.GetVTSList());
                task.Wait();
                _listVTSModel = task.Result;
                _listAdapter = new VTSListAdapter(this, _listVTSModel);
                _listView.Adapter = _listAdapter;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short);               
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {           
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                LoadListView();                                    
            }
            if (resultCode == Result.FirstUser)
            {
                AlertDialog(_vacationsViewModel.Localaizer.Localize("NotSavedDataOffline"), null);
            }
        }

        #region Drawer Menu
        private void OnLeftDrawerItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _drawerLayout.CloseDrawers();
            Intent intent = new Intent(this, typeof(CreateScreenActivity));
            intent.PutExtra("typevacation", leftmenu[e.Id]);
            StartActivityForResult(intent, CREATE_VACATION);
        }

        private void OnRightDrawerItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _drawerLayout.CloseDrawers();
            Intent intent = new Intent(this, typeof(CreateScreenActivity));
            intent.PutExtra("typevacation", rightmenu[e.Id]);
            StartActivityForResult(intent, CREATE_VACATION);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    _drawerLayout.CloseDrawer(_rightDrawer);
                    _drawerToggle.OnOptionsItemSelected(item);
                    return true;

                case Resource.Id.action_add:
                    if (_drawerLayout.IsDrawerOpen(_rightDrawer))
                    {
                        _drawerLayout.CloseDrawer(_rightDrawer);
                    }
                    else {
                        _drawerLayout.OpenDrawer(_rightDrawer);
                        _drawerLayout.CloseDrawer(_leftDrawer);
                    }
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.action_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (_drawerLayout.IsDrawerOpen((int)GravityFlags.Left))
            {
                outState.PutString("DrawerState", "Opened");
            }
            else
            {
                outState.PutString("DrawerState", "Closed");
            }

            base.OnSaveInstanceState(outState);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            _drawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            _drawerToggle.OnConfigurationChanged(newConfig);
        }

        #endregion Drawer Menu
        
        #region List Events
        private void OnListViewItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(this, typeof(EditScreenActivity));
            intent.PutExtra("ID", _listVTSModel[e.Position].Id);            
            StartActivityForResult(intent, EDIT_VACATION);
        }

        private void OnListViewLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            if (_listVTSModel != null)
            {
                ShowConfirmation(_listVTSModel[e.Position]);            
            }            
        }

        private void ShowConfirmation(VTSModel vts)
        {
            var alert = new AlertDialog.Builder(this).Create();

            var alertView = LayoutInflater.Inflate(Resource.Layout.confirmationPopup, null);
            alertView.FindViewById<TextView>(Resource.Id.textView2).Text = _vacationsViewModel.Localaizer.Localize("confirmDelete");
            alertView.FindViewById<Button>(Resource.Id.confirmation_popup_cancel_button).Text = _vacationsViewModel.Localaizer.Localize("cancel_button");
            alertView.FindViewById<Button>(Resource.Id.confirmation_popup_ok_button).Text = _vacationsViewModel.Localaizer.Localize("delButton");
            alertView.FindViewById<TextView>(Resource.Id.textView2).Typeface = FontLoader.GetFontBold(this);
            alertView.FindViewById<Button>(Resource.Id.confirmation_popup_cancel_button).Typeface = FontLoader.GetFontNormal(this);
            alertView.FindViewById<Button>(Resource.Id.confirmation_popup_ok_button).Typeface = FontLoader.GetFontNormal(this);
            alertView.FindViewById<Button>(Resource.Id.confirmation_popup_cancel_button).Click += (s, e) =>
            {
                alert.Hide();
                alert.Cancel();               
            };
            alertView.FindViewById<Button>(Resource.Id.confirmation_popup_ok_button).Click += async (s, e) =>
            {
                alert.Hide();
                alert.Cancel();
                await Delete(vts);
            };

            alert.SetView(alertView);
            alert.Show();            
        }

        private async Task Delete(VTSModel vts)
        {   
            var success = await _vacationsViewModel.DeleteVacationInfo(vts);
            if (success)
            {
                LoadListView();
            }
            else
            {
                AlertDialog(_vacationsViewModel.Localaizer.Localize("error") + " " + _vacationsViewModel.ErrorMessage, null);
            }            
        }
        #endregion List Events

        protected void AlertDialog(String message, EventHandler<DialogClickEventArgs> onPositiveAction)
        {
            AlertDialog.Builder builder;
            builder = new AlertDialog.Builder(this);
            builder.SetMessage(message);
            builder.SetCancelable(false);
            builder.SetPositiveButton(Resources.GetString(Resource.String.ok_button), onPositiveAction);
            builder.Show();
        }
    }
}

