using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MyAvaloniaApp;

public partial class MainView : UserControl
{
    public MainView() => InitializeComponent();

    private void OnOpenClicked(object? sender, RoutedEventArgs e)
    {
        var text = UrlBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            StatusText.Text = "Enter a URL.";
            return;
        }

        if (!text.Contains("://", StringComparison.Ordinal))
            text = "https://" + text;

        if (!Uri.TryCreate(text, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            StatusText.Text = "Please enter a valid http or https URL.";
            return;
        }

        UrlBox.Text = uri.ToString();
        Browser.IsVisible = true;
        Browser.Source = uri;
        StatusText.Text = "Loading...";
    }

    private void OnBackClicked(object? sender, RoutedEventArgs e)
    {
        if (!Browser.GoBack())
            StatusText.Text = "No previous page.";
    }

    private void OnForwardClicked(object? sender, RoutedEventArgs e)
    {
        if (!Browser.GoForward())
            StatusText.Text = "No next page.";
    }

    private void OnReloadClicked(object? sender, RoutedEventArgs e)
    {
        if (!Browser.Refresh())
            StatusText.Text = "The page cannot be refreshed yet.";
    }

    private void OnCloseClicked(object? sender, RoutedEventArgs e)
    {
        Browser.IsVisible = false;
        StatusText.Text = "Browser closed.";
    }

    private void OnNavigationCompleted(object? sender, WebViewNavigationCompletedEventArgs e)
    {
        StatusText.Text = e.IsSuccess ? "Page loaded." : "Could not load the page.";
    }
}
