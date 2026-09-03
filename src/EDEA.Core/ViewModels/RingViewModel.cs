using EDEA.Models;

namespace EDEA.ViewModels;

/// <summary>
/// View model that wraps a <see cref="Ring"/> for display in the ring table.
/// </summary>
public class RingViewModel : ViewModelBase
{
    /// <summary>
    /// Gets the ring name.
    /// </summary>
    /// <value>The ring name.</value>
    public string Name { get; }

    /// <summary>
    /// Gets the ring type.
    /// </summary>
    /// <value>The ring type.</value>
    public string Type { get; }

    /// <summary>
    /// Gets the formatted ring mass.
    /// </summary>
    /// <value>The ring mass string.</value>
    public string Mass { get; }

    /// <summary>
    /// Gets the formatted inner radius of the ring.
    /// </summary>
    /// <value>The inner radius string.</value>
    public string InnerRadius { get; }

    /// <summary>
    /// Gets the formatted outer radius of the ring.
    /// </summary>
    /// <value>The outer radius string.</value>
    public string OuterRadius { get; }

    /// <summary>
    /// Gets the formatted ring width.
    /// </summary>
    /// <value>The ring width string.</value>
    public string Width { get; }

    /// <summary>
    /// Gets the formatted ring density.
    /// </summary>
    /// <value>The ring density string.</value>
    public string Density { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RingViewModel"/> class.
    /// </summary>
    /// <param name="ring">The ring model to wrap.</param>
    /// <param name="bodyName">The name of the body the ring belongs to.</param>
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
