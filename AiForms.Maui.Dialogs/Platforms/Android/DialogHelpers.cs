using System;
using System.Drawing;
using AiForms.Dialogs;
using AiForms.Dialogs.Extensions;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using Microsoft.Maui.Platform;
using Rect = Android.Graphics.Rect;
using Size = System.Drawing.Size;
using View = Microsoft.Maui.Controls.View;
using AView = Android.Views.View;
using MauiSize = Microsoft.Maui.Graphics.Size;
using Microsoft.Maui.Controls.Platform;
using Application = Microsoft.Maui.Controls.Application;
using Android.OS;

namespace AiForms.Dialogs.Droid;

public static class DialogHelpers
{
    internal static Context Context => Platform.CurrentActivity;    

    internal static AndroidX.Fragment.App.FragmentManager FragmentManager => (Context as Activity)?.GetFragmentManager();

    static int? _statusbarHeight;
    internal static int StatusBarHeight => _statusbarHeight ?? GetBarSize().statusBar;

    static int? _navigationBarHeight;
    internal static int NavigationBarHeight => _navigationBarHeight ?? GetBarSize().navigationBar;

    static Size? _contentSize;
    internal static Size ContentSize
    {
        get
        {
            if(_contentSize != null)
            {
                return _contentSize.Value;
            }

            Rect contentSize = new Rect();
            (Context as Activity)?.Window.DecorView.GetWindowVisibleDisplayFrame(contentSize);
            _contentSize = new Size(contentSize.Width(), contentSize.Height());

            int availableHeight = Context.Resources.DisplayMetrics.HeightPixels;

            _statusBarHeightInContent = availableHeight - contentSize.Height();

            return _contentSize.Value;
        }
    }

    internal static (int statusBar, int navigationBar) GetBarSize()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
        {
#pragma warning disable CA1416 // プラットフォームの互換性を検証
            var metrics = (Context as Activity).WindowManager.CurrentWindowMetrics;
            var inset = metrics.WindowInsets.GetInsetsIgnoringVisibility(WindowInsets.Type.SystemBars());

            _statusbarHeight = inset.Top;
            _navigationBarHeight = inset.Bottom;
#pragma warning restore CA1416 // プラットフォームの互換性を検証
        }
        else
        {
            _statusbarHeight =
        Context.Resources.GetDimensionPixelSize(Context.Resources.GetIdentifier("status_bar_height", "dimen", "android"));
            _navigationBarHeight =
        Context.Resources.GetDimensionPixelSize(Context.Resources.GetIdentifier("navigation_bar_height", "dimen", "android"));
        }

