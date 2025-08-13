using Avalonia;
using Avalonia.Controls;
using SastCSharpTest.Helper;
using SastCSharpTest.ViewModels;

namespace SastCSharpTest.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel();

        UpdateTheme();

        // 订阅主题变化事件
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