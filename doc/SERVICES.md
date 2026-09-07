# EDEA-Services und Provider

Diese Dokumentation beschreibt die Diensteschicht von EDEA. Die Kern-Logik liegt plattformunabhängig in `src/EDEA.Core/Services/`; plattformspezifische Implementierungen (Avalonia) liegen in `src/EDEA.Avalonia/Services/` und werden über `PlatformServices` registriert.

## Übersicht – Kern-Services (`src/EDEA.Core/Services/`)

| Service | Zweck | Quelldatei |
|---------|-------|------------|
| `EDFileWatcher` | Überwacht das Elite-Dangerous-Saved-Games-Verzeichnis auf Änderungen an Journal, NavRoute und Status. | [`EDFileWatcher.cs`](../src/EDEA.Core/Services/EDFileWatcher.cs) |
| `JournalStore` | Liest die aktuelle Journal-Datei aus und feuert `JournalUpdated` mit neuen Zeilen. | [`JournalStore.cs`](../src/EDEA.Core/Services/JournalStore.cs) |
| `JournalProvider` | Parst Journal-Events und aktualisiert `StarSystem`/`Body`-Objekte. | [`JournalProvider.cs`](../src/EDEA.Core/Services/JournalProvider.cs) |
| `JournalHistoryImporter` | Importiert historische `Journal*.log`-Dateien im Hintergrund. | [`JournalHistoryImporter.cs`](../src/EDEA.Core/Services/JournalHistoryImporter.cs) |
| `StarSystemProvider` | Zentrale Koordination von aktuellem System, Route, Umgebung und UI-Events. | [`StarSystemProvider.cs`](../src/EDEA.Core/Services/StarSystemProvider.cs) |
| `HistoryProvider` | Verwaltet die Sternensystem-Historie, persistiert über `SQLiteStore`. | [`HistoryProvider.cs`](../src/EDEA.Core/Services/HistoryProvider.cs) |
| `RouteProvider` | Verwaltet Navigationsroute, Plotter-Routen-Import und Sperrstatus. | [`RouteProvider.cs`](../src/EDEA.Core/Services/RouteProvider.cs) |
| `StatusProvider` | Liest `Status.json` und meldet Activity, Location, GuiFocus und Treibstoff. | [`StatusProvider.cs`](../src/EDEA.Core/Services/StatusProvider.cs) |
| `PlanetsOfInterestProvider` | Prüft Planeten gegen benutzerdefinierte Klassifikationsfilter (POI). | [`PlanetsOfInterestProvider.cs`](../src/EDEA.Core/Services/PlanetsOfInterestProvider.cs) |
| `BiologyCatalogProvider` | Lädt den Biologie-Katalog aus `bio_catalog.json` (Artenregeln, Werte). | [`BiologyCatalogProvider.cs`](../src/EDEA.Core/Services/BiologyCatalogProvider.cs) |
| `BiologyRuleEvaluator` | Wertet Katalog-Regeln gegen Planet- und Systemdaten aus (BioScan-kompatibel). | [`BiologyRuleEvaluator.cs`](../src/EDEA.Core/Services/BiologyRuleEvaluator.cs) |
| `GalacticRegionProvider` | Galaktische Regionen, Guardian- und Tuber-Zonen aus `regions.json`. | [`GalacticRegionProvider.cs`](../src/EDEA.Core/Services/GalacticRegionProvider.cs) |
| `NebulaProvider` | Nebel-Volumen aus `nebulae.json`. | [`NebulaProvider.cs`](../src/EDEA.Core/Services/NebulaProvider.cs) |
| `CodexTracker` | Verfolgt Codex-Einträge pro Region und galaxisweit (Erstentdeckungen). | [`CodexTracker.cs`](../src/EDEA.Core/Services/CodexTracker.cs) |
| `EdDataProvider` | Liest statische Spieldaten aus `mc.dat` (Module, FSD, Guardian FSD Booster). | [`EdDataProvider.cs`](../src/EDEA.Core/Services/EdDataProvider.cs) |
| `SettingsProvider` | Lädt und speichert `UserSettings` als JSON; migriert Sprachphrasen-Defaults. | [`SettingsProvider.cs`](../src/EDEA.Core/Services/SettingsProvider.cs) |
| `SpanshService` | Kapselt Spansh-Routen- und Namensauflösung gegenüber `WebApiProvider`. | [`SpanshService.cs`](../src/EDEA.Core/Services/SpanshService.cs) |
| `WebApiProvider` | Koordiniert alle EDSM-, Spansh- und Canonn-Web-API-Aufrufe. | [`WebApiProvider.cs`](../src/EDEA.Core/Services/WebApiProvider.cs) |
| `WebApiLoadingStatusChangedEventHandler` | Delegat für Ladezustandsänderungen des `WebApiProvider`. | [`WebApiLoadingStatusChangedEventHandler.cs`](../src/EDEA.Core/Services/WebApiLoadingStatusChangedEventHandler.cs) |
| `PlatformServices` | Statische Registrierung der plattformspezifischen Service-Implementierungen. | [`PlatformServices.cs`](../src/EDEA.Core/Services/PlatformServices.cs) |

