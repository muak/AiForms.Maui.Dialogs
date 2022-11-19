namespace AiForms.Dialogs;

public class ToastView : ExtraView
{
    public static BindableProperty DurationProperty =
        BindableProperty.Create(
            nameof(Duration),
            typeof(int),
            typeof(ToastView),
            1500, // 1-3500 
            defaultBindingMode: BindingMode.OneWay
        );

    public int Duration
    {
        get { return (int)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }
}
