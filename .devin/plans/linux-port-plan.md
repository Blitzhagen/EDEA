# Plan: WPF → Avalonia UI Port für Windows und Linux

## Ziel
EDEA soll langfristig plattformübergreifend unter Windows und Linux lauffähig sein. **Windows hat Priorität** und wird zuerst auf Avalonia stabilisiert. Linux folgt danach.

**Wichtig:** Die Avalonia-Version muss sich in **Funktion und Erscheinungsbild 1:1 an den Master-Branch** anlehnen. Abweichungen werden nur dann akzeptiert, wenn sie technisch zwingend notwendig sind (z. B. andere Renderer, fehlende WPF-Controls) und müssen dokumentiert werden.

**HUD-Window hat oberste Priorität:** Das HUD-Overlay ist die primäre UI-Interaktionsfläche während des Spiels. Das Hauptfenster liegt zu ~90% der Spielzeit im Hintergrund; der Nutzer arbeitet fast ausschließlich über das HUD. Jede Regression oder visuelle Abweichung im HUD beeinträchtigt die Spielbarkeit direkt. Daher wird das HUD als eigenständiger, besonders sorgfältiger Arbeitspaket behandelt.

## Grundsätze
- Arbeit ausschließlich auf Branch `linux-port`
- `master` bleibt unberührt und stabil
- Nach jeder Phase: Build + Test + Commit
- Keine Massenmigration — schrittweise, Fenster für Fenster
- UI-Fehler so weit wie möglich vermeiden durch inkrementelles Vorgehen
- `CommunityToolkit.Mvvm` bleibt das ViewModel-Toolkit
- Nur UI-relevante WPF-Abhängigkeiten werden ersetzt, Businesslogik bleibt erhalten
- **HUD-Window wird vor allen anderen komplexen Fenstern funktionsfähig gemacht, falls technisch sinnvoll (Proof-of-Concept)**

## Git-Konventionen
- Alle Commits auf `linux-port` werden ausschließlich mit der folgenden Autor-Identität erstellt:
  - **Name:** `Blitzhagen`
  - **E-Mail:** `matritt1985@gmail.com`
- Keine `Co-Authored-By` oder `Devin`-Referenzen in Commit-Messages
- Commit-Messages beschreiben das „Warum“ und folgen dem Projektstil

## 1:1-Funktions- und Designziele
- **Feature-Parität 100%**: Jede Funktion aus dem WPF-Master muss in der Avalonia-Version verfügbar sein
- **Visuelle Gleichheit**: Pixelgenaue Übereinstimmung bei:
  - Farbthemen und Farbverläufen
  - Schriftarten, Größen und Abständen
  - Icons und Symbolen
  - Layout- und Tab-Positionierung (TabControl links)
  - Statusleiste, benutzerdefinierte Titelleiste, Fensterkanten
  - **Transparentem HUD-Overlay inklusive Click-Through, Bewegbarkeit, Resize, Auto-Hide-Buttons**
- **Verhalten**: Gleiche Startup-Sequenz, gleiche Hotkeys, gleiche Dialog-Abläufe, gleiche Fehlerbehandlung
- **Abweichungen nur nach Genehmigung**: Jede sichtbare oder funktionale Abweichung muss dokumentiert und begründet werden

### Messbare Vergleichskriterien
| Kriterium | Nachweis |
|-----------|----------|
| Layout | Screenshot-Vergleich WPF vs. Avalonia (Fenster für Fenster) |
| Farben | Color-Picker-Abgleich der Hauptfarben |
| Schriftarten | Font-Family, Size, Weight in allen Hauptbereichen identisch |
| Tabellenspalten | Breite, Sortierung, Formatierung identisch |
| Hotkeys | Automatisierte Tastenabfolge prüfen |
| HUD-Verhalten | Start während Spiel, Tab-Wechsel, GUI-Fokus-Wechsel, Mouse-Pass-Through, Bewegung, Resize |
| Funktionen | Checkliste aus `doc/CHECKLIST.md` abarbeiten |

## Architektur-Grundlagen aus doc/

### MVVM-Struktur
- **Models** (`src/EDEA/Models/`): Datenklassen, reine Wertobjekte
- **Views** (`src/EDEA/Views/`): WPF `UserControl`s, Layout
- **Windows** (`src/EDEA/Windows/`): WPF-Fenster
- **ViewModels** (`src/EDEA/ViewModels/`): CommunityToolkit.Mvvm
- **Commands** (`src/EDEA/Commands/`): `ICommand`-Implementierungen
- **Converters** (`src/EDEA/Converters/`): WPF-Value-Converter
- **Services** (`src/EDEA/Services/`): Singleton-Provider
- **Stores** (`src/EDEA/Stores/`): SQLite/Dapper

