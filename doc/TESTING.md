# EDEA – Testprojekt

Dieses Dokument beschreibt das Testprojekt `test/EDEA.Tests`, den aktuellen Stand der Testabdeckung und die Ausführung der Tests.

## Projektübersicht

Das Testprojekt befindet sich im Ordner `test/EDEA.Tests/` und nutzt **xUnit** als Test-Framework.

| Eigenschaft | Wert |
| --- | --- |
| Projektdatei | `test/EDEA.Tests/EDEA.Tests.csproj` |
| Zielframework | `net8.0` |
| Test-Framework | xUnit 2.5.3 |
| Test-SDK | `Microsoft.NET.Test.Sdk` 18.9.0 |
| Coverage-Werkzeug | `coverlet.collector` 6.0.0 |

## Aktuelle Testabdeckung

| Klasse / Datei | Beschreibung |
| --- | --- |
| `UnitTest1` | Platzhalter-Klasse mit einer leeren `Test1`-Methode. |

Die Testabdeckung ist derzeit minimal. Es existiert lediglich ein leerer Platzhalter-Test, der die Build- und Ausführungskette des Testprojekts verifiziert.

## Build und Ausführung

### Gesamte Lösung bauen

```powershell
dotnet build "C:\Users\Matze\Desktop\EDEA\EDEA.slnx"
```

### Nur das Testprojekt bauen

```powershell
dotnet build "C:\Users\Matze\Desktop\EDEA\test\EDEA.Tests\EDEA.Tests.csproj"
```

### Tests ausführen

```powershell
dotnet test "C:\Users\Matze\Desktop\EDEA\EDEA.slnx"
```

Alternativ direkt im Testprojekt:

```powershell
dotnet test "C:\Users\Matze\Desktop\EDEA\test\EDEA.Tests\EDEA.Tests.csproj"
```

## Empfohlene Erweiterungen

Für eine sinnvolle Testabdeckung sollten in Zukunft Kategorien wie folgende ergänzt werden:

- Konvertertests (`Convert` und `ConvertBack`)
- Hilfsmethodentests (`Helpsters`)
- Enum-Validierungen
- SQLiteStore-CRUD-Operationen (In-Memory-Datenbank)
- Befehlstests auf `CanExecute`/`Execute`-Logik
