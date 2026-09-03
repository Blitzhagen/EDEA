namespace EDEA.Models;

/// <summary>
/// Represents the common classification data of a ship module.
/// </summary>
public class ModuleClassification
{
    /// <summary>
    /// Gets or sets the module identifier.
    /// </summary>
    /// <value>The module identifier.</value>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the module type.
    /// </summary>
    /// <value>The module type.</value>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the module name.
    /// </summary>
    /// <value>The module name.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the module class.
    /// </summary>
    /// <value>The module class.</value>
    public string Class { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the module rating.
    /// </summary>
    /// <value>The module rating.</value>
    public string Rating { get; set; } = string.Empty;
}
