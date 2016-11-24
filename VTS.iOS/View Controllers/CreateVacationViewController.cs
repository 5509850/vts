using System;
using UIKit;
using VTS.Core.Business.ViewModel;
using VTS.iOS.Ninject;
using System.Threading.Tasks;
using VTS.Core.CrossCutting.Helpers;
using Foundation;
using VTS.iOS.Helpers;
using VTS.Core.Business;
using VTS.Core.Data.Models;
using VTS.iOS.Navigation;

namespace VTS.iOS.View_Controllers
{
    public class CreateVacationViewController : EditVacationViewController
    {
        public int TypeVacation { get; set; }
        protected UIBarButtonItem createButton;
        protected override void FetchDataToControl()
        {            
            _vacationError.Text = string.Empty;
            _vacationsViewModel = FactorySingleton.Factory.Get<VacationsViewModel>();

            var title = string.Empty;
            if (_vacationsViewModel.IsOnlineMode)
            {
                title = Localize("createVacationRequest");
            }
            else
            {
                title = Localize("createVacationRequestOffline");
            }
            NavigationItem.Title = title;
            NavigationItem.SetHidesBackButton(false, false);

            createButton = new UIBarButtonItem();
            createButton.Title = Localize("done");
            NavigationItem.RightBarButtonItem = createButton;

            backButton = new UIBarButtonItem();
            backButton.Title = Localize("cancel_button");
            NavigationItem.LeftBarButtonItem = backButton;
            InintAndLocalizeNewControl();
            LoadFonts();
        }

        private async void InintAndLocalizeNewControl()
        {
            if (TypeVacation == Utils.LIST_VACATION)
            {
                GoToListVacationScreen();
                return;
            }            
            if (_vacationsViewModel == null)
            {
                GoToLoginScreen();
                return;
            }            
            _vacationInfo = await _vacationsViewModel.CreateDraftVacationInfo();
            if (_vacationInfo == null && _vacationsViewModel.State == UserState.Unauthorized)
            {
                GoToLoginScreen();
                return;
            }
            _vacationError.Text = string.Empty;
            switch (TypeVacation)
            {
                case Utils.VACATION_REQUEST:
                    {
                        _vacationInfo.Type.Key = "VAC";
                        _vacationInfo.Type.Value = "Regular (VAC)";
                        break;
                    }
                case Utils.SICK_REQUEST:
                    {
                        _vacationInfo.Type.Key = "ILL";
                        _vacationInfo.Type.Value = "Illness (ILL)";
                        break;
                    }
                case Utils.OVERTIME_REQUEST:
                    {
                        _vacationInfo.Type.Key = "OVT";
                        _vacationInfo.Type.Value = "Overtime (OVT)";
                        break;
                    }
                case Utils.LIVEWOP_REQUEST:
                    {
                        _vacationInfo.Type.Key = "POV";
                        _vacationInfo.Type.Value = "Without pay (POV)";
                        break;
                    }
                case Utils.EXCEPTIONAL_REQUEST:
                    {
                        _vacationInfo.Type.Key = "EXV";
                        _vacationInfo.Type.Value = "EXCEPTIONAL (EXV)";
                        break;
                    }

                default:
                    {
                        _vacationInfo.Type.Key = "VAC";
                        _vacationInfo.Type.Value = "Regular (VAC)";
                        break;
                    }
            }
            await _vacationsViewModel.UpdateDraftVacationInfo(_vacationInfo);
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
            _vacationStatus.TextColor = UIColor.Black;            
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

        private void GoToListVacationScreen()
        {
            FactorySingleton.Factory.Get<NavigationService>()
                .Navigate(new VacationViewController());
        }

        protected override void ApplyEvents()
        {
            createButton.Clicked += CreateButtonClicked;
            backButton.Clicked += BackButtonClicked;
            _vacationStartDateBtn.ValueChanged += VacationStartDateBtnValueChanged;
            _vacationEndDateBtn.ValueChanged += VacationEndDateBtnValueChanged;
            _vacationPickImageFromGallery.TouchUpInside += VacationPickImageFromGalleryTouchUpInside;
            _vacationPickImageFromCamera.TouchUpInside += VacationPickImageFromCameraTouchUpInside;
            _imagePicker.FinishedPickingMedia += HandleFinishedPickingImage;
            _imagePicker.Canceled += ImagePickerCanceled;
        }

        protected override void DisposeResource()
        {
            createButton.Clicked -= CreateButtonClicked;
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

        private void CreateButtonClicked(object sender, EventArgs e)
        {
            Create();
        }

        protected async Task<bool> Create()
        {
            bool result = false;          
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
                        await _vacationsViewModel.UpdateDraftVacationInfo(_vacationInfo);
                        result = await _vacationsViewModel.SendDraftVacationInfo();
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
    }
}
