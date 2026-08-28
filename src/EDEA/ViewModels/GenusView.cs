namespace EDEA.ViewModels;

public class GenusView
{
    public string BodyName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal VistaGenomicsValue { get; set; }
    public bool IsAnalysed { get; set; }
    public double? ClonalColonyRange { get; set; }
}
