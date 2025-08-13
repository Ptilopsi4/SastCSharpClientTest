using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace SastCSharpTest.Services;

internal static class NavigationService
{
    public static ContentControl? ContentFrame { get; set; }

    private static readonly Stack<object> _navigationStack = new();

    public static void Navigate(Type pageType, object? parameter = null)
    {
        if (ContentFrame == null) return;

        try
        {
            var page = Activator.CreateInstance(pageType);
            if (page == null) return;

            if (ContentFrame.Content != null)
            {
                _navigationStack.Push(ContentFrame.Content);
            }

            ContentFrame.Content = page;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
        }
    }

    public static void NavigateBack()
    {
        if (ContentFrame == null || _navigationStack.Count == 0) return;

        try
        {
            var previousPage = _navigationStack.Pop();
            ContentFrame.Content = previousPage;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigate back error: {ex.Message}");
        }
    }

    public static bool CanGoBack => _navigationStack.Count > 0;

    public static void ClearNavigationStack()
    {
        _navigationStack.Clear();
    }
}
