using AiForms.Dialogs;
using Android.Views;
using Java.IO;

namespace AiForms.Dialogs.Droid;

public class ExtraDialogPayload : Java.Lang.Object, ISerializable
{
    public DialogView DialogView { get; set; }
    public ViewGroup ContentView { get; set; }

    public ExtraDialogPayload(DialogView dialogView, ViewGroup contentView)
    {
        DialogView = dialogView;
        ContentView = contentView;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DialogView = null;
            ContentView = null;
        }
        base.Dispose(disposing);
    }
}
