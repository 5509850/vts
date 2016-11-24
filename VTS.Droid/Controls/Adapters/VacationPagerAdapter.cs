using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using VTS.Core.CrossCutting.Helpers;
using Android.Support.V4.View;
using VTS.Droid.Controls.Fragments;
using Android.Provider;
using Android.Graphics;
using System.Threading.Tasks;
using VTS.Droid.Screens;
using VTS.Droid.Helpers;
using VTS.Core.Business.ViewModel;
using VTS.Core.Data.Models;

namespace VTS.Droid.Adapters
{
    public class VacationPagerAdapter : PagerAdapter
    {
        List<string> items = new List<string>();
        private ViewGroup _container;
        private Android.Net.Uri _imageUri;
        private Android.Net.Uri _imageUriFromFile;        
        private const int GALLERY_CAPTURE_IMAGE_REQUEST_CODE = 0;
        private readonly int CAMERA_CAPTURE_IMAGE_REQUEST_CODE = 1;
        private VacationsViewModel _vacationsViewModel;
        private VacationInfoModel _vacationInfo;

        public VacationPagerAdapter() : base()
        {
            items.Add("VAC");
            items.Add("ILL");
            items.Add("OVT");
            items.Add("POV");
            items.Add("EXV");
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object obj) 
        {
           return view == obj;
        }           
      
        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            _container = container;
            View view = LayoutInflater.From(container.Context).Inflate(Resource.Layout.VacationDetail, container, false);
            var packageName = Application.Context.PackageName.ToString();
            view.Id = _container.Context.Resources.GetIdentifier("view_" + position, "id", packageName);
            container.AddView(view);
            TextView ItemType = view.FindViewById<TextView>(Resource.Id.ItemType);
            Button ItemStartDateBtn = view.FindViewById<Button>(Resource.Id.ItemStartDateBtn);
            Button ItemEndDateBtn = view.FindViewById<Button>(Resource.Id.ItemEndDateBtn);            
            Button ItemPickImageFromGallery = view.FindViewById<Button>(Resource.Id.ItemPickImageFromGallery);
            Button ItemPickImageFromCamera = view.FindViewById<Button>(Resource.Id.ItemPickImageFromCamera);
            ItemStartDateBtn.Click += delegate { ShowDialog(0); };
            ItemEndDateBtn.Click += delegate { ShowDialog(1); };
            ItemStartDateBtn.Text = DateTime.Now.ToLocalTime().ToString("d");
            ItemEndDateBtn.Text = DateTime.Now.ToLocalTime().ToString("d");
            ItemPickImageFromGallery.Click += onItemPickImageFromGalleryButtonClicked;
            ItemPickImageFromCamera.Click += onItemPickImageFromCameraButtonClicked;            
            int pos = position;
            ItemType.Text = items[pos];            
            view.FindViewById<TextView>(Resource.Id.ItemError).Text = string.Empty;
            FillData(view);
            return view;
        }

