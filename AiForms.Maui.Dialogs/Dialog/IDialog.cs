namespace AiForms.Dialogs;

public interface IDialog
{
    Task<bool> ShowAsync<TView>(object viewModel = null) where TView : DialogView;
    Task<bool> ShowAsync<TViewModel>();
    Task<bool> ShowAsync(DialogView view, object viewModel = null);
    Task<bool> ShowAsync(object viewModel);
    Task<TResult> ShowResultAsync<TResult>(object viewModel);
    Task<TResult> ShowResultAsync<TView, TResult>(object viewModel = null) where TView : DialogView;
    IReusableDialog Create<TView>(object viewModel = null) where TView : DialogView;
    IReusableDialog Create(DialogView view, object viewModel = null);

}
