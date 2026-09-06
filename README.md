# ED Exploration Assistant (EDEA)

EDEA ist ein Open-Source-Begleitprogramm (Companion App) für **Elite Dangerous**, das sich auf die **Erkundung der Galaxie** spezialisiert. Das Programm liest das Journal des Spiels live mit und wertet daraus Systeme, Himmelskörper, biologische Signale, Routen und Erkundungsdaten aus.

## Was macht EDEA?

- **Journal-Auswertung in Echtzeit**: EDEA überwacht das aktuelle Elite-Dangerous-Journal sowie `Status.json` und `NavRoute.json` und reagiert sofort auf Sprünge, Scans, Landungen und biologische Analysen.
- **Route**: Anzeige der aktuellen NavRoute mit Informationen zu den anstehenden Systemen (inkl. EDSM-Daten).
- **Himmelskörper**: Tabelle aller Körper im aktuellen System mit Entdeckungsstatus, Terraformbarkeit, kartiertem Status, Wertigkeit und geschätztem kartografischem Wert.
- **Biologie**: Erkennung und Auswertung biologischer Signale, Artenvorhersage auf Basis statistischer Daten von Canonn Research sowie geschätzte Vista-Genomics-Werte.
- **HUD-Fenster**: Ein immer im Vordergrund liegendes, transparentes Overlay mit Maus-Durchgriff (Click-Through), das Route, Himmelskörper oder Biologie direkt über dem Spiel anzeigt.
- **Historie und Statistik**: Auswertung der gesamten Erkundungshistorie und der aktuellen Erkundungstour – Systeme, Körper, Ringe, Biologica, Erstentdeckungen und geschätzte Credit-Werte.
- **Sprachausgabe**: Optionale Ansagen (Text-to-Speech) für Entdeckungen, Signale und Ereignisse.
- **Datenquellen**: System- und Routendaten von [EDSM](https://www.edsm.net) und [Spansh](https://www.spansh.co.uk), Werte und statistische Daten für die Artenvorhersage von **Canonn Research**.
- **Datenschutz**: Keine Telemetrie, kein Auto-Updater, keine Original-Spiel-Assets. Alle Daten werden lokal in einer SQLite-Datenbank gespeichert.

## Ursprung und Dank

Dieses Projekt basiert auf dem Quellcode des ursprünglichen EDEA-Programms. Der ursprüngliche Code wurde durch **Dekompilierung des Original-Programms** gewonnen und als Grundlage für diese Weiterentwicklung verwendet.

Ein besonderer Dank geht an den ursprünglichen Entwickler **CMDR Panostrede**, der uns freundlicherweise erlaubt hat, den Code zu nutzen und das Programm in dieser Form bereitzustellen.

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

Einstellungen, Datenbank und Logs liegen unter `%LOCALAPPDATA%\EDEA.Core`.

## Dokumentation

Weitere technische Dokumentation befindet sich im Ordner `doc/`:

- [Dokumentationsplanung](doc/DOCS_PLAN.md)
- [Architektur](doc/ARCHITECTURE.md)
- [Erste Schritte](doc/GETTING_STARTED.md)

Den vollständigen Überblick über alle Artikel gibt der `doc/`-Ordner.

## Lizenz

MIT License.
