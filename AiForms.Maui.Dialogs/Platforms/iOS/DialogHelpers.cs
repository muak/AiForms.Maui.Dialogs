using System;
using System.Reflection;
using AiForms.Dialogs;
using UIKit;
using CoreGraphics;
using Microsoft.Maui.Platform;
using AiForms.Dialogs.Extensions;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Linq;

namespace AiForms.Dialogs.iOS;

[Foundation.Preserve(AllMembers = true)]
public static class DialogHelpers
{
    internal static UIViewController RootViewController{
        get{            
            var vc = GetKeyWindow()?.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }
            return vc;
        }
    }

    internal static UIWindow GetKeyWindow()
    {
        if (UIDevice.CurrentDevice.CheckSystemVersion(15, 0))
        {
            var scenes = UIApplication.SharedApplication.ConnectedScenes;
            var windowScene = scenes.ToArray().First() as UIWindowScene;
            return windowScene?.KeyWindow;
        }

#pragma warning disable XI0001 // Apple API の使用法に関するアドバイスを通知します
        return UIApplication.SharedApplication.Windows.FirstOrDefault(x => x.IsKeyWindow);
#pragma warning restore XI0001 // Apple API の使用法に関するアドバイスを通知します
    }

    internal static IPlatformViewHandler CreateNewHandler(VisualElement view)
    {
        var handler = view.ToHandler(view.FindMauiContext());

        handler.PlatformView.AutoresizingMask = UIViewAutoresizing.All;
        handler.PlatformView.ContentMode = UIViewContentMode.ScaleToFill;        

        return handler;
    }

    internal static Size Measure(ExtraView view)
    {
        var window = GetKeyWindow();

        var dWidth = window.Bounds.Width;
        var dHeight = window.Bounds.Height;

        double fWidth = dWidth;
        bool isFixWidth = true;
        bool isFixHeight = true;
        var marginTop = view.Margin.Top;
        var marginLeft = view.Margin.Left;
        var marginBottom = view.Margin.Bottom;
        var marginRight = view.Margin.Right;

        if (view.ProportionalWidth >= 0)
        {
            fWidth = dWidth * view.ProportionalWidth;
        }
        else if (view.HorizontalLayoutAlignment == LayoutAlignment.Fill)
        {
            fWidth = dWidth - marginLeft - marginRight;
        }
        else if(view.WidthRequest == -1)
        {
            fWidth = double.PositiveInfinity;
            isFixWidth = false;
        }
        else if(view.WidthRequest >= 0)
        {
            fWidth = view.WidthRequest;
        }

        double fHeight = dHeight;
        if(view.ProportionalHeight >= 0)
        {
            fHeight = dHeight * view.ProportionalHeight;
        }
        else if (view.VerticalLayoutAlignment == LayoutAlignment.Fill)
        {
            fHeight = dHeight - marginTop - marginBottom;
        }
        else if(view.HeightRequest == -1)
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

            var reqWidth = isFixWidth ? fWidth : sizeRequest.Width;
            var reqHeight = isFixHeight ? fHeight : sizeRequest.Height;

            return new Size(reqWidth,reqHeight);
        }

        // If both width and height are proportional, Measure is not called.
        return new Size(fWidth, fHeight);
    }

    internal static CGRect GetCurrentPageRect(UIView parentView)
    {
        var activePage = Application.Current.MainPage.GetActivePage();
        var activeHandler = activePage.ToHandler(activePage.FindMauiContext());

        var rect = activeHandler.PlatformView.ConvertRectToView(activeHandler.PlatformView.Bounds, parentView);

        activeHandler = null;

        return rect;
    }

    internal static UIView GetCurrentPageView()
    {
        var activePage = Application.Current.MainPage.GetActivePage();
        var activeHandler = activePage.ToHandler(activePage.FindMauiContext());

        var view = activeHandler.PlatformView;

        activeHandler = null;

        return view;
    }

    internal static void SetLayoutAlignment(UIView targetView,UIView parentView,ExtraView dialog)
    {
        switch (dialog.VerticalLayoutAlignment)
        {
            case LayoutAlignment.Start:
                targetView.TopAnchor.ConstraintEqualTo(parentView.TopAnchor, dialog.OffsetY).Active = true;
                break;
            case LayoutAlignment.End:
                targetView.BottomAnchor.ConstraintEqualTo(parentView.BottomAnchor, dialog.OffsetY).Active = true;
                break;
            default:
                targetView.CenterYAnchor.ConstraintEqualTo(parentView.CenterYAnchor, dialog.OffsetY).Active = true;
                break;
        }

        switch (dialog.HorizontalLayoutAlignment)
        {
            case LayoutAlignment.Start:
                targetView.LeftAnchor.ConstraintEqualTo(parentView.LeftAnchor, dialog.OffsetX).Active = true;
                break;
            case LayoutAlignment.End:
                targetView.RightAnchor.ConstraintEqualTo(parentView.RightAnchor, dialog.OffsetX).Active = true;
                break;
            default:
                targetView.CenterXAnchor.ConstraintEqualTo(parentView.CenterXAnchor, dialog.OffsetX).Active = true;
                break;
        }
    }    
}
