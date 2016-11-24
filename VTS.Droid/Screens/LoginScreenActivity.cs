using System;
using Android.App;
using Android.OS;
using Android.Widget;
using VTS.Core.Business.ViewModel;
using Android.Content;
using VTS.Droid.Helpers;
using Android.Views.InputMethods;
using System.Threading.Tasks;
using VTS.Core.CrossCutting.Helpers;

namespace VTS.Droid.Screens
{
    [Activity(Label = "@string/vts",  MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.NoTitleBar")]
    public class LoginScreenActivity : BaseScreenActivity
    {
        private LoginViewModel _loginViewModel;
        private TextView _vtsTitle;
        private EditText _userName;
        private EditText _password;
        private Button _loginButton;
        private TextView _error;
        private TextView _epamConfidential;
        private ImageView _logo;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);            
            SetContentView(Resource.Layout.LoginScreen);
            _loginButton = FindViewById<Button>(Resource.Id.LoginBtn);
            _userName = FindViewById<EditText>(Resource.Id.LoginUsername);
            _password = FindViewById<EditText>(Resource.Id.LoginPassword);
            _error = FindViewById<TextView>(Resource.Id.LoginError);
            _vtsTitle = FindViewById<TextView>(Resource.Id.title_vts);
            _epamConfidential = FindViewById<TextView>(Resource.Id.EpamConfidential);
            _logo = FindViewById<ImageView>(Resource.Id.Logo); 
            _loginViewModel = FactorySingleton.Factory.Get<LoginViewModel>();
            _logo.Click += OnLogoClick;
            _loginButton.Click += LoginButtontnClick;
            _password.EditorAction += PasswordEditorAction;
            LocalizeControl();         
            LoadFonts();
            RetrieveUsername();
        }

        private void OnLogoClick(object sender, EventArgs e)
        {
            Utils.HideSoftKeyboard(this);
        }

        private async void CheckOnline()
        {
            if (!_loginViewModel.IsDeviceOnline())
            {
                DisplayErrorMessage(_loginViewModel.Localizer.Localize("OffLine"));
            }
            else
            {
                bool isReacheble = await _loginViewModel.IsReachableHost();
                if (!isReacheble)
                {
                    DisplayErrorMessage(_loginViewModel.Localizer.Localize("NotReachableHost"));
                }
                else
                {
                    DisplayErrorMessage(string.Empty);
                }                
            }
        }

        private void LocalizeControl()
        {
            _vtsTitle.Text = _loginViewModel.LoginModelLocalize.Name;            
            _userName.Hint = _loginViewModel.LoginModelLocalize.UserName;
            _password.Hint = _loginViewModel.LoginModelLocalize.Password;
            _loginButton.Text = _loginViewModel.LoginModelLocalize.Login;
            _epamConfidential.Text = _loginViewModel.LoginModelLocalize.Confidential;
        }

        private async void PasswordEditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            e.Handled = false;
            if (e.ActionId == ImeAction.Done || e.ActionId == ImeAction.Next)
            {
                await LoginDo();
                e.Handled = true;
            }
        }

        private void LoadFonts()
        {           
            _epamConfidential.Typeface = FontLoader.GetFontLight(this);
            _loginButton.Typeface = _vtsTitle.Typeface = FontLoader.GetFontBold(this);
            _error.Typeface =  _userName.Typeface = _password.Typeface = FontLoader.GetFontNormal(this);
        }

        private async void LoginButtontnClick(object sender, EventArgs e)
        {
            await LoginDo();
        }

        private async Task LoginDo()
        {
            Utils.HideSoftKeyboard(this);
            _error.Visibility = Android.Views.ViewStates.Invisible;
            try
            {
                if (ValidateInput())
                {
                    _loginButton.Enabled = false;
                    ShowProgress(_loginViewModel.Localizer.Localize("UiActivityAuthenticating"));
                    bool result = await AuteAuthenticateAsync(_userName.Text, _password.Text);                                       
                    //HideProgress();
                    if (result)
                    {
                        _error.Text = string.Empty;
                        GoToVacationScreen();
                    }
                    else
                    {
                        _loginButton.Enabled = true;
                        DisplayErrorMessage(_loginViewModel.AuthorizationError);
                        HideProgress();
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayErrorMessage("Error = " + ex.Message);
            }
        }

        private async Task<bool> AuteAuthenticateAsync(string name, string password)
        {
            await Task.Delay(100);
            return await _loginViewModel.SignIn(_userName.Text, _password.Text);
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(_userName.Text))
            {
                DisplayErrorMessage(_loginViewModel.Localizer.Localize("EmailEmpty"));
                return false;
            }

            if (string.IsNullOrEmpty(_password.Text))
            {
                DisplayErrorMessage(_loginViewModel.Localizer.Localize("PasswordEmpty"));
                return false;
            }
            return true;
        }

        private async void GoToVacationScreen()
        {
            Savelogin();
            Intent intent = new Intent(this, typeof(MainScreenActivity));
            StartActivity(intent);
            // HideProgress();
            Finish();
           // await ViewDispose();
        }

        private void DisplayErrorMessage(string text)
        {
           _error.Visibility = Android.Views.ViewStates.Visible;
           _error.Text = text;           
        }

        protected override void OnResume()
        {
            base.OnResume();
            CheckOnline();
        }

        private Task ViewDispose()
        {           
            _logo.Dispose();
            _logo.Click -= OnLogoClick;
            _loginButton.Click -= LoginButtontnClick;
            _password.EditorAction -= PasswordEditorAction;
            return Helper.Complete();
        }

        private void RetrieveUsername()
        {
            var prefs = Application.Context.GetSharedPreferences(GetString(Resource.String.ApplicationName), FileCreationMode.Private);            
            if (prefs != null)
            {
                _userName.Text = prefs.GetString("userNameEdit", String.Empty);                
            }            
        }

        private void Savelogin()
        {
            var prefs = Application.Context.GetSharedPreferences(GetString(Resource.String.ApplicationName), FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("userNameEdit", _userName.Text);
            prefEditor.PutString("passNameEdit", _password.Text);            
            prefEditor.Commit();
        }
    }
}
