using System;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;

namespace AiForms.Dialogs.Droid;

public static class DrawableExtension
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:プラットフォームの互換性を検証", Justification = "<保留中>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1422:プラットフォームの互換性を検証", Justification = "<保留中>")]
    public static void SetColorFilterEx(this Drawable drawable, Android.Graphics.Color color)
    {       
        if(Build.VERSION.SdkInt >= BuildVersionCodes.Q)
        {
            drawable.SetColorFilter(new BlendModeColorFilter(color, Android.Graphics.BlendMode.SrcIn));
        }
        else
        {
            drawable.SetColorFilter(color, PorterDuff.Mode.SrcIn);
        }
    }
}

