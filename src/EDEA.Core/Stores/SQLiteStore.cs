using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EDEA.Models;
using Microsoft.Data.Sqlite;
using log4net;

namespace EDEA.Stores;

/// <summary>
/// Provides SQLite database access for storing and retrieving star systems, bodies, rings, and genera.
/// </summary>
public class SQLiteStore
{
    /// <summary>
    /// The singleton instance of the store.
    /// </summary>
    private static SQLiteStore? instance;

    /// <summary>
    /// The connection string used to open the SQLite database.
    /// </summary>
    private readonly string _connectionString;

    /// <summary>
    /// Whether the database schema has already been initialized.
    /// </summary>
    private bool _initialized;

    /// <summary>
    /// The logger used by this store.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(SQLiteStore));

    /// <summary>
    /// Occurs when the star systems table has been updated.
    /// </summary>
    public event EventHandler? DatabaseStarSystemTableUpdated;

    private const string UPSERT_STAR_SYSTEM_SQL = @"
            INSERT INTO StarSystems (
                Id64,
                Name,
                StarClass,
                PrimaryStarName,
                TotalBodyCount,
                TotalNonBodyCount,
                WasReadFromJournal,
                WasReadFromEdsm,
                WasRequestedFromEdsm,
                EdsmName,
                EdsmPrimaryStarType,
                EdsmPrimaryStarName,
                EdsmPrimaryStarIsScoopable,
                EdsmTotalBodyCount,
                StarPositionX,
                StarPositionY,
                StarPositionZ,
                IsTripHistory,
                AllBodiesFound,
                Population,
                Region
            ) VALUES (
                @Id,
                @Name,
                @StarClass,
                @PrimaryStarName,
                @TotalBodyCount,
                @TotalNonBodyCount,
                @WasReadFromJournal,
                @WasReadFromEdsm,
                @WasRequestedFromEdsm,
                @EdsmName,
                @EdsmPrimaryStarType,
                @EdsmPrimaryStarName,
                @EdsmPrimaryStarIsScoopable,
                @EdsmTotalBodyCount,
                @StarPositionX,
                @StarPositionY,
                @StarPositionZ,
                @IsTripHistory,
                @AllBodiesFound,
                @Population,
                @Region
            ) ON CONFLICT(Id64) DO UPDATE SET
                PrimaryStarName = CASE WHEN @PrimaryStarName IS NULL OR TRIM(@PrimaryStarName) = '' THEN PrimaryStarName ELSE @PrimaryStarName END,
                TotalBodyCount = @TotalBodyCount,
                TotalNonBodyCount = @TotalNonBodyCount,
                WasReadFromJournal = CASE WHEN WasReadFromJournal = 1 THEN WasReadFromJournal ELSE @WasReadFromJournal END,
                WasReadFromEdsm = CASE WHEN WasReadFromEdsm = 1 THEN WasReadFromEdsm ELSE @WasReadFromEdsm END,
                WasRequestedFromEdsm = CASE WHEN WasRequestedFromEdsm = 1 THEN WasRequestedFromEdsm ELSE @WasRequestedFromEdsm END,
                EdsmName = @EdsmName,
                EdsmPrimaryStarType = @EdsmPrimaryStarType,
                EdsmPrimaryStarName = @EdsmPrimaryStarName,
                EdsmPrimaryStarIsScoopable = @EdsmPrimaryStarIsScoopable,
                EdsmTotalBodyCount = @EdsmTotalBodyCount,
                StarPositionX = @StarPositionX,
                StarPositionY = @StarPositionY,
                StarPositionZ = @StarPositionZ,
                IsTripHistory = @IsTripHistory,
                AllBodiesFound = @AllBodiesFound,
                Population = @Population,
                Region = CASE WHEN @Region IS NULL THEN Region ELSE @Region END
        ";

    private const string UPSERT_BODY_SQL = @"
            INSERT INTO Bodies (
                Id64,
                StarSystemId,
                Id,
                Name,
                Type,
                Distance,
                WasDiscovered,
                WasReadFromJournal,
                WasReadFromEdsm,
                EdsmDiscoveryCommander,
                Radius,
                Mass,
                OrbitalInclination,
                CartographicValue,
                CartographicMaxValue,
                CartographicBaseValue,
                CartographicFirstDiscoveryBonusValue,
                CartographicFirstDiscoveryBonusWithoutEfficiencyValue,
                CartographicFirstDiscoveryBonusWithoutSurfaceScanValue,
                RingsReserveLevel,
                HasBiological,
                HasGeological,
                IsValuable
            ) VALUES (
                @Id64,
                @StarSystemId,
                @Id,
                @Name,
                @Type,
                @Distance,
                @WasDiscovered,
                @WasReadFromJournal,
                @WasReadFromEdsm,
                @EdsmDiscoveryCommander,
                @Radius,
                @Mass,
                @OrbitalInclination,
                @CartographicValue,
                @CartographicMaxValue,
                @CartographicBaseValue,
                @CartographicFirstDiscoveryBonusValue,
                @CartographicFirstDiscoveryBonusWithoutEfficiencyValue,
                @CartographicFirstDiscoveryBonusWithoutSurfaceScanValue,
                @RingsReserveLevel,
                @HasBiological,
                @HasGeological,
                @IsValuable
            ) ON CONFLICT(Id64) DO UPDATE SET
                Name = @Name,
                Type = @Type,
                Distance = @Distance,
                WasDiscovered = @WasDiscovered,
                WasReadFromJournal = CASE WHEN WasReadFromJournal = 1 THEN WasReadFromJournal ELSE @WasReadFromJournal END,
                WasReadFromEdsm = CASE WHEN WasReadFromEdsm = 1 THEN WasReadFromEdsm ELSE @WasReadFromEdsm END,
                EdsmDiscoveryCommander = @EdsmDiscoveryCommander,
                Radius = @Radius,
                Mass = @Mass,
                OrbitalInclination = @OrbitalInclination,
                CartographicValue = @CartographicValue,
                CartographicMaxValue = @CartographicMaxValue,
                CartographicBaseValue = @CartographicBaseValue,
                CartographicFirstDiscoveryBonusValue = @CartographicFirstDiscoveryBonusValue,
                CartographicFirstDiscoveryBonusWithoutEfficiencyValue = @CartographicFirstDiscoveryBonusWithoutEfficiencyValue,
                CartographicFirstDiscoveryBonusWithoutSurfaceScanValue = @CartographicFirstDiscoveryBonusWithoutSurfaceScanValue,
                RingsReserveLevel = @RingsReserveLevel,
                HasBiological = @HasBiological,
                HasGeological = @HasGeological,
                IsValuable = @IsValuable
        ";

