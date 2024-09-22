using System;
namespace AiForms.Dialogs;

public interface IDialogViewModel<T>
{
    Task DialogInitializeAsync(T parameter);
}

