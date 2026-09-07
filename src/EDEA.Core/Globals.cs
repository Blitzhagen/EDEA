using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using EDEA.Enums;
using EDEA.Models;
using EDEA.Properties;

namespace EDEA
{
    /// <summary>
    /// Global constants, dictionaries and localization helpers used throughout the application.
    /// </summary>
    public static class Globals
    {
        public static readonly Dictionary<string, string> PlotterStarClasses = new Dictionary<string, string>
    {
        { "Neutron", "Neutron Star" },
        { "Unknown", "" },
        { "Refuel", "Refuel Star" }
    };

        public const int RetriesOnFileLock = 10;
        public const int TimeOutOnFileLock = 500;
        public const int StandardMainFontSize = 12;
        public const int StandardHeaderFontSize = 14;
        public const int StandardSmallFontSize = 10;
        public const int StandardHistoryFontSize = 20;
        public const int StandardHistoryTotalValueFontSize = 30;
        public const int StandardMainIconSize = 15;
        public const int StandardMediumIconSize = 12;
        public const int StandardSmallIconSize = 7;
        public const int StandardTabViewBorderTopSize = 21;
        public const int CanonnBioStatsRelevanceThreshold = 3;
        public const int VistaGenomicsFirstDiscoveryBonusBaseValueMultiplier = 4;
        public const int MaxNumberOfEdsmRouteAheadRequests = 25;
        public const int MaxNumberOfSurroundingStarSystems = 25;
        public const string NavRouteFileName = "NavRoute.json";
        public const string JournalFileExtension = ".log";
        public const string JournalFileRegExPattern = "Journal\\..*\\.log";
        public const string StatusFileName = "Status.json";
        public const string Hyphen = "â€“";
        public static string StatusNoData => Resources.StatusNoData;
        public static string StarName => Resources.BodyTypeStar;
        public static string PlanetName => Resources.BodyTypePlanet;
        public static string JournalTerraformingStateTrueDescription => Resources.TerraformingState_Terraformable;
        public static string EDSMTerraformingStateTrueDescription => Resources.TerraformingState_CandidateForTerraforming;
        public static string CommanderNamePrefix => Resources.CommanderNamePrefix;
        public static string TooltipJournalIcon => Resources.TooltipJournalIcon;
        public static string TooltipEdsmOnly => Resources.TooltipEdsmOnly;
        public static string TooltipNewIcon => Resources.TooltipNewIcon;
        public static string TooltipValuableBodyIcon => Resources.TooltipValuableBodyIcon;
        public static string TooltipValuableGenusIcon => Resources.TooltipValuableGenusIcon;
        public static string TooltipValuableGenusPredictedIcon => Resources.TooltipValuableGenusPredictedIcon;
        public static string TooltipLoadingIcon => Resources.TooltipLoadingIcon;
        public static string TooltipPopulatedIcon => Resources.TooltipPopulatedIcon;
        public static string TooltipTerraformableIcon => Resources.TooltipTerraformableIcon;
        public static string TooltipLandableIcon => Resources.TooltipLandableIcon;
        public static string TooltipTouchdownIcon => Resources.TooltipTouchdownIcon;
        public static string TooltipLandableTouchdownCombiIcon => Resources.TooltipLandableTouchdownCombiIcon;
        public static string TooltipRingsIcon => Resources.TooltipRingsIcon;
        public static string TooltipGeologicalsIcon => Resources.TooltipGeologicalsIcon;
        public static string TooltipBiologicalsIcon => Resources.TooltipBiologicalsIcon;
        public static string TooltipScoopableIcon => Resources.TooltipScoopableIcon;
        public static string TooltipCurrentBodyIcon => Resources.TooltipCurrentBodyIcon;
        public static string TooltipCurrentSystemIcon => Resources.TooltipCurrentSystemIcon;
        public static string TooltipJumpDestinationSystemIcon => Resources.TooltipJumpDestinationSystemIcon;
        public static string TooltipSurfaceScannedStatusIcon => Resources.TooltipSurfaceScannedStatusIcon;
        public static string TooltipScannedAndWasMappedIcon => Resources.TooltipScannedAndWasMappedIcon;
        public static string TooltipScannedAndFirstMappedIcon => Resources.TooltipScannedAndFirstMappedIcon;
        public static string TooltipUnscannedAndWasMappedIcon => Resources.TooltipUnscannedAndWasMappedIcon;
        public static string TooltipUnknownSurfaceScanStatusIcon => Resources.TooltipUnknownSurfaceScanStatusIcon;
        public static string TooltipAnalysisCompleteIcon => Resources.TooltipAnalysisCompleteIcon;
        public static string TooltipPlanetOfInterestIcon => Resources.TooltipPlanetOfInterestIcon;
        public static string TooltipGenusIsFirstDiscovery => Resources.TooltipGenusIsFirstDiscovery;
        public static string TooltipDistanceToPreviousSystem => Resources.TooltipDistanceToPreviousSystem;
        public static string TooltipDistanceToCurrentSystem => Resources.TooltipDistanceToCurrentSystem;
        public const string EdsmValueCurrency = "Cr";
        public const string PlotterUrl = "https://www.spansh.co.uk/plotter";
        public static string MenuItemPlotterRouteGenerate => Resources.MenuItem_PlotterRouteGenerate;
        public static string MenuItemPlotterRouteClear => Resources.MenuItem_PlotterRouteClear;
        public static string MenuItemRouteLock => Resources.MenuItem_RouteLock;
        public static string MenuItemRouteUnlock => Resources.MenuItem_RouteUnlock;
        public static string MenuItemRouteImport => Resources.MenuItem_RouteImport;
        public static string MenuItemRouteExport => Resources.MenuItem_RouteExport;
        public static string MenuItemHudOpen => Resources.MenuItem_HudOpen;
        public static string MenuItemHudClose => Resources.MenuItem_HudClose;
        public static string MenuItemHudPassThroughEnable => Resources.MenuItem_HudPassThroughEnable;
        public static string MenuItemHudPassThroughDisable => Resources.MenuItem_HudPassThroughDisable;
        public const string FeedbackReportIssueMailAddress = "edea@edea.app";
        public static string FeedbackReportIssueMailSubject => Resources.FeedbackReportIssueMailSubject;
        public const string FeedbackReportIssueMailBody = "";
        public static string ColumnHeaderName => Resources.ColumnHeaderName;
        public static string ColumnHeaderType => Resources.ColumnHeaderType;
        public static string ColumnHeaderTemperature => Resources.ColumnHeaderTemperature;
        public static string ColumnHeaderAtmosphere => Resources.ColumnHeaderAtmosphere;
        public static string ColumnHeaderEdsmDiscoverer => Resources.ColumnHeaderEdsmDiscoverer;
        public static string ColumnHeaderCartographicValue => Resources.ColumnHeaderCartographicValue;
        public static string ColumnHeaderOverallProgress => Resources.ColumnHeaderOverallProgress;
        public static string ColumnHeaderDistance => Resources.ColumnHeaderDistance;
        public static string ColumnHeaderJump => Resources.ColumnHeaderJump;
        public static string ColumnHeaderSystemName => Resources.ColumnHeaderSystemName;
        public static string ColumnHeaderStarClass => Resources.ColumnHeaderStarClass;
        public static string ColumnHeaderDiscoveryStatus => Resources.ColumnHeaderDiscoveryStatus;
        public static string ColumnHeaderSpecies => Resources.ColumnHeaderSpecies;
        public static string ColumnHeaderVariant => Resources.ColumnHeaderVariant;
        public static string ColumnHeaderScans => Resources.ColumnHeaderScans;
        public static string ColumnHeaderClonalColonyRange => Resources.ColumnHeaderClonalColonyRange;
        public static string ColumnHeader1stScanDistance => Resources.ColumnHeader1stScanDistance;
        public static string ColumnHeader2ndScanDistance => Resources.ColumnHeader2ndScanDistance;
        public static string ColumnHeaderVistaGenomicsValue => Resources.ColumnHeaderVistaGenomicsValue;

