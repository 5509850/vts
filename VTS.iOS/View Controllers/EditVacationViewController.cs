using Cirrious.FluentLayouts.Touch;
using System;
using System.Drawing;
using UIKit;
using VTS.Core.Business.ViewModel;
using VTS.iOS.Helpers;
using VTS.iOS.Ninject;
using System.Threading.Tasks;
using VTS.iOS.Navigation;
using VTS.Core.Data.Models;
using VTS.Core.CrossCutting.Helpers;
using Foundation;
using VTS.Core.Business;

namespace VTS.iOS.View_Controllers
{
    public class EditVacationViewController : ViewControllerBase
    {
        #region filds      

        public int ID { get; set; }      
        protected VacationsViewModel _vacationsViewModel;
        protected VacationInfoModel _vacationInfo;
        protected DateTime _startDate;
        protected DateTime _endDate;
        protected UIBarButtonItem backButton, doneButton;        
        protected UILabel _labelEmployee;
        protected UILabel _employee;
        protected UILabel _labelVacationType;
        protected UILabel _vacationType;
        protected UILabel _labelApprover;
        protected UILabel _approver;
        protected UILabel _vacationError;
        protected UIDatePicker _vacationStartDateBtn;
        protected UIDatePicker _vacationEndDateBtn;
        protected UILabel _labelDuration;
        protected UILabel _duration;
        protected UILabel _labelVacationStatus;
        protected UILabel _vacationStatus;
        protected UILabel _labelAttachments;        
        protected UIButton _vacationPickImageFromGallery;
        protected UIButton _vacationPickImageFromCamera;   
        protected UIImageView _vacationImage;
        protected UIImagePickerController _imagePicker;
        protected byte[] _tempImg; 
           
        #endregion

        #region private var
        int topPading = 80;     
        int buttonHight = 30;
        int buttonWidth = 100;
        int spacing = 20;
        int datepickerHeigh = 60;       
        int imagesize = 80;
        private bool _approved = false;
        protected bool _keepResource = false;
        #endregion

        #region override methodes
        protected override void CreateLayout()
        {
            _labelEmployee = new UILabel();
            _labelEmployee.TextAlignment = UITextAlignment.Left;
            Add(_labelEmployee);

            _employee = new UILabel();
            _employee.TextAlignment = UITextAlignment.Left;
            Add(_employee);

            _labelVacationType = new UILabel();
            _labelVacationType.TextAlignment = UITextAlignment.Left;
            Add(_labelVacationType);

            _vacationType = new UILabel();
            _vacationType.TextAlignment = UITextAlignment.Left;
            Add(_vacationType);

            _labelApprover = new UILabel();
            _labelApprover.TextAlignment = UITextAlignment.Left;
            Add(_labelApprover);

            _approver = new UILabel();
            _approver.TextAlignment = UITextAlignment.Left;
            Add(_approver);

             _vacationError = new UILabel();
            _vacationError.TextColor = UIColor.FromRGB(235, 106, 90);
            _vacationError.TextAlignment = UITextAlignment.Center;
            Add(_vacationError);
           
            _vacationStartDateBtn = new UIDatePicker();
            _vacationStartDateBtn.Mode = UIDatePickerMode.Date;            
            Add(_vacationStartDateBtn);
          
            _vacationEndDateBtn = new UIDatePicker();
            _vacationEndDateBtn.Mode = UIDatePickerMode.Date;
            Add(_vacationEndDateBtn);

            _labelDuration = new UILabel();
            _labelDuration.TextAlignment = UITextAlignment.Left;
            Add(_labelDuration);

            _duration = new UILabel();
            _duration.TextAlignment = UITextAlignment.Left;
            Add(_duration);

            _labelVacationStatus = new UILabel();
            _labelVacationStatus.TextAlignment = UITextAlignment.Left;
            Add(_labelVacationStatus);

            _vacationStatus = new UILabel();
            _vacationStatus.TextAlignment = UITextAlignment.Left;
            Add(_vacationStatus);

            _labelAttachments = new UILabel();
            _labelAttachments.TextAlignment = UITextAlignment.Center;
            Add(_labelAttachments);          

            _vacationPickImageFromGallery = new UIButton();
            _vacationPickImageFromGallery.SetTitleColor(UIColor.FromRGB(255, 255, 255), UIControlState.Normal);
            _vacationPickImageFromGallery.BackgroundColor = UIColor.FromRGB(39, 171, 192);
            Add(_vacationPickImageFromGallery);

            _vacationPickImageFromCamera = new UIButton();
            _vacationPickImageFromCamera.SetTitleColor(UIColor.FromRGB(255, 255, 255), UIControlState.Normal);
            _vacationPickImageFromCamera.BackgroundColor = UIColor.FromRGB(39, 171, 192);
            Add(_vacationPickImageFromCamera);

            _vacationImage = new UIImageView(UIImage.FromFile("Images/person.png"));
            _vacationImage.Frame = new Rectangle(0, 0, 80, 80);
            Add(_vacationImage);

            _imagePicker = new UIImagePickerController();           
        }

