# EDEA – Erste Schritte

Diese Anleitung beschreibt Voraussetzungen, Build, Konfiguration und erforderliche Ressourcen für EDEA.

## Voraussetzungen

- **.NET SDK**: .NET 8 SDK (`net8.0`).
- **Betriebssystem**: Windows 10 oder neuer, oder Linux x64 (X11 oder Wayland; Details siehe Abschnitt [Linux](#linux)).
- **Elite Dangerous**: Installiert und gespielt, damit Journal-Dateien existieren.
- **Internet**: Für EDSM- und Spansh-Abfragen sowie für die Sprachausgabe (Microsoft Edge TTS).

EDEA fragt EDSM und Spansh selbstständig ab – ein Parallelbetrieb von EDMarketConnector ist **nicht** erforderlich.

## Repository

```bash
git clone <repository-url>
cd EDEA
```

## Build

Im Repository-Root:

```powershell
dotnet build EDEA.slnx
```

Tests:

```powershell
dotnet test EDEA.slnx --no-build
```

Das Solution-File `EDEA.slnx` enthält:

- `src/EDEA.Core/EDEA.Core.csproj` – plattformunabhängige Kernbibliothek (Modelle, ViewModels, Services, SQLite-Store).
- `src/EDEA.Avalonia/EDEA.Avalonia.csproj` – Avalonia-UI-Anwendung.
- `test/EDEA.Tests/EDEA.Tests.csproj` – Unit-Tests.

## Ausführung

```powershell
dotnet run --project src\EDEA.Avalonia\EDEA.Avalonia.csproj
```

Oder die erzeugte `EDEA.Avalonia.exe` im Ausgabeverzeichnis starten.

### Veröffentlichung ohne .NET-Installation

Self-contained-Build für Windows x64 (Zielrechner braucht kein .NET):

```powershell
dotnet publish src\EDEA.Avalonia\EDEA.Avalonia.csproj -c Release -r win-x64 --self-contained -o publish\win-x64
```

Das Ergebnis liegt in `publish\win-x64` inklusive aller Laufzeit-Ressourcen.

Self-contained-Build für Linux x64 (inkl. `.desktop`-Datei und Icon, als `tar.gz`):

```bash
bash packaging/linux/build-linux.sh
```

Erzeugt `dist/EDEA-<Version>-linux-x64.tar.gz`. Entpacken und starten:

```bash
tar -xzf EDEA-<Version>-linux-x64.tar.gz
cd EDEA-<Version>-linux-x64
./EDEA.Avalonia
```

## Konfiguration

### Journal-Pfad

EDEA benötigt den Pfad zu den Elite-Dangerous-Savegame-Dateien. Standard:

```text
%USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous
```

Unter Linux läuft Elite Dangerous über Steam/Proton – EDEA erkennt den Journal-Ordner
im Proton-Prefix automatisch (u. a. `~/.steam/steam/steamapps/compatdata/359320/pfx/drive_c/users/steamuser/Saved Games/Frontier Developments/Elite Dangerous`).

Falls der Pfad abweicht:

- EDEA starten.
- `Einstellungen` öffnen.
- Im Bereich **Konfiguration** den korrekten Ordner für `EdSavedGamePath` setzen.

### Einstellungen

Einstellungen werden in `%LOCALAPPDATA%\EDEA\settings.json` (Windows) bzw. `~/.local/share/EDEA/settings.json` (Linux) gespeichert. Wichtige Bereiche:

- **Application**: Sprache (`Language`), ausgewählter Tab (`SelectedTabIndex`).
- **Other**: Journal-Pfad, automatischer Tab-Wechsel, Schwellenwerte für wertvolle Körper und Arten.
- **HudWindow**: Sichtbarkeit der HUD-Spalten, Deckkraft (`Opacity`), Ausblendverhalten.
- **Speech**: Aktivierung und Textvorlagen für die Sprachausgabe. Gespeicherte Standardtexte folgen der UI-Sprache; eigene Anpassungen bleiben erhalten.
- **Hotkeys**: Benutzerdefinierte Tastenkombinationen.
- **PlanetsOfInterest**: Filterkriterien für interessante Planeten.
- **Spansh**: Einstellungen für Spansh-Routen.

Unterstützte Sprachen: `Auto` (Systemsprache), `en`, `de`, `ru`. Die Sprache lässt sich in den Einstellungen **live umschalten** – ohne Neustart. Die Infrastruktur ist für `es`, `fr` und `pt-BR` vorbereitet (diese fallen ohne Sprachdatei auf Englisch zurück).

### Erste Nutzung

1. EDEA starten.
2. Über `Einstellungen` den Journal-Pfad prüfen.
3. Elite Dangerous starten oder ein Journal laden.
4. Den gewünschten Tab auswählen (Route, Himmelskörper, Biologie, Umgebung, Historie).

## Ressourcen

Die folgenden Dateien werden als `Content` mit `CopyToOutputDirectory=PreserveNewest` in die Ausgabe kopiert und müssen neben der Anwendung liegen:

- `src/EDEA.Avalonia/Resources/mc.dat` – statische Spieldaten (Module, FSD, Guardian-FSD-Booster).
- `src/EDEA.Avalonia/Resources/bio_catalog.json` – Biologie-Katalog (Artenregeln, Vista-Genomics-Werte).
- `src/EDEA.Avalonia/Resources/regions.json` – Galaktische Regionen inkl. Guardian-/Tuber-Zonen.
- `src/EDEA.Avalonia/Resources/nebulae.json` – Nebel-Volumen.
- `src/EDEA.Avalonia/Assets/app.ico` – Anwendungssymbol (eingebettet als Avalonia-Ressource).

## Linux

EDEA läuft unter Linux über den Avalonia-X11-Backend – auf Wayland-Desktops automatisch
als XWayland-Client (keine weitere Konfiguration nötig).

### Voraussetzungen unter Linux

- **Desktop**: X11-Session oder Wayland-Session mit XWayland (auf allen gängigen
  Distributionen vorhanden). Steam Deck: Desktop-Modus.
- **Elite Dangerous** via Steam/Proton, im Fenster- oder randlosen Modus
  („Borderless") – exklusives Vollbild ist Compositor-abhängig und wird nicht garantiert.
- **Audio**: PortAudio (`libportaudio.so` ist im Paket enthalten; ALSA/PulseAudio/
  PipeWire werden über PortAudio angesprochen).

### Globale Hotkeys

Hotkeys werden über zwei Mechanismen parallel bereitgestellt:

- **X11-Grabs** (`XGrabKey`): Feuern, solange ein X11/XWayland-Fenster den Fokus hat.
  Da Elite Dangerous über Proton selbst als XWayland-Client läuft, funktionieren die
  Hotkeys im Spiel-Fokus – auch unter Wayland.
- **XDG-Portal** (`org.freedesktop.portal.GlobalShortcuts`): Desktop-Integration für
  Wayland-native Fokus-Fälle; Verfügbarkeit hängt vom Compositor ab (KDE Plasma ≥ 6.1
  unterstützt es, GNOME derzeit kaum). Feuern beide Mechanismen, werden Doppel-Events
  automatisch unterdrückt.

Falls Hotkeys unter Wayland bei fokussiertem Nicht-X11-Fenster nicht greifen, liegt das
am fehlenden Portal-Support des Compositors – nicht an EDEA. Solange ED fokussiert ist,
greift der X11-Grab immer.

### HUD-Click-Through

Der Maus-Durchgriff nutzt die XShape-Input-Region (leere Region = alle Klicks fallen
durchs HUD auf das Fenster darunter). Unter Wayland übersetzt XWayland das zu
`wl_surface.set_input_region` – funktioniert auf allen gängigen Compositoren.

### Bekannte Einschränkungen

- **Steam Deck / Game Mode**: Gamescope behandelt Overlays eigen; das HUD kann im
  Game Mode von anderen Fenstern verdeckt werden. Im Desktop-Modus verhält sich alles
  wie auf einer normalen Distribution.
- **Exklusives Vollbild**: Ob Overlays über exklusivem Vollbild sichtbar bleiben,
  entscheidet der Compositor. Empfehlung: ED im randlosen Fenstermodus betreiben.
- **Natives Wayland-Backend**: EDEA nutzt bewusst X11/XWayland – globale Hotkeys und
  Click-Through hätten unter nativem Wayland keine portable Umsetzung.

## Dateien im Anwendungsdatenverzeichnis

Nach dem ersten Start legt EDEA unter `%LOCALAPPDATA%\EDEA` (Windows) bzw. `~/.local/share/EDEA` (Linux) folgende Dateien an:

- `settings.json` – Benutzereinstellungen.
- `windowstate.json` – Fensterpositionen und -zustände.
- `db/EDEA.db` – SQLite-Datenbank (Historie, Codex-Scans, Journal-Import-Index).
- `log/` – Anwendungslogs (log4net, rotierend).

## Fehlerbehebung

- **„Missing Journal Files!"**: Journal-Pfad in den Einstellungen korrigieren.
- **Keine Web-Daten**: Internetverbindung prüfen; EDSM/Spansh können auch kurzfristig nicht erreichbar sein.
- **Keine Sprachausgabe**: Edge TTS benötigt Internet; ohne Verbindung stehen keine Stimmen zur Verfügung.
