# EDEA – Datenmodelle

Dieses Dokument beschreibt alle im Ordner `src/EDEA/Models` enthaltenen Datenmodelle. Die Modelle sind nach fachlichen Bereichen gruppiert und decken Sternensysteme, Klassifikationen, Journalverarbeitung, biologische Statistiken, Benutzereinstellungen, Hotkeys, Sprachausgabe und Web-API-Parameter ab.

---

## 1. Kerndaten

| Klasse | Kurzbeschreibung |
|--------|------------------|
| `StarSystem` | Sternensystem mit seinen Körpern und den zugehörigen Metadaten. |
| `Star` | Stern innerhalb eines Sternensystems. |
| `Body` | Himmelskörper innerhalb eines Sternensystems. |
| `Planet` | Planet innerhalb eines Sternensystems. |
| `Ring` | Planetarer oder stellarrer Ring. |
| `Ship` | Schiff des Spielers und dessen Sprungfähigkeiten. |
| `Genus` | Biologische Gattung mit Scan- und Analysedaten. |
| `Status` | Aktueller Elite-Dangerous-Spielerstatus. |

---

## 2. Klassifikation und Typen

| Klasse | Kurzbeschreibung |
|--------|------------------|
| `BodyType` | Typ eines Himmelskörpers. |
| `GenusClassification` | Klassifikationsdaten einer biologischen Gattung. |
| `GravityRange` | Schwerkraftbereich mit optionalen Min-/Max-Werten. |
| `PlanetClassification` | Filterbare Klassifikation für Planeten. |
| `HyperdriveClassification` | Klassifikation eines Hyperraumantriebsmoduls. |
| `GuardianFsdBoosterClassification` | Klassifikation eines Guardian-Frame-Shift-Drive-Boosters. |
| `ModuleClassification` | Gemeinsame Klassifikationsdaten eines Schiffsmoduls. |
| `DistanceRange` | Entfernungsbereich mit optionalen Min-/Max-Werten. |
| `TemperatureRange` | Temperaturbereich mit optionalen Min-/Max-Werten. |
| `RingType` | Zusammensetzungstyp eines Rings. |
| `RingReserveLevel` | Ressourcenreserveniveau eines Rings. |
| `IntRange` | Ganzzahliger Bereich mit optionalen Min-/Max-Werten. |

---

## 3. Journal und Historie

| Klasse | Kurzbeschreibung |
|--------|------------------|
| `EdFileEvent` | Dateisystemereignis für eine Elite-Dangerous-Journaldatei. |
| `EdGuiFocus` | Aktueller Elite-Dangerous-GUI-Fokus. |
| `EdStatusFlags` | Status-Flags aus der Elite-Dangerous-Statusdatei. |
| `EdStatusFlags2` | Zusätzliche Status-Flags aus der Elite-Dangerous-Statusdatei. |
| `HistoryData` | Aggregiert Erkundungs- und Biologie-Statistiken für eine Historien-Sitzung. |
| `HistoryStatistics` | Häufigste und seltenste Titel einer Historien-Statistik-Kategorie. |
| `JournalImportReportData` | Verfolgt Fortschritt und Ergebnisse eines Journal-Imports. |
| `JournalLine` | Einzelne Zeile aus dem Elite-Dangerous-Journal. |
| `JournalPlanetMemory` | Speichert und aktualisiert Planetendaten aus dem Journal. |
| `JournalPlanetMemoryItem` | Einzelnes Planeten-Datenelement während der Journalverarbeitung. |
| `JournalSystemMemory` | Speichert und aktualisiert Sternensystemdaten aus dem Journal. |
| `JournalSystemMemoryItem` | Einzelnes Sternensystem-Datenelement während der Journalverarbeitung. |
| `LocationOnPlanet` | Geografische Position auf einem Planeten. |

---

## 4. Bio-Statistik (Canonn)

| Klasse | Kurzbeschreibung |
|--------|------------------|
| `CanonnBioStatsHistograms` | Histogrammdaten für Canonn-biologische Statistiken. |
| `CanonnBioStatsMinMaxData` | Einzelner Min-/Max-/Wert-Datensatz für biologische Statistiken. |
| `CanonnBioStatsObject` | Canonn-biologisches Statistik-Objekt. |

