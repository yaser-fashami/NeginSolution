using Negin.Core.Domain.Entities;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Entities.Operation;

namespace Negin.Core.Domain.Dtos;
public record PreLoadingDischargeInvoiceDto(
											string InvoiceNo,
											DateTime InvoiceDate,
											LoadingDischarge LoadingDiascharge,
											int LoadingDischargeTariffId,
											ShippingLineCompany ShippingLineCompany,
											byte DiscountPercent,
											ulong DiscountAmount,
											uint PerTonPrice,
											ulong LDCostR,
											double Tonage,
											ulong CraneCostR,
											ulong InventoryTariffPrice,
											ulong InventoryCostR,
											ulong TotalCostR,
											VesselStoppage VesselStoppage,
											Vessel Vessel,
											byte VatPercent,
											ulong VatCostR,
											string CurrentUser,
											string CurrentUserEmail
										  );