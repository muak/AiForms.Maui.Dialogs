using System;
namespace AiForms.Dialogs;

public partial class Loading
{
    public IReusableLoading Create<TView>(object viewModel = null) where TView : LoadingView
    {
        throw new NotImplementedException();
    }

    public IReusableLoading Create(LoadingView view, object viewModel = null)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void Hide()
    {
        throw new NotImplementedException();
    }

    public void SetMessage(string message)
    {
        throw new NotImplementedException();
    }

    public void Show(string message = null, bool isCurrentScope = false)
    {
        throw new NotImplementedException();
    }

    public Task StartAsync(Func<IProgress<double>, Task> action, string message = null, bool isCurrentScope = false)
    {
        throw new NotImplementedException();
    }
}