    private const string UPSERT_PLANET_BODY_SQL = @"
            INSERT INTO Bodies (
                Id64,
                StarSystemId,
                Id,
                Name,
                Type,
                Distance,
                WasDiscovered,
                WasMapped,
                WasFootfalled,
                WasReadFromJournal,
                WasReadFromEdsm,
                EdsmDiscoveryCommander,
                PlanetClass,
                IsLandable,
                TerraformingState,
                SurfaceScanned,
                Gravity,
                GeologicalCount,
                BiologicalCount,
                SurfaceTemperature,
                Touchdown,
                Volcanism,
                Atmosphere,
                Radius,
                ParentStarId,
                ParentPlanetId,
                Mass,
                OrbitalInclination,
                EfficientlyScanned,
                CartographicValue,
                CartographicMaxValue,
                CartographicBaseValue,
                CartographicFirstDiscoveryBonusValue,
                CartographicSurfaceScanValue,
                CartographicFirstSurfaceScanBonusValue,
                CartographicEfficientlyScannedBonusValue,
                CartographicFirstDiscoveryBonusWithoutEfficiencyValue,
                CartographicFirstDiscoveryBonusWithoutSurfaceScanValue,
                RingsReserveLevel,
                HasBiological,
                HasGeological,
                IsValuable,
                IsTerraformable,
                AtmosphereType,
                AtmosphereCompositionCsv,
                SurfacePressure,
                MaterialsCsv,
                OrbitalPeriod
            ) VALUES (
                @Id64,
                @StarSystemId,
                @Id,
                @Name,
                @Type,
                @Distance,
                @WasDiscovered,
                @WasMapped,
                @WasFootfalled,
                @WasReadFromJournal,
                @WasReadFromEdsm,
                @EdsmDiscoveryCommander,
                @PlanetClass,
                @IsLandable,
                @TerraformingState,
                @SurfaceScanned,
                @Gravity,
                @GeologicalCount,
                @BiologicalCount,
                @SurfaceTemperature,
                @Touchdown,
                @Volcanism,
                @Atmosphere,
                @Radius,
                @ParentStarId,
                @ParentPlanetId,
                @Mass,
                @OrbitalInclination,
                @EfficientlyScanned,
                @CartographicValue,
                @CartographicMaxValue,
                @CartographicBaseValue,
                @CartographicFirstDiscoveryBonusValue,
                @CartographicSurfaceScanValue,
                @CartographicFirstSurfaceScanBonusValue,
                @CartographicEfficientlyScannedBonusValue,
                @CartographicFirstDiscoveryBonusWithoutEfficiencyValue,
                @CartographicFirstDiscoveryBonusWithoutSurfaceScanValue,
                @RingsReserveLevel,
                @HasBiological,
                @HasGeological,
                @IsValuable,
                @IsTerraformable,
                @AtmosphereType,
                @AtmosphereCompositionCsv,
                @SurfacePressure,
                @MaterialsCsv,
                @OrbitalPeriod
            ) ON CONFLICT(Id64) DO UPDATE SET
                Name = @Name,
                Type = @Type,
                Distance = @Distance,
                WasDiscovered = @WasDiscovered,
                WasMapped = @WasMapped,
                WasFootfalled = @WasFootfalled,
                WasReadFromJournal = CASE WHEN WasReadFromJournal = 1 THEN WasReadFromJournal ELSE @WasReadFromJournal END,
                WasReadFromEdsm = CASE WHEN WasReadFromEdsm = 1 THEN WasReadFromEdsm ELSE @WasReadFromEdsm END,
                EdsmDiscoveryCommander = @EdsmDiscoveryCommander,
                PlanetClass = @PlanetClass,
                IsLandable = @IsLandable,
                TerraformingState = @TerraformingState,
                SurfaceScanned = @SurfaceScanned,
                Gravity = @Gravity,
                GeologicalCount = @GeologicalCount,
                BiologicalCount = @BiologicalCount,
                SurfaceTemperature = @SurfaceTemperature,
                Touchdown = @Touchdown,
                Volcanism = @Volcanism,
                Atmosphere = @Atmosphere,
                Radius = @Radius,
                ParentStarId = @ParentStarId,
                ParentPlanetId = @ParentPlanetId,
                Mass = @Mass,
                OrbitalInclination = @OrbitalInclination,
                EfficientlyScanned = @EfficientlyScanned,
                CartographicValue = @CartographicValue,
                CartographicMaxValue = @CartographicMaxValue,
                CartographicBaseValue = @CartographicBaseValue,
                CartographicFirstDiscoveryBonusValue = @CartographicFirstDiscoveryBonusValue,
                CartographicSurfaceScanValue = @CartographicSurfaceScanValue,
                CartographicFirstSurfaceScanBonusValue = @CartographicFirstSurfaceScanBonusValue,
                CartographicEfficientlyScannedBonusValue = @CartographicEfficientlyScannedBonusValue,
                CartographicFirstDiscoveryBonusWithoutEfficiencyValue = @CartographicFirstDiscoveryBonusWithoutEfficiencyValue,
                CartographicFirstDiscoveryBonusWithoutSurfaceScanValue = @CartographicFirstDiscoveryBonusWithoutSurfaceScanValue,
                RingsReserveLevel = @RingsReserveLevel,
                HasBiological = @HasBiological,
                HasGeological = @HasGeological,
                IsValuable = @IsValuable,
                IsTerraformable = @IsTerraformable,
                AtmosphereType = @AtmosphereType,
                AtmosphereCompositionCsv = @AtmosphereCompositionCsv,
                SurfacePressure = @SurfacePressure,
                MaterialsCsv = @MaterialsCsv,
                OrbitalPeriod = @OrbitalPeriod
        ";

    private const string UPSERT_STAR_BODY_SQL = @"
            INSERT INTO Bodies (
                Id64,
                StarSystemId,
                Id,
                Name,
                Type,
                Distance,
                WasDiscovered,
                WasReadFromJournal,
                WasReadFromEdsm,
                EdsmDiscoveryCommander,
                StarType,
                Luminosity,
                Radius,
                Mass,
                OrbitalInclination,
                CartographicValue,
                CartographicMaxValue,
                CartographicBaseValue,
                CartographicFirstDiscoveryBonusValue,
                CartographicFirstDiscoveryBonusWithoutEfficiencyValue,
                CartographicFirstDiscoveryBonusWithoutSurfaceScanValue,
                RingsReserveLevel,
                HasBiological,
                HasGeological,
                IsValuable
            ) VALUES (
                @Id64,
                @StarSystemId,
                @Id,
                @Name,
                @Type,
                @Distance,
                @WasDiscovered,
                @WasReadFromJournal,
                @WasReadFromEdsm,
                @EdsmDiscoveryCommander,
                @StarType,
                @Luminosity,
                @Radius,
                @Mass,
                @OrbitalInclination,
                @CartographicValue,
                @CartographicMaxValue,
                @CartographicBaseValue,
                @CartographicFirstDiscoveryBonusValue,
                @CartographicFirstDiscoveryBonusWithoutEfficiencyValue,
                @CartographicFirstDiscoveryBonusWithoutSurfaceScanValue,
                @RingsReserveLevel,
                @HasBiological,
                @HasGeological,
                @IsValuable
            ) ON CONFLICT(Id64) DO UPDATE SET
                Name = @Name,
                Type = @Type,
                Distance = @Distance,
                WasDiscovered = @WasDiscovered,
                WasReadFromJournal = CASE WHEN WasReadFromJournal = 1 THEN WasReadFromJournal ELSE @WasReadFromJournal END,
                WasReadFromEdsm = CASE WHEN WasReadFromEdsm = 1 THEN WasReadFromEdsm ELSE @WasReadFromEdsm END,
                EdsmDiscoveryCommander = @EdsmDiscoveryCommander,
                StarType = @StarType,
                Luminosity = @Luminosity,
                Radius = @Radius,
                Mass = @Mass,
                OrbitalInclination = @OrbitalInclination,
                CartographicValue = @CartographicValue,
                CartographicMaxValue = @CartographicMaxValue,
                CartographicBaseValue = @CartographicBaseValue,
                CartographicFirstDiscoveryBonusValue = @CartographicFirstDiscoveryBonusValue,
                CartographicFirstDiscoveryBonusWithoutEfficiencyValue = @CartographicFirstDiscoveryBonusWithoutEfficiencyValue,
                CartographicFirstDiscoveryBonusWithoutSurfaceScanValue = @CartographicFirstDiscoveryBonusWithoutSurfaceScanValue,
                RingsReserveLevel = @RingsReserveLevel,
                HasBiological = @HasBiological,
                HasGeological = @HasGeological,
                IsValuable = @IsValuable
        ";

    private const string UPSERT_RING_SQL = @"
            INSERT INTO Rings (Name, BodyId, StarSystemId, Type, Mass, InnerRadius, OuterRadius, Width, Density)
            VALUES (@Name, @BodyId, @StarSystemId, @Type, @Mass, @InnerRadius, @OuterRadius, @Width, @Density)
            ON CONFLICT(StarSystemId, BodyId, Name) DO UPDATE SET
                Type = @Type,
                Mass = @Mass,
                InnerRadius = @InnerRadius,
                OuterRadius = @OuterRadius,
                Width = @Width,
                Density = @Density
        ";