### Zentrale WPF-Blocker
- 22 `.xaml` Dateien (MainWindow, HudWindow, 9 Dialoge, 8 Views, 3 Ressourcen-Dateien)
- 67 `.cs` Dateien mit `System.Windows` / `System.Windows.*` Verwendungen
- `user32.dll` P/Invoke für globale Hotkeys und HUD-Window-Styles
- `Microsoft.Web.WebView2` (About-Fenster/Lizenzen)
- `NAudio` / `SayIt` für Audio/Speech
- `Jot` für Fenstergeometrie (Avalonia-Version von Jot prüfen)
- WPF-spezifische Packages: `Material.Icons.WPF`, `Microsoft.Xaml.Behaviors.Wpf`, `PixiEditor.ColorPicker`
- WPF `ColorThemeHelper` und `Resources`/`Application.Current.Resources`

### App-Lifecycle (WPF)
1. `App` Konstruktor: log4net, User-Agent, Singleton-Mutex, ToolTip-Timeouts, Jot initialisieren
2. `OnStartup` → `RunStartup`
3. Lade Reihenfolge: Settings → Preferences → Sprache → SQLite → EDFileWatcher → JournalStore → HistoryProvider → StarSystemProvider → WebApiProvider → JournalProvider → RouteProvider → StatusProvider → PlanetsOfInterestProvider → JournalHistoryImporter → HotkeyProvider → MainViewModel → MainWindow
4. Hotkey-Listener an MainWindow anhängen
5. `OnExit` → `StarSystemProvider.HandleApplicationShutdown()`

### DataTemplates / Tab-Auswahl
- `MainWindow` bindet `TabViewModels` (`ObservableCollection<TabViewModel>`)
- `TabTemplateSelector` wählt DataTemplate anhand ViewModel-Typ
- `HudWindow` nutzt DataTemplates für `BodyHudTableView`, `NavRouteHudTableView`, `GenusHudTableView`
- Tabs: Route, Bodies, Biologicals, Surroundings, History

### HUD-Window: Besondere Bedeutung und Aufbau
Das HUD-Window ist das **wichtigste Fenster der Anwendung**, da es während des Spiels fast ausschließlich genutzt wird.

| Aspekt | WPF-Realisierung |
|--------|------------------|
| Fensterstil | `WindowStyle="None"`, `AllowsTransparency="True"`, `Topmost="True"`, `ResizeMode="CanResizeWithGrip"` |
| Hintergrund | Vollständig transparent (`#00FFFFFF`) |
| Inhalt | Systeminfo oben, kompakte DataGrid-Tabelle unten |
| View-Auswahl | Per `DataTemplate` in `HudWindow.xaml`; ViewModel-Typ löst `BodyHudTableView`, `NavRouteHudTableView` oder `GenusHudTableView` auf |
| Automatische View | `HudWindowTabViewModel = Auto` → zeigt `MainViewModel.SelectedTabIndex` an |
| Fixierte View | `HudWindowTabViewModel = BodyTableViewModel` oder `NavRouteTableViewModel` |
| Sichtbarkeit | `StatusProvider.GuiFocusUpdated` + `UserSettingsHudWindow.HideOn*` Flags |
| Opacity | `UserSettingsHudWindow.Opacity` (Default `0.1`) |
| Mouse-Pass-Through | `user32.dll` `GetWindowLong`/`SetWindowLong` mit `WS_EX_TRANSPARENT` |
| Bewegung | Unsichtbares `Rectangle` (`HudWindowMoveGrabber`) mit `DragMove()` |
| Cursor | `Mouse.OverrideCursor = Cursors.SizeAll` beim Bewegen |
| Buttons | Close-/Resize-Buttons blenden per `MouseEnter`/`MouseLeave` ein/aus |
| Button-Farben | Aus `Application.Current.Resources` (`MainColor`, `MainBackgroundColor`) |
| Fensterzustand | `JotSettingsProvider.Tracker.Track(hudWindow)` speichert Position/Größe |
| Hotkeys | Globaler Hotkey öffnet/schließt HUD; anderer toggelt Mouse-Pass-Through |
| Spalten-Sichtbarkeit | **Alle drei HUD-Views** (Body, Route, Genus) binden Spalten direkt an `Preferences.HudWindow.*ColumnVisible` |

