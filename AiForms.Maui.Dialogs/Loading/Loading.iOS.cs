using System;
namespace AiForms.Dialogs;

public partial class Loading
{
    LoadingConfig _config => Configurations.LoadingConfig;

    DefaultLoading _defaultInstance;
    DefaultLoading DefaultInstance => _defaultInstance ??= new DefaultLoading();    

    public Loading()
    {
    }

    public IReusableLoading Create<TView>(object viewModel = null) where TView : LoadingView
    {
        var view = ExtraView.InstanceCreator<TView>.Create();
        return Create(view, viewModel);
    }

    public IReusableLoading Create(LoadingView view, object viewModel = null)
    {
        if (viewModel != null)
        {
            view.BindingContext = viewModel;
        }
        return new ReusableLoading(view);
    }

    public void Show(string message = null, bool isCurrentScope = false)
    {
        if (DefaultInstance.IsRunning)
        {
            return;
        }

        DefaultInstance.Show(message, isCurrentScope);
    }

    public async Task StartAsync(Func<IProgress<double>, Task> action, string message = null, bool isCurrentScope = false)
    {
        await DefaultInstance.StartAsync(action, message, isCurrentScope);
        Hide();
    }


    public void Hide()
    {
        DefaultInstance.Hide();
        if (!_config.IsReusable)
        {
            _defaultInstance.Dispose();
            _defaultInstance = null;
        }
    }

    public void SetMessage(string message)
    {
        if (_defaultInstance != null)
        {
            _defaultInstance.SetMessage(message);
        }
    }

    public void Dispose()
    {
        _defaultInstance?.Dispose();
        _defaultInstance = null;
    }
}

