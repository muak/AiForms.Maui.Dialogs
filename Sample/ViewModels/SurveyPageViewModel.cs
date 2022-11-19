using Reactive.Bindings;

namespace Sample.ViewModels;

public class SurveyPageViewModel:BindableBase
{
    public ReactiveCommand ShowDialogCommand { get; } = new ReactiveCommand();
    public SurveyPageViewModel()
    {
        var dialog = AiForms.Dialogs.Dialog.Instance;
        ShowDialogCommand.Subscribe(async _ =>
        {
            var ret = await dialog.ShowAsync<Views.TestDialog>();
            if (ret)
            {
                //await dialog.ShowAsync<DialogTestView>();
            }
        });
    }
}
