# EDEA – Helper, Enums und Stores

Übersicht über Hilfsklassen, Aufzählungstypen und Datenspeicher.

## Helper

### `Helpsters` (`src/EDEA.Core/Helpers/Helpsters.cs`)

Statische Hilfsmethoden, u. a.:

- `ConvertJObjectValue<T>` – typsichere Extraktion von Werten aus `JObject`/`JsonNode` beim Journal-Parsing.
- Diverse Format-/Konvertierungs-Helfer für Anzeige und Datenverarbeitung.

### UI-Behaviors (`src/EDEA.Avalonia/Helpers/`)

- `DataGridSingleSortBehavior` – erzwingt Einzelspalten-Sortierung im DataGrid.
- `ToolTipDataContextBehavior` – DataContext-Weitergabe in Tooltips.

### Controls (`src/EDEA.Avalonia/Controls/`)

- `BadgePanel` – Icon+Zähler-Badge für Grid-Zellen.

## Enums (`src/EDEA.Core/Enums/`)

| Enum | Zweck |
|------|-------|
| `HotkeyId` | Belegbare globale Hotkey-Aktionen. |
| `SpanshRoutingAlgorithm` | Algorithmen für Spansh-Routen. |
| `StarSystemExplorationStatus` | Unentdeckt / teilweise / vollständig erforscht. |
| `SurroundingsRadius` | Radien für die Umgebungsabfrage. |
| `UserSelectableInputStringListsKey` | Schlüssel für benutzerdefinierte Auswahllisten. |
| `WepApiQueryType` | Art der Web-API-Abfrage (EDSM/Spansh/…). |

Weitere Aufzählungstypen in `src/EDEA.Core/`: `BodyType`, `RingType`, `RingReserveLevel` (Models) und `SurfaceScanStatus` (Wurzelverzeichnis).

## Stores

### `SQLiteStore` (`src/EDEA.Core/Stores/SQLiteStore.cs`)

- Singleton, Datenbank unter `%LOCALAPPDATA%\EDEA.Core\db\EDEA.db`.
- Tabellen siehe [`DATABASE.md`](./DATABASE.md).
- Dapper-basierte CRUD-Methoden für Systeme, Körper, Ringe, Genera, Codex-Scans und Import-Index.

### `JournalStore` (`src/EDEA.Core/Services/JournalStore.cs`)

- Singleton, liest die aktuelle Journal-Datei inkrementell (Positionsbasiertes Nachlesen, Retry-Logik).
- Feuert `JournalUpdated` für neue Zeilen.

## Statische Registrierung

### `PlatformServices` (`src/EDEA.Core/Services/PlatformServices.cs`)

Statische Registry für plattformspezifische Dienste (`ISpeechService`, `IGlobalHotkeyService`, `IWindowStateService`, `IHudWindowService`, `IDialogService`, `IClipboardService`, `IScreenService`, `IColorThemeService`, `IDispatcher`, `IUiTimerService`, `IPlatformService`). Wird beim Start in `App.axaml.cs` (`RegisterPlatformServices`) mit den Avalonia-Implementierungen befüllt.

### `Globals` (`src/EDEA.Core/Globals.cs`)

- `ApplicationFolder` – Verzeichnis der laufenden Assembly (Ressourcen-Lookup).
- `AppDataFolder` – `%LOCALAPPDATA%\<AssemblyName>` → `EDEA.Core`.
- `AppVersionString`, diverse `ColumnHeader*`-Zugriffe und `EdsmToJournalPlanetClasses`-Mapping.

### `Preferences` (`src/EDEA.Core/Preferences.cs`)

Statische Fassade auf `UserSettings` (`User`, `Colors`, `Other`, `Application`, `Hotkeys`, `HudWindow`, `PlanetsOfInterest`, `Speech`, `Spansh`) mit Zugriff auf den `SettingsProvider` (`SaveUserSettings`, `ReloadUserSettings`).
