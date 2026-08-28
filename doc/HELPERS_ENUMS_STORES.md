# EDEA – Hilfsklassen, Enums und SQLite-Store

Dieses Dokument fasst die Hilfsklassen, Aufzählungen und den zentralen Datenspeicher von EDEA zusammen.

## ColorThemeHelper

`ColorThemeHelper` in `src/EDEA/Helpers/ColorThemeHelper.cs` überträgt konfigurierte Farbwerte in die WPF-Anwendungsressourcen, damit das Farbschema zur Laufzeit angepasst werden kann.

| Methode | Beschreibung |
| --- | --- |
| `ApplyCurrentColors()` | Wendet alle aktuell konfigurierten Farben auf die Anwendungsressourcen an. |
| `ApplyColor(string propertyName, Color color)` | Sucht alle Ressourcen, die zur angegebenen Farbeigenschaft gehören, und aktualisiert deren Brush. |

## Helpsters

`Helpsters` in `src/EDEA/Helpers/Helpsters.cs` enthält allgemeine Dienstmethoden, die an vielen Stellen in der Anwendung genutzt werden.

| Methode | Beschreibung |
| --- | --- |
| `FirstLetterToUpperCase(string input)` | Wandelt den ersten Buchstaben eines Strings in einen Großbuchstaben um. |
| `RemoveAtmosphereAndVolcanismDescriptors(string input)` | Entfernt bekannte Atmosphären- und Vulkanismus-Präfixe aus einem String. |
| `DoubleToHumanRounded(double journalValue)` | Rundet eine Gleitkommazahl auf eine menschenlesbare Genauigkeit abhängig von ihrer Größenordnung. |
| `GetLatestJournalFile(string? path)` | Liefert die zuletzt geschriebene Journal-Datei in einem Verzeichnis. |
| `GetJournalFiles(string? path)` | Liefert alle Journal-Dateien eines Verzeichnisses sortiert nach Schreibzeit. |
| `ConvertJObjectValue<T>(JsonNode?, string, T)` | Konvertiert einen JSON-Knotenwert in den angegebenen Zieltyp. |
| `DetermineParentIdsOfBody(JsonNode?, DataSource)` | Ermittelt die IDs des Elternsterns und des Elternplaneten eines Body anhand der JSON-Daten. |
| `CheckStarClassForScoopable(string? starClass)` | Prüft, ob eine Sternenklasse zur Treibstoffaufnahme geeignet ist. |
| `CalculateDistanceBetweenSystems(...)` | Berechnet den Abstand zwischen zwei Sternensystemen in Lichtjahren. |
| `GetEdsmValuesFromJournalValue(...)` | Liefert die EDSM-Schlüssel, die einem Journal-Wert entsprechen. |
| `GetUniqueJournalValues(...)` | Liefert eine sortierte Liste eindeutiger Journal-Werte aus einem Dictionary. |
| `GetRingType(string?)` | Parst eine Datenquellen-Beschreibung in den `RingType`. |
| `GetRingTypeSourceDesciption(DataSource, RingType)` | Liefert die Datenquellen-Beschreibung für einen Ringtyp. |
| `GetRingTypeName(RingType)` | Liefert den lokalisierten Namen eines Ringtyps. |
| `GetRingReserveLevelName(RingReserveLevel)` | Liefert den lokalisierten Namen einer Ring-Reserve-Stufe. |
| `GetRingReserveLevel(string?)` | Parst eine Datenquellen-Beschreibung in den `RingReserveLevel`. |
| `OpenHyperlink(Hyperlink?)` | Öffnet den URI eines Hyperlinks im Standardbrowser. |

## SQLiteStore

`SQLiteStore` in `src/EDEA/Stores/SQLiteStore.cs` ist der zentrale Datenbankzugriff per Dapper auf eine SQLite-Datenbank. Er verwaltet Sternensysteme, Körper, Ringe, Gattungen und Planeten-Klassifikationen und bietet Statistikabfragen für den Verlauf.

