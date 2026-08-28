# EDEA – Technische Dokumentationsplanung

> Projekt: **Elite Dangerous Exploration Assistent (EDEA)**  
> Erstellt am: 28.08.2026  
> Autor: Blitzhagen  
> Phasen-Status: **Phase 1 abgeschlossen – Freigabe erforderlich**

---

## 1. Projektzusammenfassung

EDEA ist ein Open-Source-WPF-Desktop-Companion für **Elite Dangerous**. Ziel ist eine funktionsreiche, wartbare und datenschutzfreundliche Begleitanwendung für Erkundungs- und Routen-Aktivitäten im Spiel.

### 1.1 Technologiestack

| Bereich | Technologie |
|---------|-------------|
| Framework | .NET 8 (`net8.0-windows10.0.19041.0`) |
| UI-Framework | WPF (`UseWPF=true`) |
| MVVM-Toolkit | CommunityToolkit.Mvvm 8.4.2 |
| Datenzugriff | Dapper 2.1.79 + Microsoft.Data.Sqlite 8.0.8 |
| Einstellungen/Persistenz | Jot 2.1.17 |
| Logging | log4net 3.3.1 |
| Audio | NAudio 2.2.1 |
| Icons | Material.Icons.WPF 3.0.2 |
| Web-Rendering | Microsoft.Web.WebView2 1.0.4129.50 |
| Verhalten/Interaktion | Microsoft.Xaml.Behaviors.Wpf 1.1.122 |
| Farbwahl | PixiEditor.ColorPicker 3.4.2.3 |
| Sprachausgabe | SayIt 1.0.6 |

### 1.2 Projektlösung

```text
EDEA.slnx
├── src/EDEA/EDEA.csproj        (Hauptanwendung)
└── test/EDEA.Tests/EDEA.Tests.csproj  (Unit-Tests)
```

### 1.3 Globale Build-Eigenschaften

- `LangVersion`: 12.0
- `Nullable`: enable
- `ImplicitUsings`: enable
- `TreatWarningsAsErrors`: false
- `AnalysisLevel`: latest

---

## 2. Architekturüberblick

EDEA folgt einem klassischen **MVVM-Muster** mit zusätzlichen Singleton-Services, die Spiel-Daten verarbeiten, zuständige externe APIs ansprechen und die UI-ViewModels mit Echtzeitinformationen versorgen.

### 2.1 Schichten

```text
Präsentation (XAML / Views / Windows)
      |
ViewModels (CommunityToolkit.Mvvm)
      |
Commands / Converters / Helpers
      |
Services (Singleton-Provider)
      |
Stores (SQLite/Dapper)  +  Datei-Beobachter (Journal)
      |
Externe Datenquellen (EDSM, Spansh, Elite-Journal)
```

### 2.2 Zentrale Komponenten

- `App` (`App.xaml.cs`) – Singleton-Mutex, Log4Net-Konfiguration, Jot-Tracking, Startup-Initialisierung aller Services.
- `MainWindow` – Hauptfenster mit Tab-basierter Navigation.
- `MainViewModel` – zentrales ViewModel, koordiniert Sub-ViewModels und window state.
- `SQLiteStore` – zentrale Datenbankzugriffsinstanz.
- `EDFileWatcher` – überwacht das Elite-Dangerous-Journalverzeichnis.
- Provider-Services: `StarSystemProvider`, `JournalProvider`, `RouteProvider`, `StatusProvider`, `HistoryProvider`, `PlanetsOfInterestProvider`, `WebApiProvider`, `SpeechProvider`, `HotkeyProvider`.

### 2.3 Geplante Mermaid-Diagramme

Die folgenden Diagramme sollen in Phase 3 im Ordner `doc/` erstellt werden:

1. **Systemarchitektur** – Übersicht der MVVM-Schichten und Service-Interaktionen.
2. **Datenflussdiagramm** – Journal-Events → `EDFileWatcher` → `JournalStore` → Provider → ViewModels → UI.
3. **API-Integrationsdiagramm** – `WebApiProvider` mit EDSM, Spansh und lokalen Speicher.
4. **Routenplanungsdiagramm** – Benutzer-Eingaben → `SpanshService` / `RouteProvider` → UI.

---

## 3. Geplante Dokumentationsstruktur

Alle Dokumentationsdateien werden unter `doc/` abgelegt. Die bestehende `README.md` im Repository-Root wird in Phase 3 um einen Verweis auf `doc/` ergänzt, aber ansonsten nicht verändert.

### 3.1 Übersicht der geplanten Dateien

