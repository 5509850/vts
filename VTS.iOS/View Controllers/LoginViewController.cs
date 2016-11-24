using Cirrious.FluentLayouts.Touch;
using System;
using System.Drawing;
using UIKit;
using VTS.Core.Business.ViewModel;
using VTS.iOS.Helpers;
using VTS.iOS.Ninject;
using System.Threading.Tasks;
using VTS.iOS.Navigation;

namespace VTS.iOS.View_Controllers
{
    public class LoginViewController : ViewControllerBase
    {
        #region fields
        private UILabel _titleView;
        private UIImageView _logo;
        private UILabel _errorLabel;
        private UITextField _userName;
        private UITextField _password;
        private UIButton _loginButton;
        private UILabel _epamConfidential;
        #endregion

        #region private var
        int errorless = 10;
        int buttonHight = 30;
        int spacing = 20;
        int confidentialHight = 200;
       // nfloat screenWidth = UIScreen.MainScreen.Bounds.Width;
        int lefRightmargin = 50;
        private LoginViewModel _loginViewModel;
        #endregion

        protected override void CreateLayout()
        {
            _titleView = new UILabel();
            _titleView.TextColor = UIColor.FromRGB(255, 255, 255);
            _titleView.BackgroundColor = UIColor.FromRGB(26, 156, 176);
            _titleView.TextAlignment = UITextAlignment.Center;            
            Add(_titleView);

            _logo = new UIImageView(UIImage.FromBundle("logo.png"));
            _logo.Frame = new Rectangle(0, 0, 200, 80);
            Add(_logo);

            _errorLabel = new UILabel();
            _errorLabel.TextColor = UIColor.FromRGB(235, 106, 90);
            _errorLabel.TextAlignment = UITextAlignment.Center;            
            Add(_errorLabel);

            _userName = new UITextField();            
            _userName.BackgroundColor = UIColor.FromRGB(255, 255, 255);
            _userName.AutocorrectionType = UITextAutocorrectionType.No;
            Add(_userName);

            _password = new UITextField();            
            _password.BackgroundColor = UIColor.FromRGB(255, 255, 255);
            _userName.AutocorrectionType = UITextAutocorrectionType.No;
            _password.SecureTextEntry = true;
            Add(_password);

            _loginButton = new UIButton();            
            _loginButton.SetTitleColor(UIColor.FromRGB(255, 255, 255), UIControlState.Normal);
            _loginButton.BackgroundColor = UIColor.FromRGB(163, 198, 68);
            Add(_loginButton);

            _epamConfidential = new UILabel();            
            _epamConfidential.TextAlignment = UITextAlignment.Justified;
            _epamConfidential.TextColor = UIColor.FromRGB(182, 170, 170);
            _epamConfidential.LineBreakMode = UILineBreakMode.WordWrap;
            _epamConfidential.Lines = 10;
            Add(_epamConfidential);
            NavigationController.NavigationBarHidden = true;
        } 
               
        protected override void ApplyConstraints()
        {
            View.BackgroundColor = UIColor.FromRGB(230, 229, 228);
            View.AddConstraints(
                _titleView.AtTopOf(View, 20),
                _titleView.AtLeftOf(View),
                _titleView.Width().EqualTo(UIScreen.MainScreen.Bounds.Width),
                _titleView.Height().EqualTo(buttonHight * 2),

                _logo.Below(_titleView),
                _logo.WithSameCenterX(_titleView),
                _logo.Height().EqualTo(80),
                _logo.Width().EqualTo(200),

                _errorLabel.Below(_logo),
                _errorLabel.WithSameCenterX(_titleView),
                _errorLabel.WithSameWidth(View),
                _errorLabel.Height().EqualTo(buttonHight).Minus(errorless),

                _userName.Below(_errorLabel),
                _userName.WithSameCenterX(_titleView),
                _userName.WithSameWidth(_titleView).Minus(lefRightmargin),
                _userName.Height().EqualTo(buttonHight),

                _password.Below(_userName, spacing),
                _password.WithSameCenterX(_userName),
                _password.WithSameWidth(_userName),
                _password.WithSameHeight(_userName),

                _loginButton.Below(_password, spacing),
                _loginButton.WithSameCenterX(_userName),
                _loginButton.WithSameWidth(_userName),
                _loginButton.WithSameHeight(_userName),

                _epamConfidential.Below(_loginButton),
                _epamConfidential.WithSameCenterX(_userName),
                _epamConfidential.WithSameWidth(_userName),
                _epamConfidential.Height().EqualTo(confidentialHight)
                );
        }
        
