
using Negin.Core.Domain.Entities.Basic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Negin.Core.Domain.Entities.Billing;

public class VesselStoppage: BaseEntity
{
    public ulong VoyageId { get; set; }
    public Voyage? Voyage { get; set; }
    public DateTime? ETA { get; set; }
    public DateTime? ATA { get; set; }
    public DateTime? ETD { get; set; }
    public DateTime? ATD { get; set; }
    public uint? OriginPortId { get; set; }
    public Port? OriginPort { get; set; }
    public uint? PreviousPortId { get; set; }
    public Port? PreviousPort { get; set; }
    public uint? NextPortId { get; set; }
    public Port? NextPort { get; set; }

    [NotMapped]
    public string? ETATime { get; set; }
    [NotMapped]
    public string? ATATime { get; set; }
    [NotMapped]
    public string? ETDTime { get; set; }
    [NotMapped]
    public string? ATDTime { get; set; }
    [NotMapped]
    public DayOfWeek? ETADayOfTheWeek { get; set; }
    [NotMapped]
    public DayOfWeek? ATADayOfTheWeek { get; set; }
    [NotMapped]
    public DayOfWeek? ETDDayOfTheWeek { get; set; }
    [NotMapped]
    public DayOfWeek? ATDDayOfTheWeek { get; set; }
}

