using System;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Android.Graphics;
using AiForms.Dialogs.Droid;
using Microsoft.Maui.Platform;

namespace AiForms.Dialogs;

public class DefaultLoading:LoadingBase
{
    LoadingConfig _config => Configurations.LoadingConfig;
    TextView _messageLabel;
    string _message;

    public DefaultLoading(LoadingPlatformDialog loadingDialog):base(loadingDialog)
    {
        OnceInitializeAction = Initialize;
    }

    public override void Dispose()
    {
        _messageLabel?.Dispose();
        _messageLabel = null;

        base.Dispose();
    }

    public async Task StartAsync(Func<IProgress<double>, Task> action, string message = null, bool isCurrentScope = false)
    {
        Show(message, isCurrentScope);
        Progress = new Progress<double>();
        Progress.ProgressChanged += ProgressAction;
        await action(Progress);
    }

    public void Show(string message = null, bool isCurrentScope = false)
    {
        IsDialogShownTcs = new TaskCompletionSource<bool>();
        OnceInitializeAction?.Invoke();

        var payload = new LoadingDialogPayload(ContentView,IsDialogShownTcs);

        var bundle = new Bundle();
        bundle.PutSerializable(LoadingDialogPayload.PayloadKey, payload);
        PlatformDialog.Arguments = bundle;

        _message = message ?? _config.DefaultMessage;
        _messageLabel.Text = _message;

        PlatformDialog.Show(FragmentManager, Loading.LoadingDialogTag);
    }

    public void Hide()
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

        Task.Run(async () =>
        {
            // Wait for ensuring that the dialog is created. 
            // Because it sometimes crashes or freezes when executing a very short process.
            await IsDialogShownTcs.Task;
            var dialog = FragmentManager.FindFragmentByTag(Loading.LoadingDialogTag) as LoadingPlatformDialog;
            dialog?.Dismiss();
            ContentView.RemoveFromParent();
            if(!Configurations.LoadingConfig.IsReusable)
            {
                this.Dispose();
            }
        });
    }

    void ProgressAction(object sender, double progress)
    {
        SetMessageInner(_message, progress);
    }

    public void SetMessage(string message)
    {
        SetMessageInner(message);
    }

    void SetMessageInner(string message, double progress = -1)
    {
        _message = message ?? _config.DefaultMessage;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if(_messageLabel is null)
            {
                return;
            }
            if (progress >= 0)
            {
                _messageLabel.Text = string.Format(_config.ProgressMessageFormat, _message, progress);
            }
            else
            {
                _messageLabel.Text = _message;
            }
        });
    }

    void Initialize()
    {
        OnceInitializeAction = null;

        ContentView.SetBackgroundColor(_config.OverlayColor.ToPlatform());
        ContentView.Alpha = (float)_config.Opacity;

        var innerView = (DialogHelpers.Context as Activity).LayoutInflater.Inflate(Resource.Layout.loadingdialoglayout, null);

        var progress = innerView.FindViewById<Android.Widget.ProgressBar>(Resource.Id.progress);
        _messageLabel = innerView.FindViewById<TextView>(Resource.Id.loading_message);

        progress.IndeterminateDrawable.SetColorFilterEx(_config.IndicatorColor.ToPlatform());
        _messageLabel.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)_config.FontSize);
        _messageLabel.SetTextColor(_config.FontColor.ToPlatform());

        using (var param = new FrameLayout.LayoutParams(
        ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
        {
            Gravity = GravityFlags.Center,
        })
        {
            DialogHelpers.SetOffsetMargin(param, _config.OffsetX, _config.OffsetY);
            ContentView.AddView(innerView, 0, param);
        };
    }
}