        protected override void ApplyConstraints()
        {
            View.BackgroundColor = UIColor.FromRGB(230, 229, 228);
            View.AddConstraints(
                _labelEmployee.AtTopOf(View, topPading),
                _labelEmployee.AtLeftOf(View, UIScreen.MainScreen.Bounds.Width / 9),
                _labelEmployee.Width().EqualTo(buttonWidth),
                _labelEmployee.Height().EqualTo(buttonHight),

                    _employee.AtTopOf(View, topPading),
                    _employee.AtRightOf(View, UIScreen.MainScreen.Bounds.Width / 9),
                    _employee.WithSameTop(_labelEmployee),
                    _employee.Width().EqualTo(buttonWidth),
                    _employee.Height().EqualTo(buttonHight),

                _labelVacationType.Below(_labelEmployee),
                _labelVacationType.WithSameLeft(_labelEmployee),
                _labelVacationType.WithSameWidth(_labelEmployee),
                _labelVacationType.WithSameHeight(_labelEmployee),

                    _vacationType.Below(_employee),
                    _vacationType.WithSameLeft(_employee),
                    _vacationType.WithSameWidth(_employee),
                    _vacationType.WithSameHeight(_employee),

                _labelApprover.Below(_labelVacationType),
                _labelApprover.WithSameLeft(_labelVacationType),
                _labelApprover.WithSameWidth(_labelVacationType),
                _labelApprover.WithSameHeight(_labelVacationType),

                    _approver.Below(_vacationType),
                    _approver.WithSameLeft(_vacationType),
                    _approver.WithSameWidth(_vacationType),
                    _approver.WithSameHeight(_vacationType),

            _vacationError.Below(_vacationType, spacing),
            _vacationError.AtLeftOf(View),
            _vacationError.Width().EqualTo(UIScreen.MainScreen.Bounds.Width),
            _vacationError.Height().EqualTo(buttonHight),

            _vacationStartDateBtn.Below(_vacationError),
            _vacationStartDateBtn.WithSameCenterX(View),
            _vacationStartDateBtn.Width().EqualTo(UIScreen.MainScreen.Bounds.Width).Minus((UIScreen.MainScreen.Bounds.Width / 9) * 2),
            _vacationStartDateBtn.Height().EqualTo(datepickerHeigh),

            _vacationEndDateBtn.Below(_vacationStartDateBtn),
            _vacationEndDateBtn.WithSameCenterX(View),
            _vacationEndDateBtn.Width().EqualTo(UIScreen.MainScreen.Bounds.Width).Minus((UIScreen.MainScreen.Bounds.Width / 9) * 2),
                _vacationEndDateBtn.Height().EqualTo(datepickerHeigh),

                _labelDuration.Below(_vacationEndDateBtn),
                _labelDuration.WithSameLeft(_labelVacationType),
                _labelDuration.WithSameWidth(_labelVacationType),
                _labelDuration.WithSameHeight(_labelVacationType),

                    _duration.Below(_vacationEndDateBtn),
                    _duration.WithSameLeft(_vacationType),
                    _duration.WithSameWidth(_vacationType),
                    _duration.WithSameHeight(_vacationType),

                _labelVacationStatus.Below(_labelDuration),
                _labelVacationStatus.WithSameLeft(_labelVacationType),
                _labelVacationStatus.WithSameWidth(_labelVacationType),
                _labelVacationStatus.WithSameHeight(_labelVacationType),

                    _vacationStatus.Below(_duration),
                    _vacationStatus.WithSameLeft(_vacationType),
                    _vacationStatus.WithSameWidth(_vacationType),
                    _vacationStatus.WithSameHeight(_vacationType),                    

        _labelAttachments.Below(_vacationStatus),
        _labelAttachments.AtLeftOf(View),
        _labelAttachments.Width().EqualTo(UIScreen.MainScreen.Bounds.Width),
        _labelAttachments.Height().EqualTo(buttonHight),

                _vacationPickImageFromGallery.Below(_labelAttachments),
                _vacationPickImageFromGallery.WithSameLeft(_labelVacationType),
                _vacationPickImageFromGallery.WithSameWidth(_labelVacationType),
                _vacationPickImageFromGallery.WithSameHeight(_labelVacationType),

                    _vacationPickImageFromCamera.Below(_labelAttachments),
                    _vacationPickImageFromCamera.WithSameLeft(_vacationType),
                    _vacationPickImageFromCamera.WithSameWidth(_vacationType),
                    _vacationPickImageFromCamera.WithSameHeight(_vacationType),

        _vacationImage.Below(_vacationPickImageFromGallery, spacing),
        _vacationImage.WithSameCenterX(_labelAttachments),
        _vacationImage.Width().EqualTo(imagesize),
        _vacationImage.Height().EqualTo(imagesize)
                );
        }

