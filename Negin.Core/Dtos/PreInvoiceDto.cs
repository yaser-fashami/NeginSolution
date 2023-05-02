using Negin.Core.Domain.Aggregates.Basic;
namespace Negin.Core.Domain.Dtos;

public class PreInvoiceDto
{
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public ulong VoyageId { get; set; }
    public Voyage Voyage { get; set; }
    public string CurrentUser { get; set; }
    public string CurrentUserEmail { get; set; }
    public IList<PreInvoiceDetailDto> PreInvoiceDetails { get; set; }
    public uint TotalDwellingHour { get; set; }
    public uint TotalDwellingDays { get; set; }
    public ulong SumPriceR { get; set; }
    public decimal SumPriceD { get; set; }
    public ulong SumPriceRVat { get; set; }
}
