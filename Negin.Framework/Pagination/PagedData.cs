using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negin.Framework.Pagination;

public class PagedData<TKey>
{
    public List<TKey> Data { get; set; }
    public PageInfo PageInfo { get; set; }
}
