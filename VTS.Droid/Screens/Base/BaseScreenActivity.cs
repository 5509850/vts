using System;

using Android.App;
using Android.Content;
using Android.OS;

namespace VTS.Droid.Screens
{
    public class BaseScreenActivity : Activity
    {
        protected ProgressDialog _progressDialog = null;
        protected bool cancelProgress = false;
        protected void SetActivityTitle(string activityTitle)
        {            
            if (!string.IsNullOrEmpty(activityTitle))
            {
                Title = activityTitle;
            }
        }

        protected void AlertDialog(string message, EventHandler<DialogClickEventArgs> onPositiveAction)
        {
            AlertDialog.Builder builder;
            builder = new AlertDialog.Builder(this);
            builder.SetMessage(message);
            builder.SetCancelable(false);
            builder.SetPositiveButton(Resources.GetString(Resource.String.okButton), onPositiveAction);
            builder.Show();
        }

        protected void ShowProgress(int resId)
        {
            ShowProgress(GetText(resId));
        }

        protected void ShowProgressCancel(int resId)
        {
            ShowProgressCancel(GetText(resId));
        }

        protected void ShowProgressCancel(string text)
        {
            _progressDialog = new ProgressDialog(this);
            _progressDialog.SetMessage(text);
            _progressDialog.SetCancelable(true);
            _progressDialog.Indeterminate = true;
            _progressDialog.CancelEvent += (object sender, EventArgs e) => cancelProgress = true;
            _progressDialog.Show();
        }

        protected void ShowProgress(string text)
        {
            _progressDialog = new ProgressDialog(this);
            _progressDialog.SetMessage(text);
            _progressDialog.SetCancelable(false);
            _progressDialog.Indeterminate = true;
            _progressDialog.Show();
        }

        protected void HideProgress()
        {
            if (_progressDialog != null)
            {
                _progressDialog.Dismiss();
                _progressDialog = null;
            }
        }       
    }
}