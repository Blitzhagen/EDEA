using System.Text.Json.Nodes;
using EDEA.Models;
using EDEA.Models.Biology;
using EDEA.Services;

namespace EDEA.Tests;

/// <summary>
/// Tests for the BioScan-derived biology rule evaluator and catalog provider.
/// </summary>
public class BiologyRuleEvaluatorTests
{
    /// <summary>
    /// Builds a test star system at the Sol coordinates with a G main star.
    /// </summary>
    private static StarSystem BuildSystem(string name = "Sol", string starType = "G", string luminosity = "V")
    {
        var system = new StarSystem(10477373803, name)
        {
            StarPositionX = -8.5,
            StarPositionY = -22.2,
            StarPositionZ = -69.8,
            StarClass = starType
        };
        var star = new Star(0, 10477373803, name, 0.0, starType, 695500000.0, 1.0, null)
        {
            Luminosity = luminosity,
            StarSystem = system
        };
        system.TryAddOrUpdateBody(star, ignoreSpeechOutput: true, DataSource.Journal, out _);
        return system;
    }

    /// <summary>
    /// Builds a test planet matching the Aleoida Arcus ruleset by default.
    /// </summary>
    private static Planet BuildPlanet(StarSystem system, string planetClass = "Rocky body", string atmosphereType = "CarbonDioxide", double gravity = 0.2, double temperature = 178.0, double pressureAtm = 0.02, string volcanism = "", int bodyId = 5)
    {
        var planet = new Planet(bodyId, system.Id, system.Name + " " + bodyId, 1000.0, planetClass, true, string.Empty, gravity, temperature, volcanism, "thin carbon dioxide atmosphere", 2000000.0, 0, null, 0.1, null)
        {
            AtmosphereType = atmosphereType,
            SurfacePressure = pressureAtm,
            OrbitalPeriod = 86400.0 * 10,
            BiologicalCount = 1,
            StarSystem = system
        };
        system.TryAddOrUpdateBody(planet, ignoreSpeechOutput: true, DataSource.Journal, out _);
        return planet;
    }

    private static BioEvaluationContext BuildContext(Planet planet)
    {
        return BiologyCatalogProvider.BuildContext(planet);
    }

    private static JsonObject Ruleset(string json)
    {
        return (JsonObject)JsonNode.Parse(json)!;
    }

    /// <summary>
    /// Sol is inside the Inner Orion Spur region (id 18).
    /// </summary>
    [Fact]
    public void FindRegion_Sol_IsInnerOrionSpur()
    {
        var (id, name) = GalacticRegionProvider.FindRegion(-8.5, -22.2, -69.8);
        Assert.Equal(18, id);
        Assert.Equal("Inner Orion Spur", name);
    }

    /// <summary>
    /// Coordinates far outside the map return no region.
    /// </summary>
    [Fact]
    public void FindRegion_OutsideMap_ReturnsNull()
    {
        var (id, _) = GalacticRegionProvider.FindRegion(-200000.0, 0, -200000.0);
        Assert.Null(id);
    }

