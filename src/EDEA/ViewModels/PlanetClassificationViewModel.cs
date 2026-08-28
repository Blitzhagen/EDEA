using System;
using System.Collections.Generic;
using System.Reflection;
using EDEA.Models;
using log4net;

using EDEA.Enums;

namespace EDEA.ViewModels;

public class PlanetClassificationViewModel : ViewModelBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(PlanetClassificationViewModel));

    private int stringListStringMaxCount = 3;

    public PlanetClassification PlanetClassification { get; }

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

    public string PlanetClassesAsString => generateStringListViewString(PlanetClasses);

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

    public string AtmospheresAsString => generateStringListViewString(Atmospheres);

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

    public string VolcanismsAsString => generateStringListViewString(Volcanisms);

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

    public string StarClassesAsString => generateStringListViewString(StarClasses);

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

    public string RingTypesAsString => generateStringListViewString(RingTypes);

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

    public string RingReserveLevelsAsString => generateStringListViewString(RingReserveLevels);

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

    public PlanetClassificationViewModel(PlanetClassification planetClassification)
    {
        PlanetClassification = planetClassification;
    }

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

    private string generateStringListViewString(List<string> stringList)
    {
        if (stringList.Count <= stringListStringMaxCount)
        {
            return string.Join(", ", stringList);
        }
        return $"{string.Join(", ", stringList.ToArray(), 0, stringListStringMaxCount)} and {stringList.Count - stringListStringMaxCount} more";
    }
}
