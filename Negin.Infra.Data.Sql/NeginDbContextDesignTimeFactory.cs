using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Negin.Infrastructure;

namespace Negin.Infra.Data.Sql;

public class NeginDbContextDesignTimeFactory : IDesignTimeDbContextFactory<NeginDbContext>
{
    public NeginDbContext CreateDbContext(string[] args)
    {
        string cnn = @"Server=.; Initial Catalog=NeginDB; User ID=sa; Password=1qaz!QAZ; Encrypt=false;";
        var options = new DbContextOptionsBuilder().UseSqlServer(cnn).Options;
        return new NeginDbContext(options);
    }
}
