using AiForms.Dialogs;
using Sample.ViewModels;

namespace Sample.Views.Dialogs;

public partial class VmDialog : DialogView
{
    public VmDialog()
    {
        InitializeComponent();
    }

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        DialogNotifier.Complete(new VmTestResult { Name = "Abc" });
    }

}
