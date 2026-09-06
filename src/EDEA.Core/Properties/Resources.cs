using System.Globalization;
using System.Resources;

namespace EDEA.Properties;

/// <summary>Provides access to localized application resources.</summary>
public static class Resources
{
    /// <summary>The <see cref="ResourceManager"/> used to access localized resources.</summary>
    private static readonly ResourceManager _resourceManager = new("EDEA.Properties.Resources", typeof(Resources).Assembly);

    /// <summary>The <see cref="CultureInfo"/> used for resource lookups.</summary>
    private static CultureInfo _culture = CultureInfo.CurrentUICulture;

    /// <summary>Gets or sets the culture used for resource lookups.</summary>
    /// <value>The current UI culture for resources.</value>
    public static CultureInfo Culture
    {
        get => _culture;
        set => _culture = value;
    }

    /// <summary>Gets the resource string with the specified name.</summary>
    /// <param name="name">The name of the resource.</param>
    /// <returns>The resource string, or the name wrapped in brackets if not found.</returns>
    private static string Get(string name)
    {
        return _resourceManager.GetString(name, _culture)
            ?? _resourceManager.GetString(name, new CultureInfo("en"))
            ?? $"[{name}]";
    }

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MainWindow_Title => Get(nameof(MainWindow_Title));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ApplicationLanguage => Get(nameof(ApplicationLanguage));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_ReloadEdsm => Get(nameof(MenuItem_ReloadEdsm));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_ResetTripData => Get(nameof(MenuItem_ResetTripData));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_ImportJournalHistory => Get(nameof(MenuItem_ImportJournalHistory));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_DeleteExplorationHistory => Get(nameof(MenuItem_DeleteExplorationHistory));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_Preferences => Get(nameof(MenuItem_Preferences));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_FeedbackReportIssue => Get(nameof(MenuItem_FeedbackReportIssue));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_About => Get(nameof(MenuItem_About));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string StatusBar_CurrentSystem => Get(nameof(StatusBar_CurrentSystem));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string StatusBar_DiscoveredBodies => Get(nameof(StatusBar_DiscoveredBodies));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string StatusBar_NonBodySignals => Get(nameof(StatusBar_NonBodySignals));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string StatusBar_CurrentActivity => Get(nameof(StatusBar_CurrentActivity));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string HudWindow_Title => Get(nameof(HudWindow_Title));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string AboutWindow_Title => Get(nameof(AboutWindow_Title));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string AboutWindow_AppName => Get(nameof(AboutWindow_AppName));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string AboutWindow_Version => Get(nameof(AboutWindow_Version));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string AboutWindow_Description => Get(nameof(AboutWindow_Description));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string AboutWindow_License => Get(nameof(AboutWindow_License));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string AboutWindow_OK => Get(nameof(AboutWindow_OK));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string FeedbackReportIssueWindow_Title => Get(nameof(FeedbackReportIssueWindow_Title));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string FeedbackReportIssueWindow_Headline => Get(nameof(FeedbackReportIssueWindow_Headline));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string FeedbackReportIssueWindow_Description => Get(nameof(FeedbackReportIssueWindow_Description));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string FeedbackReportIssueWindow_Name => Get(nameof(FeedbackReportIssueWindow_Name));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string FeedbackReportIssueWindow_Email => Get(nameof(FeedbackReportIssueWindow_Email));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string FeedbackReportIssueWindow_Message => Get(nameof(FeedbackReportIssueWindow_Message));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string FeedbackReportIssueWindow_Send => Get(nameof(FeedbackReportIssueWindow_Send));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string FeedbackReportIssueWindow_Cancel => Get(nameof(FeedbackReportIssueWindow_Cancel));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Title => Get(nameof(PreferencesWindow_Title));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Save => Get(nameof(PreferencesWindow_Save));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Cancel => Get(nameof(PreferencesWindow_Cancel));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_ResetPreferences => Get(nameof(PreferencesWindow_ResetPreferences));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_ResetWarning => Get(nameof(PreferencesWindow_ResetWarning));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_NoShipData => Get(nameof(RoutePlotterWindow_NoShipData));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_ErrorMessage => Get(nameof(RoutePlotterWindow_ErrorMessage));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_TryAgain => Get(nameof(RoutePlotterWindow_TryAgain));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_Cancel => Get(nameof(RoutePlotterWindow_Cancel));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_GenerateRoute => Get(nameof(RoutePlotterWindow_GenerateRoute));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_ShipName => Get(nameof(RoutePlotterWindow_ShipName));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_FrameShiftDrive => Get(nameof(RoutePlotterWindow_FrameShiftDrive));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_FsdBooster => Get(nameof(RoutePlotterWindow_FsdBooster));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_Class => Get(nameof(RoutePlotterWindow_Class));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_Rating => Get(nameof(RoutePlotterWindow_Rating));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_CargoCount => Get(nameof(RoutePlotterWindow_CargoCount));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_Supercharged => Get(nameof(RoutePlotterWindow_Supercharged));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_FsdOptimalMass => Get(nameof(RoutePlotterWindow_FsdOptimalMass));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_SourceSystem => Get(nameof(RoutePlotterWindow_SourceSystem));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_DestinationSystem => Get(nameof(RoutePlotterWindow_DestinationSystem));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_Algorithm => Get(nameof(RoutePlotterWindow_Algorithm));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_UseSupercharge => Get(nameof(RoutePlotterWindow_UseSupercharge));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_UseFsdInjections => Get(nameof(RoutePlotterWindow_UseFsdInjections));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_ExcludeSecondaryStars => Get(nameof(RoutePlotterWindow_ExcludeSecondaryStars));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_RefuelEveryScoopable => Get(nameof(RoutePlotterWindow_RefuelEveryScoopable));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_Efficiency => Get(nameof(RoutePlotterWindow_Efficiency));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string JournalHistoryImportWindow_Title => Get(nameof(JournalHistoryImportWindow_Title));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string JournalHistoryImportWindow_JournalFilesProcessed => Get(nameof(JournalHistoryImportWindow_JournalFilesProcessed));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string JournalHistoryImportWindow_DetectingStarSystems => Get(nameof(JournalHistoryImportWindow_DetectingStarSystems));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string JournalHistoryImportWindow_DetectingBodies => Get(nameof(JournalHistoryImportWindow_DetectingBodies));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string JournalHistoryImportWindow_DetectingAdditionalData => Get(nameof(JournalHistoryImportWindow_DetectingAdditionalData));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string JournalHistoryImportWindow_NewSystemsAdded => Get(nameof(JournalHistoryImportWindow_NewSystemsAdded));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string JournalHistoryImportWindow_SystemsUpdated => Get(nameof(JournalHistoryImportWindow_SystemsUpdated));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string JournalHistoryImportWindow_SystemsIgnored => Get(nameof(JournalHistoryImportWindow_SystemsIgnored));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string JournalHistoryImportWindow_StartImport => Get(nameof(JournalHistoryImportWindow_StartImport));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string JournalHistoryImportWindow_StopImport => Get(nameof(JournalHistoryImportWindow_StopImport));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string InputStringDialogWindow_Title => Get(nameof(InputStringDialogWindow_Title));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string InputStringDialogWindow_OK => Get(nameof(InputStringDialogWindow_OK));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string InputStringDialogWindow_Cancel => Get(nameof(InputStringDialogWindow_Cancel));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string InputStringListDialogWindow_Title => Get(nameof(InputStringListDialogWindow_Title));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string InputStringListDialogWindow_OK => Get(nameof(InputStringListDialogWindow_OK));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string InputStringListDialogWindow_Cancel => Get(nameof(InputStringListDialogWindow_Cancel));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Tab_Appearance => Get(nameof(PreferencesWindow_Tab_Appearance));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Tab_HudWindow => Get(nameof(PreferencesWindow_Tab_HudWindow));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Tab_Speech => Get(nameof(PreferencesWindow_Tab_Speech));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Tab_PlanetsOfInterest => Get(nameof(PreferencesWindow_Tab_PlanetsOfInterest));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_SubTab_BasicAttributes => Get(nameof(PreferencesWindow_SubTab_BasicAttributes));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_SubTab_SurfaceConditions => Get(nameof(PreferencesWindow_SubTab_SurfaceConditions));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_SubTab_RingRelated => Get(nameof(PreferencesWindow_SubTab_RingRelated));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_SubTab_ParentPlanet => Get(nameof(PreferencesWindow_SubTab_ParentPlanet));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Tab_GlobalHotkeys => Get(nameof(PreferencesWindow_Tab_GlobalHotkeys));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Tab_Configuration => Get(nameof(PreferencesWindow_Tab_Configuration));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderName => Get(nameof(ColumnHeaderName));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderType => Get(nameof(ColumnHeaderType));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderTemperature => Get(nameof(ColumnHeaderTemperature));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderAtmosphere => Get(nameof(ColumnHeaderAtmosphere));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderEdsmDiscoverer => Get(nameof(ColumnHeaderEdsmDiscoverer));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderCartographicValue => Get(nameof(ColumnHeaderCartographicValue));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderOverallProgress => Get(nameof(ColumnHeaderOverallProgress));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderDistance => Get(nameof(ColumnHeaderDistance));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderJump => Get(nameof(ColumnHeaderJump));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderSystemName => Get(nameof(ColumnHeaderSystemName));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderStarClass => Get(nameof(ColumnHeaderStarClass));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderDiscoveryStatus => Get(nameof(ColumnHeaderDiscoveryStatus));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderSpecies => Get(nameof(ColumnHeaderSpecies));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderVariant => Get(nameof(ColumnHeaderVariant));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderScans => Get(nameof(ColumnHeaderScans));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderClonalColonyRange => Get(nameof(ColumnHeaderClonalColonyRange));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeader1stScanDistance => Get(nameof(ColumnHeader1stScanDistance));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeader2ndScanDistance => Get(nameof(ColumnHeader2ndScanDistance));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string ColumnHeaderVistaGenomicsValue => Get(nameof(ColumnHeaderVistaGenomicsValue));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_ClearGalaxyPlotterRoute => Get(nameof(MenuItem_ClearGalaxyPlotterRoute));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_GenerateGalaxyPlotterRoute => Get(nameof(MenuItem_GenerateGalaxyPlotterRoute));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_UnlockRoute => Get(nameof(MenuItem_UnlockRoute));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_LockRoute => Get(nameof(MenuItem_LockRoute));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_ExportLockedRoute => Get(nameof(MenuItem_ExportLockedRoute));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_ImportLockedRoute => Get(nameof(MenuItem_ImportLockedRoute));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_OpenHudWindow => Get(nameof(MenuItem_OpenHudWindow));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_CloseHudWindow => Get(nameof(MenuItem_CloseHudWindow));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_EnableHudMousePassThrough => Get(nameof(MenuItem_EnableHudMousePassThrough));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_DisableHudMousePassThrough => Get(nameof(MenuItem_DisableHudMousePassThrough));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipJournalIcon => Get(nameof(TooltipJournalIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipEdsmOnly => Get(nameof(TooltipEdsmOnly));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipNewIcon => Get(nameof(TooltipNewIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipValuableBodyIcon => Get(nameof(TooltipValuableBodyIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipValuableGenusIcon => Get(nameof(TooltipValuableGenusIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipValuableGenusPredictedIcon => Get(nameof(TooltipValuableGenusPredictedIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipLoadingIcon => Get(nameof(TooltipLoadingIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipPopulatedIcon => Get(nameof(TooltipPopulatedIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipTerraformableIcon => Get(nameof(TooltipTerraformableIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipLandableIcon => Get(nameof(TooltipLandableIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipTouchdownIcon => Get(nameof(TooltipTouchdownIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipLandableTouchdownCombiIcon => Get(nameof(TooltipLandableTouchdownCombiIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipRingsIcon => Get(nameof(TooltipRingsIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipGeologicalsIcon => Get(nameof(TooltipGeologicalsIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipBiologicalsIcon => Get(nameof(TooltipBiologicalsIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipScoopableIcon => Get(nameof(TooltipScoopableIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipCurrentBodyIcon => Get(nameof(TooltipCurrentBodyIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipCurrentSystemIcon => Get(nameof(TooltipCurrentSystemIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipJumpDestinationSystemIcon => Get(nameof(TooltipJumpDestinationSystemIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipSurfaceScannedStatusIcon => Get(nameof(TooltipSurfaceScannedStatusIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipScannedAndWasMappedIcon => Get(nameof(TooltipScannedAndWasMappedIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipScannedAndFirstMappedIcon => Get(nameof(TooltipScannedAndFirstMappedIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipUnscannedAndWasMappedIcon => Get(nameof(TooltipUnscannedAndWasMappedIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipUnknownSurfaceScanStatusIcon => Get(nameof(TooltipUnknownSurfaceScanStatusIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipAnalysisCompleteIcon => Get(nameof(TooltipAnalysisCompleteIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipPlanetOfInterestIcon => Get(nameof(TooltipPlanetOfInterestIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipGenusIsFirstDiscovery => Get(nameof(TooltipGenusIsFirstDiscovery));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipDistanceToPreviousSystem => Get(nameof(TooltipDistanceToPreviousSystem));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TooltipDistanceToCurrentSystem => Get(nameof(TooltipDistanceToCurrentSystem));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_Colors => Get(nameof(PreferencesWindow_Label_Colors));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_DisplaySize => Get(nameof(PreferencesWindow_Label_DisplaySize));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Radio_Small => Get(nameof(PreferencesWindow_Radio_Small));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Radio_Standard => Get(nameof(PreferencesWindow_Radio_Standard));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Radio_Large => Get(nameof(PreferencesWindow_Radio_Large));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Radio_Huge => Get(nameof(PreferencesWindow_Radio_Huge));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_Behavior => Get(nameof(PreferencesWindow_Label_Behavior));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Checkbox_AutomaticTabSwitching => Get(nameof(PreferencesWindow_Checkbox_AutomaticTabSwitching));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_Columns => Get(nameof(PreferencesWindow_Label_Columns));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Bodies => Get(nameof(PreferencesWindow_Bodies));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Route => Get(nameof(PreferencesWindow_Route));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Biologicals => Get(nameof(PreferencesWindow_Biologicals));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_BodiesColumn_SurfaceScanIcon => Get(nameof(PreferencesWindow_BodiesColumn_SurfaceScanIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_BodiesColumn_PlanetOfInterestIcon => Get(nameof(PreferencesWindow_BodiesColumn_PlanetOfInterestIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_BodiesColumn_ValuableBodyIcon => Get(nameof(PreferencesWindow_BodiesColumn_ValuableBodyIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_BodiesColumn_RingsIcon => Get(nameof(PreferencesWindow_BodiesColumn_RingsIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_BodiesColumn_GeologicalsIcon => Get(nameof(PreferencesWindow_BodiesColumn_GeologicalsIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_BodiesColumn_BiologicalsIcon => Get(nameof(PreferencesWindow_BodiesColumn_BiologicalsIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_BodiesColumn_TerraformableIcon => Get(nameof(PreferencesWindow_BodiesColumn_TerraformableIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_BodiesColumn_LandableIcon => Get(nameof(PreferencesWindow_BodiesColumn_LandableIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_BodiesColumn_FirstDiscoveryIcon => Get(nameof(PreferencesWindow_BodiesColumn_FirstDiscoveryIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_RouteColumn_ScoopableStarIconStarClass => Get(nameof(PreferencesWindow_RouteColumn_ScoopableStarIconStarClass));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_RouteColumn_ExplorationStatusIcon => Get(nameof(PreferencesWindow_RouteColumn_ExplorationStatusIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_RouteColumn_PlanetOfInterestIcon => Get(nameof(PreferencesWindow_RouteColumn_PlanetOfInterestIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_RouteColumn_ValuableBodiesIcon => Get(nameof(PreferencesWindow_RouteColumn_ValuableBodiesIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_RouteColumn_TerraformablesIcon => Get(nameof(PreferencesWindow_RouteColumn_TerraformablesIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_RouteColumn_LandablesIcon => Get(nameof(PreferencesWindow_RouteColumn_LandablesIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_RouteColumn_PopulatedIcon => Get(nameof(PreferencesWindow_RouteColumn_PopulatedIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_GenusColumn_AnalysisCompleteIcon => Get(nameof(PreferencesWindow_GenusColumn_AnalysisCompleteIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_GenusColumn_ValuableSpeciesIcon => Get(nameof(PreferencesWindow_GenusColumn_ValuableSpeciesIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_GenusColumn_FirstDiscoveryIcon => Get(nameof(PreferencesWindow_GenusColumn_FirstDiscoveryIcon));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_Opacity => Get(nameof(PreferencesWindow_Label_Opacity));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_View => Get(nameof(PreferencesWindow_Label_View));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Radio_SyncedWithMainWindow => Get(nameof(PreferencesWindow_Radio_SyncedWithMainWindow));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_AutomaticHideOn => Get(nameof(PreferencesWindow_Label_AutomaticHideOn));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_InShipSrvFighter => Get(nameof(PreferencesWindow_Label_InShipSrvFighter));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_HideOn_NoActivePanel => Get(nameof(PreferencesWindow_HideOn_NoActivePanel));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_HideOn_InternalPanel => Get(nameof(PreferencesWindow_HideOn_InternalPanel));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_HideOn_ExternalPanel => Get(nameof(PreferencesWindow_HideOn_ExternalPanel));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_HideOn_CommsPanel => Get(nameof(PreferencesWindow_HideOn_CommsPanel));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_HideOn_RolePanel => Get(nameof(PreferencesWindow_HideOn_RolePanel));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_HideOn_StationServices => Get(nameof(PreferencesWindow_HideOn_StationServices));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_HideOn_GalaxyMap => Get(nameof(PreferencesWindow_HideOn_GalaxyMap));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_HideOn_SystemMap => Get(nameof(PreferencesWindow_HideOn_SystemMap));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_HideOn_Orrery => Get(nameof(PreferencesWindow_HideOn_Orrery));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_HideOn_FssMode => Get(nameof(PreferencesWindow_HideOn_FssMode));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_HideOn_SaaMode => Get(nameof(PreferencesWindow_HideOn_SaaMode));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_HideOn_Codex => Get(nameof(PreferencesWindow_HideOn_Codex));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_OnFoot => Get(nameof(PreferencesWindow_Label_OnFoot));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_HideOn_Always => Get(nameof(PreferencesWindow_HideOn_Always));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_Outputs => Get(nameof(PreferencesWindow_Label_Outputs));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_SpeechOutputText => Get(nameof(PreferencesWindow_Label_SpeechOutputText));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_TextPlaceholder => Get(nameof(PreferencesWindow_Label_TextPlaceholder));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_Example => Get(nameof(PreferencesWindow_Label_Example));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Button_TestSpeechOutput => Get(nameof(PreferencesWindow_Button_TestSpeechOutput));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_Voice => Get(nameof(PreferencesWindow_Label_Voice));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_Rate => Get(nameof(PreferencesWindow_Label_Rate));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_Volume => Get(nameof(PreferencesWindow_Label_Volume));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_CriteriaSets => Get(nameof(PreferencesWindow_Label_CriteriaSets));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Button_Add => Get(nameof(PreferencesWindow_Button_Add));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Button_Rename => Get(nameof(PreferencesWindow_Button_Rename));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Button_Delete => Get(nameof(PreferencesWindow_Button_Delete));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_DistanceToMainStar => Get(nameof(PreferencesWindow_Label_DistanceToMainStar));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_HigherThan => Get(nameof(PreferencesWindow_Label_HigherThan));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_AndLowerThan => Get(nameof(PreferencesWindow_Label_AndLowerThan));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_BodyRadius => Get(nameof(PreferencesWindow_Label_BodyRadius));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_OrbitalInclination => Get(nameof(PreferencesWindow_Label_OrbitalInclination));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_IsLandable => Get(nameof(PreferencesWindow_Label_IsLandable));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_PlanetClassOneOf => Get(nameof(PreferencesWindow_Label_PlanetClassOneOf));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Button_Set => Get(nameof(PreferencesWindow_Button_Set));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_StarClassOneOf => Get(nameof(PreferencesWindow_Label_StarClassOneOf));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_Gravity => Get(nameof(PreferencesWindow_Label_Gravity));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_Temperature => Get(nameof(PreferencesWindow_Label_Temperature));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_AtmosphereOneOf => Get(nameof(PreferencesWindow_Label_AtmosphereOneOf));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_VolcanismOneOf => Get(nameof(PreferencesWindow_Label_VolcanismOneOf));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_RingsTotalWidth => Get(nameof(PreferencesWindow_Label_RingsTotalWidth));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_RingWidth => Get(nameof(PreferencesWindow_Label_RingWidth));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_RingDensity => Get(nameof(PreferencesWindow_Label_RingDensity));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_RingTypeOneOf => Get(nameof(PreferencesWindow_Label_RingTypeOneOf));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_RingReserveLevelOneOf => Get(nameof(PreferencesWindow_Label_RingReserveLevelOneOf));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_CriteriaSetForParentPlanet => Get(nameof(PreferencesWindow_Label_CriteriaSetForParentPlanet));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_Assignment => Get(nameof(PreferencesWindow_Label_Assignment));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_JournalFilesPath => Get(nameof(PreferencesWindow_Label_JournalFilesPath));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_Thresholds => Get(nameof(PreferencesWindow_Label_Thresholds));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_ValuableBodies => Get(nameof(PreferencesWindow_Label_ValuableBodies));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_ValuableSpecies => Get(nameof(PreferencesWindow_Label_ValuableSpecies));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_BiologicalsViewAltitude => Get(nameof(PreferencesWindow_Label_BiologicalsViewAltitude));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Label_ResetPreferences => Get(nameof(PreferencesWindow_Label_ResetPreferences));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string StatusNoData => Get(nameof(StatusNoData));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string StatusSingleBody => Get(nameof(StatusSingleBody));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string UnitBodySingular => Get(nameof(UnitBodySingular));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string UnitBodyPlural => Get(nameof(UnitBodyPlural));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string UnitCredits => Get(nameof(UnitCredits));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string UnitPercent => Get(nameof(UnitPercent));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TabHeader_PlotterRoute => Get(nameof(TabHeader_PlotterRoute));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TabHeader_LockedRoute => Get(nameof(TabHeader_LockedRoute));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TabHeader_Route => Get(nameof(TabHeader_Route));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TabHeader_Bodies => Get(nameof(TabHeader_Bodies));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TabHeader_Biologicals => Get(nameof(TabHeader_Biologicals));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TabHeader_Surroundings => Get(nameof(TabHeader_Surroundings));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TabHeader_History => Get(nameof(TabHeader_History));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string NoSurroundingsInfo => Get(nameof(NoSurroundingsInfo));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_Loading_Part1 => Get(nameof(RoutePlotterWindow_Loading_Part1));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_Loading_Line2 => Get(nameof(RoutePlotterWindow_Loading_Line2));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_Loading_Line4 => Get(nameof(RoutePlotterWindow_Loading_Line4));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_Loading_Line5 => Get(nameof(RoutePlotterWindow_Loading_Line5));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string NoRouteInfo => Get(nameof(NoRouteInfo));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string NoBodiesInfo => Get(nameof(NoBodiesInfo));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Hud_BodyExplorationStatus => Get(nameof(Hud_BodyExplorationStatus));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Hud_NonBodyExplorationStatus => Get(nameof(Hud_NonBodyExplorationStatus));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_Title => Get(nameof(RoutePlotterWindow_Title));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_CarrierName => Get(nameof(RoutePlotterWindow_CarrierName));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_CarrierType => Get(nameof(RoutePlotterWindow_CarrierType));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_CarrierFuel => Get(nameof(RoutePlotterWindow_CarrierFuel));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_CarrierRange => Get(nameof(RoutePlotterWindow_CarrierRange));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotterWindow_CarrierCapacity => Get(nameof(RoutePlotterWindow_CarrierCapacity));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MessageBoxTitle_Error => Get(nameof(MessageBoxTitle_Error));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_Status => Get(nameof(Templates_Tooltip_Status));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_BodyCount => Get(nameof(Templates_Tooltip_BodyCount));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_Journal => Get(nameof(Templates_Tooltip_Journal));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_AllBodiesFound => Get(nameof(Templates_Tooltip_AllBodiesFound));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_EDSM => Get(nameof(Templates_Tooltip_EDSM));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_CartographicProgress => Get(nameof(Templates_Tooltip_CartographicProgress));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_ValueAchieved => Get(nameof(Templates_Tooltip_ValueAchieved));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_AchievableValue => Get(nameof(Templates_Tooltip_AchievableValue));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_VistaGenomicsProgress => Get(nameof(Templates_Tooltip_VistaGenomicsProgress));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_ValueAchievedLabel => Get(nameof(Templates_Tooltip_ValueAchievedLabel));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_BiologicalsAnalyzed => Get(nameof(Templates_Tooltip_BiologicalsAnalyzed));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_OverallProgress => Get(nameof(Templates_Tooltip_OverallProgress));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_TotalValueAchieved => Get(nameof(Templates_Tooltip_TotalValueAchieved));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_BaseValue => Get(nameof(Templates_Tooltip_BaseValue));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_SurfaceScanValue => Get(nameof(Templates_Tooltip_SurfaceScanValue));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_BonusesValue => Get(nameof(Templates_Tooltip_BonusesValue));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_Radius => Get(nameof(Templates_Tooltip_Radius));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_Gravity => Get(nameof(Templates_Tooltip_Gravity));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_Mass => Get(nameof(Templates_Tooltip_Mass));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_OrbitalInclination => Get(nameof(Templates_Tooltip_OrbitalInclination));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_FirstDiscoveryBonus => Get(nameof(Templates_Tooltip_FirstDiscoveryBonus));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_FirstSurfaceScanBonus => Get(nameof(Templates_Tooltip_FirstSurfaceScanBonus));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_EfficientlyScannedBonus => Get(nameof(Templates_Tooltip_EfficientlyScannedBonus));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_MatchingCriteriaSets => Get(nameof(Templates_Tooltip_MatchingCriteriaSets));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_PlanetsOfInterest => Get(nameof(Templates_Tooltip_PlanetsOfInterest));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_TotalWidthOfRings => Get(nameof(Templates_Tooltip_TotalWidthOfRings));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_ReserveLevel => Get(nameof(Templates_Tooltip_ReserveLevel));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_BiologicalSignalsDetected => Get(nameof(Templates_Tooltip_BiologicalSignalsDetected));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_AnalysedDiscoveredBiologicals => Get(nameof(Templates_Tooltip_AnalysedDiscoveredBiologicals));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_FirstDiscoveryBonusApplicable => Get(nameof(Templates_Tooltip_FirstDiscoveryBonusApplicable));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_Yes => Get(nameof(Templates_Tooltip_Yes));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_No => Get(nameof(Templates_Tooltip_No));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Hotkey_Unassigned => Get(nameof(Hotkey_Unassigned));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Templates_Tooltip_PossibleOccurrences => Get(nameof(Templates_Tooltip_PossibleOccurrences));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Headline_CurrentExplorationTrip => Get(nameof(History_Headline_CurrentExplorationTrip));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Headline_EntireExplorationHistory => Get(nameof(History_Headline_EntireExplorationHistory));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Group_EstimatedTotalValueOfExplorationData => Get(nameof(History_Group_EstimatedTotalValueOfExplorationData));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Group_EstimatedCartographicValues => Get(nameof(History_Group_EstimatedCartographicValues));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Group_EstimatedVistaGenomicsValues => Get(nameof(History_Group_EstimatedVistaGenomicsValues));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Group_Systems => Get(nameof(History_Group_Systems));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Group_Bodies => Get(nameof(History_Group_Bodies));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Group_Rings => Get(nameof(History_Group_Rings));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Group_Biologicals => Get(nameof(History_Group_Biologicals));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_Base => Get(nameof(History_Panel_Base));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_SurfaceScan => Get(nameof(History_Panel_SurfaceScan));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_Bonuses => Get(nameof(History_Panel_Bonuses));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_FirstDiscoveryBonus => Get(nameof(History_Panel_FirstDiscoveryBonus));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_Discovered => Get(nameof(History_Panel_Discovered));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_FirstDiscovery => Get(nameof(History_Panel_FirstDiscovery));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_MostFrequent => Get(nameof(History_Panel_MostFrequent));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_Rarest => Get(nameof(History_Panel_Rarest));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_Signals => Get(nameof(History_Panel_Signals));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_Mapped => Get(nameof(History_Panel_Mapped));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_Terraformable => Get(nameof(History_Panel_Terraformable));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_WithTouchdowns => Get(nameof(History_Panel_WithTouchdowns));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_Valuable => Get(nameof(History_Panel_Valuable));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_WithRings => Get(nameof(History_Panel_WithRings));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_Analysed => Get(nameof(History_Panel_Analysed));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_MostFrequentAnalysed => Get(nameof(History_Panel_MostFrequentAnalysed));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string History_Panel_RarestAnalysed => Get(nameof(History_Panel_RarestAnalysed));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string GenusTable_PossibleOccurrences => Get(nameof(GenusTable_PossibleOccurrences));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string GenusHud_PossibleOccurrences => Get(nameof(GenusHud_PossibleOccurrences));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string CurrentStatus_WaitingForGame => Get(nameof(CurrentStatus_WaitingForGame));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string CurrentStatus_ExploringSystem => Get(nameof(CurrentStatus_ExploringSystem));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string CurrentStatus_UsingPlotterRoute => Get(nameof(CurrentStatus_UsingPlotterRoute));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string CurrentStatus_UsingLockedRoute => Get(nameof(CurrentStatus_UsingLockedRoute));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string CurrentStatus_PlanningRoute => Get(nameof(CurrentStatus_PlanningRoute));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string CurrentStatus_Jumping => Get(nameof(CurrentStatus_Jumping));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string CurrentStatus_ExploringPlanet => Get(nameof(CurrentStatus_ExploringPlanet));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string CurrentStatus_Loitering => Get(nameof(CurrentStatus_Loitering));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Styles_TextBoxPlaceholder_Any => Get(nameof(Styles_TextBoxPlaceholder_Any));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Status_Unknown => Get(nameof(Status_Unknown));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Status_Unexplored => Get(nameof(Status_Unexplored));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Status_Unscanned => Get(nameof(Status_Unscanned));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Status_Incomplete => Get(nameof(Status_Incomplete));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Status_Complete => Get(nameof(Status_Complete));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string UnitFileSingular => Get(nameof(UnitFileSingular));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string UnitFilePlural => Get(nameof(UnitFilePlural));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string UnitSystemSingular => Get(nameof(UnitSystemSingular));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string UnitSystemPlural => Get(nameof(UnitSystemPlural));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Button_Close => Get(nameof(Button_Close));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string JournalHistoryImport_IntroText_Found => Get(nameof(JournalHistoryImport_IntroText_Found));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string JournalHistoryImport_IntroText_None => Get(nameof(JournalHistoryImport_IntroText_None));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RouteIsLoading_PleaseWait => Get(nameof(RouteIsLoading_PleaseWait));

    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_Welcome => Get(nameof(Speech_Welcome));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_Goodbye => Get(nameof(Speech_Goodbye));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_GeologicalSignals => Get(nameof(Speech_GeologicalSignals));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_BiologicalSignals => Get(nameof(Speech_BiologicalSignals));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_FirstDiscoverySystem => Get(nameof(Speech_FirstDiscoverySystem));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_FirstDiscoveryBody => Get(nameof(Speech_FirstDiscoveryBody));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_Terraformable => Get(nameof(Speech_Terraformable));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_Landable => Get(nameof(Speech_Landable));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_ValuableBody => Get(nameof(Speech_ValuableBody));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_ValuableGenusPredicted => Get(nameof(Speech_ValuableGenusPredicted));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_ValuableGeneraPredicted => Get(nameof(Speech_ValuableGeneraPredicted));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_LeaveClonalColonyRange => Get(nameof(Speech_LeaveClonalColonyRange));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_EnterClonalColonyRange => Get(nameof(Speech_EnterClonalColonyRange));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_MatchingClassificationsFound => Get(nameof(Speech_MatchingClassificationsFound));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_MatchingClassificationFound => Get(nameof(Speech_MatchingClassificationFound));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_Ring => Get(nameof(Speech_Ring));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_RingCount => Get(nameof(Speech_RingCount));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_GeologicalSignalSingular => Get(nameof(Speech_GeologicalSignalSingular));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_GeologicalSignalPlural => Get(nameof(Speech_GeologicalSignalPlural));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_BiologicalSignalSingular => Get(nameof(Speech_BiologicalSignalSingular));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_BiologicalSignalPlural => Get(nameof(Speech_BiologicalSignalPlural));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_PoiCriteriaSetSingular => Get(nameof(Speech_PoiCriteriaSetSingular));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Speech_PoiCriteriaSetPlural => Get(nameof(Speech_PoiCriteriaSetPlural));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RingType_Unknown => Get(nameof(RingType_Unknown));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RingType_MetalRich => Get(nameof(RingType_MetalRich));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RingType_Metallic => Get(nameof(RingType_Metallic));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RingType_Rocky => Get(nameof(RingType_Rocky));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RingType_Icy => Get(nameof(RingType_Icy));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RingReserveLevel_Unknown => Get(nameof(RingReserveLevel_Unknown));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RingReserveLevel_Pristine => Get(nameof(RingReserveLevel_Pristine));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RingReserveLevel_Major => Get(nameof(RingReserveLevel_Major));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RingReserveLevel_Common => Get(nameof(RingReserveLevel_Common));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RingReserveLevel_Low => Get(nameof(RingReserveLevel_Low));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RingReserveLevel_Depleted => Get(nameof(RingReserveLevel_Depleted));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_Ring => Get(nameof(SpeechOutput_Ring));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_RingCount => Get(nameof(SpeechOutput_RingCount));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_Welcome => Get(nameof(SpeechOutput_Welcome));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_Goodbye => Get(nameof(SpeechOutput_Goodbye));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_GeologicalSignals => Get(nameof(SpeechOutput_GeologicalSignals));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_BiologicalSignals => Get(nameof(SpeechOutput_BiologicalSignals));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_FirstDiscoverySystem => Get(nameof(SpeechOutput_FirstDiscoverySystem));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_FirstDiscoveryBody => Get(nameof(SpeechOutput_FirstDiscoveryBody));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_Terraformable => Get(nameof(SpeechOutput_Terraformable));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_Landable => Get(nameof(SpeechOutput_Landable));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_ValuableBody => Get(nameof(SpeechOutput_ValuableBody));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_ValuableGenusPredicted => Get(nameof(SpeechOutput_ValuableGenusPredicted));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_ValuableGeneraPredicted => Get(nameof(SpeechOutput_ValuableGeneraPredicted));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_LeaveClonalColonyRange => Get(nameof(SpeechOutput_LeaveClonalColonyRange));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_EnterClonalColonyRange => Get(nameof(SpeechOutput_EnterClonalColonyRange));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_MatchingClassificationFound => Get(nameof(SpeechOutput_MatchingClassificationFound));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string SpeechOutput_MatchingClassificationsFound => Get(nameof(SpeechOutput_MatchingClassificationsFound));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string UnitLightYears => Get(nameof(UnitLightYears));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string UnitLightSeconds => Get(nameof(UnitLightSeconds));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string UnitMeters => Get(nameof(UnitMeters));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string UnitKilometers => Get(nameof(UnitKilometers));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string WordOf => Get(nameof(WordOf));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PlanetOfInterest_NearbyHMC => Get(nameof(PlanetOfInterest_NearbyHMC));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PlanetOfInterest_HighGravityLandable => Get(nameof(PlanetOfInterest_HighGravityLandable));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PlanetOfInterest_GuardianRuins => Get(nameof(PlanetOfInterest_GuardianRuins));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string BodyTypeStar => Get(nameof(BodyTypeStar));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string BodyTypePlanet => Get(nameof(BodyTypePlanet));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TerraformingState_Terraformable => Get(nameof(TerraformingState_Terraformable));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string TerraformingState_CandidateForTerraforming => Get(nameof(TerraformingState_CandidateForTerraforming));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string CommanderNamePrefix => Get(nameof(CommanderNamePrefix));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_PlotterRouteGenerate => Get(nameof(MenuItem_PlotterRouteGenerate));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_PlotterRouteClear => Get(nameof(MenuItem_PlotterRouteClear));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_RouteLock => Get(nameof(MenuItem_RouteLock));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_RouteUnlock => Get(nameof(MenuItem_RouteUnlock));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_RouteImport => Get(nameof(MenuItem_RouteImport));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_RouteExport => Get(nameof(MenuItem_RouteExport));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RouteImport_InvalidFileTitle => Get(nameof(RouteImport_InvalidFileTitle));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RouteImport_InvalidFileMessage => Get(nameof(RouteImport_InvalidFileMessage));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_HudOpen => Get(nameof(MenuItem_HudOpen));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_HudClose => Get(nameof(MenuItem_HudClose));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_HudPassThroughEnable => Get(nameof(MenuItem_HudPassThroughEnable));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string MenuItem_HudPassThroughDisable => Get(nameof(MenuItem_HudPassThroughDisable));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string FeedbackReportIssueMailSubject => Get(nameof(FeedbackReportIssueMailSubject));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string GenusTable_UnknownBiologicalSignalsInfo => Get(nameof(GenusTable_UnknownBiologicalSignalsInfo));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotter_No => Get(nameof(RoutePlotter_No));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string RoutePlotter_Yes => Get(nameof(RoutePlotter_Yes));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string Preferences_ParentPlanetNone => Get(nameof(Preferences_ParentPlanetNone));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Combo_DoesNotMatter => Get(nameof(PreferencesWindow_Combo_DoesNotMatter));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Combo_No => Get(nameof(PreferencesWindow_Combo_No));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Combo_Yes => Get(nameof(PreferencesWindow_Combo_Yes));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Tooltip_ParentPlanetCriteria => Get(nameof(PreferencesWindow_Tooltip_ParentPlanetCriteria));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Info_BodyValueThreshold => Get(nameof(PreferencesWindow_Info_BodyValueThreshold));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Info_SpeciesValueThreshold => Get(nameof(PreferencesWindow_Info_SpeciesValueThreshold));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string PreferencesWindow_Info_BiologicalsViewAltitude => Get(nameof(PreferencesWindow_Info_BiologicalsViewAltitude));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string WordStar => Get(nameof(WordStar));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string StatusBodiesExploredOfTotal => Get(nameof(StatusBodiesExploredOfTotal));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string StatusNonBodyBelts => Get(nameof(StatusNonBodyBelts));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string StatusScanMissing => Get(nameof(StatusScanMissing));
    /// <summary>Gets the localized string for this resource.</summary>
    /// <value>The resource string value.</value>
    public static string StatusBodiesMissing => Get(nameof(StatusBodiesMissing));
}