# EDEA – Modelle

Alle Modelle liegen in `src/EDEA.Core/Models/` (plus `Models/Biology/`). Sie sind reine Datenklassen bzw. `ObservableObject`s und frei von UI-Abhängigkeiten.

## Kerndaten (Spiel)

| Modell | Inhalt |
|--------|--------|
| `StarSystem` | System: `Id64`, Name, Koordinaten, Sternklassen, Körperliste, Region/Nebel, Herkunfts-Flags (Journal/EDSM). |
| `Planet` | Himmelskörper: Typ, Distanz, Scan-Felder (Atmosphäre/-zusammensetzung, Druck, Temperatur, Gravitation, Vulkanismus, Materialien, Umlaufzeit, Eltern-Sterne), Status-Flags. |
| `Star` | Stern (Typ/Klasse). |
| `BodyType` | Stern/Planet u. a. |
| `Ring`, `RingType`, `RingReserveLevel` | Ringdaten und Klassifikationen. |
| `Genus` | Gefundene biologische Gattung auf einem Körper (Art, Variante, Scans, Positionen). |
| `GenusClassification` | Gattungs-/Arten-Klassifikation (Werte, Wahrscheinlichkeiten). |
| `Ship` | Aktuelles Schiff inkl. FSD-Daten für den Routenplotter. |
| `FleetCarrier` | Flottenträger (Carrier-Routen). |
| `Status`, `EdStatusFlags`, `EdStatusFlags2`, `EdGuiFocus`, `EdFileEvent` | `Status.json`-Auswertung (Aktivität, Ort, GuiFocus, Flags). |
| `LocationOnPlanet` | Position auf einem Körper (für Klonkolonie-Distanzen). |
| `SurfaceScanStatus` | `src/EDEA.Core/` – Kartierungs-Status eines Körpers. |

## Biologie (`Models/Biology/BiologyModels.cs`)

Modelltypen für die BioScan-kompatible Vorhersage-Engine: Katalog-Einträge (Gattungen, Arten, Farbvarianten), Regelwerk-Daten und Auswertungsergebnisse. Datengrundlage: `src/EDEA.Avalonia/Resources/bio_catalog.json` (plus `regions.json`, `nebulae.json`).

## Klassifikationen / Filter

| Modell | Inhalt |
|--------|--------|
| `PlanetClassification` | Benutzerdefinierter Kriteriensatz für „Planets of Interest". |
| `ModuleClassification`, `HyperdriveClassification`, `GuardianFsdBoosterClassification` | Modul-/FSD-Daten aus `mc.dat`. |
| `DistanceRange`, `GravityRange`, `TemperatureRange` | Wertebereiche für Filter/Regeln. |

## Historie und Import

| Modell | Inhalt |
|--------|--------|
| `HistoryData`, `HistoryStatistics` | Auswertung der Erkundungshistorie (Reise/gesamt). |
| `JournalLine` | Eine Journal-Zeile. |
| `JournalSystemMemory`, `JournalSystemMemoryItem`, `JournalPlanetMemory`, `JournalPlanetMemoryItem` | Zwischenstände beim Journal-Parsing. |
| `ImportedJournalFile`, `JournalImportReportData` | Import-Index bzw. -Bericht. |

## Sprachausgabe (Speech-Outputs)

`SpeechOutput` (Basis) und Ableitungen `SpeechOutputBody`, `SpeechOutputCommander`, `SpeechOutputPlanet`, `SpeechOutputRing`, `SpeechOutputSystem`, `SpeechOutputSpecies`, `SpeechOutputPlanetClassification`, `SpeechOutputCartographicValues`, `SpeechOutputItem`, `SpeechOutputMatchingClassificationsCount`, `SpeechOutputRingsCount`, `SpeechOutputValuableSpeciesCount` – kapseln die Platzhalter-Werte pro Ansagetyp; `SpeechOutputPlaceholderKeys` listet alle Schlüssel.

## Einstellungen

`UserSettings` (Aggregat) mit `UserSettingsApplication`, `UserSettingsColors`, `UserSettingsHotkeys`, `UserSettingsHudWindow`, `UserSettingsPlanetsOfInterest`, `UserSettingsSpeech`, `SpanshSettings`; zudem `Hotkey`/`HotkeyItem`, `ColorItem`, `DataSource`.

## Web-API-Parameter

`WebApiRequest`, `WebApiParameter` (+ `…EdsmStarystem`, `…EdsmSurroundings`, `…SpanshBasicSystemData`, `…SpanshGalaxyRoute`), `WepApiQueryData` – Parameterobjekte für EDSM/Spansh-Aufrufe. `CanonnBioStatsObject`/`…Histograms`/`…MinMaxData` modellieren die Canonn-Biostatistik-Antwort.
