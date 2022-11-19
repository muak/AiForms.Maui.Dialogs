using System;
using System.Threading.Tasks;
using AiForms.Dialogs;
using AiForms.Dialogs.Droid;
using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using Application = Microsoft.Maui.Controls.Application;

namespace AiForms.Dialogs;

public class ReusableLoading: LoadingBase,IReusableLoading
{
    IPlatformViewHandler _handler;
    LoadingView _loadingView;

    public ReusableLoading(LoadingView loadingView,LoadingPlatformDialog loadingDialog):base(loadingDialog)
    {
        _loadingView = loadingView;

        OnceInitializeAction = Initialize;
    }

    public override void Dispose()
    {
        _loadingView.Destroy();
        _loadingView.BindingContext = null;
        _loadingView.Parent = null;
        _loadingView = null;

        if (_handler != null)
        {            
            if (!_handler.PlatformView.IsDisposed())
            {
                var platformView = _handler.PlatformView;
                _handler?.DisconnectHandler();
                platformView.RemoveFromParent();
                platformView.Dispose();                
            }
            
            _handler = null;
        }

        base.Dispose();
    }

    public void Show(bool isCurrentScope = false)
    {
        if (IsRunning()) return;

        ShowInner();
    }

    public async Task StartAsync(Func<IProgress<double>, Task> action, bool isCurrentScope = false)
    {
        await WaitDialogDestroy();

        ShowInner();
        Progress = new Progress<double>();
        Progress.ProgressChanged += ProgressAction;
        await action(Progress);
        await Hide();            
    }

    void ShowInner()
    {
        IsDialogShownTcs = new TaskCompletionSource<bool>();
        OnceInitializeAction?.Invoke();

        var payload = new LoadingDialogPayload(ContentView,IsDialogShownTcs, _loadingView);

        var bundle = new Bundle();
        bundle.PutSerializable(LoadingDialogPayload.PayloadKey, payload);
        PlatformDialog.Arguments = bundle;

        PlatformDialog.Show(FragmentManager, Loading.LoadingDialogTag);
    }

    public async Task Hide()
    {
        if (Progress != null)
        {
            Progress.ProgressChanged -= ProgressAction;
            Progress = null;
        }

        var anim = new AlphaAnimation(ContentView.Alpha, 0.0f);
        anim.Duration = 250;
        anim.FillAfter = true;
        
        ContentView.StartAnimation(anim);
        _loadingView?.RunDismissalAnimation();
       

        _ = Task.Run(async () =>
        {
            // Wait for ensuring that the dialog is created. 
            // Because it sometimes crashes or freezes when executing a very short process.
            await IsDialogShownTcs.Task;
            var dialog = FragmentManager.FindFragmentByTag(Loading.LoadingDialogTag) as LoadingPlatformDialog;
            MainThread.BeginInvokeOnMainThread(() => 
            {
                dialog?.Dismiss();
                ContentView.RemoveFromParent();
            });               
        });

        await Task.Delay(250); // wait for animation
    }

    void ProgressAction(object sender, double progress)
    {
        _loadingView.Progress = progress;
    }

    void Initialize()
    {
        OnceInitializeAction = null;

        ContentView.SetBackgroundColor(_loadingView.OverlayColor.ToPlatform());
        ContentView.Alpha = 1f;
        ContentView.SetPadding(0, (int)DialogHelpers.Context.ToPixels(24), 0, 0); // Statusbar

        _loadingView.Parent = Application.Current.MainPage;

        _handler = DialogHelpers.CreateNewHandler(_loadingView);

        if (_loadingView.CornerRadius > 0)
        {
            var nativeView = _handler.PlatformView as ViewGroup;
            var border = new GradientDrawable();
            border.SetCornerRadius(DialogHelpers.Context.ToPixels(_loadingView.CornerRadius));
            if (!_loadingView.BackgroundColor.IsDefault())
            {
                border.SetColor(_loadingView.BackgroundColor.ToPlatform());
            }
            nativeView.ClipToOutline = true;
            nativeView.SetBackground(border);
        }

        var measure = DialogHelpers.Measure(_loadingView);

        _loadingView.Layout(new Rect(0, 0, measure.Width, measure.Height));


        var width = (int)DialogHelpers.Context.ToPixels(_loadingView.Bounds.Width);
        var height = (int)DialogHelpers.Context.ToPixels(_loadingView.Bounds.Height);

        using (var param = new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent
            )
        {
            Width = width,
            Height = height,
            Gravity = DialogHelpers.GetGravity(_loadingView)
        })
        {
            DialogHelpers.SetOffsetMargin(param, _loadingView);
            ContentView.AddView(_handler.PlatformView, 0, param);
        }
    }
}
