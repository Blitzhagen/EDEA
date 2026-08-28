# EDEA-Anwendungslebenszyklus

Dieses Dokument beschreibt den Start, die Initialisierung und das Beenden der EDEA-WPF-Anwendung.

## Überblick

Die Anwendung wird über `App.xaml` gestartet. Die zugehörige Code-Behind-Klasse `App` (`src/EDEA/App.xaml.cs`) übernimmt Logging, Singleton-Schutz, Spracheinstellung, Jot-Tracking und die sukzessive Initialisierung aller Services.

## App-Konstruktor

Der `App`-Konstruktor führt folgende Schritte aus:

- **log4net initialisieren** – `ConfigureLog4Net()` erzeugt ein Rolling-File-Appender-Setup.
- **User-Agent setzen** – Der zentrale `HttpClient` bekommt einen User-Agent aus Assembly-Name und Version.
- **Singleton-Prüfung** – Ein globaler Mutex stellt sicher, dass nur eine EDEA-Instanz läuft. Bei Konflikt wird die Anwendung beendet.
- **Tooltip-Timeouts** – Globale `ToolTipService`-Metadaten werden angepasst.
- **Jot initialisieren** – `JotSettingsProvider.Tracker` speichert Fensterpositionen und -größen in `%LOCALAPPDATA%\EDEA\jot`.
- **EDMC-Hinweis** – Es wird geprüft, ob *Elite Dangerous Market Connector (EDMC)* läuft.
- **Exception-Handler registrieren** – Dispatcher- und AppDomain-Unhandled-Exception-Handler.

## Logging

- Log-Datei: `%LOCALAPPDATA%\EDEA\log\EDEA.log`
- Rotation: maximal 5 MB, bis zu 5 Backups.
- **DEBUG-Build**: Trace-Appender und Datei-Appender loggen DEBUG-Level; im Release ab INFO.
- Log-Muster: `dd MMM yyyy HH:mm:ss,fff %-5level | message (logger)`

## OnStartup und RunStartup

`OnStartup` ruft `RunStartup()` auf. Bei einer Exception wird diese geloggt und weitergeworfen.

`RunStartup` initialisiert die Anwendung in folgender Reihenfolge:

1. `SettingsProvider` – lädt `settings.json` aus `%LOCALAPPDATA%\EDEA`.
2. `Preferences` – globale Einstellungs-Referenz wird gesetzt.
3. `ColorThemeHelper.ApplyCurrentColors()` – aktuelle Farben anwenden.
4. `ApplyLanguage()` – UI- und Thread-Culture basierend auf `Preferences.Application.Language` setzen.
5. `SQLiteStore` – Singleton-Datenbank öffnen (`EDEA.db`).
6. `EDFileWatcher` – Journal-Verzeichnis überwachen.
7. `JournalStore` – Journal-Zeilen speichern/lesen.
8. `HistoryProvider` – Historie aus SQLite laden.
9. `StarSystemProvider` – aktuelles System verwalten.
10. `WebApiProvider` – EDSM/Spansh-Aufrufe vorbereiten.
11. `JournalProvider` – Journal-Event-Verarbeitung.
12. `RouteProvider` – Routenlogik.
13. `StatusProvider` – Status.json-Verarbeitung.
14. `PlanetsOfInterestProvider` – POI-Filter.
15. `JournalHistoryImporter` – Import historischer Journale.
16. `HotkeyProvider` – globale Hotkeys.
17. `MainViewModel` – erzeugen und mit Providern verbinden.
18. `MainWindow` – erzeugen, `DataContext` setzen, anzeigen und Jot-Tracking aktivieren.
19. `viewModel.RestoreWindows()` – vorher geöffnete Fenster und Tab wiederherstellen.
20. `journalProvider.Initialize()` – erste Journal-Parsung starten.
21. Hotkey-Listener an `MainWindow` anhängen.

## Exception-Handler

### DispatcherUnhandledException

- Unbehandelte UI-Thread-Exceptions werden geloggt.
- Ein `MessageBox`-Fehlerdialog wird angezeigt.
- `e.Handled = true`
- Die Anwendung wird mit Exit-Code `1` beendet.

### AppDomain.CurrentDomain.UnhandledException

- Nicht-UI-Thread-Exceptions werden geloggt.
- Kein Benutzerdialog, da der Prozess meist unkontrolliert endet.

## Jot-Tracking

- Bibliothek: `Jot` (`JotSettingsProvider`)
- Speicherort: `%LOCALAPPDATA%\EDEA\jot`
- Getrackt für alle `Window`-Instanzen:
  - `Top`, `Left`, `Width`, `Height`, `WindowState`
- Persistierung beim `Closing`-Event.
- `MainWindow` erhält explizit den Namen `MainWindow`, damit Jot die Fenstergeometrie wiederherstellen kann.

## MainWindow

- Wird in `RunStartup` nach dem `MainViewModel` instanziiert.
- Enthält benutzerdefinierte Titelleisten-Steuerung (verschieben, minimieren, maximieren, schließen).
- Das geschlossene Hauptfenster löst `MainViewModel.OnMainWindowClosed` aus.
- Dort werden offene Fensterzustände persistiert und `Preferences.SaveUserSettings()` aufgerufen.

## OnExit

- `App.OnExit` benachrichtigt `StarSystemProvider` über `HandleApplicationShutdown()`.
- Anschließend wird die WPF-Anwendung beendet.

## Zusammenfassung des Lebenszyklus

```text
1. Prozessstart
2. App-Konstruktor (Logging, Mutex, Jot, Sprache, Exception-Handler)
3. OnStartup -> RunStartup
4. Einstellungen laden
5. SQLite / EDFileWatcher / Provider initialisieren
6. MainViewModel + MainWindow erzeugen
7. Fenster wiederherstellen
8. Journal-Parsung starten
9. Hotkeys registrieren
10. Benutzerinteraktion
11. OnMainWindowClosed -> Einstellungen speichern
12. OnExit -> Anwendung beenden
```
