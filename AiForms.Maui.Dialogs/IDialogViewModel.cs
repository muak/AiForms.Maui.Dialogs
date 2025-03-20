using System;
namespace AiForms.Dialogs;

public interface IDialogViewModel<T>: IDialogViewModelDestroy
{
    /// <summary>
    /// ViewModel Initialize
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    Task DialogInitializeAsync(T parameter);
}

public interface IDialogViewModelDestroy
{
    /// <summary>
    /// ViewModel Cleanup (Optional)
    /// </summary>
    void Destroy()
    {
    }
}

