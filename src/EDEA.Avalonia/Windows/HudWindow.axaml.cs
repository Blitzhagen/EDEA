using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using EDEA.Avalonia.ViewModels;
using EDEA.Services;

namespace EDEA.Avalonia.Windows;

/// <summary>
/// Avalonia HUD window proof-of-concept.
/// </summary>
public partial class HudWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HudWindow"/> class.
    /// </summary>
    public HudWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HudWindow"/> class with the star system provider.
    /// </summary>
    /// <param name="starSystemProvider">The star system provider.</param>
    public HudWindow(StarSystemProvider starSystemProvider)
    {
        InitializeComponent();
        DataContext = new HudViewModel(starSystemProvider);
    }

    /// <summary>
    /// Starts dragging the HUD window when the grabber is pressed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer event data.</param>
    private void HudWindowMoveGrabber_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }

    /// <summary>
    /// Closes the HUD window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The routed event data.</param>
    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
