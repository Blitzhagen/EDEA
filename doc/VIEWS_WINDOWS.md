# EDEA – Views und Fenster

Alle UI-Dateien liegen in `src/EDEA.Avalonia/` (AXAML, Avalonia 11). Statische Texte werden über `{l:Loc Key}` lokalisiert (Live-Sprachwechsel).

## Views (`Views/`)

| View | Inhalt |
|------|--------|
| `MainWindow.axaml` | Hauptfenster: Menüleiste, TabControl (Route, Himmelskörper, Biologie, Umgebung, Historie), Statusleiste. |
| `RouteTableView.axaml` | Routen-Tabelle (Hauptfenster). |
| `NavRouteHudTableView.axaml` | Routen-Tabelle (HUD). |
| `BodyTableView.axaml` | Himmelskörper-Tabelle (Hauptfenster). |
| `BodyHudTableView.axaml` | Himmelskörper-Tabelle (HUD). |
| `GenusTableView.axaml` | Biologie-Tabelle (Hauptfenster, inkl. Vorhersage-Grid). |
| `GenusHudTableView.axaml` | Biologie-Tabelle (HUD). |
| `SurroundingsTableView.axaml` | Umgebungs-Tabelle. |
| `HistoryTableView.axaml` | Historie/Statistik. |
| `BodyRingsTooltip.axaml` | Tooltip-View für Ringdaten eines Körpers. |

Tooltip-Templates für komplexe Zellen-Tooltips liegen in `ToolTipTemplates.axaml` (App-Ressourcen).

## Fenster (`Windows/`)

| Fenster | Zweck |
|---------|-------|
| `PreferencesWindow.axaml` | Einstellungen (Aussehen/Farben/Größe/Sprache, HUD, Sprachausgabe, Planets of Interest, Hotkeys, Konfiguration). |
| `HudWindow.axaml` | Transparentes Overlay mit den Tabs Route, Himmelskörper, Biologie; Click-Through-fähig. |
| `RoutePlotterWindow.axaml` | Neutron-/Spansh-Routenplotter inkl. Fleet-Carrier-Modus. |
| `JournalHistoryImportWindow.axaml` | Fortschrittsdialog des Journal-Imports. |
| `AboutWindow.axaml` | Info-Fenster (Version, Libraries, Credits). |
| `FeedbackReportIssueWindow.axaml` | Feedback-/Problem-Formular. |
| `HotkeyInputDialog.axaml` | Erfasst neue Hotkey-Kombinationen. |
| `InputStringDialogWindow.axaml` | Generischer Texteingabe-Dialog. |
| `InputStringListDialogWindow.axaml` | Generischer Auswahllisten-Dialog. |
| `MessageBoxWindow.axaml` | Einfache Meldungsbox. |

## Styles und Ressourcen

- `App.axaml`: globale Styles, Brushes (Farbschema aus den Einstellungen), Tooltip-Templates-Einbindung.
- Anzeigegröße/Farben werden zur Laufzeit vom `AvaloniaColorThemeService` als `DynamicResource` gesetzt.
- HUD-Styles: `Classes="Hud"` auf den DataGrids + kompakte Header über `TabHeaderToHeightConverter`.

## Fensterzustand

`AvaloniaWindowStateService` persistiert Position/Größe/Sichtbarkeit der Fenster in `%LOCALAPPDATA%\EDEA.Core\windowstate.json`; `MainViewModel.RestoreWindows()` öffnet beim Start zuvor offene Fenster (u. a. HUD).
