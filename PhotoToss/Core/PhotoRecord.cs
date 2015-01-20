using System;
using System.Collections.Generic;


namespace PhotoToss.Core
{
    public class PhotoRecord
    {
        public string id {get; set;}
        public string caption { get; set; }
        public long totalshares { get; set; }
        public string ownername { get; set; }
        public string ownerid { get; set; }
        public List<string> tags { get; set; }
        public string sharedfromname { get; set; }
        public string sharedfromid { get; set; }
        public int myshares { get; set; }
        public int mysharesessions { get; set; }
        public DateTime received { get; set; }
        public DateTime created { get; set; }
        public DateTime lastsharedbyuser { get; set; }
        public DateTime lastshared { get; set; }
        public double createdlat { get; set; }
        public double receivedlong { get; set; }
        public double createdlong { get; set; }
        public double receivedlat { get; set; }
        public string receivedcaption { get; set; }
        public string receivedtags { get; set; }
        public string imageUrl { get; set; }
        public string catchUrl { get; set; }

        private object cachedImage = null;
        private object cachedCatchImage = null;


        public static PhotoRecord MakeSample()
        {
            PhotoRecord newRec = new PhotoRecord();
            newRec.id = "0";
            newRec.caption = "some image";
            newRec.totalshares = 1000;
            newRec.ownerid = "0";
            newRec.ownername = "davevr";
            newRec.tags = new List<string>() { "sheep", "nose", "fred" };
            newRec.sharedfromid = "0";
            newRec.sharedfromname = "davevr";
            newRec.mysharesessions = 15;
            newRec.myshares = 89;
            newRec.received = DateTime.Now;
            newRec.created = DateTime.Now;
            newRec.lastshared = DateTime.Now;
            newRec.lastsharedbyuser = DateTime.Now;
            newRec.createdlat = 34.0824;
            newRec.createdlong = -118.3941;
            newRec.receivedlat = 34.0824;
            newRec.receivedlong = -118.3941;
            newRec.receivedcaption = "cool share";
            newRec.imageUrl = "http://lh6.ggpht.com/mmXPSLXJbkXflBYX525inqAmT93u409QyD9KJgkvEyvPhCNxwbiZIhDG-KTTVvP39Z0G88AmHcLk50S81wHy6us7x3a7JFQo9A";
            newRec.catchUrl = "http://lh6.ggpht.com/mmXPSLXJbkXflBYX525inqAmT93u409QyD9KJgkvEyvPhCNxwbiZIhDG-KTTVvP39Z0G88AmHcLk50S81wHy6us7x3a7JFQo9A";
            

            return newRec;

        }

        public object CachedImage
        {
            get { return cachedImage; }
            set { cachedImage = value; }
        }

        public object CachedCatchImage
        {
            get { return cachedCatchImage; }
            set { cachedCatchImage = value; }
        }

    }

   

}