using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using SastCSharpTest.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SastCSharpTest.Helper;

/// <summary>
/// Avalonia 主题管理助手类
/// </summary>
public static class ThemeHelper
{
    private const string SelectedAppThemeKey = "SelectedAppTheme";
    private const string IsAcrylicModeKey = "IsAcrylicMode";

    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "SastCSharpTest",
        "settings.json"
    );

    public static Window? AppWindow;

    public static bool IsAcrylicMode { get; private set; }

    /// <summary>
    /// 获取当前应用的实际主题
    /// </summary>
    public static ThemeVariant ActualTheme
    {
        get
        {
            if (Application.Current is App app)
            {
                return app.RequestedThemeVariant ?? ThemeVariant.Default;
            }
            return ThemeVariant.Default;
        }
    }

    /// <summary>
    /// 获取或设置应用主题（带本地存储持久化）
    /// </summary>
    public static ThemeVariant RootTheme
    {
        get
        {
            if (Application.Current is App app)
            {
                return app.RequestedThemeVariant ?? ThemeVariant.Default;
            }
            return ThemeVariant.Default;
        }
        set
        {
            if (Application.Current is App app)
            {
                app.RequestedThemeVariant = value;
            }

            SaveThemeSetting(value.ToString());
        }
    }

    /// <summary>
    /// 初始化主题设置
    /// </summary>
    public static void Initialize()
    {
        string? savedTheme = LoadThemeSetting();
        bool isAcrylic = LoadAcrylicModeSetting();
        
        IsAcrylicMode = isAcrylic;

        if (isAcrylic)
        {
            if (Application.Current is App app)
            {
                app.RequestedThemeVariant = ThemeVariant.Default;
            }

            ApplyThemeToWindow();
        }
        else if (!string.IsNullOrEmpty(savedTheme))
        {
            try
            {
                var themeVariant = savedTheme switch
                {
                    "Light" => ThemeVariant.Light,
                    "Dark" => ThemeVariant.Dark,
                    _ => ThemeVariant.Default
                };
                
                if (Application.Current is App app)
                {
                    app.RequestedThemeVariant = themeVariant;
                }

                ApplyThemeToWindow();
            }
            catch
            {
                if (Application.Current is App app)
                {
                    app.RequestedThemeVariant = ThemeVariant.Default;
                }
            }
        }
    }

    /// <summary>
    /// 应用当前主题到窗口
    /// </summary>
    private static void ApplyThemeToWindow()
    {
        if (AppWindow is MainWindow mainWindow)
        {
            bool isDark = IsDarkTheme();
            
            if (IsAcrylicMode)
            {
                mainWindow.Background = Avalonia.Media.Brushes.Transparent;
                var textColor = isDark ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Black;
                mainWindow.Foreground = new Avalonia.Media.SolidColorBrush(textColor);
            }
            else if (isDark)
            {
                mainWindow.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#1E1E1E"));
                mainWindow.Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.White);
            }
            else
            {
                mainWindow.Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.White);
                mainWindow.Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Black);
            }
            
            mainWindow.UpdateNavigationButtonsColor();
        }
    }

    /// <summary>
    /// 判断当前是否为深色主题
    /// </summary>
    public static bool IsDarkTheme()
    {
        var currentTheme = ActualTheme;

        if (currentTheme == ThemeVariant.Default)
        {
            return IsSystemDarkTheme();
        }

        return currentTheme == ThemeVariant.Dark;
    }

    /// <summary>
    /// 切换主题
    /// </summary>
    public static void ToggleTheme()
    {
        RootTheme = IsDarkTheme() ? ThemeVariant.Light : ThemeVariant.Dark;
    }

    /// <summary>
    /// 设置特定主题
    /// </summary>
    public static void SetTheme(ThemeVariant theme)
    {
        IsAcrylicMode = false;
        SaveAcrylicModeSetting(false);
        RootTheme = theme;
    }

    /// <summary>
    /// 设置亚克力主题模式
    /// </summary>
    public static void SetAcrylicMode()
    {
        IsAcrylicMode = true;
        SaveAcrylicModeSetting(true);

        if (Application.Current is App app)
        {
            app.RequestedThemeVariant = ThemeVariant.Default;
        }

        if (AppWindow is MainWindow mainWindow)
        {
            mainWindow.Background = Avalonia.Media.Brushes.Transparent;

            var textColor = IsDarkTheme() ? Avalonia.Media.Colors.White : Avalonia.Media.Colors.Black;
            mainWindow.UpdateNavigationButtonsColor();
        }
    }

    /// <summary>
    /// 更新UI元素以匹配当前主题
    /// </summary>
    public static void UpdateUIForCurrentTheme()
    {
        if (AppWindow is MainWindow mainWindow)
        {
            mainWindow.UpdateNavigationButtonsColor();
        }
    }

    /// <summary>
    /// 检查系统是否使用深色主题
    /// </summary>
    private static bool IsSystemDarkTheme()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                var value = key?.GetValue("AppsUseLightTheme");
                return value is int intValue && intValue == 0;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 保存主题设置到本地文件
    /// </summary>
    private static void SaveThemeSetting(string theme)
    {
        SaveSetting(SelectedAppThemeKey, theme);
    }

    /// <summary>
    /// 保存亚克力模式设置到本地文件
    /// </summary>
    private static void SaveAcrylicModeSetting(bool isAcrylic)
    {
        SaveSetting(IsAcrylicModeKey, isAcrylic);
    }

    /// <summary>
    /// 保存设置到本地文件
    /// </summary>
    private static void SaveSetting(string key, object value)
    {
        try
        {
            var settingsDir = Path.GetDirectoryName(SettingsPath);
            if (!Directory.Exists(settingsDir))
            {
                Directory.CreateDirectory(settingsDir!);
            }

            var settings = new Dictionary<string, object>();

            if (File.Exists(SettingsPath))
            {
                var existingJson = File.ReadAllText(SettingsPath);
                var existingSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(existingJson);
                if (existingSettings != null)
                {
                    settings = existingSettings;
                }
            }

            settings[key] = value;

            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(SettingsPath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"保存设置失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 从本地文件加载主题设置
    /// </summary>
    private static string? LoadThemeSetting()
    {
        var settings = LoadSettings();
        if (settings?.TryGetValue(SelectedAppThemeKey, out var themeValue) == true)
        {
            return themeValue?.ToString();
        }
        return null;
    }

    /// <summary>
    /// 从本地文件加载亚克力模式设置
    /// </summary>
    private static bool LoadAcrylicModeSetting()
    {
        var settings = LoadSettings();
        if (settings?.TryGetValue(IsAcrylicModeKey, out var value) == true)
        {
            if (value is JsonElement element && element.ValueKind == JsonValueKind.True)
            {
                return true;
            }
            if (value is bool boolValue)
            {
                return boolValue;
            }
        }
        return false;
    }

    /// <summary>
    /// 从本地文件加载所有设置
    /// </summary>
    private static Dictionary<string, object>? LoadSettings()
    {
        try
        {
            if (!File.Exists(SettingsPath))
                return null;

            var json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载设置失败: {ex.Message}");
            return null;
        }
    }
}
