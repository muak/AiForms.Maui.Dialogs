using System;
namespace AiForms.Dialogs
{
    public partial class Toast
    {
        public void Show<TView>(object viewModel = null) where TView : ToastView
        {
            throw new NotImplementedException();
        }

        public void Show(ToastView view, object viewModel = null)
        {
            throw new NotImplementedException();
        }
    }
}