| Datei | Inhalt |
|-------|--------|
| `README.md` (Root) | Kurzer Hinweis auf `doc/`-Ordner (Phase 3) |
| `doc/ARCHITECTURE.md` | Architektur- & MVVM-Beschreibung inkl. Mermaid-Diagrammen |
| `doc/GETTING_STARTED.md` | Build, Installation, Konfiguration |
| `doc/APP_LIFECYCLE.md` | `App.xaml.cs`, Startup, Logging, Jot, Exception-Handling |
| `doc/MODELS.md` | Alle Datenmodelle (`Models/`) |
| `doc/SERVICES.md` | Alle Service-Provider (`Services/`) |
| `doc/VIEWMODELS.md` | Alle ViewModels (`ViewModels/`) |
| `doc/VIEWS_WINDOWS.md` | XAML-Views (`Views/`) und WPF-Windows (`Windows/`) |
| `doc/COMMANDS.md` | Befehlsklassen (`Commands/`) |
| `doc/CONVERTERS.md` | Wertkonverter und Template-Selektoren (`Converters/`) |
| `doc/HELPERS_ENUMS_STORES.md` | Hilfsklassen, Enums und SQLite-Store |
| `doc/API_INTEGRATION.md` | EDSM, Spansh, WebApiProvider, externe Datenflüsse |
| `doc/DATABASE.md` | SQLite-Schema, Dapper-Nutzung, Migration/Konventionen |
| `doc/TESTING.md` | Testprojekt und Testabdeckung |
| `doc/DOCS_PLAN.md` | Diese Planungsdatei (Wird in Phase 4 validiert) |

---

## 4. Detailliertes Inhaltsverzeichnis pro geplanter Dokument

### 4.1 ARCHITECTURE.md

1. Ziele und Prinzipien
2. MVVM-Überblick
3. Service-Orientierung und Singleton-Entwurf
4. Datenfluss (Mermaid)
5. Schichtdiagramm (Mermaid)
6. Sicherheits-/Datenschutzaspekte
7. Erweiterbarkeit

### 4.2 GETTING_STARTED.md

1. Systemvoraussetzungen
2. .NET 8 SDK
3. Klonen & Build (`dotnet build EDEA.slnx`)
4. Konfiguration (Journal-Pfad, Sprache, HUD)
5. EDMC-Integration
6. Laufzeit-Abhängigkeiten (`mc.dat`, `gc.dat`, `app.ico`)

### 4.3 APP_LIFECYCLE.md

1. `App`-Klasse (`App.xaml.cs`)
   - Singleton-Mutex
   - `ConfigureLog4Net`
   - `OnStartup` / `RunStartup`
   - Service-Initialisierungssequenz
   - Exception-Handler
   - Sprachanwendung
2. `MainWindow` (`MainWindow.xaml`, `MainWindow.xaml.cs`)
3. `Globals` und `AssemblyInfo`
4. `BindingProxy`
5. `DisplaySize`

### 4.4 MODELS.md

Dokumentation aller 67 Klassen im Ordner `Models/`, gruppiert nach fachlichen Bereichen:

#### 4.4.1 Kerndaten
- `StarSystem`
- `Star`
- `Body`
- `Planet`
- `Ring`
- `Ship`
- `Genus`
- `Status`

#### 4.4.2 Klassifikation & Typen
- `BodyType`
- `GenusClassification`
- `GravityRange`
- `PlanetClassification`
- `HyperdriveClassification`
- `GuardianFsdBoosterClassification`
- `ModuleClassification`
- `DistanceRange`
- `TemperatureRange`
- `RingType`
- `RingReserveLevel`

#### 4.4.3 Journal & Historie
- `EdFileEvent`
- `EdGuiFocus`
- `EdStatusFlags`
- `EdStatusFlags2`
- `HistoryData`
- `HistoryStatistics`
- `JournalImportReportData`
- `JournalLine`
- `JournalPlanetMemory`
- `JournalPlanetMemoryItem`
- `JournalSystemMemory`
- `JournalSystemMemoryItem`
- `LocationOnPlanet`

#### 4.4.4 Bio-Statistik (Canonn)
- `CanonnBioStatsHistograms`
- `CanonnBioStatsMinMaxData`
- `CanonnBioStatsObject`

#### 4.4.5 Benutzereinstellungen
- `UserSettings`
- `UserSettingsApplication`
- `UserSettingsColors`
- `UserSettingsHotkeys`
- `UserSettingsHudWindow`
- `UserSettingsPlanetsOfInterest`
- `ColorItem`
- `SpanshSettings`
- `DataSource`

#### 4.4.6 Hotkeys
- `Hotkey`
- `HotkeyItem`