    /// <summary>
    /// Gravity boundary checks: below min or above max eliminates the ruleset.
    /// </summary>
    [Fact]
    public void Matches_GravityBoundaries()
    {
        StarSystem system = BuildSystem();
        Planet planet = BuildPlanet(system, gravity: 0.2);
        BioEvaluationContext ctx = BuildContext(planet);
        var regionMap = BiologyCatalogProvider.Catalog.RegionMap;

        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"min_gravity": 0.2, "max_gravity": 0.2}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"min_gravity": 0.21}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"max_gravity": 0.19}"""), ctx, regionMap));
    }

    /// <summary>
    /// Temperature boundaries: matching inside the range, eliminated outside.
    /// </summary>
    [Fact]
    public void Matches_TemperatureBoundaries()
    {
        StarSystem system = BuildSystem();
        Planet planet = BuildPlanet(system, temperature: 178.0);
        BioEvaluationContext ctx = BuildContext(planet);
        var regionMap = BiologyCatalogProvider.Catalog.RegionMap;

        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"min_temperature": 175.0, "max_temperature": 180.0}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"min_temperature": 179.0}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"max_temperature": 177.0}"""), ctx, regionMap));
        // Missing temperature must not eliminate
        planet.SurfaceTemperature = 0.0;
        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"min_temperature": 175.0, "max_temperature": 180.0}"""), ctx, regionMap));
    }

    /// <summary>
    /// Pressure boundaries in atmospheres (journal pascal values are converted on ingest).
    /// </summary>
    [Fact]
    public void Matches_PressureBoundaries()
    {
        StarSystem system = BuildSystem();
        Planet planet = BuildPlanet(system, pressureAtm: 0.02);
        BioEvaluationContext ctx = BuildContext(planet);
        var regionMap = BiologyCatalogProvider.Catalog.RegionMap;

        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"min_pressure": 0.0161, "max_pressure": 0.021}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"min_pressure": 0.021}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"max_pressure": 0.02}"""), ctx, regionMap));
        // Missing pressure must not eliminate
        planet.SurfacePressure = 0.0;
        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"min_pressure": 0.0161}"""), ctx, regionMap));
    }

    /// <summary>
    /// Atmosphere rule checks the AtmosphereType identifier.
    /// </summary>
    [Fact]
    public void Matches_Atmosphere()
    {
        StarSystem system = BuildSystem();
        Planet planet = BuildPlanet(system, atmosphereType: "CarbonDioxide");
        BioEvaluationContext ctx = BuildContext(planet);
        var regionMap = BiologyCatalogProvider.Catalog.RegionMap;

        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"atmosphere": ["CarbonDioxide"]}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"atmosphere": ["Ammonia"]}"""), ctx, regionMap));
        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"atmosphere": "Any"}"""), ctx, regionMap));

        planet.AtmosphereType = "";
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"atmosphere": "Any"}"""), ctx, regionMap));
    }

    /// <summary>
    /// Atmosphere component rule checks the composition percentages.
    /// </summary>
    [Fact]
    public void Matches_AtmosphereComponent()
    {
        StarSystem system = BuildSystem();
        Planet planet = BuildPlanet(system);
        planet.AtmosphereComposition = new Dictionary<string, double> { ["CarbonDioxide"] = 99.9 };
        BioEvaluationContext ctx = BuildContext(planet);
        var regionMap = BiologyCatalogProvider.Catalog.RegionMap;

        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"atmosphere_component": {"CarbonDioxide": 90.0}}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"atmosphere_component": {"CarbonDioxide": 100.0}}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"atmosphere_component": {"Oxygen": 1.0}}"""), ctx, regionMap));
    }

    /// <summary>
    /// Volcanism rules: substring match, exact '=' match, 'Any', 'None' and '!' negation.
    /// </summary>
    [Fact]
    public void Matches_Volcanism()
    {
        StarSystem system = BuildSystem();
        Planet planet = BuildPlanet(system, volcanism: "Major Silicate Vapour Geysers Volcanism");
        BioEvaluationContext ctx = BuildContext(planet);
        var regionMap = BiologyCatalogProvider.Catalog.RegionMap;

        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"volcanism": ["major silicate vapour"]}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"volcanism": ["metallic magma"]}"""), ctx, regionMap));
        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"volcanism": ["=major silicate vapour geysers volcanism"]}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"volcanism": ["=silicate vapour"]}"""), ctx, regionMap));
        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"volcanism": "Any"}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"volcanism": "None"}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"volcanism": "!silicate"}"""), ctx, regionMap));

        planet.Volcanism = "";
        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"volcanism": "None"}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"volcanism": "Any"}"""), ctx, regionMap));
        // '!' negation requires some volcanism present
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"volcanism": "!silicate"}"""), ctx, regionMap));
    }

    /// <summary>
    /// Body type and system-level bodies rules.
    /// </summary>
    [Fact]
    public void Matches_BodyTypeAndBodies()
    {
        StarSystem system = BuildSystem();
        Planet planet = BuildPlanet(system, planetClass: "Rocky body");
        BioEvaluationContext ctx = BuildContext(planet);
        var regionMap = BiologyCatalogProvider.Catalog.RegionMap;

        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"body_type": ["Rocky body", "High metal content body"]}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"body_type": ["Icy body"]}"""), ctx, regionMap));
        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"bodies": ["Rocky body"]}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"bodies": ["Water world"]}"""), ctx, regionMap));
    }

    /// <summary>
    /// Star checks: star_check semantics including super giants and main/parent/any-star rules.
    /// </summary>
    [Fact]
    public void Matches_StarRules()
    {
        StarSystem system = BuildSystem(starType: "G", luminosity: "V");
        Planet planet = BuildPlanet(system);
        BioEvaluationContext ctx = BuildContext(planet);
        var regionMap = BiologyCatalogProvider.Catalog.RegionMap;

        Assert.True(BiologyRuleEvaluator.StarCheck("G", "G_WhiteSuperGiant"));
        Assert.True(BiologyRuleEvaluator.StarCheck("M", "M_RedSuperGiant"));
        Assert.False(BiologyRuleEvaluator.StarCheck("K", "G"));

        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"main_star": "G"}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"main_star": "K"}"""), ctx, regionMap));
        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"main_star": [["G", "V"]]}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"main_star": [["G", "III"]]}"""), ctx, regionMap));
        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"star": ["K", "G"]}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"star": ["K"]}"""), ctx, regionMap));
        // parent_star falls back to the main star when no other stars are known
        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"parent_star": ["G"]}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"parent_star": ["K"]}"""), ctx, regionMap));
    }

    /// <summary>
    /// Region rules: Sol is in the Inner Orion Spur; '!' negation eliminates.
    /// </summary>
    [Fact]
    public void Matches_Regions()
    {
        StarSystem system = BuildSystem();
        Planet planet = BuildPlanet(system);
        BioEvaluationContext ctx = BuildContext(planet);
        var regionMap = BiologyCatalogProvider.Catalog.RegionMap;
        Assert.Equal(18, ctx.Region);

        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"regions": ["orion-cygnus"]}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"regions": ["!orion-cygnus"]}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"regions": ["amphora"]}"""), ctx, regionMap));
        // Unknown region (no coordinates and no stored region) disables the check entirely
        system.StarPositionX = null;
        system.Region = null;
        ctx = BuildContext(planet);
        Assert.Null(ctx.Region);
        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"regions": ["amphora"]}"""), ctx, regionMap));
    }

    /// <summary>
    /// Distance and orbital period rules.
    /// </summary>
    [Fact]
    public void Matches_DistanceAndOrbitalPeriod()
    {
        StarSystem system = BuildSystem();
        Planet planet = BuildPlanet(system);
        planet.Distance = 1000.0;
        planet.OrbitalPeriod = 86400.0 * 10;
        BioEvaluationContext ctx = BuildContext(planet);
        var regionMap = BiologyCatalogProvider.Catalog.RegionMap;

        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"distance": 999.0}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"distance": 1001.0}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"max_orbital_period": 864000.0}"""), ctx, regionMap));
        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"max_orbital_period": 8640000.0}"""), ctx, regionMap));
        planet.OrbitalPeriod = null;
        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"max_orbital_period": 864000.0}"""), ctx, regionMap));
    }

    /// <summary>
    /// The system rule restricts species to a named system.
    /// </summary>
    [Fact]
    public void Matches_System()
    {
        StarSystem system = BuildSystem(name: "HIP 87621");
        var star = new Star(0, system.Id, "HIP 87621", 0.0, "K", 695500000.0, 1.0, null) { StarSystem = system };
        system.TryAddOrUpdateBody(star, ignoreSpeechOutput: true, DataSource.Journal, out _);
        Planet planet = BuildPlanet(system);
        BioEvaluationContext ctx = BuildContext(planet);
        var regionMap = BiologyCatalogProvider.Catalog.RegionMap;

        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"system": "HIP 87621"}"""), ctx, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"system": "Sol"}"""), ctx, regionMap));
    }

    /// <summary>
    /// End-to-end: a carbon dioxide rocky world around a K star predicts Aleoida species via the real catalog.
    /// (Aleoida has no color variant for G stars, so a K star is required.)
    /// </summary>
    [Fact]
    public void Predict_Aleoida_OnCarbonDioxideRockyWorld()
    {
        StarSystem system = BuildSystem(starType: "K");
        Planet planet = BuildPlanet(system);
        planet.AtmosphereComposition = new Dictionary<string, double> { ["CarbonDioxide"] = 100.0 };

        BiologyCatalogProvider.PredictOccurrenceOfSpecies(planet);

        Assert.NotEmpty(planet.PredictedSpecies);
        string dump = string.Join("; ", planet.PredictedSpecies.Select(p => p.Name + "/" + p.Species));
        Assert.True(planet.PredictedSpecies.Any(p => p.Name == "Aleoida"), "Predicted: " + dump);
        Assert.True(planet.PredictedSpecies.Any(p => p.Species.StartsWith("Aleoida")), "Predicted: " + dump);
        var aleoida = planet.PredictedSpecies.First(p => p.Name == "Aleoida");
        Assert.Equal(150, aleoida.ClonalColonyRange);
        Assert.True(aleoida.VistaGenomicsBaseValue > 0);
    }

    /// <summary>
    /// No biological signals means no prediction at all.
    /// </summary>
    [Fact]
    public void Predict_NoBioSignals_NoPredictions()
    {
        StarSystem system = BuildSystem();
        Planet planet = BuildPlanet(system);
        planet.BiologicalCount = 0;

        BiologyCatalogProvider.PredictOccurrenceOfSpecies(planet);

        Assert.Empty(planet.PredictedSpecies);
    }

    /// <summary>
    /// Lookup helpers for values and clonal colony ranges by localized names.
    /// </summary>
    [Fact]
    public void Catalog_Lookups()
    {
        Assert.True(BiologyCatalogProvider.GetVistaGenomicsValueForSpecies("Bacterium Aurasus") > 0);
        Assert.Equal(0, BiologyCatalogProvider.GetVistaGenomicsValueForSpecies("Nonexistent Species"));
        Assert.Equal(150, BiologyCatalogProvider.GetClonalColonyRangeForGenus("Aleoida"));
        Assert.Equal(0, BiologyCatalogProvider.GetClonalColonyRangeForGenus("Nonexistent"));
    }

    /// <summary>
    /// Real-world check: the nitrogen world "Blu Ain VY-P c19-603 A 5" (0.517 g, 127.8 K,
    /// 0.0121 atm, polonium present) predicts Bacterium Informem via its element colors.
    /// </summary>
    [Fact]
    public void Predict_BacteriumInformem_NitrogenWorldWithPolonium()
    {
        StarSystem system = BuildSystem(starType: "K");
        var planet = new Planet(10, system.Id, system.Name + " A 5", 1126.0, "High metal content body", true, string.Empty, 0.5172, 127.79, string.Empty, "thin nitrogen atmosphere", 2000000.0, 0, null, 1.0, null)
        {
            AtmosphereType = "Nitrogen",
            SurfacePressure = 0.0121,
            OrbitalPeriod = 130980288.98,
            Materials = new HashSet<string> { "iron", "nickel", "sulphur", "carbon", "chromium", "phosphorus", "germanium", "selenium", "niobium", "molybdenum", "polonium" },
            BiologicalCount = 1,
            StarSystem = system
        };
        system.TryAddOrUpdateBody(planet, ignoreSpeechOutput: true, DataSource.Journal, out _);

        BiologyCatalogProvider.PredictOccurrenceOfSpecies(planet);

        string dump = string.Join("; ", planet.PredictedSpecies.Select(p => p.Species));
        Assert.Contains(planet.PredictedSpecies, p => p.Species == "Bacterium Informem" && p.Variant == "Lime");
        Assert.True(planet.PredictedSpecies.Any(p => p.Species == "Bacterium Informem"), "Predicted: " + dump);

        // Without materials the element-colored species must be eliminated (BioScan semantics)
        planet.Materials.Clear();
        planet.PredictedSpecies.Clear();
        BiologyCatalogProvider.PredictOccurrenceOfSpecies(planet);
        Assert.DoesNotContain(planet.PredictedSpecies, p => p.Species == "Bacterium Informem");
    }

    /// <summary>
    /// BioScan derives parent stars only from leading capital letters of the body name.
    /// A moon of a number-named sub star ("3 a") falls back to the system main star,
    /// so Stratum species (no "G" color) must be eliminated even though the real parent
    /// star is a Y dwarf.
    /// </summary>
    [Fact]
    public void PredictOccurrence_MoonOfNumberedSubStar_UsesMainStarForColors()
    {
        var system = new StarSystem(49660500676435L, "Test Sys")
        {
            StarPositionX = 933.375,
            StarPositionY = 178.71875,
            StarPositionZ = 15156.1875,
            StarClass = "G"
        };
        var mainStar = new Star(0, 49660500676435L, "Test Sys", 0.0, "G", 695500000.0, 1.0, null)
        {
            Luminosity = "Vab",
            StarSystem = system
        };
        system.TryAddOrUpdateBody(mainStar, ignoreSpeechOutput: true, DataSource.Journal, out _);
        var subStar = new Star(20, 49660500676435L, "Test Sys 3", 2000.0, "Y", 400000000.0, 0.05, null)
        {
            Luminosity = "V",
            StarSystem = system
        };
        system.TryAddOrUpdateBody(subStar, ignoreSpeechOutput: true, DataSource.Journal, out _);
        var planet = new Planet(21, 49660500676435L, "Test Sys 3 a", 2100.0, "Rocky body", true, string.Empty, 0.16, 171.0, string.Empty, "thin carbon dioxide atmosphere", 2000000.0, 20, null, 0.1, null)
        {
            AtmosphereType = "CarbonDioxide",
            SurfacePressure = 0.05,
            OrbitalPeriod = 685383.0,
            Materials = new HashSet<string> { "iron", "sulphur", "carbon", "antimony" },
            BiologicalCount = 3,
            StarSystem = system
        };
        system.TryAddOrUpdateBody(planet, ignoreSpeechOutput: true, DataSource.Journal, out _);

        // The name "3 a" has no leading letter: the main star is the color source.
        List<Star> parents = system.GetParentStars(planet);
        Assert.Single(parents);
        Assert.Same(mainStar, parents[0]);

        BiologyCatalogProvider.PredictOccurrenceOfSpecies(planet);
        Assert.DoesNotContain(planet.PredictedSpecies, match => match.Name == "Stratum");
        Assert.Contains(planet.PredictedSpecies, match => match.Species == "Bacterium Aurasus");
    }

    /// <summary>
    /// A prediction computed from incomplete data must be invalidated when a later
    /// journal merge adds biology-relevant data (e.g. materials), so that the next
    /// prediction trigger recomputes the full candidate list.
    /// </summary>
    [Fact]
    public void UpdatePlanet_NewBiologyData_InvalidatesPrediction()
    {
        StarSystem system = BuildSystem();
        Planet planet = BuildPlanet(system);
        BiologyCatalogProvider.PredictOccurrenceOfSpecies(planet);
        Assert.NotEmpty(planet.PredictedSpecies);
        planet.InitialPredictionOfSpecies = true;

        var rescan = new Planet(planet.Id, system.Id, planet.Name, 1000.0, "Rocky body", true, string.Empty, 0.2, 178.0, string.Empty, "thin carbon dioxide atmosphere", 2000000.0, 0, null, 0.1, null)
        {
            AtmosphereType = "CarbonDioxide",
            SurfacePressure = 0.02,
            OrbitalPeriod = 86400.0 * 10,
            Materials = new HashSet<string> { "sulphur", "antimony" }
        };
        planet.UpdatePlanet(rescan, DataSource.Journal);
        Assert.Empty(planet.PredictedSpecies);
        Assert.False(planet.InitialPredictionOfSpecies);
    }

    /// <summary>
    /// A merge that adds no biology-relevant data must keep the cached prediction.
    /// </summary>
    [Fact]
    public void UpdatePlanet_SameBiologyData_KeepsPrediction()
    {
        StarSystem system = BuildSystem();
        Planet planet = BuildPlanet(system);
        planet.Materials = new HashSet<string> { "sulphur" };
        BiologyCatalogProvider.PredictOccurrenceOfSpecies(planet);
        int count = planet.PredictedSpecies.Count;
        planet.InitialPredictionOfSpecies = true;

        var rescan = new Planet(planet.Id, system.Id, planet.Name, 1000.0, "Rocky body", true, string.Empty, 0.2, 178.0, string.Empty, "thin carbon dioxide atmosphere", 2000000.0, 0, null, 0.1, null)
        {
            AtmosphereType = "CarbonDioxide",
            SurfacePressure = 0.02,
            OrbitalPeriod = 86400.0 * 10,
            Materials = new HashSet<string> { "sulphur" }
        };
        planet.UpdatePlanet(rescan, DataSource.Journal);
        Assert.Equal(count, planet.PredictedSpecies.Count);
        Assert.True(planet.InitialPredictionOfSpecies);
    }

    /// <summary>
    /// Round-trip: a planet with biology fields must persist and reload MaterialsCsv,
    /// AtmosphereCompositionCsv, SurfacePressure and OrbitalPeriod.
    /// </summary>
    [Fact]
    public async Task SQLiteStore_PersistsBiologyFields()
    {
        string dbPath = Path.Combine(Path.GetTempPath(), "edea_test_" + Guid.NewGuid().ToString("N") + ".sqlite");
        try
        {
            var store = EDEA.Stores.SQLiteStore.Instance(dbPath);
            var system = new StarSystem(12345L, "Test Sys") { StarClass = "K" };
            var planet = new Planet(10, 12345L, "Test Sys A 5", 100.0, "High metal content body", true, string.Empty, 0.5, 127.0, string.Empty, "thin nitrogen atmosphere", 1.0, 1, null, 1.0, null)
            {
                AtmosphereType = "Nitrogen",
                AtmosphereComposition = new Dictionary<string, double> { ["Nitrogen"] = 100.0 },
                SurfacePressure = 0.012,
                Materials = new HashSet<string> { "iron", "polonium" },
                OrbitalPeriod = 130980288.0,
                BiologicalCount = 1,
                StarSystem = system
            };
            system.TryAddOrUpdateBody(planet, ignoreSpeechOutput: true, DataSource.Journal, out _);
            await store.InsertOrUpdateStarSystemAsync(system, false);

            StarSystem? loaded = store.ReadStarSystem(12345L);
            Planet? loadedPlanet = loaded?.Bodies.Values.OfType<Planet>().FirstOrDefault(p => p.Id == 10);
            Assert.NotNull(loadedPlanet);
            Assert.Equal("Nitrogen", loadedPlanet!.AtmosphereType);
            Assert.Contains("polonium", loadedPlanet.Materials);
            Assert.Contains("iron", loadedPlanet.Materials);
            Assert.Equal(100.0, loadedPlanet.AtmosphereComposition["Nitrogen"]);
            Assert.Equal(0.012, loadedPlanet.SurfacePressure, 3);
            Assert.Equal(130980288.0, loadedPlanet.OrbitalPeriod);
        }
        finally
        {
            // keep db for inspection: " + dbPath
        }
    }

    /// <summary>
    /// Journal Scan materials and atmosphere composition must be parsed so element-color
    /// species like Bacterium Informem can be resolved.
    /// </summary>
    [Fact]
    public void ScanEvent_ReadMaterials_ParsesJournalArrays()
    {
        const string scanJson = """
            {"event":"Scan","BodyName":"X A 5","BodyID":10,"PlanetClass":"High metal content body",
             "Atmosphere":"thin nitrogen atmosphere","AtmosphereType":"Nitrogen",
             "AtmosphereComposition":[{"Name":"Nitrogen","Percent":100.0}],
             "SurfacePressure":1223.76062,
             "Materials":[{"Name":"iron","Percent":22.9},{"Name":"polonium","Percent":0.64}],
             "OrbitalPeriod":130980288.98,"SurfaceGravity":5.072,"SurfaceTemperature":127.79,"Volcanism":""}
            """;
        var jObject = System.Text.Json.Nodes.JsonNode.Parse(scanJson)!.AsObject();
        var providerType = typeof(EDEA.Services.JournalProvider);

        var readMaterials = providerType.GetMethod("ReadMaterials", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;
        var materials = (HashSet<string>)readMaterials.Invoke(null, new object[] { jObject })!;
        Assert.Contains("polonium", materials);
        Assert.Contains("iron", materials);

        var readAtmosphere = providerType.GetMethod("ReadAtmosphereComposition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;
        var composition = (Dictionary<string, double>)readAtmosphere.Invoke(null, new object[] { jObject })!;
        Assert.Equal(100.0, composition["Nitrogen"]);
    }

    /// <summary>
    /// Codex-key lookups are language independent: German localized names like "Hirnbaum"
    /// resolve through the codex key, and narrowing matches observed genera by key.
    /// </summary>
    [Fact]
    public void Catalog_KeyBasedLookups_AndGermanNarrowing()
    {
        // Codex-key lookups work regardless of localized names
        Assert.Equal(150, BiologyCatalogProvider.GetClonalColonyRangeForGenus("$Codex_Ent_Aleoids_Genus_Name;"));
        Assert.Equal(100, BiologyCatalogProvider.GetClonalColonyRangeForGenus("$Codex_Ent_Brancae_Name;"));
        Assert.True(BiologyCatalogProvider.GetVistaGenomicsValueForSpecies("$Codex_Ent_Aleoids_01_Name;") > 0);
        // Localized names still work
        Assert.Equal(150, BiologyCatalogProvider.GetClonalColonyRangeForGenus("Aleoida"));

        // A genus observed with a German localized name (e.g. "Hirnbaum" for Brain Tree)
        // still narrows predictions via its codex key.
        StarSystem system = BuildSystem(starType: "K");
        Planet planet = BuildPlanet(system);
        planet.AtmosphereComposition = new Dictionary<string, double> { ["CarbonDioxide"] = 100.0 };
        planet.BiologicalCount = 1;
        planet.TryAddOrUpdateGenus(new Genus("Aleoida-DE", system.Id, planet.Id, null, codexKey: "$Codex_Ent_Aleoids_Genus_Name;"));

        BiologyCatalogProvider.PredictOccurrenceOfSpecies(planet);

        Assert.NotEmpty(planet.PredictedSpecies);
        Assert.All(planet.PredictedSpecies, p => Assert.Equal("$Codex_Ent_Aleoids_Genus_Name;", p.GenusKey));
    }

    /// <summary>
    /// parse_variant splits codex names into genus, species and color.
    /// </summary>
    [Fact]
    public void ParseVariant_Works()
    {
        var (genus, species, color) = BiologyCatalogProvider.ParseVariant("$Codex_Ent_Aleoids_01_Name;");
        Assert.Equal("$Codex_Ent_Aleoids_Genus_Name;", genus);
        Assert.Equal("$Codex_Ent_Aleoids_01_Name;", species);
        Assert.Equal("", color);

        var (genus2, species2, color2) = BiologyCatalogProvider.ParseVariant("$Codex_Ent_Aleoids_01_K_Name;");
        Assert.Equal("$Codex_Ent_Aleoids_Genus_Name;", genus2);
        Assert.Equal("$Codex_Ent_Aleoids_01_Name;", species2);
        Assert.Equal("Turquoise", color2);
    }

    /// <summary>
    /// guardian: true requires the system to be inside a guardian nebula zone
    /// (Hen 2-333: 750 ly around -840.66/-561.16/13361.81).
    /// </summary>
    [Fact]
    public void Guardian_InsideAndOutsideZone()
    {
        var regionMap = BiologyCatalogProvider.Catalog.RegionMap;

        StarSystem inside = BuildSystem();
        inside.StarPositionX = -840.65625;
        inside.StarPositionY = -561.15625;
        inside.StarPositionZ = 13361.8125;
        BioEvaluationContext ctxInside = BuildContext(BuildPlanet(inside));

        StarSystem outside = BuildSystem();
        BioEvaluationContext ctxOutside = BuildContext(BuildPlanet(outside));

        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"guardian": true}"""), ctxInside, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"guardian": true}"""), ctxOutside, regionMap));
        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"guardian": false}"""), ctxOutside, regionMap));
    }

    /// <summary>
    /// tuber requires the system to be inside one of the named tuber zones
    /// (Arcadian Stream: 200-600 ly around 8885/-20/20535).
    /// </summary>
    [Fact]
    public void Tuber_InsideAndOutsideZone()
    {
        var regionMap = BiologyCatalogProvider.Catalog.RegionMap;

        StarSystem inside = BuildSystem();
        inside.StarPositionX = 8885 + 400.0;
        inside.StarPositionY = -20.0;
        inside.StarPositionZ = 20535.0;
        BioEvaluationContext ctxInside = BuildContext(BuildPlanet(inside));

        StarSystem outside = BuildSystem();
        BioEvaluationContext ctxOutside = BuildContext(BuildPlanet(outside));

        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"tuber": ["Arcadian Stream"]}"""), ctxInside, regionMap));
        Assert.True(BiologyRuleEvaluator.Matches(Ruleset("""{"tuber": "Any"}"""), ctxInside, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"tuber": ["No Such Zone"]}"""), ctxInside, regionMap));
        Assert.False(BiologyRuleEvaluator.Matches(Ruleset("""{"tuber": ["Arcadian Stream"]}"""), ctxOutside, regionMap));
    }
}
