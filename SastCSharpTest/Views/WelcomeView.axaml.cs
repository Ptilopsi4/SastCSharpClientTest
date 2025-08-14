using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using SastCSharpTest.Services;
using Avalonia;
using Avalonia.Threading;

namespace SastCSharpTest.Views
{
    public partial class WelcomeView : UserControl
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
                var animationStyles = Resources["AnimationStyles"] as Styles;
                if (animationStyles != null)
                {
                    Styles.Add(animationStyles);
                }
        }

        private void LargeButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(MoreInfoView));
        }
    }
}