using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SastCSharpTest.Helper;
using SastCSharpTest.Services;
using SastCSharpTest.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SastCSharpTest.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<string> themes = new() { "明亮", "黑暗", "跟随系统设置", "亚克力" };

    [ObservableProperty]
    private string selectedTheme;

    public ICommand HelpCommand { get; }

    public SettingsViewModel()
    {
        if (ThemeHelper.IsAcrylicMode)
        {
            SelectedTheme = "亚克力";
        }
        else
        {
            var currentTheme = ThemeHelper.RootTheme;
            SelectedTheme = GetThemeDisplayName(currentTheme);
        }
        
        HelpCommand = new RelayCommand(() => NavigationService.Navigate(typeof(MoreInfoView)));
    }

    partial void OnSelectedThemeChanged(string value)
    {
        SwitchTheme(value);
    }

    private void SwitchTheme(string theme)
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = desktop.MainWindow as MainWindow;
            if (mainWindow == null) return;

            switch (theme)
            {
                case "黑暗":
                    ThemeHelper.SetTheme(ThemeVariant.Dark);
                    UpdateWindowAppearance(mainWindow, ThemeVariant.Dark);
                    break;
                case "明亮":
                    ThemeHelper.SetTheme(ThemeVariant.Light);
                    UpdateWindowAppearance(mainWindow, ThemeVariant.Light);
                    break;
                case "跟随系统设置":
                    ThemeHelper.SetTheme(ThemeVariant.Default);
                    UpdateWindowAppearance(mainWindow, ThemeVariant.Default);
                    break;
                case "亚克力":
                    ThemeHelper.SetAcrylicMode();
                    var textColor = ThemeHelper.IsDarkTheme() ? Colors.White : Colors.Black;
                    UpdateNavigationButtonStyle(mainWindow, textColor);
                    break;
            }

            mainWindow.UpdateNavigationButtonsColor();
        }
    }

    private void UpdateWindowAppearance(MainWindow mainWindow, ThemeVariant theme)
    {
        bool isDark = theme == ThemeVariant.Dark ||
                     (theme == ThemeVariant.Default && ThemeHelper.IsDarkTheme());

        if (isDark)
        {
            mainWindow.Background = new SolidColorBrush(Color.Parse("#1E1E1E"));
            mainWindow.Foreground = new SolidColorBrush(Colors.White);
            UpdateNavigationButtonStyle(mainWindow, Colors.White);
        }
        else
        {
            mainWindow.Background = new SolidColorBrush(Colors.White);
            mainWindow.Foreground = new SolidColorBrush(Colors.Black);
            UpdateNavigationButtonStyle(mainWindow, Colors.Black);
        }
    }

    private void UpdateNavigationButtonStyle(MainWindow mainWindow, Color textColor)
    {
        // 更新导航按钮的前景色
        if (mainWindow.FindControl<Button>("WelcomeButton") is Button welcomeButton)
            welcomeButton.Foreground = new SolidColorBrush(textColor);

        if (mainWindow.FindControl<Button>("FriendButton") is Button friendButton)
            friendButton.Foreground = new SolidColorBrush(textColor);

        if (mainWindow.FindControl<Button>("SettingsButton") is Button settingsButton)
            settingsButton.Foreground = new SolidColorBrush(textColor);
    }

    private string GetThemeDisplayName(ThemeVariant theme)
    {
        return theme.ToString() switch
        {
            "Light" => "明亮",
            "Dark" => "黑暗",
            _ => "跟随系统设置"
        };
    }
}