### Biologie-Komponente ↔ HUD
- `GenusHudTableView` wird im HUD angezeigt, wenn `MainViewModel.SelectedTabIndex` auf den Biologicals-Tab zeigt und `HudWindowTabViewModel = Auto` ist
- `UserSettingsHudWindow` enthält `GenusColumn*` Eigenschaften für Spalten-Sichtbarkeit
- **Biologie-Einstellungen sind somit nicht isoliert, sondern direkt über `UserSettingsHudWindow` mit der HUD-Darstellung verknüpft**

---

## Phase 0: Vorbereitung
- Auf Branch `linux-port` arbeiten
- Sicherstellen, dass `master` sauber baut
- Ggf. einen `v1.x-stable` Tag auf `master` setzen
- Bestehende Tests laufen lassen (`dotnet test EDEA.slnx`)
- `EDEA.slnx`, `Directory.Build.props` und alle `.csproj`-Dateien auf Kompatibilität mit neuem `EDEA.Core` prüfen
- Commit

## Phase 1: Plattformunabhängiges `EDEA.Core` extrahieren
> Hinweis: Der Quellcode ist vollständig mit XML-Dokumentationskommentaren versehen (`doc/CHECKLIST.md`: 185 von 186 `.cs`-Dateien). Das hilft beim Identifizieren plattformunabhängiger vs. WPF-spezifischer Klassen.

1. Neues Projekt `src/EDEA.Core/EDEA.Core.csproj` als Class Library (`net8.0`)
2. `EDEA.slnx` erweitern, `Directory.Build.props` prüfen
3. Ordner/Dateien, die **UI-frei** in `EDEA.Core` übernommen werden:
   - `Enums/`
   - `Stores/` (SQLiteStore, JournalStore)
   - `Helpers/` ohne `ColorThemeHelper` (bleibt WPF-spezifisch)
   - `Preferences.cs` (wenn UI-frei)
   - `Globals.cs`
   - `HudWindowTabViewModel.cs`
   - `DisplaySize.cs`
   - `SurfaceScanStatus.cs`
4. `Models/` vorbereiten, aber **noch nicht** verschieben — siehe Phase 1.5
5. `Services/` vorbereiten, aber **noch nicht** verschieben — siehe Phase 1.5 und Phase 2
6. `Properties/Resources.resx` und `Resources.de.resx` in `EDEA.Core` verschieben
7. Altes WPF-`EDEA` und `test/EDEA.Tests` referenzieren `EDEA.Core`
8. Build + Tests müssen weiter passen
9. Commit

## Phase 1.5: WPF-Typen in `EDEA.Core` abstrahieren
> Notwendig, weil `Models` und `Services` WPF-spezifische Typen verwenden (`System.Windows.Media.Color`, `System.Windows.Input.Key`, `System.Windows.Input.ModifierKeys`, `Dispatcher`). Diese müssen vor dem Verschieben nach `EDEA.Core` durch plattformneutrale Typen ersetzt werden.

1. **Eigene Farbtyp in `EDEA.Core` anlegen**
   - `EDEA.Core/Drawing/Color.cs` mit `R`, `G`, `B`, `A` als `byte` Properties
   - Factory-Methoden: `FromArgb(byte a, byte r, byte g, byte b)`, `FromRgb(byte r, byte g, byte b)`
   - Konvertierungen nach/von `System.Windows.Media.Color` (WPF) und `Avalonia.Media.Color` (Avalonia) in UI-Projekten
2. **Eigene Eingabetypen in `EDEA.Core` anlegen**
   - `EDEA.Core/Input/Key.cs`: Enum für Tasten (möglichst kompatibel zu WPF/Avalonia Werte)
   - `EDEA.Core/Input/ModifierKeys.cs`: Enum für `None`, `Alt`, `Control`, `Shift`, `Windows`
   - Konvertierungsmethoden nach/von `System.Windows.Input.Key` und `System.Windows.Input.ModifierKeys`
3. **Models auf Core-Typen umstellen**
   - `ColorItem.cs` und `UserSettingsColors.cs`: `System.Windows.Media.Color` → `EDEA.Core.Drawing.Color`
   - `Hotkey.cs`, `HotkeyItem.cs`, `UserSettingsHotkeys.cs`: `System.Windows.Input.Key`/`ModifierKeys` → `EDEA.Core.Input.Key`/`ModifierKeys`
4. **Models nach `EDEA.Core` verschieben**
   - Alle `Models/` Dateien von `src/EDEA/Models` nach `src/EDEA.Core/Models` verschieben
   - Namespaces bleiben `EDEA.Models`
