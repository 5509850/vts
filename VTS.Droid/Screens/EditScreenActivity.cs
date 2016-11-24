using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Provider;
using Android.Graphics;
using System.Threading.Tasks;
using VTS.Core.CrossCutting.Helpers;
using VTS.Core.Business.ViewModel;
using VTS.Core.Data.Models;
using VTS.Droid.Helpers;
using Android.Views;

namespace VTS.Droid.Screens
{
    [Activity(Label = "EditScreenActivity", ParentActivity = typeof(MainScreenActivity))]
    public class EditScreenActivity : BaseScreenActivity
    {
        private VacationsViewModel _vacationsViewModel;
        private VacationInfoModel _vacationInfo;
        private DateTime _startDate;
        private DateTime _endDate;
        private Android.Net.Uri _imageUri;
        private Android.Net.Uri _imageUriFromFile;
        private TextView _labelEmployee;
        private TextView _employee;
        private TextView _lavelVacationType;
        private TextView _vacationType;
        private TextView _labelApprover;
        private TextView _approver;        
        private Button _vacationStartDateBtn;
        private Button _vacationEndDateBtn;
        private TextView _labelDuration;
        private TextView _duration;
        private TextView _labelVacationStatus;
        private TextView _vacationStatus;
        private TextView _labelAttachments;        
        private TextView _vacationError;
        private ImageView _vacationImageView;
        private Button _vacationPickImageFromGallery;
        private Button _vacationPickImageFromCamera;        
        private int _id;
        private const int from = 0;
        private const int to = 1;
        private const int GALLERY_CAPTURE_IMAGE_REQUEST_CODE = 0;
        private readonly int CAMERA_CAPTURE_IMAGE_REQUEST_CODE = 1;
        private bool changed = false;
        private bool _approved = false;       

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.ActionBar);
            ActionBar.SetDisplayHomeAsUpEnabled(true);            
            ActionBar.SetHomeButtonEnabled(true);         
            
            if (Intent.Extras != null)
            {                
                    _id = Intent.Extras.GetInt("ID");
            }
            else
            {
                Finish();
            }
            SetContentView(Resource.Layout.VacationDetail);
            _vacationsViewModel = FactorySingleton.Factory.Get<VacationsViewModel>();
            if (_vacationsViewModel.IsOnlineMode)
            {
                ActionBar.SetTitle(Resource.String.editRequest);
            }
            else
            {
                ActionBar.SetTitle(Resource.String.editRequestOffline);
            }
            
