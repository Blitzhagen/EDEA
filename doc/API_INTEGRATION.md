# EDEA – API-Integration

Dieses Dokument beschreibt die externen Web-APIs, die EDEA verwendet. Alle Aufrufe laufen zentral über `WebApiProvider` (`src/EDEA.Core/Services/WebApiProvider.cs`), der Rate-Limits beachtet und parallele Requests drosselt.

## EDSM (Elite Dangerous Star Map)

Basis: `https://www.edsm.net`

| Aufruf | Endpunkt | Zweck |
|--------|----------|-------|
| Systemdaten | `api-v1/system?systemName=…&showPrimaryStar=1&showId=1&showCoordinates=1&showInformation=1` | Basisdaten, Koordinaten, Primärstern, Entdecker-Infos |
| Körper | `api-system-v1/bodies?systemName=…` | Himmelskörper eines Systems |
| Umgebung | `api-v1/sphere-systems?systemName=…&showId=1&showPrimaryStar=1&showCoordinates=1&showInformation=1&radius=…` | Systeme im Radius (Surroundings-Tab) |

EDSM-Daten werden lokal in der SQLite-Datenbank gecacht; bereits bekannte Systeme werden nicht erneut abgefragt.

## Spansh

Basis: `https://spansh.co.uk`

| Aufruf | Endpunkt | Zweck |
|--------|----------|-------|
| Namenssuche | `api/systems/field_values/system_names?q=…` | Autovervollständigung von Systemnamen |
| Generische Route | `api/generic/route` (POST) | Routenberechnung mit Filtern |
| Neutronenroute | `api/route` (POST, Job) | Neutron-Plotter; Ergebnis via `api/results/{jobId}` |
| Fleet-Carrier-Route | `api/fleetcarrier/route?…` | Carrier-Routen; Ergebnis via `api/results/{jobId}` |
| Basisdaten | `api/systems/…` | System-Basisdaten für den Plotter-Dialog |

Job-basierte Routen (Neutron, Carrier) werden asynchron gestartet; das Ergebnis wird später abgeholt (`api/results/{jobId}`) und ist im Browser unter `https://www.spansh.co.uk/plotter/results/{jobId}` bzw. `…/fleet-carrier/results/{jobId}` vergleichbar.

## Canonn Research

| Aufruf | Endpunkt | Zweck |
|--------|----------|-------|
| Biostatistik | `https://api.canonn.tech/biostats?genus=…` | Histogramme/Min-Max-Statistiken pro Genus (Nachschlagewerk) |

Die eigentliche **Artenvorhersage** läuft lokal über `BiologyCatalogProvider`/`BiologyRuleEvaluator` auf Basis von `Resources/bio_catalog.json`, `regions.json` und `nebulae.json` – ohne Laufzeit-Abhängigkeit von Canonn.

## Ressourcen-Strategie

- Zentraler `HttpClient` mit User-Agent aus Assembly-Name/-Version.
- `WebApiProvider` begrenzt gleichzeitige Anfragen und beachtet `x-rate-limit`-Header.
- `WebApiLoadingStatusChanged` meldet Ladezustände an die UI (Statusleiste/Tab-Badges).
- Ohne Internetverbindung arbeitet EDEA mit den lokalen Daten (Datenbank, Journals) weiter; Web-bezogene Funktionen entfallen dann.
