using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V4.View;
using VTS.Droid.Adapters;

namespace VTS.Droid.Controls.Fragments
{
    public class VacationTabsFragment : Fragment
    {
        private VacationTabScrollView _slidingTabScrollView;
        public static ViewPager _viewPager;

        public VacationPagerAdapter Adapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.VacationFragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            _slidingTabScrollView = view.FindViewById<VacationTabScrollView>(Resource.Id.sliding_tabs);
            _viewPager = view.FindViewById<ViewPager>(Resource.Id.viewpager);

            Adapter = new VacationPagerAdapter();
            _viewPager.Adapter = Adapter;

            _slidingTabScrollView.ViewPager = _viewPager;
        }
    }
}