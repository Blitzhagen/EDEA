# Changelog

## [1.0.0] - 2026-09-08

EDEA (Elite Dangerous Exploration Assistant) ist ein Open-Source-Begleitprogramm für **Elite Dangerous**, das sich auf die Erkundung der Galaxie spezialisiert.

### Funktionen

- **Live-Journal-Auswertung:** Liest das Spiel-Journal in Echtzeit und zeigt Systeme, Himmelskörper, biologische Signale, Route und Erkundungsdaten an.
- **Systemübersicht:** Anzahl der Körper, Entdeckungsstatus, FSS-Signale (geologisch/biologisch), EDSM-Anreicherung und geschätzte Systemwerte.
- **Himmelskörper:** Terraformbarkeit, Landbarkeit, wertvolle Welten, Ringe, „Planets of Interest“, Entdeckerstatus und geschätzte Kartografie-Werte.
- **Exobiologie:** Artenvorhersage auf Basis von Körpertyp, Atmosphäre und Region (Canonn-Research-Daten), Scan-Fortschritt, Klonkolonie-Abstand und Vista-Genomics-Werte.
- **Route:** NavRoute mit Sprungnummern, Sternklassen, tankbaren Sternen und Systeminformationen; Hotkey zum Kopieren des nächsten Ziels in die Zwischenablage.
- **HUD-Overlay:** Transparentes, immer im Vordergrund liegendes Fenster mit den Tabs **Route**, **Himmelskörper** und **Biologie**; Maus-Durchgriff (Click-Through) wahlweise aktivierbar.
- **Sprachausgabe:** Konfigurierbare Text-to-Speech-Ansagen für fast alle Ereignisse, inklusive individueller Texte und Testfunktion.
- **Einstellungen:** Frei wählbare Farben, vier Schriftgrößenstufen, HUD-Spalten, Deckkraft, automatischer Tab-Wechsel, Globale Hotkeys und mehr.
- **Historie und Statistik:** Lokale SQLite-Datenbank für die aktuelle Tour und die gesamte Erkundungshistorie; Journal-Import älterer Dateien.
- **Datenquellen:** EDSM, Spansh und Canonn Research.

### Technik

- .NET 8 / C#
- Avalonia UI 11
- Windows-x64, self-contained
- SQLite (Microsoft.Data.Sqlite + Dapper)
- log4net, NAudio, SayIt, Material.Icons.Avalonia

### Hinweise

- Dieses Release ist **Windows-only**.
- Einstellungen, Datenbank und Logs liegen unter `%LOCALAPPDATA%\EDEA`.
- Das Release-Paket enthält **keine Debug-Symbole (`.pdb`)** und kein Debug-Logging.
