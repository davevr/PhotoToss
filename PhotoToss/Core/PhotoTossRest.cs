﻿using System;
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
	public delegate void Toss_callback(TossRecord theResult);

    public class PhotoTossRest
    {
        private RestClient apiClient;
        private static PhotoTossRest _singleton = null;
		private string apiPath = "http://phototoss-server-01.appspot.com/api/";  //"http://localhost:8080/api/";  //"http://phototoss-server-01.appspot.com/api/";//"http://127.0.0.1:8080/api/"; //"http://phototoss-server-01.appspot.com/api/";//"http://www.photostore.com/api/";
        //private Random rndBase = new Random();
        private string _uploadURL;
		private string _catchURL;
		private User _currentUser = null;
		public PhotoRecord CurrentImage { get; set; }

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

		public User CurrentUser
		{
			get { return _currentUser; }
		}

        public void GetUserImages(PhotoRecordList_callback callback)
        {
			string fullURL = "images";

			RestRequest request = new RestRequest(fullURL, Method.GET);

			apiClient.ExecuteAsync<List<PhotoRecord>>(request, (response) =>
				{
					if (response == null)
						return;
					if (response.StatusCode == HttpStatusCode.OK)
					{
						List<PhotoRecord> imageList = response.Data;

					 	callback(imageList);
					}
					else
						callback(null);
				});
        }

        public void Login(string username, string password, User_callback callback)
        {
			string fullURL = "user/login";

			RestRequest request = new RestRequest(fullURL, Method.POST);
			request.AddParameter ("username", username);
			request.AddParameter ("password", password);

			apiClient.ExecuteAsync<User>(request, (response) =>
				{
					User newUser = response.Data;

					if (newUser != null)
					{
						_currentUser = newUser;
						Utilities.SafeSaveSetting(Utilities.USERNAME, username);
						Utilities.SafeSaveSetting(Utilities.PASSWORD, password);
						callback(newUser);
					}
					else
						callback(null);
				});
        }

		public void CreateAccount(string username, string password, User_callback callback)
		{
			string fullURL = "user/create";

			RestRequest request = new RestRequest(fullURL, Method.POST);
			request.AddParameter ("username", username);
			request.AddParameter ("password", password);

			apiClient.ExecuteAsync<User>(request, (response) =>
				{
					User newUser = response.Data;

					if (newUser != null)
					{
						_currentUser = newUser;
						Utilities.SafeSaveSetting(Utilities.USERNAME, username);
						Utilities.SafeSaveSetting(Utilities.PASSWORD, password);
						callback(newUser);
					}
					else
						callback(null);
				});
		}

		public void SetRecoveryEmail(string emailAddr, String_callback callback)
		{
			// to do...
			callback(emailAddr);
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

		public void GetCatchURL(String_callback callback)
		{
			string fullURL = "catchURL";

			RestRequest request = new RestRequest(fullURL, Method.GET);

			apiClient.ExecuteAsync(request, (response) =>
				{
					_catchURL = response.Content;
					callback(_catchURL);
				});

		}

	
			

		public void GetImage(String_callback callback)
		{
			string fullURL = "image";

			RestRequest request = new RestRequest(fullURL, Method.GET);

			apiClient.ExecuteAsync(request, (response) =>
				{
					_uploadURL = response.Content;
					callback(_uploadURL);
				});

		}


		public void StartToss(long imageId, int gameType, double longitude, double latitude, Toss_callback callback)
		{
			string fullURL = "toss";

			RestRequest request = new RestRequest(fullURL, Method.POST);
			request.AddParameter ("image", imageId);
			request.AddParameter ("game", gameType);
			request.AddParameter ("long", longitude);
			request.AddParameter ("lat", latitude);

			apiClient.ExecuteAsync<TossRecord>(request, (response) =>
				{
					callback(response.Data);
				});

		}

		public void CatchToss(Stream photoStream, long tossid, double longitude, double latitude, PhotoRecord_callback callback)
		{
			RestClient onetimeClient = new RestClient();

			var request = new RestRequest(_catchURL, Method.POST);
			request.AddHeader("Accept", "*/*");
			//request.AlwaysMultipartFormData = true;
			request.AddParameter("toss", tossid);
			request.AddParameter("long", longitude);
			request.AddParameter("lat", latitude);
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

		public void GetTossStatus(String_callback callback)
		{
			string fullURL = "toss/status";

			RestRequest request = new RestRequest(fullURL, Method.GET);

			apiClient.ExecuteAsync(request, (response) =>
				{
					_uploadURL = response.Content;
					callback(_uploadURL);
				});

		}



        public void UploadImage(Stream photoStream, string caption, string tags, double longitude, double latitude, PhotoRecord_callback callback)
        {
            RestClient onetimeClient = new RestClient();
			onetimeClient.CookieContainer = apiClient.CookieContainer;

            var request = new RestRequest(_uploadURL, Method.POST);
            request.AddHeader("Accept", "*/*");
            //request.AlwaysMultipartFormData = true;
            request.AddParameter("caption", caption);
            request.AddParameter("tags", tags);
			request.AddParameter("long", longitude);
			request.AddParameter("lat", latitude);
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