5. **Services nach `EDEA.Core` verschieben (plattformunabhängige Teile)**
   - Verschieben: `WebApiProvider`, `StarSystemProvider`, `JournalProvider`, `RouteProvider`, `HistoryProvider`, `StatusProvider`, `PlanetsOfInterestProvider`, `SettingsProvider`, `EDFileWatcher`, `EdDataProvider`, `GeneraIndexProvider`, `SpanshService`, `JournalHistoryImporter`, `JournalStore`, `WebApiLoadingStatusChangedEventHandler`
   - **Nicht** verschieben: `HotkeyProvider`, `SpeechProvider`, `JotSettingsProvider`
   - `JournalHistoryImporter.BackgroundWorker` auf `Task` + `IProgress<T>` + `CancellationToken` umstellen
   - `Dispatcher`-Aufrufe vorübergehend durch `SynchronizationContext` oder ähnliches ersetzen; saubere `IDispatcher`-Abstraktion folgt in Phase 2
6. **WPF-Projekt anpassen**
   - `using`-Direktiven in `EDEA` auf `EDEA.Core.Drawing` und `EDEA.Core.Input` erweitern
   - Konverter (`BooleanToBrushConverter`, `ColorThemeHelper`, etc.) auf `EDEA.Core.Drawing.Color` umstellen
   - `HotkeyProvider` konvertiert zwischen `EDEA.Core.Input.*` und `System.Windows.Input.*`
7. Build + Tests auf Windows
8. Commit

## Phase 2: Plattformabstraktionen einführen
Ziel: Alle plattformabhängigen Services hinter Interfaces verbergen, ohne dass `EDEA.Core` Windows-spezifische APIs kennt.

1. **Pfad- und Prozess-Service**
   - Interface `IPlatformService`:
     - `string GetAppDataFolder()`
     - `void OpenFolder(string path)`
     - `void OpenUri(string uri)`
   - Windows-Implementierung: `EDEA.Windows.Plattform.WindowsPlatformService`
2. **Hotkeys**
   - Interface `IGlobalHotkeyService`:
     - `void Register(HotkeyId id, Hotkey hotkey)`
     - `void Unregister(HotkeyId id)`
     - `event Action<HotkeyId>? HotkeyPressed`
   - Windows-Implementierung: `EDEA.Windows.Hotkeys.WindowsGlobalHotkeyService` (nutzt `user32.dll`)
3. **Audio / Speech**
   - Interface `ISpeechService`:
     - `Task SpeakAsync(string text, CancellationToken ct)`
     - `Task<IReadOnlyList<string>> GetVoicesAsync()`
     - `void Stop()`
   - Windows-Implementierung: `EDEA.Windows.Speech.WindowsSpeechService` (NAudio + SayIt)
4. **Fenster-Styles (HUD)**
   - Interface `IHudWindowService`:
     - `void SetClickThrough(nint windowHandle, bool enabled)`
     - `nint GetWindowHandle(object window)`
   - Windows-Implementierung: `EDEA.Windows.Hud.WindowsHudWindowService`
   - Hinweis: `IHudWindowService` arbeitet mit Native-Handles (`nint`), niemals mit UI-Framework-Typen wie `Window`
5. **Fenstergeometrie**
   - `Jot` auf Avalonia-Kompatibilität prüfen; falls nicht verfügbar, eigenen `IWindowStateService` bauen
   - Interface `IWindowStateService` für speichern/laden Fensterzustände
6. **Color-Theme**
   - `ColorThemeHelper` bleibt in WPF, neuer `IColorThemeService` mit Avalonia-Implementierung
7. **UI-Thread / Dispatcher**
   - Interface `IDispatcher`:
     - `void Invoke(Action action)`
     - `Task InvokeAsync(Func<Task> action)`
   - Windows-Implementierung: `EDEA.Windows.Threading.WindowsDispatcher` (Wrapper um `Application.Current.Dispatcher`)
   - Befreit `EDEA.Core` von `System.Windows.Threading`
8. **Dialoge / MessageBox**
   - Eigenes `EDEA.Core.Dialogs.DialogResult` Enum anlegen (kein `System.Windows.MessageBoxResult` in Core)
   - Interface `IDialogService`:
     - `DialogResult ShowConfirmation(string title, string message, ...)`
     - `void ShowInformation(...)`, `void ShowError(...)`, `void ShowWarning(...)`
   - Windows-Implementierung: `EDEA.Windows.Dialogs.WindowsDialogService` (WPF `MessageBox` mapped auf Core-`DialogResult`)
   - Alle `MessageBox.Show` Aufrufe in Core/ViewModels durch `IDialogService` ersetzen
