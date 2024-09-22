using System;
using AiForms.Dialogs;
using Reactive.Bindings;

namespace Sample.ViewModels.Dialogs;

public class VmDialogViewModel: IDialogViewModel<int>
{
    public string Title { get; set; }
    public ReactivePropertySlim<int> Number { get; } = new();

    public VmDialogViewModel()
    {
    }

    public async Task DialogInitializeAsync(int parameter)
    {
        await Task.Delay(100);

        Number.Value = parameter;        
    }
}

