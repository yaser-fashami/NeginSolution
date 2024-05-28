
using Negin.Core.Domain.Dtos;
using Negin.Framework.Exceptions;

namespace Negin.Services.Billing;

public interface IInvoiceCalculator
{
    Task<PreInvoiceDto> CalculateAsync(ulong voyageId, IEnumerable<ulong> vesselStoppages);
    Task<BLMessage> Invoicing(PreInvoiceDto preInvoice);
    Task<PreLoadingDischargeInvoiceDto> CalculateLoadingDischargeInvoiceAsync(LoadingDischargeInvoiceDto loadingDischargeDto);
    Task<BLMessage> LoadingDischargeInvoicing(PreLoadingDischargeInvoiceDto preLoadingDischargeInvoiceDto);
}