9. **Zwischenablage**
   - Interface `IClipboardService`:
     - `void SetText(string text)`
   - Windows-Implementierung: `EDEA.Windows.Clipboard.WindowsClipboardService`
10. **Browser / URI öffnen**
    - `IPlatformService.OpenUri` erweitern
    - Alle `Process.Start` mit URLs durch Service ersetzen
11. **Timer**
    - Interface `ITimerService`:
      - `ITimer CreateTimer(TimeSpan interval, Action callback)`
      - Ersatz für `DispatcherTimer`
    - `JournalHistoryImporter.BackgroundWorker` auf `Task` + `IProgress<T>` + `CancellationToken` umstellen
12. **Bildschirm / Fenstergeometrie**
    - Eigene Core-Structs `ScreenSize` und `ScreenRect` anlegen (keine `System.Windows.Size`/`Rect` in Core)
    - Interface `IScreenService`:
      - `ScreenSize GetPrimaryScreenSize()`
      - `ScreenRect GetWorkingArea()`
    - Ersatz für `SystemParameters.VirtualScreenWidth`/`VirtualScreenHeight`
13. Build + Tests auf Windows
14. Commit

## Phase 3: Avalonia-Projekt anlegen
1. Neues Projekt `src/EDEA.Avalonia/EDEA.Avalonia.csproj` mit Targets:
   - Zuerst: `net8.0-windows10.0.19041.0` (Windows-only während des Ports)
   - Später: `net8.0` (Linux + Windows)
2. `EDEA.slnx` erweitern
3. NuGet-Packages:
   - `Avalonia` 11.x
   - `Avalonia.Desktop`
   - `Avalonia.Themes.Fluent`
   - `Avalonia.Fonts.Inter`
   - `Avalonia.Xaml.Behaviors`
   - `Avalonia.Controls.DataGrid`
   - `Avalonia.Svg.Skia` (optional für Icons)
   - `CommunityToolkit.Mvvm`
   - `Jot` nur verwenden, wenn Avalonia-Kompatibilität nachweislich gegeben; sonst eigener `IWindowStateService`
4. Projektreferenz zu `EDEA.Core`
5. Grundstruktur:
   - `Program.cs` (Entry-Point) mit Singleton-Mutex und App-Builder
   - `App.axaml` / `App.axaml.cs` (`OnFrameworkInitializationCompleted` statt `OnStartup`)
   - `MainWindow.axaml` / `MainWindow.axaml.cs`
   - Ordner `Views/`, `Windows/`, `Converters/`, `Commands/`, `Services/` (Plattformimpl.)
6. Exception-Handler:
   - `AppDomain.CurrentDomain.UnhandledException`
   - Avalonia `AppBuilder.LogToTrace` + `UnhandledException` abonnieren
   - MessageBox-Dialog über `IDialogService` bei UI-Exceptions
7. App-Icon und Fenster-Icons:
   - `EDEA.res` / `app.ico` durch Avalonia-kompatible Ressourcen ersetzen
   - Icon in `ApplicationIcon` / `Window.Icon` konfigurieren
8. Erstes Build-Ziel: Avalonia-App startet und zeigt leeres Hauptfenster
9. Commit

## Phase 4: ViewModels in Core- und Avalonia-Projekt aufteilen
Ziel: ViewModels sollen in `EDEA.Core` oder `EDEA.Avalonia` liegen, je nachdem, ob sie UI-spezifisch sind.

1. **ViewModels, die plattformunabhängig in `EDEA.Core` bleiben:**
   - `ViewModelBase`
   - `TabViewModel`
   - `MainViewModel` (muss `Dispatcher`-Aufrufe abstrahieren)
   - `BodyTableViewModel`
   - `NavRouteTableViewModel`
   - `GenusTableViewModel`
   - `SurroundingsTableViewModel`
   - `HistoryViewModel`
   - `StarSystemViewModel`
   - `BodyViewModel`
   - `GenusViewModel`
   - `GenusClassificationViewModel`
   - `HistoryDataViewModel`
   - `HotkeyViewModel` (kein UI-spezifisches Window)
   - `PlanetClassificationViewModel`
   - `RingViewModel`
   - `SurroundingView`
   - `RouteView` / `RoutePlotterViewModel` (nur Data, kein WPF-Window)
2. **ViewModels, die UI-spezifisch in `EDEA.Avalonia` (bzw. `EDEA.Windows`) liegen:**
   - `HudViewModel` (HUD-Window-Logik, P/Invoke)
   - `PreferencesViewModel` (Fenster-Commands, Dialoge)
   - `AboutViewModel`
   - `FeedbackReportIssueViewModel`
   - `JournalHistoryImportViewModel`
