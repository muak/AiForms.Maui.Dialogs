using System;

namespace AiForms.Dialogs;

public partial class Loading: ILoading
{
    static readonly Lazy<ILoading> Implementation = new Lazy<ILoading>(() => new Loading(), System.Threading.LazyThreadSafetyMode.PublicationOnly);
    public static ILoading Instance => Implementation.Value;

}

