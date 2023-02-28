
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Interfaces;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;
using Negin.Framework.Utilities;
using Negin.Infra.Data.Sql.Migrations;
using Negin.Infrastructure;
using System.Net.Http;
using static Negin.Framework.Exceptions.SqlException;
using SqlException = Negin.Framework.Exceptions.SqlException;

namespace Negin.Infra.Data.Sql.EFRepositories;

public class ShippingLineCompanyRepository : IShippingLineCompanyRepository
{
    private readonly NeginDbContext _neginDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ShippingLineCompanyRepository(NeginDbContext neginDbContext, IHttpContextAccessor httpContextAccessor)
    {
        _neginDbContext = neginDbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<SqlException> CreateShippingLineAsync(ShippingLineCompany shippingLine, IList<uint> AgentAssignedIds)
    {
        var result = new SqlException();
        if (shippingLine.IsOwner && !shippingLine.IsAgent && AgentAssignedIds == null)
        {
            result.SqlState = SqlExceptionState.AgentIsEmpty;
            result.Message = "this company must has at least one agent! because it`s type is owner.";
        }
        else
        {
			try
            {
                shippingLine = (ShippingLineCompany)Util.TrimAllStringFields(shippingLine);
                shippingLine.CreatedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId();
                shippingLine.CreateDate = DateTime.Now;
                await _neginDbContext.ShippingLines.AddAsync(shippingLine);
                _neginDbContext.SaveChanges();
                if (shippingLine.IsOwner && shippingLine.IsAgent)
                {
                    _neginDbContext.AgentsShippingLine.Add(new AgentShippingLine()
                    {
                        ShippingLineCompanyId = shippingLine.Id,
                        AgentShippingLineCompanyId = shippingLine.Id
                    });
                    await _neginDbContext.SaveChangesAsync();
                }
                else if (shippingLine.IsOwner && !shippingLine.IsAgent && AgentAssignedIds != null)
                {
                    foreach (var agentId in AgentAssignedIds)
                    {
						_neginDbContext.AgentsShippingLine.Add(new AgentShippingLine()
						{
							ShippingLineCompanyId = shippingLine.Id,
							AgentShippingLineCompanyId = agentId
						});
					}
					await _neginDbContext.SaveChangesAsync();
				}
                result.State = true;
            }
            catch (UniqueConstraintException ex)
            {
                var oldShippingline = _neginDbContext.ShippingLines.Single(c => c.ShippingLineName == shippingLine.ShippingLineName);

                if (oldShippingline.IsDelete)
                {
                    _neginDbContext.ShippingLines.Remove(oldShippingline);
                    _neginDbContext.SaveChanges();
                    result.State = true;
                }
                else
                {
                    result.SqlState = SqlExceptionState.DuplicateName;
                    result.Message = "This shipping line name is exist. please enter another name";
                }
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = ex.Message;
            }
        }

        return result;
    }

    public async Task<ShippingLineCompany> GetShippingLineAsync(uint id)
    {
        return await _neginDbContext.ShippingLines.Include(c => c.Agents).ThenInclude(c=>c.AgentShippingLineCompany).AsNoTracking().SingleAsync(p => p.Id == id);
    }

    public async Task<PagedData<ShippingLineCompany>> GetPaginationShippingLineCompaniesAsync(int pageNumber = 1, int pageSize = 10, string filter = "")
    {
        bool noFilter = string.IsNullOrWhiteSpace(filter);

        var shippingLines = _neginDbContext.ShippingLines.Include(c => c.Agents).ThenInclude(c=>c.AgentShippingLineCompany).AsNoTracking()
            .Where(x => x.IsDelete == false)
            .Where(c => noFilter
                || c.ShippingLineName.Contains(filter)
                || c.City.Contains(filter)
                || c.Email.Contains(filter)
                || c.NationalCode.Contains(filter));

        PagedData<ShippingLineCompany> result = new PagedData<ShippingLineCompany>()
        {
            PageInfo = new()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = await shippingLines.CountAsync()
            },
            Data = await shippingLines.OrderBy(c => c.ShippingLineName).ToPagination(pageNumber, pageSize).ToListAsync()
        };

        return result;
    }