3. Commands in `EDEA.Avalonia` oder `EDEA.Core` je nach UI-Abhängigkeit
4. Build + Tests
5. Commit

## Phase 5: UI-Fenster inkrementell portieren (Window für Window)
Reihenfolge von einfach nach komplex:

1. `AboutWindow` → `AboutWindow.axaml`
2. `InputStringDialogWindow` / `InputStringListDialogWindow`
3. `FeedbackReportIssueWindow`
4. `JournalHistoryImportWindow`
5. `PreferencesWindow`
6. **HudWindow** (siehe separate Phase 5.1 — kritischste Komponente)
7. `RoutePlotterWindow`
8. `MainWindow`
9. Views/Tables (`BodyTableView`, `RouteTableView`, `GenusTableView`, `HistoryTableView`, `SurroundingsTableView`, sowie HUD-Varianten)

### Phase 5.1: HudWindow (separat, kritisch)
Das HUD-Window ist die **wichtigste UI-Komponente** des Programms und wird deshalb als separates, besonders sorgfältiges Arbeitspaket behandelt.

Schritte:
1. **HUD-Proof-of-Concept** anlegen:
   - `HudWindow.axaml` mit transparentem, borderless Overlay
   - `SystemDecorations="None"`, `TransparencyLevelHint="Transparent"`, `Background="Transparent"`, `Topmost="True"`
   - Bewegbarkeit mit Pointer-Events (Ersatz für `DragMove()`)
   - Mouse-Pass-Through über `IHudWindowService`
   - Resize-Grip in Avalonia nachbilden
2. **HudViewModel** portieren:
   - `Application.Current.Dispatcher.Invoke` durch `IDispatcher` ersetzen
   - `WindowInteropHelper`/`HWND` durch `IHudWindowService` abstrahieren
   - `Mouse.OverrideCursor` durch Avalonia `Cursor`-Property
   - `Application.Current.Resources` durch Avalonia `Application.Resources`
   - `FindName` für Close-/Resize-Buttons ersetzen
3. **HUD-Views** portieren:
   - `BodyHudTableView.axaml`
   - `NavRouteHudTableView.axaml`
   - `GenusHudTableView.axaml`
   - Alle Spalten-Sichtbarkeiten binden an `Preferences.HudWindow.*ColumnVisible`
   - `x:Static Preferences.HudWindow` → Avalonia-kompatible Resource/Bindings-Lösung
4. **DataTemplates** im `HudWindow.axaml` auf ViewModel-Typen abbilden
5. **Hotkeys** für HUD öffnen/schließen und Mouse-Pass-Through umschalten
6. **Jot** oder `IWindowStateService` für Fenstergeometrie
7. **Intensiver manueller Test** mit echten Elite-Dangerous-Journal-Daten
8. Commit

### Portierungsschritte für alle Fenster
- `.xaml` → `.axaml` konvertieren
- `Window` → `Avalonia.Controls.Window`
- `UserControl` → `Avalonia.Controls.UserControl`
- `System.Windows.*` → `Avalonia.*` Namespaces
- `DataGrid` → Avalonia `DataGrid` (aus `Avalonia.Controls.DataGrid`)
- `TabControl` mit `TabStripPlacement="Left"` → Avalonia `TabControl` mit Styling
- `DataTemplate` / `TabTemplateSelector` → Avalonia `DataTemplate`
- `Application.Current.Resources` / `StaticResource` / `DynamicResource` → Avalonia `Application.Resources` / `StaticResource` / `DynamicResource`
- Converters (`IValueConverter`) an Avalonia `IValueConverter` anpassen
- `Binding`, `RelativeSource`, `ElementName` Syntax prüfen
- `Popup` / `ToolTip` anpassen
- `Material.Icons.WPF` → Avalonia-Icons (z. B. `Avalonia.Svg.Skia`, `Projektanker.Icons.Avalonia` oder eigene SVGs)
- Code-Behind anpassen (z. B. `this.DataContext = this;`)
- Fenster-Properties wie `ResizeMode`, `WindowStartupLocation`, `WindowStyle`, `Topmost`, `ShowInTaskbar`, `SizeToContent` 1:1 auf Avalonia abbilden
- `DragMove()` der benutzerdefinierten Titelleiste ersetzen
- `Hyperlink.RequestNavigate` durch `IPlatformService.OpenUri` ersetzen
- Globale `ToolTipService`-Delays in Avalonia-App-Start konfigurieren
- Build + manueller Test
- Commit

