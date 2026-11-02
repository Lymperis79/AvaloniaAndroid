using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MyAvaloniaApp;

public partial class MainView : UserControl
{
    private NativeWebView? _browser;

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

        try
        {
            EnsureBrowser();
            UrlBox.Text = uri.ToString();
            _browser!.IsVisible = true;
            _browser.Source = uri;
            StatusText.Text = "Loading...";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Could not initialize browser: {ex.Message}";
        }
    }

    private void EnsureBrowser()
    {
        if (_browser is not null)
            return;

        _browser = new NativeWebView
        {
            Source = new Uri("about:blank"),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
        };
        _browser.NavigationCompleted += OnNavigationCompleted;
        BrowserHost.Children.Add(_browser);
    }

    private void OnBackClicked(object? sender, RoutedEventArgs e)
    {
        if (_browser is null || !_browser.GoBack())
            StatusText.Text = "No previous page.";
    }

    private void OnForwardClicked(object? sender, RoutedEventArgs e)
    {
        if (_browser is null || !_browser.GoForward())
            StatusText.Text = "No next page.";
    }

    private void OnReloadClicked(object? sender, RoutedEventArgs e)
    {
        if (_browser is null || !_browser.Refresh())
            StatusText.Text = "The page cannot be refreshed yet.";
    }

    private void OnCloseClicked(object? sender, RoutedEventArgs e)
    {
        if (_browser is not null)
            _browser.IsVisible = false;

        StatusText.Text = "Browser closed.";
    }

    private void OnNavigationCompleted(object? sender, WebViewNavigationCompletedEventArgs e)
    {
        StatusText.Text = e.IsSuccess ? "Page loaded." : "Could not load the page.";
    }
}
