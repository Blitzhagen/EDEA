# EDEA-Services und Provider

Diese Dokumentation beschreibt die Diensteschicht von EDEA. Fast alle Services sind als Singletons ausgelegt und werden in [`App.xaml.cs`](../src/EDEA/App.xaml.cs) beim Start initialisiert. Sie kapseln Dateiüberwachung, Journal-Verarbeitung, externe Web-APIs, Datenbankzugriff, Sprachausgabe und UI-nahe Zustände.

## Übersicht aller Services

| Service | Zweck | Quelldatei |
|---------|-------|------------|
| `EDFileWatcher` | Überwacht das Elite-Dangerous-Saved-Games-Verzeichnis auf Änderungen an Journal, NavRoute und Status. | [`src/EDEA/Services/EDFileWatcher.cs`](../src/EDEA/Services/EDFileWatcher.cs) |
| `EdDataProvider` | Liest statische Spieldaten aus `mc.dat` (Module, FSD, Guardian FSD Booster). | [`src/EDEA/Services/EdDataProvider.cs`](../src/EDEA/Services/EdDataProvider.cs) |
| `GeneraIndexProvider` | Lädt biologische Klassifikationsdaten aus `gc.json` und stellt Genus-Klassifikationen bereit. | [`src/EDEA/Services/GeneraIndexProvider.cs`](../src/EDEA/Services/GeneraIndexProvider.cs) |
| `HistoryProvider` | Verwaltet die im Speicher gehaltene Sternensystem-Historie, persistiert über `SQLiteStore`. | [`src/EDEA/Services/HistoryProvider.cs`](../src/EDEA/Services/HistoryProvider.cs) |
| `HotkeyProvider` | Registriert globale Hotkeys und verknüpft sie mit `MainViewModel`-Aktionen. | [`src/EDEA/Services/HotkeyProvider.cs`](../src/EDEA/Services/HotkeyProvider.cs) |
| `JotSettingsProvider` | Zentrale Instanz des `Jot`-Trackers für Fensterpositionen und -zustände. | [`src/EDEA/Services/JotSettingsProvider.cs`](../src/EDEA/Services/JotSettingsProvider.cs) |
| `JournalHistoryImporter` | Importiert historische Journal-Dateien im Hintergrund (`BackgroundWorker`). | [`src/EDEA/Services/JournalHistoryImporter.cs`](../src/EDEA/Services/JournalHistoryImporter.cs) |
| `JournalProvider` | Parst aktuelle Journal-Events und aktualisiert Sternensysteme und Körper. | [`src/EDEA/Services/JournalProvider.cs`](../src/EDEA/Services/JournalProvider.cs) |
| `JournalStore` | Liest die aktuelle Journal-Datei aus und feuert `JournalUpdated` mit neuen Zeilen. | [`src/EDEA/Services/JournalStore.cs`](../src/EDEA/Services/JournalStore.cs) |
| `PlanetsOfInterestProvider` | Prüft Planeten gegen benutzerdefinierte Klassifikationsfilter (POI). | [`src/EDEA/Services/PlanetsOfInterestProvider.cs`](../src/EDEA/Services/PlanetsOfInterestProvider.cs) |
| `RouteProvider` | Verwaltet Navigationsroute, Plotter-Routen-Import und Sperrstatus. | [`src/EDEA/Services/RouteProvider.cs`](../src/EDEA/Services/RouteProvider.cs) |
| `SettingsProvider` | Lädt und speichert `UserSettings` als JSON. | [`src/EDEA/Services/SettingsProvider.cs`](../src/EDEA/Services/SettingsProvider.cs) |
| `SpanshService` | Kapselt Spansh-Routen- und Namensauflösung gegenüber `WebApiProvider`. | [`src/EDEA/Services/SpanshService.cs`](../src/EDEA/Services/SpanshService.cs) |
| `SpeechProvider` | Text-to-Speech-Warteschlange mit `SayIt` (Edge TTS) und `NAudio`. | [`src/EDEA/Services/SpeechProvider.cs`](../src/EDEA/Services/SpeechProvider.cs) |
| `StarSystemProvider` | Zentrale Koordination von aktuellem System, Route, Umgebung und UI-Events. | [`src/EDEA/Services/StarSystemProvider.cs`](../src/EDEA/Services/StarSystemProvider.cs) |
| `StatusProvider` | Liest `Status.json` und teilt Activity, Location, GuiFocus und Treibstoff mit. | [`src/EDEA/Services/StatusProvider.cs`](../src/EDEA/Services/StatusProvider.cs) |
| `WebApiLoadingStatusChangedEventHandler` | Delegat für Ladezustandsänderungen des `WebApiProvider`. | [`src/EDEA/Services/WebApiLoadingStatusChangedEventHandler.cs`](../src/EDEA/Services/WebApiLoadingStatusChangedEventHandler.cs) |
| `WebApiProvider` | Koordiniert alle EDSM-, Spansh- und Canonn-Web-API-Aufrufe. | [`src/EDEA/Services/WebApiProvider.cs`](../src/EDEA/Services/WebApiProvider.cs) |

