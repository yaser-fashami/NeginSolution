using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Negin.Framework.Pagination;

public static class PaginationExtention
{
    public static IQueryable<TEntity> ToPagination<TEntity>(this IQueryable<TEntity> query, int page, int count)
    {
        return page == 1 ? query.Take(count) : query.Skip((page - 1) * count).Take(count);
    }
}
