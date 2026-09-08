# ED Exploration Assistant (EDEA)

EDEA ist ein Open-Source-Begleitprogramm (Companion App) für **Elite Dangerous**, das sich auf die **Erkundung der Galaxie** spezialisiert. Das Programm liest das Journal des Spiels live mit und wertet daraus Systeme, Himmelskörper, biologische Signale, Routen und Erkundungsdaten aus.

## Ursprung und Dank

Dieses Projekt basiert auf dem Quellcode des ursprünglichen **Elite Dangerous Exploration Buddy** von **CMDR Panostrede**. Der ursprüngliche Code wurde durch **Dekompilierung des Original-Programms mit `ilspycmd` (ILSpy)** gewonnen und als Grundlage für diese Weiterentwicklung verwendet.

Dabei sei ausdrücklich auch die Arbeit gewürdigt, die darin steckte: die ursprüngliche Entwicklung des Programms durch CMDR Panostrede ebenso wie die mühsame Rekonstruktion aus dem Dekompilat — ein decompilierter Zwischenstand ist kein fertiges Projekt; Namen, Struktur und Zusammenhänge mussten Stück für Stück wiederhergestellt werden, bevor daraus ein weiterentwickelbarer Quellcode werden konnte.

Ein besonderer Dank geht an den ursprünglichen Entwickler **CMDR Panostrede**, der uns freundlicherweise erlaubt hat, den Code zu nutzen und das Programm in dieser Form bereitzustellen.

## Was macht EDEA?

EDEA hilft Explorern dabei, **keine wertvollen Entdeckungen zu übersehen** und den Ertrag einer Erkundungstour zu maximieren. Dazu wertet das Programm das Spiel-Journal in Echtzeit aus und beantwortet die Fragen, die im Spiel sonst umständlich nachgeschlagen werden müssten:

### Beim Sprung in ein neues System

- **Sofortiger Überblick**: Anzahl der Himmelskörper, Entdeckungsstatus des Systems (unentdeckt, teilweise, vollständig) und Fortschritt in Prozent.
- **FSS-Signale**: Anzahl geologischer und biologischer Signale pro Körper – sofort erkennbar, ob sich ein Detail-Scan lohnt.
- **Automatische EDSM-Abfrage**: Bekannte Systeme werden mit EDSM-Daten angereichert (Körper, Entdecker, Werte); bereits gecachte Systeme werden lokal aus der Datenbank geladen.
- **Route**: Die NavRoute wird mit Sprungnummern, Sternklassen, tankbaren Sternen und Systeminformationen dargestellt. Der Name des nächsten Systems kann per Hotkey in die Zwischenablage kopiert werden.

### Beim Scannen von Himmelskörpern

- **Welche Körper lohnen sich?** Markierung von terraformbaren, landbaren und besonders wertvollen Welten (konfigurierbarer Schwellenwert), Ringen und „Planets of Interest“.
- **Status pro Körper**: entdeckt, Erstentdeckung, kartiert (DSS), gelandet – inklusive Angabe, wer der Entdecker war.
- **Geschätzte kartografische Werte**: Basis-, Oberflächenscan- und Bonus-Werte für jeden Körper und das ganze System.

### Bei der Exobiologie

- **Artenvorhersage**: Anhand der biologischen Signale, des Körpertyps, der Atmosphäre und der Region schlägt EDEA die wahrscheinlichsten Arten und Varianten vor – auf Basis statistischer Daten von **Canonn Research**.
- **Scan-Fortschritt**: Anzahl der bereits gesammelten Proben pro Genus und welche Analyse noch fehlt.
- **Klonkolonie-Abstand**: Anzeige und Ansage, ob die Mindest- bzw. Maximaldistanz zwischen zwei Proben eingehalten wird – damit kein Scan ungültig wird.
- **Vista-Genomics-Werte**: Geschätzter Verkaufswert jeder Art, inklusive Erstentdeckungs-Bonus.

### HUD-Overlay

- Ein transparentes, immer im Vordergrund liegendes Fenster mit den Tabs **Route**, **Himmelskörper** und **Biologie** – alle wichtigen Informationen direkt über dem Spiel.
- **Maus-Durchgriff (Click-Through)**: Das HUD blockiert keine Eingaben im Spiel; Bedienelemente und die Scrollbar werden im Durchgriff-Modus automatisch ausgeblendet.

