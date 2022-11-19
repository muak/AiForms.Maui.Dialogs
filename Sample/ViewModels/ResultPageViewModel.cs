using System;
using Prism.Navigation;
using System.Collections.Generic;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Sample.ViewModels;

public class ResultPageViewModel:BindableBase, IInitializeAsync
{
    public ObservableCollection<TestItem> ItemsSource { get; set; }
    public ResultPageViewModel()
    {
    }

    public Task InitializeAsync(INavigationParameters parameters)
    {
        ItemsSource = new ObservableCollection<TestItem>(
            parameters.GetValue<List<TestItem>>("item")
        );
        RaisePropertyChanged(nameof(ItemsSource));

        return Task.CompletedTask;
    }
}
