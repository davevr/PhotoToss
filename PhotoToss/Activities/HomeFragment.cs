using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;

using ZXing;
using ZXing.Mobile;


namespace PhotoToss
{
    public class HomeFragment : Fragment
    {
        public MainActivity MainPage { get; set; }
        MobileBarcodeScanner scanner;
        ImageView previewImage;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.HomeFragment, container, false);

            var catchBtn = view.FindViewById<Button>(Resource.Id.catchButton);
            previewImage = view.FindViewById<ImageView>(Resource.Id.previewImage);
            Button flashButton;
            View zxingOverlay;
            scanner = new MobileBarcodeScanner(this.Activity);
            catchBtn.Click += async delegate
            {

                //Tell our scanner we want to use a custom overlay instead of the default
                scanner.UseCustomOverlay = true;

                //Inflate our custom overlay from a resource layout
                zxingOverlay = LayoutInflater.FromContext(this.Activity).Inflate(Resource.Layout.ZxingOverlay, null);

                //Find the button from our resource layout and wire up the click event
                flashButton = zxingOverlay.FindViewById<Button>(Resource.Id.buttonZxingFlash);
                flashButton.Click += (sender, e) => scanner.ToggleTorch();

                //Set our custom overlay
                
                scanner.CustomOverlay = zxingOverlay;

                //Start scanning!
                var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
                options.PossibleFormats = new List<ZXing.BarcodeFormat>() {  ZXing.BarcodeFormat.AZTEC };
                var result = await scanner.Scan(options);
                

                HandleScanResult(result);
            };

            var tossBtn = view.FindViewById<Button>(Resource.Id.tossButton);

            tossBtn.Click += delegate { 

                Activity.StartActivity(typeof(TossActivity)); 
            };


            return view;
        }

        void HandleScanResult(ZXing.Result result)
        {
            /*
             * 
             * Ok, here is some very simple solution to grab the frame

        At the Result.cs class, add properties to handle width, height and source image

        public Android.Graphics.YuvImage SourceYuv { get; set; }
        public int SourceWidth  { get; set; }
        public int SourceHeight  { get; set; }
        in ZXingSurfaceView.cs modify the OnPreviewFrame method to save the raw image

        ...at the beginning before the processing
        var img = new YuvImage(bytes, ImageFormatType.Nv21, cameraParameters.PreviewSize.Width, cameraParameters.PreviewSize.Height, null);
        ...result calculations...
        result.SourceWidth = width;
        result.SourceHeight = height;
        result.SourceYuv = img;
             * 
             * 
        later in our own code when we get the results we can easily save it to a file or do whatever we want

        string filenamefile = DateTime.Now.Ticks.ToString() + ".jpg";
        string filename = System.IO.Path.Combine(Values.DownloadsFolder(), filenamefile);
        Android.Graphics.Rect rect = new Android.Graphics.Rect(0, 0, result.SourceWidth, result.SourceHeight); 

        using (var os = new FileStream(filename, FileMode.CreateNew))
        {
                 result.SourceYuv.CompressToJpeg(rect, 100, os);
                 os.Flush();
                     os.Close();
         }
             * 
             */
            string msg = "";
            Bitmap updateImage = null;

            if (result != null && !string.IsNullOrEmpty(result.Text))
            {
                msg = "Found Barcode: " + result.Text;
                string imageId = result.Text.Substring(result.Text.Length - 24);
                string baseURL = "https://s3-us-west-2.amazonaws.com/blahguaimages/image/";
                string urlExt = "-D.jpg";
                string url = baseURL + imageId + urlExt;

                updateImage = GetImageBitmapFromUrl(url);
                    
            }
            else
                msg = "Scanning Canceled!";


            this.Activity.RunOnUiThread(() => 
                {
                    Toast.MakeText(this.Activity, msg, ToastLength.Short).Show();

                    if (updateImage != null)
                        previewImage.SetImageBitmap(updateImage);

                });
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }



        public void Refresh()
        {

        }
    }
}