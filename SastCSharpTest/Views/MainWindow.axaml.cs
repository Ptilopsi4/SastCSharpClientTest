using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using SastCSharpTest.Helper;
using SastCSharpTest.Services;

namespace SastCSharpTest.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            NavigationService.ContentFrame = ContentFrame;
            ContentFrame.Content = new WelcomeView();

            UpdateNavigationButtonsColor();
        }

        public void UpdateNavigationButtonsColor()
        {
            Color textColor = ThemeHelper.IsDarkTheme() ? Colors.White : Colors.Black;

            if (WelcomeButton != null)
                WelcomeButton.Foreground = new SolidColorBrush(textColor);

            if (FriendButton != null)
                FriendButton.Foreground = new SolidColorBrush(textColor);

            if (SettingsButton != null)
                SettingsButton.Foreground = new SolidColorBrush(textColor);
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string tag)
            {
                switch (tag)
                {
                    case "WelcomeView":
                        NavigationService.Navigate(typeof(WelcomeView));
                        break;
                    case "FriendView":
                        NavigationService.Navigate(typeof(FriendView));
                        break;
                    case "SettingsView":
                        NavigationService.Navigate(typeof(SettingsView));
                        break;
                }
                UpdateNavigationButtonsColor();
            }
        }
    }
}