using System;
namespace Sample.ViewModels.Dialogs;

public class BindableLayoutDialogViewModel
{
    public List<string> ItemsSource { get; }

    public BindableLayoutDialogViewModel()
    {
        ItemsSource = new List<string>
        {
            "AAA",
            "BBB",
            "CCC",
            "DDD",
            "EEE",
            "FFF"
        };
    }
}