        protected override void FetchDataToControl()
        {
            if (ID == 0)
            {
                return;
            }
            _vacationError.Text = string.Empty;
            _vacationsViewModel = FactorySingleton.Factory.Get<VacationsViewModel>();

            var title = string.Empty;
            if (_vacationsViewModel.IsOnlineMode)
            {
                title = Localize("editRequest"); 
            }
            else
            {
                title = Localize("editRequestOffline"); 
            }
            NavigationItem.Title = title;
            NavigationItem.SetHidesBackButton(false, false);

            doneButton = new UIBarButtonItem();
            doneButton.Title = Localize("done");            
            NavigationItem.RightBarButtonItem = doneButton;            

            backButton = new UIBarButtonItem();
            backButton.Title = Localize("cancel_button");            
            NavigationItem.LeftBarButtonItem = backButton;          
            InintAndLocalizeControl();
            LoadFonts();
        }

        protected override void ApplyEvents()
        {
            doneButton.Clicked += DoneButtonClicked;
            backButton.Clicked += BackButtonClicked;          
            _vacationStartDateBtn.ValueChanged += VacationStartDateBtnValueChanged;
            _vacationEndDateBtn.ValueChanged += VacationEndDateBtnValueChanged;
            _vacationPickImageFromGallery.TouchUpInside += VacationPickImageFromGalleryTouchUpInside;            
            _vacationPickImageFromCamera.TouchUpInside += VacationPickImageFromCameraTouchUpInside;
            _imagePicker.FinishedPickingMedia += HandleFinishedPickingImage;
            _imagePicker.Canceled += ImagePickerCanceled;         
        }

        protected void ImagePickerCanceled(object sender, EventArgs e)
        {
            _imagePicker.DismissModalViewController(true);
            _keepResource = false;
        }

        protected void HandleFinishedPickingImage(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            bool isImage = false;
            switch (e.Info[UIImagePickerController.MediaType].ToString())
            {
                case "public.image":                 
                    isImage = true;
                    break;
                case "public.video":                   
                    break;
            }           
            if (isImage)
            {
                UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
                if (originalImage != null && _vacationImage != null)
                {                    
                    _vacationImage.Image = originalImage;
                    _tempImg = ImageConverter.GetByteByImg(originalImage);
                }
            }            
            _imagePicker.DismissModalViewController(true);
            _keepResource = false;
        }
        #endregion

        protected void VacationStartDateBtnValueChanged(object sender, EventArgs e)
        {
            _startDate = Utils.NSDateToDateTime(_vacationStartDateBtn.Date).Date;
            checkDateRange();          
        }