---

## 5. Benutzereinstellungen

| Klasse | Kurzbeschreibung |
|--------|------------------|
| `UserSettings` | Vollständige Benutzereinstellungen der Anwendung. |
| `UserSettingsApplication` | Anwendungsbezogene Benutzereinstellungen. |
| `UserSettingsColors` | Farbbasierte Benutzereinstellungen. |
| `UserSettingsHotkeys` | Benutzereinstellungen für Hotkeys. |
| `UserSettingsHudWindow` | Benutzereinstellungen für das HUD-Fenster. |
| `UserSettingsPlanetsOfInterest` | Benutzereinstellungen für Planeten von Interesse. |
| `UserSettingsSpeech` | Sprachbezogene Benutzereinstellungen. |
| `UserSettingsOther` | Verschiedene Benutzereinstellungen. |
| `ColorItem` | Farbeinstellungselement, das an eine Anwendungseigenschaft gebunden ist. |
| `SpanshSettings` | Benutzerdefinierbare Einstellungen für den Spansh-Routenplotter. |
| `DataSource` | Datenquelle eines Modells. |

---

## 6. Hotkeys

| Klasse | Kurzbeschreibung |
|--------|------------------|
| `Hotkey` | Konfigurierbare Tastatur-Hotkey. |
| `HotkeyItem` | Hotkey-Eintrag in der Benutzeroberfläche. |

---

## 7. Sprachausgabe

| Klasse | Kurzbeschreibung |
|--------|------------------|
| `SpeechOutput` | Sprachausgabe mit Platzhalterwerten. |
| `SpeechOutputBody` | Platzhalter für einen Himmelskörper. |
| `SpeechOutputCartographicValues` | Platzhalter für kartografische Werte. |
| `SpeechOutputCommander` | Platzhalter für den Commander-Namen. |
| `SpeechOutputItem` | Bearbeitbares Sprachausgabe-Element, das an Benutzereinstellungen gebunden ist. |
| `SpeechOutputMatchingClassificationsCount` | Platzhalter für die Anzahl passender Klassifikationen. |
| `SpeechOutputPlaceholderKeys` | Identifiziert Platzhalter in Sprachausgabe-Vorlagen. |
| `SpeechOutputPlanet` | Platzhalter für einen Planeten. |
| `SpeechOutputPlanetClassification` | Platzhalter für einen Planetenklassifikations-Treffer. |
| `SpeechOutputRing` | Platzhalter für einen Ring. |
| `SpeechOutputRingsCount` | Platzhalter für die Anzahl der Ringe eines Körpers. |
| `SpeechOutputSpecies` | Platzhalter für eine biologische Art. |
| `SpeechOutputSystem` | Platzhalter für ein Sternensystem. |
| `SpeechOutputValuableSpeciesCount` | Platzhalter für die Anzahl wertvoller Arten. |

---

## 8. Web-API-Parameter

| Klasse | Kurzbeschreibung |
|--------|------------------|
| `WebApiParameter` | Basisklasse für Web-API-Anfrageparameter. |
| `WebApiParameterEdsmStarystem` | Parameter für eine EDSM-Sternensystem-Web-API-Anforderung. |
| `WebApiParameterEdsmSurroundings` | Parameter für eine EDSM-Umgebungs-Web-API-Anforderung. |
| `WebApiParameterSpanshBasicSystemData` | Parameter für eine Spansh-Basis-Systemdaten-Web-API-Anforderung. |
| `WebApiParameterSpanshGalaxyRoute` | Parameter für eine Spansh-Galaxie-Routen-Web-API-Anforderung. |
| `WebApiRequest` | Asynchrone Web-API-Anforderung mit Callback-Behandlung. |
| `WepApiQueryData` | Daten einer Web-API-Abfrage. |

---

## 9. Detaillierte Dokumentation ausgewählter Klassen

### 9.1 `StarSystem`

