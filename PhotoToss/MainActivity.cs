using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Android.Support.V7.Widget;
using Android.Support.V7.View;
using Android.Support.V7.AppCompat;
using Android.Support.V7.App;
using Android.Support.V4.Widget;


namespace PhotoToss
{
    [Activity(Label = "PhotoToss", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/Theme.AppCompat.Light")]
    public class MainActivity : Android.Support.V7.App.ActionBarActivity
    {
        private String[] mDrawerTitles = new string[] { "PhotoToss", "Browse", "Leaderboards", "Profile"};
        private DrawerLayout mDrawerLayout;
        private ListView mDrawerList;
        private string mDrawerTitle;
        private MyDrawerToggle mDrawerToggle;

        private HomeFragment homePage;
        private BrowseFragment browsePage;
        private StatsFragment statsPage;
        private ProfileFragment profilePage;

        class MyDrawerToggle : Android.Support.V7.App.ActionBarDrawerToggle
        {
            private string openString, closeString;
            private MainActivity baseActivity;

            public MyDrawerToggle(Activity activity, DrawerLayout drawerLayout, int openDrawerContentDescRes, int closeDrawerContentDescRes) :
                base(activity, drawerLayout, openDrawerContentDescRes, closeDrawerContentDescRes)
            {
                baseActivity = (MainActivity)activity;
                openString = baseActivity.Resources.GetString(openDrawerContentDescRes);
                closeString = baseActivity.Resources.GetString(closeDrawerContentDescRes);
            }
            public override void OnDrawerOpened(View drawerView)
            {
                base.OnDrawerOpened(drawerView);
                //baseActivity.Title = openString;


            }

            public override void OnDrawerClosed(View drawerView)
            {
                base.OnDrawerClosed(drawerView);
                //baseActivity.Title = closeString;
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // set up drawer
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mDrawerList = FindViewById<ListView>(Resource.Id.left_drawer);
            // Set the adapter for the list view
            mDrawerList.Adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItemChecked, mDrawerTitles);
            // Set the list's click listener
            mDrawerList.ItemClick += mDrawerList_ItemClick;

            mDrawerToggle = new MyDrawerToggle(this, mDrawerLayout, Resource.String.drawer_open, Resource.String.drawer_close);


            mDrawerLayout.SetDrawerListener(mDrawerToggle);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            
            selectItem(0);




        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            mDrawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            mDrawerToggle.OnConfigurationChanged(newConfig);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (mDrawerToggle.OnOptionsItemSelected(item))
            {
                return true;
            }

            // Handle your other action bar items...

            return base.OnOptionsItemSelected(item);
        }

        void mDrawerList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            selectItem(e.Position);
        }

        private Android.App.Fragment oldPage = null;

        private void selectItem(int position)
        {
            Android.App.Fragment newPage = null;
            var fragmentManager = this.FragmentManager;
            var ft = fragmentManager.BeginTransaction();
            bool firstTime = false;
            string pageName = "";

            switch (position)
            {
                case 0:
                    if (homePage == null)
                    {
                        homePage = new HomeFragment();
                        homePage.MainPage = this;
                        firstTime = true;
                    }
                    newPage = homePage;
                    pageName = "PhotoToss";
                    break;
                case 1:
                    if (browsePage == null)
                    {
                        browsePage = new BrowseFragment();
                        browsePage.MainPage = this;
                        firstTime = true;
                    }
                    newPage = browsePage;
                    break;
                case 2:
                    if (statsPage == null)
                    {
                        statsPage = new StatsFragment();
                        statsPage.MainPage = this;
                        firstTime = true;
                    }
                    newPage = statsPage;
                    break;
                case 3:
                    if (profilePage == null)
                    {
                        profilePage = new ProfileFragment();
                        profilePage.MainPage = this;
                        firstTime = true;
                    }
                    newPage = profilePage;
                    break;
            }

            if (oldPage != newPage)
            {
                if (oldPage != null)
                {
                    // to do - deactivate it
                    ft.Hide(oldPage);

                }

                oldPage = newPage;

                if (newPage != null)
                {
                    if (firstTime)
                        ft.Add(Resource.Id.fragmentContainer, newPage);
                    else
                        ft.Show(newPage);
                }

                ft.Commit();

                // update selected item title, then close the drawer
                if (!String.IsNullOrEmpty(pageName))
                    Title = pageName;
                else
                    Title = mDrawerTitles[position];

                mDrawerList.SetItemChecked(position, true);
                mDrawerLayout.CloseDrawer(mDrawerList);
            }
        }

        protected override void OnTitleChanged(Java.Lang.ICharSequence title, Android.Graphics.Color color)
        {
            //base.OnTitleChanged (title, color);
            this.SupportActionBar.Title = title.ToString();
        }


    }
}

