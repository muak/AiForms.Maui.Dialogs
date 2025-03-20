using AiForms.Dialogs;
using Reactive.Bindings;
using Sample.ViewModels.Dialogs;

namespace Sample.ViewModels;

public class SurveyPageViewModel:BindableBase, IPageLifecycleAware
{
    public ReactiveCommand ShowDialogCommand { get; } = new ReactiveCommand();
    public SurveyPageViewModel()
    {
        var loading = AiForms.Dialogs.Loading.Instance;
        ShowDialogCommand.Subscribe(async _ =>
        {
            // var custom = loading.CreateFromModel<VmLoadingViewModel>();
            // custom.Show();
            //
            // await Task.Delay(5000);
            //
            // await custom.Hide();
            var ret = await AiForms.Dialogs.Dialog.Instance.ShowResultFromModelAsync<VmDialogViewModel,int, VmTestResult>(50);
        });
    }

    public void OnAppearing()
    {
    }

    public void OnDisappearing()
    {
        
    }
}
