# EDEA – Befehle

Dieses Dokument beschreibt alle in `src/EDEA/Commands/` implementierten Befehle. Befehle kapseln UI-Operationen und leiten diese an Provider, ViewModels oder Dialoge weiter. Alle abgeleiteten Befehle setzen auf `CommandBase` auf.

## CommandBase

`CommandBase` ist die abstrakte Basisklasse für alle Befehle und implementiert `ICommand`.

| Eigenschaft / Methode | Beschreibung |
| --- | --- |
| `CanExecute(object? parameter)` | Gibt standardmäßig `true` zurück; kann in abgeleiteten Klassen überschrieben werden. |
| `Execute(object? parameter)` | Abstrakte Ausführungslogik; muss in jedem Befehl implementiert werden. |
| `CanExecuteChanged` | Ereignis, das auslöst, wenn sich die Ausführbarkeit ändert. |
| `OnCanExecuteChanged()` | Löst `CanExecuteChanged` aus. |

## Befehlsübersicht

| Befehl | Zweck | Basisklasse | Wichtige Parameter |
| --- | --- | --- | --- |
| `AddCustomPlanetFilterCommand` | Fügt der Einstellungsansicht einen neuen benutzerdefinierten Planetenfilter hinzu. | `CommandBase` | `PreferencesViewModel preferencesViewModel` |
| `AssignHotkeyCommand` | Weist einem Hotkey eine neue Tastenkombination zu. | `CommandBase` | `HotkeyProvider hotkeyProvider`, `PreferencesWindow preferencesWindow` |
| `CancelPreferencesCommand` | Bricht ausstehende Einstellungsänderungen ab und schließt das Einstellungsfenster. | `CommandBase` | `PreferencesViewModel viewModel`, `Window window` |
| `ClearHistoryCommand` | Löscht nach Rückfrage die aktuelle Trip-Historie oder die gesamte Explorationshistorie. | `CommandBase` | `MainViewModel mainViewModel`, `HistoryProvider historyProvider`, `StarSystemProvider starSystemProvider` |
| `CloseWindowCommand` | Schließt das zugehörige WPF-Fenster. | `CommandBase` | `Window window` |
| `CopyToClipboardCommand` | Kopiert Texte oder numerische Werte in die Zwischenablage und zeigt optional ein Popup an. | `CommandBase` | `Execute(parameter)`: `object?` (Text, Zahl oder `CopyToClipboardCommandParameter`) |
| `EnableDisableHudWindowMousePassThroughCommand` | Schaltet den Maus-Durchgriff des HUD-Fensters um. | `CommandBase` | `HudViewModel hudViewModel` |
| `GenerateClearPlotterRouteCommand` | Löscht eine vorhandene Plotter-Route oder öffnet das Routen-Plotter-Fenster. | `CommandBase` | `RoutePlotterViewModel routePlotterViewModel`, `RouteProvider routeProvider`, `Action? onCleared` |
| `ImportJournalHistoryCommand` | Öffnet das Fenster zum Importieren der Journal-Historie. | `CommandBase` | `JournalHistoryImportViewModel importJournalHistoryViewModel` |
| `ImportSpanshRouteCommand` | Importiert eine Spansh-Routen-Datei in die Anwendung. | `CommandBase` | `MainViewModel mainViewModel`, `RouteProvider routeProvider` |
| `LoadEdsmSystemDataCommand` | Lädt EDSM-Systemdaten und wechselt in den Bodies-Tab. | `CommandBase` | `MainViewModel mainViewModel`, `StarSystemProvider starSystemProvider` |
| `LockUnlockRouteCommand` | Sperrt oder entsperrt die aktuelle Route. | `CommandBase` | `MainViewModel mainViewModel`, `RouteProvider routeProvider` |
| `MailToFeedbackReportIssueMailAddressCommand` | Öffnet das Standard-Mailprogramm mit vorkonfiguriertem Feedback. | `CommandBase` | – |
| `OpenCloseHudWindowCommand` | Öffnet oder schließt das HUD-Fenster je nach aktuellem Zustand. | `CommandBase` | `HudViewModel hudViewModel` |
| `OpenLogfileFolderCommand` | Öffnet den Anwendungs-Logordner im Windows-Explorer. | `CommandBase` | – |
| `PlaySpeechCommand` | Spielt die aktuell ausgewählte Sprachausgabevorschau ab. | `CommandBase` | `PreferencesViewModel preferencesViewModel` |
| `RemoveCustomPlanetFilterCommand` | Entfernt nach Rückfrage den ausgewählten benutzerdefinierten Planetenfilter. | `CommandBase` | `PreferencesViewModel preferencesViewModel` |
| `RenameCustomPlanetFilterCommand` | Benennt den ausgewählten benutzerdefinierten Planetenfilter um. | `CommandBase` | `PreferencesViewModel preferencesViewModel` |
| `RestoreDefaultPreferencesCommand` | Stellt Standardeinstellungen nach Bestätigung wieder her und schließt das Fenster. | `CommandBase` | `PreferencesViewModel preferencesViewModel`, `PreferencesWindow preferencesWindow` |
| `SaveAndClosePreferencesCommand` | Speichert alle Einstellungen und schließt das Einstellungsfenster. | `CommandBase` | `PreferencesViewModel preferencesViewModel`, `PreferencesWindow preferencesWindow` |
| `SaveSettingsCommand` | Speichert die aktuellen Einstellungen persistiert. | `CommandBase` | `SettingsProvider settingsProvider` |
| `SelectStringsFromStringListCommand` | Öffnet einen Dialog zur Auswahl von Zeichenketten für die Planetenklassifikation. | `CommandBase` | `PreferencesViewModel preferencesViewModel`; `Execute(parameter)`: `UserSelectableInputStringListsKey` |
| `SetJournalFolderCommand` | Legt das Elite-Dangerous-Saved-Game-Verzeichnis mit den Journal-Dateien fest. | `CommandBase` | `PreferencesViewModel preferencesViewModel` |
| `ShowAboutWindowCommand` | Öffnet das Info-Fenster. | `CommandBase` | `AboutViewModel aboutViewModel` |
| `ShowFeedbackReportIssueWindowCommand` | Öffnet das Feedback- und Issue-Report-Fenster. | `CommandBase` | `FeedbackReportIssueViewModel feedbackReportIssueViewModel` |
| `ShowPreferencesWindowCommand` | Öffnet das Einstellungsfenster. | `CommandBase` | `PreferencesViewModel preferencesViewModel` |

## Hilfsobjekte

| Typ | Zweck | Wichtige Parameter |
| --- | --- | --- |
| `CopyToClipboardCommandParameter` | Parameter-Container für `CopyToClipboardCommand`. | `string text`, `Popup popup` |
