
namespace Negin.Framework.Pagination;

public class PageInfo
{
    public PageInfo()
    {
        PageNumber = 1;
        PageSize = 10;
    }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public string Title { get; set; }
    public string PageName { get; set; }
    public int PageNumber { get; set; }
    public string Filter { get; set; }
    public string Specification { get; set; }

    public int PageCount => (int)Math.Ceiling((double)TotalCount / PageSize);
}
