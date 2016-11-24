using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using VTS.Core.Data.Models;
using VTS.Droid.Helpers;

namespace VTS.Droid.Adapters
{
    public class VTSListAdapter : BaseAdapter<VTSModel>
    {
        Activity _context;
        List<VTSModel> _list;

        public VTSListAdapter(Activity context, List<VTSModel> list)
            : base()
        {
            this._context = context;
            this._list = list;
        }

        public override int Count
        {
            get { return _list.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override VTSModel this[int index]
        {
            get { return _list[index]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
                view = _context.LayoutInflater.Inflate(Resource.Layout.VTSListItem, parent, false);
            }
            VTSModel item = this[position];
            view.FindViewById<TextView>(Resource.Id.VacationType).Text = item.VacationType;
            view.FindViewById<TextView>(Resource.Id.VacationType).Typeface = FontLoader.GetFontBold(_context);
            view.FindViewById<TextView>(Resource.Id.Date).Text = item.Date;
            view.FindViewById<TextView>(Resource.Id.Date).Typeface = FontLoader.GetFontNormal(_context);
            var imageStatus = view.FindViewById<ImageView>(Resource.Id.Status);
           
            if (item.Status.Equals(VacationStatus.Approved.ToString()))
            {
                imageStatus.SetImageResource(Resource.Drawable.greencircle);
            }
            else {
                    if (item.Status.Equals(VacationStatus.Rejected.ToString()))
                    {
                        imageStatus.SetImageResource(Resource.Drawable.redcircle);
                    }
                    else
                        {
                            imageStatus.SetImageResource(Resource.Drawable.greycircle);
                        }                    
                 }
            return view;
        }
    }
}

