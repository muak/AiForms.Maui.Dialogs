using System;
using System.Threading.Tasks;
using AiForms.Dialogs;
using Android.Runtime;
using Android.Views;
using Java.IO;

namespace AiForms.Dialogs.Droid;

public class LoadingDialogPayload : Java.Lang.Object, ISerializable
{
    public static readonly string PayloadKey = "loadingDialogPayload";
    public LoadingView LoadingView { get; set; }
    public ViewGroup ContentView { get; set; }
    public TaskCompletionSource<bool> IsShownTcs { get; set; }

    public LoadingDialogPayload(ViewGroup contentView, TaskCompletionSource<bool> tcs, LoadingView loadingView = null)
    {
        LoadingView = loadingView;
        ContentView = contentView;
        IsShownTcs = tcs;
    }

    public LoadingDialogPayload(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
    {
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            LoadingView = null;
            ContentView = null;
            IsShownTcs = null;
        }
        base.Dispose(disposing);
    }
}
