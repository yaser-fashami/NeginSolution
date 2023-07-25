using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Negin.Infrastructure;
using Negin.Services.Billing;

namespace Negin.ServicesTest;

public class InvoiceCalculatorTest
{
    private readonly IInvoiceCalculator _invoiceCalculator;
    public InvoiceCalculatorTest()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("localhost");
        context.Request.Path = new PathString("/path");
        var accessor = new HttpContextAccessor();
        accessor.HttpContext = context;

        var options = new DbContextOptionsBuilder<NeginDbContext>()
            .UseSqlServer("Server=.; Initial Catalog=NeginDB; User ID=sa; Password=1qaz!QAZ; Encrypt=false;")
            .Options;

        var ctx = new NeginDbContext(options);
        _invoiceCalculator = new InvoiceCalculator(ctx, accessor);

    }

    [Fact]
    public void CalculateAsyncTest()
    {
        List<ulong> vesselStoppagesId = new List<ulong>()
        {
            10008
        };
        var result = _invoiceCalculator.CalculateAsync(20004, vesselStoppagesId).Result;

        Assert.Equal((uint)83, result.TotalDwellingHour);
        Assert.Equal((decimal)554.78, result.SumPriceD);
    }
}