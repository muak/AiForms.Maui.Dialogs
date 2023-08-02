using System;
using AiForms.Dialogs;
using Reactive.Bindings;
using Sample.ViewModels.Dialogs;
using Sample.Views.Dialogs;

namespace Sample.ViewModels
{
    public class VmTestViewModel
    {
        public ReactiveCommand ShowVmCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ShowVmResultCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ShowViewResultCommand { get; } = new ReactiveCommand();
        public ReactiveCommand ShowVmTypeCommand { get; } = new ReactiveCommand();

        public VmTestViewModel()
        {
            ShowVmCommand.Subscribe(async _ =>
            {
                await Dialog.Instance.ShowAsync(new VmDialogViewModel { Title = "hoge" });
            });

            ShowVmResultCommand.Subscribe(async _ =>
            {
                var ret = await Dialog.Instance.ShowResultAsync<VmTestResult>(new VmDialogViewModel { Title = "hoge" });
            });

            ShowViewResultCommand.Subscribe(async _ =>
            {
                var ret = await Dialog.Instance.ShowResultAsync<VmDialog,VmTestResult>(new VmDialogViewModel { Title = "hoge" });
            });

            ShowVmTypeCommand.Subscribe(async _ =>
            {
                await Dialog.Instance.ShowFromModelAsync<VmDialogViewModel>();
            });
        }
    }

    public class VmTestResult
    {
        public string Name { get; set; }
    }
}

