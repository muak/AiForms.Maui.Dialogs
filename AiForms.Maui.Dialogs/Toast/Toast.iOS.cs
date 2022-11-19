using System;
using System.Runtime.InteropServices;
using AiForms.Dialogs.iOS;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Platform;
using UIKit;

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
        if (viewModel != null)
        {
            view.BindingContext = viewModel;
        }
        view.Parent = Application.Current.MainPage;

        var handler = DialogHelpers.CreateNewHandler(view);

        var measure = DialogHelpers.Measure(view);
        view.Layout(new Rect(new Point(0,0),measure));

        handler.PlatformView.Alpha = 0;
        if (view.CornerRadius > 0)
        {
            handler.PlatformView.Layer.CornerRadius = view.CornerRadius;
            handler.PlatformView.Layer.MasksToBounds = true;
        }
        if (view.BorderWidth > 0)
        {
            handler.PlatformView.Layer.BorderWidth = (float)view.BorderWidth;
            handler.PlatformView.Layer.BorderColor = view.BorderColor.ToCGColor();
        }

        SetView(view, handler.PlatformView);

        view.Parent = null;

        view.RunPresentationAnimation();
        UIView.Animate(
            0.25,
            () => handler.PlatformView.Alpha = (NFloat)view.Opacity
        );


        var timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(Math.Max(view.Duration - 250, 0));
        timer.IsRepeating = false;
        timer.Tick += (s, e) =>
        {
            view.RunDismissalAnimation();
            UIView.Animate(
                0.25,
                () => handler.PlatformView.Alpha = 0,
                () => {
                    view.Parent = null;
                    handler.PlatformView.RemoveConstraints(handler.PlatformView.Constraints);
                    view.DisposeModalAndChildHandlers();
                    handler.DisconnectHandler();
                    handler = null;
                    view.Destroy();
                    view.BindingContext = null;
                    view = null;
                }
            );
            timer.Stop();
        };
        timer.Start();
    }

    void SetView(ToastView view, UIView nativeView)
    {
        var window = DialogHelpers.GetKeyWindow();

        nativeView.TranslatesAutoresizingMaskIntoConstraints = false;

        window.AddSubview(nativeView);

        nativeView.WidthAnchor.ConstraintEqualTo((NFloat)view.Bounds.Width).Active = true;
        nativeView.HeightAnchor.ConstraintEqualTo((NFloat)view.Bounds.Height).Active = true;

        DialogHelpers.SetLayoutAlignment(nativeView, window, view);
    }
}

