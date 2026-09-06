# EDEA – Erste Schritte

Diese Anleitung beschreibt Voraussetzungen, Build, Konfiguration und erforderliche Ressourcen für EDEA.

## Voraussetzungen

- **Betriebssystem**: Windows 10 Version 19041 (20H1) oder neuer.
- **.NET SDK**: .NET 8 SDK (`net8.0-windows10.0.19041.0`).
- **Elite Dangerous**: Installiert und gespielt, damit Journal-Dateien existieren.
- **EDMarketConnector (empfohlen)**: Ermöglicht EDSM-Datenanreicherung und flüssigere Journal-Verarbeitung.
- **Internet**: Optional, für EDSM- und Spansh-Abfragen.

## Repository

```bash
git clone <repository-url>
cd EDEA
```

## Build

Im Repository-Root:

```bash
dotnet build EDEA.slnx
```

Alternativ für Release:

```bash
dotnet build EDEA.slnx -c Release
```

Das Solution-File `EDEA.slnx` enthält:

- `src/EDEA/EDEA.csproj` (Hauptanwendung)
- `test/EDEA.Tests/EDEA.Tests.csproj` (Unit-Tests)

## Ausführung

Nach erfolgreichem Build:

```bash
dotnet run --project src/EDEA/EDEA.csproj
```

Oder die erzeugte `.exe` im Ausgabeverzeichnis starten.

## Konfiguration

### Journal-Pfad

EDEA benötigt den Pfad zu den Elite-Dangerous-Savegame-Dateien. Standard:

```text
%USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous
```

Falls der Pfad abweicht:

- EDEA starten.
- `Preferences` öffnen.
- Im Bereich **Configuration** den korrekten Ordner für `EdSavedGamePath` setzen.
- EDEA neu starten.

### EDMC

*Elite Dangerous Market Connector* sollte parallel laufen, damit Journal-Events und EDSM-Daten optimal verfügbar sind. EDEA prüft beim Start, ob EDMC läuft, und gibt ggf. eine Warnung aus.

### Einstellungen

Einstellungen werden in `%LOCALAPPDATA%\EDEA\settings.json` gespeichert. Wichtige Bereiche:

- **Application**: Sprache (`Language`), ausgewählter Tab (`SelectedTabIndex`), Fensterzustände.
- **Other**: Journal-Pfad, automatischer Tab-Wechsel, wertvolle Body-/Genus-Schwellen, biologische Sichtbarkeit.
- **HudWindow**: Sichtbarkeit und Anordnung der HUD-Spalten, Deckkraft (`Opacity`), Ausblendverhalten.
- **Speech**: Aktivierung und Textvorlagen für Sprachausgabe.
- **Hotkeys**: Benutzerdefinierte Tastenkombinationen.
- **PlanetsOfInterest**: Filterkriterien für interessante Planeten.
- **Spansh**: Einstellungen für Spansh-Routen.

Unterstützte Sprachen: `Auto`, `en`, `de`, `es`, `fr`, `ru`, `pt-BR`.

### Erste Nutzung

1. EDEA starten.
2. Im Willkommensdialog oder über `Preferences` den Journal-Pfad prüfen.
3. EDMC starten (empfohlen).
4. Elite Dangerous starten oder ein Journal laden.
5. Den gewünschten Tab auswählen (Bodies, Route, Surroundings, History, Biologicals).

## Ressourcen

Das Projekt enthält folgende Ressourcen, die in die Ausgabe kopiert werden:

- `src/EDEA/Resources/app.ico` – Anwendungssymbol.
- `src/EDEA/Resources/mc.dat` – Inhalt in `EDEA.csproj` als `Content` mit `CopyToOutputDirectory=PreserveNewest`.
- `src/EDEA/Resources/gc.json` – Inhalt in `EDEA.csproj` als `Content` mit `CopyToOutputDirectory=PreserveNewest`.

Diese Dateien müssen im Build-Output im Anwendungsverzeichnis vorhanden sein.

## Dateien im Anwendungsdatenverzeichnis

Nach dem ersten Start legt EDEA unter `%LOCALAPPDATA%\EDEA` folgende Dateien an:

- `settings.json` – Benutzereinstellungen.
- `db/EDEA.db` – SQLite-Datenbank.
- `log/EDEA.log` – Anwendungslog.
- `jot/` – Jot-Fensterzustände.

## Fehlerbehebung

- **"Missing Journal Files!"**: Journal-Pfad in den Einstellungen korrigieren.
- **EDEA startet nicht**: Prüfen, ob bereits eine EDEA-Instanz läuft (Singleton-Mutex).
- **Keine Web-Daten**: Internetverbindung und EDMC-Status prüfen.
