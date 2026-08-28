namespace EDEA.Models;

public class Status
{
    public int? GuiFocus { get; set; }
    public ulong Flags { get; set; }
    public ulong Flags2 { get; set; }
    public string? Ship { get; set; }
    public string? ShipIdent { get; set; }
    public double FuelMain { get; set; }
    public double FuelReservoir { get; set; }
    public string? LegalState { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Altitude { get; set; }
    public string? BodyName { get; set; }
    public string? SystemName { get; set; }
}
