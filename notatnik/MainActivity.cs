using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Xamarin.Essentials;
using notatnik.moduls;
using System.Security.Cryptography;

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
            IStringCipher stringCipher = new StringCipher();

            string salt = await SecureStorage.GetAsync("salt");
            string hash = encrypter.GetHash(password, salt);
            if (button.Pressed && await SecureStorage.GetAsync("hash") == hash && await SecureStorage.GetAsync("secret") != null)
            {
                SetContentView(Resource.Layout.activity_after_password);
                var secret = await SecureStorage.GetAsync("secret");
                string key = await SecureStorage.GetAsync("key");
                secret = stringCipher.Decrypt(secret, key);
                FindViewById<EditText>(Resource.Id.textView1).Text = secret;
            }
            else if(button.Pressed && await SecureStorage.GetAsync("hash") != hash)
            {
                SetContentView(Resource.Layout.activity_main);
                FindViewById<EditText>(Resource.Id.textView1).Hint = "Złe hasło";
            }
            else if(button.Pressed && await SecureStorage.GetAsync("hash") == hash && await SecureStorage.GetAsync("secret") == null)
            {
                SetContentView(Resource.Layout.activity_after_password);
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
            Button button = (Button)v;
            var password = FindViewById<EditText>(Resource.Id.textView1).Text;
            if (button.Pressed && password == string.Empty)
            {
                SetContentView(Resource.Layout.activity_main);
                FindViewById<EditText>(Resource.Id.textView1).Hint = "Hasło nie może być puste!";
            }
            else if(button.Pressed && SecureStorage.GetAsync("hash") == null)
            {
                SetContentView(Resource.Layout.activity_main);
                FindViewById<EditText>(Resource.Id.textView1).Hint = "Konto już istnieje";
            }
            else
            {
                string salt = encrypter.GetSalt(password);
                string hash = encrypter.GetHash(password, salt);
                await SecureStorage.SetAsync("hash", hash);
                await SecureStorage.SetAsync("salt", salt);
                SymmetricAlgorithm sym = new RijndaelManaged();
                sym.GenerateKey();
                string key = sym.Key.ToString();
                await SecureStorage.SetAsync("key", key);
                SetContentView(Resource.Layout.activity_main);
            }
        }
        [Java.Interop.Export("PowButton")]
        public void PowButton(View v)
        {
            SetContentView(Resource.Layout.activity_main);
        }
        [Java.Interop.Export("ZapButton")]
        async public void ZapButton(View v)
        {
            IStringCipher stringCipher = new StringCipher();
            string key = await SecureStorage.GetAsync("key");
            string secret = stringCipher.Encrypt(FindViewById<EditText>(Resource.Id.textView1).Text, key);
            await SecureStorage.SetAsync("secret", secret);
        }
    }
}