## Übersicht – Plattform-Services (`src/EDEA.Avalonia/Services/`)

Implementierungen der Abstraktionen aus `src/EDEA.Core/Services/Abstractions/`:

| Service | Interface | Zweck |
|---------|-----------|-------|
| `AvaloniaSpeechService` | `ISpeechService` | Sprachausgabe per SayIt (Microsoft Edge TTS, online) und NAudio-Wiedergabe. |
| `AvaloniaGlobalHotkeyService` | `IGlobalHotkeyService` | Registriert globale Hotkeys (Win32 `RegisterHotKey`). |
| `AvaloniaWindowStateService` | `IWindowStateService` | Persistiert Fensterpositionen in `windowstate.json` unter `%LOCALAPPDATA%\EDEA.Core`. |
| `AvaloniaHudWindowService` | `IHudWindowService` | Steuert das HUD-Overlay (Click-Through, Transparenz, Position). |
| `AvaloniaDialogService` | `IDialogService` | Standard-Dialoge (Eingabe, Auswahl, Meldungen). |
| `AvaloniaClipboardService` | `IClipboardService` | Zwischenablage-Zugriff. |
| `AvaloniaScreenService` | `IScreenService` | Bildschirmgeometrie. |
| `AvaloniaColorThemeService` | `IColorThemeService` | Wendet Farben und Anzeigegröße an. |
| `AvaloniaDispatcher` | `IDispatcher` | UI-Thread-Marshal über den Avalonia-Dispatcher. |
| `AvaloniaUiTimerService` | `IUiTimerService` | UI-Timer für periodische Aktualisierungen. |
| `AvaloniaPlatformService` | `IPlatformService` | Plattform-Hilfsfunktionen (Prozesse, Pfade). |

## Datei- und Journal-Services

### `EDFileWatcher`

- **Zweck:** `FileSystemWatcher`-basierte Beobachtung des Saved-Games-Ordners.
- **Events:** `JournalFileChanged`, `NavRouteFileChanged`, `StatusFileChanged`.

### `JournalStore`

- **Zweck:** Liest das aktuelle Journal inkrementell aus und stellt neue Zeilen bereit.
- **Event:** `JournalUpdated`.
- **Intern:** Liest in Blöcken, merkt sich die Leseposition und puffert mit Retry-Logik.

### `JournalProvider`

- **Zweck:** Parst Journal-Zeilen und aktualisiert `StarSystem`/`Body`-Objekte, inklusive Scan-Felder für die Biologie-Vorhersage (Atmosphäre, Druck, Zusammensetzung, Materialien, Umlaufzeit, Eltern-Sterne).
- **Event:** `ParsedJournalDataUpdated`.

### `JournalHistoryImporter`

- **Zweck:** Importiert historische `Journal*.log`-Dateien; bereits importierte, unveränderte Dateien werden übersprungen (Import-Index in der Datenbank).
- **Eigenschaften:** `StatusData`, `StatusPercentage`.

## Daten- und Zustands-Services

### `StarSystemProvider`

- **Zweck:** Zentrale Datenhalde und Koordinator: `CurrentSystem`, `DestinationSystem`, `CurrentPlanet`, `StarSystemsOnRoute`, `SurroundingStarSystems`, `CurrentShip`, `CurrentActivity`, `CommanderName`.
- **Events:** `GuiDataUpdated`, `GuiLocationDataUpdated`, `CurrentPlanetChanged`, `RouteLoadingStatusChanged`, `SurroundingsLoadingStatusChanged` u. a.