        protected override void FetchDataToControl()
        {
            _errorLabel.Text = string.Empty;
            _loginViewModel = FactorySingleton.Factory.Get<LoginViewModel>();
            LocalizeControl();
            LoadFonts();
        }

        protected override void ApplyEvents()
        {
            _userName.ShouldReturn = delegate
            {
                if (!string.IsNullOrEmpty(_userName.Text))
                {
                    _password.BecomeFirstResponder();
                    return true;
                }
                return false;
            };
            _password.ShouldReturn = delegate
            {
                if (!string.IsNullOrEmpty(_password.Text))
                {
                    LoginDo();
                    return true;
                }
                return false;
            };
            _loginButton.TouchUpInside += LoginButtonTouchUpInside;         
        }

        private void LoginButtonTouchUpInside(object sender, EventArgs e)
        {
            LoginDo();
        }

        protected override void DisposeResource()
        {
            _userName.ShouldReturn = null;
            _password.ShouldReturn = null;
            _loginButton.TouchUpInside -= LoginButtonTouchUpInside;
            _loginViewModel = null;
            _logo = null;
        }

        private void LoadFonts()
        {
            _epamConfidential.Font = FontLoader.GetFontLight();
            _loginButton.Font = 
                _titleView.Font = FontLoader.GetFontBold();
            _errorLabel.Font = 
                _userName.Font = 
                _password.Font = FontLoader.GetFontNormal();
        }

        private void LocalizeControl()
        {           
            _titleView.Text = _loginViewModel.LoginModelLocalize.Name;
            _userName.Placeholder = _loginViewModel.LoginModelLocalize.UserName;
            _password.Placeholder = _loginViewModel.LoginModelLocalize.Password;
            _loginButton.SetTitle(_loginViewModel.LoginModelLocalize.Login, UIControlState.Normal);
            _epamConfidential.Text = _loginViewModel.LoginModelLocalize.Confidential;
        }

        private void HideKeyboard()
        {
            _userName.ResignFirstResponder();
            _password.ResignFirstResponder();
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);           
        }                      

        private async Task LoginDo()
        {
            HideKeyboard();
            _errorLabel.Text = string.Empty;
            try
            {
                if (ValidateInput())
                {
                    _loginButton.Enabled = false;                  
                    bool result = await _loginViewModel.SignIn(_userName.Text, _password.Text);                     
                    if (result)
                    {
                        _errorLabel.Text = string.Empty;
                        GoToVacationScreen();
                    }
                    else
                    {
                        _loginButton.Enabled = true;
                        DisplayErrorMessage(_loginViewModel.AuthorizationError);                       
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayErrorMessage("Error = " + ex.Message);
            }
        }

        private void GoToVacationScreen()
        {
            _errorLabel.Text = Localize("wait");
            FactorySingleton.Factory.Get<NavigationService>()
                .NavigateWithSlide(new VacationViewController());  
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(_userName.Text))
            {
                DisplayErrorMessage(Localize("EmailEmpty"));
                return false;
            }

            if (string.IsNullOrEmpty(_password.Text))
            {
                DisplayErrorMessage(Localize("PasswordEmpty"));
                return false;
            }
            return true;
        }
      
        private void DisplayErrorMessage(string v)
        {
             _errorLabel.Text = v;
        }
    }
}
