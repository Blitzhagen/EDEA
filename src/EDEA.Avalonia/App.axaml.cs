using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EDEA.Avalonia.Services;
using EDEA.Avalonia.Views;
using EDEA.Services;

namespace EDEA.Avalonia;

/// <summary>
/// Avalonia application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the XAML components.
    /// </summary>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Called when the application framework has been initialized.
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        RegisterPlatformServices();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Registers Avalonia-specific platform service implementations.
    /// </summary>
    private static void RegisterPlatformServices()
    {
        PlatformServices.Dispatcher = new AvaloniaDispatcher();
        PlatformServices.Clipboard = new AvaloniaClipboardService();
        PlatformServices.Speech = new AvaloniaSpeechService();
        PlatformServices.Dialog = new AvaloniaDialogService();
        PlatformServices.Platform = new AvaloniaPlatformService();
        PlatformServices.HudWindow = new AvaloniaHudWindowService();
    }
}