#### 4.4.7 Sprachausgabe
- `SpeechOutput`
- `SpeechOutputBody`
- `SpeechOutputCartographicValues`
- `SpeechOutputCommander`
- `SpeechOutputItem`
- `SpeechOutputMatchingClassificationsCount`
- `SpeechOutputPlaceholderKeys`
- `SpeechOutputPlanet`
- `SpeechOutputPlanetClassification`
- `SpeechOutputRing`
- `SpeechOutputRingsCount`
- `SpeechOutputSpecies`
- `SpeechOutputSystem`
- `SpeechOutputValuableSpeciesCount`

#### 4.4.8 Web-API-Parameter
- `WebApiParameter`
- `WebApiParameterEdsmStarystem`
- `WebApiParameterEdsmSurroundings`
- `WebApiParameterSpanshBasicSystemData`
- `WebApiParameterSpanshGalaxyRoute`
- `WebApiRequest`
- `WepApiQueryData`

### 4.5 SERVICES.md

Dokumentation aller 18 Dateien im Ordner `Services/`:

1. `EDFileWatcher` – Journal-Verzeichnisüberwachung
2. `EdDataProvider` – gemeinsame Elite-Dangerous-Datenlogik
3. `GeneraIndexProvider` – Genus-Index-Verwaltung
4. `HistoryProvider` – Spielhistorie
5. `HotkeyProvider` – Hotkey-Registrierung
6. `JotSettingsProvider` – Fenster-/Einstellungs-Tracking
7. `JournalHistoryImporter` – Import historischer Journal-Dateien
8. `JournalProvider` – Verarbeitung aktueller Journal-Events
9. `JournalStore` – Speicherung der Journal-Zeilen
10. `PlanetsOfInterestProvider` – Filter/POI-Verwaltung
11. `RouteProvider` – Routenlogik und -zustand
12. `SettingsProvider` – Anwendungseinstellungen
13. `SpanshService` – Schnittstelle zu Spansh.io
14. `SpeechProvider` – Text-to-Speech-Integration
15. `StarSystemProvider` – aktuelles Sternensystem
16. `StatusProvider` – Elite-Status-Flags
17. `WebApiLoadingStatusChangedEventHandler` – API-Lade-Events
18. `WebApiProvider` – zentrale Web-API-Koordination

### 4.6 VIEWMODELS.md

Dokumentation aller 26 ViewModels:

- `MainViewModel`
- `HudViewModel`
- `AboutViewModel`
- `FeedbackReportIssueViewModel`
- `BodyTableViewModel` / `BodyViewModel`
- `GenusTableViewModel` / `GenusViewModel` / `GenusClassificationViewModel` / `GenusView`
- `HistoryViewModel` / `HistoryDataViewModel`
- `HotkeyViewModel`
- `JournalHistoryImportViewModel`
- `NavRouteTableViewModel`
- `PlanetClassificationViewModel`
- `PreferencesViewModel`
- `RingView` / `RingViewModel`
- `RoutePlotterViewModel`
- `RouteView`
- `StarSystemViewModel`
- `SurroundingsTableViewModel`
- `SurroundingView`
- `TabViewModel`
- `ViewModelBase`

(Weitere `HudWindowTabViewModel` im Projektroot wird in APP_LIFECYCLE behandelt.)

### 4.7 VIEWS_WINDOWS.md

Dokumentation der XAML-Views und WPF-Windows:

#### Views (`Views/`)
- `BodyHudTableView`
- `BodyTableView`
- `GenusHudTableView`
- `GenusTableView`
- `HistoryTableView`
- `NavRouteHudTableView`
- `RouteTableView`
- `SurroundingsTableView`

#### Windows (`Windows/`)
- `AboutWindow`
- `FeedbackReportIssueWindow`
- `HudWindow`
- `InputStringDialogWindow`
- `InputStringListDialogWindow`
- `JournalHistoryImportWindow`
- `PreferencesWindow`
- `RoutePlotterWindow`

#### Hauptfenster
- `MainWindow`

#### Ressourcen (`Resources/`)
- `Icons.xaml`
- `Styles.xaml`
- `Templates.xaml`

### 4.8 COMMANDS.md

Dokumentation aller 28 Befehle:

- `CommandBase`
- `AddCustomPlanetFilterCommand`
- `AssignHotkeyCommand`
- `CancelPreferencesCommand`
- `ClearHistoryCommand`
- `CloseWindowCommand`
- `CopyToClipboardCommand` / `CopyToClipboardCommandParameter`
- `EnableDisableHudWindowMousePassThroughCommand`
- `GenerateClearPlotterRouteCommand`
- `ImportJournalHistoryCommand`
- `ImportSpanshRouteCommand`
- `LoadEdsmSystemDataCommand`
- `LockUnlockRouteCommand`
- `MailToFeedbackReportIssueMailAddressCommand`
- `OpenCloseHudWindowCommand`
- `OpenLogfileFolderCommand`
- `PlaySpeechCommand`
- `RemoveCustomPlanetFilterCommand`
- `RenameCustomPlanetFilterCommand`
- `RestoreDefaultPreferencesCommand`
- `SaveAndClosePreferencesCommand`
- `SaveSettingsCommand`
- `SelectStringsFromStringListCommand`
- `SetJournalFolderCommand`
- `ShowAboutWindowCommand`
- `ShowFeedbackReportIssueWindowCommand`
- `ShowPreferencesCommand`

