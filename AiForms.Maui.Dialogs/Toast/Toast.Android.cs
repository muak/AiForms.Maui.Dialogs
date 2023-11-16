﻿using System.Threading.Tasks;
using AiForms.Dialogs;
using AiForms.Dialogs.Droid;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using Math = System.Math;

namespace AiForms.Dialogs;

public partial class Toast
{

    public void Show<TView>(object viewModel = null) where TView : ToastView
    {
        var view = ExtraView.InstanceCreator<TView>.Create();
        Show(view, viewModel);
    }

    public void Show(ToastView view, object viewModel = null)
    {
        view.Parent = Application.Current.MainPage;
        if (viewModel != null)
        {
            view.BindingContext = viewModel;
        }

        var toast = new Android.Widget.Toast(DialogHelpers.Context);

        var offsetX = (int)DialogHelpers.Context.ToPixels(view.OffsetX);
        var offsetY = (int)DialogHelpers.Context.ToPixels(view.OffsetY);

        // HACK: For some reason, the offset direction is reversed when GravityFlags contains Left or Bottom.
        if (view.HorizontalLayoutAlignment == LayoutAlignment.End)
        {
            offsetX *= -1;
        }
        if (view.VerticalLayoutAlignment == LayoutAlignment.End)
        {
            offsetY *= -1;
        }

        toast.SetGravity(DialogHelpers.GetGravity(view), offsetX, offsetY);
        toast.Duration = Android.Widget.ToastLength.Long;

        var viewHandler = DialogHelpers.CreateNewHandler(view);

        var measure = DialogHelpers.Measure(view);
        view.Layout(new Rect(new Point(0, 0), measure));

        var realW = (int)DialogHelpers.Context.ToPixels(measure.Width);
        var realH = (int)DialogHelpers.Context.ToPixels(measure.Height);

        var layout = new LinearLayout(DialogHelpers.Context);
        using (var param = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
        {
            Width = realW,
            Height = realH
        })
        {
            layout.LayoutParameters = param;
        }

        using (var param = new LinearLayout.LayoutParams(realW, realH)
        {
            Width = realW,
            Height = realH
        })
        {
            layout.AddView(viewHandler.PlatformView, param);
        }

        if (view.CornerRadius > 0)
        {
            var border = new GradientDrawable();
            border.SetCornerRadius(DialogHelpers.Context.ToPixels(view.CornerRadius));
            if (!view.BackgroundColor.IsDefault())
            {
                border.SetColor(view.BackgroundColor.ToAndroid());
                border.Alpha = (int)(view.Opacity * 255);
            }
            layout.ClipToOutline = true;
            layout.SetBackground(border);
        }

#pragma warning disable CA1422 // プラットフォームの互換性を検証
        toast.View = layout;
#pragma warning restore CA1422 // プラットフォームの互換性を検証

        view.RunPresentationAnimation();

        toast.Show();

        var duration = Math.Max(Math.Min(view.Duration - 260, 3500), 0); // give a bit millisecond margin 

        var handler = new Handler(Looper.MainLooper);

        handler.PostDelayed(new Runnable(view.RunDismissalAnimation), duration);

        handler.PostDelayed(new Runnable(() =>
        {
            //view.RunDismissalAnimation();
            //await Task.Delay(250);
            toast?.Cancel();

            view.Parent = null;

            if (!viewHandler.PlatformView.IsDisposed())
            {
                var platformView = viewHandler.PlatformView;
                viewHandler.DisconnectHandler();
                platformView.RemoveFromParent();
                platformView.Dispose();
            }

            layout.Dispose();

            // I coudn't reproduce https://github.com/muak/AiForms.Dialogs/issues/2.
            // But I let this code disabled because it has no influent even if it is disabled.
            //toast.View = null; 
            viewHandler = null;
            toast?.Dispose();
            toast = null;

            view.Destroy();
            view.BindingContext = null;
        }), view.Duration);
    }
}
