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

namespace notatnik.moduls
{
    public interface IEncrypter
    {
        string GetSalt(string values);
        string GetHash(string vaule, string salt);
    }
}