using Negin.Core.Domain.Aggregates.Billing;
using Negin.Core.Domain.Aggregates.Operation;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.Core.Domain.Dtos;

public class PreInvoiceDetailDto
{
    public VesselStoppage VesselStoppage { get; set; }
    public VesselStoppageInvoiceDetail VesselStoppageInvoiceDetail { get; set; }
    public CleaningServiceInvoiceDetail CleaningServiceInvoiceDetail { get; set; }
    public byte? VATPercent { get; set; }
    public string StoppageIssuedBy { get; set; }
    public Currency Currency { get; set; }
    public byte? DiscountPercent { get; set; }
}
