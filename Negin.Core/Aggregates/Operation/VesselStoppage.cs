using Negin.Core.Domain.Aggregates.Basic;
using Negin.Core.Domain.Aggregates.Billing;
using Negin.Core.Domain.Entities;
using Negin.Core.Domain.Entities.Basic;
using Negin.Framework.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Negin.Core.Domain.Aggregates.Operation;

public class VesselStoppage : BaseAuditableEntity<ulong>
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

    public virtual VesselStoppageInvoiceDetail? VesselStoppageInvoiceDetail { get; set; }
    public virtual CleaningServiceInvoiceDetail? CleaningServiceInvoiceDetail { get; set; }

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
    [NotMapped]
    public VesselStoppageStatus? Status { get; set; }
    [NotMapped]
    public int Percentage { get; set; }

    public void SetStatus()
    {
        if (ATA != null && ATD == null)
        {
            Status = VesselStoppage.VesselStoppageStatus.InProcess;
            if (ETD != null)
            {
                var now = DateTime.Now.MiladiToShamsi();
                if (ETD > now)
                {
                    var diff = ETD.Value.DayOfYear - ATA.Value.DayOfYear;

                    var diffNowDays = now.Value.DayOfYear - ATA.Value.DayOfYear;
                    Percentage = (diffNowDays * 100) / diff == 100 ? 99 : (diffNowDays * 100) / diff;
                }
                else
                {
                    Percentage = 80;
                }
            }
            else
            {
                Percentage = 20;
            }
        }
        else if (ATA != null && ATD != null)
        {
            Status = VesselStoppage.VesselStoppageStatus.Gone;
            Percentage = 100;
        }
        else if (ATA == null && ATD == null)
        {
            Status = VesselStoppage.VesselStoppageStatus.WaitForVessel;
        }
        if (VesselStoppageInvoiceDetail != null && VesselStoppageInvoiceDetail.Invoice?.Status != Invoice.InvoiceStatus.IsCancel)
        {
            Status = VesselStoppage.VesselStoppageStatus.Invoiced;
        }
    }

    public enum VesselStoppageStatus
    {
        WaitForVessel = 0, InProcess = 1, Gone = 2, Invoiced = 3
	}
}

