using Negin.Core.Domain.Entities;
using Negin.Core.Domain.Entities.Basic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Negin.Core.Domain.Entities.Operation;
public class LoadingDischarge : BaseAudit_SoftDeleteable_Entity<ulong>
{
    public ulong VesselStoppageId { get; set; }
    public VesselStoppage? VesselStoppage { get; set; }
    public LoadingDischargeMethod Method { get; set; }
    public double Tonage { get; set; }
    public int LoadingDischargeTariffDetailId { get; set; }
    public LoadingDischargeTariffDetails? LoadingDischargeTariffDetail { get; set; }
    public bool HasCrane { get; set; }
    public bool HasInventory { get; set; }

    [NotMapped]
    public LoadingDischargeStatus? Status { get; set; }

    public enum LoadingDischargeMethod
    {
        LOAD = 0,
        DISC = 1
    }

    public enum LoadingDischargeStatus
    {
        notInvoiced = 0,
        Invoiced = 1
    }

}
