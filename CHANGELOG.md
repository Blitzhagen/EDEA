# Changelog

## [1.0.0] - 2026-09-08

### Added
- Windows-x64 Publish-Profil (`dotnet publish -p:PublishProfile=Windows`)
- `.gitignore` um `*.log` und `releases/` ergänzt

### Changed
- **AppData-Ordner:** Löscht `EDEA.Core` und verwendet jetzt `%LocalAppData%\EDEA`. Vorhandene Einstellungen, Datenbank, Fensterzustand und Routen-Datei werden automatisch migriert.
- **Logging:** Debug-Logs werden jetzt nur im `Debug`-Build geschrieben; im `Release`-Build werden nur `INFO`, `WARN`, `ERROR` und `FATAL` geloggt.
- **Release-Builds:** `DebugType=none` und `DebugSymbols=false` — keine `.pdb`-Dateien mehr im Release-Paket.
- **Version** auf `1.0.0.0` gesetzt.

### Fixed
- **Body-/Surroundings-HUD:** EDSM-Globe-Icon und Current-Body-GPS-Icon werden jetzt linksbündig ausgerichtet, sodass die Symbole sauber untereinander stehen.
- **Route-HUD:** Scroll-Position, Auto-Scroll, Tab-Guard und Berechnung/Aktualisierung der Route-Status-Flags (`current`/`jump`/`past`) stabilisiert.
