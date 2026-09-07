# EDEA-Anwendungslebenszyklus

Dieses Dokument beschreibt Start, Initialisierung und Beenden der EDEA-Avalonia-Anwendung.

## Einstiegspunkt

`Program.Main` (`src/EDEA.Avalonia/Program.cs`) startet die Avalonia-App über `StartWithClassicDesktopLifetime`:

```csharp
AppBuilder.Configure<App>()
    .UsePlatformDetect()
    .WithInterFont()
    .LogToTrace();
```

## App.Initialize

`App.Initialize` (`src/EDEA.Avalonia/App.axaml.cs`):

- Lädt das XAML (`AvaloniaXamlLoader.Load`).
- Initialisiert `ToolTipDataContextBehavior` (Tooltip-DataContext-Weiterleitung).

Der statische `App`-Konstruktor konfiguriert log4net (Rolling-File-Appender, Log unter `%LOCALAPPDATA%\EDEA.Core\log\`).

## OnFrameworkInitializationCompleted

Reihenfolge beim Start:

1. **Exception-Handler** registrieren (`AppDomain.UnhandledException`, `TaskScheduler.UnobservedTaskException`, `Dispatcher.UIThread.UnhandledException`).
2. **`RegisterPlatformServices()`** – befüllt `PlatformServices` mit den Avalonia-Implementierungen (Speech, Hotkeys, Fensterzustand, Dialoge, Clipboard, Screen, ColorTheme, Dispatcher, UiTimer, HUD, Platform).
3. **`InitializeSettings()`** –
   - `SettingsProvider` lädt `settings.json` aus `%LOCALAPPDATA%\EDEA.Core` und setzt `Preferences.User`. Gespeicherte Standard-Sprachphrasen werden migriert.
   - `ApplyLanguage()` setzt `CurrentCulture`, `CurrentUICulture` und `Resources.Culture` (wirkt live über `CultureChanged`).
   - `ColorTheme.ApplyCurrentColors()` + `ApplyCurrentDisplaySize()`.
4. **`RunStartup()`** – Provider-Kette aufbauen:
   - `EDFileWatcher` (Journal-/Status-Verzeichnis)
   - `JournalStore` (inkrementelles Lesen)
   - `SQLiteStore` (`db/EDEA.db`)
   - `HistoryProvider`, `StarSystemProvider`, `WebApiProvider`
   - `JournalProvider`, `RouteProvider`, `StatusProvider`, `PlanetsOfInterestProvider`
   - `JournalHistoryImporter` – startet den Import alter Journal-Dateien automatisch, wenn neue gefunden wurden
   - `journalProvider.Initialize()` – erstes Parsen
5. **Hauptfenster**: `MainWindow` mit `MainViewModel` erzeugen, `WindowState.Track(...)` für Fensterposition, `ShutdownMode.OnMainWindowClose`, `RestoreWindows()` stellt weitere Fenster (HUD etc.) wieder her.

## Sprachumschaltung zur Laufzeit

`App.ApplyLanguage()` kann jederzeit aufgerufen werden (Sprach-ComboBox in den Einstellungen). Unterstützt: `Auto`, `en`, `de`, `ru` (Infrastruktur für `es`, `fr`, `pt-BR` vorbereitet). `Resources.CultureChanged` invalidiert alle `{l:Loc ...}`-Bindungen, lädt die Edge-TTS-Stimmliste neu und migriert Standard-Sprachphrasen.

## Beenden

- `ShutdownMode.OnMainWindowClose` – die App endet mit dem Hauptfenster.
- `MainWindow.Closing` → `MainViewModel.OnMainWindowClosing()` räumt auf (u. a. Fensterzustände, Hotkeys, Speech-Shutdown).
- `SettingsProvider.Save()` persistiert die Einstellungen.

## Logging

- log4net Rolling-File-Appender unter `%LOCALAPPDATA%\EDEA.Core\log\` (max. 5 MB, bis zu 5 Backups).
- Log-Muster: `dd MMM yyyy HH:mm:ss,fff %-5level | message (logger)`.
