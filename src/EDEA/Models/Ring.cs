using System;

namespace EDEA.Models;

/// <summary>
/// Represents a planetary or stellar ring.
/// </summary>
public class Ring
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The ring name.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the star system identifier.
    /// </summary>
    /// <value>The star system identifier.</value>
    public long StarSystemId { get; set; }

    /// <summary>
    /// Gets or sets the body identifier.
    /// </summary>
    /// <value>The body identifier.</value>
    public int BodyId { get; set; }

    /// <summary>
    /// Gets or sets the ring type.
    /// </summary>
    /// <value>The ring type.</value>
    public RingType Type { get; set; }

    /// <summary>
    /// Gets or sets the mass.
    /// </summary>
    /// <value>The ring mass.</value>
    public long Mass { get; set; }

    /// <summary>
    /// Gets or sets the inner radius.
    /// </summary>
    /// <value>The inner radius.</value>
    public long InnerRadius { get; set; }

    /// <summary>
    /// Gets or sets the outer radius.
    /// </summary>
    /// <value>The outer radius.</value>
    public long OuterRadius { get; set; }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    /// <value>The ring width.</value>
    public long Width { get; set; }

    /// <summary>
    /// Gets or sets the density.
    /// </summary>
    /// <value>The ring density.</value>
    public double Density { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ring"/> class.
    /// </summary>
    public Ring()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Ring"/> class and calculates width and density.
    /// </summary>
    /// <param name="name">The ring name.</param>
    /// <param name="starSystemId">The star system identifier.</param>
    /// <param name="bodyId">The body identifier.</param>
    /// <param name="type">The ring type.</param>
    /// <param name="mass">The ring mass.</param>
    /// <param name="innerRadius">The inner radius.</param>
    /// <param name="outerRadius">The outer radius.</param>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="Ring"/> class from persisted data.
    /// </summary>
    /// <param name="name">The ring name.</param>
    /// <param name="bodyId">The body identifier.</param>
    /// <param name="starSystemId">The star system identifier.</param>
    /// <param name="type">The ring type as a numeric value.</param>
    /// <param name="mass">The ring mass.</param>
    /// <param name="innerRadius">The inner radius.</param>
    /// <param name="outerRadius">The outer radius.</param>
    /// <param name="width">The ring width.</param>
    /// <param name="density">The ring density.</param>
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
