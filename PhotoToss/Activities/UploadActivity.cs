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
using Android.Net;

using PhotoToss.Core;

using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;

namespace PhotoToss
{
    [Activity(Label = "Upload to PhotoToss", Icon = "@drawable/iconnoborder", Theme = "@style/Theme.AppCompat.Light", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class UploadActivity : Activity
    {
        private ImageView imageView;
        private EditText captionText;
        private TextView tagField;
        private EditText newTagText;
        private Button addTagBtn;
        private Button uploadBtn;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.UploadActivity);

            imageView = FindViewById<ImageView>(Resource.Id.imageView);
            captionText = FindViewById<EditText>(Resource.Id.captionText);
            tagField = FindViewById<TextView>(Resource.Id.tagsField);
            newTagText = FindViewById<EditText>(Resource.Id.NewTagText);
            addTagBtn = FindViewById<Button>(Resource.Id.AddTagBtn);
            uploadBtn = FindViewById<Button>(Resource.Id.UploadBtn);

            imageView.SetImageURI(Uri.FromFile(MainActivity._file));


            uploadBtn.Click += uploadBtn_Click;

        }

        void uploadBtn_Click(object sender, EventArgs e)
        {
            System.IO.Stream photoStream = System.IO.File.OpenRead(MainActivity._file.AbsolutePath);
            string caption = captionText.Text;
            string tags = tagField.Text;

            if (photoStream != null)
            {
                //System.IO.Stream fileStream = System.IO.File.OpenRead(imgPath);
                PhotoTossRest.Instance.UploadImage(photoStream, caption, tags, (newRec) =>
                    {
                        MainActivity._uploadPhotoRecord = newRec;
                        Finish();
                    });
            }
        }
    }
}