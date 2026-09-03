namespace EDEA.Core.Input;

/// <summary>
/// Specifies the possible key values on a keyboard.
/// This is a platform-independent replacement for <see cref="System.Windows.Input.Key"/>.
/// The numeric values match the WPF <see cref="System.Windows.Input.Key"/> enum so that
/// simple casts can be used when interoperating with WPF on Windows.
/// </summary>
public enum Key
{
    /// <summary>No key pressed.</summary>
    None = 0,

    /// <summary>The BACKSPACE key.</summary>
    Back = 2,

    /// <summary>The TAB key.</summary>
    Tab = 3,

    /// <summary>The ENTER key.</summary>
    Enter = 6,

    /// <summary>The PAUSE key.</summary>
    Pause = 7,

    /// <summary>The ESC key.</summary>
    Escape = 13,

    /// <summary>The SPACE key.</summary>
    Space = 18,

    /// <summary>The PAGE UP key.</summary>
    PageUp = 19,

    /// <summary>The PAGE DOWN key.</summary>
    PageDown = 20,

    /// <summary>The END key.</summary>
    End = 21,

    /// <summary>The HOME key.</summary>
    Home = 22,

    /// <summary>The LEFT ARROW key.</summary>
    Left = 23,

    /// <summary>The UP ARROW key.</summary>
    Up = 24,

    /// <summary>The RIGHT ARROW key.</summary>
    Right = 25,

    /// <summary>The DOWN ARROW key.</summary>
    Down = 26,

    /// <summary>The INSERT key.</summary>
    Insert = 31,

    /// <summary>The DELETE key.</summary>
    Delete = 32,

    /// <summary>The 0 key.</summary>
    D0 = 34,

    /// <summary>The 1 key.</summary>
    D1 = 35,

    /// <summary>The 2 key.</summary>
    D2 = 36,

    /// <summary>The 3 key.</summary>
    D3 = 37,

    /// <summary>The 4 key.</summary>
    D4 = 38,

    /// <summary>The 5 key.</summary>
    D5 = 39,

    /// <summary>The 6 key.</summary>
    D6 = 40,

    /// <summary>The 7 key.</summary>
    D7 = 41,

    /// <summary>The 8 key.</summary>
    D8 = 42,

    /// <summary>The 9 key.</summary>
    D9 = 43,

    /// <summary>The A key.</summary>
    A = 44,

    /// <summary>The B key.</summary>
    B = 45,

    /// <summary>The C key.</summary>
    C = 46,

    /// <summary>The D key.</summary>
    D = 47,

    /// <summary>The E key.</summary>
    E = 48,

    /// <summary>The F key.</summary>
    F = 49,

    /// <summary>The G key.</summary>
    G = 50,

    /// <summary>The H key.</summary>
    H = 51,

    /// <summary>The I key.</summary>
    I = 52,

    /// <summary>The J key.</summary>
    J = 53,

    /// <summary>The K key.</summary>
    K = 54,

    /// <summary>The L key.</summary>
    L = 55,

    /// <summary>The M key.</summary>
    M = 56,

    /// <summary>The N key.</summary>
    N = 57,

    /// <summary>The O key.</summary>
    O = 58,

    /// <summary>The P key.</summary>
    P = 59,

    /// <summary>The Q key.</summary>
    Q = 60,

    /// <summary>The R key.</summary>
    R = 61,

    /// <summary>The S key.</summary>
    S = 62,

    /// <summary>The T key.</summary>
    T = 63,

    /// <summary>The U key.</summary>
    U = 64,

    /// <summary>The V key.</summary>
    V = 65,

    /// <summary>The W key.</summary>
    W = 66,

    /// <summary>The X key.</summary>
    X = 67,

    /// <summary>The Y key.</summary>
    Y = 68,

    /// <summary>The Z key.</summary>
    Z = 69,

    /// <summary>The left Windows logo key.</summary>
    LWin = 70,

    /// <summary>The right Windows logo key.</summary>
    RWin = 71,

