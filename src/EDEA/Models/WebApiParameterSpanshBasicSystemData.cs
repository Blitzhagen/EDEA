using System.Collections.ObjectModel;

namespace EDEA.Models;

/// <summary>
/// Represents the parameters for a Spansh basic system data web API request.
/// </summary>
public class WebApiParameterSpanshBasicSystemData : WebApiParameter
{
    /// <summary>
    /// Gets the collection of star systems.
    /// </summary>
    /// <value>The star systems.</value>
    public ObservableCollection<StarSystem> StarSystems { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiParameterSpanshBasicSystemData"/> class.
    /// </summary>
    /// <param name="starSystems">The star systems.</param>
    public WebApiParameterSpanshBasicSystemData(ObservableCollection<StarSystem> starSystems)
    {
        StarSystems = starSystems;
    }
}
