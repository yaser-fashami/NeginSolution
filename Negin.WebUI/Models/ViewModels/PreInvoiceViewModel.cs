using Negin.Core.Domain.Dtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace Negin.WebUI.Models.ViewModels;

public class PreInvoiceViewModel : PreInvoiceDto
{
    public PreInvoiceViewModel(PreInvoiceDto preInvoiceDto)
    {
        this.InvoiceNo = preInvoiceDto.InvoiceNo;
        this.InvoiceDate = preInvoiceDto.InvoiceDate;
        this.VoyageId = preInvoiceDto.VoyageId;
        this.Voyage = preInvoiceDto.Voyage;
        this.CurrentUser = preInvoiceDto.CurrentUser;
        this.CurrentUserEmail = preInvoiceDto.CurrentUserEmail;
        this.PreInvoiceDetails = preInvoiceDto.PreInvoiceDetails;
        this.TotalDwellingHour = preInvoiceDto.TotalDwellingHour;
        this.TotalDwellingDays = preInvoiceDto.TotalDwellingDays;
        this.SumPriceR = preInvoiceDto.SumPriceR;
        this.SumPriceD = preInvoiceDto.SumPriceD;
        this.SumPriceRVat = preInvoiceDto.SumPriceRVat;
    }

}
