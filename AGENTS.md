# EDEA Project Notes

## Project Structure

- `src/EDEA.Core/` - Core library with models, view models, services, and stores.
- `src/EDEA.Avalonia/` - Avalonia UI project (successor to the removed WPF project).
- `test/EDEA.Tests/` - Unit tests.

## Build & Test Commands

```powershell
dotnet build EDEA.slnx
dotnet test EDEA.slnx --no-build
```

## Run Avalonia Application

```powershell
dotnet run --project src\EDEA.Avalonia\EDEA.Avalonia.csproj
```

## Important Notes

- The WPF project was removed in Phase 9. The solution now only builds `EDEA.Core`, `EDEA.Avalonia`, and `EDEA.Tests`.
- `PlatformServices` is populated at startup in `EDEA.Avalonia/App.axaml.cs`.
- `EDEA.Avalonia` targets `net8.0` and uses Avalonia 11.
- `AppDataFolder` uses `Environment.SpecialFolder.LocalApplicationData` for cross-platform compatibility.
