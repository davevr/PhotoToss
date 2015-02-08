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
using Android.Support.V4.App;
using Android.Graphics;
using Android.Support.V4.View;

using PhotoToss.Core;

namespace PhotoToss
{
	[Activity(ScreenOrientation=Android.Content.PM.ScreenOrientation.Portrait )]
    public class FirstRunActivity : FragmentActivity
    {
        private NonSwipeViewPager mPager;
        private PagerAdapter mPagerAdapter;

        protected override void OnCreate(Bundle bundle)
        {
			RequestWindowFeature (WindowFeatures.NoTitle);
			Window.SetFlags (WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ViewPager);
            mPager = FindViewById<NonSwipeViewPager>(Resource.Id.pager);
            mPagerAdapter = new ScreenSlidePageAdapter(SupportFragmentManager);
            mPager.Adapter = mPagerAdapter;
            mPager.TouchEnabled = false;
        }

        public void FinishSignin()
        {
            SetResult(Result.Ok);
            Finish();
        }

        public void FinishCreateAccount()
        {
            GoToNext();
        }

        public override void OnBackPressed()
        {
            if (mPager.CurrentItem == 0)
            {
                //base.OnBackPressed();
            }
            else
            {
                mPager.CurrentItem--;
            }
        }

        public void GoToNext()
        {
            mPager.CurrentItem++;
        }

        private class ScreenSlidePageAdapter : FragmentStatePagerAdapter
        {
            private SignInFragment page1 = null;
            private ProfileFragment page2 = null;

            public ScreenSlidePageAdapter(Android.Support.V4.App.FragmentManager mgr)
                : base(mgr)
            {
                // do nothing
            }

            public override int Count
            {
                get { return 2; }
            }

            public override Android.Support.V4.App.Fragment GetItem(int position)
            {
                Android.Support.V4.App.Fragment newFragment = null;

                switch (position)
                {
                    case 0:
                        // first page
                        if (page1 == null)
                        {
                            page1 = new SignInFragment();
                        }

                        newFragment = page1;

                        break;

                    case 1:
                        if (page2 == null)
                        {
                            page2 = new ProfileFragment();
                            page2.IsInitialSignIn = true;
                        }
                        newFragment = page2;
                        break;

                  
                }

                return newFragment;
            }
        }
    }
}