## Datei- und Journal-Services

### `EDFileWatcher`

- **Zweck:** `FileSystemWatcher`-basierte Beobachtung des Elite-Dangerous-Saved-Games-Ordners.
- **Wichtige Eigenschaften:** `JournalFilePath`, `NavRouteFilePath`, `StatusFilePath`, `JournalFileName`, `NavRouteFileName`, `StatusFileName`.
- **Events:** `JournalFileChanged`, `NavRouteFileChanged`, `StatusFileChanged`.
- **Methode:** `Instance()` erzeugt bei Bedarf den Watcher und initialisiert die relevanten Dateipfade.

### `JournalStore`

- **Zweck:** Liest das aktuelle Journal inkrementell aus und stellt die neu hinzugekommenen Zeilen bereit.
- **Eigenschaften:** `GetJournal()` liefert das gesamte Journal als `IEnumerable<string>`.
- **Event:** `JournalUpdated` (Parameter `sequelRead`, `lastJournalAddition`).
- **Intern:** Liest 1.024 Byte-Blöcke, setzt `journalPosition` und puffert mit Retry-Logik bis zu 10 Versuchen.

### `JournalProvider`

- **Zweck:** Parst die vom `JournalStore` gelieferten Zeilen und aktualisiert `StarSystem`/`Body`-Objekte.
- **Methoden:** `processJournalScanEvent(...)`, `Initialize()` startet den ersten Parse.
- **Event:** `ParsedJournalDataUpdated`.

### `JournalHistoryImporter`

- **Zweck:** Importiert historische `Journal*.log`-Dateien per `BackgroundWorker`.
- **Eigenschaften:** `StatusData` (`JournalImportReportData`), `StatusPercentage`.
- **Methode:** `StartJournalImport()`.

## Daten- und Zustands-Services

### `StarSystemProvider`

- **Zweck:** Zentrale Datenhalde und Koordinator. Verwaltet `CurrentSystem`, `DestinationSystem`, `CurrentPlanet`, `StarSystemsOnRoute`, `SurroundingStarSystems`, `CurrentShip` und `CurrentActivity`.
- **Methoden:** `RegisterProvider(object)`, `HandleLoadEdsmSystemDataCommand(bool)`, `HandleApplicationShutdown()`.
- **Events:** `GuiDataUpdated`, `GuiLocationDataUpdated`, `CurrentPlanetChanged`, `RouteLoadingStatusChanged`, `SurroundingsLoadingStatusChanged` u. a.

### `HistoryProvider`

- **Zweck:** Hält eine `ConcurrentDictionary<long, StarSystem>` der bereits besuchten Systeme und synchronisiert mit `SQLiteStore`.
- **Methoden:** `AddOrUpdateStarSystem(...)`, `TryGetStarSystem(...)`, `IsStarSystemExisting(...)`, `GetAllStarSystems()`.
- **Event:** `HistoryUpdated`.

### `RouteProvider`

- **Zweck:** Baut und verwaltet die aktuelle Route für die UI (`RouteView`).
- **Eigenschaften:** `Route` (`List<RouteView>`), `IsCustomRoute`, `IsLocked`.
- **Methoden:** `ReadStarsSystems(...)`, `ImportSpanshRouteFile(...)`, `ImportPlotterRoute(...)`, `LockRoute()`, `UnlockRoute()`, `DeletePlotterRoute()`.
- **Events:** `RouteChanged`, `SystemsOnRouteChanged`.

