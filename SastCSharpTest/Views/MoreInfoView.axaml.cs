using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SastCSharpTest.Views
{
    public partial class MoreInfoView : UserControl
    {
        public MoreInfoView()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void InputElement_OnTapped(object? sender, TappedEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                string url = textBlock.Tag as string;
                if (!string.IsNullOrEmpty(url))
                {
                    try
                    {
                        var uri = new Uri(url);
                        await OpenBrowser(uri);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"无法打开URL: {ex.Message}");
                    }
                }
            }
        }

        private async Task OpenBrowser(Uri uri)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = uri.AbsoluteUri,
                    UseShellExecute = true
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", uri.AbsoluteUri);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", uri.AbsoluteUri);
            }
        }
    }
}