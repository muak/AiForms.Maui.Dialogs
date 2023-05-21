using System;
using System.Threading;
using System.Threading.Tasks;
using AiForms.Dialogs;
using AiForms.Dialogs.Droid;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.Fragment.App;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Platform;
using Animation = Android.Views.Animations.Animation;
using Application = Microsoft.Maui.Controls.Application;
using AView = Android.Views.View;

namespace AiForms.Dialogs;

public class ReusableDialog : Java.Lang.Object, IReusableDialog
{
    ExtraPlatformDialog _platformDialog;
    AndroidX.Fragment.App.FragmentManager FragmentManager => DialogHelpers.FragmentManager;
    DialogView _dlgView;
    IPlatformViewHandler _handler;
    ViewGroup _contentView;
    ViewGroup _container;
    Action OnceInitializeAction;
    Guid _guid;

    public ReusableDialog(DialogView view)
    {
        _dlgView = view;
        _guid = Guid.NewGuid();

        // Because the process can't be executed until application completely loads,
        // set the action here to execute later on.
        OnceInitializeAction = Initialize;
    }

    void Initialize()
    {
        _dlgView.Parent = Application.Current.MainPage;

        _handler = DialogHelpers.CreateNewHandler(_dlgView);

        var measure = DialogHelpers.Measure(_dlgView);

        _dlgView.Layout(new Rect(0, 0, measure.Width, measure.Height));

        _container = DialogHelpers.SetViewAppearance(_dlgView, _handler.PlatformView as ViewGroup);

        _contentView = new FrameLayout(DialogHelpers.Context);
        using (var param = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent))
        {
            _contentView.LayoutParameters = param;
        }

        var fixPaddingTop = _dlgView.OverlayColor.IsTransparentOrDefault() ? DialogHelpers.StatusBarHeightInContent : 0;

        if (_dlgView.UseCurrentPageLocation)
        {
            var padding = DialogHelpers.CalcWindowPadding();
            _contentView.SetPadding(0, padding.top - fixPaddingTop, 0, padding.bottom);
        }
        else
        {
            _contentView.SetPadding(0, DialogHelpers.StatusBarHeightInContent - fixPaddingTop, 0, 0);
        }

        _contentView.SetBackgroundColor(_dlgView.OverlayColor.ToPlatform());
        _contentView.SetClipChildren(false);
        _contentView.SetClipToPadding(false);

        _contentView.FocusableInTouchMode = true;
        _contentView.Touch += _contentView_Touch;

        var width = DialogHelpers.Context.ToPixels(_dlgView.Bounds.Width);
        var height = DialogHelpers.Context.ToPixels(_dlgView.Bounds.Height);

        using (var param = new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
        {
            Width = (int)width,
            Height = (int)height,
            Gravity = DialogHelpers.GetGravity(_dlgView),
        })
        {
            DialogHelpers.SetOffsetMargin(param, _dlgView);
            _contentView.AddView(_container, 0, param);
        };

        // For now, Dynamic resizing is gaven to only Dialog.
        _dlgView.LayoutNative = () =>
        {
            if (_handler == null || _handler.PlatformView.IsDisposed()) return;

            var p = _handler.PlatformView.LayoutParameters as FrameLayout.LayoutParams;
            var w = (int)DialogHelpers.Context.ToPixels(_dlgView.Bounds.Width);
            var h = (int)DialogHelpers.Context.ToPixels(_dlgView.Bounds.Height);


            using (var param = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
            {
                Width = w,
                Height = h,
                Gravity = p.Gravity
            })
            {
                DialogHelpers.SetOffsetMargin(param, _dlgView);
                _handler.PlatformView.LayoutParameters = param;
            };
        };