### `HistoryProvider`

- **Zweck:** `ConcurrentDictionary<long, StarSystem>` der besuchten Systeme, synchronisiert mit `SQLiteStore`.
- **Event:** `HistoryUpdated`.

### `RouteProvider`

- **Zweck:** Verwaltet die aktuelle Route (NavRoute, Plotter- und Spansh-Import, Sperrstatus).
- **Events:** `RouteChanged`, `SystemsOnRouteChanged`.

### `StatusProvider`

- **Zweck:** Parst `Status.json` und meldet Spielstatus an den `StarSystemProvider`.
- **Events:** `StatusUpdated`, `LocationUpdated`, `GuiFocusUpdated`, `ShipFuelUpdated`.

### `PlanetsOfInterestProvider`

- **Zweck:** Prüft Planeten gegen die in den Einstellungen hinterlegten `PlanetClassification`-Filter.

### `SettingsProvider`

- **Zweck:** Persistiert `UserSettings` als JSON in `%LOCALAPPDATA%\EDEA.Core\settings.json`.
- **Migration:** Ersetzt gespeicherte Sprachphrasen, die noch einem bekannten Standardtext entsprechen, durch den Standard der aktuellen UI-Sprache – beim Laden und live bei Sprachwechsel (`Resources.CultureChanged`).

## Biologie-Services

### `BiologyCatalogProvider`

- **Zweck:** Lädt `bio_catalog.json` (editierbarer Katalog mit Arten, Farbvarianten, Regeln und Vista-Genomics-Werten) und liefert die Vorhersage-Pipeline: Kandidaten, Farben, Eingrenzung über die Anzahl biologischer Signale, Cache.
- **Ersetzt:** das frühere `GeneraIndexProvider`/`gc.json`-Verfahren.

### `BiologyRuleEvaluator`

- **Zweck:** Wertet Katalog-Kriterien aus (Atmosphäre, Gravitation, Temperatur, Druck, Stern- und Körpertypen, Regionen, Nebel, Guardian-/Tuber-Zonen, Vulkanismus u. a.) – BioScan-kompatibel.
- **Robustheit:** Fehlerhafte Regeln führen zu „kein Treffer" plus Log-Warnung statt zum Abbruch.

### `GalacticRegionProvider` / `NebulaProvider`

- **Zweck:** Räumliche Zuordnung von Systemen zu Galaxie-Regionen, Guardian-/Tuber-Zonen und Nebeln anhand der JSON-Daten.

### `CodexTracker`

- **Zweck:** Führt Buch über bekannte Codex-Einträge (persistiert in `CodexScans`), um Erstentdeckungen galaxisweit und regionsbezogen zu erkennen.

## Web- und Externe-Services

### `WebApiProvider`

- **Zweck:** Zentrale Anlaufstelle für EDSM, Spansh und Canonn.
- **Methoden:** EDSM-System- und Umgebungsabfragen, Spansh-Basisdaten sowie Galaxy-/Neutron-Routen, Canonn-Biostatistik.
- **Event:** `WebApiLoadingStatusChanged`.
- Siehe [`API_INTEGRATION.md`](./API_INTEGRATION.md).

### `SpanshService`

- **Zweck:** Höherwertige Spansh-Operationen für den Routenplotter (`GetRouteByNameAsync`, `ResolveSystemId64Async`, `SearchSystemNamesAsync`).

## Hinweise zur Migration vom WPF-Original

- `GeneraIndexProvider` + `gc.json` wurden durch `BiologyCatalogProvider`/`BiologyRuleEvaluator` + `bio_catalog.json` ersetzt.
- `SpeechProvider` wurde durch `AvaloniaSpeechService` (SayIt/Edge-TTS statt `System.Speech`) ersetzt.
- `HotkeyProvider` wurde durch `AvaloniaGlobalHotkeyService` ersetzt.
- `JotSettingsProvider`/`Jot` wurde durch `AvaloniaWindowStateService` (einfache JSON-Persistenz) ersetzt.
- `SettingsProvider` (Jot-basiert) wurde durch den JSON-basierten `SettingsProvider` ersetzt.
