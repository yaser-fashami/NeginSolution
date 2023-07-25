using Negin.Core.Domain.Interfaces;
using System.Reflection;

namespace Negin.Services.ApplicationServices;

public class AppVersionService : IAppVersionService
{
	public string Version => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
}
