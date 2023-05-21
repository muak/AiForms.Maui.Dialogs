namespace AiForms.Dialogs;

public static class Configurations
{
    static LoadingConfig _LoadingConfig;

    public static LoadingConfig LoadingConfig
    {
        get { return _LoadingConfig ??= new LoadingConfig(); }
        set { _LoadingConfig = value; }
    }

    internal static Func<Type, object> Resolve = t => new object();
    internal static Func<Type, Type> ViewTypeGetter = t => t;

    /// <summary>
    /// IocContainer Settings
    /// </summary>
    /// <param name="viewTypeGetter">Function that takes a ViewModel type as an argument and returns a View type</param>
    /// <param name="viewResolver">Function to return an instance from a View type</param>
    public static void SetIocConfig(Func<Type,Type> viewTypeGetter,  Func<Type,object> viewResolver = null)
    {
        Resolve = viewResolver;
        ViewTypeGetter = viewTypeGetter;
    }
}