        return (_statusbarHeight.Value, _navigationBarHeight.Value);
    }

    // Height of status bar included in content area
    static int? _statusBarHeightInContent;
    internal static int StatusBarHeightInContent
    {
        get
        {
            if (!_contentSize.HasValue)
            {
                _ = ContentSize;
            }
            
            return _statusBarHeightInContent.Value;
        }
    }

    internal static int DisplayHeight => StatusBarHeight + ContentSize.Height;

    internal static IPlatformViewHandler CreateNewHandler(View view)
    {
        return view.ToHandler(view.FindMauiContext());
    }

    internal static Android.App.Dialog CreateFullScreenTransparentDialog(AView contentView)
    {
        var dialog = new Android.App.Dialog(Context, Resource.Style.NoDimDialogFragmentStyle);

        dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
        dialog.SetContentView(contentView);

        dialog.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));
        dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

        return dialog;
    }

    internal static MauiSize Measure(ExtraView view)
    {
        var dWidth = Context.FromPixels(ContentSize.Width);
        var dHeight = Context.FromPixels(ContentSize.Height);

        bool isFixWidth = true;
        bool isFixHeight = true;
        var marginTop = view.DialogMargin.Top;
        var marginLeft = view.DialogMargin.Left;
        var marginBottom = view.DialogMargin.Bottom;
        var marginRight = view.DialogMargin.Right;

        double fWidth = dWidth;
        if (view.ProportionalWidth >= 0)
        {
            fWidth = dWidth * view.ProportionalWidth;
        }
        else if (view.HorizontalLayoutAlignment == LayoutAlignment.Fill)
        {
            fWidth = dWidth - marginLeft - marginRight;
        }
        else if (view.WidthRequest == -1)
        {
            fWidth = double.PositiveInfinity;
            isFixWidth = false;
        }
        else if (view.WidthRequest >= 0)
        {
            fWidth = view.WidthRequest;
        }

        double fHeight = dHeight;
        double maxHeight = dHeight - marginTop - marginBottom;
        if (view.ProportionalHeight >= 0)
        {
            fHeight = dHeight * view.ProportionalHeight;
        }
        else if (view.VerticalLayoutAlignment == LayoutAlignment.Fill)
        {
            fHeight = maxHeight;
        }
        else if (view.HeightRequest == -1)
        {
            fHeight = double.PositiveInfinity;
            isFixHeight = false;
        }
        else if (view.HeightRequest >= 0)
        {
            fHeight = view.HeightRequest;
        }


        if (!isFixWidth || !isFixHeight)
        {
            var handler = (IPlatformViewHandler)view.Handler;
            
            var sizeRequest = handler.VirtualView.Measure(fWidth, fHeight);
            var requestHeight = Math.Min(sizeRequest.Height, maxHeight);

            var reqWidth = isFixWidth ? fWidth : sizeRequest.Width;            
            var reqHeight = isFixHeight ? fHeight : requestHeight;

            return new MauiSize(reqWidth, reqHeight);           
        }

        return new MauiSize(fWidth, fHeight);
    }

    internal static void SetOffsetMargin(FrameLayout.LayoutParams layoutParams, ExtraView view)
    {
        var offsetX = (int)Context.ToPixels(view.OffsetX);
        var offsetY = (int)Context.ToPixels(view.OffsetY);

        // the offset direction is reversed when GravityFlags contains Left or Bottom.
        if (view.HorizontalLayoutAlignment == LayoutAlignment.End)
        {
            layoutParams.RightMargin = offsetX * -1;
        }
        else
        {
            layoutParams.LeftMargin = offsetX;
        }

        if (view.VerticalLayoutAlignment == LayoutAlignment.End)
        {
            layoutParams.BottomMargin = offsetY * -1;
        }
        else
        {
            layoutParams.TopMargin = offsetY;
        }
    }

    internal static void SetOffsetMargin(FrameLayout.LayoutParams layoutParams, int offsetX,int offsetY)
    {
        layoutParams.LeftMargin = (int)Context.ToPixels(offsetX);
        layoutParams.TopMargin = (int)Context.ToPixels(offsetY);
    }

    internal static ViewGroup SetViewAppearance(ExtraView virtualView,ViewGroup nativeView)
    {            
        if (virtualView.CornerRadius > 0 && virtualView.BorderWidth > 0)
        {
            var wrapper = new CardView(Context);
            wrapper.Radius = Context.ToPixels(virtualView.CornerRadius);
            wrapper.SetCardBackgroundColor(virtualView.BorderColor.ToPlatform());
            wrapper.CardElevation = 0;
            var borderW = (int)Context.ToPixels(virtualView.BorderWidth);
            wrapper.SetContentPadding(borderW, borderW, borderW, borderW);
            wrapper.SetClipChildren(true);

            var inner = nativeView;
            var border = new GradientDrawable();
            var innerRadius = Math.Max(virtualView.CornerRadius - virtualView.BorderWidth, 0);
            border.SetCornerRadius(Context.ToPixels(innerRadius));
            if (!virtualView.BackgroundColor.IsDefault())
            {
                border.SetColor(virtualView.BackgroundColor.ToPlatform());
            }

            inner.SetBackground(border);
            inner.ClipToOutline = true;

            wrapper.AddView(inner);
            return wrapper;
        }

        if(virtualView.CornerRadius > 0 || virtualView.BorderWidth > 0)
        {
            var border = new GradientDrawable();
            if (virtualView.CornerRadius > 0)
            {
                border.SetCornerRadius(Context.ToPixels(virtualView.CornerRadius));
            }
            if (!virtualView.BackgroundColor.IsDefault())
            {
                border.SetColor(virtualView.BackgroundColor.ToPlatform());
            }

            if (virtualView.BorderWidth > 0)
            {
                var borderW = (int)Context.ToPixels(virtualView.BorderWidth);
                border.SetStroke(borderW, virtualView.BorderColor.ToPlatform());
                nativeView.SetPadding(borderW, borderW, borderW, borderW);
            }

            nativeView.SetBackground(border);
            nativeView.ClipToOutline = true;
        }

        return nativeView;
    }

    internal static GravityFlags GetGravity(ExtraView view)
    {
        GravityFlags gravity = GravityFlags.NoGravity;
        switch (view.VerticalLayoutAlignment)
        {
            case LayoutAlignment.Start:
                gravity |= GravityFlags.Top;
                break;
            case LayoutAlignment.End:
                gravity |= GravityFlags.Bottom;
                break;
            default:
                gravity |= GravityFlags.CenterVertical;
                break;
        }

        switch (view.HorizontalLayoutAlignment)
        {
            case LayoutAlignment.Start:
                gravity |= GravityFlags.Left;
                break;
            case LayoutAlignment.End:
                gravity |= GravityFlags.Right;
                break;
            default:
                gravity |= GravityFlags.CenterHorizontal;
                break;
        }

        return gravity;
    }

    internal static (int top, int bottom) CalcWindowPadding()
    {
        var activePage = Application.Current.MainPage.GetActivePage();
        var activeHandler = activePage.ToHandler(activePage.FindMauiContext());
       
        var rect = new Rect();       
        activeHandler.PlatformView.GetGlobalVisibleRect(rect);

        var top = rect.Top;
        // If the device is that StatusBarSize is not included in ContentArea
        if(StatusBarHeightInContent == 0)
        {            
            top -= StatusBarHeight;  
        }

        return (top, DisplayHeight - rect.Bottom);
    }
}
