# EDEA – ViewModels

ViewModels sind aufgeteilt: die inhaltsbezogenen Tabellen-/Zeilen-ViewModels liegen in `src/EDEA.Core/ViewModels/` (plattformunabhängig), anwendungsnahe ViewModels in `src/EDEA.Avalonia/ViewModels/`. Basis ist `CommunityToolkit.Mvvm` (`ObservableObject`, `RelayCommand`); gemeinsame Basisklasse ist `ViewModelBase`.

## Core-ViewModels (`src/EDEA.Core/ViewModels/`)

| ViewModel | Rolle |
|-----------|-------|
| `TabViewModel` | Basisklasse aller Tabs: `TabHeader` (ressourcen-key-basiert, live sprachumschaltbar), `TabVisibility`, `TabName`. |
| `NavRouteTableViewModel` | Route-Tab (NavRoute, Plotter-/Spansh-Route, Sprungliste). |
| `BodyTableViewModel` | Himmelskörper-Tab. |
| `BodyViewModel` | Zeilenmodell eines Körpers (Status, Werte, Signale, Icon-Flags, Tooltips). |
| `GenusTableViewModel` | Biologie-Tab (Gattungen + vorhergesagte Arten). |
| `GenusViewModel` | Zeilenmodell einer Gattung (Scans, Klonkolonie-Distanzen, Wert). |
| `GenusClassificationViewModel` | Zeilenmodell einer vorhergesagten Art (Reichweite, Wert, Codex-Status). |
| `SurroundingsTableViewModel` | Umgebungs-Tab (EDSM-Sphäre). |
| `HistoryViewModel`, `HistoryDataViewModel` | Historie-Tab: Reise-/Gesamtstatistik. |
| `StarSystemViewModel` | System-Zeile (Route/Umgebung) mit Fortschritt und Tooltip-Daten. |
| `RingViewModel` | Ring-Zeile (Tooltip `BodyRingsTooltip`). |
| `PlanetClassificationViewModel` | POI-Kriteriensatz-Zeile. |
| `HotkeyViewModel` | Zeile in der Hotkey-Belegung. |
| `RouteView` | Zeilenmodell für die Routenliste. |
| `ViewModelBase` | Gemeinsame Basisklasse. |

## Avalonia-ViewModels (`src/EDEA.Avalonia/ViewModels/`)

| ViewModel | Rolle |
|-----------|-------|
| `MainViewModel` | Hauptfenster: Tabs, Menü-Commands, Statusleiste, Titel, Fenster-Wiederherstellung. |
| `HudViewModel` | HUD-Fenster (ausgewählte Ansicht, Transparenz, Pass-Through). |
| `AboutViewModel` | Info-Fenster (Version). |
| `FeedbackReportIssueViewModel` | Feedback-Formular. |
| `JournalHistoryImportViewModel` | Journal-Import-Dialog (Fortschritt, Ergebnis). |
| `RoutePlotterViewModel` | Neutron-/Spansh-Routenplotter (Parameter, Laufstatus). |

## Datenfluss

Die ViewModels beobachten Provider-Events (`GuiDataUpdated`, `RouteChanged`, `HistoryUpdated`, `StatusUpdated` u. a.) aus `EDEA.Core.Services` und heben geänderte Eigenschaften per `OnPropertyChanged`. Lokalisierte Anzeigetexte werden bei `Resources.CultureChanged` erneut gemeldet (Live-Sprachwechsel).
