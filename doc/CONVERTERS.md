# EDEA – Wertkonverter und Selektoren

Dieses Dokument beschreibt die in `src/EDEA/Converters/` enthaltenen WPF-Value-Converter, Multi-Value-Converter sowie den `TabTemplateSelector` in `src/EDEA/Selectors/`. Sie kümmern sich um die Darstellung von Daten in XAML, beispielsweise die Umwandlung boolescher Werte in Farben oder Symbole.

## Wertkonverter

| Konverter | Konvertierungsrichtung | Einsatzzweck |
| --- | --- | --- |
| `BooleanToBrushConverter` | `bool` → `Brush` (`ConvertBack` nicht unterstützt) | Wandelt einen booleschen Wert in eine WPF-Farbe um. Über `TrueBrush` und `FalseBrush` lassen sich die Farben konfigurieren. |
| `BooleanToSymbolConverter` | `bool` → `string` (`ConvertBack` nicht unterstützt) | Wandelt einen booleschen Wert in ein Symbol-Zeichen (z. B. Checkmark) um, optional invertiert. |
| `CommanderNameToBrushColorConverter` | Werte-Array → `Brush` (`ConvertBack` nicht unterstützt) | Wählt anhand des Commander-Namens eine Brush-Farbe aus den Anwendungsressourcen aus. |
| `ElementsToCopyToClipboardCommandParameterConverter` | `object[]` → `CopyToClipboardCommandParameter` (`ConvertBack` nicht unterstützt) | Kombiniert einen Text und ein Popup-Objekt zu einem Parameter für `CopyToClipboardCommand`. |
| `EnumToBoolConverter` | `enum` ↔ `bool` | Prüft, ob ein Aufzählungswert dem per Parameter übergebenen Wert entspricht und konvertiert zurück. |
| `IntEqualsConverter` | `int` → `bool` (`ConvertBack` liefert `int` bei `true`) | Vergleicht eine Ganzzahl mit dem im Parameter angegebenen Wert. |
| `InverseBooleanConverter` | `bool` ↔ `bool` | Invertiert einen booleschen Wert in beide Richtungen. |
| `NullToBoolConverter` | `object?` → `bool` (`ConvertBack` nicht unterstützt) | Gibt `true` zurück, wenn der Wert nicht `null` ist. |
| `NullableDoubleToStringConverter` | `double?` ↔ `string` | Konvertiert eine nullable `double`-Zahl in einen String und zurück unter Verwendung der aktuellen Kultur. |
| `NullableLongToStringConverter` | `long?` ↔ `string` | Konvertiert eine nullable `long`-Zahl in einen String und zurück. |
| `SystemExplorationStatusToBrushColorConverter` | `StarSystemExplorationStatus`/Route-View → `Brush` (`ConvertBack` nicht unterstützt) | Wählt eine Farbe anhand des Erforschungs-Status eines Sternensystems oder einer Route. |

## Selektoren

| Selektor | Auswahllogik | Einsatzzweck |
| --- | --- | --- |
| `TabTemplateSelector` | Wählt ein `DataTemplate` anhand des ViewModel-Namens des Tabs. | Ermöglicht tab-spezifische Darstellungen in der Hauptnavigation. |
