namespace AiForms.Dialogs;

public interface IReusableDialog: IDisposable
{
    Task<bool> ShowAsync();
}
