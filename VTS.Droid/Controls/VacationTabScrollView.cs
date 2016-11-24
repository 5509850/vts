using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Android.Util;
using VTS.Droid.Adapters;

namespace VTS.Droid.Controls
{
    public class VacationTabScrollView : HorizontalScrollView
    {
        private const int TITLE_OFFSET_DIPS = 24;
        private const int TAB_VIEW_PADDING_DIPS = 16;
        private const int TAB_VIEW_TEXT_SIZE_SP = 12;
        private int _titleOffset;
        private int _tabViewLayoutID;
        private int _tabViewTextViewID;
        private ViewPager _viewPager;
        private ViewPager.IOnPageChangeListener _viewPagerPageChangeListener;

        private static VacationTabStrip _tabStrip;

        private int _scrollState;

        public interface TabColorizer
        {
            int GetIndicatorColor(int position);
            int GetDividerColor(int position);
        }

        public VacationTabScrollView(Context context) : this(context, null) { }

        public VacationTabScrollView(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }

        public VacationTabScrollView(Context context, IAttributeSet attrs, int defaultStyle) : base(context, attrs, defaultStyle)
        {
            //Disable the scroll bar
            HorizontalScrollBarEnabled = false;

            //Make sure the tab strips fill the view
            FillViewport = true;
            this.SetBackgroundColor(Android.Graphics.Color.Rgb(0xE5, 0xE5, 0xE5)); //Gray color

            _titleOffset = (int)(TITLE_OFFSET_DIPS * Resources.DisplayMetrics.Density);

            _tabStrip = new VacationTabStrip(context);
            this.AddView(_tabStrip, LayoutParams.MatchParent, LayoutParams.MatchParent);
        }

        public TabColorizer CustomTabColorizer
        {
            set { _tabStrip.CustomTabColorizer = value; }
        }

        public int[] SelectedIndicatorColor
        {
            set { _tabStrip.SelectedIndicatorColors = value; }
        }

        public int[] DividerColors
        {
            set { _tabStrip.DividerColors = value; }
        }

        public ViewPager.IOnPageChangeListener OnPageListener
        {
            set { _viewPagerPageChangeListener = value; }
        }

        public ViewPager ViewPager
        {
            set
            {
                _tabStrip.RemoveAllViews();

                _viewPager = value;
                if (value != null)
                {
                    value.PageSelected += value_PageSelected;
                    value.PageScrollStateChanged += value_PageScrollStateChanged;
                    value.PageScrolled += value_PageScrolled;
                    PopulateTabStrip();
                }
            }
        }

        private void value_PageScrolled(object sender, ViewPager.PageScrolledEventArgs e)
        {
            int tabCount = _tabStrip.ChildCount;

            if ((tabCount == 0) || (e.Position < 0) || (e.Position >= tabCount))
            {
                //if any of these conditions apply, return, no need to scroll
                return;
            }

            _tabStrip.OnViewPagerPageChanged(e.Position, e.PositionOffset);

            View selectedTitle = _tabStrip.GetChildAt(e.Position);

            int extraOffset = (selectedTitle != null ? (int)(e.Position * selectedTitle.Width) : 0);

            ScrollToTab(e.Position, extraOffset);

            if (_viewPagerPageChangeListener != null)
            {
                _viewPagerPageChangeListener.OnPageScrolled(e.Position, e.PositionOffset, e.PositionOffsetPixels);
            }
        }

        private void value_PageScrollStateChanged(object sender, ViewPager.PageScrollStateChangedEventArgs e)
        {
            _scrollState = e.State;

            if (_viewPagerPageChangeListener != null)
            {
                _viewPagerPageChangeListener.OnPageScrollStateChanged(e.State);
            }
        }

        private void value_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            if (_scrollState == ViewPager.ScrollStateIdle)
            {
                _tabStrip.OnViewPagerPageChanged(e.Position, 0f);
                ScrollToTab(e.Position, 0);

            }

            if (_viewPagerPageChangeListener != null)
            {
                _viewPagerPageChangeListener.OnPageSelected(e.Position);
            }
        }

        private void PopulateTabStrip()
        {
            PagerAdapter adapter = _viewPager.Adapter;

            for (int i = 0; i < adapter.Count; i++)
            {
                TextView tabView = CreateDefaultTabView(Context);
                tabView.Text = ((VacationPagerAdapter)adapter).GetHeaderTitle(i);
                tabView.SetTextColor(Android.Graphics.Color.Black);
                tabView.Tag = i;
                tabView.Click += tabView_Click;
                _tabStrip.AddView(tabView);
            }
        }

        private void tabView_Click(object sender, EventArgs e)
        {
            TextView clickTab = (TextView)sender;
            int pageToScrollTo = (int)clickTab.Tag;
            _viewPager.CurrentItem = pageToScrollTo;
        }

        private TextView CreateDefaultTabView(Android.Content.Context context)
        {
            TextView textView = new TextView(context)
            {
                LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1f)
            };

            textView.Gravity = GravityFlags.Center;
            textView.SetTextSize(ComplexUnitType.Sp, TAB_VIEW_TEXT_SIZE_SP);
            textView.Typeface = Android.Graphics.Typeface.DefaultBold;

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Honeycomb)
            {
                TypedValue outValue = new TypedValue();
                Context.Theme.ResolveAttribute(Android.Resource.Attribute.SelectableItemBackground, outValue, false);
                textView.SetBackgroundResource(outValue.ResourceId);
            }

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.IceCreamSandwich)
            {
                textView.SetAllCaps(true);
            }

            int padding = (int)(TAB_VIEW_PADDING_DIPS * Resources.DisplayMetrics.Density);
            textView.SetPadding(padding, padding, padding, padding);

            return textView;
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            if (_viewPager != null)
            {
                ScrollToTab(_viewPager.CurrentItem, 0);
            }
        }

        private void ScrollToTab(int tabIndex, int extraOffset)
        {
            int tabCount = _tabStrip.ChildCount;

            if (tabCount == 0 || tabIndex < 0 || tabIndex >= tabCount)
            {
                return;
            }

            View selectedChild = _tabStrip.GetChildAt(tabIndex);
            if (selectedChild != null)
            {
                int scrollAmountX = selectedChild.Left + extraOffset;

                if (tabIndex > 0 || extraOffset > 0)
                {
                    scrollAmountX -= _titleOffset;
                }

                this.ScrollTo(scrollAmountX, 0);
            }
        }

    }
}