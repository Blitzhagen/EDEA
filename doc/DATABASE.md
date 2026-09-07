# EDEA – Datenbank

EDEA speichert die Erkundungshistorie lokal in einer SQLite-Datenbank. Zugriff läuft über `SQLiteStore` (`src/EDEA.Core/Stores/SQLiteStore.cs`) mit `Microsoft.Data.Sqlite` und Dapper.

## Ort

```text
%LOCALAPPDATA%\EDEA.Core\db\EDEA.db
```

Der Ordner wird beim Start angelegt; Schema-Migrationen (`CREATE TABLE IF NOT EXISTS`, Spalten-Nachrüstung) laufen beim Öffnen der Verbindung.

## Tabellen

### `StarSystems`

Besuchte/bekannte Systeme (Schlüssel `Id64`):

- `Id64`, `Name`, `StarClass`, `PrimaryStarName`
- `TotalBodyCount`, `TotalNonBodyCount`
- Herkunfts-Flags: `WasReadFromJournal`, `WasReadFromEdsm`, `WasRequestedFromEdsm`
- EDSM-Spiegel: `EdsmName`, `EdsmPrimaryStarType`, `EdsmPrimaryStarName`, u. a.
- Erweiterte Systemdaten für die Biologie-Vorhersage (Region, Nebel, Koordinaten)

### `Bodies`

Himmelskörper pro System (`Id64`, `StarSystemId`, `Id`, `Name`, `Type`, `Distance`):

- Entdeckungs-/Kartierungs-Status: `WasDiscovered`, `WasMapped`, `WasFootfalled`, `EdsmDiscoveryCommander`
- Herkunfts-Flags: `WasReadFromJournal`, `WasReadFromEdsm`
- Erweiterte Felder für die Biologie-Engine: Atmosphärentyp/-zusammensetzung, Oberflächendruck, Materialien, Umlaufzeit, Eltern-Sterne u. a.

### `Rings`

Ringe eines Körpers, eindeutig je `(StarSystemId, BodyId, Name)`:

- `Type`, `Mass`, `InnerRadius`, `OuterRadius`, `Width`, `Density`

### `Genera`

Gefundene biologische Gattungen/Arten auf Körpern:

- `Name`, `Species`, `Variant`, `ScanCount`, `IsAnalysed`, `VistaGenomicsValue`
- Scan-Positionen: `LongitudeAt1stScan`, `LatitudeAt1stScan`, `LongitudeAt2ndScan`, … (Klonkolonie-Reichweiten-Prüfung)

### `CodexScans`

Bekannte Codex-Einträge, eindeutig je `(Region, Biological)` – Grundlage des `CodexTracker` für regions- und galaxisweite Erstentdeckungen.

### `ImportedJournalFiles`

Import-Index historischer Journal-Dateien (`FileName`, `LastWriteTimeUtc`, `Length`) – unveränderte Dateien werden beim Import übersprungen.

## Hinweise

- Die Datenbank ist reiner Cache/Verlauf – alle Inhalte lassen sich aus Journals und EDSM rekonstruieren.
- Alte Datenbanken unter `%LOCALAPPDATA%\EDEA` (WPF-Vorgänger) werden nicht mehr verwendet.
