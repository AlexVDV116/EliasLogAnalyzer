using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace EliasLogAnalyzer.MAUI.Resources.Controls;

public class CustomCheckBox : ContentView
{
    private readonly BoxView _checkBox;

    private static readonly BindableProperty IsCheckedProperty =
        BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(CustomCheckBox), false, propertyChanged: OnIsCheckedChanged);

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public CustomCheckBox()
    {
        _checkBox = new BoxView
        {
            WidthRequest = 20,
            HeightRequest = 20,
            Color = Colors.White,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center
        };

        _checkBox.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(() => IsChecked = !IsChecked)
        });
        
        var frame = new Frame
        {
            WidthRequest = 20,
            HeightRequest = 20,
            CornerRadius = 0,
            BorderColor = Color.FromArgb("FF02A3A4"),
            Content = _checkBox,
            Padding = 0
        };

        Content = frame;
    }

    private static void OnIsCheckedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var checkBox = (CustomCheckBox)bindable;
        checkBox._checkBox.Color = (bool)newValue ? Color.FromArgb("FF02A3A4") : Color.FromArgb("FFFFFFFF");
    }
}