| Methode / Eigenschaft | Beschreibung |
| --- | --- |
| `Instance` | Singleton-Instanz des Stores. |
| `Initialize()` | Initialisiert Verbindung und Datenbankschema. |
| `GetOrCreateStarSystem(...)` | Liest oder erzeugt ein Sternensystem anhand seiner Id64. |
| `ReadAllStarSystemBasicData()` | Liefert Basisdaten aller Sternensysteme. |
| `InsertOrUpdateStarSystemAsync(...)` | Fügt ein Sternensystem inklusive Körper asynchron ein oder aktualisiert es. |
| `UpsertBody(Body)` | Fügt einen Körper ein oder aktualisiert diesen. |
| `GetBodies(long systemId64)` | Liefert alle Körper eines Sternensystems. |
| `SaveRingsForBody(...)` | Speichert die Ringe eines Körpers. |
| `GetRingsForBody(...)` | Liefert die Ringe eines Körpers. |
| `UpsertGenus(Genus)` | Fügt eine Gattung ein oder aktualisiert diese. |
| `GetGeneraForBody(...)` | Liefert alle Gattungen eines Körpers. |
| `GetBodyCount(long systemId64)` | Zählt die Körper eines Sternensystems. |
| `ClearAllStarSystems()` | Leert alle Sternensystem-, Körper-, Ring- und Gattungsdaten. |
| `ResetTripHistory()` | Setzt das Trip-Historien-Flag für alle Sternensysteme zurück. |
| `ResetIncompleteAnalysisForGenera()` | Setzt unvollständige Genus-Analysen zurück. |
| `ReadAllPlanetClassifications()` | Liefert alle Planeten-Klassifikationen. |

Weitere öffentliche Methoden liefern aggregierte Statistiken für Historie, Cartographie, Biologie und Ringe.

## Resources

`Resources` in `src/EDEA/Properties/Resources.cs` bietet typisierte Lesezugriffe auf die lokalisierten Ressourcenstrings der Anwendung. Über `EDEA.Properties.Resources` können Fenstertitel, Menüeinträge, Statusleistentexte und Dialogbezeichner an die aktuelle UI-Kultur gebunden werden.

## Enums

Aufzählungen befinden sich in `src/EDEA/Enums/`.

### HotkeyId

Identifiziert die Hotkeys der Anwendung.

| Wert | Beschreibung |
| --- | --- |
| `ToggleHudWindow` | HUD-Fenster anzeigen/verbergen. |
| `ToggleHudMousePassThrough` | Maus-Durchgriff des HUD-Fensters umschalten. |
| `OpenRouteTab` | Route-Tab öffnen. |
| `OpenBodiesTab` | Bodies-Tab öffnen. |
| `OpenBiologicalsTab` | Biologicals-Tab öffnen. |
| `OpenSurroundingsTab` | Surroundings-Tab öffnen. |
| `OpenHistoryTab` | History-Tab öffnen. |
| `TryCopyNextSystemToClipboard` | Nächstes Route-System in die Zwischenablage kopieren. |
| `QuitSpeechOutput` | Laufende Sprachausgabe stoppen. |

### SpanshRoutingAlgorithm

Unterstützte Spansh-Routing-Algorithmen.

| Wert | Beschreibung |
| --- | --- |
| `Fuel` | Standard-Treibstoff-Optimierung. |
| `Fuel_Jumps` | Treibstoff- und Sprunganzahl-Optimierung. |
| `Guided` | Geführte Route mit Schritt-für-Schritt-Wegpunkten. |
| `Optimistic` | Optimistische Annahme idealer Bedingungen. |
| `Pessimistic` | Pessimistische Annahme konservativer Bedingungen. |

### StarSystemExplorationStatus

Erforschungszustand eines Sternensystems.

| Wert | Beschreibung |
| --- | --- |
| `Unknown` | Status unbekannt. |
| `Unexplored` | Noch nicht erforscht. |
| `Unscanned` | Noch nicht gescannt. |
| `Incomplete` | Erforschung unvollständig. |
| `Complete` | Erforschung abgeschlossen. |

### SurroundingsRadius

Suchradius für die Surroundings-Ansicht in Lichtjahren.

| Wert | Beschreibung |
| --- | --- |
| `Close` | 15 Lichtjahre. |
| `Midrange` | 30 Lichtjahre. |
| `Far` | 60 Lichtjahre. |
| `Full` | 100 Lichtjahre. |

### UserSelectableInputStringListsKey

Schlüssel für benutzerdefinierbare Auswahllisten in den Planetenfiltern.

| Wert | Beschreibung |
| --- | --- |
| `PlanetClasses` | Planetenklassen. |
| `Atmospheres` | Atmosphären-Typen. |
| `Volcanisms` | Vulkanismus-Typen. |
| `StarClasses` | Sternenklassen. |
| `RingTypes` | Ringtypen. |
| `RingReserveLevels` | Ring-Reserve-Stufen. |

### WepApiQueryType

Web-API-Anfragetypen.

| Wert | Beschreibung |
| --- | --- |
| `GetQuery` | GET-Query-Anfrage. |
| `GetPath` | GET-Pfad-Anfrage. |
| `PostFormUrlEncodedContent` | POST-Anfrage mit formular-kodierter URL-Kodierung. |
