using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Interfaces;
using Negin.Infra.Data.Sql.EFRepositories;
using Negin.Infrastructure;

namespace Negin.Services.BasicInfo;
public class BasicInfoService : IBasicInfoService
{
    private readonly NeginDbContext _neginDbContext;
    private readonly IBasicInfoRepository _basicInfoRepository;

    public BasicInfoService(NeginDbContext neginDbContext, IBasicInfoRepository basicInfoRepository)
    {
        _neginDbContext = neginDbContext;
        _basicInfoRepository = basicInfoRepository;
    }

    public async Task<List<LoadingDischargeTariffDetails>> GetLastLoadingDischargeTariffDetailAsync()
    {
        var lastLoadingDischargeTariffId = await _neginDbContext.LoadingDischargeTariffs.OrderByDescending(x => x.EffectiveDate).Select(c => c.Id).FirstAsync();

        return await _neginDbContext.LoadingDischargeTariffDetails
                            .Include(c => c.LoadingDischargeTariff)
                            .Where(c => c.LoadingDischargeTariffId == lastLoadingDischargeTariffId)
                            .ToListAsync();


    }

    public JsonResult AddLoadingDischargeTariffDetail(List<LoadingDischargeTariffDetails> loadingDischargeTariffDetails, string description, DateTime effectiveDate)
    {
        using var transaction = _neginDbContext.Database.BeginTransaction();

        var loadingDischargeTariff = _basicInfoRepository.CreateLoadingDischargeTariffAsync(new LoadingDischargeTariff { Description = description, EffectiveDate = effectiveDate }).Result;
        if (!loadingDischargeTariff.State)
        {
            transaction.Rollback();
            return new JsonResult(loadingDischargeTariff.Message);
        }
        else
        {
            loadingDischargeTariffDetails.ForEach(c => c.LoadingDischargeTariffId = (int)loadingDischargeTariff.sqlResult);

            var result = _basicInfoRepository.CreateLoadingDischargeTariffDetailsAsync(loadingDischargeTariffDetails).Result;
            if (!result.State)
            {
                transaction.Rollback();
                return new JsonResult(result.Message);
            }
        }
        transaction.Commit();
        return new JsonResult(loadingDischargeTariff.sqlResult);

    }
}
