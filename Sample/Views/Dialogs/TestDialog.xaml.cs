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
        this.HeightRequest += 50;
    }

    public override void Destroy()
    {
        
    }
}