        protected void VacationEndDateBtnValueChanged(object sender, EventArgs e)
        {            
            _endDate = Utils.NSDateToDateTime(_vacationEndDateBtn.Date).Date;
            checkDateRange();           
        }
        
        protected void VacationPickImageFromCameraTouchUpInside(object sender, EventArgs e)
        {
            _keepResource = true;
            _imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
            _imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.Camera);
            NavigationController.PresentModalViewController(_imagePicker, true);
        }
        protected void VacationPickImageFromGalleryTouchUpInside(object sender, EventArgs e)
        {
            _keepResource = true;
            _imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            _imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);            
            NavigationController.PresentModalViewController(_imagePicker, true);
        }

        protected void checkDateRange()
        {
            if (_startDate != null && _endDate != null)
            {
                _vacationError.Text = (_startDate > _endDate) ? Localize("dateError") : string.Empty;
                _duration.Text = (_startDate > _endDate) ? string.Empty : String.Format("{0} {1}", ConverterHelper.CalculateDuration(_startDate, _endDate), Localize("days"));
            }
        }

        protected override void DisposeResource()
        {
            if (ID == 0)
            {
                return;
            }
            doneButton.Clicked -= DoneButtonClicked;            
            backButton.Clicked -= BackButtonClicked;
            _vacationStartDateBtn.ValueChanged -= VacationStartDateBtnValueChanged;
            _vacationEndDateBtn.ValueChanged -= VacationEndDateBtnValueChanged;
            _vacationPickImageFromGallery.TouchUpInside -= VacationPickImageFromGalleryTouchUpInside;
            _vacationPickImageFromCamera.TouchUpInside -= VacationPickImageFromCameraTouchUpInside;
            if (!_keepResource)
            {
                _imagePicker.FinishedPickingMedia -= HandleFinishedPickingImage;
                _imagePicker.Canceled -= ImagePickerCanceled;
                _tempImg = null;
                _vacationImage.Image = null;
                _vacationImage = null;
            }
        }

        protected void LoadFonts()
        {
            _employee.Font = FontLoader.GetFontNormal();
            _vacationType.Font = FontLoader.GetFontNormal();
            _approver.Font = FontLoader.GetFontNormal();           
            _duration.Font = FontLoader.GetFontNormal();
            _vacationStatus.Font = FontLoader.GetFontNormal();
            _vacationError.Font = FontLoader.GetFontNormal();
            _vacationPickImageFromGallery.Font = FontLoader.GetFontNormal();
            _vacationPickImageFromCamera.Font = FontLoader.GetFontNormal();
            _labelEmployee.Font = FontLoader.GetFontBold();
            _labelVacationType.Font = FontLoader.GetFontBold();
            _labelApprover.Font = FontLoader.GetFontBold();
            _labelDuration.Font = FontLoader.GetFontBold();
            _labelVacationStatus.Font = FontLoader.GetFontBold();
            _labelAttachments.Font = FontLoader.GetFontBold();
        }

        protected async void InintAndLocalizeControl()
        {
            if (_vacationsViewModel == null)
            {
                GoToLoginScreen();
                return;
            }
            _vacationInfo = await _vacationsViewModel.GetVacationInfo(ID);
            if (_vacationInfo == null && _vacationsViewModel.State == UserState.Unauthorized)
            {
                GoToLoginScreen();
                return;
            }
            _vacationError.Text = string.Empty;
            _employee.Text = _vacationInfo.Employee.FullName;
            _vacationType.Text = _vacationInfo.Type.Value;
            _approver.Text = _vacationInfo.Approver.FullName;
            _startDate = ConverterHelper.ConvertMillisecToDateTime(_vacationInfo.StartDate).Date;            
            //_vacationStartDateBtn.MinimumDate = (NSDate)DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            _vacationStartDateBtn.SetDate((NSDate)_startDate.Date.AddHours(2), true);
            _endDate = ConverterHelper.ConvertMillisecToDateTime(_vacationInfo.EndDate).Date;
            //_vacationEndDateBtn.MinimumDate = (NSDate)DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            _vacationEndDateBtn.SetDate((NSDate)_endDate.Date.AddHours(2), true);                        
            _duration.Text = string.Format("{0} {1}", ConverterHelper.CalculateDuration(_startDate, _endDate), Localize("days"));
            if (_vacationInfo.Status.Value.Equals(VacationStatus.Approved.ToString()))
            {
                _approved = true;
                _vacationStatus.TextColor = UIColor.Green;
                _vacationStartDateBtn.Enabled =
                    _vacationPickImageFromGallery.Enabled =
                    _vacationPickImageFromCamera.Enabled =
                    _vacationEndDateBtn.Enabled = false;
            }
            else
            {
                _approved = false;
                if (_vacationInfo.Status.Value.Equals(VacationStatus.Rejected.ToString()))
                {
                    _vacationStatus.TextColor = UIColor.Red;                
                }
                else
                {
                    if (_vacationInfo.Status.Value.Equals(VacationStatus.Closed.ToString()))
                    {                     
                        _vacationStatus.TextColor = UIColor.Black;                                                               
                    }
                    else
                    {
                        _vacationStatus.TextColor = UIColor.Black;                                        
                    }
                }
            }
            if (_vacationsViewModel.Image != null)
            {
                UIImage img = ImageConverter.ToImage(_vacationsViewModel.Image);
                if (img != null)
                {
                    _vacationImage.Image = img;
                }                
            }
            _vacationStatus.Text = _vacationInfo.Status.Value;
            var translate = Localize(_vacationInfo.Status.Value);
            if (!translate.Equals("N/A"))
            {
                _vacationStatus.Text = translate;
            }
            _labelEmployee.Text = _vacationsViewModel.Employee;
            _labelVacationType.Text = _vacationsViewModel.VacationType;
            _labelApprover.Text = _vacationsViewModel.Approver;
            _labelDuration.Text = _vacationsViewModel.Duration;
            _labelVacationStatus.Text = _vacationsViewModel.Status;
            _labelAttachments.Text = _vacationsViewModel.Attachments;
            _vacationPickImageFromGallery.SetTitle(_vacationsViewModel.Gallery, UIControlState.Normal);
            _vacationPickImageFromCamera.SetTitle(_vacationsViewModel.Camera, UIControlState.Normal);
        }      

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);           
        }

        protected void GoToVacationScreen()
        {
            _vacationError.Text = Localize("wait");
            FactorySingleton.Factory.Get<NavigationService>()
               .NavigateWithSlide(new VacationViewController());
        }

        protected void GoToLoginScreen()
        {            
            FactorySingleton.Factory.Get<NavigationService>()
                .Navigate(new LoginViewController());
        }

        protected void BackButtonClicked(object sender, EventArgs e)
        {
            GoToVacationScreen();
        }

        protected void DoneButtonClicked(object sender, EventArgs e)
        {
            Save();    
        }

        protected async Task<bool> Save()
        {
            bool result = false;
            if (_approved)
            {
                _vacationError.Text = Localize("dateApproved");
                Allert(_vacationError.Text);                
                return false;
            }
            if (_startDate > _endDate)
            {
                _vacationError.Text = Localize("dateError");
                Allert(_vacationError.Text);
                return false;
            }
            else
            {
                _vacationError.Text = string.Empty;
                _vacationInfo.StartDate = ConverterHelper.ConvertDateTimeToMillisec(_startDate);
                _vacationInfo.EndDate = ConverterHelper.ConvertDateTimeToMillisec(_endDate);

                if (_tempImg != null)
                {
                    _vacationsViewModel.Image = _tempImg;
                }
                try
                {
                    if (_vacationInfo != null)
                    {
                        _vacationError.Text = Localize("sending");
                        _vacationInfo.Status.Value = VacationStatus.WaitingForApproval.ToString();
                        result = await _vacationsViewModel.UpdateOrCreateVacationInfo(_vacationInfo);                        
                    }
                }
                catch (Exception ex)
                {
                    _vacationError.Text = ex.Message;                    
                    return false;
                }
                GoToVacationScreen();
            }
            return true;
        }
        protected void Allert(string message)
        {
            UIAlertController okAlertController = UIAlertController.Create(Localize("Attention"), message, UIAlertControllerStyle.Alert);
            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            PresentViewController(okAlertController, true, null);
        }
    }
}
