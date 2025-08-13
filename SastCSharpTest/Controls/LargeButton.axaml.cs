using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SastCSharpTest.Helper;
using System;

namespace SastCSharpTest.Controls;

public sealed partial class LargeButton : UserControl
{
    public static readonly StyledProperty<string> MainTitleProperty =
        AvaloniaProperty.Register<LargeButton, string>(nameof(MainTitle), defaultValue: "");

    public string MainTitle
    {
        get => GetValue(MainTitleProperty);
        set => SetValue(MainTitleProperty, value);
    }

    public static readonly StyledProperty<string> SubTitleProperty =
        AvaloniaProperty.Register<LargeButton, string>(nameof(SubTitle), defaultValue: "");

    public string SubTitle
    {
        get => GetValue(SubTitleProperty);
        set => SetValue(SubTitleProperty, value);
    }

    public static readonly StyledProperty<Control> IconProperty =
        AvaloniaProperty.Register<LargeButton, Control>(nameof(Icon), defaultValue: null);

    public Control Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public event EventHandler<RoutedEventArgs>? Click;

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Click?.Invoke(this, new RoutedEventArgs());
    }

    public LargeButton()
    {
        InitializeComponent();
        UpdateTheme();

        // 订阅ActualThemeVariantChanged事件以响应主题变化
        if (Application.Current != null)
        {
            Application.Current.ActualThemeVariantChanged += (s, e) => UpdateTheme();
        }
    }

    private void UpdateTheme()
    {
        bool isDarkTheme = ThemeHelper.IsDarkTheme();

        Classes.Remove("LightTheme");
        Classes.Remove("DarkTheme");

        if (isDarkTheme)
        {
            Classes.Add("DarkTheme");
        }
        else
        {
            Classes.Add("LightTheme");
        }
    }
}