### Besonderheiten
- `MainWindow` hat **keinen Standardrahmen** (`WindowStyle="None"`, `AllowsTransparency="True"`) → in Avalonia `SystemDecorations="None"`, `TransparencyLevelHint="Transparent"`, `Background="Transparent"`
- **Benutzerdefinierte Titelleiste** muss neu implementiert werden
- **Statusleiste** und **TabControl links**
- **HUD** transparentes Overlay mit Click-Through, Bewegung, Resize und Auto-Hide-Buttons

## Phase 6: Windows-spezifische Features in Avalonia aktivieren
1. **Globale Hotkeys** via `IGlobalHotkeyService` (Windows-Implementation)
2. **HUD Click-Through** via `IHudWindowService` (Windows-Implementation)
3. **Ordnerdialoge** via Avalonia `OpenFolderDialog`
4. **Fenstergeometrie** speichern/laden (Jot Avalonia oder eigener `IWindowStateService`)
5. **Audio/Speech** via `ISpeechService` (Windows-Implementation)
6. **WebView2** im About-Fenster entfernen oder durch Link-Liste ersetzen
7. Build + Tests
8. Commit

## Phase 7: Avalonia-Version unter Windows stabilisieren (1:1-Validierung)
Ziel: Die Avalonia-Version muss auf Windows exakt wie der WPF-Master funktionieren und aussehen.

1. Vollständige Avalonia-App unter Windows bauen
2. **Automatisierte Vorabprüfung (keine Regression):**
   - Alle Unit-Tests auf `EDEA.Core` laufen lassen
   - Avalonia-App startet ohne Exception
   - Smoke-Test: Einstellungen laden, SQLite öffnen, Journal-Verzeichnis erkennen
3. **HUD-Validierung (höchste Priorität):**
   - HUD öffnet/schließt per Hotkey
   - HUD folgt korrekt dem aktuellen Tab (Body/Route/Genus)
   - Mouse-Pass-Through funktioniert (Klicks gehen durch, wenn aktiviert)
   - HUD lässt sich bewegen und resizen
   - Close-/Resize-Buttons erscheinen bei MouseEnter, verschwinden bei MouseLeave
   - HUD blendet korrekt bei GUI-Fokus-Wechsel ein/aus (`HideOn*` Flags)
   - Opacity wirkt wie in WPF
   - Spalten-Sichtbarkeit für Body, Route und Genus stimmt mit Einstellungen überein
   - Screenshot-Vergleich WPF-HUD vs. Avalonia-HUD
4. Feature-Parität mit WPF-Version prüfen (manuelle Checkliste):
   - Journal-Verarbeitung
   - EDSM/Spansh-Abfragen
   - Tab-Wechsel
   - Hotkeys
   - Einstellungen
   - Farbthemen
   - Sprachausgabe
5. Visueller 1:1-Abgleich (Fenster für Fenster):
   - Screenshots WPF vs. Avalonia in identischer Auflösung und Zustand
   - Abgleich von Farben, Abständen, Schriftarten, Icons, Tab-Positionen
   - Benutzerdefinierte Titelleiste pixelgenau nachbilden
6. Input-Dialoge, Menüs, Tooltips und Statusleiste prüfen
7. **Bugfix-Policy**: Keine neue Phase beginnen, bevor alle in Phase 7 gefundenen Abweichungen behoben oder dokumentiert-genehmigt sind
8. Paralleler Betrieb WPF + Avalonia im selben Branch
9. Abweichungen dokumentieren und nur nach Freigabe akzeptieren
10. Fehler beheben
11. Commit

## Phase 8: Linux-Target aktivieren
1. `EDEA.Avalonia.csproj` TargetFramework auf `net8.0` ändern
2. Plattform-Implementierungen für Linux ergänzen:
   - `LinuxPlatformService`
   - `LinuxGlobalHotkeyService` (X11/Wayland, ggf. deaktivieren/hinter Option verbergen)
   - `LinuxSpeechService` (z. B. `espeak`, `speech-dispatcher`, SystemTTS)
   - `LinuxHudWindowService`
3. AppData-Pfad für Linux (`XDG_CONFIG_HOME`)
4. `OpenFolder` mit `xdg-open`
5. `FileSystemWatcher` auf Linux-Verhalten testen (besonders für `Status.json` und `NavRoute.json`)
6. Build unter Linux (WSL / VM)
7. Tests laufen lassen
8. Commit

