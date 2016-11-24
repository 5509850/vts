using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Util;

namespace VTS.Droid.Controls
{
    public class VacationTabStrip : LinearLayout
    {
        //Copy and paste from here................................................................
        private const int DEFAULT_BOTTOM_BORDER_THICKNESS_DIPS = 3;
        private const byte DEFAULT_BOTTOM_BORDER_COLOR_ALPHA = 0X26;
        private const int SELECTED_INDICATOR_THICKNESS_DIPS = 8;
        private int[] INDICATOR_COLORS = { 0x4CD2EE, 0x187EDF };
        private int[] DIVIDER_COLORS = { 0xC5C5C5 };

        private const int DEFAULT_DIVIDER_THICKNESS_DIPS = 1;
        private const float DEFAULT_DIVIDER_HEIGHT = 0.5f;

        //Bottom border
        private int _bottomBorderThickness;
        private Paint _bottomBorderPaint;
        private int _defaultBottomBorderColor;

        //Indicator
        private int _selectedIndicatorThickness;
        private Paint _selectedIndicatorPaint;

        //Divider
        private Paint _dividerPaint;
        private float _dividerHeight;

        //Selected position and offset
        private int _selectedPosition;
        private float _selectionOffset;

        //Tab colorizer
        private VacationTabScrollView.TabColorizer _customTabColorizer;
        private SimpleTabColorizer _defaultTabColorizer;
        //Stop copy and paste here........................................................................

        //Constructors
        public VacationTabStrip(Context context) : this(context, null)
        { }

        public VacationTabStrip(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SetWillNotDraw(false);

            float density = Resources.DisplayMetrics.Density;

            TypedValue outValue = new TypedValue();
            context.Theme.ResolveAttribute(Android.Resource.Attribute.ColorForeground, outValue, true);
            int themeForeGround = outValue.Data;
            _defaultBottomBorderColor = SetColorAlpha(themeForeGround, DEFAULT_BOTTOM_BORDER_COLOR_ALPHA);

            _defaultTabColorizer = new SimpleTabColorizer();
            _defaultTabColorizer.IndicatorColors = INDICATOR_COLORS;
            _defaultTabColorizer.DividerColors = DIVIDER_COLORS;

            _bottomBorderThickness = (int)(DEFAULT_BOTTOM_BORDER_THICKNESS_DIPS * density);
            _bottomBorderPaint = new Paint();
            _bottomBorderPaint.Color = GetColorFromInteger(0xC5C5C5); //Gray

            _selectedIndicatorThickness = (int)(SELECTED_INDICATOR_THICKNESS_DIPS * density);
            _selectedIndicatorPaint = new Paint();

            _dividerHeight = DEFAULT_DIVIDER_HEIGHT;
            _dividerPaint = new Paint();
            _dividerPaint.StrokeWidth = (int)(DEFAULT_DIVIDER_THICKNESS_DIPS * density);
        }

        public VacationTabScrollView.TabColorizer CustomTabColorizer
        {
            set
            {
                _customTabColorizer = value;
                this.Invalidate();
            }
        }

        public int[] SelectedIndicatorColors
        {
            set
            {
                _customTabColorizer = null;
                _defaultTabColorizer.IndicatorColors = value;
                this.Invalidate();
            }
        }

        public int[] DividerColors
        {
            set
            {
                _defaultTabColorizer = null;
                _defaultTabColorizer.DividerColors = value;
                this.Invalidate();
            }
        }

        private Color GetColorFromInteger(int color)
        {
            return Color.Rgb(Color.GetRedComponent(color), Color.GetGreenComponent(color), Color.GetBlueComponent(color));
        }

        private int SetColorAlpha(int color, byte alpha)
        {
            return Color.Argb(alpha, Color.GetRedComponent(color), Color.GetGreenComponent(color), Color.GetBlueComponent(color));
        }

        public void OnViewPagerPageChanged(int position, float positionOffset)
        {
            _selectedPosition = position;
            _selectionOffset = positionOffset;
            this.Invalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            int height = Height;
            int tabCount = ChildCount;
            int dividerHeightPx = (int)(Math.Min(Math.Max(0f, _dividerHeight), 1f) * height);
            VacationTabScrollView.TabColorizer tabColorizer = _customTabColorizer != null ? _customTabColorizer : _defaultTabColorizer;

            //Thick colored underline below the current selection
            if (tabCount > 0)
            {
                View selectedTitle = GetChildAt(_selectedPosition);
                int left = selectedTitle.Left;
                int right = selectedTitle.Right;
                int color = tabColorizer.GetIndicatorColor(_selectedPosition);

                if (_selectionOffset > 0f && _selectedPosition < (tabCount - 1))
                {
                    int nextColor = tabColorizer.GetIndicatorColor(_selectedPosition + 1);
                    if (color != nextColor)
                    {
                        color = blendColor(nextColor, color, _selectionOffset);
                    }

                    View nextTitle = GetChildAt(_selectedPosition + 1);
                    left = (int)(_selectionOffset * nextTitle.Left + (1.0f - _selectionOffset) * left);
                    right = (int)(_selectionOffset * nextTitle.Right + (1.0f - _selectionOffset) * right);
                }

                _selectedIndicatorPaint.Color = GetColorFromInteger(color);
                canvas.DrawRect(left, height - _selectedIndicatorThickness, right, height, _selectedIndicatorPaint);

                //Creat vertical dividers between tabs
                int separatorTop = (height - dividerHeightPx) / 2;
                for (int i = 0; i < ChildCount; i++)
                {
                    View child = GetChildAt(i);
                    _dividerPaint.Color = GetColorFromInteger(tabColorizer.GetDividerColor(i));
                    canvas.DrawLine(child.Right, separatorTop, child.Right, separatorTop + dividerHeightPx, _dividerPaint);
                }

                canvas.DrawRect(0, height - _bottomBorderThickness, Width, height, _bottomBorderPaint);
            }
        }

        private int blendColor(int color1, int color2, float ratio)
        {
            float inverseRatio = 1f - ratio;
            float r = (Color.GetRedComponent(color1) * ratio) + (Color.GetRedComponent(color2) * inverseRatio);
            float g = (Color.GetGreenComponent(color1) * ratio) + (Color.GetGreenComponent(color2) * inverseRatio);
            float b = (Color.GetBlueComponent(color1) * ratio) + (Color.GetBlueComponent(color2) * inverseRatio);

            return Color.Rgb((int)r, (int)g, (int)b);
        }

        private class SimpleTabColorizer : VacationTabScrollView.TabColorizer
        {
            private int[] mIndicatorColors;
            private int[] mDividerColors;

            public int GetIndicatorColor(int position)
            {
                return mIndicatorColors[position % mIndicatorColors.Length];
            }

            public int GetDividerColor(int position)
            {
                return mDividerColors[position % mDividerColors.Length];
            }

            public int[] IndicatorColors
            {
                set { mIndicatorColors = value; }
            }

            public int[] DividerColors
            {
                set { mDividerColors = value; }
            }
        }
    }
}