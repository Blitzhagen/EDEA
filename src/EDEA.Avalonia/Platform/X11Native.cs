using System;
using System.Runtime.InteropServices;

namespace EDEA.Avalonia.Platform;

/// <summary>
/// Minimal libX11/libXext interop used for global hotkeys (XGrabKey) and
/// click-through input regions (XShape) on X11 and XWayland.
/// </summary>
internal static class X11Native
{
    private const string LibX11 = "libX11";
    private const string LibXext = "libXext";

    // Event types.
    internal const int KeyPress = 2;

    // Modifier masks (X.h).
    internal const uint ShiftMask = 1 << 0;
    internal const uint LockMask = 1 << 1;
    internal const uint ControlMask = 1 << 2;
    internal const uint Mod1Mask = 1 << 3;
    internal const uint Mod2Mask = 1 << 4;
    internal const uint Mod4Mask = 1 << 6;

    /// <summary>Mask covering all modifiers that can be part of a hotkey.</summary>
    internal const uint HotkeyModifierMask = ShiftMask | ControlMask | Mod1Mask | Mod4Mask;

    /// <summary>Mask of "lock" modifiers (CapsLock/NumLock) that must be grabbed in all combinations.</summary>
    internal const uint LockVariantMask = LockMask | Mod2Mask;

    // Grab modes.
    internal const int GrabModeSync = 0;
    internal const int GrabModeAsync = 1;

    // XShape (Xext) shape kinds.
    internal const int ShapeBounding = 0;
    internal const int ShapeClip = 1;
    internal const int ShapeInput = 2;

    // XShape operations.
    internal const int ShapeSet = 0;
    internal const int ShapeUnion = 1;
    internal const int ShapeIntersect = 2;
    internal const int ShapeSubtract = 3;
    internal const int ShapeInvert = 4;

    /// <summary>
    /// XEvent union (192 bytes). Only the fields needed for KeyPress are mapped.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 192)]
    internal struct XEventUnion
    {
        /// <summary>The X event type (e.g. <see cref="KeyPress"/>).</summary>
        [FieldOffset(0)] public int Type;

        /// <summary>xkey.state — modifier state at the time of the key press.</summary>
        [FieldOffset(80)] public uint KeyState;

        /// <summary>xkey.keycode — the physical key code.</summary>
        [FieldOffset(84)] public uint KeyCode;
    }

    /// <summary>
    /// XErrorEvent structure passed to the error handler.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XErrorEvent
    {
        public int Type;
        public IntPtr Display;
        public ulong ResourceId;
        public ulong Serial;
        public byte ErrorCode;
        public byte RequestCode;
        public byte MinorCode;
    }

    /// <summary>
    /// X error handler delegate. Must be kept alive to prevent GC.
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int XErrorHandler(IntPtr display, ref XErrorEvent errorEvent);

    /// <summary>
    /// Enables Xlib multi-threading. Must be called before any other Xlib call
    /// when more than one thread uses the display connection.
    /// </summary>
    [DllImport(LibX11)]
    internal static extern int XInitThreads();

    [DllImport(LibX11)]
    internal static extern IntPtr XOpenDisplay([MarshalAs(UnmanagedType.LPStr)] string? displayName);

    [DllImport(LibX11)]
    internal static extern int XCloseDisplay(IntPtr display);

    /// <summary>Returns the root window of the default screen.</summary>
    [DllImport(LibX11)]
    internal static extern ulong XDefaultRootWindow(IntPtr display);

    /// <summary>Returns the display file descriptor for use with poll/select.</summary>
    [DllImport(LibX11)]
    internal static extern int XConnectionNumber(IntPtr display);

    /// <summary>Looks up the keysym for a keysym name (e.g. "F5", "h", "KP_1"). Returns 0 on unknown names.</summary>
    [DllImport(LibX11)]
    internal static extern ulong XStringToKeysym([MarshalAs(UnmanagedType.LPStr)] string name);

    /// <summary>Converts a keysym to a keycode using the server's keyboard mapping.</summary>
    [DllImport(LibX11)]
    internal static extern byte XKeysymToKeycode(IntPtr display, ulong keysym);

    /// <summary>Establishes a passive grab on a key combination.</summary>
    [DllImport(LibX11)]
    internal static extern int XGrabKey(IntPtr display, int keycode, uint modifiers, ulong grabWindow,
        int ownerEvents, int pointerMode, int keyboardMode);

    /// <summary>Releases a passive key grab.</summary>
    [DllImport(LibX11)]
    internal static extern int XUngrabKey(IntPtr display, int keycode, uint modifiers, ulong grabWindow);

    /// <summary>Returns the number of events queued for this connection.</summary>
    [DllImport(LibX11)]
    internal static extern int XPending(IntPtr display);

    /// <summary>Reads the next event from the queue (blocking if empty).</summary>
    [DllImport(LibX11)]
    internal static extern int XNextEvent(IntPtr display, ref XEventUnion eventUnion);

    /// <summary>Flushes the request buffer and waits for all pending errors/replies.</summary>
    [DllImport(LibX11)]
    internal static extern int XSync(IntPtr display, int discard);

    /// <summary>Installs a process-wide X error handler.</summary>
    [DllImport(LibX11)]
    internal static extern IntPtr XSetErrorHandler(XErrorHandler handler);

    /// <summary>
    /// Combines a region/rectangles with the specified shape kind of a window.
    /// Passing <see cref="IntPtr.Zero"/> for <paramref name="rects"/> with
    /// <paramref name="nRects"/> == 0 sets an empty shape (full input pass-through
    /// for <see cref="ShapeInput"/>).
    /// </summary>
    [DllImport(LibXext)]
    internal static extern void XShapeCombineRectangles(IntPtr display, ulong window, int shapeKind,
        int xOff, int yOff, IntPtr rects, int nRects, int op, int ordering);

    /// <summary>
    /// Combines a pixmap mask with the specified shape kind. Passing 0 (None) as
    /// <paramref name="srcMask"/> resets the shape to its default — for
    /// <see cref="ShapeInput"/> that restores the full window input region.
    /// </summary>
    [DllImport(LibXext)]
    internal static extern void XShapeCombineMask(IntPtr display, ulong window, int shapeKind,
        int xOff, int yOff, ulong srcMask, int op);

    /// <summary>Queries whether the XShape extension is available.</summary>
    [DllImport(LibXext)]
    internal static extern bool XShapeQueryExtension(IntPtr display, out int eventBase, out int errorBase);
}