### Sprachausgabe

- Konfigurierbare Ansagen (Text-to-Speech) für nahezu alle Ereignisse: Begrüßung, geologische/biologische Signale, Erstentdeckungen von Systemen und Körpern, terraformbare und landbare Welten, wertvolle Körper, Gattungsprognosen, Klonkolonie-Reichweite und mehr.
- Jede Ansage lässt sich einzeln ein-/ausschalten und der Text frei anpassen.

### Einstellungen

EDEA lässt sich in nahezu jedem Bereich an die eigenen Bedürfnisse anpassen:

- **Aussehen**: Alle Farben der Oberfläche sind frei wählbar (Farbwähler mit HSV-, Slider- und Hex-Eingabe), die Anzeigegröße ist in vier Stufen einstellbar, und der automatische Tab-Wechsel kann aktiviert werden.
- **HUD-Fenster**: Jede Spalte der drei HUD-Tabs (Route, Himmelskörper, Biologie) lässt sich einzeln ein- oder ausblenden; Deckkraft und dargestellte Ansicht (synchron zum Hauptfenster oder fest gewählt) sind konfigurierbar. Zusätzlich lässt sich festlegen, bei welchen Spiel-Panels das HUD automatisch ausgeblendet wird (z. B. Galaxie- und Systemkarte, FSS-Modus, SAA-Modus, Codex, Stationsdienste, zu Fuß u. v. m.).
- **Sprachausgabe**: Für jedes Ereignis kann der Ansagetext mit Platzhaltern frei formuliert und einzeln aktiviert werden – inklusive Beispielen und Test-Funktion. Stimme, Sprechgeschwindigkeit und Lautstärke sind einstellbar.
- **Planets of Interest**: Eigene Kriteriensätze (Basisattribute, Oberflächenbedingungen, Ringe, Mutterplanet) definieren, welche Welten als besonders interessant markiert werden.
- **Globale Hotkeys**: Tastenkombinationen für wichtige Aktionen sind frei belegbar.
- **Konfiguration**: Pfad zu den Journal-Dateien, Schwellenwerte für wertvolle Körper und Arten sowie die Höhe der Biologie-Ansicht; auf Wunsch lassen sich alle Einstellungen auf die Standardwerte zurücksetzen.

### Historie und Statistik

- **Aktuelle Erkundungstour** und **gesamte Erkundungshistorie** getrennt ausgewertet: entdeckte Systeme und Körper, Erstentdeckungen, häufigste und seltenste Sternklassen/Ringtypen/Arten sowie die geschätzten Gesamtwerte aller Erkundungsdaten.
- **Journal-Import**: Alte Journal-Dateien können nachträglich importiert werden; bereits importierte, unveränderte Dateien werden beim Start übersprungen.
- Alle Daten werden lokal in einer SQLite-Datenbank gespeichert und stehen sofort nach dem Start wieder zur Verfügung.

### Datenquellen

- System- und Routendaten von [EDSM](https://www.edsm.net) und [Spansh](https://www.spansh.co.uk)
- Werte und statistische Daten für die Artenvorhersage von **Canonn Research**

### Datenschutz

Keine Telemetrie, kein Auto-Updater, keine Original-Spiel-Assets. Alle Daten bleiben lokal.

## Technik

- .NET 8 / C#
- Avalonia UI 11 (plattformübergreifende Oberfläche)
- SQLite (Microsoft.Data.Sqlite + Dapper) für die lokale Historie
- CommunityToolkit.Mvvm
- log4net
- Material.Icons.Avalonia
- PixiEditor.ColorPicker.AvaloniaUI
- NAudio + SayIt (Sprachausgabe)

## Bauen und starten

Voraussetzung: .NET 8 SDK.

```powershell
dotnet build EDEA.slnx
dotnet run --project src\EDEA.Avalonia\EDEA.Avalonia.csproj
```

Tests:

```powershell
dotnet test EDEA.slnx
```

Einstellungen, Datenbank und Logs liegen unter `%LOCALAPPDATA%\EDEA`.

## Dokumentation

Weitere technische Dokumentation befindet sich im Ordner `doc/`:

- [Architektur](doc/ARCHITECTURE.md)
- [Erste Schritte](doc/GETTING_STARTED.md)

Den vollständigen Überblick über alle Artikel gibt der `doc/`-Ordner.

## Lizenz

MIT License.
