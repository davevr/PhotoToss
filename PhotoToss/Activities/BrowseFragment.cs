using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace PhotoToss
{
    public class BrowseFragment : Android.Support.V4.App.Fragment
    {
        public MainActivity MainPage { get; set; }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.BrowseFragment, container, false);




            return view;
        }

        public void Refresh()
        {

        }
    }
}