        OnceInitializeAction = null;
    }

    public async Task<bool> ShowAsync()
    {
        var dialog = FragmentManager.FindFragmentByTag(_guid.ToString()) as ExtraPlatformDialog;
        if (dialog != null)
        {
            return false;
        }

        _dlgView.SetUp();

        OnceInitializeAction?.Invoke();

        var tcs = new TaskCompletionSource<bool>();

        async void cancel(object sender, DialogNotifierArgs e)
        {
            _dlgView.RunDismissalAnimation();
            await Dismiss();
            tcs.SetResult(false);
        }
        async void complete(object sender, DialogNotifierArgs e)
        {
            _dlgView.RunDismissalAnimation();
            await Dismiss();
            tcs.SetResult(true);
        }

        _dlgView.DialogNotifierInternal.Canceled += cancel;
        _dlgView.DialogNotifierInternal.Completed += complete;


        var payload = new ExtraDialogPayload(_dlgView, _contentView);
        var bundle = new Bundle();
        bundle.PutSerializable("extraDialogPayload", payload);
        _platformDialog = new ExtraPlatformDialog();
        _platformDialog.Arguments = bundle;
        _platformDialog.Show(FragmentManager, _guid.ToString());

        try
        {
            return await tcs.Task;
        }
        finally
        {
            _dlgView.DialogNotifierInternal.Canceled -= cancel;
            _dlgView.DialogNotifierInternal.Completed -= complete;
            _dlgView.TearDown();
            payload.Dispose();
            bundle.Dispose();

        }
    }

    public async Task<TResult> ShowResultAsync<TResult>()
    {
        var dialog = FragmentManager.FindFragmentByTag(_guid.ToString()) as ExtraPlatformDialog;
        if (dialog != null)
        {
            return default;
        }

        _dlgView.SetUp();

        OnceInitializeAction?.Invoke();

        var tcs = new TaskCompletionSource<TResult>();

        async void cancel(object sender, DialogNotifierArgs e)
        {
            _dlgView.RunDismissalAnimation();
            await Dismiss();
            tcs.SetResult(default);
        }
        async void complete(object sender, DialogNotifierArgs e)
        {
            _dlgView.RunDismissalAnimation();
            await Dismiss();
            tcs.SetResult((TResult)e.Result);
        }

        _dlgView.DialogNotifierInternal.Canceled += cancel;
        _dlgView.DialogNotifierInternal.Completed += complete;

        var payload = new ExtraDialogPayload(_dlgView, _contentView);
        var bundle = new Bundle();
        bundle.PutSerializable("extraDialogPayload", payload);
        _platformDialog = new ExtraPlatformDialog();
        _platformDialog.Arguments = bundle;
        _platformDialog.Show(FragmentManager, _guid.ToString());

        try
        {
            return await tcs.Task;
        }
        finally
        {
            _dlgView.DialogNotifierInternal.Canceled -= cancel;
            _dlgView.DialogNotifierInternal.Completed -= complete;
            _dlgView.TearDown();
            payload.Dispose();
            bundle.Dispose();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _dlgView.Destroy();
            _dlgView.LayoutNative = null;
            _dlgView.BindingContext = null;
            _dlgView.Parent = null;
            _dlgView.Handler = null;
            _dlgView = null;

            _contentView.Touch -= _contentView_Touch;

            _container?.Dispose();
            _container = null;
            
            _contentView.Dispose();
            _contentView = null;

            _handler = null;

            OnceInitializeAction = null;
        }
        base.Dispose(disposing);
    }

    void _contentView_Touch(object sender, AView.TouchEventArgs e)
    {
        if (e.Event.Action != MotionEventActions.Down || !_dlgView.IsCanceledOnTouchOutside)
        {
            e.Handled = false;
            return;
        }

        Android.Graphics.Rect rect = new Android.Graphics.Rect();
        _handler.PlatformView.GetHitRect(rect);

        if (!rect.Contains((int)e.Event.GetX(), (int)e.Event.GetY()))
        {
            _dlgView.DialogNotifierInternal.Cancel();
            return;
        }

        e.Handled = false;
    }

    async Task Dismiss()
    {
        var tcs = new TaskCompletionSource<bool>();

        var anim = new AlphaAnimation(_contentView.Alpha, 0.0f);
        anim.Duration = 250;
        anim.FillAfter = true;

        void handler(object sender, Animation.AnimationEndEventArgs e)
        {
            tcs.SetResult(true);
        };

        anim.AnimationEnd += handler;

        _contentView.StartAnimation(anim);

        await tcs.Task;
        anim.AnimationEnd -= handler;

        var dialog = FragmentManager.FindFragmentByTag(_guid.ToString()) as ExtraPlatformDialog;
        dialog.Clear();
        dialog.Dismiss();
        _contentView.RemoveFromParent();
        _handler.PlatformView.RemoveFromParent();
        _handler.DisconnectHandler();
        //_handler.PlatformView.Dispose(); // Unnecessary as it is discarded by DisconnectHandler

        _platformDialog.Dispose();
        _platformDialog = null;

        await Task.Delay(250); // wait for a bit time until the dialog is completely released.
    }

    
}