## Phase 9: WPF entfernen (optional, wenn Avalonia stabil)
1. Altes WPF-Projekt `src/EDEA/` entfernen
2. `EDEA.Avalonia` in `EDEA` umbenennen
3. Lösung `EDEA.slnx` anpassen
4. README aktualisieren
5. Publish-Profile für Windows (`win-x64`) und Linux (`linux-x64`) anlegen:
   - Self-contained / Framework-dependent prüfen
   - Trimming-Einstellungen testen
   - App-Icon und Ressourcen korrekt einbinden
6. Finaler Build + Test
7. Commit

---

## Risiken & Massnahmen

| Risiko | Massnahme |
|--------|-----------|
| WPF-XAML nicht 1:1 portierbar | Pro Fenster isoliert vorgehen, keine Massenmigration |
| `CommunityToolkit.Mvvm` Source-Generatoren | ViewModels in Core- und Avalonia-Projekt korrekt namespacen |
| Hotkeys/Window-Styles funktionieren nicht auf Linux | Hinter Interface abstrahieren, Windows-Implementation zuerst |
| Audio bricht | `NAudio`/`SayIt` durch plattformneutrales Interface ersetzen |
| Datenbank/Pfade | `Path.Combine` beibehalten, `XDG_CONFIG_HOME` für Linux verwenden |
| `DataGrid`-Verhalten in Avalonia anders | Jede Tabelle manuell testen, Spaltendefinitionen prüfen |
| Custom TitleBar/Transparentes MainWindow | In Avalonia `SystemDecorations="None"` + `TransparencyLevelHint` |
| `WebView2` Lizenzen-Seite | Durch statische Link-Liste ersetzen |
| `Jot` nicht Avalonia-kompatibel | Vor Phase 3 Kompatibilität prüfen, sonst eigener `IWindowStateService` |
| `FileSystemWatcher` unter Linux eingeschränkt | Fallback-Polling in `EDFileWatcher` implementieren oder testen |
| HUD-Funktionsverlust oder visuelle Abweichung | Separate Phase 5.1, Proof-of-Concept, intensive manuelle Tests, Screenshot-Vergleich |
| HUD Mouse-Pass-Through oder DragMove funktioniert nicht | Früher Proof-of-Concept unter Windows, Interface-basierte Plattformabstraktion |
| HUD-Spalten-Sichtbarkeit stimmt nicht | `x:Static Preferences.HudWindow` auf Avalonia-kompatible Binding-Lösung umstellen und testen |
| WPF-Version zerstört | Arbeit ausschließlich auf `linux-port`, `master` unberührt |
| Visuelle 1:1-Forderung nicht erreichbar | Screenshot-Vergleich, Pixelabgleich, dokumentierte Ausnahmen |
| Icons/Symbole sehen anders aus | Material.Icons.WPF ersetzen durch exakte SVG-Nachbildungen |
| Schriftarten rendern unterschiedlich | Gleiche Font-Family und -Größen erzwingen, ggf. Font-Subset einbinden |
| Farben/Ränder weichen auf Linux ab | Farbdefinitionen zentralisieren und auf beiden Plattformen testen |
| Späte Bugfixes nach Fenster-Port | Phase 7 Bugfix-Policy: keine neue Phase vor vollständigem 1:1-Abgleich |
| Publish-Trimming entfernt benötigte XAML-Styles | Trim-Modus `partial` oder Trimmer-Root konfigurieren |

---

## Dokumentation, die während des Ports hilfreich ist
- `doc/ARCHITECTURE.md` — MVVM-Überblick
- `doc/APP_LIFECYCLE.md` — Start-/Exit-Sequenz
- `doc/VIEWS_WINDOWS.md` — Fenster und DataTemplates
- `doc/VIEWMODELS.md` — ViewModel-Struktur
- `doc/SERVICES.md` — Provider und Singletons
- `doc/COMMANDS.md` — Commands
- `doc/CONVERTERS.md` — Value-Converter
- `doc/TESTING.md` — Test-Setup

---

## Geschätzter Aufwand
- **Mindestens 1–2 Monate Vollzeit** für Core-Extraktion, Avalonia-Port und Windows-1:1-Validierung
- **Der HUD-Port allein kann 1–2 Wochen** in Anspruch nehmen, inklusive Proof-of-Concept, visuellem Abgleich und realen Spieltests
- **Linux-Port** kommt danach, mit zusätzlichem Aufwand für Hotkeys, Audio und Window-Manager-Integration
- Der visuelle 1:1-Abgleich ist der zeitintensivste Teil, da viele kleine UI-Details in Avalonia anders gerendert werden
- Beginn: **Phase 1** (`EDEA.Core` extrahieren)
