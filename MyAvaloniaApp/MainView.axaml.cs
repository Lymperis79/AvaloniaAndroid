using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MyAvaloniaApp;

public partial class MainView : UserControl
{
    public MainView() => InitializeComponent();

    private async void OnOpenClicked(object? sender, RoutedEventArgs e)
    {
        var text = UrlBox.Text?.Trim();
        if (!Uri.TryCreate(text, UriKind.Absolute, out var uri))
        {
            StatusText.Text = "Invalid URL";
            return;
        }

        var launcher = TopLevel.GetTopLevel(this)?.Launcher;
        if (launcher is null)
        {
            StatusText.Text = "Launcher not available on this platform";
            return;
        }

        StatusText.Text = await launcher.LaunchUriAsync(uri) ? "Opened" : "Could not open the URL";
    }
}
