using Microsoft.AspNetCore.Mvc;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.Services.BasicInfo;
public interface IBasicInfoService
{
    Task<List<LoadingDischargeTariffDetails>> GetLastLoadingDischargeTariffDetailAsync();

    JsonResult AddLoadingDischargeTariffDetail(List<LoadingDischargeTariffDetails> loadingDischargeTariffDetails, string description, DateTime effectiveDate);
}
