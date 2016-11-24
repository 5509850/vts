using System;
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Android.Support.V7.App;
using Android.Support.V4.Widget;


namespace VTS.Droid
{
    public class MainDrawerToggle : SupportActionBarDrawerToggle
    {
        private ActionBarActivity _hostActivity;
        private int _openedResource;
        private int _closedResource;

        public MainDrawerToggle(ActionBarActivity host, DrawerLayout drawerLayout, int openedResource, int closedResource)
            : base(host, drawerLayout, openedResource, closedResource)
        {
            _hostActivity = host;
            _openedResource = openedResource;
            _closedResource = closedResource;
        }

        public override void OnDrawerOpened(Android.Views.View drawerView)
        {
            int drawerType = (int)drawerView.Tag;

            if (drawerType == 0)
            {
                base.OnDrawerOpened(drawerView);
                _hostActivity.SupportActionBar.SetTitle(_openedResource);
            }
        }

        public override void OnDrawerClosed(Android.Views.View drawerView)
        {
            int drawerType = (int)drawerView.Tag;

            if (drawerType == 0)
            {
                base.OnDrawerClosed(drawerView);
                _hostActivity.SupportActionBar.SetTitle(_closedResource);
            }
        }

        public override void OnDrawerSlide(Android.Views.View drawerView, float slideOffset)
        {
            int drawerType = (int)drawerView.Tag;

            if (drawerType == 0)
            {
                base.OnDrawerSlide(drawerView, slideOffset);
            }
        }
    }
}

