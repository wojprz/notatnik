using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Xamarin.Essentials;
using notatnik.moduls;

namespace notatnik
{
   
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
        }
        [Java.Interop.Export("ZalButton")]
        async public void ZalButton(View v)
        {
            Button button = (Button)v;
            var password = FindViewById<EditText>(Resource.Id.textView1).Text;
            IEncrypter encrypter = new Encrypter();

            string salt = await SecureStorage.GetAsync("salt");
            string hash = encrypter.GetHash(password, salt);
            if (button.Pressed && await SecureStorage.GetAsync("hash") == hash)
            {
                    SetContentView(Resource.Layout.activity_after_password);
            }
            else
            {
                FindViewById<EditText>(Resource.Id.textView1).Hint = "Złe hasło";
            }
        }

        [Java.Interop.Export("ZmiButton")]
        async public void ZmiButton(View v)
        {
            IEncrypter encrypter = new Encrypter();

            var password = FindViewById<EditText>(Resource.Id.textInputEditText1).Text;
            string salt = encrypter.GetSalt(password);
            string hash = encrypter.GetHash(password, salt);
            await SecureStorage.SetAsync("hash", hash);
            await SecureStorage.SetAsync("salt", salt);
            SetContentView(Resource.Layout.activity_after_password);
        }

        [Java.Interop.Export("StwButton")]
        async public void StwButton(View v)
        {
            
            IEncrypter encrypter = new Encrypter();
            var password = FindViewById<EditText>(Resource.Id.textInputEditText2).Text;
            if (password == string.Empty)
            {
                SetContentView(Resource.Layout.activity_main);
                FindViewById<EditText>(Resource.Id.textInputEditText2).Hint = "Puste";
            }
            else if(SecureStorage.GetAsync("hash") == null)
            {
                SetContentView(Resource.Layout.activity_main);
                FindViewById<EditText>(Resource.Id.textInputEditText2).Hint = "Istnieje";
            }
            else
            {
                string salt = encrypter.GetSalt(password);
                string hash = encrypter.GetHash(password, salt);
                await SecureStorage.SetAsync("hash", hash);
                await SecureStorage.SetAsync("salt", salt);
                SetContentView(Resource.Layout.activity_main);
            }
        }
        [Java.Interop.Export("PowButton")]
        public void PowButton(View v)
        {
            SetContentView(Resource.Layout.activity_main);
        }
    }
}