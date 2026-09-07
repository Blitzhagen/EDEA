# EDEA – Aktionen und Befehle

Die früheren separaten `ICommand`-Klassen (`src/EDEA/Commands/`, WPF) existieren nicht mehr. UI-Aktionen sind heute:

1. **`ICommand`-Properties auf dem `MainViewModel`** (`src/EDEA.Avalonia/ViewModels/MainViewModel.cs`), erzeugt über `CommunityToolkit.Mvvm` (`RelayCommand`) und aus den Menüs per `{Binding}` gebunden.
2. **Code-Behind-Handler** in den Fenstern (z. B. `PreferencesWindow.axaml.cs`) für Einstellungs-Interaktionen.
3. **Globale Hotkeys** über `AvaloniaGlobalHotkeyService` (`IGlobalHotkeyService`), die auf dieselben ViewModel-Aktionen zugreifen.

## Befehle des MainViewModel

| Command | Aktion |
|---------|--------|
| `ShowAboutWindowCommand` | Öffnet das Info-Fenster. |
| `ShowPreferencesWindowCommand` | Öffnet die Einstellungen. |
| `ShowFeedbackReportIssueWindowCommand` | Öffnet Feedback/Problem-Melden. |
| `OpenCloseHudWindowCommand` | HUD-Fenster ein-/ausblenden. |
| `ToggleHudMousePassThroughCommand` | HUD-Maus-Durchgriff umschalten. |
| `ImportJournalHistoryCommand` | Historischen Journal-Import starten. |
| `ReloadEdsmDataCommand` | EDSM-Daten des aktuellen Systems neu laden. |
| `GenerateClearPlotterRouteCommand` | Neutron-Plotter-Route erzeugen/löschen. |
| `LockUnlockRouteCommand` | Route sperren/entsperren. |
| `ImportSpanshRouteCommand` | Spansh-Route importieren. |

Weitere Aktionen als Methoden: `CopyNextSystemToClipboard()`, `OpenRouteTab()`, `RefreshMenuItems()`, `ToggleHudMousePassThrough()`, `RestoreWindows()`, `OnMainWindowClosing()`.

## Globale Hotkeys

- Verwaltung: `AvaloniaGlobalHotkeyService` (Win32 `RegisterHotKey`).
- Belegung in den Einstellungen unter **Globale Hotkeys** (`HotkeyViewModel`, `UserSettingsHotkeys`); `HotkeyInputDialog` erfasst neue Tastenkombinationen.
- Belegbare Aktionen entsprechen den `HotkeyId`-Werten (`src/EDEA.Core/Enums/HotkeyId.cs`) – u. a. HUD ein/aus, Mouse-Pass-Through, Tab-Wechsel, Systemname kopieren, Sprachausgabe stoppen.

## Fenster- und Tab-Interaktionen

- Menüeinträge in `Views/MainWindow.axaml` binden an die Commands des `MainViewModel`.
- Tab-Sichtbarkeit und automatischer Wechsel laufen über `TabViewModel`/`TabVisibilityConverter`.
- Daten-Grids: Sortierung über `DataGridSingleSortBehavior` (einzelne Spalte) und `SortMemberPath`; Zeilen-Kontextmenüs/Tooltips über Templates in `ToolTipTemplates.axaml`.
