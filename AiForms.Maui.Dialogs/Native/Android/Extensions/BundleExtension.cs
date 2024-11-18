using System;
using Android.OS;
using Java.IO;

namespace AiForms.Dialogs.Droid;

public static class BundleExtension
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1422:プラットフォームの互換性を検証", Justification = "<保留中>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:プラットフォームの互換性を検証", Justification = "<保留中>")]
    public static T GetSerializable<T>(this Bundle bundle, string key) where T : class, ISerializable
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
        {
            return bundle.GetSerializable(key, Java.Lang.Class.FromType(typeof(T))) as T;
        }
        else
        {
            return bundle.GetSerializable(key) as T;
        }
    }
}

