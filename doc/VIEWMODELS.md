# ViewModels

Diese Dokumentation beschreibt die ViewModel-Schicht von EDEA. Die ViewModels kapseln die Präsentationslogik, werten Daten der Services aus und stellen bindbare Properties und Commands für die XAML-Views bereit.

## Grundlagen

Alle ViewModels basieren auf `CommunityToolkit.Mvvm`:

- `ViewModelBase` (`src/EDEA/ViewModels/ViewModelBase.cs`) erbt von `ObservableObject` und dient als gemeinsame Basisklasse.
- `TabViewModel` (`src/EDEA/ViewModels/TabViewModel.cs`) erbt von `ViewModelBase` und fügt Tab-spezifische Eigenschaften `TabHeader`, `TabVisibility` und `TabName` hinzu.
- Für Benachrichtigungen an die UI wird `OnPropertyChanged` verwendet.
- Commands werden in der Regel in Klassen unter `src/EDEA/Commands/` implementiert und als `ICommand` an die ViewModels gebunden.

## Zentrales ViewModel: `MainViewModel`

`MainViewModel` (`src/EDEA/ViewModels/MainViewModel.cs`) koordiniert die gesamte Hauptansicht. Es wird in `App` erzeugt und an `MainWindow` als `DataContext` zugewiesen.

### Verantwortlichkeiten

- Verwaltung der Sub-ViewModels für Tabellen, HUD, Einstellungen und Dialoge.
- Empfang von Änderungen aus den Providern (`StarSystemProvider`, `RouteProvider`, `WebApiProvider`, `StatusProvider`, `HistoryProvider`, `HotkeyProvider`, `PlanetsOfInterestProvider`).
- Steuerung der sichtbaren Tabs und des Fensterzustands.
- Bereitstellung der Menü-Commands.

### Konstruktorabhängigkeiten

| Service | Zweck |
|---------|-------|
| `HotkeyProvider` | Verfügbare Hotkeys |
| `WebApiProvider` | EDSM/Spansh-Ladezustand |
| `StarSystemProvider` | Aktuelles Sternensystem und aktueller Planet |
| `HistoryProvider` | Historiendaten |
| `RouteProvider` | Aktive Route, Sperrstatus |
| `StatusProvider` | Elite-Status (GUI-Fokus, On-Foot) |
| `PlanetsOfInterestProvider` | Planeten-Filter und POIs |
| `JournalHistoryImporter` | Import historischer Journal-Dateien |

### Wichtige Properties

| Property | Bedeutung |
|----------|-----------|
| `TabViewModels` | `ObservableCollection<TabViewModel>` mit den fünf Haupt-Tabs |
| `SelectedTabIndex` | Index des aktuell gewählten Tabs |
| `CurrentSystemViewModel` | `StarSystemViewModel` für das aktuelle System |
| `CurrentPlanet` | `BodyViewModel?` für den aktuellen Planeten |
| `CurrentStatus` | Text der aktuellen Aktivität |
| `Title` | Fenstertitel |
| `DataIsLoading` | Gibt an, ob gerade Web-API- oder Routen-Daten geladen werden |
| `IsRouteAvailable` | Gibt an, ob eine benutzerdefinierte Route aktiv ist |
| `Hotkeys` | Verfügbare Tastenkürzel |

### Commands

| Command | Beschreibung |
|---------|--------------|
| `ReloadEdsmDataCommand` | EDSM-Daten für das aktuelle System neu laden |
| `ShowAboutWindowCommand` | Über-Fenster anzeigen |
| `OpenCloseHudWindowCommand` | HUD-Fenster öffnen oder schließen |
| `ShowFeedbackReportIssueWindowCommand` | Feedback-/Fehlermelde-Fenster öffnen |
| `ShowPreferencesWindowCommand` | Einstellungsfenster öffnen |
| `ClearHistoryCommand` | Historie/Expedition zurücksetzen |
| `GenerateClearPlotterRouteCommand` | Neutronen-Route erzeugen oder (falls nicht gesperrt) löschen |
| `CopySystemNameToClipboardCommand` | Systemname in die Zwischenablage kopieren |
| `ImportJournalHistoryCommand` | Journal-Historie importieren |
| `EnableDisableHudWindowMousePassThroughCommand` | Maus-Durchklick für HUD umschalten |
| `LockUnlockRouteCommand` | Aktuelle Route sperren/entsperren (ohne Löschen der Plotter-Route) |
| `ImportSpanshRouteCommand` | Spansh-Route importieren |

## Tab-Verwaltung

Das Hauptfenster zeigt fünf Tabs an, deren Inhalte über `TabViewModel`-Instanzen gesteuert werden.

### Standard-Tabs

| Tab | ViewModel | Sichtbarkeit |
|-----|-----------|--------------|
| Route | `NavRouteTableViewModel` | `Visible` |
| Körper | `BodyTableViewModel` | `Visible` |
| Biologisches | `GenusTableViewModel` | `Collapsed` |
| Umgebung | `SurroundingsTableViewModel` | `Visible` |
| Historie | `HistoryViewModel` | `Visible` |

`MainViewModel.SelectedTabIndex` steuert den aktiven Tab. Änderungen lösen `SelectedTabIndexChanged` aus. Mit `OpenTabOfType(Type, bool)` wechselt das ViewModel je nach Elite-Dangerous-Aktivität automatisch den Tab (sofern `AutomaticTabSwitching` aktiviert ist):

- `Activity.None` / `Other` → Historie
- `Activity.ExploreSystem` → Körper
- `Activity.GalaxyMap` / `Jump` → Route
- `Activity.ExplorePlanet` → Biologisches (bei vorhandenen Genus-Daten)

