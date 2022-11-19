using System;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;

namespace AiForms.Dialogs.Droid;

public static class DrawableExtension
{
    public static void SetColorFilterEx(this Drawable drawable, Android.Graphics.Color color)
    {
        if(Build.VERSION.SdkInt >= BuildVersionCodes.Q)
        {
#pragma warning disable XA0001 // Android API の使用法に関する問題を見つける
            drawable.SetColorFilter(new BlendModeColorFilter(color, Android.Graphics.BlendMode.SrcIn));
#pragma warning restore XA0001 // Android API の使用法に関する問題を見つける
        }
        else
        {
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
            drawable.SetColorFilter(color, PorterDuff.Mode.SrcIn);
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
        }
    }
}

