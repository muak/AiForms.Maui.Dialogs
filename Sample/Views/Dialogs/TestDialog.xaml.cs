using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AiForms.Dialogs;

namespace Sample.Views;

public partial class TestDialog : DialogView
{
    public TestDialog()
    {
        InitializeComponent();
    }

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            //(this.Parent as Page).DisplayAlert("", "Hoge","OK");
            //(this.Parent as Page).DisplayAlert("", "Fuga", "OK");
            AiForms.Dialogs.Dialog.Instance.ShowAsync<DialogTestView>();
        });
        //DialogNotifier.Complete();
                 
    }

    public override void Destroy()
    {
        
    }
}
