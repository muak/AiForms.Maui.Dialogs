namespace AiForms.Dialogs;

public class Dialog: IDialog
{
    static readonly Lazy<IDialog> Implementation = new(() => new Dialog(), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    public static IDialog Instance => Implementation.Value;

    public Dialog()
    {
    }

    public IReusableDialog Create<TView>(object viewModel = null) where TView : DialogView
    {
        var view = ExtraView.InstanceCreator<TView>.Create();
        return Create(view, viewModel);
    }

    public IReusableDialog Create(DialogView view, object viewModel = null)
    {
        if (viewModel != null)
        {
            view.BindingContext = viewModel;
        }
        return new ReusableDialog(view);
    }

    public async Task<bool> ShowAsync<TView>(object viewModel = null) where TView : DialogView
    {
        using var dlg = Create<TView>(viewModel);
        return await dlg.ShowAsync();
    }

    public async Task<bool> ShowFromModelAsync<TViewModel>()
    {
        var vm = Configurations.Resolve(typeof(TViewModel));
        return await ShowAsync(vm);
    }

    public async Task<bool> ShowAsync(DialogView view, object viewModel = null)
    {
        using var dlg = Create(view, viewModel);
        return await dlg.ShowAsync();        
    }

    public async Task<bool> ShowAsync(object viewModel)
    {
        var viewType = Configurations.ViewTypeGetter(viewModel.GetType()) ?? throw new KeyNotFoundException("ViewType not found");
        var view = Configurations.Resolve(viewType) as DialogView ?? throw new KeyNotFoundException("View not resolved");
        using var dlg = Create(view, viewModel);

        return await dlg.ShowAsync();
    }

    public async Task<TResult> ShowResultAsync<TView, TResult>(object viewModel = null) where TView : DialogView
    {
        using var dlg = Create<TView>(viewModel);
        return await dlg.ShowResultAsync<TResult>();
    }

    public async Task<TResult> ShowResultAsync<TResult>(object viewModel)
    {
        var viewType = Configurations.ViewTypeGetter(viewModel.GetType()) ?? throw new KeyNotFoundException("ViewType not found");
        var view = Configurations.Resolve(viewType) as DialogView ?? throw new KeyNotFoundException("View not resolved");
        using var dlg = Create(view, viewModel);

        return await dlg.ShowResultAsync<TResult>();
    }
}

