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



using PhotoToss.Core;

namespace PhotoToss
{
    public class HomeFragment : Fragment
    {
        public MainActivity MainPage { get; set; }
        GridView imageGrid;
        public List<PhotoRecord> PhotoList { get; set; }

        public event Action PulledToRefresh;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.HomeFragment, container, false);

            imageGrid = view.FindViewById<GridView>(Resource.Id.imagesView);
            imageGrid.Visibility = ViewStates.Invisible;
            imageGrid.Adapter = new PhotoRecordAdapter(this.Activity, this);
            imageGrid.NumColumns = 2;
            imageGrid.StretchMode = StretchMode.StretchColumnWidth;
            imageGrid.ItemClick += imageGrid_ItemClick;

           
           

            Refresh();
            return view;
        }

        void imageGrid_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            this.Activity.StartActivity(typeof(TossActivity));
        }

        

        



        public void Refresh()
        {
            PhotoTossRest.Instance.GetUserImages((userImageList) =>
                {
                    this.PhotoList = userImageList;
                    if (PhotoList.Count == 0)
                    {
                        imageGrid.Visibility = ViewStates.Invisible;
                    }
                    else
                    {
                        imageGrid.Visibility = ViewStates.Visible;
                        imageGrid.InvalidateViews();
                        imageGrid.SmoothScrollToPosition(0);
                    }

                });
        }

        public void AddImage(PhotoRecord newRec)
        {
            PhotoList.Add(newRec);
            this.Activity.RunOnUiThread(() =>
                {
                    imageGrid.Visibility = ViewStates.Visible;
                    imageGrid.InvalidateViews();
                    imageGrid.SmoothScrollToPosition(0);
                });
        }

        public class PhotoRecordAdapter : BaseAdapter
        {
            private readonly Context context;
            private readonly HomeFragment home;
            private int itemWidth = 256;

            public PhotoRecordAdapter(Context c, HomeFragment theFragment)
            {
                context = c;
                var metrics = context.Resources.DisplayMetrics;
                int screenWidth = metrics.WidthPixels;
                float margin = 0f;
                float marginPixels = margin * context.Resources.DisplayMetrics.Density;
                itemWidth = (int)(((float)screenWidth - (marginPixels * 3f)) / 2f);


                home = theFragment;
            }

            public override int Count
            {
                get 
                {
                    if ((home != null) && (home.PhotoList != null))
                        return home.PhotoList.Count; 
                    else
                        return 0;
                }
            }

            public override Java.Lang.Object GetItem(int position)
            {
                return null;
            }

            public override long GetItemId(int position)
            {
                return 0;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                ImageView imageView;
                TextView captionText;
                View curView;
                PhotoRecord curRec = home.PhotoList[position];


                if (true)//convertView == null)
                {
                    curView = home.Activity.LayoutInflater.Inflate(Resource.Layout.photoGridCell,null);

                    //curView.LayoutParameters = new ViewGroup.LayoutParams(itemWidth, itemWidth); 
                  

                    //imageView.SetPadding(8, 8, 8, 8);
                }
                else
                {
                    curView = convertView;
                }
                imageView = curView.FindViewById<ImageView>(Resource.Id.imageView);
                captionText = curView.FindViewById<TextView>(Resource.Id.captionText);
                imageView.SetScaleType(ImageView.ScaleType.CenterCrop);

                captionText.Text = curRec.caption;

                Bitmap imageBitmap = (Bitmap)curRec.CachedImage;
                if (imageBitmap == null)
                {
                    imageBitmap = GetImageBitmapFromUrl(curRec.imageUrl + "=s" + itemWidth.ToString());
                    home.PhotoList[position].CachedImage = imageBitmap;
                }
                imageView.SetImageBitmap(imageBitmap);

                return curView;
            }

            private Bitmap GetImageBitmapFromUrl(string url)
            {
                Bitmap imageBitmap = null;

                try 
                {
                    using (var webClient = new WebClient())
                    {
                        var imageBytes = webClient.DownloadData(url);
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                        }
                    }
                
                }
                catch (Exception)
                { }
               

                return imageBitmap;
            }

            
        }
    }

}