`StarSystem` ist das zentrale Modell für ein Sternensystem. Es enthält Identifikation, Koordinaten, das zugehörige Hauptgestirn sowie die im System vorhandenen `Body`-Objekte. Über verschiedene Flags wird nachvollzogen, ob die Daten aus dem Elite-Dangerous-Journal, EDSM oder einer Route stammen. Die Klasse bietet Methoden zum Hinzufügen und Aktualisieren von Körpern und zur Berechnung der Tankbarkeit des Primärsterns.

| Eigenschaft | Typ | Beschreibung |
|-------------|-----|--------------|
| `Id` | `long` | System-ID. |
| `Name` | `string` | Name des Sternensystems. |
| `StarPositionX` | `double?` | X-Koordinate der Sternposition. |
| `StarPositionY` | `double?` | Y-Koordinate der Sternposition. |
| `StarPositionZ` | `double?` | Z-Koordinate der Sternposition. |
| `StarClass` | `string?` | Klasse des Primärsterns. |
| `PrimaryStarName` | `string?` | Name des Primärsterns. |
| `PrimaryStarIsScoopable` | `bool` | Gibt an, ob der Primärstern tankbar ist. |
| `Bodies` | `IReadOnlyDictionary<int, Body>` | Körper des Systems, indiziert nach Körper-ID. |
| `JumpDistance` | `int` | Sprungdistanz. |
| `JumpDistanceLy` | `double` | Sprungdistanz in Lichtjahren. |
| `TotalBodyCount` | `int` | Gesamtzahl bekannter Körper. |
| `TotalNonBodyCount` | `int` | Anzahl der Nicht-Himmelskörper. |
| `NavBeaconScanBodyCount` | `int` | Anzahl gescannter Körper per Navigationsbake. |
| `AllBodiesFound` | `bool` | Alle Körper des Systems gefunden. |
| `WasReadFromJournal` | `bool` | System aus dem Journal gelesen. |
| `WasReadFromEdsm` | `bool` | System aus EDSM gelesen. |
| `WasRequestedFromEdsm` | `bool` | System von EDSM angefragt. |
| `WasReadFromEdsmOnly` | `bool` | System nur aus EDSM, nicht aus dem Journal. |
| `NeedsEdsmSystemUpdate` | `bool` | EDSM-Systemupdate erforderlich. |
| `NeedsEdsmBodiesUpdate` | `bool` | EDSM-Körperupdate erforderlich. |
| `IsPastSystemInRoute` | `bool` | Liegt als vergangenes System in der Route. |
| `IsCurrentSystemInRoute` | `bool` | Ist das aktuelle System in der Route. |
| `IsJumpDestinationSystemInRoute` | `bool` | Ist Sprungziel in der Route. |
| `IsSystemInRouteAhead` | `bool` | Liegt voraus in der Route. |
| `EdsmName` | `string?` | Name gemäß EDSM. |
| `EdsmPrimaryStarType` | `string?` | EDSM-Primärstern-Typ. |
| `EdsmPrimaryStarName` | `string?` | EDSM-Primärstern-Name. |
| `EdsmPrimaryStarIsScoopable` | `bool` | EDSM-Primärstern tankbar. |
| `EdsmTotalBodyCount` | `int?` | EDSM-Gesamtkörperanzahl. |
| `IsTripHistory` | `bool` | System ist Teil der Reisehistorie. |
| `Population` | `long` | Population des Systems. |

### 9.2 `Planet`

`Planet` erbt von `Body` und ergänzt planetenspezifische Eigenschaften wie Klasse, Landbarkeit, Terraforming-Status, Vulkanismus, Atmosphäre, Signale und Gattungen. Die Klasse berechnet und verwaltet außerdem kartografische Werte für Oberflächenscans.