    private const string UPSERT_GENUS_SQL = @"
            INSERT INTO Genera (Name, BodyId, StarSystemId, Species, Variant, CodexKey, SpeciesKey, VariantKey, ScanCount, IsAnalysed, VistaGenomicsValue, LongitudeAt1stScan, LatitudeAt1stScan, LongitudeAt2ndScan, LatitudeAt2ndScan, VistaGenomicsMaxValue, VistaGenomicsBaseValue, VistaGenomicsFirstDiscoveryBonusValue, IsFirstDiscovered, WasLogged)
            VALUES (@Name, @BodyId, @StarSystemId, @Species, @Variant, @CodexKey, @SpeciesKey, @VariantKey, @ScanCount, @IsAnalysed, @VistaGenomicsValue, @LongitudeAt1stScan, @LatitudeAt1stScan, @LongitudeAt2ndScan, @LatitudeAt2ndScan, @VistaGenomicsMaxValue, @VistaGenomicsBaseValue, @VistaGenomicsFirstDiscoveryBonusValue, @IsFirstDiscovered, @WasLogged)
            ON CONFLICT(StarSystemId, BodyId, Name) DO UPDATE SET
                Species = @Species,
                Variant = @Variant,
                CodexKey = CASE WHEN @CodexKey IS NULL OR @CodexKey = '' THEN CodexKey ELSE @CodexKey END,
                SpeciesKey = CASE WHEN @SpeciesKey IS NULL OR @SpeciesKey = '' THEN SpeciesKey ELSE @SpeciesKey END,
                VariantKey = CASE WHEN @VariantKey IS NULL OR @VariantKey = '' THEN VariantKey ELSE @VariantKey END,
                ScanCount = @ScanCount,
                IsAnalysed = @IsAnalysed,
                VistaGenomicsValue = @VistaGenomicsValue,
                LongitudeAt1stScan = @LongitudeAt1stScan,
                LatitudeAt1stScan = @LatitudeAt1stScan,
                LongitudeAt2ndScan = @LongitudeAt2ndScan,
                LatitudeAt2ndScan = @LatitudeAt2ndScan,
                VistaGenomicsMaxValue = @VistaGenomicsMaxValue,
                VistaGenomicsBaseValue = @VistaGenomicsBaseValue,
                VistaGenomicsFirstDiscoveryBonusValue = @VistaGenomicsFirstDiscoveryBonusValue,
                IsFirstDiscovered = @IsFirstDiscovered,
                WasLogged = @WasLogged
        ";

    /// <summary>
    /// Returns the singleton instance of the <see cref="SQLiteStore"/>.
    /// </summary>
    /// <param name="dbPath">Optional path to the SQLite database file.</param>
    /// <returns>The <see cref="SQLiteStore"/> instance.</returns>
    public static SQLiteStore Instance(string? dbPath = null)
    {
        if (instance == null)
        {
            instance = new SQLiteStore(dbPath);
        }
        return instance;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SQLiteStore"/> class.
    /// </summary>
    /// <param name="dbPath">The path to the SQLite database file, or <c>null</c> to use the default path.</param>
    private SQLiteStore(string? dbPath)
    {
        var path = dbPath ?? Path.Combine(
            Globals.AppDataFolder,
            "edea.sqlite");

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        _connectionString = $"Data Source={path}";
        EnsureSchema();
    }

    /// <summary>
    /// Opens a new database connection.
    /// </summary>
    /// <returns>An open database connection.</returns>
    private IDbConnection Connect() => new SqliteConnection(_connectionString);

    /// <summary>
    /// Creates the database schema if it does not already exist.
    /// </summary>
    private void EnsureSchema()
    {
        if (_initialized) return;

        using var db = Connect();
        db.Open();
        db.Execute(@"
            CREATE TABLE IF NOT EXISTS StarSystems (
                Id64 INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                StarClass TEXT,
                PrimaryStarName TEXT,
                TotalBodyCount INTEGER,
                TotalNonBodyCount INTEGER,
                WasReadFromJournal INTEGER,
                WasReadFromEdsm INTEGER,
                WasRequestedFromEdsm INTEGER,
                EdsmName TEXT,
                EdsmPrimaryStarType TEXT,
                EdsmPrimaryStarName TEXT,
                EdsmPrimaryStarIsScoopable INTEGER,
                EdsmTotalBodyCount INTEGER,
                StarPositionX REAL,
                StarPositionY REAL,
                StarPositionZ REAL,
                IsTripHistory INTEGER NOT NULL DEFAULT 0,
                AllBodiesFound INTEGER,
                Population INTEGER,
                Region INTEGER
            );

            CREATE TABLE IF NOT EXISTS Bodies (
                Id64 INTEGER PRIMARY KEY,
                StarSystemId INTEGER NOT NULL,
                Id INTEGER NOT NULL,
                Name TEXT NOT NULL,
                Type INTEGER NOT NULL,
                Distance REAL NOT NULL,
                WasDiscovered INTEGER,
                WasMapped INTEGER,
                WasFootfalled INTEGER,
                WasReadFromJournal INTEGER,
                WasReadFromEdsm INTEGER,
                EdsmDiscoveryCommander TEXT,
                PlanetClass TEXT,
                IsLandable INTEGER,
                TerraformingState TEXT,
                SurfaceScanned INTEGER,
                Gravity REAL,
                GeologicalCount INTEGER,
                BiologicalCount INTEGER,
                StarType TEXT,
                SurfaceTemperature REAL,
                Touchdown INTEGER,
                Volcanism TEXT,
                Atmosphere TEXT,
                Radius REAL,
                ParentStarId INTEGER,
                ParentPlanetId INTEGER,
                Mass REAL,
                OrbitalInclination REAL,
                EfficientlyScanned INTEGER,
                CartographicValue INTEGER,
                CartographicMaxValue INTEGER,
                CartographicBaseValue INTEGER,
                CartographicFirstDiscoveryBonusValue INTEGER,
                CartographicSurfaceScanValue INTEGER,
                CartographicFirstSurfaceScanBonusValue INTEGER,
                CartographicEfficientlyScannedBonusValue INTEGER,
                CartographicFirstDiscoveryBonusWithoutEfficiencyValue INTEGER,
                CartographicFirstDiscoveryBonusWithoutSurfaceScanValue INTEGER,
                RingsReserveLevel INTEGER NOT NULL DEFAULT 0,
                HasGeological INTEGER,
                HasBiological INTEGER,
                IsValuable INTEGER,
                IsTerraformable INTEGER,
                AtmosphereType TEXT,
                AtmosphereCompositionCsv TEXT,
                SurfacePressure REAL,
                MaterialsCsv TEXT,
                OrbitalPeriod REAL,
                Luminosity TEXT
            );

            CREATE TABLE IF NOT EXISTS Rings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                BodyId INTEGER NOT NULL,
                StarSystemId INTEGER NOT NULL,
                Type INTEGER NOT NULL,
                Mass INTEGER NOT NULL,
                InnerRadius INTEGER NOT NULL,
                OuterRadius INTEGER NOT NULL,
                Width INTEGER NOT NULL,
                Density REAL NOT NULL,
                UNIQUE(StarSystemId, BodyId, Name)
            );

            CREATE TABLE IF NOT EXISTS Genera (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                BodyId INTEGER NOT NULL,
                StarSystemId INTEGER NOT NULL,
                Species TEXT,
                Variant TEXT,
                ScanCount INTEGER,
                IsAnalysed INTEGER,
                VistaGenomicsValue REAL,
                LongitudeAt1stScan REAL,
                LatitudeAt1stScan REAL,
                LongitudeAt2ndScan REAL,
                LatitudeAt2ndScan REAL,
                VistaGenomicsMaxValue INTEGER,
                VistaGenomicsBaseValue INTEGER,
                VistaGenomicsFirstDiscoveryBonusValue INTEGER,
                IsFirstDiscovered INTEGER,
                WasLogged INTEGER,
                CodexKey TEXT,
                SpeciesKey TEXT,
                VariantKey TEXT,
                UNIQUE(StarSystemId, BodyId, Name)
            );

            CREATE TABLE IF NOT EXISTS CodexScans (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Region INTEGER NOT NULL,
                Biological TEXT NOT NULL,
                StarSystemId INTEGER,
                UNIQUE(Region, Biological)
            );

            CREATE TABLE IF NOT EXISTS ImportedJournalFiles (
                FileName TEXT PRIMARY KEY,
                LastWriteTimeUtc INTEGER NOT NULL,
                Length INTEGER NOT NULL
            );

        ");

        // Schema migration for databases created before the biology rewrite.
        EnsureColumn(db, "StarSystems", "Region", "INTEGER");
        EnsureColumn(db, "Bodies", "AtmosphereType", "TEXT");
        EnsureColumn(db, "Bodies", "AtmosphereCompositionCsv", "TEXT");
        EnsureColumn(db, "Bodies", "SurfacePressure", "REAL");
        EnsureColumn(db, "Bodies", "MaterialsCsv", "TEXT");
        EnsureColumn(db, "Bodies", "OrbitalPeriod", "REAL");
        EnsureColumn(db, "Bodies", "Luminosity", "TEXT");
        EnsureColumn(db, "Genera", "CodexKey", "TEXT");
        EnsureColumn(db, "Genera", "SpeciesKey", "TEXT");
        EnsureColumn(db, "Genera", "VariantKey", "TEXT");

        // Schema version 2 (biology rewrite): the new body fields only exist in journal
        // Scan events, so journals imported before this version must be re-imported once.
        long userVersion = db.ExecuteScalar<long>("PRAGMA user_version");
        if (userVersion < 3)
        {
            db.Execute("DELETE FROM ImportedJournalFiles");
            db.Execute("PRAGMA user_version = 3");
            log.Info("Journal file import list cleared - journals will be re-imported once to backfill biology scan data");
        }

        _initialized = true;
    }

    /// <summary>
    /// Adds a column to a table when it does not exist yet.
    /// </summary>
    /// <param name="db">The open database connection.</param>
    /// <param name="table">The table name.</param>
    /// <param name="column">The column name.</param>
    /// <param name="type">The SQLite column type.</param>
    private static void EnsureColumn(IDbConnection db, string table, string column, string type)
    {
        // PRAGMA table_info returns rows; the column name is the second field.
        var existing = db.Query<(long cid, string name, string type, long notnull, object? dflt_value, long pk)>($"PRAGMA table_info({table})")
            .Select(row => row.name).ToList();
        if (!existing.Contains(column))
        {
            db.Execute($"ALTER TABLE {table} ADD COLUMN {column} {type}");
            log.Info($"Added column {column} to table {table}");
        }
    }

    /// <summary>
    /// Stores a codex biological discovery for the given region (idempotent).
    /// </summary>
    /// <param name="region">The galactic region identifier.</param>
    /// <param name="biological">The codex entry name.</param>
    /// <param name="starSystemId">The star system identifier.</param>
    public void UpsertCodexScan(int region, string biological, long starSystemId)
    {
        using var db = Connect();
        db.Open();
        db.Execute(@"
            INSERT INTO CodexScans (Region, Biological, StarSystemId)
            VALUES (@region, @biological, @starSystemId)
            ON CONFLICT(Region, Biological) DO UPDATE SET StarSystemId = @starSystemId",
            new { region, biological, starSystemId });
    }

    /// <summary>
    /// Checks whether a codex biological identifier exists, optionally limited to a region.
    /// </summary>
    /// <param name="region">The region identifier, or <see langword="null"/> for a galaxy-wide check.</param>
    /// <param name="biological">The codex entry name.</param>
    /// <returns><see langword="true"/> when a matching entry exists.</returns>
    public bool CodexScanExists(int? region, string biological)
    {
        using var db = Connect();
        db.Open();
        if (region.HasValue)
        {
            return db.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM CodexScans WHERE Region = @region AND Biological = @biological",
                new { region = region.Value, biological }) > 0;
        }
        return db.ExecuteScalar<int>(
            "SELECT COUNT(1) FROM CodexScans WHERE Biological = @biological",
            new { biological }) > 0;
    }

