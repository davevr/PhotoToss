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

using PhotoToss.Core;

namespace PhotoToss
{
	[Activity(ScreenOrientation=Android.Content.PM.ScreenOrientation.Portrait )]
    public class FirstRunActivity : Activity
    {
        private EditText usernameField;
        private EditText passwordField;
        private EditText confirmPassword;
        private EditText emailField;
        private Button signInBtn;
        private Button createAccountBtn;
        private TextView prepSignIn;
        private TextView emailPrompt;
        private ProgressDialog progressDlg;

        protected override void OnCreate(Bundle bundle)
        {
			RequestWindowFeature (WindowFeatures.NoTitle);
			Window.SetFlags (WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            base.OnCreate(bundle);

            SetContentView(Resource.Layout.SignInLayout);

            progressDlg = new ProgressDialog(this);
            progressDlg.SetProgressStyle(ProgressDialogStyle.Spinner);

            FindViewById<TextView>(Resource.Id.textView1).SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
            usernameField = FindViewById<EditText>(Resource.Id.usernameField);
            usernameField.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
            usernameField.AfterTextChanged += HandleTextValueChanged;

            passwordField = FindViewById<EditText>(Resource.Id.password);
            passwordField.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
            passwordField.AfterTextChanged += HandleTextValueChanged;

            confirmPassword = FindViewById<EditText>(Resource.Id.password2);
            confirmPassword.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
            confirmPassword.AfterTextChanged += HandleTextValueChanged;

            emailPrompt = FindViewById<TextView>(Resource.Id.emailPrompt);
            emailPrompt.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);

            emailField = FindViewById<EditText>(Resource.Id.emailAddrField);
            emailField.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);

            createAccountBtn = FindViewById<Button>(Resource.Id.createBtn);
            createAccountBtn.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);

            createAccountBtn.Click += (snder, e) =>
            {
                progressDlg.SetMessage("signing in...");
                progressDlg.Show();
                string userName = usernameField.Text.Trim();
                string password = passwordField.Text;
                signInBtn.Enabled = false;
                createAccountBtn.Enabled = false;


                // sign in
				PhotoTossRest.Instance.CreateAccount(userName, password, CreateAccountResultCallback);

            };

            prepSignIn = FindViewById<TextView>(Resource.Id.prepSignIn);
            prepSignIn.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);

            prepSignIn.Click += (object sender, EventArgs e) =>
            {
                PrepForSignIn();


            };

            signInBtn = FindViewById<Button>(Resource.Id.signInBtn);
            signInBtn.SetTypeface(MainActivity.bodyFace, TypefaceStyle.Normal);
            signInBtn.Visibility = ViewStates.Gone;

            signInBtn.Click += (object sender, EventArgs e) =>
            {
                progressDlg.SetMessage("signing in...");
                progressDlg.Show();
                string userName = usernameField.Text.Trim();
                string password = passwordField.Text;
                signInBtn.Enabled = false;
                createAccountBtn.Enabled = false;

                // sign in
				PhotoTossRest.Instance.Login(userName, password, SiginInResultCallback);
            };

            createAccountBtn.Enabled = false;
            signInBtn.Enabled = false;


        }

		protected override void OnStop ()
		{
			progressDlg.Dismiss ();
			base.OnStop ();
		}

        void HandleTextValueChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            string usernameText = usernameField.Text;
            string passwordText = passwordField.Text;
            string confirmText = confirmPassword.Text;
            string emailText = emailField.Text;

            if (String.IsNullOrEmpty(usernameText) || String.IsNullOrEmpty(passwordText) ||
                (usernameText.Length < 3) || (passwordText.Length < 3))
            {
                signInBtn.Enabled = false;
                createAccountBtn.Enabled = false;

            }
            else
            {
                signInBtn.Enabled = true;

                if (passwordText == confirmText)
                    createAccountBtn.Enabled = true;
                else
                    createAccountBtn.Enabled = false;
            }
        }

        private void SiginInResultCallback(User result)
        {

            if (result != null)
            {
                MainActivity.analytics.PostLogin();
                RunOnUiThread(() =>
                {
                    progressDlg.Hide();
                    FinishSignin();
                });
            }
            else
            {
                MainActivity.analytics.PostSessionError("signinfailed");

                DisplayAlert("Sign in Failed", "Unable to sign in.  Check username and password");
                RunOnUiThread(() =>
                {
                    progressDlg.Hide();
                    signInBtn.Enabled = true;
                    createAccountBtn.Enabled = true;
                    HandleTextValueChanged(null, null);
                });
            }

        }

        private void CreateAccountResultCallback(User result)
        {
            if (result == null)
            {
                MainActivity.analytics.PostRegisterUser();
                RunOnUiThread(() =>
	                {
	                    progressDlg.Hide();
	                   string emailAddress = emailField.Text.Trim();

	                    if (!String.IsNullOrEmpty(emailAddress))
	                    {
							PhotoTossRest.Instance.SetRecoveryEmail(emailAddress, (resultStr) =>
	                        {
								RunOnUiThread(() =>
		                            {
										FinishSignin();
		                            });
	                        });
	                        
	                    }
	                    else
	                    {
							FinishSignin();
	                    }
	                        
	                });
            }
            else
            {
                MainActivity.analytics.PostSessionError("registerfailed-");

                DisplayAlert("Create Account Failed", "Unable to create account.  Check username");
                RunOnUiThread(() =>
                {
                    progressDlg.Hide();
                    signInBtn.Enabled = true;
                    createAccountBtn.Enabled = true;
                    HandleTextValueChanged(null, null);
                });
            }
        }

        public void DisplayAlert(string titleString, string descString)
        {
            RunOnUiThread(() =>
            {
                AlertDialog alert = new AlertDialog.Builder(this).Create();
                alert.SetTitle(titleString);
                alert.SetMessage(descString);
                alert.SetButton("ok", (sender, args) =>
                {
                    alert.Dismiss();
                });
                alert.Show();
            });

        }

        void PrepForSignIn()
        {
            signInBtn.Visibility = ViewStates.Visible;
            confirmPassword.Visibility = ViewStates.Gone;
            emailPrompt.Visibility = ViewStates.Gone;
            emailField.Visibility = ViewStates.Gone;
            prepSignIn.Visibility = ViewStates.Gone;
            createAccountBtn.Visibility = ViewStates.Gone;
        }

		void FinishSignin()
		{
			SetResult (Result.Ok);
			Finish();
		}

		public override void OnBackPressed ()
		{
			//base.OnBackPressed ();
		}

    }
}