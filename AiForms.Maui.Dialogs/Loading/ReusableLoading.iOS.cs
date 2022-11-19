using System;
using System.Threading.Tasks;
using UIKit;
using System.Collections.Generic;
using AiForms.Dialogs.iOS;
using Microsoft.Maui.Platform;
using System.Runtime.InteropServices;

namespace AiForms.Dialogs;

[Foundation.Preserve(AllMembers = true)]
public class ReusableLoading: LoadingBase,IReusableLoading
{
    LoadingView _loadingView;
    IPlatformViewHandler _handler;

    public ReusableLoading(LoadingView loadingView)
    {
        _loadingView = loadingView;

        OnceInitializeAction = Initialize;
    }

    public override void Dispose()
    {
        _loadingView.Parent = null;
        _loadingView.DisposeModalAndChildHandlers();
        _handler.DisconnectHandler();
        _handler = null;
    
        _loadingView.Destroy();
        _loadingView.BindingContext = null;
        _loadingView = null;

        base.Dispose();
    }

    public async Task StartAsync(Func<IProgress<double>, Task> action, bool isCurrentScope = false)
    {
        if(!await WaitDismiss())
        {
            return;
        }

        ShowInner(isCurrentScope);
        Progress = new Progress<double>();
        Progress.ProgressChanged += ProgressAction;
        await action(Progress);
        await Hide();
    }

    public void Show(bool isCurrentScope = false)
    {
        if(IsRunning) {
            return;
        }

        ShowInner(isCurrentScope);
    }

    void ShowInner(bool isCurrentScope)
    {
        IsRunning = true;

        OnceInitializeAction?.Invoke(isCurrentScope);

        if (IsCurrentScope.HasValue && IsCurrentScope != isCurrentScope)
        {
            SetOverlayConstrants(isCurrentScope);
        }

        IsCurrentScope = isCurrentScope;

        _loadingView.RunPresentationAnimation();

        UIView.Animate(0.25, () => OverlayView.Alpha = 1f, () => { });
    }

    public async Task Hide()
    {

        if (Progress != null)
        {
            Progress.ProgressChanged -= ProgressAction;
            Progress = null;
            if (_loadingView != null)
            {
                //_loadingView.Progress = 0d;
            }
        }

        await UIView.AnimateAsync(
            0.25, // duration
            () => { OverlayView.Alpha = 0; }
        );

        _loadingView?.RunDismissalAnimation();
        IsRunning = false;
    }


    void ProgressAction(object sender, double progress)
    {
        if (_loadingView != null)
        {
            _loadingView.Progress = progress;
        }
    }

    void Initialize(bool isCurrentScope = false)
    {
        OnceInitializeAction = null;

        OverlayView.BackgroundColor = _loadingView.OverlayColor.ToPlatform();
        OverlayView.Alpha = 0f;
        OverlayView.TranslatesAutoresizingMaskIntoConstraints = false;

        SetOverlayConstrants(isCurrentScope);

        _loadingView.Parent = Application.Current.MainPage;

        _handler = DialogHelpers.CreateNewHandler(_loadingView);

        if (_loadingView.CornerRadius > 0)
        {
            _handler.PlatformView.Layer.CornerRadius = _loadingView.CornerRadius;
            _handler.PlatformView.Layer.MasksToBounds = true;
        }
        if (_loadingView.BorderWidth > 0)
        {
            _handler.PlatformView.Layer.BorderWidth = (float)_loadingView.BorderWidth;
            _handler.PlatformView.Layer.BorderColor = _loadingView.BorderColor.ToCGColor();
        }

        var measure = DialogHelpers.Measure(_loadingView);
        _loadingView.Layout(new Rect(0, 0, measure.Width, measure.Height));

        var nativeView = _handler.PlatformView;

        nativeView.TranslatesAutoresizingMaskIntoConstraints = false;

        OverlayView.AddSubview(nativeView);

        nativeView.WidthAnchor.ConstraintEqualTo((NFloat)_loadingView.Bounds.Width).Active = true;
        nativeView.HeightAnchor.ConstraintEqualTo((NFloat)_loadingView.Bounds.Height).Active = true;

        DialogHelpers.SetLayoutAlignment(nativeView, OverlayView, _loadingView);
    }
}