    /// <summary>
    /// Retrieves a star system by its Id64.
    /// </summary>
    /// <param name="id64">The star system identifier.</param>
    /// <returns>The star system, or <c>null</c> when not found.</returns>
    public StarSystem? GetStarSystem(long id64)
    {
        using var db = Connect();
        db.Open();
        return db.QueryFirstOrDefault<StarSystem>(
            @"SELECT Id64 AS Id, Name, StarClass, PrimaryStarName, TotalBodyCount, TotalNonBodyCount,
                     WasReadFromJournal, WasReadFromEdsm, WasRequestedFromEdsm,
                     EdsmName, EdsmPrimaryStarType, EdsmPrimaryStarName, EdsmPrimaryStarIsScoopable, EdsmTotalBodyCount,
                     StarPositionX, StarPositionY, StarPositionZ, IsTripHistory, AllBodiesFound, Population, Region
              FROM StarSystems WHERE Id64 = @id64",
            new { id64 });
    }

    /// <summary>
    /// Gets an existing star system or creates it when it does not exist.
    /// </summary>
    /// <param name="id64">The star system identifier.</param>
    /// <param name="name">The star system name.</param>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="z">The Z coordinate.</param>
    /// <param name="starClass">The optional star class.</param>
    /// <returns>The existing or newly created star system.</returns>
    public StarSystem GetOrCreateStarSystem(long id64, string name, double x, double y, double z, string? starClass)
    {
        using var db = Connect();
        db.Open();

        var existing = db.QueryFirstOrDefault<StarSystem>(
            @"SELECT Id64 AS Id, Name, StarClass, StarPositionX, StarPositionY, StarPositionZ, IsTripHistory
              FROM StarSystems WHERE Id64 = @id64",
            new { id64 });

        if (existing is not null)
            return existing;

        var s = new StarSystem { Id = id64, Name = name, StarPositionX = x, StarPositionY = y, StarPositionZ = z, StarClass = starClass, IsTripHistory = true };
        s.UpdateRegion();
        db.Execute(@"
            INSERT INTO StarSystems (Id64, Name, StarClass, StarPositionX, StarPositionY, StarPositionZ, IsTripHistory)
            VALUES (@Id, @Name, @StarClass, @StarPositionX, @StarPositionY, @StarPositionZ, @IsTripHistory)
            ON CONFLICT(Id64) DO NOTHING
        ", s);

        return s;
    }

    /// <summary>
    /// Reads basic data for all star systems.
    /// </summary>
    /// <returns>A list of star systems with basic data.</returns>
    public List<StarSystem> ReadAllStarSystemBasicData()
    {
        using var db = Connect();
        db.Open();
        try
        {
            return db.Query<StarSystem>(
                @"SELECT
                    Id64 AS Id,
                    Name,
                    StarClass,
                    PrimaryStarName,
                    TotalBodyCount,
                    TotalNonBodyCount,
                    WasReadFromJournal,
                    WasReadFromEdsm,
                    WasRequestedFromEdsm,
                    EdsmName,
                    EdsmPrimaryStarType,
                    EdsmPrimaryStarName,
                    EdsmPrimaryStarIsScoopable,
                    EdsmTotalBodyCount,
                    StarPositionX,
                    StarPositionY,
                    StarPositionZ,
                    IsTripHistory,
                    AllBodiesFound,
                    Population,
                    Region
                FROM StarSystems").ToList();
        }
        catch (Exception exception)
        {
            log.Error("Could not read star system basic data from database", exception);
        }
        return new List<StarSystem>();
    }

    /// <summary>
    /// Reads a star system and all associated bodies from the database.
    /// </summary>
    /// <param name="starSystemId">The star system identifier.</param>
    /// <returns>The star system with its bodies, or <c>null</c> when not found.</returns>
    public StarSystem? ReadStarSystem(long starSystemId)
    {
        if (starSystemId == 0L)
            return null;

        using var db = Connect();
        db.Open();
        try
        {
            var bodies = new List<Body>();
            using (var dataReader = db.ExecuteReader(
                "SELECT * FROM Bodies WHERE StarSystemId = @starSystemId", new { starSystemId }))
            {
                var bodyParser = dataReader.GetRowParser<Body>();
                var planetParser = dataReader.GetRowParser<Planet>();
                var starParser = dataReader.GetRowParser<Star>();

                while (dataReader.Read())
                {
                    try
                    {
                        var type = (BodyType)dataReader.GetInt32(dataReader.GetOrdinal("Type"));
                        Body? body = type switch
                        {
                            BodyType.Planet => planetParser(dataReader),
                            BodyType.Star => starParser(dataReader),
                            _ => bodyParser(dataReader)
                        };
                        if (body != null)
                            bodies.Add(body);
                    }
                    catch (Exception exception)
                    {
                        log.Error($"Could not parse a body row for star system {starSystemId} from database", exception);
                    }
                }
            }

            var starSystem = db.QueryFirstOrDefault<StarSystem>(
                @"SELECT Id64 AS Id, Name, StarClass, PrimaryStarName, TotalBodyCount, TotalNonBodyCount,
                         WasReadFromJournal, WasReadFromEdsm, WasRequestedFromEdsm,
                         EdsmName, EdsmPrimaryStarType, EdsmPrimaryStarName, EdsmPrimaryStarIsScoopable, EdsmTotalBodyCount,
                         StarPositionX, StarPositionY, StarPositionZ, IsTripHistory, AllBodiesFound, Population, Region
                  FROM StarSystems WHERE Id64 = @starSystemId",
                new { starSystemId });

            if (starSystem == null)
                return null;

            foreach (var body in bodies)
            {
                body.StarSystem = starSystem;
                if (body is Planet planet)
                {
                    try
                    {
                        var genera = db.Query<Genus>(
                            "SELECT * FROM Genera WHERE StarSystemId = @starSystemId AND BodyId = @bodyId",
                            new { starSystemId = planet.StarSystemId, bodyId = planet.Id }).ToList();
                        foreach (var genus in genera)
                            planet.TryAddOrUpdateGenus(genus);
                    }
                    catch (Exception exception)
                    {
                        log.Error($"Could not read genera for planet '{planet.Name}' from database", exception);
                    }
                }

                try
                {
                    var rings = db.Query<Ring>(
                        "SELECT * FROM Rings WHERE StarSystemId = @starSystemId AND BodyId = @bodyId",
                        new { starSystemId = body.StarSystemId, bodyId = body.Id }).ToList();
                    if (rings.Count > 0)
                    {
                        foreach (var ring in rings)
                            body.TryAddOrUpdateRing(ring, DataSource.History);
                    }
                }
                catch (Exception exception)
                {
                    log.Error($"Could not read rings for body '{body.Name}' ({body.Id}) from database", exception);
                }

                starSystem.TryAddOrUpdateBody(body, ignoreSpeechOutput: true, DataSource.History, out var _);
            }

            return starSystem;
        }
        catch (Exception exception)
        {
            log.Error($"Could not read star system '{starSystemId}' from database: {exception}", exception);
        }
        return null;
    }

    /// <summary>
    /// Inserts or updates a body in the database.
    /// </summary>
    /// <param name="body">The body to persist.</param>
    public void UpsertBody(Body body)
    {
        body.HasBiological = body is Planet biologicalPlanet && biologicalPlanet.BiologicalCount > 0;
        body.HasGeological = body is Planet geologicalPlanet && geologicalPlanet.GeologicalCount > 0;
        body.IsValuable = body.CartographicMaxValue >= Preferences.Other.ValuableBodyThreshold;

        using var db = Connect();
        db.Open();
        try
        {
            if (body is Planet planet)
                db.Execute(UPSERT_PLANET_BODY_SQL, planet);
            else if (body is Star star)
                db.Execute(UPSERT_STAR_BODY_SQL, star);
            else
                db.Execute(UPSERT_BODY_SQL, body);
        }
        catch (Exception exception)
        {
            log.Error($"Could not upsert body '{body.Name}' ({body.Id}) to database", exception);
        }
    }

    /// <summary>
    /// Gets a body by its Id64.
    /// </summary>
    /// <param name="id64">The body identifier.</param>
    /// <returns>The body, or <c>null</c> when not found.</returns>
    public Body? GetBody(long id64)
    {
        using var db = Connect();
        db.Open();
        return db.QueryFirstOrDefault<Body>(
            "SELECT * FROM Bodies WHERE Id64 = @id64", new { id64 });
    }

    /// <summary>
    /// Gets all bodies belonging to the specified star system.
    /// </summary>
    /// <param name="systemId64">The star system identifier.</param>
    /// <returns>The bodies in the star system.</returns>
    public IEnumerable<Body> GetBodies(long systemId64)
    {
        using var db = Connect();
        db.Open();
        return db.Query<Body>(
            "SELECT * FROM Bodies WHERE StarSystemId = @systemId64", new { systemId64 });
    }

    /// <summary>
    /// Replaces the rings for a body with the specified rings.
    /// </summary>
    /// <param name="systemId64">The star system identifier.</param>
    /// <param name="bodyId">The body identifier.</param>
    /// <param name="rings">The rings to persist.</param>
    public void SaveRingsForBody(long systemId64, int bodyId, IEnumerable<Ring> rings)
    {
        using var db = Connect();
        db.Open();
        db.Execute("DELETE FROM Rings WHERE StarSystemId = @systemId64 AND BodyId = @bodyId",
            new { systemId64, bodyId });

        foreach (var ring in rings)
        {
            db.Execute(@"
                INSERT INTO Rings (Name, BodyId, StarSystemId, Type, Mass, InnerRadius, OuterRadius, Width, Density)
                VALUES (@Name, @BodyId, @StarSystemId, @Type, @Mass, @InnerRadius, @OuterRadius, @Width, @Density)
            ", ring);
        }
    }

    /// <summary>
    /// Gets all rings for the specified body.
    /// </summary>
    /// <param name="systemId64">The star system identifier.</param>
    /// <param name="bodyId">The body identifier.</param>
    /// <returns>The rings of the body.</returns>
    public IEnumerable<Ring> GetRingsForBody(long systemId64, int bodyId)
    {
        using var db = Connect();
        db.Open();
        return db.Query<Ring>(
            "SELECT * FROM Rings WHERE StarSystemId = @systemId64 AND BodyId = @bodyId", new { systemId64, bodyId });
    }

    /// <summary>
    /// Inserts or updates a genus in the database.
    /// </summary>
    /// <param name="genus">The genus to persist.</param>
    public void UpsertGenus(Genus genus)
    {
        using var db = Connect();
        db.Open();
        try
        {
            db.Execute(UPSERT_GENUS_SQL, genus);
        }
        catch (Exception exception)
        {
            log.Error($"Could not upsert genus '{genus.Name}' to database", exception);
        }
    }

    /// <summary>
    /// Gets all genera for the specified body.
    /// </summary>
    /// <param name="systemId64">The star system identifier.</param>
    /// <param name="bodyId">The body identifier.</param>
    /// <returns>The genera of the body.</returns>
    public IEnumerable<Genus> GetGeneraForBody(long systemId64, int bodyId)
    {
        using var db = Connect();
        db.Open();
        return db.Query<Genus>(
            "SELECT * FROM Genera WHERE StarSystemId = @systemId64 AND BodyId = @bodyId", new { systemId64, bodyId });
    }

    /// <summary>
    /// Counts the bodies in the specified star system.
    /// </summary>
    /// <param name="systemId64">The star system identifier.</param>
    /// <returns>The number of bodies.</returns>
    public int GetBodyCount(long systemId64)
    {
        using var db = Connect();
        db.Open();
        return db.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM Bodies WHERE StarSystemId = @systemId64", new { systemId64 });
    }

    /// <summary>
    /// Inserts or updates a star system and all its bodies asynchronously.
    /// </summary>
    /// <param name="starSystem">The star system to persist.</param>
    /// <param name="raiseDatabaseStarSystemTableUpdatedEvent">Whether to raise the <see cref="DatabaseStarSystemTableUpdated"/> event after persisting.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InsertOrUpdateStarSystemAsync(StarSystem starSystem, bool raiseDatabaseStarSystemTableUpdatedEvent = true)
    {
        using var db = Connect();
        db.Open();
        try
        {
            await db.ExecuteAsync(UPSERT_STAR_SYSTEM_SQL, starSystem);
        }
        catch (Exception exception)
        {
            log.Error($"Could not insert star system '{starSystem.Name}' ({starSystem.Id}) to database", exception);
        }

        foreach (Body body in starSystem.Bodies.Values)
        {
            body.StarSystem = starSystem;
            body.StarSystemId = starSystem.Id;
            body.Id64 = starSystem.Id * 1000 + body.Id;
            body.HasBiological = body is Planet biologicalPlanet && biologicalPlanet.BiologicalCount > 0;
            body.HasGeological = body is Planet geologicalPlanet && geologicalPlanet.GeologicalCount > 0;
            body.IsValuable = body.CartographicMaxValue >= Preferences.Other.ValuableBodyThreshold;

            try
            {
                if (body.GetType() == typeof(Body))
                {
                    await db.ExecuteAsync(UPSERT_BODY_SQL, body);
                }
                else if (body is Planet planet)
                {
                    await db.ExecuteAsync(UPSERT_PLANET_BODY_SQL, planet);
                    if (planet.HasGenera)
                    {
                        try
                        {
                            await db.ExecuteAsync(UPSERT_GENUS_SQL, planet.Genuses.Values.ToList());
                        }
                        catch (Exception exception)
                        {
                            log.Error($"Could not insert genera of body '{body.Name}' ({body.Id}) to database", exception);
                        }
                    }
                }
                else if (body is Star star)
                {
                    await db.ExecuteAsync(UPSERT_STAR_BODY_SQL, star);
                }

                if (body.HasRings)
                {
                    try
                    {
                        await db.ExecuteAsync(UPSERT_RING_SQL, body.Rings.Values.ToList());
                    }
                    catch (Exception exception)
                    {
                        log.Error($"Could not insert rings of body '{body.Name}' ({body.Id}) to database", exception);
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error($"Could not insert body '{body.Name}' ({body.Id}) to database", exception);
            }
        }

        if (raiseDatabaseStarSystemTableUpdatedEvent)
        {
            DatabaseStarSystemTableUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Clears all data from the star systems, bodies, rings, and genera tables.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ClearAllStarSystems()
    {
        using var db = Connect();
        db.Open();
        try
        {
            await db.ExecuteAsync("DELETE FROM Rings");
            await db.ExecuteAsync("DELETE FROM Genera");
            await db.ExecuteAsync("DELETE FROM Bodies");
            await db.ExecuteAsync("DELETE FROM StarSystems");
        }
        catch (Exception exception)
        {
            log.Error("Could not clear all star systems", exception);
        }
        DatabaseStarSystemTableUpdated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Resets the trip history flag for all star systems.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ResetTripHistory()
    {
        using var db = Connect();
        db.Open();
        try
        {
            await db.ExecuteAsync("UPDATE StarSystems SET IsTripHistory = 0 WHERE IsTripHistory = 1");
        }
        catch (Exception exception)
        {
            log.Error("Could not reset trip history", exception);
        }
        DatabaseStarSystemTableUpdated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Retrieves all journal files that have already been imported.
    /// </summary>
    /// <returns>The imported journal files.</returns>
    public IEnumerable<ImportedJournalFile> GetImportedJournalFiles()
    {
        using var db = Connect();
        db.Open();
        try
        {
            return db.Query<ImportedJournalFile>("SELECT FileName, LastWriteTimeUtc, Length FROM ImportedJournalFiles").ToList();
        }
        catch (Exception exception)
        {
            log.Error("Could not read imported journal files from database", exception);
        }
        return Enumerable.Empty<ImportedJournalFile>();
    }

    /// <summary>
    /// Records that a set of journal files has been imported.
    /// </summary>
    /// <param name="files">The journal files to record.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RecordImportedJournalFilesAsync(IEnumerable<ImportedJournalFile> files)
    {
        if (files == null)
        {
            return;
        }

        using var db = Connect();
        db.Open();
        try
        {
            var fileList = files.ToList();
            if (fileList.Count > 0)
            {
                await db.ExecuteAsync(@"
                    INSERT INTO ImportedJournalFiles (FileName, LastWriteTimeUtc, Length)
                    VALUES (@FileName, @LastWriteTimeUtc, @Length)
                    ON CONFLICT(FileName) DO UPDATE SET
                        LastWriteTimeUtc = excluded.LastWriteTimeUtc,
                        Length = excluded.Length", fileList);
            }
        }
        catch (Exception exception)
        {
            log.Error("Could not record imported journal files", exception);
        }
    }

    /// <summary>
    /// Resets the incomplete analysis data for all genera.
    /// </summary>
    /// <returns>The number of updated rows.</returns>
    public async Task<int> ResetIncompleteAnalysisForGenera()
    {
        using var db = Connect();
        db.Open();
        try
        {
            return await db.ExecuteAsync(@"
                UPDATE Genera
                SET ScanCount = 0, LongitudeAt1stScan = NULL, LatitudeAt1stScan = NULL, LongitudeAt2ndScan = NULL, LatitudeAt2ndScan = NULL
                WHERE ScanCount > 0 AND IsAnalysed = 0");
        }
        catch (Exception exception)
        {
            log.Error("Cannot update Genera and reset incomplete analysis for genera", exception);
        }
        return 0;
    }

    /// <summary>
    /// Reads all planet classifications from the database.
    /// </summary>
    /// <returns>A list of planet classifications.</returns>
    public List<PlanetClassification> ReadAllPlanetClassifications()
    {
        using var db = Connect();
        db.Open();
        try
        {
            return db.Query<PlanetClassification>("SELECT * FROM PlanetClassification").ToList();
        }
        catch (Exception exception)
        {
            log.Warn("Could not read planet classifications from database", exception);
        }
        return new List<PlanetClassification>();
    }

    #region HistoryProvider support

    /// <summary>
    /// Gets the count of first discovery systems.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the count to the current trip.</param>
    /// <returns>The first discovery system count.</returns>
    public async Task<int> GetSystemFirstDiscoveryCount(bool tripOnly = false)
    {
        var sql = "SELECT count(StarSystems.Id64) FROM StarSystems LEFT JOIN Bodies ON StarSystems.PrimaryStarName = Bodies.Name AND Bodies.StarSystemId = StarSystems.Id64 WHERE Bodies.WasDiscovered = 0" + (tripOnly ? " AND StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<int?>(sql) ?? 0;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get system first discovery count", exception);
        }
        return 0;
    }

    /// <summary>
    /// Gets the sum of cartographic base values for all bodies.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the sum to the current trip.</param>
    /// <returns>The total cartographic base value.</returns>
    public async Task<long> GetBodyCartographicBaseValueSum(bool tripOnly = false)
    {
        var sql = "SELECT ifnull(sum(Bodies.CartographicBaseValue),0) FROM Bodies LEFT JOIN StarSystems ON Bodies.StarSystemId = StarSystems.Id64 WHERE Bodies.WasReadFromJournal = 1" + (tripOnly ? " AND StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<long?>(sql) ?? 0L;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get body cartographic base value sum", exception);
        }
        return 0L;
    }

    /// <summary>
    /// Gets statistics about star class frequencies.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the statistics to the current trip.</param>
    /// <returns>The star class statistics.</returns>
    public async Task<HistoryStatistics> GetSystemStarClassStatistics(bool tripOnly = false)
    {
        var sql = "SELECT StarClass, count(*) as Amount from StarSystems WHERE StarClass IS NOT NULL" + (tripOnly ? " AND IsTripHistory = 1" : string.Empty) + " GROUP BY StarClass ORDER BY Amount DESC";
        var starClassStats = new HistoryStatistics();
        using var db = Connect();
        db.Open();
        try
        {
            var rows = (await db.QueryAsync(sql)).ToList();
            if (rows.Count > 0)
            {
                starClassStats.MostFrequentTitle = Convert.ToString(rows.First().StarClass);
                starClassStats.MostFrequentCount = Convert.ToInt32(rows.First().Amount);
                starClassStats.RarestTitle = Convert.ToString(rows.Last().StarClass);
                starClassStats.RarestCount = Convert.ToInt32(rows.Last().Amount);
            }
        }
        catch (Exception exception)
        {
            log.Error("Cannot get system star class statistics", exception);
        }
        return starClassStats;
    }

    /// <summary>
    /// Gets the highest sum of discovery and EDSM body counts.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the sum to the current trip.</param>
    /// <returns>The body signal sum.</returns>
    public async Task<int> GetBodySignalSum(bool tripOnly = false)
    {
        var sql = "SELECT ifnull(sum(TotalBodyCount),0) as Discovery, ifnull(sum(EdsmTotalBodyCount), 0) as Edsm FROM StarSystems" + (tripOnly ? " WHERE IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            var rows = (await db.QueryAsync(sql)).ToList();
            if (rows.Count > 0)
            {
                return Math.Max(Convert.ToInt32(rows.First().Discovery), Convert.ToInt32(rows.First().Edsm));
            }
        }
        catch (Exception exception)
        {
            log.Error("Cannot get body signal sum", exception);
        }
        return 0;
    }

    /// <summary>
    /// Gets the count of bodies.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the count to the current trip.</param>
    /// <returns>The body count.</returns>
    public async Task<int> GetBodyCount(bool tripOnly = false)
    {
        var sql = "SELECT count(Bodies.Id64) FROM Bodies LEFT JOIN StarSystems ON Bodies.StarSystemId = StarSystems.Id64 WHERE Bodies.Type != 0" + (tripOnly ? " AND StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<int?>(sql) ?? 0;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get body count", exception);
        }
        return 0;
    }

    /// <summary>
    /// Gets the count of first discovery bodies.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the count to the current trip.</param>
    /// <returns>The first discovery body count.</returns>
    public async Task<int> GetBodyFirstDiscoveryCount(bool tripOnly = false)
    {
        var sql = "SELECT count(Bodies.Id64) FROM Bodies LEFT JOIN StarSystems ON Bodies.StarSystemId = StarSystems.Id64 WHERE Bodies.Type != 0 AND Bodies.WasDiscovered = 0" + (tripOnly ? " AND StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<int?>(sql) ?? 0;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get body first discovery count", exception);
        }
        return 0;
    }

    /// <summary>
    /// Gets the count of terraformable bodies.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the count to the current trip.</param>
    /// <returns>The terraformable body count.</returns>
    public async Task<int> GetBodyTerraformableCount(bool tripOnly = false)
    {
        var sql = "SELECT count(Bodies.Id64) FROM Bodies LEFT JOIN StarSystems ON Bodies.StarSystemId = StarSystems.Id64 WHERE (Bodies.TerraformingState = 'Terraformable' OR Bodies.TerraformingState = 'Candidate for terraforming')" + (tripOnly ? " AND StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<int?>(sql) ?? 0;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get body terraformable count", exception);
        }
        return 0;
    }

    /// <summary>
    /// Gets the count of valuable bodies.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the count to the current trip.</param>
    /// <returns>The valuable body count.</returns>
    public async Task<int> GetBodyValuableBodyCount(bool tripOnly = false)
    {
        var sql = "SELECT count(Bodies.Id64) FROM Bodies LEFT JOIN StarSystems ON Bodies.StarSystemId = StarSystems.Id64 WHERE Bodies.CartographicMaxValue >= @threshold" + (tripOnly ? " AND StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<int?>(sql, new { threshold = Preferences.Other.ValuableBodyThreshold }) ?? 0;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get valuable body count", exception);
        }
        return 0;
    }

    /// <summary>
    /// Gets the sum of cartographic surface scan values for all bodies.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the sum to the current trip.</param>
    /// <returns>The total surface scan value.</returns>
    public async Task<long> GetBodyCartographicSurfaceScanValueSum(bool tripOnly = false)
    {
        var sql = "SELECT ifnull(sum(Bodies.CartographicSurfaceScanValue),0) FROM Bodies LEFT JOIN StarSystems ON Bodies.StarSystemId = StarSystems.Id64 WHERE Bodies.SurfaceScanned = 1" + (tripOnly ? " AND StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<long?>(sql) ?? 0L;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get body cartographic surface scan value sum", exception);
        }
        return 0L;
    }

    /// <summary>
    /// Gets the count of surface scanned bodies.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the count to the current trip.</param>
    /// <returns>The surface scan count.</returns>
    public async Task<int> GetBodySurfaceScanCount(bool tripOnly = false)
    {
        var sql = "SELECT count(Bodies.Id64) FROM Bodies LEFT JOIN StarSystems ON Bodies.StarSystemId = StarSystems.Id64 WHERE Bodies.SurfaceScanned = 1" + (tripOnly ? " AND StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<int?>(sql) ?? 0;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get body surface scan count", exception);
        }
        return 0;
    }

    /// <summary>
    /// Gets the count of bodies with touchdown.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the count to the current trip.</param>
    /// <returns>The touchdown count.</returns>
    public async Task<int> GetBodyTouchdownCount(bool tripOnly = false)
    {
        var sql = "SELECT count(Bodies.Id64) FROM Bodies LEFT JOIN StarSystems ON Bodies.StarSystemId = StarSystems.Id64 WHERE Bodies.Touchdown = 1" + (tripOnly ? " AND StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<int?>(sql) ?? 0;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get body touchdown count", exception);
        }
        return 0;
    }

    /// <summary>
    /// Gets the sum of cartographic values for all bodies.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the sum to the current trip.</param>
    /// <returns>The total cartographic value.</returns>
    public async Task<long> GetBodyCartographicValueSum(bool tripOnly = false)
    {
        var sql = "SELECT ifnull(sum(Bodies.CartographicValue),0) FROM Bodies LEFT JOIN StarSystems ON Bodies.StarSystemId = StarSystems.Id64" + (tripOnly ? " WHERE StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<long?>(sql) ?? 0L;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get bodies cartographic value sum", exception);
        }
        return 0L;
    }

    /// <summary>
    /// Gets the count of distinct bodies that have rings.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the count to the current trip.</param>
    /// <returns>The ring body count.</returns>
    public async Task<int> GetBodyRingBodyCount(bool tripOnly = false)
    {
        var sql = "SELECT count(*) FROM (SELECT DISTINCT Rings.StarSystemId, Rings.BodyId FROM Rings LEFT JOIN StarSystems ON Rings.StarSystemId = StarSystems.Id64" + (tripOnly ? " WHERE StarSystems.IsTripHistory = 1" : string.Empty) + ")";
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<int?>(sql) ?? 0;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get ring body count", exception);
        }
        return 0;
    }

