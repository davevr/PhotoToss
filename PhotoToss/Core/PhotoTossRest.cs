using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using RestSharp;
using System.Runtime.Serialization;
using ServiceStack.Text;
using System.Threading.Tasks;

namespace PhotoToss.Core
{
    public delegate void PhotoRecordList_callback(List<PhotoRecord> theResult);
    public delegate void PhotoRecord_callback(PhotoRecord theResult);
    public delegate void User_callback(User theResult);
    public delegate void String_callback(String theResult);

    public class PhotoTossRest
    {
        private RestClient apiClient;
        private static PhotoTossRest _singleton = null;
        private string apiPath = "http://phototoss-server-01.appspot.com/api/";//"http://127.0.0.1:8080/api/"; //"http://phototoss-server-01.appspot.com/api/";//"http://www.photostore.com/api/";
        private Random rndBase = new Random();
        private string _uploadURL;

        public PhotoTossRest()
        {
            System.Console.WriteLine("Using Production Server");
            apiClient = new RestClient(apiPath);
            apiClient.CookieContainer = new CookieContainer();
        }

        public static PhotoTossRest Instance
        {
            get
            {
                if (_singleton == null)
                    _singleton = new PhotoTossRest();
                return _singleton;
            }
        }

        public void GetUserImages(PhotoRecordList_callback callback)
        {
            List<PhotoRecord> photoList = new List<PhotoRecord>();
            // to do

            if ((photoList == null) || (photoList.Count == 0))
            {
                int rndCount = 1;// rndBase.Next(100) + 15;
                for (int i = 0; i < rndCount; i++)
                {
                    PhotoRecord newRec = PhotoRecord.MakeSample();
                    //newRec.imageUrl = "http://lh5.ggpht.com/yizAvQIwFWLXFHxj8mTE0WF_WIL2q-yTwkplk2AzQ7wYU9sUfEATajem6T5TURImyL8KMJxvp252JiCExlvcUFSZWZn7k5uL";
                    newRec.caption = "she is sweet!";
                    photoList.Add(newRec);
                }
            }

            callback(photoList);
        }

        public void Login(string username, string password, User_callback callback)
        {
            // to do...
            callback(User.MakeSample());
        }

        public void GetUploadURL(String_callback callback)
        {
            string fullURL = "uploadURL";

            RestRequest request = new RestRequest(fullURL, Method.GET);

            apiClient.ExecuteAsync(request, (response) =>
            {
                _uploadURL = response.Content;
                callback(_uploadURL);
            });

        }

        public void UploadImage(Stream photoStream, string caption, string tags, PhotoRecord_callback callback)
        {
            RestClient onetimeClient = new RestClient();

            var request = new RestRequest(_uploadURL, Method.POST);
            request.AddHeader("Accept", "*/*");
            //request.AlwaysMultipartFormData = true;
            request.AddParameter("caption", caption);
            request.AddParameter("tags", tags);
            request.AddFile("file", ReadToEnd(photoStream), "file", "image/jpeg");

            onetimeClient.ExecuteAsync(request, (response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    PhotoRecord newRec = response.Content.FromJson<PhotoRecord>();
                    callback(newRec);
                }
                else
                {
                    //error ocured during upload
                    callback(null);
                }
            });
        }

        public byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = stream.Position;
            stream.Position = 0;

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

    }
}