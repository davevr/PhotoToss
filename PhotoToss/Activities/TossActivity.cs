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

using ZXing;
using ZXing.Rendering;


namespace PhotoToss
{
    [Activity(Label = "Toss a Photo")]
    public class TossActivity : Activity
    {
        Button tossBtn;
        ImageView imageView;



        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.TossFragment);

            tossBtn = FindViewById<Button>(Resource.Id.tossButton);
            imageView = FindViewById<ImageView>(Resource.Id.aztekView);

            tossBtn.Click += async delegate
            {
                BarcodeWriter writer = new BarcodeWriter();
                writer.Format = BarcodeFormat.AZTEC;
                writer.Renderer = new BitmapRenderer();

                var metrics = Resources.DisplayMetrics;
                var widthInDp = ConvertPixelsToDp(metrics.WidthPixels);
                var heightInDp = ConvertPixelsToDp(metrics.HeightPixels);
                string baseURL = "http://phototoss.com/share/";
                string guid = "545d0e4fe4b04019b5d10ee4";
                string url = baseURL + guid;

                writer.Options.Height = 240;
                writer.Options.Width = 240;
                writer.Options.Margin = 1;

                var bitMap = writer.Write(url);

                imageView.SetImageBitmap(bitMap);


            };
        }

    

        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }
    }
}