    public async Task<SqlException> EditShippingLine(ShippingLineCompany newShippingLine, IList<uint> AgentAssignedIds)
    {
		var result = new SqlException() { State = false };
        ShippingLineCompany shippingline;
		if (newShippingLine.IsOwner && !newShippingLine.IsAgent && AgentAssignedIds == null)
		{
			result.SqlState = SqlExceptionState.AgentIsEmpty;
			result.Message = "this company must has at least one agent! because it`s type is owner.";
		}
		else
		{
			shippingline = _neginDbContext.ShippingLines.Single(p => p.Id == newShippingLine.Id);
			newShippingLine = (ShippingLineCompany)Util.TrimAllStringFields(newShippingLine);
			if (shippingline != null)
			{
				shippingline.ShippingLineName = newShippingLine.ShippingLineName;
				shippingline.EconomicCode = newShippingLine.EconomicCode;
				shippingline.Tel = newShippingLine.Tel;
				shippingline.Email = newShippingLine.Email;
				shippingline.Address = newShippingLine.Address;
				shippingline.City = newShippingLine.City;
                shippingline.NationalCode = newShippingLine.NationalCode;
				shippingline.Description = newShippingLine.Description;
				shippingline.ModifiedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId();
				shippingline.ModifiedDate = DateTime.Now;
                shippingline.IsAgent = newShippingLine.IsAgent;
                shippingline.IsOwner = newShippingLine.IsOwner;

				try
				{
					_neginDbContext.ShippingLines.Update(shippingline);
					await UpdateAgentsAsync(AgentAssignedIds, shippingline);
					_neginDbContext.SaveChanges();
					result.State = true;
				}
				catch (UniqueConstraintException ex)
				{
					var oldShippingline = _neginDbContext.ShippingLines.Single(c => c.ShippingLineName == shippingline.ShippingLineName);

					if (oldShippingline.IsDelete)
					{
						_neginDbContext.ShippingLines.Remove(oldShippingline);
						_neginDbContext.SaveChanges();
						result.State = true;
					}
					else
					{
						result.SqlState = SqlExceptionState.DuplicateName;
						result.Message = "This shipping line name is exist. please enter another name";
					}
				}
				catch (Exception ex)
				{
					result.State = false;
					result.Message = ex.Message;
				}
			}
		}

        return result;
    }

	private async Task UpdateAgentsAsync(IList<uint> AgentAssignedIds, ShippingLineCompany shippingline)
	{
		if (shippingline.IsOwner && shippingline.IsAgent)
		{
			var agentShippingline = await _neginDbContext.AgentsShippingLine.Where(c => c.ShippingLineCompanyId == shippingline.Id).ToListAsync();
			_neginDbContext.RemoveRange(agentShippingline);
			_neginDbContext.AgentsShippingLine.Add(new AgentShippingLine()
			{
				ShippingLineCompanyId = shippingline.Id,
				AgentShippingLineCompanyId = shippingline.Id
			});
			await _neginDbContext.SaveChangesAsync();
		}
		else if (shippingline.IsOwner && !shippingline.IsAgent && AgentAssignedIds != null)
		{
			var agentShippingline = await _neginDbContext.AgentsShippingLine.Where(c => c.ShippingLineCompanyId == shippingline.Id).ToListAsync();
			_neginDbContext.RemoveRange(agentShippingline);

			foreach (var agentId in AgentAssignedIds)
			{
				_neginDbContext.AgentsShippingLine.Add(new AgentShippingLine()
				{
					ShippingLineCompanyId = shippingline.Id,
					AgentShippingLineCompanyId = agentId
				});
			}
			await _neginDbContext.SaveChangesAsync();
        }
        else if (!shippingline.IsOwner && shippingline.IsAgent)
        {
			var agentShippingline = await _neginDbContext.AgentsShippingLine.Where(c => c.ShippingLineCompanyId == shippingline.Id).ToListAsync();
			_neginDbContext.RemoveRange(agentShippingline);

			await _neginDbContext.SaveChangesAsync();
		}
	}

	public void DeleteShippingLine(ulong id)
    {
        try
        {
            var shippingline = _neginDbContext.ShippingLines.Single(c => c.Id == id);
            shippingline.IsDelete = true;
            _neginDbContext.ShippingLines.Update(shippingline);
            _neginDbContext.SaveChanges();
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public async Task<IList<ShippingLineCompany>> GetAgentsAsync()
    {
        return await _neginDbContext.ShippingLines.AsNoTracking().Where(c => c.IsAgent == true && c.IsOwner == false).ToListAsync();
    }

	public async Task<IList<ShippingLineCompany>> GetOwnersAsync()
	{
		return await _neginDbContext.ShippingLines.AsNoTracking().Where(c => c.IsOwner == true).ToListAsync();
	}

	public async Task<IList<ShippingLineCompany>> GetAgentsOfOwnerAsync(uint ownerId)
	{
        var agentIds = await _neginDbContext.AgentsShippingLine.Where(c => c.ShippingLineCompanyId == ownerId).Select(c => c.AgentShippingLineCompanyId).ToListAsync();
        return await _neginDbContext.ShippingLines.Include(c => c.Agents).AsNoTracking().Where(c => agentIds.Contains(c.Id)).ToListAsync();
	}
}