        private async Task  FillData(View view)
        {
            _vacationsViewModel = FactorySingleton.Factory.Get<VacationsViewModel>();
            _vacationInfo = await _vacationsViewModel.CreateDraftVacationInfo();
            view.FindViewById<TextView>(Resource.Id.ItemEmployee).Text = _vacationInfo.Employee.FullName;
            view.FindViewById<TextView>(Resource.Id.ItemApprover).Text = _vacationInfo.Approver.FullName;
            view.FindViewById<TextView>(Resource.Id.ItemDuration).Text = String.Format("{0} {1}", ConverterHelper.CalculateDuration(DateTime.Now.Date, DateTime.Now.Date), _vacationsViewModel.Localaizer.Localize("days"));
            view.FindViewById<TextView>(Resource.Id.ItemStatus).Text = _vacationInfo.Status.Value;            
            view.FindViewById<TextView>(Resource.Id.ItemEmployee).Typeface = FontLoader.GetFontNormal((Activity)_container.Context);
            view.FindViewById<TextView>(Resource.Id.ItemType).Typeface = FontLoader.GetFontNormal((Activity)_container.Context);
            view.FindViewById<TextView>(Resource.Id.ItemApprover).Typeface = FontLoader.GetFontNormal((Activity)_container.Context);
            view.FindViewById<Button>(Resource.Id.ItemStartDateBtn).Typeface = FontLoader.GetFontNormal((Activity)_container.Context);
            view.FindViewById<Button>(Resource.Id.ItemEndDateBtn).Typeface = FontLoader.GetFontNormal((Activity)_container.Context);
            view.FindViewById<TextView>(Resource.Id.ItemDuration).Typeface = FontLoader.GetFontNormal((Activity)_container.Context);
            view.FindViewById<TextView>(Resource.Id.ItemStatus).Typeface = FontLoader.GetFontNormal((Activity)_container.Context);
            view.FindViewById<TextView>(Resource.Id.ItemError).Typeface = FontLoader.GetFontNormal((Activity)_container.Context);
            view.FindViewById<Button>(Resource.Id.ItemPickImageFromGallery).Typeface = FontLoader.GetFontNormal((Activity)_container.Context);
            view.FindViewById<Button>(Resource.Id.ItemPickImageFromCamera).Typeface = FontLoader.GetFontNormal((Activity)_container.Context);
            view.FindViewById<TextView>(Resource.Id.LabelEmployee).Typeface = FontLoader.GetFontBold((Activity)_container.Context);
            view.FindViewById<TextView>(Resource.Id.LabelType).Typeface = FontLoader.GetFontBold((Activity)_container.Context);
            view.FindViewById<TextView>(Resource.Id.LabelApprover).Typeface = FontLoader.GetFontBold((Activity)_container.Context);
            view.FindViewById<TextView>(Resource.Id.LabelDuration).Typeface = FontLoader.GetFontBold((Activity)_container.Context);
            view.FindViewById<TextView>(Resource.Id.LabelStatus).Typeface = FontLoader.GetFontBold((Activity)_container.Context);
            view.FindViewById<TextView>(Resource.Id.LabelAttachment).Typeface = FontLoader.GetFontBold((Activity)_container.Context);            
        }

        public string GetHeaderTitle(int position)
        {
            return items[position];
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object obj)
        {
            container.RemoveView((View)obj);
        }

        private void ShowDialog(int id)
        {
            DatePickerDialog dialog;

            var packageName = Application.Context.PackageName.ToString();
            int position = VacationTabsFragment._viewPager.CurrentItem;
            int viewId = _container.Context.Resources.GetIdentifier("view_" + position, "id", packageName);

            View currentView = _container.FindViewById(viewId);

            if (id == 0)
            {
                Button ItemStartDateBtn = currentView.FindViewById<Button>(Resource.Id.ItemStartDateBtn);
                DateTime time = DateTime.Parse(ItemStartDateBtn.Text);

                dialog = new DatePickerDialog(_container.Context, HandleStartDateSet, time.Year, time.Month - 1, time.Day);
            }
            else {
                Button ItemEndDateBtn = currentView.FindViewById<Button>(Resource.Id.ItemEndDateBtn);
                DateTime time = DateTime.Parse(ItemEndDateBtn.Text);

                dialog = new DatePickerDialog(_container.Context, HandleEndDateSet, time.Year, time.Month - 1, time.Day);
            }
            try
            {
                dialog.DatePicker.MinDate = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }            
            dialog.Show();
        }

        private void HandleStartDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            var packageName = Application.Context.PackageName.ToString();
            int position = VacationTabsFragment._viewPager.CurrentItem;
            int viewId = _container.Context.Resources.GetIdentifier("view_" + position, "id", packageName);

            View currentView = _container.FindViewById(viewId);