### 4.9 CONVERTERS.md

Dokumentation der 12 WPF-Wertkonverter:

- `BooleanToBrushConverter`
- `BooleanToSymbolConverter`
- `CommanderNameToBrushColorConverter`
- `ElementsToCopyToClipboardCommandParameterConverter`
- `EnumToBoolConverter`
- `IntEqualsConverter`
- `InverseBooleanConverter`
- `NullToBoolConverter`
- `NullableDoubleToStringConverter`
- `NullableLongToStringConverter`
- `SystemExplorationStatusToBrushColorConverter`
- `TabTemplateSelector`

### 4.10 HELPERS_ENUMS_STORES.md

- `Helpers/ColorThemeHelper`
- `Helpers/Helpsters`
- `Stores/SQLiteStore`
- `Properties/Resources`

#### Enums (`Enums/`)
- `HotkeyId`
- `SpanshRoutingAlgorithm`
- `StarSystemExplorationStatus`
- `SurroundingsRadius`
- `UserSelectableInputStringListsKey`
- `WepApiQueryType`

### 4.11 API_INTEGRATION.md

1. Unterstützte externe Datenquellen
2. `WebApiProvider` – Architektur und HTTP-Client-Handhabung
3. EDSM-Integration (`LoadEdsmSystemDataCommand`, `WebApiParameterEdsmStarystem`, `WebApiParameterEdsmSurroundings`)
4. Spansh-Integration (`SpanshService`, `WebApiParameterSpanshBasicSystemData`, `WebApiParameterSpanshGalaxyRoute`)
5. Routing-Algorithmen (`SpanshRoutingAlgorithm`)
6. Fehlerbehandlung & Retry-Strategien
7. User-Agent & Rate-Limiting

### 4.12 DATABASE.md

1. SQLite-Schema-Übersicht
2. Dapper-Repository-Muster
3. `SQLiteStore` – CRUD-Operationen
4. Journal-Speicherung (`JournalStore`)
5. Historie & POI-Tabellen
6. Migration & Initialisierungskonventionen

### 4.13 TESTING.md

1. Testprojekt `test/EDEA.Tests`
2. `UnitTest1` – aktuelle Testabdeckung
3. Empfohlene Testkategorien
4. Build & Ausführung der Tests

---

## 5. Phasen-Mapping

| Phase | Liefergegenstand | Verantwortliche Dateien / Bereiche |
|-------|------------------|------------------------------------|
| **1** | Git-Setup + `doc/DOCS_PLAN.md` | Fertig (wartet auf Freigabe) |
| **2** | Inline-Dokumentation (Docstrings) | Alle `.cs`-Dateien in `src/EDEA/` und `test/EDEA.Tests/`, z.B. `MainViewModel.cs`, `StarSystemProvider.cs` |
| **3** | High-Level-Dokumentation | `doc/*.md` inkl. Mermaid-Diagramme, README-Ergänzung |
| **4** | Review & Validierung | Linkcheck, `doc/CHECKLIST.md` |

---

## 6. Akzeptanzkriterien für die finale Dokumentation

- [ ] Alle öffentlichen Klassen, Methoden, Properties und Enums in `src/EDEA` sind mit XML-Docstrings/Inline-Docstrings versehen (Phase 2).
- [ ] Jeder `doc/*.md`-Artikel aus Kapitel 3 ist vollständig erstellt (Phase 3).
- [ ] Mermaid-Diagramme sind in mindestens `ARCHITECTURE.md` und `API_INTEGRATION.md` eingebettet (Phase 3).
- [ ] `README.md` enthält einen gut sichtbaren Verweis auf `doc/` (Phase 3).
- [ ] Alle internen Markdown-Links zwischen `README.md` und `doc/` funktionieren (Phase 4).
- [ ] Eine finale `doc/CHECKLIST.md` bestätigt, dass alle Module dokumentiert wurden (Phase 4).
- [ ] Keine offenen `TODO`- oder `FIXME`-Markierungen in der Dokumentation.

---

## 7. Nächste Schritte

> **Phase 2 wartet auf Freigabe:**  
> Nach Genehmigung dieses Plans werden systematisch alle `.cs`-Quelldateien mit standardisierten, lückenlosen Docstrings (Parameter, Typen, Rückgabewerte, Exceptions) ergänzt. Es werden zwei Beispieldateien zur Vorab-Freigabe gezeigt.
