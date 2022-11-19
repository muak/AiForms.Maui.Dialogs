using System;

namespace AiForms.Dialogs;

[Obsolete("Toast will be deprecated.")]
public partial class Toast: IToast
{
    static readonly Lazy<IToast> Implementation = new Lazy<IToast>(() => new Toast(), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    public static IToast Instance => Implementation.Value;

    public Toast()
    {
    }
}