            Button itemStartDateBtn = currentView.FindViewById<Button>(Resource.Id.ItemStartDateBtn);
            itemStartDateBtn.Text = e.Date.ToString("d");
            if (_vacationInfo != null)
            {
                _vacationInfo.StartDate = (long)(e.Date - new DateTime(1970, 1, 1)).TotalMilliseconds;
                UpdateDraft();
            }
            this.NotifyDataSetChanged();
        }

        private void HandleEndDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            var packageName = Application.Context.PackageName.ToString();
            int position = VacationTabsFragment._viewPager.CurrentItem;
            int viewId = _container.Context.Resources.GetIdentifier("view_" + position, "id", packageName);

            View currentView = _container.FindViewById(viewId);

            Button ItemEndDateBtn = currentView.FindViewById<Button>(Resource.Id.ItemEndDateBtn);
            ItemEndDateBtn.Text = e.Date.ToString("d");
            if (_vacationInfo != null)
            {
                _vacationInfo.EndDate = (long)(e.Date - new DateTime(1970, 1, 1)).TotalMilliseconds;
                UpdateDraft();
            }

            this.NotifyDataSetChanged();
        }

        private async void onItemPickImageFromGalleryButtonClicked(object sender, EventArgs e)
        {
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);

            ((Activity)_container.Context).StartActivityForResult(
                Intent.CreateChooser(imageIntent, "Select photo"), 0);
        }

        private async void onItemPickImageFromCameraButtonClicked(object sender, EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);

            var documentspath = Android.OS.Environment.DirectoryDocuments;

            string mediaType = Android.OS.Environment.DirectoryPictures;
            Java.IO.File mediaStorageDir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(mediaType).Path);
            var file = new Java.IO.File(mediaStorageDir.Path, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            _imageUriFromFile = Android.Net.Uri.FromFile(file);

            intent.PutExtra(MediaStore.ExtraOutput, _imageUriFromFile);
            ((Activity)_container.Context).StartActivityForResult(intent, CAMERA_CAPTURE_IMAGE_REQUEST_CODE);
        }

        public void ActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok)
            {
                var packageName = Application.Context.PackageName.ToString();
                int position = VacationTabsFragment._viewPager.CurrentItem;
                int viewId = _container.Context.Resources.GetIdentifier("view_" + position, "id", packageName);
                View currentView = _container.FindViewById(viewId);
                ImageView image = currentView.FindViewById<ImageView>(Resource.Id.ItemImageView);

                if (requestCode == CAMERA_CAPTURE_IMAGE_REQUEST_CODE)
                {
                    Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                    mediaScanIntent.SetData(_imageUriFromFile);
                    ContextWrapper wrapper = new ContextWrapper(_container.Context);
                    wrapper.SendBroadcast(mediaScanIntent);

                    var file = new Java.IO.File(mediaScanIntent.Data.EncodedPath);
                    Bitmap bitmap = BitmapFactory.DecodeFile(file.Path);

                    image.SetImageBitmap(bitmap);
                    _imageUri = _imageUriFromFile;
                    return;
                }
                else {
                    _imageUri = data.Data;
                    image.SetImageURI(data.Data);
                }
                if (_imageUri != null)
                {
                    _vacationsViewModel.Image = Utils.GetByteByURI(((Activity)_container.Context).ApplicationContext, _imageUri);
                }
            }
        }

        private async void UpdateDraft()
        {
            await _vacationsViewModel.UpdateDraftVacationInfo(_vacationInfo);
        }
        public Task ViewDispose()
        {
            var packageName = Application.Context.PackageName.ToString();

            for (int position = 0; position <= 4; position++)
            {
                int viewId = _container.Context.Resources.GetIdentifier("view_" + position, "id", packageName);
                View view = _container.FindViewById(viewId);

                if (view != null)
                {
                    Button ItemStartDateBtn = view.FindViewById<Button>(Resource.Id.ItemStartDateBtn);
                    Button ItemEndDateBtn = view.FindViewById<Button>(Resource.Id.ItemEndDateBtn);                    
                    Button ItemPickImageFromGallery = view.FindViewById<Button>(Resource.Id.ItemPickImageFromGallery);
                    Button ItemPickImageFromCamera = view.FindViewById<Button>(Resource.Id.ItemPickImageFromCamera);

                    ImageView image = view.FindViewById<ImageView>(Resource.Id.ItemImageView);

                    image.SetImageBitmap(null);
                    image.Dispose();
                    image = null;

                    ItemStartDateBtn.Click -= delegate {
                        ShowDialog(0);
                    };
                    ItemEndDateBtn.Click -= delegate {
                        ShowDialog(1);
                    };
                    ItemPickImageFromGallery.Click -= onItemPickImageFromGalleryButtonClicked;
                    ItemPickImageFromCamera.Click -= onItemPickImageFromCameraButtonClicked;                   
                }
            }
            return Helper.Complete();
        }
    }
}

