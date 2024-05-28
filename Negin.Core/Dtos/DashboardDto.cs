namespace Negin.Core.Domain.Dtos;

public record DashboardDto
{
    public uint ActiveVoyages { get; set; }
    public uint Gone { get; set; }
    public uint InProcess { get; set; }
    public uint WaitForVessel { get; set; }
    public uint VesselStoppageCount { get; set; }
    public uint Invoiced { get; set; }
    public uint NotInvoiced { get; set; }

}
