using EDEA.Models;

namespace EDEA.ViewModels;

public class RingViewModel : ViewModelBase
{
    public string Name { get; }

    public string Type { get; }

    public string Mass { get; }

    public string InnerRadius { get; }

    public string OuterRadius { get; }

    public string Width { get; }

    public string Density { get; }

    public RingViewModel(Ring ring, string bodyName)
    {
        Name = ring.Name.Replace(bodyName, string.Empty).Replace("Ring", string.Empty).Trim();
        Type = Helpsters.GetRingTypeSourceDesciption(DataSource.Edsm, ring.Type);
        Mass = $"{ring.Mass:n0} Mt";
        InnerRadius = $"{ring.InnerRadius / 1000:n0} km";
        OuterRadius = $"{ring.OuterRadius / 1000:n0} km";
        Width = $"{ring.Width / 1000:n0} km";
        Density = $"{ring.Density:n2} Mt/km²";
    }
}
