using AiForms.Dialogs;

namespace Sample.Views.Dialogs;

public partial class BindableLayoutDialog : DialogView
{
    public BindableLayoutDialog()
    {
        InitializeComponent();
    }

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        DialogNotifier.Complete();
    }
}
