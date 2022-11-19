using AiForms.Dialogs;
using Android.App;
using Android.OS;
using Android.Views;
using System.Threading.Tasks;
using System;

namespace AiForms.Dialogs.Droid;

public class LoadingPlatformDialog : AndroidX.Fragment.App.DialogFragment
{
    LoadingView _loadingView;
    ViewGroup _contentView;
    public TaskCompletionSource<bool> DestroyTcs { get; private set; }

    public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
    {
        base.OnCreateDialog(savedInstanceState);

        System.Diagnostics.Debug.WriteLine("OnCreateDialog");

        var payload = Arguments.GetSerializable(LoadingDialogPayload.PayloadKey) as LoadingDialogPayload;

        _loadingView = payload.LoadingView;
        _contentView = payload.ContentView;
        var isShowTcs = payload.IsShownTcs;

        payload.Dispose();
       
        var dialog = DialogHelpers.CreateFullScreenTransparentDialog(_contentView);

        Cancelable = false;
        dialog.SetCancelable(false);
        dialog.SetCanceledOnTouchOutside(false);

        DestroyTcs = new TaskCompletionSource<bool>();

        try
        {

            return dialog;
        }
        finally
        {
            isShowTcs.SetResult(true);
        }
    }

    public override void OnStart()
    {
        base.OnStart();

        _loadingView?.RunPresentationAnimation();
    }

    public override void OnDestroyView()
    {
        base.OnDestroyView();

        _loadingView = null;
        _contentView = null;
        Dialog?.Dispose();
        DestroyTcs.SetResult(true);
        DestroyTcs = null;

        System.Diagnostics.Debug.WriteLine("OnDestroyView");
    }
}
