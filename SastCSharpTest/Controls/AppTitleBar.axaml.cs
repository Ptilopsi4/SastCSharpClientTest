using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System;
using System.Threading.Tasks;

namespace SastCSharpTest.Controls
{
    public partial class AppTitleBar : UserControl
    {
        private bool _isCompact = false;
        private bool _isActive = true;

        public AppTitleBar()
        {
            InitializeComponent();

            PointerPressed += OnPointerPressed;
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var window = this.GetVisualRoot() as Window;
            window?.BeginMoveDrag(e);
        }

        public async Task SetCompactAsync(bool compact)
        {
            _isCompact = compact;
            RootGrid.Margin = compact ? new Thickness(45, 0, 0, 0) : new Thickness(0);

            var titleStackPanel = TitleStackPanel;
            var titleTranslation = titleStackPanel.RenderTransform as TranslateTransform;

            if (titleTranslation == null)
            {
                return;
            }

            var anim = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(220),
                Easing = new ExponentialEaseOut(),

                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0),
                        Setters =
                        {
                            new Setter
                            {
                                Property = TranslateTransform.XProperty,
                                Value = compact ? 0 : 45
                            }
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1),
                        Setters =
                        {
                            new Setter
                            {
                                Property = TranslateTransform.XProperty,
                                Value = compact ? 45 : 0
                            }
                        }
                    }
                }
            };

            await anim.RunAsync(titleTranslation);
        }

        public void SetActive(bool isActive)
        {
            _isActive = isActive;
            TitleTextBlock.Foreground = isActive
                ? Brushes.Black
                : new SolidColorBrush(Color.Parse("#8E8E8E"));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        private void Minimize_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var window = this.GetVisualRoot() as Window;
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }

        private void Maximize_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var window = this.GetVisualRoot() as Window;
            if (window != null)
            {
                window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        }

        private void Close_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var window = this.GetVisualRoot() as Window;
            window?.Close();
        }
    }
}
