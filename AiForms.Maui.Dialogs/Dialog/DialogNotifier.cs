namespace AiForms.Dialogs;

public interface IDialogNotifier
{
    void Complete();
    void Cancel();
    void Complete<T>(T result);
}

public class DialogNotifier:IDialogNotifier
{
    internal event EventHandler<DialogNotifierArgs> Completed;
    internal event EventHandler<DialogNotifierArgs> Canceled;

    public void Complete()
    {
        Completed?.Invoke(this, new DialogNotifierArgs(true));
    }

    public void Complete<T>(T result)
    {
        Completed?.Invoke(this, new DialogNotifierArgs(result));
    }

    public void Cancel()
    {
        Canceled?.Invoke(this, new DialogNotifierArgs(false));
    }
}

public class DialogNotifierArgs: EventArgs
{
    public object Result { get; set; }

    public DialogNotifierArgs() { }
    public DialogNotifierArgs(object result)
    {
        Result = result;
    }
}