### `StatusProvider`

- **Zweck:** Parst `Status.json` und meldet Game-Status an den `StarSystemProvider`.
- **Eigenschaft:** `Current` (`Status?`).
- **Events:** `StatusUpdated`, `LocationUpdated`, `GuiFocusUpdated`, `ShipFuelUpdated`.

### `PlanetsOfInterestProvider`

- **Zweck:** Prüft Planeten gegen die in den Einstellungen hinterlegten `PlanetClassification`-Filter.
- **Methoden:** `IsBodyOfInterest(...)`, `MatchingClassificationNames(...)`, `FindAndSetMatchingPlanetClassifications(...)`, `SetPlanetClassifications(...)`, `GetClonedPlanetClassifications()`.

### `SettingsProvider`

- **Zweck:** Persistiert `UserSettings` als JSON.
- **Eigenschaft:** `Settings` (`UserSettings`).
- **Methoden:** `Save()`, `Reload()`.

### `JotSettingsProvider`

- **Zweck:** Hält die zentrale `Jot.Tracker`-Instanz für Fensterzustände.
- **Feld:** `public static Tracker Tracker`.

## Web- und Externe-Services

### `WebApiProvider`

- **Zweck:** Zentrale Anlaufstelle für EDSM, Spansh und Canonn.
- **Wichtige Eigenschaften:** `httpClient`, `isLoading`, `AbsoluteRequestsInSession`, `xRateRemaining`, `maxActiveRequests` (17), `requestWaitDelay` (700 ms).
- **Methoden:**
  - EDSM: `EdsmCheckAndRequestStarSystemInformation(...)`, `EdsmRequestSurroundingStarSystemsInformation(...)`.
  - Spansh: `SpanshRequestBasicSystemData(...)`, `SpanshRequestGalaxyRouteCalculation(...)`, `SpanshRequestNeutronRouteCalculation(...)`.
  - Canonn: `CanonnRequestBioStatsAsync(...)`.
- **Event:** `WebApiLoadingStatusChanged`.
- Siehe [`API_INTEGRATION.md`](./API_INTEGRATION.md).

### `SpanshService`

- **Zweck:** Höherwertige Spansh-API-Operationen für `RoutePlotterViewModel`.
- **Methoden:** `GetRouteByNameAsync(...)`, `ResolveSystemId64Async(...)`, `SearchSystemNamesAsync(...)`.
- **Eigenschaft:** `IsLoading`.

## Statische und Hilfs-Services

### `EdDataProvider`

- **Zweck:** Liest `mc.dat` (Base64-kodiertes JSON) und liefert `ModuleClassifications` (FSD und Guardian Booster).
- **Eigenschaft:** `ModuleClassifications`.

### `GeneraIndexProvider`

- **Zweck:** Liest `gc.json` und liefert `GenusClassification`-Objekte inklusive Schwerkraft-, Temperatur- und Atmosphären-Ranges.
- **Eigenschaft:** Interne `_genusClassifications`-Liste.

### `SpeechProvider`

- **Zweck:** Sprachausgabe-Warteschlange.
- **Methoden:** `Speak(...)`, `ShutUp()`, `GetInstalledVoices()`, `GetVoiceNames()`, `SpeakFirstDiscoverySystem(...)`, `SpeakTerraformable(...)`, `SpeakLandable(...)` u. v. m.
- **Events:** `SpeechSynthesizerStateChanged`, `VoicesLoaded`.

### `HotkeyProvider`

- **Zweck:** Globale Tastenkürzel für HUD, Tabs und Sprachstopp.
- **Eigenschaft:** `Hotkeys` (`Dictionary<string, HotkeyViewModel>`).
- **Methoden:** `Instance(...)`, `AttachHotkeyListener(...)`, `AssignHotKey(...)`.

### `WebApiLoadingStatusChangedEventHandler`

- **Zweck:** Delegatentyp für `WebApiProvider.WebApiLoadingStatusChanged`.
- **Signatur:** `void (object? sender, WebApiParameter? webApiParameter)`.