            InintControl();
            FillData();                                  
        }

        private void InintControl()
        {  
            _employee = FindViewById<TextView>(Resource.Id.ItemEmployee);
            _vacationType = FindViewById<TextView>(Resource.Id.ItemType);
            _approver = FindViewById<TextView>(Resource.Id.ItemApprover);
            _vacationStartDateBtn =  FindViewById<Button>(Resource.Id.ItemStartDateBtn);
            _vacationEndDateBtn = FindViewById<Button>(Resource.Id.ItemEndDateBtn); 
            _duration = FindViewById<TextView>(Resource.Id.ItemDuration);        
            _vacationStatus = FindViewById<TextView>(Resource.Id.ItemStatus);            
            _vacationError = FindViewById<TextView>(Resource.Id.ItemError);
            _vacationImageView = FindViewById<ImageView>(Resource.Id.ItemImageView);
            _vacationPickImageFromGallery = FindViewById<Button>(Resource.Id.ItemPickImageFromGallery);
            _vacationPickImageFromCamera = FindViewById<Button>(Resource.Id.ItemPickImageFromCamera);
            _labelEmployee = FindViewById<TextView>(Resource.Id.LabelEmployee);
            _lavelVacationType = FindViewById<TextView>(Resource.Id.LabelType);
            _labelApprover = FindViewById<TextView>(Resource.Id.LabelApprover);    
            _labelDuration = FindViewById<TextView>(Resource.Id.LabelDuration);
            _labelVacationStatus = FindViewById<TextView>(Resource.Id.LabelStatus);
            _labelAttachments = FindViewById<TextView>(Resource.Id.LabelAttachment);

            _employee.Typeface = FontLoader.GetFontNormal(this);
            _vacationType.Typeface = FontLoader.GetFontNormal(this);
            _approver.Typeface = FontLoader.GetFontNormal(this);
            _vacationStartDateBtn.Typeface = FontLoader.GetFontNormal(this);
            _vacationEndDateBtn.Typeface = FontLoader.GetFontNormal(this);
            _duration.Typeface = FontLoader.GetFontNormal(this);
            _vacationStatus.Typeface = FontLoader.GetFontNormal(this);            
            _vacationError.Typeface = FontLoader.GetFontNormal(this);            
            _vacationPickImageFromGallery.Typeface = FontLoader.GetFontNormal(this);
            _vacationPickImageFromCamera.Typeface = FontLoader.GetFontNormal(this);
            _labelEmployee.Typeface = FontLoader.GetFontBold(this);
            _lavelVacationType.Typeface = FontLoader.GetFontBold(this);
            _labelApprover.Typeface = FontLoader.GetFontBold(this);
            _labelDuration.Typeface = FontLoader.GetFontBold(this);
            _labelVacationStatus.Typeface = FontLoader.GetFontBold(this);
            _labelAttachments.Typeface = FontLoader.GetFontBold(this);

            _vacationStartDateBtn.Click += delegate { ShowDialog(from); };
            _vacationEndDateBtn.Click += delegate { ShowDialog(to); };
            _vacationPickImageFromGallery.Click += onVacationPickImageFromGalleryClick;
            _vacationImageView.Click += onVacationPickImageFromGalleryClick;
            _vacationPickImageFromCamera.Click += onVacationPickImageFromCameraClick;           
        }

        private async void FillData()
        {
            if (_vacationsViewModel == null)
            {
                return;
            }
            _vacationInfo = await _vacationsViewModel.GetVacationInfo(_id);
            if (_vacationsViewModel.IsOnlineMode && _vacationInfo == null)
            {
                bool result = await Relogin();
                if (result)
                {
                    _vacationInfo = await _vacationsViewModel.GetVacationInfo(_id);
                }
            }
            if (_vacationInfo == null)
            {              
                Intent myIntent = new Intent(this, typeof(MainScreenActivity));
                SetResult(Result.FirstUser, myIntent);                
                Exit();
                return;
            }
            _vacationError.Text = string.Empty;
            _employee.Text = _vacationInfo.Employee.FullName;
            _vacationType.Text = _vacationInfo.Type.Value;
            _approver.Text = _vacationInfo.Approver.FullName;
            _startDate = ConverterHelper.ConvertMillisecToDateTime(_vacationInfo.StartDate);
            _vacationStartDateBtn.Text = _startDate.ToString("d");
            _endDate = ConverterHelper.ConvertMillisecToDateTime(_vacationInfo.EndDate);
            _vacationEndDateBtn.Text = _endDate.ToString("d");
            _duration.Text = String.Format("{0} {1}", ConverterHelper.CalculateDuration(_startDate, _endDate), _vacationsViewModel.Localaizer.Localize("days"));
            if (_vacationInfo.Status.Value.Equals(VacationStatus.Approved.ToString()))
            {
                _vacationStatus.SetTextColor(Android.Graphics.Color.DarkGreen);
                _approved = true;
                _vacationStartDateBtn.Enabled =
                    _vacationPickImageFromGallery.Enabled =
                    _vacationImageView.Enabled =
                    _vacationPickImageFromCamera.Enabled =
                    _vacationEndDateBtn.Enabled = false;               
            }
            else {
                _approved = false;
                if (_vacationInfo.Status.Value.Equals(VacationStatus.Rejected.ToString()))
                {
                    _vacationStatus.SetTextColor(Android.Graphics.Color.DarkRed);                    
                }
                else
                {
                    if (_vacationInfo.Status.Value.Equals(VacationStatus.Closed.ToString()))
                    {
                        _vacationStatus.SetTextColor(Android.Graphics.Color.Black); 
                        _approved = true;
                        _vacationStartDateBtn.Enabled =
                            _vacationPickImageFromGallery.Enabled =
                            _vacationImageView.Enabled =
                            _vacationPickImageFromCamera.Enabled =
                            _vacationEndDateBtn.Enabled = false;     
                    }
                    else
                    {
                        _vacationStatus.SetTextColor(Android.Graphics.Color.Black);                    
                    }                    
                }
            }
            if (_vacationsViewModel.Image != null)
            {
                Bitmap bmp = BitmapFactory.DecodeByteArray(_vacationsViewModel.Image, 0, _vacationsViewModel.Image.Length);
                _vacationImageView.SetImageBitmap(bmp);
            }
            _vacationStatus.Text = _vacationInfo.Status.Value;
            var translate = _vacationsViewModel.Localaizer.Localize(_vacationInfo.Status.Value);
            if (!translate.Equals("N/A"))
            {
                _vacationStatus.Text = translate;
            }            
            _labelEmployee.Text = _vacationsViewModel.Employee;
            _lavelVacationType.Text = _vacationsViewModel.VacationType;
            _labelApprover.Text = _vacationsViewModel.Approver;            
            _labelDuration.Text = _vacationsViewModel.Duration;
            _labelVacationStatus.Text = _vacationsViewModel.Status;
            _labelAttachments.Text = _vacationsViewModel.Attachments;            
            _vacationPickImageFromGallery.Text = _vacationsViewModel.Gallery;
            _vacationPickImageFromCamera.Text = _vacationsViewModel.Camera; 
        }

        private async Task<bool> Relogin()
        {
            var prefs = Application.Context.GetSharedPreferences(GetString(Resource.String.ApplicationName), FileCreationMode.Private);
            bool result = false;
            if (prefs != null)
            {
                result = await FactorySingleton.Factory.Get<LoginViewModel>().SignIn(
                                prefs.GetString("userNameEdit", String.Empty),
                                prefs.GetString("passNameEdit", String.Empty));
            }
            return result;
        }

        #region events

        protected override Dialog OnCreateDialog(int id)
        {
            DatePickerDialog dialog;
            if (id == from)
            {
                dialog = new DatePickerDialog(this, HandleStartDateSet, _startDate.Year, _startDate.Month - 1, _startDate.Day);
            }
            else
            {
                dialog = new DatePickerDialog(this, HandleEndDateSet, _endDate.Year, _endDate.Month - 1, _endDate.Day);
            }
            try
            {
                long min = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
                dialog.DatePicker.MinDate = min;
            }
            catch (Exception ex)
            {
                var sd = ex.Message;
            }
            
            return dialog;
        }

        private void HandleStartDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            changed = true;
            _startDate = e.Date;
            var button = FindViewById<Button>(Resource.Id.ItemStartDateBtn);
            button.Text = _startDate.ToString("d");
            checkDateRange();
        }

        private void HandleEndDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            changed = true;
            _endDate = e.Date;
            var button = FindViewById<Button>(Resource.Id.ItemEndDateBtn);
            button.Text = _endDate.ToString("d");
            checkDateRange();
        }

        private void onVacationPickImageFromCameraClick(object sender, EventArgs e)
        {            
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            var documentspath = Android.OS.Environment.DirectoryDocuments;
            string mediaType = Android.OS.Environment.DirectoryPictures;
            Java.IO.File mediaStorageDir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(mediaType).Path);
            var file = new Java.IO.File(mediaStorageDir.Path, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            _imageUriFromFile = Android.Net.Uri.FromFile(file);
            intent.PutExtra(MediaStore.ExtraOutput, _imageUriFromFile);
            StartActivityForResult(intent, CAMERA_CAPTURE_IMAGE_REQUEST_CODE);
        }

        private async void onVacationPickImageFromGalleryClick(object sender, EventArgs e)
        {            
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(
                Intent.CreateChooser(imageIntent, "Select photo"), GALLERY_CAPTURE_IMAGE_REQUEST_CODE);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                changed = true;
                if (requestCode == CAMERA_CAPTURE_IMAGE_REQUEST_CODE)
                {
                    Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                    mediaScanIntent.SetData(_imageUriFromFile);
                    SendBroadcast(mediaScanIntent);

                    var file = new Java.IO.File(mediaScanIntent.Data.EncodedPath);
                    Bitmap bitmap = BitmapFactory.DecodeFile(file.Path);

                    _vacationImageView.SetImageBitmap(bitmap);
                    _imageUri = _imageUriFromFile;
                    return;
                }
                else
                {
                    if (requestCode == GALLERY_CAPTURE_IMAGE_REQUEST_CODE)
                    {
                        _imageUri = data.Data;
                        _vacationImageView.SetImageURI(_imageUri);
                    }
                }
            }
        }
        
        #endregion

        private void checkDateRange()
        {
            if (_startDate != null && _endDate != null)
            {
                _vacationError.Text = (_startDate > _endDate) ? _vacationsViewModel.Localaizer.Localize("dateError") : string.Empty;
                _duration.Text = (_startDate > _endDate) ? string.Empty : String.Format("{0} {1}", ConverterHelper.CalculateDuration(_startDate, _endDate), _vacationsViewModel.Localaizer.Localize("days"));
            }
        }
        private async Task<bool> Save()
        {
            bool result = false;
            if (_approved)
            {
                _vacationError.Text = _vacationsViewModel.Localaizer.Localize("dateApproved");
                AlertDialog(_vacationsViewModel.Localaizer.Localize("dateApproved"), null);
                return false;
            }
            if (_startDate > _endDate)
            {
                _vacationError.Text = _vacationsViewModel.Localaizer.Localize("dateError");
                AlertDialog(_vacationsViewModel.Localaizer.Localize("dateError"), null);                
                return false;
            }
            else
            {
                _vacationError.Text = string.Empty;
                _vacationInfo.StartDate = (long)(_startDate - new DateTime(1970, 1, 1)).TotalMilliseconds;
                _vacationInfo.EndDate = (long)(_endDate - new DateTime(1970, 1, 1)).TotalMilliseconds;

                if (_imageUri != null)
                {
                    _vacationsViewModel.Image = Utils.GetByteByURI(this.ApplicationContext, _imageUri);
                }
                try
                {
                    if (_vacationInfo != null)
                    {
                        ShowProgress(_vacationsViewModel.Localaizer.Localize("sending"));
                        await Task.Delay(500);
                        result = await _vacationsViewModel.UpdateOrCreateVacationInfo(_vacationInfo);
                        HideProgress();
                    }
                }
                catch (Exception ex)
                {
                    _vacationError.Text = ex.Message;
                    HideProgress();
                    return false;
                }
                Intent myIntent = new Intent(this, typeof(MainScreenActivity));
                SetResult(Result.Ok, myIntent);
                Exit();
            }
            return true;
        }
        private async void Exit()
        {
            Finish();
            await ViewDispose();
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

        private bool ReturnToMainScreen()
        {
            if (changed)
            {
                var alert = new AlertDialog.Builder(this).Create();

                var alertView = LayoutInflater.Inflate(Resource.Layout.confirmationPopup, null);
                alertView.FindViewById<TextView>(Resource.Id.textView2).Text = _vacationsViewModel.Localaizer.Localize("confirmationChange");
                alertView.FindViewById<Button>(Resource.Id.confirmation_popup_cancel_button).Text = _vacationsViewModel.Localaizer.Localize("cancel_button");
                alertView.FindViewById<Button>(Resource.Id.confirmation_popup_ok_button).Text = _vacationsViewModel.Localaizer.Localize("ok_button");
                alertView.FindViewById<TextView>(Resource.Id.textView2).Typeface = FontLoader.GetFontBold(this);
                alertView.FindViewById<Button>(Resource.Id.confirmation_popup_cancel_button).Typeface = FontLoader.GetFontNormal(this);
                alertView.FindViewById<Button>(Resource.Id.confirmation_popup_ok_button).Typeface = FontLoader.GetFontNormal(this);  
                alertView.FindViewById<Button>(Resource.Id.confirmation_popup_cancel_button).Click += (s, e) =>
                {
                    alert.Hide();
                    alert.Cancel();
                    Exit();
                };
                alertView.FindViewById<Button>(Resource.Id.confirmation_popup_ok_button).Click += async (s, e) =>
                {
                    alert.Hide();
                    alert.Cancel();
                    await Save();
                };

                alert.SetView(alertView);
                alert.Show();
                return false;
            }
            ViewDispose();
            return true;          
        }
        #endregion

        public override void OnBackPressed()
        {
            if (ReturnToMainScreen())
            {
                ViewDispose();
                base.OnBackPressed();
            }
        }

        protected override void OnSaveInstanceState(Bundle outstate)
        {
            try
            {
                DismissDialog(from);
                DismissDialog(to);
            }
            catch (Exception ex)
            {
                var sd = ex.Message;
            }
            
            base.OnSaveInstanceState(outstate);
        }        
       
        private Task ViewDispose()
        {           
            //_vacationImageView.SetImageBitmap(null);
            //_vacationImageView.Dispose();
            //_vacationImageView = null;

            //_vacationStartDateBtn.Click -= delegate { ShowDialog(0); };
            //_vacationEndDateBtn.Click -= delegate { ShowDialog(1); };
            //_vacationUpdate.Click -= onVacationUpdateButtonClicked;
            //_vacationPickImageFromGallery.Click -= onVacationPickImageFromGalleryClicked;
            //_vacationPickImageFromCamera.Click -= onVacationPickImageFromCameraClicked;

            return Helper.Complete();
        }
    }
}