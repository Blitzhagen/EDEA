namespace EDEA.ViewModels;

/// <summary>
/// Simple data transfer object for genus information used in the UI.
/// </summary>
public class GenusView
{
    /// <summary>
    /// Gets or sets the name of the body the genus is found on.
    /// </summary>
    /// <value>The body name.</value>
    public string BodyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the genus name.
    /// </summary>
    /// <value>The genus name.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Vista Genomics value.
    /// </summary>
    /// <value>The Vista Genomics value.</value>
    public decimal VistaGenomicsValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the genus is analysed.
    /// </summary>
    /// <value><c>true</c> if the genus is analysed; otherwise, <c>false</c>.</value>
    public bool IsAnalysed { get; set; }

    /// <summary>
    /// Gets or sets the clonal colony range in meters.
    /// </summary>
    /// <value>The clonal colony range.</value>
    public double? ClonalColonyRange { get; set; }
}
