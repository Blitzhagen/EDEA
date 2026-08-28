using System;

namespace EDEA.Models;

public class Ring
{
    public string Name { get; set; } = string.Empty;

    public long StarSystemId { get; set; }

    public int BodyId { get; set; }

    public RingType Type { get; set; }

    public long Mass { get; set; }

    public long InnerRadius { get; set; }

    public long OuterRadius { get; set; }

    public long Width { get; set; }

    public double Density { get; set; }

    public Ring()
    {
    }

    public Ring(string name, long starSystemId, int bodyId, RingType type, long mass, long innerRadius, long outerRadius)
    {
        Name = name;
        StarSystemId = starSystemId;
        BodyId = bodyId;
        Type = type;
        Mass = mass;
        InnerRadius = innerRadius;
        OuterRadius = outerRadius;
        Width = OuterRadius - InnerRadius;
        double outerArea = Math.PI * (Convert.ToDouble(OuterRadius) * Convert.ToDouble(OuterRadius) - Convert.ToDouble(InnerRadius) * Convert.ToDouble(InnerRadius));
        double density = outerArea > 0.0 ? Convert.ToDouble(Mass) * 1000000.0 / outerArea : 0.0;
        Density = double.IsFinite(density) ? density : 0.0;
    }

    public Ring(string name, long bodyId, long starSystemId, long type, long mass, long innerRadius, long outerRadius, long width, double density)
    {
        Name = name;
        StarSystemId = starSystemId;
        BodyId = Convert.ToInt32(bodyId);
        Type = (RingType)type;
        Mass = mass;
        InnerRadius = innerRadius;
        OuterRadius = outerRadius;
        Width = width;
        Density = double.IsFinite(density) ? density : 0.0;
    }
}