    /// <summary>
    /// Gets the sum of biological counts for all bodies.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the sum to the current trip.</param>
    /// <returns>The genus signal sum.</returns>
    public async Task<int> GetGenusSignalSum(bool tripOnly = false)
    {
        var sql = "SELECT ifnull(sum(Bodies.BiologicalCount),0) FROM Bodies LEFT JOIN StarSystems ON Bodies.StarSystemId = StarSystems.Id64" + (tripOnly ? " WHERE StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<int?>(sql) ?? 0;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get genus signal sum", exception);
        }
        return 0;
    }

    /// <summary>
    /// Gets the count of all genera.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the count to the current trip.</param>
    /// <returns>The genus count.</returns>
    public async Task<int> GetGenusCount(bool tripOnly = false)
    {
        var sql = "SELECT count(Genera.Name) FROM Genera LEFT JOIN StarSystems ON Genera.StarSystemId = StarSystems.Id64" + (tripOnly ? " WHERE StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<int?>(sql) ?? 0;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get genus count", exception);
        }
        return 0;
    }

    /// <summary>
    /// Gets the count of fully analysed genera.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the count to the current trip.</param>
    /// <returns>The analysed genus count.</returns>
    public async Task<int> GetGenusAnalysisCompleteCount(bool tripOnly = false)
    {
        var sql = "SELECT count(Genera.Name) FROM Genera LEFT JOIN StarSystems ON Genera.StarSystemId = StarSystems.Id64 WHERE Genera.IsAnalysed = 1" + (tripOnly ? " AND StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<int?>(sql) ?? 0;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get genus analysis complete count", exception);
        }
        return 0;
    }

    /// <summary>
    /// Gets the count of first discovered genera.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the count to the current trip.</param>
    /// <returns>The first discovery genus count.</returns>
    public async Task<int> GetGenusFirstDiscoveryCount(bool tripOnly = false)
    {
        var sql = "SELECT count(Genera.Name) FROM Genera LEFT JOIN StarSystems ON Genera.StarSystemId = StarSystems.Id64 WHERE Genera.IsFirstDiscovered = 1" + (tripOnly ? " AND StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<int?>(sql) ?? 0;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get genus first discovery count", exception);
        }
        return 0;
    }

    /// <summary>
    /// Gets the sum of Vista Genomics values for all analysed genera.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the sum to the current trip.</param>
    /// <returns>The total Vista Genomics value.</returns>
    public async Task<long> GetGenusVistaGenomicsValueSum(bool tripOnly = false)
    {
        var sql = "SELECT ifnull(sum(Genera.VistaGenomicsValue),0) FROM Genera LEFT JOIN StarSystems ON Genera.StarSystemId = StarSystems.Id64 WHERE Genera.IsAnalysed = 1" + (tripOnly ? " AND StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return (long)(await db.QueryFirstOrDefaultAsync<double>(sql));
        }
        catch (Exception exception)
        {
            log.Error("Cannot get genus vista genomics value sum", exception);
        }
        return 0L;
    }

    /// <summary>
    /// Gets the sum of Vista Genomics base values for all analysed genera.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the sum to the current trip.</param>
    /// <returns>The total Vista Genomics base value.</returns>
    public async Task<long> GetGenusVistaGenomicsBaseValueSum(bool tripOnly = false)
    {
        var sql = "SELECT ifnull(sum(Genera.VistaGenomicsBaseValue),0) FROM Genera LEFT JOIN StarSystems ON Genera.StarSystemId = StarSystems.Id64 WHERE Genera.IsAnalysed = 1" + (tripOnly ? " AND StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<long?>(sql) ?? 0L;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get genus vista genomics base value sum", exception);
        }
        return 0L;
    }

    /// <summary>
    /// Gets the most and least frequent genus statistics.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the statistics to the current trip.</param>
    /// <returns>The genus statistics.</returns>
    public async Task<HistoryStatistics> GetGenusStatistics(bool tripOnly = false)
    {
        var sql = "SELECT Genera.Name, count(Genera.Name) AS Amount FROM Genera LEFT JOIN StarSystems ON Genera.StarSystemId = StarSystems.Id64 WHERE Genera.IsAnalysed = 1" + (tripOnly ? " AND StarSystems.IsTripHistory = 1" : string.Empty) + " GROUP BY Genera.Name ORDER BY Amount DESC";
        var bioStats = new HistoryStatistics();
        using var db = Connect();
        db.Open();
        try
        {
            var rows = (await db.QueryAsync(sql)).ToList();
            if (rows.Count > 0)
            {
                bioStats.MostFrequentTitle = Convert.ToString(rows.First().Name);
                bioStats.MostFrequentCount = Convert.ToInt32(rows.First().Amount);
                bioStats.RarestTitle = Convert.ToString(rows.Last().Name);
                bioStats.RarestCount = Convert.ToInt32(rows.Last().Amount);
            }
        }
        catch (Exception exception)
        {
            log.Error("Cannot get genus statistics", exception);
        }
        return bioStats;
    }

    /// <summary>
    /// Gets the count of rings.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the count to the current trip.</param>
    /// <returns>The ring count.</returns>
    public async Task<int> GetRingCount(bool tripOnly = false)
    {
        var sql = "SELECT count(Rings.Name) FROM Rings LEFT JOIN StarSystems ON Rings.StarSystemId = StarSystems.Id64" + (tripOnly ? " WHERE StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<int?>(sql) ?? 0;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get ring count", exception);
        }
        return 0;
    }

    /// <summary>
    /// Gets the count of first discovery rings.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the count to the current trip.</param>
    /// <returns>The first discovery ring count.</returns>
    public async Task<int> GetRingFirstDiscoveryCount(bool tripOnly = false)
    {
        var sql = "SELECT count(Rings.Name) FROM Rings LEFT JOIN Bodies ON Rings.BodyId = Bodies.Id AND Rings.StarSystemId = Bodies.StarSystemId LEFT JOIN StarSystems ON Rings.StarSystemId = StarSystems.Id64 WHERE Bodies.WasDiscovered = 0" + (tripOnly ? " AND StarSystems.IsTripHistory = 1" : string.Empty);
        using var db = Connect();
        db.Open();
        try
        {
            return await db.QueryFirstOrDefaultAsync<int?>(sql) ?? 0;
        }
        catch (Exception exception)
        {
            log.Error("Cannot get ring first discovery count", exception);
        }
        return 0;
    }

    /// <summary>
    /// Gets the most and least frequent ring type statistics.
    /// </summary>
    /// <param name="tripOnly">Whether to limit the statistics to the current trip.</param>
    /// <returns>The ring statistics.</returns>
    public async Task<HistoryStatistics> GetRingStatistics(bool tripOnly = false)
    {
        var sql = "SELECT Rings.Type, count(Rings.Type) as Amount FROM Rings LEFT JOIN StarSystems ON Rings.StarSystemId = StarSystems.Id64" + (tripOnly ? " WHERE StarSystems.IsTripHistory = 1" : string.Empty) + " GROUP BY Rings.Type Order by Amount DESC";
        var ringStats = new HistoryStatistics();
        using var db = Connect();
        db.Open();
        try
        {
            var rows = (await db.QueryAsync(sql)).ToList();
            if (rows.Count > 0)
            {
                ringStats.MostFrequentTitle = Enum.GetName(typeof(RingType), Convert.ToInt32(rows.First().Type));
                ringStats.MostFrequentCount = Convert.ToInt32(rows.First().Amount);
                ringStats.RarestTitle = Enum.GetName(typeof(RingType), Convert.ToInt32(rows.Last().Type));
                ringStats.RarestCount = Convert.ToInt32(rows.Last().Amount);
            }
        }
        catch (Exception exception)
        {
            log.Error("Cannot get ring statistics", exception);
        }
        return ringStats;
    }

    #endregion
}
