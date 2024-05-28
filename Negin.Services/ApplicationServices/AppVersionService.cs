using Microsoft.Extensions.Configuration;
using Negin.Core.Domain.Interfaces;
using System.Reflection;

namespace Negin.Services.ApplicationServices;

public class AppVersionService : IAppVersionService
{
    private readonly IConfiguration _configuration;

    public AppVersionService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Version => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version.Substring(0,5);

    public Beneficiary Beneficiary => Enum.Parse<Beneficiary>(_configuration.GetSection("Beneficiary").Value);
}
