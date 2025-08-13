using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Newtonsoft.Json;
using SastCSharpTest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SastCSharpTest.Views
{
    /// <summary>
    /// An Avalonia UserControl that can be used on its own or navigated to within a ContentControl.
    /// </summary>
    public partial class FriendView : UserControl
    {
        private List<Friend> friends = new List<Friend>();

        public FriendView()
        {
            InitializeComponent();
            SetComboBox();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void SetComboBox()
        {
            string jsonData = File.ReadAllText(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "res/data.json"
                )
            );
            friends = JsonConvert.DeserializeObject<Friend[]>(jsonData)!.ToList();

            var friendOption = this.FindControl<ComboBox>("FriendOption");
            friendOption.ItemsSource = friends.Select(friend => friend.Name);
            friendOption.SelectedIndex = 0;
        }

        private async void SetImgAsync(string filePath)
        {
            try
            {
                using var fileStream = File.OpenRead(filePath);
                var bitmap = new Bitmap(fileStream);

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var imageBox = this.FindControl<Image>("ImageBox");
                    imageBox.Source = bitmap;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading image: {ex.Message}");
            }
        }

        private void SetTextBoxEnabled(bool isEnabled)
        {
            var nameBox = this.FindControl<TextBox>("NameBox");
            var descriptionBox = this.FindControl<TextBox>("DescriptionBox");

            nameBox.IsEnabled = isEnabled;
            descriptionBox.IsEnabled = isEnabled;
        }

        /*
         *
         * 事件处理函数
         *
         */

        private void ChooseFriend(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            var friendName = e.AddedItems[0]?.ToString();
            if (string.IsNullOrEmpty(friendName)) return;

            var friend = friends.First(f => f.Name == friendName);

            var nameBox = this.FindControl<TextBox>("NameBox");
            var descriptionBox = this.FindControl<TextBox>("DescriptionBox");
            var editButton = this.FindControl<Button>("EditButton");

            nameBox.Text = friend.Name;
            descriptionBox.Text = friend.Description;
            editButton.IsVisible = true;
            SetTextBoxEnabled(false);

            // 构建图像路径并加载
            string imagePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                friend.ImgUrl
            );
            SetImgAsync(imagePath);
        }

        private void EditFriend(object sender, RoutedEventArgs e)
        {
            SetTextBoxEnabled(true);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            var mainGrid = this.FindControl<Grid>("MainGrid");

            ApplyWideLayout(mainGrid);

            if (this.GetVisualRoot() is Window window)
            {
                window.PropertyChanged += (sender, args) =>
                {
                    if (args.Property == Window.ClientSizeProperty)
                    {
                        var newSize = window.ClientSize;
                        if (newSize.Width >= 800)
                        {
                            ApplyWideLayout(mainGrid);
                        }
                        else
                        {
                            ApplyNarrowLayout(mainGrid);
                        }
                    }
                };
            }
        }

        private void ApplyWideLayout(Grid grid)
        {
            grid.ColumnDefinitions.Clear();
            grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));

            grid.RowDefinitions.Clear();
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            grid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));

            var imageBox = this.FindControl<Image>("ImageBox");
            if (imageBox != null)
            {
                Grid.SetColumn(imageBox, 1);
                Grid.SetRow(imageBox, 1);
            }
        }

        private void ApplyNarrowLayout(Grid grid)
        {
            grid.ColumnDefinitions.Clear();
            grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));
            grid.ColumnDefinitions.Add(new ColumnDefinition(0, GridUnitType.Pixel));

            grid.RowDefinitions.Clear();
            grid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            var imageBox = this.FindControl<Image>("ImageBox");
            if (imageBox != null)
            {
                Grid.SetColumn(imageBox, 0);
                Grid.SetRow(imageBox, 0);
            }
        }
    }
}