    /// <summary>The APPLICATION key.</summary>
    Apps = 72,

    /// <summary>The 0 key on the numeric keypad.</summary>
    NumPad0 = 74,

    /// <summary>The 1 key on the numeric keypad.</summary>
    NumPad1 = 75,

    /// <summary>The 2 key on the numeric keypad.</summary>
    NumPad2 = 76,

    /// <summary>The 3 key on the numeric keypad.</summary>
    NumPad3 = 77,

    /// <summary>The 4 key on the numeric keypad.</summary>
    NumPad4 = 78,

    /// <summary>The 5 key on the numeric keypad.</summary>
    NumPad5 = 79,

    /// <summary>The 6 key on the numeric keypad.</summary>
    NumPad6 = 80,

    /// <summary>The 7 key on the numeric keypad.</summary>
    NumPad7 = 81,

    /// <summary>The 8 key on the numeric keypad.</summary>
    NumPad8 = 82,

    /// <summary>The 9 key on the numeric keypad.</summary>
    NumPad9 = 83,

    /// <summary>The Multiply key.</summary>
    Multiply = 84,

    /// <summary>The Add key.</summary>
    Add = 85,

    /// <summary>The Subtract key.</summary>
    Subtract = 87,

    /// <summary>The Decimal key.</summary>
    Decimal = 88,

    /// <summary>The Divide key.</summary>
    Divide = 89,

    /// <summary>The F1 key.</summary>
    F1 = 90,

    /// <summary>The F2 key.</summary>
    F2 = 91,

    /// <summary>The F3 key.</summary>
    F3 = 92,

    /// <summary>The F4 key.</summary>
    F4 = 93,

    /// <summary>The F5 key.</summary>
    F5 = 94,

    /// <summary>The F6 key.</summary>
    F6 = 95,

    /// <summary>The F7 key.</summary>
    F7 = 96,

    /// <summary>The F8 key.</summary>
    F8 = 97,

    /// <summary>The F9 key.</summary>
    F9 = 98,

    /// <summary>The F10 key.</summary>
    F10 = 99,

    /// <summary>The F11 key.</summary>
    F11 = 100,

    /// <summary>The F12 key.</summary>
    F12 = 101,

    /// <summary>The NUM LOCK key.</summary>
    NumLock = 114,

    /// <summary>The SCROLL LOCK key.</summary>
    Scroll = 115,

    /// <summary>The left SHIFT key.</summary>
    LeftShift = 116,

    /// <summary>The right SHIFT key.</summary>
    RightShift = 117,

    /// <summary>The left CTRL key.</summary>
    LeftCtrl = 118,

    /// <summary>The right CTRL key.</summary>
    RightCtrl = 119,

    /// <summary>The left ALT key.</summary>
    LeftAlt = 120,

    /// <summary>The right ALT key.</summary>
    RightAlt = 121,

    /// <summary>The OEM Semicolon key.</summary>
    OemSemicolon = 140,

    /// <summary>The OEM Plus key.</summary>
    OemPlus = 141,

    /// <summary>The OEM Comma key.</summary>
    OemComma = 142,

    /// <summary>The OEM Minus key.</summary>
    OemMinus = 143,

    /// <summary>The OEM Period key.</summary>
    OemPeriod = 144,

    /// <summary>The OEM Question key.</summary>
    OemQuestion = 145,

    /// <summary>The OEM Tilde key.</summary>
    OemTilde = 146,

    /// <summary>The OEM Open Brackets key.</summary>
    OemOpenBrackets = 149,

    /// <summary>The OEM Pipe key.</summary>
    OemPipe = 150,

    /// <summary>The OEM Close Brackets key.</summary>
    OemCloseBrackets = 151,

    /// <summary>The OEM Quotes key.</summary>
    OemQuotes = 152,

    /// <summary>The OEM Backslash key.</summary>
    OemBackslash = 154,

    /// <summary>A special key used to clear all literal input in an IME.</summary>
    OemClear = 5,
}
