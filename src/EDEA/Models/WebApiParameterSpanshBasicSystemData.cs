using System.Collections.ObjectModel;

namespace EDEA.Models;

public class WebApiParameterSpanshBasicSystemData : WebApiParameter
{
    public ObservableCollection<StarSystem> StarSystems { get; }

    public WebApiParameterSpanshBasicSystemData(ObservableCollection<StarSystem> starSystems)
    {
        StarSystems = starSystems;
    }
}