`SelectTabByName(string)` erlaubt eine schlüsselwortbasierte Tab-Auswahl.

## Wichtige Detail-ViewModels

### `StarSystemViewModel`

Verpackt ein `StarSystem`-Model für die Darstellung in der Route- und Umgebungs-Tabelle. Es liefert bindbare Eigenschaften wie `Name`, `StarClass`, `JumpDistance`, `ExplorationStatus`, kartografische Werte sowie Counts für Planeten, Terraformierbare, Landbare und wertvolle Körper. Darüber hinaus berechnet es den kombinierten Erforschungsstatus aus Journal- und EDSM-Daten.

### `BodyViewModel`

Verpackt ein `Body` für die Darstellung in der Körper-Tabelle. Enthält Eigenschaften für Name, Typ, Atmosphäre, Gravitation, Temperatur, Oberflächenscan-Status, Ringe, geologische/biologische Signale, Genus-Vista-Genomics-Werte und kartografische Einnahmen. Sortierwerte (z. B. `DistanceSort`, `CartographicValueSort`) dienen DataGrid-Spalten.

### `HudViewModel`

Verwaltet das Overlay-Fenster (`HudWindow`). Es reagiert auf `SelectedTabIndexChanged` und `GuiHudDataUpdated` des `MainViewModel`, blendet das Fenster je nach GUI-Fokus ein oder aus und kann die Mauspass-through-Eigenschaft per `SetMousePassThrough` umschalten. `CurrentViewModel` bestimmt, welche HUD-Tabelle (`BodyHudTableView`, `NavRouteHudTableView`, `GenusHudTableView`) angezeigt wird.

### `RoutePlotterViewModel`

Steuert das `RoutePlotterWindow` für Neutronen-Routenberechnungen über Spansh. Der Nutzer wählt Start- und Zielsystem, Schiffsdaten (FSD, Guardian-Booster, Supercharged) werden automatisch ermittelt. `ShowRoutePlotterWindow` erzeugt das Fenster, `requestRouteCalculation` startet die API-Anfrage, deren Ergebnis über `RouteProvider.ImportPlotterRoute` in die Route-Tabelle übernommen wird.

### `PreferencesViewModel`

Verwaltet das `PreferencesWindow`. Es bindet Einstellungen aus `Preferences.Application`, `Preferences.HudWindow` und `Preferences.Spansh`, stellt Farbwahl, Display-Größe, HUD-Spalten, Sprachausgabe, Hotkeys und benutzerdefinierte Planetenfilter dar. Commands wie `SaveAndClosePreferencesCommand` oder `RestoreDefaultPreferencesCommand` werden im Konstruktor bzw. beim Öffnen des Fensters zugewiesen.

## Übersicht aller ViewModels

| Klasse | Datei | Zweck |
|--------|-------|-------|
| `ViewModelBase` | `ViewModelBase.cs` | Gemeinsame Basisklasse aller ViewModels |
| `TabViewModel` | `TabViewModel.cs` | Basisklasse für Haupt-Tabs |
| `MainViewModel` | `MainViewModel.cs` | Zentrales ViewModel der Hauptansicht |
| `AboutViewModel` | `AboutViewModel.cs` | Daten für das Über-Fenster |
| `FeedbackReportIssueViewModel` | `FeedbackReportIssueViewModel.cs` | Daten für Feedback-/Fehlermelde-Fenster |
| `HudViewModel` | `HudViewModel.cs` | HUD-Overlay-Fenster und Inhaltssteuerung |
| `StarSystemViewModel` | `StarSystemViewModel.cs` | Darstellung eines Sternensystems |
| `BodyViewModel` | `BodyViewModel.cs` | Darstellung eines Himmelskörpers |
| `BodyTableViewModel` | `BodyTableViewModel.cs` | Logik für die Körper-Tabelle |
| `GenusTableViewModel` | `GenusTableViewModel.cs` | Logik für die biologische Tabelle |
| `GenusViewModel` | `GenusViewModel.cs` | Darstellung eines Genus-Eintrags |
| `GenusView` | `GenusView.cs` | Darstellungshilfe für Genus-Daten |
| `GenusClassificationViewModel` | `GenusClassificationViewModel.cs` | Darstellung einer Genus-Klassifizierung |
| `HistoryViewModel` | `HistoryViewModel.cs` | Logik für den Historie-Tab |
| `HistoryDataViewModel` | `HistoryDataViewModel.cs` | Darstellung aggregierter Historiendaten |
| `HotkeyViewModel` | `HotkeyViewModel.cs` | Darstellung und Belegung eines Hotkeys |
| `JournalHistoryImportViewModel` | `JournalHistoryImportViewModel.cs` | Fortschritt und Status des Journal-Imports |
| `NavRouteTableViewModel` | `NavRouteTableViewModel.cs` | Logik für die Routen-Tabelle |
| `PlanetClassificationViewModel` | `PlanetClassificationViewModel.cs` | Darstellung einer Planeten-Klassifizierung |
| `PreferencesViewModel` | `PreferencesViewModel.cs` | Logik für das Einstellungsfenster |
| `RingView` | `RingView.cs` | Darstellungshilfe für Ringe |
| `RingViewModel` | `RingViewModel.cs` | Darstellung eines Ring-Eintrags |
| `RoutePlotterViewModel` | `RoutePlotterViewModel.cs` | Logik für den Neutronen-Plotter |
| `RouteView` | `RouteView.cs` | Darstellungshilfe für Routen |
| `SurroundingsTableViewModel` | `SurroundingsTableViewModel.cs` | Logik für die Umgebungs-Tabelle |
| `SurroundingView` | `SurroundingView.cs` | Darstellungshilfe für benachbarte Systeme |
