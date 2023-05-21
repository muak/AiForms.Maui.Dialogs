namespace AiForms.Dialogs;

public interface IReusableDialog: IDisposable
{
    Task<bool> ShowAsync();
    Task<TResult> ShowResultAsync<TResult>();
}