| Eigenschaft | Typ | Beschreibung |
|-------------|-----|--------------|
| `PlanetClass` | `string` | Klasse des Planeten. |
| `IsLandable` | `bool` | Gibt an, ob der Planet landbar ist. |
| `TerraformingState` | `string` | Status der Terraformisierung. |
| `WasMapped` | `bool` | Planet wurde kartografiert. |
| `WasFootfalled` | `bool?` | Planet wurde betreten (optional). |
| `SurfaceScanned` | `bool` | Oberfläche wurde gescannt. |
| `Gravity` | `double` | Oberflächengravitation. |
| `SurfaceTemperature` | `double` | Oberflächentemperatur. |
| `GeologicalCount` | `int` | Anzahl geologischer Signale. |
| `BiologicalCount` | `int` | Anzahl biologischer Signale. |
| `IsCurrentPlanetInSystem` | `bool` | Ist der aktuelle Planet im System. |
| `Genuses` | `IReadOnlyDictionary<string, Genus>` | Auf dem Planeten gefundene Gattungen. |
| `HasGenera` | `bool` | Planet besitzt Gattungen. |
| `Touchdown` | `bool` | Touchdown wurde durchgeführt. |
| `Volcanism` | `string` | Vulkanismus-Beschreibung. |
| `Atmosphere` | `string` | Atmosphären-Beschreibung. |
| `ParentStarId` | `int?` | ID des Elternsterns. |
| `ParentPlanetId` | `int?` | ID eines Elternplaneten (Monde). |
| `PredictedSpecies` | `List<GenusClassification>` | Vorhergesagte Arten. |
| `InitialPredictionOfSpecies` | `bool` | Initiale Artvorhersage wurde durchgeführt. |
| `MatchingPlanetClassifications` | `List<PlanetClassification>` | Passende Planetenklassifikationen. |
| `MatchingPlanetClassificationsAnnounced` | `bool` | Passende Klassifikationen wurden angesagt. |
| `IsTerraformable` | `bool` | Planet ist terraformierbar. |
| `ParentStar` | `Star?` | Elternstern des Planeten. |
| `ParentPlanet` | `Planet?` | Elternplanet (bei Monden). |
| `EfficientlyScanned` | `bool` | Planet wurde effizient gescannt. |
| `CartographicSurfaceScanValue` | `int` | Wert des Oberflächenscans. |
| `CartographicFirstSurfaceScanBonusValue` | `int` | Bonus für den ersten Oberflächenscan. |
| `CartographicEfficientlyScannedBonusValue` | `int` | Bonus für das effiziente Scannen. |

### 9.3 `Body`

`Body` ist die Basisklasse für alle Himmelskörper. Sie enthält die gemeinsamen Eigenschaften wie Identifikation, Name, Typ, Entfernung, Masse, Radius, kartografische Werte, Ringe und Quelleninformationen. Von `Body` leiten sich `Planet` und `Star` ab.

| Eigenschaft | Typ | Beschreibung |
|-------------|-----|--------------|
| `Id64` | `long` | 64-Bit-ID für SQLite-Primärschlüssel-Persistenz. |
| `Id` | `int` | Körper-ID. |
| `BodyId` | `int` | Alias für `Id` (Spaltenmapping). |
| `StarSystemId` | `long` | ID des übergeordneten Sternensystems. |
| `SystemId64` | `long` | Alias für `StarSystemId` (Spaltenmapping). |
| `Name` | `string` | Name des Körpers. |
| `ShortName` | `string` | Kurzname relativ zum System. |
| `Type` | `BodyType` | Typ des Körpers. |
| `IsPlanet` | `bool` | Körper ist ein Planet. |
| `IsStar` | `bool` | Körper ist ein Stern. |
| `IsPlanetOrStar` | `bool` | Körper ist Planet oder Stern. |
| `Distance` | `double` | Entfernung vom Ankunftspunkt. |
| `WasDiscovered` | `bool` | Körper wurde bereits entdeckt. |
| `WasReadFromJournal` | `bool` | Körper aus dem Journal gelesen. |
| `WasReadFromEdsm` | `bool` | Körper aus EDSM gelesen. |
| `WasReadFromEdsmOnly` | `bool` | Körper nur aus EDSM, nicht aus dem Journal. |
| `EdsmDiscoveryCommander` | `string?` | Name des EDSM-Entdeckers. |
| `EdsmDiscoverer` | `string?` | Alias für `EdsmDiscoveryCommander`. |
| `StarSystem` | `StarSystem?` | Übergeordnetes Sternensystem. |
| `Radius` | `double` | Radius des Körpers. |
| `Mass` | `double` | Masse des Körpers. |
| `CartographicValue` | `int` | Aktueller kartografischer Wert. |
| `CartographicMaxValue` | `int` | Maximaler erreichbarer kartografischer Wert. |
| `CartographicBaseValue` | `int` | Basiswert für Kartografie. |
| `CartographicFirstDiscoveryBonusValue` | `int` | Erstentdeckungsbonus. |
| `CartographicFirstDiscoveryBonusWithoutEfficiencyValue` | `int` | Erstentdeckungsbonus ohne Effizienz. |
| `CartographicFirstDiscoveryBonusWithoutSurfaceScanValue` | `int` | Erstentdeckungsbonus ohne Oberflächenscan. |
| `Rings` | `IReadOnlyDictionary<string, Ring>` | Ringe des Körpers. |
| `HasRings` | `bool` | Körper besitzt Ringe. |
| `RingsReserveLevel` | `RingReserveLevel` | Reserveniveau der Ringe. |
| `RingsTotalWidth` | `long` | Gesamtbreite aller Ringe. |
| `OrbitalInclination` | `double?` | Bahnneigung. |
| `HasBiological` | `bool` | Körper hat biologische Signale. |
| `HasGeological` | `bool` | Körper hat geologische Signale. |
| `IsValuable` | `bool` | Körper gilt als wertvoll. |

