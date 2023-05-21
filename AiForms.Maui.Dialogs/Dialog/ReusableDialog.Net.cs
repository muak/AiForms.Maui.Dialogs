using System;

namespace AiForms.Dialogs
{
    public class ReusableDialog: IReusableDialog
    {
        public ReusableDialog(DialogView view)
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ShowAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ShowResultAsync<TResult>()
        {
            throw new NotImplementedException();
        }
    }
}

