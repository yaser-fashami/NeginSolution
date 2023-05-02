
using Negin.Core.Domain.Dtos;
using Negin.Framework.Exceptions;

namespace Negin.Services.Billing;

public interface IInvoiceCalculator
{
    Task<PreInvoiceDto> CalculateAsync(ulong voyageId, IEnumerable<string> vesselStoppages);
    Task<BLMessage> Invoicing(PreInvoiceDto preInvoice);
}