### 9.4 `UserSettings`

`UserSettings` ist das Wurzelmodell für alle Anwendungseinstellungen. Es setzt sich aus mehreren thematischen Untereinstellungen zusammen, die über `CommunityToolkit.Mvvm` `[ObservableProperty]`-Attribute als öffentliche Eigenschaften generiert werden. Die Klasse bietet eine zentrale Methode, um alle oder einzelne Einstellungen auf ihre Standardwerte zurückzusetzen.

| Eigenschaft | Typ | Beschreibung |
|-------------|-----|--------------|
| `Speech` | `UserSettingsSpeech` | Sprachausgabe-Einstellungen. |
| `Other` | `UserSettingsOther` | Sonstige Einstellungen (Pfade, Schwellenwerte). |
| `Application` | `UserSettingsApplication` | Anwendungsbezogene Einstellungen. |
| `Colors` | `UserSettingsColors` | Farbschema-Einstellungen. |
| `Hotkeys` | `UserSettingsHotkeys` | Tastenkürzel-Einstellungen. |
| `HudWindow` | `UserSettingsHudWindow` | HUD-Fenster-Einstellungen. |
| `PlanetsOfInterest` | `UserSettingsPlanetsOfInterest` | Filter und Klassifikationen interessanter Planeten. |
| `Spansh` | `SpanshSettings` | Einstellungen für den Spansh-Routenplotter. |

| Methode | Beschreibung |
|---------|--------------|
| `SetDefaultValues(PropertyInfo? singlePropertyInfo = null)` | Setzt alle oder eine einzelne Einstellung auf den Standardwert zurück. |

### 9.5 `WebApiRequest`

`WebApiRequest` kapselt eine asynchrone HTTP-Anforderung an externe APIs wie EDSM oder Spansh. Im Konstruktor wird die Anforderung initialisiert und sofort versendet. Die Klasse berücksichtigt Aktivitätslimits, EDSM-Rate-Limiting und führt beim Abschluss einen definierten Callback aus.

| Eigenschaft | Typ | Beschreibung |
|-------------|-----|--------------|
| `Id` | `Guid` | Eindeutige Kennung der Anforderung. |
| `Parameter` | `WebApiParameter` | Parameter-Objekt, das an den Callback übergeben wird. |

Neben den öffentlichen Eigenschaften implementiert die Klasse `IEquatable<WebApiRequest>` und vergleicht Anforderungen anhand ihrer `Id`. Die interne Verarbeitung unterstützt GET-Abfragen über Query oder Pfad sowie POST-Anforderungen mit Form-URL-kodiertem Inhalt und reagiert auf `X-Rate-Limit-Remaining` Header.
