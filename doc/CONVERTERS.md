# EDEA – Converter und Behaviors

Wertkonverter und Attached Behaviors für die Avalonia-Oberfläche (`src/EDEA.Avalonia/`). Die früheren WPF-`IValueConverter` existieren nicht mehr.

## Converter (`Converters/`)

| Converter | Zweck |
|-----------|-------|
| `BoolToResourceBrushConverter` | Mappt `bool` auf eine Resource-Brush. `ConverterParameter` im Format `TrueBrush;FalseBrush[;DefaultBrush]` – nützlich für Zustandsfarben (z. B. abgeschlossen/wertvoll). |
| `InverseBoolConverter` | `true` ↔ `false` – für `IsVisible`-Negationen u. ä. |
| `SystemExplorationStatusToBrushColorConverter` | Färbt den `StarSystemExplorationStatus` (unentdeckt/teilweise/vollständig) mit den Statusfarben ein. |
| `TabHeaderToHeightConverter` | Rechnet Tab-Header-Schriftgröße/-Höhe auf kompakte Header um. |
| `TabVisibilityConverter` | Blendet Tabs anhand des `TabVisibility`-Status ein/aus. |

## Behaviors und Controls (`Helpers/`, `Controls/`)

| Element | Zweck |
|---------|-------|
| `DataGridSingleSortBehavior` | Erzwingt Sortierung nach genau einer Spalte (Avalonia-DataGrid erlaubt sonst Multi-Sort). |
| `ToolTipDataContextBehavior` | Reicht den DataContext in Tooltips weiter, damit Tooltip-Templates binden können; statisch initialisiert in `App.Initialize`. |
| `BadgePanel` | Kleines Panel für Icon+Zähler-Badges in Grid-Zellen (z. B. Signal-Icons mit Anzahl). |

## Lokalisierungs-Markup (`Localization/`)

| Element | Zweck |
|---------|-------|
| `LocExtension` | Markup-Extension `{l:Loc ResourceKey}` – liefert eine Binding auf `LocalizedStrings`. |
| `LocalizedStrings` | Indexer über `EDEA.Properties.Resources.Lookup`; invalidiert alle Bindungen bei `Resources.CultureChanged` (Live-Sprachwechsel). |
