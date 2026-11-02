using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MyAvaloniaApp;

public partial class MainView : UserControl
{
    private NativeWebView? _browser;
    private readonly string _settingsFile;

    public MainView()
    {
        InitializeComponent();
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _settingsFile = Path.Combine(appData, "MyAvaloniaApp", "saved-webpage.txt");

        try
        {
            if (File.Exists(_settingsFile))
                UrlBox.Text = File.ReadAllText(_settingsFile).Trim();
        }
        catch
        {
            // The settings panel remains usable if saved settings cannot be read.
        }
    }

    private void OnMenuClicked(object? sender, RoutedEventArgs e)
    {
        SettingsPanel.IsVisible = !SettingsPanel.IsVisible;
        if (SettingsPanel.IsVisible)
            UrlBox.Focus();
    }

    private void OnCancelSettingsClicked(object? sender, RoutedEventArgs e)
    {
        SettingsPanel.IsVisible = false;
        StatusText.Text = string.Empty;
    }

    private void OnSavePageClicked(object? sender, RoutedEventArgs e)
    {
        if (!TryGetPageUri(out var uri))
            return;

        try
        {
            var directory = Path.GetDirectoryName(_settingsFile)!;
            Directory.CreateDirectory(directory);
            File.WriteAllText(_settingsFile, uri.ToString());

            SettingsPanel.IsVisible = false;
            NavigateTo(uri);
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Could not save webpage: {ex.Message}";
        }
    }

    private bool TryGetPageUri(out Uri uri)
    {
        uri = null!;
        var text = UrlBox.Text?.Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            StatusText.Text = "Please enter a webpage address.";
            return false;
        }

        if (!text.Contains("://", StringComparison.Ordinal))
            text = "https://" + text;

        if (!Uri.TryCreate(text, UriKind.Absolute, out uri!) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            StatusText.Text = "Please enter a valid http or https address.";
            return false;
        }

        UrlBox.Text = uri.ToString();
        return true;
    }

    private void NavigateTo(Uri uri)
    {
        try
        {
            EnsureBrowser();
            _browser!.IsVisible = true;
            _browser.Source = uri;
            StatusText.Text = "Loading webpage...";
        }
        catch (Exception ex)
        {
            SettingsPanel.IsVisible = true;
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
            ShowStatus("No previous page.");
    }

    private void OnForwardClicked(object? sender, RoutedEventArgs e)
    {
        if (_browser is null || !_browser.GoForward())
            ShowStatus("No next page.");
    }

    private void OnReloadClicked(object? sender, RoutedEventArgs e)
    {
        if (_browser is null || !_browser.Refresh())
            ShowStatus("The page cannot be refreshed yet.");
    }

    private void OnNavigationCompleted(object? sender, WebViewNavigationCompletedEventArgs e)
    {
        ShowStatus(e.IsSuccess ? "Page loaded." : "Could not load the page.");
    }

    private void ShowStatus(string message)
    {
        // Status messages must not reopen the settings panel.
        // The menu button remains available while the webpage is displayed.
        StatusText.Text = message;
    }
}
