# EDEA – Testprojekt

Dieses Dokument beschreibt das Testprojekt `test/EDEA.Tests`, die Testabdeckung und die Ausführung der Tests.

## Projektübersicht

| Eigenschaft | Wert |
| --- | --- |
| Projektdatei | `test/EDEA.Tests/EDEA.Tests.csproj` |
| Zielframework | `net8.0` |
| Test-Framework | xUnit |
| Test-SDK | `Microsoft.NET.Test.Sdk` |

## Aktuelle Testabdeckung

Der Schwerpunkt liegt auf der **Biologie-Vorhersage-Engine** (`BiologyRuleEvaluatorTests`): Regel-Auswertung des `bio_catalog.json`-Katalogs gegen Planet- und Systemdaten. Abgedeckt sind u. a.:

- Atmosphären-, Druck-, Temperatur- und Gravitations-Kriterien
- Sternklassen- und Eltern-Stern-Regeln
- Körpertyp-, Vulkanismus- und Material-Regeln
- Regionen-, Guardian- und Tuber-Zonen-Prüfung (echte Zonen-Daten aus `regions.json`)
- Verhalten bei unbekannten Kriterien und fehlerhaften Regeln (kein Treffer statt Abbruch)

Stand: 27 Tests, alle grün.

## Build und Ausführung

```powershell
dotnet build EDEA.slnx
dotnet test EDEA.slnx --no-build
```

## Empfohlene Erweiterungen

- `CodexTracker`-Tests (regions-/galaxisweite Erstentdeckungen)
- `SettingsProvider`-Tests (Sprachphrasen-Migration)
- `SQLiteStore`-CRUD gegen In-Memory-Datenbank
- Konvertertests (`BoolToResourceBrushConverter`, `InverseBoolConverter`)
