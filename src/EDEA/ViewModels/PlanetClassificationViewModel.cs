using System;
using System.Collections.Generic;
using System.Reflection;
using EDEA.Models;
using log4net;

using EDEA.Enums;

namespace EDEA.ViewModels;

/// <summary>
/// View model that wraps a <see cref="PlanetClassification"/> for editing and display.
/// </summary>
public class PlanetClassificationViewModel : ViewModelBase
{
    /// <summary>
    /// Logger instance for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(PlanetClassificationViewModel));

    /// <summary>
    /// The maximum number of items to display in a string list preview.
    /// </summary>
    private int stringListStringMaxCount = 3;

    /// <summary>
    /// Gets the underlying planet classification model.
    /// </summary>
    /// <value>The planet classification model.</value>
    public PlanetClassification PlanetClassification { get; }

    /// <summary>
    /// Gets or sets the identifier of the classification.
    /// </summary>
    /// <value>The identifier.</value>
    public string Id
    {
        get
        {
            return PlanetClassification.Id;
        }
        set
        {
            PlanetClassification.Id = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the classification is active.
    /// </summary>
    /// <value><c>true</c> if the classification is active; otherwise, <c>false</c>.</value>
    public bool IsActive
    {
        get
        {
            return PlanetClassification.IsActive;
        }
        set
        {
            PlanetClassification.IsActive = value;
        }
    }

    /// <summary>
    /// Gets or sets the name of the classification.
    /// </summary>
    /// <value>The classification name.</value>
    public string Name
    {
        get
        {
            return PlanetClassification.Name;
        }
        set
        {
            PlanetClassification.Name = value;
        }
    }

    /// <summary>
    /// Gets or sets the list of planet classes.
    /// </summary>
    /// <value>The planet classes.</value>
    public List<string> PlanetClasses
    {
        get
        {
            return PlanetClassification.PlanetClasses;
        }
        set
        {
            PlanetClassification.PlanetClasses = value;
            OnPropertyChanged("PlanetClassesAsString");
        }
    }

    /// <summary>
    /// Gets the planet classes as a readable string.
    /// </summary>
    /// <value>The planet classes string.</value>
    public string PlanetClassesAsString => generateStringListViewString(PlanetClasses);

    /// <summary>
    /// Gets or sets the list of atmospheres.
    /// </summary>
    /// <value>The atmospheres.</value>
    public List<string> Atmospheres
    {
        get
        {
            return PlanetClassification.Atmospheres;
        }
        set
        {
            PlanetClassification.Atmospheres = value;
            OnPropertyChanged("AtmospheresAsString");
        }
    }

    /// <summary>
    /// Gets the atmospheres as a readable string.
    /// </summary>
    /// <value>The atmospheres string.</value>
    public string AtmospheresAsString => generateStringListViewString(Atmospheres);

    /// <summary>
    /// Gets or sets the list of volcanisms.
    /// </summary>
    /// <value>The volcanisms.</value>
    public List<string> Volcanisms
    {
        get
        {
            return PlanetClassification.Volcanisms;
        }
        set
        {
            PlanetClassification.Volcanisms = value;
            OnPropertyChanged("VolcanismsAsString");
        }
    }

    /// <summary>
    /// Gets the volcanisms as a readable string.
    /// </summary>
    /// <value>The volcanisms string.</value>
    public string VolcanismsAsString => generateStringListViewString(Volcanisms);

    /// <summary>
    /// Gets or sets the list of star classes.
    /// </summary>
    /// <value>The star classes.</value>
    public List<string> StarClasses
    {
        get
        {
            return PlanetClassification.StarClasses;
        }
        set
        {
            PlanetClassification.StarClasses = value;
            OnPropertyChanged("StarClassesAsString");
        }
    }

    /// <summary>
    /// Gets the star classes as a readable string.
    /// </summary>
    /// <value>The star classes string.</value>
    public string StarClassesAsString => generateStringListViewString(StarClasses);

    /// <summary>
    /// Gets or sets the list of ring types.
    /// </summary>
    /// <value>The ring types.</value>
    public List<string> RingTypes
    {
        get
        {
            return PlanetClassification.RingTypes;
        }
        set
        {
            PlanetClassification.RingTypes = value;
            OnPropertyChanged("RingTypesAsString");
        }
    }

    /// <summary>
    /// Gets the ring types as a readable string.
    /// </summary>
    /// <value>The ring types string.</value>
    public string RingTypesAsString => generateStringListViewString(RingTypes);

    /// <summary>
    /// Gets or sets the list of ring reserve levels.
    /// </summary>
    /// <value>The ring reserve levels.</value>
    public List<string> RingReserveLevels
    {
        get
        {
            return PlanetClassification.RingReserveLevels;
        }
        set
        {
            PlanetClassification.RingReserveLevels = value;
            OnPropertyChanged("RingReserveLevelsAsString");
        }
    }

    /// <summary>
    /// Gets the ring reserve levels as a readable string.
    /// </summary>
    /// <value>The ring reserve levels string.</value>
    public string RingReserveLevelsAsString => generateStringListViewString(RingReserveLevels);

    /// <summary>
    /// Gets or sets the minimum gravity.
    /// </summary>
    /// <value>The minimum gravity.</value>
    public double? GravityMin
    {
        get
        {
            return PlanetClassification.GravityMin;
        }
        set
        {
            PlanetClassification.GravityMin = value;
        }
    }

    /// <summary>
    /// Gets or sets the maximum gravity.
    /// </summary>
    /// <value>The maximum gravity.</value>
    public double? GravityMax
    {
        get
        {
            return PlanetClassification.GravityMax;
        }
        set
        {
            PlanetClassification.GravityMax = value;
        }
    }

    /// <summary>
    /// Gets or sets the minimum temperature.
    /// </summary>
    /// <value>The minimum temperature.</value>
    public double? TemperatureMin
    {
        get
        {
            return PlanetClassification.TemperatureMin;
        }
        set
        {
            PlanetClassification.TemperatureMin = value;
        }
    }

    /// <summary>
    /// Gets or sets the maximum temperature.
    /// </summary>
    /// <value>The maximum temperature.</value>
    public double? TemperatureMax
    {
        get
        {
            return PlanetClassification.TemperatureMax;
        }
        set
        {
            PlanetClassification.TemperatureMax = value;
        }
    }

    /// <summary>
    /// Gets or sets the minimum distance.
    /// </summary>
    /// <value>The minimum distance.</value>
    public double? DistanceMin
    {
        get
        {
            return PlanetClassification.DistanceMin;
        }
        set
        {
            PlanetClassification.DistanceMin = value;
        }
    }

    /// <summary>
    /// Gets or sets the maximum distance.
    /// </summary>
    /// <value>The maximum distance.</value>
    public double? DistanceMax
    {
        get
        {
            return PlanetClassification.DistanceMax;
        }
        set
        {
            PlanetClassification.DistanceMax = value;
        }
    }

    /// <summary>
    /// Gets or sets the minimum radius in kilometers.
    /// </summary>
    /// <value>The minimum radius in kilometers.</value>
    public double? RadiusMin
    {
        get
        {
            return PlanetClassification.RadiusMin / 1000.0;
        }
        set
        {
            PlanetClassification.RadiusMin = value * 1000.0;
        }
    }

    /// <summary>
    /// Gets or sets the maximum radius in kilometers.
    /// </summary>
    /// <value>The maximum radius in kilometers.</value>
    public double? RadiusMax
    {
        get
        {
            return PlanetClassification.RadiusMax / 1000.0;
        }
        set
        {
            PlanetClassification.RadiusMax = value * 1000.0;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the planet must be landable.
    /// </summary>
    /// <value><c>true</c> if landable is required; otherwise, <c>false</c>.</value>
    public bool? Landable
    {
        get
        {
            return PlanetClassification.Landable;
        }
        set
        {
            PlanetClassification.Landable = value;
        }
    }

    /// <summary>
    /// Gets or sets the minimum ring width in kilometers.
    /// </summary>
    /// <value>The minimum ring width in kilometers.</value>
    public long? RingWidthMin
    {
        get
        {
            return PlanetClassification.RingWidthMin / 1000;
        }
        set
        {
            PlanetClassification.RingWidthMin = value * 1000;
        }
    }

    /// <summary>
    /// Gets or sets the maximum ring width in kilometers.
    /// </summary>
    /// <value>The maximum ring width in kilometers.</value>
    public long? RingWidthMax
    {
        get
        {
            return PlanetClassification.RingWidthMax / 1000;
        }
        set
        {
            PlanetClassification.RingWidthMax = value * 1000;
        }
    }

    /// <summary>
    /// Gets or sets the minimum total rings width in kilometers.
    /// </summary>
    /// <value>The minimum total rings width in kilometers.</value>
    public long? RingsTotalWidthMin
    {
        get
        {
            return PlanetClassification.RingsTotalWidthMin / 1000;
        }
        set
        {
            PlanetClassification.RingsTotalWidthMin = value * 1000;
        }
    }

    /// <summary>
    /// Gets or sets the maximum total rings width in kilometers.
    /// </summary>
    /// <value>The maximum total rings width in kilometers.</value>
    public long? RingsTotalWidthMax
    {
        get
        {
            return PlanetClassification.RingsTotalWidthMax / 1000;
        }
        set
        {
            PlanetClassification.RingsTotalWidthMax = value * 1000;
        }
    }

    /// <summary>
    /// Gets or sets the minimum ring density.
    /// </summary>
    /// <value>The minimum ring density.</value>
    public double? RingDensityMin
    {
        get
        {
            return PlanetClassification.RingDensityMin;
        }
        set
        {
            PlanetClassification.RingDensityMin = value;
        }
    }

    /// <summary>
    /// Gets or sets the maximum ring density.
    /// </summary>
    /// <value>The maximum ring density.</value>
    public double? RingDensityMax
    {
        get
        {
            return PlanetClassification.RingDensityMax;
        }
        set
        {
            PlanetClassification.RingDensityMax = value;
        }
    }

    /// <summary>
    /// Gets or sets the identifier of the parent planet classification.
    /// </summary>
    /// <value>The parent classification identifier.</value>
    public string ParentPlanetClassificationId
    {
        get
        {
            return PlanetClassification.ParentPlanetClassificationId;
        }
        set
        {
            PlanetClassification.ParentPlanetClassificationId = value;
        }
    }

    /// <summary>
    /// Gets or sets the minimum orbital inclination.
    /// </summary>
    /// <value>The minimum orbital inclination.</value>
    public double? OrbitalInclinationMin
    {
        get
        {
            return PlanetClassification.OrbitalInclinationMin;
        }
        set
        {
            PlanetClassification.OrbitalInclinationMin = value;
        }
    }

    /// <summary>
    /// Gets or sets the maximum orbital inclination.
    /// </summary>
    /// <value>The maximum orbital inclination.</value>
    public double? OrbitalInclinationMax
    {
        get
        {
            return PlanetClassification.OrbitalInclinationMax;
        }
        set
        {
            PlanetClassification.OrbitalInclinationMax = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlanetClassificationViewModel"/> class.
    /// </summary>
    /// <param name="planetClassification">The planet classification model.</param>
    public PlanetClassificationViewModel(PlanetClassification planetClassification)
    {
        PlanetClassification = planetClassification;
    }

    /// <summary>
    /// Retrieves the input string list for the specified user-selectable list.
    /// </summary>
    /// <param name="userSelectableInputStringListsKey">The key identifying the list.</param>
    /// <returns>The input string list, or an empty list if the key is invalid.</returns>
    public List<string> GetInputStringListByKey(UserSelectableInputStringListsKey userSelectableInputStringListsKey)
    {
        try
        {
            return GetType().GetProperty(Enum.GetName(typeof(UserSelectableInputStringListsKey), userSelectableInputStringListsKey)!)!.GetValue(this, null) as List<string> ?? new List<string>();
        }
        catch (Exception exception)
        {
            log.Error("Error on getting input string list", exception);
            return new List<string>();
        }
    }

    /// <summary>
    /// Sets the input string list for the specified user-selectable list.
    /// </summary>
    /// <param name="userSelectableInputStringListsKey">The key identifying the list.</param>
    /// <param name="inputStringList">The input string list.</param>
    public void SetInputStringListByKey(UserSelectableInputStringListsKey userSelectableInputStringListsKey, List<string> inputStringList)
    {
        try
        {
            GetType().GetProperty(Enum.GetName(typeof(UserSelectableInputStringListsKey), userSelectableInputStringListsKey)!)!.SetValue(this, inputStringList, null);
        }
        catch (Exception exception)
        {
            log.Error("Error on setting input string list", exception);
        }
    }

    /// <summary>
    /// Generates a readable preview string from a list of strings.
    /// </summary>
    /// <param name="stringList">The string list.</param>
    /// <returns>The preview string.</returns>
    private string generateStringListViewString(List<string> stringList)
    {
        if (stringList.Count <= stringListStringMaxCount)
        {
            return string.Join(", ", stringList);
        }
        return $"{string.Join(", ", stringList.ToArray(), 0, stringListStringMaxCount)} and {stringList.Count - stringListStringMaxCount} more";
    }
}
