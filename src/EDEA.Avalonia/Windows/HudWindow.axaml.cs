using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using EDEA.Avalonia.ViewModels;
using EDEA.Services;
using System;

namespace EDEA.Avalonia.Windows;

/// <summary>
/// Avalonia HUD window.
/// </summary>
public partial class HudWindow : Window
{
    /// <summary>
    /// The initial pointer position at the start of a resize operation.
    /// </summary>
    private Point _resizeStartPoint;

    /// <summary>
    /// The initial window size at the start of a resize operation.
    /// </summary>
    private Size _resizeStartSize;

    /// <summary>
    /// The resize button used to initiate a resize operation.
    /// </summary>
    private Control? _resizeButton;

    /// <summary>
    /// The move grabber used to initiate a manual move operation.
    /// </summary>
    private Control? _moveGrabber;

    /// <summary>
    /// The pointer position on screen at the start of a manual move.
    /// </summary>
    private PixelPoint _moveStartPointer;

    /// <summary>
    /// The window position at the start of a manual move.
    /// </summary>
    private PixelPoint _moveStartPosition;

    /// <summary>
    /// Gets or sets whether window dragging is handled client-side instead of via
    /// the window manager (required for override-redirect windows, which the WM
    /// ignores for _NET_WM_MOVERESIZE requests).
    /// </summary>
    public bool UseManualMove { get; set; }
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
    public HudWindow(StarSystemProvider starSystemProvider, MainViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = new HudViewModel(starSystemProvider, mainViewModel);
    }

    /// <summary>
    /// Hides interactive overlay controls while mouse pass-through is active,
    /// because they cannot receive input in that state anyway.
    /// </summary>
    /// <param name="enabled">Whether mouse pass-through is enabled.</param>
    public void SetMousePassThrough(bool enabled)
    {
        CloseHudWindowButton.IsVisible = !enabled;
        ResizeHudWindowButton.IsVisible = !enabled;
        HudWindowMoveGrabber.IsVisible = !enabled;

        if (enabled)
        {
            HudRootGrid.Classes.Add("PassThrough");
        }
        else
        {
            HudRootGrid.Classes.Remove("PassThrough");
        }
    }

    /// <summary>
    /// Starts dragging the HUD window when the grabber is pressed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer event data.</param>
    private void HudWindowMoveGrabber_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!UseManualMove)
        {
            BeginMoveDrag(e);
            return;
        }

        _moveGrabber = sender as Control;
        _moveStartPosition = Position;
        _moveStartPointer = this.PointToScreen(e.GetPosition(this));
        e.Pointer.Capture(_moveGrabber);
    }

    /// <summary>
    /// Moves the HUD window while the grabber is being dragged (manual move for
    /// override-redirect windows).
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer event data.</param>
    private void HudWindowMoveGrabber_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_moveGrabber == null || e.Pointer.Captured != _moveGrabber)
        {
            return;
        }

        var currentPointer = this.PointToScreen(e.GetPosition(this));
        Position = new PixelPoint(
            _moveStartPosition.X + currentPointer.X - _moveStartPointer.X,
            _moveStartPosition.Y + currentPointer.Y - _moveStartPointer.Y);
    }

    /// <summary>
    /// Ends the manual move drag on the HUD window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer event data.</param>
    private void HudWindowMoveGrabber_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Pointer.Captured == _moveGrabber)
        {
            e.Pointer.Capture(null);
        }

        _moveGrabber = null;
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

    /// <summary>
    /// Starts a resize drag on the HUD window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer event data.</param>
    private void ResizeButton_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _resizeButton = sender as Control;
        _resizeStartPoint = e.GetPosition(this);
        _resizeStartSize = new Size(Width, Height);
        e.Pointer.Capture(_resizeButton);
    }

    /// <summary>
    /// Resizes the HUD window while the resize button is being dragged.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer event data.</param>
    private void ResizeButton_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (e.Pointer.Captured != _resizeButton || _resizeButton == null)
        {
            return;
        }

        var currentPoint = e.GetPosition(this);
        var delta = currentPoint - _resizeStartPoint;

        Width = Math.Max(MinWidth, _resizeStartSize.Width + delta.X);
        Height = Math.Max(MinHeight, _resizeStartSize.Height + delta.Y);
    }

    /// <summary>
    /// Ends the resize drag on the HUD window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The pointer event data.</param>
    private void ResizeButton_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Pointer.Captured == _resizeButton)
        {
            e.Pointer.Capture(null);
        }

        _resizeButton = null;
    }
}