        public static readonly string ApplicationFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        public static readonly string AppDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create),
            Assembly.GetExecutingAssembly().GetName().Name!);
        public static readonly string AppVersionString = $"{Assembly.GetExecutingAssembly().GetName().Version!.Major}.{Assembly.GetExecutingAssembly().GetName().Version!.Minor}.{Assembly.GetExecutingAssembly().GetName().Version!.Build}{(string.IsNullOrWhiteSpace(Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration) ? string.Empty : (" " + Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration))}";

        public static readonly ImmutableDictionary<string, string> EdsmToJournalPlanetClasses = new Dictionary<string, string>
    {
        { "Metal-rich body", "Metal rich body" },
        { "Class I gas giant", "Sudarsky class I gas giant" },
        { "Class II gas giant", "Sudarsky class II gas giant" },
        { "Class III gas giant", "Sudarsky class III gas giant" },
        { "Class IV gas giant", "Sudarsky class IV gas giant" },
        { "Gas giant with water-based life", "Gas giant with water based life" },
        { "Gas giant with ammonia-based life", "Gas giant with ammonia based life" },
        { "High metal content world", "High metal content body" },
        { "Ammonia world", "Ammonia world" },
        { "Earth-like world", "Earthlike body" },
        { "Water world", "Water world" },
        { "Water giant", "Water giant" },
        { "Rocky body", "Rocky body" },
        { "Icy body", "Icy body" },
        { "Rocky Ice world", "Rocky ice body" }
    }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<string, string> EdsmToJournalAtmospheres = new Dictionary<string, string>
    {
        { "No atmosphere", "" },
        { "Ammonia", "ammonia atmosphere" },
        { "Thin Ammonia", "thin ammonia atmosphere" },
        { "Thick Ammonia", "thick ammonia atmosphere" },
        { "Hot Ammonia", "hot ammonia atmosphere" },
        { "Hot thin Ammonia", "hot thin ammonia atmosphere" },
        { "Hot thick Ammonia", "hot thick ammonia atmosphere" },
        { "Ammonia-rich", "ammonia rich atmosphere" },
        { "Thin Ammonia-rich", "thin ammonia rich atmosphere" },
        { "Thick Ammonia-rich", "thick ammonia rich atmosphere" },
        { "Hot Ammonia-rich", "hot ammonia rich atmosphere" },
        { "Hot thin Ammonia-rich", "hot thin ammonia rich atmosphere" },
        { "Hot thick Ammonia-rich", "hot thick ammonia rich atmosphere" },
        { "Argon", "argon atmosphere" },
        { "Thin Argon", "thin argon atmosphere" },
        { "Thick Argon", "thick argon atmosphere" },
        { "Hot Argon", "hot argon atmosphere" },
        { "Hot thin Argon", "hot thin argon atmosphere" },
        { "Hot thick Argon", "hot thick argon atmosphere" },
        { "Argon-rich", "argon rich atmosphere" },
        { "Thin Argon-rich", "thin argon rich atmosphere" },
        { "Thick Argon-rich", "thick argon rich atmosphere" },
        { "Hot Argon-rich", "hot argon rich atmosphere" },
        { "Hot thin Argon-rich", "hot thin argon rich atmosphere" },
        { "Hot thick Argon-rich", "hot thick argon rich atmosphere" },
        { "Carbon dioxide", "carbon dioxide atmosphere" },
        { "Thin Carbon dioxide", "thin carbon dioxide atmosphere" },
        { "Thick Carbon dioxide", "thick carbon dioxide atmosphere" },
        { "Hot Carbon dioxide", "hot carbon dioxide atmosphere" },
        { "Hot thin Carbon dioxide", "hot thin carbon dioxide atmosphere" },
        { "Hot thick Carbon dioxide", "hot thick carbon dioxide atmosphere" },
        { "Carbon dioxide-rich", "carbon dioxide rich atmosphere" },
        { "Thin Carbon dioxide-rich", "thin carbon dioxide rich atmosphere" },
        { "Thick Carbon dioxide-rich", "thick carbon dioxide rich atmosphere" },
        { "Hot Carbon dioxide-rich", "hot carbon dioxide rich atmosphere" },
        { "Hot thin Carbon dioxide-rich", "hot thin carbon dioxide rich atmosphere" },
        { "Hot thick Carbon dioxide-rich", "hot thick carbon dioxide rich atmosphere" },
        { "Helium", "helium atmosphere" },
        { "Thin Helium", "thin helium atmosphere" },
        { "Thick Helium", "thick helium atmosphere" },
        { "Hot Helium", "hot helium atmosphere" },
        { "Hot thin Helium", "hot thin helium atmosphere" },
        { "Hot thick Helium", "hot thick helium atmosphere" },
        { "Methane", "methane atmosphere" },
        { "Thin Methane", "thin methane atmosphere" },
        { "Thick Methane", "thick methane atmosphere" },
        { "Hot Methane", "hot methane atmosphere" },
        { "Hot thin Methane", "hot thin methane atmosphere" },
        { "Hot thick Methane", "hot thick methane atmosphere" },
        { "Methane-rich", "methane rich atmosphere" },
        { "Thin Methane-rich", "thin methane rich atmosphere" },
        { "Thick Methane-rich", "thick methane rich atmosphere" },
        { "Hot Methane-rich", "hot methane rich atmosphere" },
        { "Hot thin Methane-rich", "hot thin methane rich atmosphere" },
        { "Hot thick Methane-rich", "hot thick methane rich atmosphere" },
        { "Neon", "neon atmosphere" },
        { "Thin Neon", "thin neon atmosphere" },
        { "Thick Neon", "thick neon atmosphere" },
        { "Hot Neon", "hot neon atmosphere" },
        { "Hot thin Neon", "hot thin neon atmosphere" },
        { "Hot thick Neon", "hot thick neon atmosphere" },
        { "Neon-rich", "neon rich atmosphere" },
        { "Thin Neon-rich", "thin neon rich atmosphere" },
        { "Thick Neon-rich", "thick neon rich atmosphere" },
        { "Hot Neon-rich", "hot neon rich atmosphere" },
        { "Hot thin Neon-rich", "hot thin neon rich atmosphere" },
        { "Hot thick Neon-rich", "hot thick neon rich atmosphere" },
        { "Nitrogen", "nitrogen atmosphere" },
        { "Thin Nitrogen", "thin nitrogen atmosphere" },
        { "Thick Nitrogen", "thick nitrogen atmosphere" },
        { "Hot Nitrogen", "hot nitrogen atmosphere" },
        { "Hot thin Nitrogen", "hot thin nitrogen atmosphere" },
        { "Hot thick Nitrogen", "hot thick nitrogen atmosphere" },
        { "Oxygen", "oxygen atmosphere" },
        { "Thin Oxygen", "thin oxygen atmosphere" },
        { "Thick Oxygen", "thick oxygen atmosphere" },
        { "Hot Oxygen", "hot oxygen atmosphere" },
        { "Hot thin Oxygen", "hot thin oxygen atmosphere" },
        { "Hot thick Oxygen", "hot thick oxygen atmosphere" },
        { "Silicate vapour", "silicate vapour atmosphere" },
        { "Thin Silicate vapour", "thin silicate vapour atmosphere" },
        { "Thick Silicate vapour", "thick silicate vapour atmosphere" },
        { "Hot Silicate vapour", "hot silicate vapour atmosphere" },
        { "Hot thin Silicate vapour", "hot thin silicate vapour atmosphere" },
        { "Hot thick Silicate vapour", "hot thick silicate vapour atmosphere" },
        { "Sulphur dioxide", "sulfur dioxide atmosphere" },
        { "Thin Sulphur dioxide", "thin sulfur dioxide atmosphere" },
        { "Thick Sulphur dioxide", "thick sulfur dioxide atmosphere" },
        { "Hot Sulphur dioxide", "hot sulfur dioxide atmosphere" },
        { "Hot thin Sulphur dioxide", "hot thin sulfur dioxide atmosphere" },
        { "Hot thick Sulphur dioxide", "hot thick sulfur dioxide atmosphere" },
        { "Water", "water atmosphere" },
        { "Thin Water", "thin water atmosphere" },
        { "Thick Water", "thick water atmosphere" },
        { "Hot Water", "hot water atmosphere" },
        { "Hot thin Water", "hot thin water atmosphere" },
        { "Hot thick Water", "hot thick water atmosphere" },
        { "Water-rich", "water rich atmosphere" },
        { "Thin Water-rich", "thin water rich atmosphere" },
        { "Thick Water-rich", "thick water rich atmosphere" },
        { "Hot Water-rich", "hot water rich atmosphere" },
        { "Hot thin Water-rich", "hot thin water rich atmosphere" },
        { "Hot thick Water-rich", "hot thick water rich atmosphere" }
    }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<string, string> EdsmToJournalVolcanisms = new Dictionary<string, string>
    {
        { "No volcanism", "" },
        { "Water Magma", "water magma volcanism" },
        { "Minor Water Magma", "minor water magma volcanism" },
        { "Major Water Magma", "major water magma volcanism" },
        { "Sulphur Dioxide Magma", "sulfur dioxide magma volcanism" },
        { "Minor Sulphur Dioxide magma", "minor sulfur dioxide magma volcanism" },
        { "Major Sulphur Dioxide magma", "major sulfur dioxide magma volcanism" },
        { "Ammonia Magma", "ammonia magma volcanism" },
        { "Minor Ammonia Magma", "minor ammonia magma volcanism" },
        { "Major Ammonia Magma", "major ammonia magma volcanism" },
        { "Methane Magma", "methane magma volcanism" },
        { "Minor Methane Magma", "minor methane magma volcanism" },
        { "Major Methane Magma", "major methane magma volcanism" },
        { "Nitrogen Magma", "nitrogen magma volcanism" },
        { "Minor Nitrogen Magma", "minor nitrogen magma volcanism" },
        { "Major Nitrogen Magma", "major nitrogen magma volcanism" },
        { "Silicate Magma", "silicate magma volcanism" },
        { "Minor Silicate Magma", "minor silicate magma volcanism" },
        { "Major Silicate Magma", "major silicate magma volcanism" },
        { "Metallic Magma", "metallic magma volcanism" },
        { "Minor Metallic Magma", "minor metallic magma volcanism" },
        { "Major Metallic Magma", "major metallic magma volcanism" },
        { "Rocky Magma", "rocky magma volcanism" },
        { "Minor Rocky Magma", "minor rocky magma volcanism" },
        { "Major Rocky Magma", "major rocky magma volcanism" },
        { "Water Geysers", "water geysers volcanism" },
        { "Minor Water Geysers", "minor water geysers volcanism" },
        { "Major Water Geysers", "major water geysers volcanism" },
        { "Carbon Dioxide Geysers", "carbon dioxide geysers volcanism" },
        { "Minor Carbon Dioxide Geysers", "minor carbon dioxide geysers volcanism" },
        { "Major Carbon Dioxide Geysers", "major carbon dioxide geysers volcanism" },
        { "Ammonia Geysers", "ammonia geysers volcanism" },
        { "Minor Ammonia Geysers", "minor ammonia geysers volcanism" },
        { "Major Ammonia Geysers", "major ammonia geysers volcanism" },
        { "Methane Geysers", "methane geysers volcanism" },
        { "Minor Methane Geysers", "minor methane geysers volcanism" },
        { "Major Methane Geysers", "major methane geysers volcanism" },
        { "Nitrogen Geysers", "nitrogen geysers volcanism" },
        { "Minor Nitrogen Geysers", "minor nitrogen geysers volcanism" },
        { "Major Nitrogen Geysers", "major nitrogen geysers volcanism" },
        { "Helium Geysers", "helium geysers volcanism" },
        { "Minor Helium Geysers", "minor helium geysers volcanism" },
        { "Major Helium Geysers", "major helium geysers volcanism" },
        { "Silicate Vapour Geysers", "silicate vapour geysers volcanism" },
        { "Minor Silicate Vapour Geysers", "minor silicate vapour geysers volcanism" },
        { "Major Silicate Vapour Geysers", "major silicate vapour geysers volcanism" }
    }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<string, string> EdsmToJournalStarClasses = new Dictionary<string, string>
    {
        { "A (Blue-White super giant) Star", "A" },
        { "A (Blue-White) Star", "A" },
        { "B (Blue-White super giant) Star", "B" },
        { "B (Blue-White) Star", "B" },
        { "Black Hole", "H" },
        { "C Star", "C" },
        { "CJ Star", "CJ" },
        { "CN Star", "CN" },
        { "F (White super giant) Star", "F" },
        { "F (White) Star", "F" },
        { "G (White-Yellow super giant) Star", "G" },
        { "G (White-Yellow) Star", "G" },
        { "Herbig Ae/Be Star", "AeBe" },
        { "K (Yellow-Orange giant) Star", "K" },
        { "K (Yellow-Orange) Star", "K" },
        { "L (Brown dwarf) Star", "L" },
        { "M (Red dwarf) Star", "M" },
        { "M (Red giant) Star", "M" },
        { "M (Red super giant) Star", "M" },
        { "MS-type Star", "MS" },
        { "Neutron Star", "N" },
        { "O (Blue-White) Star", "O" },
        { "S-type Star", "S" },
        { "T (Brown dwarf) Star", "T" },
        { "T Tauri Star", "TTS" },
        { "White Dwarf (D) Star", "D" },
        { "White Dwarf (DA) Star", "DA" },
        { "White Dwarf (DAB) Star", "DAB" },
        { "White Dwarf (DAO) Star", "DAO" },
        { "White Dwarf (DAV) Star", "DAV" },
        { "White Dwarf (DAZ) Star", "DAZ" },
        { "White Dwarf (DB) Star", "DB" },
        { "White Dwarf (DBV) Star", "DBV" },
        { "White Dwarf (DBZ) Star", "DBZ" },
        { "White Dwarf (DC) Star", "DC" },
        { "White Dwarf (DCV) Star", "DCV" },
        { "White Dwarf (DQ) Star", "DQ" },
        { "White Dwarf (DX) Star", "DX" },
        { "Wolf-Rayet C Star", "WC" },
        { "Wolf-Rayet NC Star", "WNC" },
        { "Wolf-Rayet O Star", "WO" },
        { "Wolf-Rayet Star", "W" },
        { "Y (Brown dwarf) Star", "Y" }
    }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<DataSource, ImmutableDictionary<RingType, string>> RingTypeDataSourceDescriptions = new Dictionary<DataSource, ImmutableDictionary<RingType, string>>
    {
        {
            DataSource.Journal,
            new Dictionary<RingType, string>
            {
                { RingType.MetalRich, "eRingClass_MetalRich" },
                { RingType.Metallic, "eRingClass_Metalic" },
                { RingType.Rocky, "eRingClass_Rocky" },
                { RingType.Icy, "eRingClass_Icy" }
            }.ToImmutableDictionary()
        },
        {
            DataSource.Edsm,
            new Dictionary<RingType, string>
            {
                { RingType.MetalRich, "Metal Rich" },
                { RingType.Metallic, "Metallic" },
                { RingType.Rocky, "Rocky" },
                { RingType.Icy, "Icy" }
            }.ToImmutableDictionary()
        }
    }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<Enum, List<string>> UserSelectableInputStringLists = new Dictionary<Enum, List<string>>
    {
        { UserSelectableInputStringListsKey.PlanetClasses, EdsmToJournalPlanetClasses.Select(x => x.Key).ToList() },
        { UserSelectableInputStringListsKey.Atmospheres, EdsmToJournalAtmospheres.Select(x => x.Key).ToList() },
        { UserSelectableInputStringListsKey.Volcanisms, EdsmToJournalVolcanisms.Select(x => x.Key).ToList() },
        { UserSelectableInputStringListsKey.StarClasses, Helpsters.GetUniqueJournalValues(EdsmToJournalStarClasses) },
        { UserSelectableInputStringListsKey.RingTypes, RingTypeDataSourceDescriptions[DataSource.Edsm].Select(x => x.Value).ToList() },
        { UserSelectableInputStringListsKey.RingReserveLevels, Enum.GetNames(typeof(RingReserveLevel)).ToList() }
    }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<string, string> GermanPlanetClassNames = new Dictionary<string, string>
    {
        { "metal rich body", "Metallreiche Welt" },
        { "high metal content body", "Welt mit hohem Metallgehalt" },
        { "high metal content world", "Welt mit hohem Metallgehalt" },
        { "rocky body", "Felsige Welt" },
        { "icy body", "Eiswelt" },
        { "rocky ice body", "Felsige Eiswelt" },
        { "rocky ice world", "Felsige Eiswelt" },
        { "earthlike body", "Erdähnliche Welt" },
        { "earth like world", "Erdähnliche Welt" },
        { "earth-like world", "Erdähnliche Welt" },
        { "water world", "Wasserwelt" },
        { "ammonia world", "Ammoniakwelt" },
        { "water giant", "Wasserriese" },
        { "gas giant with water based life", "Gasriese mit wasserbasiertem Leben" },
        { "gas giant with ammonia based life", "Gasriese mit ammoniakbasiertem Leben" },
        { "sudarsky class i gas giant", "Gasriese der Klasse I" },
        { "sudarsky class ii gas giant", "Gasriese der Klasse II" },
        { "sudarsky class iii gas giant", "Gasriese der Klasse III" },
        { "sudarsky class iv gas giant", "Gasriese der Klasse IV" },
        { "sudarsky class v gas giant", "Gasriese der Klasse V" },
        { "helium rich gas giant", "Heliumreicher Gasriese" },
        { "helium gas giant", "Helium-Gasriese" }
    }.ToImmutableDictionary();

        /// <summary>German dative-case planet class names (with indefinite article) for use after prepositions like "auf".</summary>
        public static readonly ImmutableDictionary<string, string> GermanPlanetClassNamesDative = new Dictionary<string, string>
    {
        { "Metallreiche Welt", "der metallreichen Welt" },
        { "Welt mit hohem Metallgehalt", "der Welt mit hohem Metallgehalt" },
        { "Felsige Welt", "der felsigen Welt" },
        { "Eiswelt", "der Eiswelt" },
        { "Felsige Eiswelt", "der felsigen Eiswelt" },
        { "Erdähnliche Welt", "der erdähnlichen Welt" },
        { "Wasserwelt", "der Wasserwelt" },
        { "Ammoniakwelt", "der Ammoniakwelt" },
        { "Wasserriese", "dem Wasserriesen" },
        { "Gasriese mit wasserbasiertem Leben", "dem Gasriesen mit wasserbasiertem Leben" },
        { "Gasriese mit ammoniakbasiertem Leben", "dem Gasriesen mit ammoniakbasiertem Leben" },
        { "Gasriese der Klasse I", "dem Gasriesen der Klasse I" },
        { "Gasriese der Klasse II", "dem Gasriesen der Klasse II" },
        { "Gasriese der Klasse III", "dem Gasriesen der Klasse III" },
        { "Gasriese der Klasse IV", "dem Gasriesen der Klasse IV" },
        { "Gasriese der Klasse V", "dem Gasriesen der Klasse V" },
        { "Heliumreicher Gasriese", "dem heliumreichen Gasriesen" },
        { "Helium-Gasriese", "dem Helium-Gasriesen" }
    }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<string, string> GermanAtmospherePrefixes = new Dictionary<string, string>
    {
        { "thin", "Dünne" },
        { "thick", "Dicke" },
        { "hot", "Heiße" },
        { "hot thin", "Heiße dünne" },
        { "hot thick", "Heiße dicke" },
        { "rich", "Reiche" }
    }.ToImmutableDictionary();

        /// <summary>German dative-case atmosphere prefixes for use in speech output.</summary>
        public static readonly ImmutableDictionary<string, string> GermanAtmospherePrefixesDative = new Dictionary<string, string>
    {
        { "thin", "dünner" },
        { "thick", "dicker" },
        { "hot", "heißer" },
        { "hot thin", "heißer dünner" },
        { "hot thick", "heißer dicker" },
        { "rich", "reicher" }
    }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<string, string> GermanAtmosphereTypes = new Dictionary<string, string>
    {
        { "ammonia", "Ammoniak-Atmosphäre" },
        { "argon", "Argon-Atmosphäre" },
        { "carbon dioxide", "Kohlendioxid-Atmosphäre" },
        { "helium", "Helium-Atmosphäre" },
        { "methane", "Methan-Atmosphäre" },
        { "metallic vapour", "Metalldampf-Atmosphäre" },
        { "neon", "Neon-Atmosphäre" },
        { "nitrogen", "Stickstoff-Atmosphäre" },
        { "oxygen", "Sauerstoff-Atmosphäre" },
        { "silicate vapour", "Silikatdampf-Atmosphäre" },
        { "sulfur dioxide", "Schwefeldioxid-Atmosphäre" },
        { "sulphur dioxide", "Schwefeldioxid-Atmosphäre" },
        { "water", "Wasser-Atmosphäre" }
    }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<string, string> GermanVolcanismPrefixes = new Dictionary<string, string>
    {
        { "minor", "Geringer" },
        { "major", "Starker" }
    }.ToImmutableDictionary();

        /// <summary>German dative-case volcanism intensity prefixes for use in speech output.</summary>
        public static readonly ImmutableDictionary<string, string> GermanVolcanismPrefixesDative = new Dictionary<string, string>
    {
        { "minor", "geringen" },
        { "major", "starken" }
    }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<string, string> GermanVolcanismElements = new Dictionary<string, string>
    {
        { "ammonia", "Ammoniak" },
        { "carbon dioxide", "Kohlendioxid" },
        { "helium", "Helium" },
        { "metallic", "Metall" },
        { "methane", "Methan" },
        { "nitrogen", "Stickstoff" },
        { "silicate", "Silikat" },
        { "silicate vapour", "Silikatdampf" },
        { "sulfur dioxide", "Schwefeldioxid" },
        { "sulphur dioxide", "Schwefeldioxid" },
        { "water", "Wasser" }
    }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<string, string> GermanVolcanismTypes = new Dictionary<string, string>
    {
        { "geysers", "Geysire" },
        { "magma", "Magma" },
        { "rocky magma", "Felsmagma" },
        { "iron magma", "Eisenmagma" }
    }.ToImmutableDictionary();

        /// <summary>German dative-case volcanism type names for use in speech output.</summary>
        public static readonly ImmutableDictionary<string, string> GermanVolcanismTypesDative = new Dictionary<string, string>
    {
        { "geysers", "Geysiren" },
        { "magma", "Magma" },
        { "rocky magma", "Felsmagma" },
        { "iron magma", "Eisenmagma" }
    }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<string, (string Minor, string Major)> GermanVolcanismIntensities = new Dictionary<string, (string, string)>
    {
        { "geysers", ("Geringe", "Starke") },
        { "magma", ("Geringes", "Starkes") },
        { "rocky magma", ("Geringes", "Starkes") },
        { "iron magma", ("Geringes", "Starkes") }
    }.ToImmutableDictionary();

        /// <summary>German dative-case volcanism intensity terms (minor/major) by type for speech output.</summary>
        public static readonly ImmutableDictionary<string, (string Minor, string Major)> GermanVolcanismIntensitiesDative = new Dictionary<string, (string, string)>
    {
        { "geysers", ("geringen", "starken") },
        { "magma", ("geringem", "starkem") },
        { "rocky magma", ("geringem", "starkem") },
        { "iron magma", ("geringem", "starkem") }
    }.ToImmutableDictionary();

        public static readonly ImmutableDictionary<string, string> GermanVolcanismSpecials = new Dictionary<string, string>
    {
        { "none", "Kein Vulkanismus" },
        { "no volcanism", "Kein Vulkanismus" }
    }.ToImmutableDictionary();

        /// <summary>
        /// Returns a localized display name for the given planet class.
        /// </summary>
        /// <param name="planetClass">The raw planet class.</param>
        /// <returns>The localized planet class or the original value.</returns>
        public static string GetLocalizedPlanetClass(string planetClass)
        {
            if (string.IsNullOrEmpty(planetClass))
                return planetClass;

            if (!Resources.Culture.TwoLetterISOLanguageName.Equals("de", StringComparison.OrdinalIgnoreCase))
                return planetClass;

            var normalized = planetClass.ToLowerInvariant().Replace('-', ' ').Trim();
            while (normalized.Contains("  "))
                normalized = normalized.Replace("  ", " ");

            return GermanPlanetClassNames.TryGetValue(normalized, out var german) ? german : planetClass;
        }

        /// <summary>
        /// Returns a localized dative-case planet class name for use after "auf".
        /// </summary>
        /// <param name="planetClass">The raw planet class.</param>
        /// <returns>The dative planet class or the original value.</returns>
        public static string GetLocalizedPlanetClassDative(string planetClass)
        {
            if (string.IsNullOrEmpty(planetClass))
                return planetClass;

            if (!Resources.Culture.TwoLetterISOLanguageName.Equals("de", StringComparison.OrdinalIgnoreCase))
                return planetClass;

            var nominative = GetLocalizedPlanetClass(planetClass);
            return GermanPlanetClassNamesDative.TryGetValue(nominative, out var dative) ? dative : nominative;
        }

        /// <summary>
        /// Returns a localized display text for the given atmosphere.
        /// </summary>
        /// <param name="atmosphere">The raw atmosphere description.</param>
        /// <returns>The localized atmosphere text or the original value.</returns>
        public static string GetLocalizedAtmosphere(string atmosphere)
        {
            if (string.IsNullOrEmpty(atmosphere) || atmosphere.Equals("no atmosphere", StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            if (!Resources.Culture.TwoLetterISOLanguageName.Equals("de", StringComparison.OrdinalIgnoreCase))
                return Helpsters.FirstLetterToUpperCase(atmosphere);

            var normalized = atmosphere.ToLowerInvariant().Trim();
            if (normalized.EndsWith(" atmosphere"))
                normalized = normalized.Substring(0, normalized.Length - " atmosphere".Length).Trim();

            var tokens = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            var resultTokens = new List<string>();
            bool isRich = false;
            if (tokens.Count > 0 && tokens[^1] == "rich")
            {
                isRich = true;
                tokens.RemoveAt(tokens.Count - 1);
            }

            string? gas = null;
            for (int wordCount = Math.Min(2, tokens.Count); wordCount >= 1; wordCount--)
            {
                var candidate = string.Join(" ", tokens.Skip(tokens.Count - wordCount));
                if (GermanAtmosphereTypes.ContainsKey(candidate))
                {
                    gas = candidate;
                    tokens.RemoveRange(tokens.Count - wordCount, wordCount);
                    break;
                }
            }

            if (gas == null)
                return Helpsters.FirstLetterToUpperCase(atmosphere);

            while (tokens.Count > 0)
            {
                bool matched = false;
                for (int wordCount = Math.Min(2, tokens.Count); wordCount >= 1; wordCount--)
                {
                    var candidate = string.Join(" ", tokens.Take(wordCount));
                    if (GermanAtmospherePrefixes.TryGetValue(candidate, out var prefix))
                    {
                        resultTokens.Add(prefix);
                        tokens.RemoveRange(0, wordCount);
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    resultTokens.Add(Helpsters.FirstLetterToUpperCase(tokens[0]));
                    tokens.RemoveAt(0);
                }
            }

            if (isRich)
                resultTokens.Add(GermanAtmospherePrefixes["rich"]);

            resultTokens.Add(GermanAtmosphereTypes[gas]);
            return string.Join(" ", resultTokens);
        }

        /// <summary>
        /// Returns a localized display text for the given volcanism.
        /// </summary>
        /// <param name="volcanism">The raw volcanism description.</param>
        /// <returns>The localized volcanism text or the original value.</returns>
        public static string GetLocalizedVolcanism(string volcanism)
        {
            if (string.IsNullOrEmpty(volcanism))
                return string.Empty;

            var normalized = volcanism.ToLowerInvariant().Trim();
            if (GermanVolcanismSpecials.TryGetValue(normalized, out var special))
                return special;

            if (!Resources.Culture.TwoLetterISOLanguageName.Equals("de", StringComparison.OrdinalIgnoreCase))
                return Helpsters.FirstLetterToUpperCase(volcanism);

            if (normalized.EndsWith(" volcanism"))
                normalized = normalized.Substring(0, normalized.Length - " volcanism".Length).Trim();

            string? prefix = null;
            foreach (var p in GermanVolcanismPrefixes.Keys)
            {
                if (normalized.StartsWith(p + " ", StringComparison.OrdinalIgnoreCase))
                {
                    prefix = p;
                    normalized = normalized.Substring(p.Length).Trim();
                    break;
                }
            }

            string? type = null;
            foreach (var t in GermanVolcanismTypes.Keys.OrderByDescending(k => k.Length))
            {
                if (normalized.EndsWith(" " + t, StringComparison.OrdinalIgnoreCase) || normalized == t)
                {
                    type = t;
                    if (normalized.Length > t.Length)
                        normalized = normalized.Substring(0, normalized.Length - t.Length).Trim();
                    else
                        normalized = string.Empty;
                    break;
                }
            }

            if (type == null)
                return Helpsters.FirstLetterToUpperCase(volcanism);

            string baseString;
            if (type == "rocky magma" || type == "iron magma")
            {
                baseString = GermanVolcanismTypes[type];
            }
            else
            {
                string? element = null;
                for (int wordCount = Math.Min(2, normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length); wordCount >= 1; wordCount--)
                {
                    var words = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length < wordCount) continue;
                    var candidate = string.Join(" ", words.Take(wordCount));
                    if (GermanVolcanismElements.TryGetValue(candidate, out var elementGerman))
                    {
                        element = elementGerman;
                        break;
                    }
                }

                if (element == null)
                    element = Helpsters.FirstLetterToUpperCase(normalized);

                baseString = type == "geysers"
                    ? $"{element}-{GermanVolcanismTypes[type]}"
                    : $"{element}-{GermanVolcanismTypes[type]}";
            }

            if (string.IsNullOrEmpty(prefix))
                return baseString;

            var (minor, major) = GermanVolcanismIntensities[type];
            var prefixGerman = prefix == "minor" ? minor : major;
            return $"{prefixGerman} {baseString}";
        }

        /// <summary>
        /// Returns a localized speech text for the given atmosphere, using dative case for use after "mit".
        /// </summary>
        /// <param name="atmosphere">The raw atmosphere description.</param>
        /// <returns>The localized atmosphere speech text or the original value.</returns>
        public static string GetLocalizedAtmosphereForSpeech(string atmosphere)
        {
            bool isGerman = Resources.Culture.TwoLetterISOLanguageName.Equals("de", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(atmosphere) || atmosphere.Equals("no atmosphere", StringComparison.OrdinalIgnoreCase))
                return isGerman ? "keiner Atmosphäre" : "no atmosphere";

            if (!isGerman)
                return atmosphere.ToLowerInvariant();

            var normalized = atmosphere.ToLowerInvariant().Trim();
            if (normalized.EndsWith(" atmosphere"))
                normalized = normalized.Substring(0, normalized.Length - " atmosphere".Length).Trim();

            var tokens = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            var resultTokens = new List<string>();
            bool isRich = false;
            if (tokens.Count > 0 && tokens[^1] == "rich")
            {
                isRich = true;
                tokens.RemoveAt(tokens.Count - 1);
            }

            string? gas = null;
            for (int wordCount = Math.Min(2, tokens.Count); wordCount >= 1; wordCount--)
            {
                var candidate = string.Join(" ", tokens.Skip(tokens.Count - wordCount));
                if (GermanAtmosphereTypes.ContainsKey(candidate))
                {
                    gas = candidate;
                    tokens.RemoveRange(tokens.Count - wordCount, wordCount);
                    break;
                }
            }

            if (gas == null)
                return atmosphere.ToLowerInvariant();

            while (tokens.Count > 0)
            {
                bool matched = false;
                for (int wordCount = Math.Min(2, tokens.Count); wordCount >= 1; wordCount--)
                {
                    var candidate = string.Join(" ", tokens.Take(wordCount));
                    if (GermanAtmospherePrefixesDative.TryGetValue(candidate, out var prefix))
                    {
                        resultTokens.Add(prefix);
                        tokens.RemoveRange(0, wordCount);
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    resultTokens.Add(tokens[0]);
                    tokens.RemoveAt(0);
                }
            }

            if (isRich)
                resultTokens.Add(GermanAtmospherePrefixesDative["rich"]);

            resultTokens.Add(GermanAtmosphereTypes[gas]);
            return string.Join(" ", resultTokens);
        }

        /// <summary>
        /// Returns a localized speech text for the given volcanism, using dative case for use after "mit".
        /// </summary>
        /// <param name="volcanism">The raw volcanism description.</param>
        /// <returns>The localized volcanism speech text or the original value.</returns>
        public static string GetLocalizedVolcanismForSpeech(string volcanism)
        {
            bool isGerman = Resources.Culture.TwoLetterISOLanguageName.Equals("de", StringComparison.OrdinalIgnoreCase);

            if (string.IsNullOrEmpty(volcanism))
                return isGerman ? "keinem Vulkanismus" : "no volcanism";

            var normalized = volcanism.ToLowerInvariant().Trim();
            if (GermanVolcanismSpecials.TryGetValue(normalized, out var special))
            {
                if (special.Equals("Kein Vulkanismus", StringComparison.OrdinalIgnoreCase))
                    return isGerman ? "keinem Vulkanismus" : "no volcanism";
                return special;
            }

            if (!isGerman)
                return volcanism.ToLowerInvariant().Replace(" volcanism", string.Empty);

            if (normalized.EndsWith(" volcanism"))
                normalized = normalized.Substring(0, normalized.Length - " volcanism".Length).Trim();

            string? prefix = null;
            foreach (var p in GermanVolcanismPrefixesDative.Keys)
            {
                if (normalized.StartsWith(p + " ", StringComparison.OrdinalIgnoreCase))
                {
                    prefix = p;
                    normalized = normalized.Substring(p.Length).Trim();
                    break;
                }
            }

            string? type = null;
            foreach (var t in GermanVolcanismTypesDative.Keys.OrderByDescending(k => k.Length))
            {
                if (normalized.EndsWith(" " + t, StringComparison.OrdinalIgnoreCase) || normalized == t)
                {
                    type = t;
                    if (normalized.Length > t.Length)
                        normalized = normalized.Substring(0, normalized.Length - t.Length).Trim();
                    else
                        normalized = string.Empty;
                    break;
                }
            }

            if (type == null)
                return volcanism.ToLowerInvariant().Replace(" volcanism", string.Empty);

            string baseString;
            if (type == "rocky magma" || type == "iron magma")
            {
                baseString = GermanVolcanismTypesDative[type];
            }
            else
            {
                string? element = null;
                for (int wordCount = Math.Min(2, normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length); wordCount >= 1; wordCount--)
                {
                    var words = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length < wordCount) continue;
                    var candidate = string.Join(" ", words.Take(wordCount));
                    if (GermanVolcanismElements.TryGetValue(candidate, out var elementGerman))
                    {
                        element = elementGerman;
                        break;
                    }
                }

                if (element == null)
                    element = Helpsters.FirstLetterToUpperCase(normalized);

                baseString = $"{element}-{GermanVolcanismTypesDative[type]}";
            }

            if (string.IsNullOrEmpty(prefix))
                return baseString;

            var (minor, major) = GermanVolcanismIntensitiesDative[type];
            var prefixGerman = prefix == "minor" ? minor : major;
            return $"{prefixGerman} {baseString}";
        }
    }
}
