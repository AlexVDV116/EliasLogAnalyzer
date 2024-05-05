namespace EliasLogAnalyzer.MAUI.Resources.Controls;

public class CustomCheckBox : ContentView
{
    private readonly BoxView checkBox;
    private readonly Frame frame;
    
    public static readonly BindableProperty IsCheckedProperty =
        BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(CustomCheckBox), false, propertyChanged: OnIsCheckedChanged);

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public CustomCheckBox()
    {
        checkBox = new BoxView
        {
            WidthRequest = 20,
            HeightRequest = 20,
            Color = Colors.White,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        checkBox.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => IsChecked = !IsChecked)
        });
        
        frame = new Frame
        {
            WidthRequest = 20,
            HeightRequest = 20,
            CornerRadius = 2,
            BorderColor = Color.FromArgb("FF02A3A4"),
            Content = checkBox,
            Padding = 2
        };

        Content = frame;
    }

    private static void OnIsCheckedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var checkBox = (CustomCheckBox)bindable;
        checkBox.checkBox.Color = (bool)newValue ? Color.FromArgb("FF02A3A4") : Color.FromArgb("FFFFFFFF");
    }
}